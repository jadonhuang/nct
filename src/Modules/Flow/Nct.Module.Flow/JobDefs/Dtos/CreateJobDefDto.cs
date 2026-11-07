using System;
using System.ComponentModel.DataAnnotations;
using Nct.ObjectMapper;
using Nct.Application;

namespace Nct.Module.Flow.JobDefs.Dtos
{    
  
    [Map(typeof(JobDef))]
    public class CreateJobDefDto:IEntityDto
    {
  
		[Display(Name ="Id")]
		public int Id { get; set; }

		[Required]
		[StringLength(50)]  
		[Display(Name ="名称")]
		public string Name { get; set; }


		[Display(Name ="运行间隔")]
		public int? IntervalMinute { get; set; }

        public bool IsEnabled { get; set; }


        [StringLength(50)]  
		[Display(Name ="Cron表达式")]
		public string IntervalCron { get; set; }

  
		[Display(Name ="执行方式")]
		public int IntervalType { get; set; }

		[Range(1,2)]
        public int JobType { get; set; }
        public bool LastExeStatus { get; set; }
        public string ExeMethod { get; set; }



    }
}
