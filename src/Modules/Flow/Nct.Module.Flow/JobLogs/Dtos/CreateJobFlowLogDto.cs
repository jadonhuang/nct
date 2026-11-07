using Nct.ObjectMapper;
using System.ComponentModel.DataAnnotations;

namespace Nct.Module.Flow.JobLogs.Dtos
{

    [Map(typeof(JobLog))]
    public class CreateJobFlowLogDto : IEntityDto
    {
  
		public int Id { get; set; }

  
		[Display(Name ="是否成功")]
		public bool IsSuccess { get; set; }

		[Required]  
		[Display(Name ="消息")]
		public string Message { get; set; }

  
		[Display(Name ="执行时间")]
		public DateTime CreateTime { get; set; }

  
		[Display(Name = "JobName")]
		public string JobName { get; set; }

  
		[Display(Name ="ElapsedTime")]
		public int ElapsedTime { get; set; }

		public int LogType { get; set; }


    }
}
