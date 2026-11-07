using System;
using System.ComponentModel.DataAnnotations;

namespace Nct.Module.Core.Messages.Dtos
{

    [Map(typeof(Message))]
    public class CreateMessageDto:IEntityDto
    {
  
		public int Id { get; set; }

		[Required]
		[StringLength(200)]  
		[Display(Name ="标题")]
		public string Title { get; set; }


		[StringLength(2000)]  
		[Display(Name ="内容")]
		public string Content { get; set; }

  
		[Display(Name ="接收人")]
		public int UserId { get; set; }


  
		[Display(Name ="状态")]
		public int Status { get; set; }

  
		[Display(Name ="消息类型")]
		public int Type { get; set; }


		[StringLength(4000)]  
		[Display(Name ="属性配置")]
		public string Props { get; set; }
  
    }
}
