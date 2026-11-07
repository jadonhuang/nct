using Nct.DataAccess.Core;
using Nct.Domain.Entities;
using Nct.Domain.Repositories;
using Nct.LinqQuery;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Nct.DataAccess
{


    public static class RepositoryExtensions
    {

        public static Database GetDatabase<TEntity>(this IRepository<TEntity> repository) where TEntity : class, IEntity
        {
            dynamic rep = repository;
            return rep.Context.Database;

        }


    }


    public class Repository<T> : Repository<DapperDbContext, T>, IRepository<T> where T : class, IEntity
    {
        public Repository(IDbContextProvider dbContextProvider) : base(dbContextProvider)
        {
        }
    }

    public class Repository<TDbContext, T> : Repository<TDbContext, T, int>, IRepository<T> where T : class, IEntity where TDbContext : DbContext
    {
        public Repository(IDbContextProvider dbContextProvider) : base(dbContextProvider)
        {
        }
    }


    public class Repository<TDbContext, T, TPrimaryKey> : IRepository<T, TPrimaryKey> where T : class, IEntity<TPrimaryKey> where TDbContext : DbContext
    {

        private readonly IDbContextProvider _dbContextProvider;

        public virtual TDbContext Context => _dbContextProvider.GetDbContext<TDbContext>();


        public Repository(IDbContextProvider dbContextProvider)
        {
            _dbContextProvider = dbContextProvider;

        }


        public IQueryable<T> GetAll()
        {
            return Context.Table<T>();
        }


        public int Count()
        {
            return GetAll().Count();
        }

        public int Count(Expression<Func<T, bool>> predicate)
        {
            return GetAll().Where(predicate).Count();
        }

        public Task<int> CountAsync()
        {
            return GetAll().CountAsync();
        }

        public Task<int> CountAsync(Expression<Func<T, bool>> predicate)
        {
            return GetAll().Where(predicate).CountAsync();
        }


        public long LongCount()
        {
            return GetAll().LongCount();
        }

        public long LongCount(Expression<Func<T, bool>> predicate)
        {
            return GetAll().Where(predicate).LongCount();
        }

        public Task<long> LongCountAsync()
        {
            return GetAll().LongCountAsync();
        }

        public Task<long> LongCountAsync(Expression<Func<T, bool>> predicate)
        {
            return GetAll().Where(predicate).LongCountAsync();
        }

        public int Delete(T entity)
        {
            return GetAll().Delete(entity);
        }

        public Task<int> DeleteAsync(T entity)
        {
            return GetAll().DeleteAsync(entity);
        }

        public int Delete(Expression<Func<T, bool>> predicate)
        {
            return GetAll().Where(predicate).Delete();
        }


        public Task<int> DeleteAsync(Expression<Func<T, bool>> predicate)
        {

            return GetAll().Where(predicate).DeleteAsync();

        }

        public T FirstOrDefault(Expression<Func<T, bool>> predicate)
        {
            return GetAll().Where(predicate).FirstOrDefault();
        }

        public Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
        {
            return GetAll().Where(predicate).FirstOrDefaultAsync();
        }

        public T Get(TPrimaryKey id)
        {
            return GetAll().Where(m => m.Id.Equals(id)).FirstOrDefault();
        }

        public Task<T> GetAsync(TPrimaryKey id)
        {
            return GetAll().Where(m => m.Id.Equals(id)).FirstOrDefaultAsync();
        }


        public List<T> GetAllList()
        {
            return GetAll().ToList();
        }

        public List<T> GetAllList(Expression<Func<T, bool>> predicate)
        {
            return GetAll().Where(predicate).ToList();
        }

        public Task<List<T>> GetAllListAsync()
        {

            return GetAll().ToListAsync();
        }

        public Task<List<T>> GetAllListAsync(Expression<Func<T, bool>> predicate)
        {
            return GetAll().Where(predicate).ToListAsync();
        }



        public int Insert(T entity)
        {
            return GetAll().Insert(entity);
        }

        public Task<int> InsertAsync(T entity)
        {
            return GetAll().InsertAsync(entity);
        }



        public int Update(T entity)
        {
            return GetAll().Update(entity);
        }

        public Task<int> UpdateAsync(T entity)
        {
            return GetAll().UpdateAsync(entity);
        }

        public int Insert<T>(IEnumerable<T> list, int? commandTimeout = null)
        {
            return Context.Database.Insert(list, commandTimeout: commandTimeout);
        }



    }
}
