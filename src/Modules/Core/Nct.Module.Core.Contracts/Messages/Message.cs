using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Nct.Domain.Entities;
using Nct.Application;

namespace Nct.Module.Core.Messages
{    
    [Table("Core_Message")]
    public class Message:IEntity,IHasCreateTime,IHasCreatorId
    {
       
		[Key]
		public int Id { get; set; }

       
		public string Title { get; set; }

       
		public string Content { get; set; }

       
		public int UserId { get; set; }

       
		public int CreatorId { get; set; }

       
		public DateTime CreateTime { get; set; }

       
		public int Status { get; set; }

       
		public int Type { get; set; }

       
		public string Props { get; set; }


    }
}
