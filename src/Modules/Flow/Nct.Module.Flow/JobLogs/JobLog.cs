using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Nct.Domain.Entities;
using Nct.Application;

namespace Nct.Module.Flow.JobLogs
{    
    [Table("Job_JobLog")]
    public class JobLog:IEntity,IHasCreateTime
    
    {
       
		[Key]
		public int Id { get; set; }

       
		public bool IsSuccess { get; set; }

       
		public string Message { get; set; }

       
		public DateTime CreateTime { get; set; }

       
		public string JobName { get; set; }

       
		public int ElapsedTime { get; set; }


        public int LogType { get; set; }


    }
}
