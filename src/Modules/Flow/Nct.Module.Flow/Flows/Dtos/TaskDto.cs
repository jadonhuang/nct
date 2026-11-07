using Nct.Domain.Entities;
using Nct.ObjectMapper;
using System.ComponentModel.DataAnnotations;

namespace Nct.Module.Flow.Flows.Dtos
{

    [Map(typeof(FlowTask))]
    public class TaskDto:IEntity,IHasCreateTime
    {
       
		[Key]
		public int Id { get; set; }

       
		public DateTime CreateTime { get; set; }

       
		public string NodeId { get; set; }

       
		public string NodeName { get; set; }

       
		public int FlowId { get; set; }

       
		public int Status { get; set; }


        public List<TaskItemDto> Items { get; set; }


    }
}
