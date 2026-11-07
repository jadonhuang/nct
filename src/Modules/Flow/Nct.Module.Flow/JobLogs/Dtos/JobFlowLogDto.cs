using System;
using System.Collections.Generic;
using Nct.ObjectMapper;
using Nct.Application;

namespace Nct.Module.Flow.JobLogs.Dtos
{

    [Map(typeof(JobLog))]
    public class JobFlowLogDto : IEntityDto
    {

        public int Id { get; set; }


        public bool IsSuccess { get; set; }

        public DateTime CreateTime { get; set; }


        public string JobName { get; set; }


        public int ElapsedTime { get; set; }


        public int LogType { get; set; }


    }
}
