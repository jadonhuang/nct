using Nct.Domain.Entities;
using Nct.Domain.Uow;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Nct.Domain.Repositories
{

    public interface IRepository<TEntity> : IRepository<TEntity, int> where TEntity : class, IEntity
    {

    }

    public interface IRepository<TEntity, TPrimaryKey>: IUnitOfWorkEnabled where TEntity : class, IEntity<TPrimaryKey>
    {

        IQueryable<TEntity> GetAll();


        TEntity Get(TPrimaryKey id);


        Task<TEntity> GetAsync(TPrimaryKey id);


        List<TEntity> GetAllList();


        Task<List<TEntity>> GetAllListAsync();


        List<TEntity> GetAllList(Expression<Func<TEntity, bool>> predicate);


        Task<List<TEntity>> GetAllListAsync(Expression<Func<TEntity, bool>> predicate);



        TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate);


        Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate);

        int Insert(TEntity entity);

        int Insert<T>(IEnumerable<T> list,int? commandTimeout = null);


        Task<int> InsertAsync(TEntity entity);


        int Update(TEntity entity);


        Task<int> UpdateAsync(TEntity entity);


        

        int Delete(TEntity entity);


        Task<int> DeleteAsync(TEntity entity);



        int Delete(Expression<Func<TEntity, bool>> predicate);


        Task<int> DeleteAsync(Expression<Func<TEntity, bool>> predicate);


        int Count();


        Task<int> CountAsync();

        int Count(Expression<Func<TEntity, bool>> predicate);


        Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate);


        long LongCount();


        Task<long> LongCountAsync();


        long LongCount(Expression<Func<TEntity, bool>> predicate);


        Task<long> LongCountAsync(Expression<Func<TEntity, bool>> predicate);

      



    }
}
