using Nct.Domain.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Nct.Module.Flow.Flows
{
    [Table("Flow_TaskItem")]
    public class TaskItem:IEntity,IHasCreateTime
    {
       
		[Key]
		public int Id { get; set; }

       
		public string NodeId { get; set; }

       
		public string NodeName { get; set; }

       
		public int TaskId { get; set; }

        public int FlowId { get; set; }

       
		public int UserId { get; set; }

       
		public string UserName { get; set; }

       
		public int Status { get; set; }

       
		public string Remark { get; set; }

        public DateTime? AuditTime { get; set; }

        public DateTime CreateTime { get; set; }



    }
}
