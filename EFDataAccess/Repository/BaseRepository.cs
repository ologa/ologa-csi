using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Metadata.Edm;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace EFDataAccess.Repository
{
    /// <summary>
    /// The EF-dependent, generic repository for data access
    /// </summary>
    /// <typeparam name="T">Type of entity for this Repository.</typeparam>
    public class BaseRepository<T> : IRepository<T> where T : class
    {
         public BaseRepository(DbContext dbContext)
        {
            if (dbContext == null) 
                throw new ArgumentNullException("Null DbContext");
            DbContext = dbContext;
            DbSet = DbContext.Set<T>();
        }

        protected DbContext DbContext { get; set; }

        protected DbSet<T> DbSet { get; set; }

        public virtual IQueryable<T> GetAll()
        {
            return DbSet;
        }

        public virtual T GetById(int id)
        {
           
            return DbSet.Find(id);
        }

        public virtual void Add(T entity)
        {
            DbEntityEntry dbEntityEntry = DbContext.Entry(entity);
            if (dbEntityEntry.State != EntityState.Detached)
            {
                dbEntityEntry.State = EntityState.Added;
            }
            else
            {
                DbSet.Add(entity);
            }
        }

        public virtual void Update(T entity)
        {
            DbEntityEntry dbEntityEntry = DbContext.Entry(entity);
            if (dbEntityEntry.State == EntityState.Detached)
            {
                DbSet.Attach(entity);
            }  
            dbEntityEntry.State = EntityState.Modified;
        }

        public virtual void Delete(T entity)
        {
            DbEntityEntry dbEntityEntry = DbContext.Entry(entity);
            if (dbEntityEntry.State != EntityState.Deleted)
            {
                dbEntityEntry.State = EntityState.Deleted;
            }
            else
            {
                DbSet.Attach(entity);
                DbSet.Remove(entity);
            }
        }

        public virtual void Delete(int id)
        {
            var entity = GetById(id);
            if (entity == null) return; // not found; assume already deleted.
            Delete(entity);
        }

        public virtual T GetRelatedEntity(T entity,String child)
        {
             DbContext.Entry(entity).Collection(child).Load();
             return entity;
        }

        public virtual void loadReference(T entity, String reference)
        {
            DbContext.Entry(entity).Reference(reference).Load();
        }

        public virtual void Reload(T entity)
        {
            DbContext.Entry(entity).Reload();
        }

        public virtual DbEntityEntry<T> GetDbEntry(T entity)
        {
            return DbContext.Entry(entity);
        }

        /// <summary>
        /// Full reload from database entity change
        /// Large objects can cause perfomance issues
        /// </summary>
        /// <param name="entity"></param>
        public virtual void FullReload(T entity)
        {
            Visit(entity, e => DbContext.Entry(e).Reload());
        }

        public virtual IEnumerable<T> GetAllByFilter(Func<T, Boolean> predicate,String[] includes = null)
        {
            
            if (includes !=null && includes.Count()> 0)
            {
                var query = DbSet.Include(includes.First());
                foreach (var include in includes.Skip(1))
                    query.Include(include);
                return query.Where<T>(predicate);
            }

            return DbSet.Where<T>(predicate);
        }

        public virtual T GetFirstByFilter(Func<T, Boolean> predicate, String[] includes = null)
        {

            if (includes != null && includes.Count() > 0)
            {
                var query = DbSet.Include(includes.First());
                foreach (var include in includes.Skip(1))
                    query.Include(include);
                return query.Where<T>(predicate).FirstOrDefault<T>();
            }

            return DbSet.Where<T>(predicate).FirstOrDefault<T>();
        }

        public Boolean CheckIfExist(Func<T, Boolean> predicate, String[] includes = null)
        {
            if (includes != null && includes.Count() > 0)
            {
                var query = DbSet.Include(includes.First());
                foreach (var include in includes.Skip(1))
                    query.Include(include);
                return query.Any<T>(predicate);
            }
            return DbSet.Any<T>(predicate);
        }

        public virtual IEnumerable<T> Get(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = "")
        {
            IQueryable<T> query = DbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            else
            {
                return query.ToList();
            }
        }

        public virtual IEnumerable<T> Get(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = DbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includes)
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return orderBy(query).ToList();
            }
            else
            {
                return query.ToList();
            }
        }

        public virtual void Visit(object entity, Action<object> action)
        {
            Action<object, DbContext, HashSet<object>, Action<object>> visitFunction = null; // Initialize first to enable recursive call.
            visitFunction = (ent, contxt, hashset, act) =>
            {
                if (ent != null && !hashset.Contains(ent))
                {
                    hashset.Add(ent);
                    act(ent);
                    var entry = contxt.Entry(ent);
                    if (entry != null)
                    {
                        foreach (var np in GetNavigationProperies(contxt, ent.GetType()))
                        {
                            if (np.ToEndMember.RelationshipMultiplicity < RelationshipMultiplicity.Many)
                            {
                                var reference = entry.Reference(np.Name);
                                if (reference.IsLoaded)
                                {
                                    visitFunction(reference.CurrentValue, contxt, hashset, action);
                                }
                            }
                            else
                            {
                                var collection = entry.Collection(np.Name);
                                if (collection.IsLoaded)
                                {
                                    var sequence = collection.CurrentValue as IEnumerable;
                                    if (sequence != null)
                                    {
                                        foreach (var child in sequence)
                                        {
                                            visitFunction(child, contxt, hashset, action);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };
            visitFunction(entity, DbContext, new HashSet<object>(), action);
        }

        // Get navigation properties of an entity type.
        public virtual IEnumerable<NavigationProperty> GetNavigationProperies(DbContext context, Type type)
        {
            var oc = ((IObjectContextAdapter)context).ObjectContext;
            var objectType = ObjectContext.GetObjectType(type); // Works with proxies and original types.

            var entityType = oc.MetadataWorkspace.GetItems(DataSpace.OSpace).OfType<EntityType>()
                               .FirstOrDefault(et => et.Name == objectType.Name);
            return entityType != null
                ? entityType.NavigationProperties
                : Enumerable.Empty<NavigationProperty>();
        }

        /// <summary>
        /// Recursively detaches entities from Context, this
        /// method detaches every navigation properties
        /// of current Entity as well.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="entity"></param>
        public virtual void Detach( T entity)
        {
            DbEntityEntry dbEntityEntry = DbContext.Entry(entity);

            if (entity == null || dbEntityEntry.State == EntityState.Detached)
                return;

            dbEntityEntry.State = EntityState.Detached;

            Type entityType = typeof(T);

            // get all navigation properties…
            Type type = entity.GetType();
            foreach (var item in type.GetProperties())
            {
                if (entityType.IsAssignableFrom(item.PropertyType))
                {
                    Detach( item.GetValue(entity, null) as T);
                    continue;
                }
                if (item.PropertyType.Name.StartsWith("EntityCollection"))
                {
                    IEnumerable coll = item.GetValue(entity, null) as IEnumerable;
                    foreach (T child in coll)
                    {
                        Detach(child);
                    }
                }
            }
        }

    }
}
