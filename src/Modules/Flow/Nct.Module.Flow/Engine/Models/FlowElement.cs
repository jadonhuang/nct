namespace Nct.Module.Flow.Engine.Models
{
    public class FlowElement
    {
        public string Id { get; set; }

        public FlowText Text { get; set; }

        public string Type { get; set; }

        public  NodeProps Properties { get; set; }

    }
}
