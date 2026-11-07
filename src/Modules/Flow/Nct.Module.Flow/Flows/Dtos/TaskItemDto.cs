using Nct.ObjectMapper;

namespace Nct.Module.Flow.Flows.Dtos
{

    [Map(typeof(TaskItem))]
    public class TaskItemDto : IEntityDto
    {

        public int Id { get; set; }


        public string NodeId { get; set; }


        public string NodeName { get; set; }


        public int TaskId { get; set; }

        public int FlowId { get; set; }


        public int CreatorId { get; set; }


        public string Applicant { get; set; }


        public int UserId { get; set; }


        public string UserName { get; set; }


        public int Status { get; set; }


        public string Remark { get; set; }


        public DateTime? AuditTime { get; set; }

        public DateTime CreateTime { get; set; }


    }
}
