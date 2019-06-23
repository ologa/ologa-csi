namespace EFDataAccess
{
    using System;
    using System.Data.Entity;
    using System.Linq;
    using VPPS.CSI.Domain;
    using EFDataAccess.Exceptions;
    using EFDataAccess.Conventions;
    using EFDataAccess.Migrations;
    using System.Data.Entity.SqlServer;
    using System.Diagnostics;
    using System.Data.Entity.Infrastructure;
    using System.Collections.Generic;
    using EFDataAccess.Logging;
    using System.Web;
    using System.Web.Caching;

    public partial class ApplicationDbContext : DbContext
    {
        private ILogger _logger = new Logger();

        public ApplicationDbContext() : base("name=ApplicationDbContext")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<ApplicationDbContext, Configuration>());
            if (System.Configuration.ConfigurationManager.AppSettings["SqlLog"].Equals("true"))
            {
                this.Database.Log = message => Debug.WriteLine(message);
            }
            
            // Habilitar o lazy loading para melhor controlo da memoria
            this.Configuration.LazyLoadingEnabled = true;

            // Aumentar o timeout para os comandos da base
            var objectContext = (this as IObjectContextAdapter).ObjectContext;
            objectContext.CommandTimeout = 5000;

            // Desabilitar a deteccao automatica de mudancas para melhorar desempenho
            Configuration.AutoDetectChangesEnabled = false;
        }

        public ApplicationDbContext(string connectionName) : base(connectionName)
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<ApplicationDbContext, Configuration>());
            if (System.Configuration.ConfigurationManager.AppSettings["SqlLog"].Equals("True"))
            {
                this.Database.Log = message => Debug.WriteLine(message);
            }
        }

        static ApplicationDbContext()
        {
            var hackToForceLoadOfSqlProviderServices = typeof(SqlProviderServices);
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public void CacheUserInfo(User user)
        {
            HttpRuntime.Cache.Insert(
              /* key */                "user",
              /* value */              user,
              /* dependencies */       null,
              /* absoluteExpiration */ Cache.NoAbsoluteExpiration,
              /* slidingExpiration */  Cache.NoSlidingExpiration,
              /* priority */           System.Web.Caching.CacheItemPriority.NotRemovable,
              /* onRemoveCallback */   null);
        }

        public User GetUserInfo()
        {
            if (!(HttpRuntime.Cache["user"] is User user))
            {
                user = this.Users.Where(u => u.Username == "Admin").FirstOrDefault();
            }
            return user;
        }

        public override int SaveChanges()
        {
            logInfo("Method saveChanges invoked ...");

            var addedAuditedEntities = ChangeTracker.Entries<AuditedEntity>()
                .Where(p => p.State == EntityState.Added)
                .Select(p => p.Entity);
            var modifiedAuditedEntities = ChangeTracker.Entries<AuditedEntity>()
                .Where(p => p.State == EntityState.Modified)
                .Select(p => p.Entity);

            var now = DateTime.Now;

            if (addedAuditedEntities.Count() > 0) { logInfo("Creating entities ..."); }
            foreach (var added in addedAuditedEntities)
            {
                logInfo(added.GetType().FullName);
                // Update dates only if the entity is not being syncronized
                if (added.SyncDate == null) { added.CreatedDate = now; }
                // Mark data as never synced
                added.SyncState = -1;
                // Set user who created the object
                SetCreatedUserID(added);
            }

            if (modifiedAuditedEntities.Count() > 0) { logInfo("Modifying entities ..."); }
            foreach (var modified in modifiedAuditedEntities)
            {
                logInfo(modified.GetType().FullName);
                // Update dates only if the entity is not being syncronized
                if (modified.SyncDate == null) { modified.LastUpdatedDate = now; }
                // Mark data has changed, only if it have been synced before
                if (modified.SyncState == 1) { modified.SyncState = 0; }
                // Set user who created the object
                SetLastUpdatedUserID(modified);
                // Store saved entities
            }

            try
            {
                return base.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Erro ao salvar os dados na BD ", null);
                throw ex;
            }
        }

        public void SetCreatedUserID(AuditedEntity auditedEntity)
        {
            User user = GetUserInfo();
            if (auditedEntity.GetType().FullName.Equals("VPPS.CSI.Domain.Beneficiary"))
            { ((Beneficiary)auditedEntity).CreatedUserID = user.UserID; }
            if (auditedEntity.GetType().FullName.Equals("VPPS.CSI.Domain.ChildStatusHistory"))
            { ((ChildStatusHistory)auditedEntity).CreatedUserID = user.UserID; }
            if (auditedEntity.GetType().FullName.Equals("VPPS.CSI.Domain.ChildStatus"))
            { ((ChildStatus)auditedEntity).CreatedUserID = user.UserID; }
        }

        public void SetLastUpdatedUserID(AuditedEntity auditedEntity)
        {
            User user = GetUserInfo();
            if (auditedEntity.GetType().FullName.Equals("VPPS.CSI.Domain.Beneficiary"))
            { ((Beneficiary)auditedEntity).LastUpdatedUserID = user.UserID; }
            if (auditedEntity.GetType().FullName.Equals("VPPS.CSI.Domain.ChildStatusHistory"))
            { ((ChildStatusHistory)auditedEntity).LastUpdatedUserID = user.UserID; }
            if (auditedEntity.GetType().FullName.Equals("VPPS.CSI.Domain.ChildStatus"))
            { ((ChildStatus)auditedEntity).LastUpdatedUserID = user.UserID; }
        }


        public void logInfo(string info)
        {
            _logger.Information(info);
        }

        public virtual DbSet<ChildStatus> ChildStatus { get; set; }
        public virtual DbSet<ChildStatusHistory> ChildStatusHistories { get; set; }
        public virtual DbSet<DomainEntity> Domains { get; set; }
        public virtual DbSet<OVCType> OVCTypes { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<SimpleEntity> SimpleEntities { get; set; }
        public virtual DbSet<HIVStatus> HIVStatus { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }
        public virtual DbSet<Beneficiary> Beneficiaries { get; set; }
        public virtual DbSet<Trimester> Trimesters { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Add<ForeignKeyNamingConvention>();
            modelBuilder.Entity<OVCType>().Property(e => e.Description).IsUnicode(false);
			modelBuilder.Entity<ChildStatusHistory>().HasOptional(c => c.ChildStatus).WithMany().HasForeignKey(c => c.ChildStatusID);
			modelBuilder.Entity<ChildStatusHistory>().HasOptional(c => c.CreatedUser).WithMany().HasForeignKey(c => c.CreatedUserID);
			modelBuilder.Entity<ChildStatusHistory>().HasOptional(c => c.LastUpdatedUser).WithMany().HasForeignKey(c => c.LastUpdatedUserID);
            modelBuilder.Entity<SimpleEntity>().HasOptional(c => c.CreatedUser).WithMany().HasForeignKey(c => c.CreatedUserID);
            modelBuilder.Entity<SimpleEntity>().HasOptional(c => c.LastUpdatedUser).WithMany().HasForeignKey(c => c.LastUpdatedUserID);
            modelBuilder.Entity<ChildStatus>().HasOptional(c => c.CreatedUser).WithMany().HasForeignKey(c => c.CreatedUserID);
            modelBuilder.Entity<ChildStatus>().HasOptional(c => c.LastUpdatedUser).WithMany().HasForeignKey(c => c.LastUpdatedUserID);
            modelBuilder.Entity<Beneficiary>().HasRequired(a => a.KinshipToFamilyHead).WithMany().HasForeignKey(a => a.KinshipToFamilyHeadID).WillCascadeOnDelete(false);
            modelBuilder.Entity<Beneficiary>().HasOptional(a => a.OVCType).WithMany().HasForeignKey(a => a.OVCTypeID).WillCascadeOnDelete(false);
            modelBuilder.Entity<Beneficiary>().HasOptional(c => c.CreatedUser).WithMany().HasForeignKey(c => c.CreatedUserID);
            modelBuilder.Entity<Beneficiary>().HasOptional(c => c.LastUpdatedUser).WithMany().HasForeignKey(c => c.LastUpdatedUserID);
        }
    }
}
