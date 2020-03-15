using System;
using System.Linq;
using System.Text;


namespace Core.PersistentStore.Repositories
{
    public interface IRepository<out TEntity> : IRepository<TEntity, int> where TEntity : class, IEntity
    {
    }
    public interface IRepository<out TEntity, TKey> where TEntity : class, IEntity<TKey>
    {
        //TODO: Add sync methods.

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        T Query<T>(Func<IQueryable<TEntity>, T> predicate);

        IQueryable<TEntity> GetAll();
    }
}
