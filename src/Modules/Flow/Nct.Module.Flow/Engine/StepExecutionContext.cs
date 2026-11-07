using Nct.Domain.Uow;
using Nct.Module.Flow.Engine.Models;

namespace Nct.Module.Flow.Engine
{
    public abstract class StepExecutionContext
    {

        private readonly static AsyncLocal<FlowNode> AsyncNode = new AsyncLocal<FlowNode>();

        private readonly static AsyncLocal<FlowNode> AsyncPreNode = new AsyncLocal<FlowNode>();

        public FlowNode Node
        {
            get
            {
                return AsyncNode.Value;
            }
            set
            {
                AsyncNode.Value = value;

            }
        }

        public FlowNode PreNode
        {
            get
            {
                return AsyncPreNode.Value;
            }
            set
            {
                AsyncPreNode.Value = value;
            }

        }

        public virtual FlowConfig FlowConfig { get; set; }


        public Action<NodeStatusChangeArgs> NodeStatusChanged { get; set; }


        public Dictionary<string, Dictionary<string, object>> Outputs { get; set; } = [];

        public virtual bool GetOutputValue(string nodeId, string key, out object value)
        {
            var success = Outputs.TryGetValue(nodeId, out Dictionary<string, object> dic);
            if (success)
            {
                success = dic.TryGetValue(key, out value);
                if (success)
                {
                    return true;
                }
            }

            value = null;
            return false;
        }


        public virtual void SetOutputValue(string nodeId, string key, object value)
        {
            if (!Outputs.TryGetValue(nodeId, out Dictionary<string, object> dic))
            {
                dic = [];
                Outputs.Add(nodeId, dic);
            }
            dic[key] = value;
        }

        public virtual string GetStepNamespace()
        {
            return GetType().Namespace + ".Steps";

        }



    }
}
