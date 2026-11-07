namespace Nct.Module.Flow.Engine.Models
{

    public class JsonData
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string DataType { get; set; }


    }
    public class JobJsonDataNodeProps : NodeProps
    {

        public string Input { get; set; }

        public List<JsonData> DataItems { get; set; } = [];


        public override List<Variable> GetOutputVariables()
        {
            return DataItems.Select(x => new Variable() { Name = x.Name, Type = x.DataType }).ToList();
        }


    }
}
