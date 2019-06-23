using EFDataAccess.Repository;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.EntityClient;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VPPS.CSI.Domain;

namespace EFDataAccess.UOW
{
    /// <summary>
    /// The "Unit of Work"  
    ///      decouples the DbContext and EF from the controllers
    ///      manages the UoW
    /// </summary>
    /// <remarks>
    /// This class implements the "Unit of Work" pattern in which
    /// the "UoW" serves as a facade for querying and saving to the database.
    /// Querying is delegated to "repositories".
    /// Each repository serves as a container dedicated to a particular
    /// root entity type such as a applicant.
    /// A repository typically exposes "Get" methods for querying and
    /// will offer add, update, and delete methods if those features are supported.
    /// The repositories rely on their parent UoW to provide the interface to the
    /// data .
    /// </remarks>
   public class UnitOfWork : IUnitOfWork, IDisposable
    {
        internal ApplicationDbContext DbContext { get; set; }

        public UnitOfWork()
        {
            //if app user has low level access change database login to a low level user
            if (EFConstants.USERObfuscateMode == EFConstants.ObfuscateMode.Masked)
                CreateDbContext("name=CSIContextReader");
            else
                CreateDbContext();
        }


        //repositories
        #region Repositries
        private Hashtable repositories;


       /// <summary>
       /// Returns a repository for the specified entity type.
       /// </summary>
       /// <typeparam name="TEntity">The type of the entity.</typeparam>
       /// <returns>A repository for the specified entity type.</returns>
       public IRepository<TEntity> Repository<TEntity>() where TEntity : class
       {
           if (repositories == null)
               repositories = new Hashtable();

           var entityTypeName = typeof(TEntity).Name;

           if (!repositories.ContainsKey(entityTypeName))
           {
               var repositoryType = typeof(BaseRepository<>);

               // Could replace this with call to DependencyDepency
               var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(TEntity)), DbContext);

               repositories.Add(entityTypeName, repositoryInstance);
           }

           return (IRepository<TEntity>)repositories[entityTypeName];
       }

        #endregion

        /// <summary>
        /// Save pending changes to the database
        /// </summary>
        public void Commit()
        {
            DbContext.SaveChanges();
        }

        public void CacheUserInfo(User user)
        {
            DbContext.CacheUserInfo(user);
        }

        protected void CreateDbContext(string connection = null)
        {
            if (string.IsNullOrEmpty(connection))
                DbContext = new ApplicationDbContext();
            else
                DbContext = new ApplicationDbContext(connection);

            DbContext.Configuration.AutoDetectChangesEnabled = false;
            DbContext.Configuration.ProxyCreationEnabled = false;
            DbContext.Configuration.LazyLoadingEnabled = false;
            DbContext.Configuration.ValidateOnSaveEnabled = false;
        }

        public int ExecuteSqlCommand(string sql)
        {
           return DbContext.Database.ExecuteSqlCommand(sql);
        }

        public void UndoChangesDbContextLevel()
        {
            foreach (DbEntityEntry entry in DbContext.ChangeTracker.Entries())
            {
                switch (entry.State)
                {
                    case EntityState.Modified:
                        entry.State = EntityState.Unchanged;
                        break;
                    case EntityState.Added:
                        entry.State = EntityState.Detached;
                        break;
                    case EntityState.Deleted:
                        entry.Reload();
                        break;
                    default: break;
                }
            }   
        }

        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (DbContext != null)
                {
                    DbContext.Dispose();
                }
            }
        }

        #endregion
    }
}
