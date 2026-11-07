using Nct.DataAccess.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nct.DataAccess
{
   public interface IDbContextProvider
    {

        TDbContext GetDbContext<TDbContext>()
           where TDbContext : DbContext;

    }
}
