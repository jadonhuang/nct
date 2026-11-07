using Castle.Core;
using Castle.MicroKernel.Context;
using Castle.MicroKernel.Handlers;
using Nct.DataAccess.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using Nct.Ioc;
using Nct.DataAccess.Core;

namespace Nct.DataAccess
{
    public class RepositoryMatchingStrategy : IGenericImplementationMatchingStrategy
    {
        private DbConfiguration _dbConfiguration;
        public RepositoryMatchingStrategy(DbConfiguration dbConfiguration)
        {
            _dbConfiguration = dbConfiguration;
        }

        public Type[] GetGenericArguments(ComponentModel model, CreationContext context)
        {

            var contextType = _dbConfiguration.DbContextType;

            if (context.GenericArguments.Length == 1)
                return new[] { contextType, context.GenericArguments[0] };

            return new[] { contextType, context.GenericArguments[0], context.GenericArguments[1] };


        }
    }
}
