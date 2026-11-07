using Nct.Module.Flow.Engine.Models;
using Nct.Module.Flow.Helpers;
using System.Collections;
using System.Diagnostics;
using static Nct.Module.Flow.Consts;

namespace Nct.Module.Flow.Engine.Steps.Job
{
    public class IterationStep : Step
    {
        public override async Task<IStepExecutionResult> Run(StepExecutionContext context)
        {
            var ctx = (FlowJobStepExecutionContext)context;
            var props = ctx.Node.Properties.To<JobIterationNodeProps>();
            var children = props.Children;
            var nodes = ctx.FlowConfig.Nodes;
            var startNode = nodes.Where(m => children.Contains(m.Id) && m.Type == NodeType.GroupStart).FirstOrDefault();
            if (startNode == null)
                return Fail("找不到开始节点");

            startNode.Text = new FlowText { Value = "迭代开始" };
            startNode.IsSlient = true;

            var inputArgTag = FlowHelper.GetVariableTags(props.Input).First();
            var inputResultValue = FlowHelper.GetVariableValueByTag(inputArgTag, ctx);
            if (!inputResultValue.IsSuccess)
                return Fail(inputResultValue.Msg);

            var inputValue =inputResultValue.Data;
            if (!inputValue.Type.StartsWith("Array"))
                return Fail("输入参数必须是数组");

            var value = inputValue.Value;
            var node = ctx.Node;

            List<object> output = [];

            StepExecutionContext NewSubCtx(object item, int index)
            {
                var subCtx = new FlowJobStepExecutionContext
                {
                    Node = startNode,
                    FlowConfig = ctx.FlowConfig,
                    Outputs = ctx.Outputs,
                    JobId = ctx.JobId,
                    NodeStatusChanged = ctx.NodeStatusChanged
                };
                subCtx.SetOutputValue(node.Id, "item", item);
                subCtx.SetOutputValue(node.Id, "index", index);
                return subCtx;
            }

            string error = null;
            if (value is IList list)
            {
                var index = 0;
                var loopNum = list.Count;
                var nodeStatusChangeArgs = new NodeStatusChangeArgs(node.Id, NodeStatus.Running);
                var stopWhenErr = props.ErrHandleMode == "stop";
                var result = new StepExecutionResult();
                var hasOutput = props.Output.IsNotEmpty();

                VariableTag outputArgTag = null;

                if (hasOutput)
                {
                    outputArgTag = FlowHelper.GetVariableTags(props.Output).First();
                }

                foreach (var item in list)
                {
                    nodeStatusChangeArgs.Data =$"{index + 1}/{loopNum}";
                    ctx.NodeStatusChanged?.Invoke(nodeStatusChangeArgs);

                    var subStepResult = new StepExecutionResult
                    {
                        StepName = $"迭代{index + 1}"
                    };
                    result.SubResults.Add(subStepResult);
                    var subCtx = NewSubCtx(item, index);
                    var success = false;
                    var stopWatch = Stopwatch.StartNew();
                    try
                    {

                        using var flowEngine = IocManager.Instance.ResolveAsDisposable<IFlowEngine>();
                        success = await flowEngine.Object.Run(subCtx);
                        subStepResult.SubResults.AddRange(flowEngine.Object.Results.Skip(1));
                        subStepResult.SubResults.AddRange(flowEngine.Object.Results[0].SubResults);
                        if (!success)
                        {
                            error += $"第{index + 1}次迭代没有成功";
                        }
                        else
                        {
                            if (hasOutput)
                            {
                                var outResult = FlowHelper.GetVariableValueByTag(outputArgTag, subCtx);
                                if (!outResult.IsSuccess)
                                {
                                    return Fail($"返回参数 {outputArgTag.Name} 值未找到");
                                }
                                else
                                {
                                    output.Add(outResult.Data.Value);
                                }

                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        error += $"第{index + 1}次迭代：{ex.Message}";
                        subStepResult.Exception = ex.Message;
                    }
                    index++;
                    subStepResult.Duration = (int)stopWatch.ElapsedMilliseconds;
                    subStepResult.Success = success;
                  
                    if (!success && stopWhenErr)
                    {
                        break;
                    }

                }

                if (error != null && stopWhenErr)
                {
                    result.Success = false;
                    return result;
                }
                if (hasOutput)
                    result.Outputs[outputArgTag.Name] = output;
                return result;

            }

            return Fail("输入参数必须是可迭代类型");

        }
    }
}
