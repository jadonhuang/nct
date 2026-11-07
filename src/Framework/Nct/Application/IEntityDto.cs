using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.Application
{
    public interface IEntityDto : IEntityDto<int>
    {

    }

    public interface IEntityDto<TPrimaryKey>
    {
       
        TPrimaryKey Id { get; set; }
    }
}
