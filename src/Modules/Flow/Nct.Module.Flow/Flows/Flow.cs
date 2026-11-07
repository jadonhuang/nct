using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Nct.Domain.Entities;
using Nct.Application;

namespace Nct.Module.Flow.Flows
{    
    [Table("Flow_Flow")]
    public class Flow:IEntity,IHasCreateTime,IHasCreatorId
    {
       
		[Key]
		public int Id { get; set; }

       
		public DateTime CreateTime { get; set; }

        public DateTime? EndTime { get; set; }

        public bool IsEnd { get; set; }

        public int CreatorId { get; set; }

       
		public string CreatorName { get; set; }

     
		public string FlowCode { get; set; }
      
		public string Title { get; set; }
      
		public int Status { get; set; }

        public  string FormDataId { get; set; }



    }
}
