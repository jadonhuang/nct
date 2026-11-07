using Nct.ObjectMapper;

namespace Nct.Module.Flow.Flows.Dtos
{

    [Map(typeof(Flow))]
    public class FlowDto : IEntityDto
    {

        public int Id { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime? EndTime { get; set; }

        public int CreatorId { get; set; }

        public string CreatorName { get; set; }

        public int FlowDefId { get; set; }

        public string Title { get; set; }

        public int Status { get; set; }

        public string FormDataId { get; set; }

        public bool IsEnd { get; set; }




    }
}
