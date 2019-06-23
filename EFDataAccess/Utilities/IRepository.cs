using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EFDataAccess.Repository
{
    public interface IRepository<T> where T : class
    {
        IQueryable<T> GetAll();
        T GetById(int id);
        void Add(T entity);
        void Update(T entity);
        void Delete(T entity);
        void Delete(int id);
        void FullReload(T entity);
        void Reload(T entity);
        T GetRelatedEntity(T entity,String child);
        void loadReference(T entity,String reference);
        IEnumerable<T> GetAllByFilter(Func<T, Boolean> predicate, String[] includes);
        T GetFirstByFilter(Func<T, Boolean> predicate, String[] includes = null);
        Boolean CheckIfExist(Func<T, Boolean> predicate,String[] includes = null);
        IEnumerable<T> Get(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = "");
        IEnumerable<T> Get(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            params Expression<Func<T, object>>[] includes);
        void Detach(T entity);
        DbEntityEntry<T> GetDbEntry(T entity);
    }
}
