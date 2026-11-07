using Nct.Module.Flow.Engine.Models;
using Nct.Module.Flow.Engine.Steps;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace Nct.Module.Flow.Engine
{
    public class FlowEngine : IFlowEngine
    {

        private readonly Dictionary<FlowNode, IStep> _stepInstances;
        private readonly IIocResolver _resolver;
        private readonly ConcurrentDictionary<string, TaskCompletionSource<bool>> _nodeCompletionSources = [];
        private readonly ConcurrentDictionary<string, bool> _executedNodes = [];

        private readonly Dictionary<string, IStepExecutionResult> _stepResultMap = [];
        private readonly Dictionary<string, bool> _nodeParallelMap = [];
        private readonly List<IStepExecutionResult> _stepResults = [];

        public FlowEngine(IIocResolver resolver)
        {
            _resolver = resolver;
            _stepInstances = [];
        }

        public virtual void Dispose()
        {
            foreach (var instance in _stepInstances.Values)
            {
                _resolver.Release(instance);
            }
            _stepInstances.Clear();

        }

        public List<IStepExecutionResult> Results => _stepResults;

        protected virtual IStep GetStepInstance(FlowNode node, StepExecutionContext context)
        {
            if (_stepInstances.TryGetValue(node, out IStep instance))
            {
                return instance;
            }

            var stepNamespace = context.GetStepNamespace();
            var nodeType = node.Type;
            string stepTypeName = stepNamespace + "." + nodeType[0].ToString().ToUpper() + nodeType[1..] + "Step";
            var type = Type.GetType(stepTypeName);
            instance = _resolver.Resolve<IStep>(type);
            _stepInstances[node] = instance;
            return instance;

        } 

        private void SetNodeCompletionSources(FlowNode node)
        {
            _nodeCompletionSources[node.Id] = new TaskCompletionSource<bool>();
        }
        private static void LinkToPreNodes(StepExecutionContext ctx, FlowNode node)
        {
            var allNextNodes = ctx.FlowConfig.GetNextNodes(node);
            node.LinkToPreNodes(allNextNodes);
        }
        public async Task<bool> Run(StepExecutionContext ctx)
        {
            SetNodeCompletionSources(ctx.Node);
            LinkToPreNodes(ctx, ctx.Node);
            return await ExecuteNode(ctx);

        }

        private async Task<bool> ExecuteNode(StepExecutionContext ctx)
        {
            var node = ctx.Node;
            if (!_executedNodes.TryAdd(node.Id, true))
                return true;


            if (node.PreNodes.Count > 0)
            {
                var predecessorTasks = node.PreNodes
                  .Select(p => _nodeCompletionSources[p.Id].Task)
                  .ToArray();

                var results = await Task.WhenAll(predecessorTasks);

                if (!results.All(m => m))
                {
                    _nodeCompletionSources[node.Id].SetResult(false);
                    return false;
                }

            }

            var nodeStatusChangeArgs = new NodeStatusChangeArgs(node.Id, NodeStatus.Running);
            if (!node.IsSlient)
                ctx.NodeStatusChanged?.Invoke(nodeStatusChangeArgs);

            var step = GetStepInstance(node, ctx);
            FlowNode[] nextNodes = null;

            IStepExecutionResult result = new StepExecutionResult();
            var stopwatch = Stopwatch.StartNew();
            try
            {
                result = await step.Run(ctx);
                if (!result.Success)
                {
                    nodeStatusChangeArgs.Status = NodeStatus.Failed;
                }
                else
                {
                    nodeStatusChangeArgs.Status = NodeStatus.Completed;
                    nextNodes = result.NextNodes;
                    if (result.Outputs != null)
                    {
                        foreach (var kv in result.Outputs)
                        {
                            ctx.SetOutputValue(node.Id, kv.Key, kv.Value);
                        }
                    }

                }
                _nodeCompletionSources[node.Id].SetResult(result.Success);

            }
            catch (Exception ex)
            {
                nodeStatusChangeArgs.Status = NodeStatus.Failed;
                _nodeCompletionSources[node.Id].SetResult(false);
                result.Exception = ex.Message;
                result.Success = false;
            }

            result.Duration = (int)stopwatch.ElapsedMilliseconds;
            result.StepName ??= node.Text.Value;
            _stepResultMap[node.Id] = result;


            var hasParallel = _nodeParallelMap.TryGetValue(node.Id, out bool isParallel);
            if (hasParallel && isParallel)
            {
                _stepResultMap[ctx.PreNode.Id].SubResults.Add(result);
            }
            else
            {
                _stepResults.Add(result);
            }

            if (!node.IsSlient)
                ctx.NodeStatusChanged?.Invoke(nodeStatusChangeArgs);

            if (result.Success && nextNodes != null && nextNodes.Length > 0)
            {
                var tasks = new List<Task<bool>>();
                var isParalleled = nextNodes.Length > 1;
                foreach (var nextNode in nextNodes)
                {
                    LinkToPreNodes(ctx, nextNode);
                    SetNodeCompletionSources(nextNode);
                    _nodeParallelMap[nextNode.Id] = isParalleled;
                    var task = Task.Run(async () =>
                    {
                        ctx.Node = nextNode;
                        ctx.PreNode = node;
                        return await ExecuteNode(ctx);
                    });

                    tasks.Add(task);
                }
                var results = await Task.WhenAll(tasks);
                return results.All(m => m);

            }
            return result.Success;

        }





    }
}
