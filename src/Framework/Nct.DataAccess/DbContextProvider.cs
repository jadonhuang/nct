using Nct.Domain.Uow;
using Nct.Ioc;
using Nct.DataAccess.Configuration;
using Nct.DataAccess.Core;
using Nct.DataAccess.Uow;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nct.DataAccess
{
    public class DbContextProvider : IDbContextProvider, ITransientDependency
    {
      
        private readonly IUnitOfWorkManager _unitOfWorkManager;

       
        public DbContextProvider(IUnitOfWorkManager unitOfWorkManager)
        {

            _unitOfWorkManager = unitOfWorkManager;
        }


        public TDbContext GetDbContext<TDbContext>() where TDbContext : DbContext
        {

            var currentUow = _unitOfWorkManager.Current;

            ArgumentNullException.ThrowIfNull(currentUow, nameof(currentUow));

           return (currentUow as DapperUnitOfWork).GetDbContext<TDbContext>();


        }
    }
}
