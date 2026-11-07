using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nct.Domain.Entities
{
   public interface IHasCreateTime
    {
        DateTime CreateTime { get; set; }

    }

    public interface IHasUpdateTime
    {
        DateTime UpdateTime { get; set; }

    }

    public interface IHasCreateUser
    {
        string CreateUser { get; set; }

    }

    public interface IHasCreatorId
    {
        int CreatorId { get; set; }

    }


    public interface IHasUpdateUser
    {
        string UpdateUser { get; set; }

    }

}
