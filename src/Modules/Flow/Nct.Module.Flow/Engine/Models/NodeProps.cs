
using Nct.Module.Flow.Helpers;
using System.Text.Json.Serialization;
namespace Nct.Module.Flow.Engine.Models
{
    [JsonConverter(typeof(NodePropsJsonConverter))]
    public abstract class NodeProps
    {
        public virtual T To<T>() where T : NodeProps
        {
            return (T)this;
        }

        public virtual List<Variable> GetOutputVariables()
        {
            return [];

        }



    }

    public  class NullNodeProps: NodeProps
    {
   
    }

}
