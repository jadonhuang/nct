using System;
using System.Collections.Generic;
using Nct.ObjectMapper;
using Nct.Application;

namespace Nct.Module.Flow.JobDefs.Dtos
{

    [Map(typeof(JobDef))]
    public class JobDefDto : IEntityDto
    {

        public int Id { get; set; }


        public string Name { get; set; }


        public DateTime? LastTime { get; set; }


        public int? IntervalMinute { get; set; }


        public string IntervalCron { get; set; }


        public int IntervalType { get; set; }


        public bool IsEnabled { get; set; }

        public int JobType { get; set; }

        public bool? LastExeSuccess { get; set; }

        public string ExeMethod { get; set; }

        public DateTime CreateTime { get; set; }



    }
}
