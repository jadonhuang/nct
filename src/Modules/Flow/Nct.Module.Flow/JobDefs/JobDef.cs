using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Nct.Domain.Entities;
using Nct.Application;

namespace Nct.Module.Flow.JobDefs
{    
    [Table("Job_JobDef")]
    public class JobDef:IEntity,IHasCreateTime, ISoftDelete
    {
       
		[Key]
		public int Id { get; set; }
    
		public string Name { get; set; }

		public DateTime? LastTime { get; set; }
 
		public int? IntervalMinute { get; set; }
    
		public string IntervalCron { get; set; }
      
		public int IntervalType { get; set; }

		public bool IsEnabled { get; set; }
    
		public bool IsDeleted { get; set; }
     
		public DateTime CreateTime { get; set; }
     
		public string FlowConfig { get; set; }

        public int JobType { get; set; }

        public bool? LastExeSuccess { get; set; }

        public string ExeMethod { get; set; }


    }
}
