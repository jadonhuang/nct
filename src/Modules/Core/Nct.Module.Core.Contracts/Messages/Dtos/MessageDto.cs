using System;

namespace Nct.Module.Core.Messages.Dtos
{

    [Map(typeof(Message))]
    public class MessageDto:IEntityDto
    {
       
	public int Id { get; set; }

       
	public string Title { get; set; }

       
	public string Content { get; set; }

       
	public int UserId { get; set; }

       
	public int? CreatorId { get; set; }

       
	public DateTime CreateTime { get; set; }

       
	public int Status { get; set; }

       
	public int Type { get; set; }

       
	public string Props { get; set; }


    }
}
