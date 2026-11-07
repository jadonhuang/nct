using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nct.Application;
using Nct.Module.Core.Messages.Dtos;

namespace Nct.Module.Core.Messages
{    

    public interface IMessageAppService: ICrudAppService<Message,MessageDto, QueryDto,CreateMessageDto>
    {


    }
}
