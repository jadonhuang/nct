using Nct.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Nct.Module.Flow.Flows
{
    [Table("Flow_Task")]
    public class FlowTask:IEntity,IHasCreateTime
    {
       
		[Key]
		public int Id { get; set; }

       
		public DateTime CreateTime { get; set; }

       
		public string NodeId { get; set; }

       
		public string NodeName { get; set; }

       
		public int FlowId { get; set; }

       
		public int Status { get; set; }

              
		public int TimeoutNotice { get; set; }


        public DateTime? LastNotifyTime { get; set; }





    }
}
