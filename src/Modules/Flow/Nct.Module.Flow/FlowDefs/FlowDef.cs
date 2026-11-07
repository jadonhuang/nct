using Nct.Domain.Entities;
using Nct.Module.Flow.Engine;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nct.Module.Flow.FlowDefs
{
    [Table("Flow_FlowDef")]
    public class FlowDef:IEntity,IHasCreateUser,IHasCreateTime,ISoftDelete
    {
       
		[Key]
		public int Id { get; set; }

       
		public string Name { get; set; }

       
		public string Code { get; set; }

       
		public string Descpt { get; set; }

       
		public string Icon { get; set; }

       
		public string AdminIds { get; set; }

       
		public string Applicant { get; set; }

       
		public bool IsDeleted { get; set; }

       
		public DateTime CreateTime { get; set; }

       
		public int Cate { get; set; }

       
		public string Data { get; set; }

       
		public string CreateUser { get; set; }

      
        public FlowConfig GetConfig()
        {
             return Data.ToObject<FlowConfig>();    
        }


    }
}
