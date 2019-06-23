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

        private static List<string> duplicatedEntities = new List<string>(){
            "VPPS.CSI.Domain.ChildStatus",
            "VPPS.CSI.Domain.SimpleEntity",
            "VPPS.CSI.Domain.OrgUnit",
            "VPPS.CSI.Domain.OrgUnitType",
            "VPPS.CSI.Domain.Partner" };

        private static List<string> uniqueWhenSaving = new List<string>(){
            "VPPS.CSI.Domain.Household",
            "VPPS.CSI.Domain.Child",
            "VPPS.CSI.Domain.Adult",
            "VPPS.CSI.Domain.Partner" };

        private static List<string> allEntitiesToSave = new List<string>(){};

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
                DuplicationControl(added);

                if (duplicatedEntities.Contains(added.GetType().FullName))
                {
                    logInfo("Possível duplicação ... veja o cenário de criação deste dado.");
                }
                if (uniqueWhenSaving.Contains(added.GetType().FullName) && allEntitiesToSave.Contains(added.GetType().FullName))
                {
                    logInfo("A ultima entidade existe duas vezes na lista. Possível cenário de duplicação.");
                    if (System.Configuration.ConfigurationManager.AppSettings["BatchObjectSave"].Contains("False"))
                    { throw new DuplicatedObjectException(); }
                }

                // Update dates only if the entity is not being syncronized
                if (added.SyncDate == null) { added.CreatedDate = now; }
                // Mark data as never synced
                added.SyncState = -1;
                // Set user who created the object
                SetCreatedUserID(added);
                // Store saved entities
                allEntitiesToSave.Add(added.GetType().FullName);
            }

            if (modifiedAuditedEntities.Count() > 0) { logInfo("Modifying entities ..."); }
            foreach (var modified in modifiedAuditedEntities)
            {
                logInfo(modified.GetType().FullName);
                DuplicationControl(modified);
                if (duplicatedEntities.Contains(modified.GetType().FullName))
                {
                    logInfo("Possível duplicação ... veja o cenário de criação deste dado.");
                }
                if (uniqueWhenSaving.Contains(modified.GetType().FullName) && allEntitiesToSave.Contains(modified.GetType().FullName))
                {
                    logInfo("A ultima entidade existe duas vezes na lista. Possível cenário de duplicação.");
                    if (System.Configuration.ConfigurationManager.AppSettings["BatchObjectSave"].Contains("False"))
                    { throw new DuplicatedObjectException(); }
                }

                // Update dates only if the entity is not being syncronized
                if (modified.SyncDate == null) { modified.LastUpdatedDate = now; }
                // Mark data has changed, only if it have been synced before
                if (modified.SyncState == 1) { modified.SyncState = 0; }
                // Set user who created the object
                SetLastUpdatedUserID(modified);
                // Store saved entities
                allEntitiesToSave.Add(modified.GetType().FullName);
            }

            allEntitiesToSave = new List<string>() { };

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

        public void DuplicationControl(AuditedEntity auditedEntity)
        {
            if (System.Configuration.ConfigurationManager.AppSettings["DuplicationControl"].Equals("True"))
            {
                if (auditedEntity.GetType().FullName.Equals("VPPS.CSI.Domain.ReferenceService"))
                { logInfo("Guid : " + ((ReferenceService)auditedEntity).ReferenceServiceGuid); }
                if (auditedEntity.GetType().FullName.Equals("VPPS.CSI.Domain.RoutineVisit"))
                { logInfo("Guid : " + ((RoutineVisit)auditedEntity).RoutineVisit_guid); }
                if (auditedEntity.GetType().FullName.Equals("VPPS.CSI.Domain.CSI"))
                { logInfo("Guid : " + ((CSI)auditedEntity).csi_guid); }
                if (auditedEntity.GetType().FullName.Equals("VPPS.CSI.Domain.CarePlan"))
                { logInfo("Guid : " + ((CarePlan)auditedEntity).careplan_guid); }
                if (auditedEntity.GetType().FullName.Equals("VPPS.CSI.Domain.Household"))
                { logInfo("Guid : " + ((Household)auditedEntity).HouseholdUniqueIdentifier); }
                if (auditedEntity.GetType().FullName.Equals("VPPS.CSI.Domain.Child"))
                { logInfo("Guid : " + ((Child)auditedEntity).child_guid); }
                if (auditedEntity.GetType().FullName.Equals("VPPS.CSI.Domain.Adult"))
                { logInfo("Guid : " + ((Adult)auditedEntity).AdultGuid); }
                if (auditedEntity.GetType().FullName.Equals("VPPS.CSI.Domain.ChildStatus"))
                {
                    logInfo("Guid : " + ((ChildStatus)auditedEntity).childstatus_guid);
                    throw new DuplicatedObjectException();
                }
                if (auditedEntity.GetType().FullName.Equals("VPPS.CSI.Domain.SimpleEntity"))
                {
                    logInfo("Id : " + ((SimpleEntity)auditedEntity).SimpleEntityID);
                    throw new DuplicatedObjectException();
                }
                if (auditedEntity.GetType().FullName.Equals("VPPS.CSI.Domain.OrgUnit"))
                {
                    logInfo("Guid : " + ((OrgUnit)auditedEntity).OrgUnit_Guid);
                    throw new DuplicatedObjectException();
                }
                if (auditedEntity.GetType().FullName.Equals("VPPS.CSI.Domain.OrgUnitType"))
                {
                    logInfo("Id : " + ((OrgUnitType)auditedEntity).OrgUnitTypeID);
                    throw new DuplicatedObjectException();
                }
                if (auditedEntity.GetType().FullName.Equals("VPPS.CSI.Domain.Partner"))
                { logInfo("Guid : " + ((Partner)auditedEntity).partner_guid); }
            }
        }

        public void SetCreatedUserID(AuditedEntity auditedEntity)
        {
            User user = GetUserInfo();

            if (auditedEntity.GetType().FullName.Equals("VPPS.CSI.Domain.ReferenceService"))
            { ((ReferenceService)auditedEntity).CreatedUserID = user.UserID; }
            if (auditedEntity.GetType().FullName.Equals("VPPS.CSI.Domain.Reference"))
            { ((Reference)auditedEntity).CreatedUserID = user.UserID; }

            if (auditedEntity.GetType().FullName.Equals("VPPS.CSI.Domain.RoutineVisit"))
            { ((RoutineVisit)auditedEntity).CreatedUserID = user.UserID; }
            if (auditedEntity.GetType().FullName.Equals("VPPS.CSI.Domain.RoutineVisitMember"))
            { ((RoutineVisitMember)auditedEntity).CreatedUserID = user.UserID; }
            if (auditedEntity.GetType().FullName.Equals("VPPS.CSI.Domain.RoutineVisitSupport"))
            { ((RoutineVisitSupport)auditedEntity).CreatedUserID = user.UserID; }

            if (auditedEntity.GetType().FullName.Equals("VPPS.CSI.Domain.CSI"))
            { ((CSI)auditedEntity).CreatedUserID = user.UserID; }
            if (auditedEntity.GetType().FullName.Equals("VPPS.CSI.Domain.CSIDomain"))
            { ((CSIDomain)auditedEntity).CreatedUserID = user.UserID; }
            if (auditedEntity.GetType().FullName.Equals("VPPS.CSI.Domain.CSIDomainScore"))
            { ((CSIDomainScore)auditedEntity).CreatedUserID = user.UserID; }

            if (auditedEntity.GetType().FullName.Equals("VPPS.CSI.Domain.Household"))
            { ((Household)auditedEntity).CreatedUserID = user.UserID; }
            if (auditedEntity.GetType().FullName.Equals("VPPS.CSI.Domain.Aid"))
            { ((Aid)auditedEntity).CreatedUserID = user.UserID; }

            if (auditedEntity.GetType().FullName.Equals("VPPS.CSI.Domain.Beneficiary"))
            { ((Beneficiary)auditedEntity).CreatedUserID = user.UserID; }
            if (auditedEntity.GetType().FullName.Equals("VPPS.CSI.Domain.ChildStatusHistory"))
            { ((ChildStatusHistory)auditedEntity).CreatedUserID = user.UserID; }
            if (auditedEntity.GetType().FullName.Equals("VPPS.CSI.Domain.ChildStatus"))
            { ((ChildStatus)auditedEntity).CreatedUserID = user.UserID; }

            if (auditedEntity.GetType().FullName.Equals("VPPS.CSI.Domain.Partner"))
            { ((Partner)auditedEntity).CreatedUserID = user.UserID; }
        }

        public void SetLastUpdatedUserID(AuditedEntity auditedEntity)
        {
            User user = GetUserInfo();

            if (auditedEntity.GetType().FullName.Equals("VPPS.CSI.Domain.ReferenceService"))
            { ((ReferenceService)auditedEntity).LastUpdatedUserID = user.UserID; }
            if (auditedEntity.GetType().FullName.Equals("VPPS.CSI.Domain.Reference"))
            { ((Reference)auditedEntity).LastUpdatedUserID = user.UserID; }

            if (auditedEntity.GetType().FullName.Equals("VPPS.CSI.Domain.RoutineVisit"))
            { ((RoutineVisit)auditedEntity).LastUpdatedUserID = user.UserID; }
            if (auditedEntity.GetType().FullName.Equals("VPPS.CSI.Domain.RoutineVisitMember"))
            { ((RoutineVisitMember)auditedEntity).LastUpdatedUserID = user.UserID; }
            if (auditedEntity.GetType().FullName.Equals("VPPS.CSI.Domain.RoutineVisitSupport"))
            { ((RoutineVisitSupport)auditedEntity).LastUpdatedUserID = user.UserID; }

            if (auditedEntity.GetType().FullName.Equals("VPPS.CSI.Domain.CSI"))
            { ((CSI)auditedEntity).LastUpdatedUserID = user.UserID; }
            if (auditedEntity.GetType().FullName.Equals("VPPS.CSI.Domain.CSIDomain"))
            { ((CSIDomain)auditedEntity).LastUpdatedUserID = user.UserID; }
            if (auditedEntity.GetType().FullName.Equals("VPPS.CSI.Domain.CSIDomainScore"))
            { ((CSIDomainScore)auditedEntity).LastUpdatedUserID = user.UserID; }

            if (auditedEntity.GetType().FullName.Equals("VPPS.CSI.Domain.Household"))
            { ((Household)auditedEntity).LastUpdatedUserID = user.UserID; }
            if (auditedEntity.GetType().FullName.Equals("VPPS.CSI.Domain.Aid"))
            { ((Aid)auditedEntity).LastUpdatedUserID = user.UserID; }

            if (auditedEntity.GetType().FullName.Equals("VPPS.CSI.Domain.Beneficiary"))
            { ((Beneficiary)auditedEntity).LastUpdatedUserID = user.UserID; }
            if (auditedEntity.GetType().FullName.Equals("VPPS.CSI.Domain.ChildStatusHistory"))
            { ((ChildStatusHistory)auditedEntity).LastUpdatedUserID = user.UserID; }
            if (auditedEntity.GetType().FullName.Equals("VPPS.CSI.Domain.ChildStatus"))
            { ((ChildStatus)auditedEntity).LastUpdatedUserID = user.UserID; }

            if (auditedEntity.GetType().FullName.Equals("VPPS.CSI.Domain.Partner"))
            { ((Partner)auditedEntity).LastUpdatedUserID = user.UserID; }
        }

        public void logInfo(string info)
        {
            _logger.Information(info);
        }

        public virtual DbSet<VPPS.CSI.Domain.Action> Actions { get; set; }
        public virtual DbSet<Adult> Adults { get; set; }
        public virtual DbSet<Answer> Answers { get; set; }
        public virtual DbSet<CareGiverRelation> CareGiverRelations { get; set; }
        public virtual DbSet<CarePlan> CarePlans { get; set; }
        public virtual DbSet<CarePlanDomain> CarePlanDomains { get; set; }
        public virtual DbSet<CarePlanDomainSupportService> CarePlanDomainSupportServices { get; set; }
        public virtual DbSet<CollaboratorRole> CollaboratorRoles { get; set; }
        public virtual DbSet<Child> Children { get; set; }
        public virtual DbSet<ChildDisability> ChildDisabilities { get; set; }
        public virtual DbSet<ChildEvent> ChildEvents { get; set; }
        public virtual DbSet<ChildPartner> ChildPartners { get; set; }
        public virtual DbSet<ChildProject> ChildProjects { get; set; }
        public virtual DbSet<ChildRegistration> ChildRegistrations { get; set; }
        public virtual DbSet<ChildStatus> ChildStatus { get; set; }
        public virtual DbSet<ChildStatusHistory> ChildStatusHistories { get; set; }
        public virtual DbSet<ChildTrainingEvent> ChildTrainingEvents { get; set; }
        public virtual DbSet<ChildTrainingEventItem> ChildTrainingEventItems { get; set; }
        public virtual DbSet<Config> Configs { get; set; }
        public virtual DbSet<ConfigType> ConfigTypes { get; set; }
        public virtual DbSet<CommunityCouncil> CommunityCouncils { get; set; }
        public virtual DbSet<CSI> CSIs { get; set; }
        public virtual DbSet<CSIDomain> CSIDomains { get; set; }
        public virtual DbSet<CSIDomainScore> CSIDomainScores { get; set; }
        public virtual DbSet<CSIDomainSource> CSIDomainSources { get; set; }
        public virtual DbSet<CSIDomainSupportService> CSIDomainSupportServices { get; set; }
        public virtual DbSet<CSIEvent> CSIEvents { get; set; }
        public virtual DbSet<CutPoint> CutPoints { get; set; }
        public virtual DbSet<Disability> Disabilities { get; set; }
        public virtual DbSet<District> Districts { get; set; }
        public virtual DbSet<DomainEntity> Domains { get; set; }
        public virtual DbSet<DomainCriteria> DomainsCriteria { get; set; }
        public virtual DbSet<Drug> Drugs { get; set; }
        public virtual DbSet<DrugDose> DrugDoses { get; set; }
        public virtual DbSet<Encounter> Encounters { get; set; }
        public virtual DbSet<EncounterDrug> EncounterDrugs { get; set; }
        public virtual DbSet<EncounterType> EncounterTypes { get; set; }
        public virtual DbSet<Event> Events { get; set; }
        public virtual DbSet<Field> Fields { get; set; }
        public virtual DbSet<FollowUp> FollowUps { get; set; }
        public virtual DbSet<Form> Forms { get; set; }
        public virtual DbSet<FormField> FormFields { get; set; }
        public virtual DbSet<GuardianRelation> GuardianRelations { get; set; }
        public virtual DbSet<Guideline> Guidelines { get; set; }
        public virtual DbSet<GraduationCriteria> GraduationsCriteria { get; set; }
        public virtual DbSet<Household> Households { get; set; }
        public virtual DbSet<Language> Languages { get; set; }
        public virtual DbSet<Location> Locations { get; set; }
        public virtual DbSet<OrgUnit> OrgUnits { get; set; }
        public virtual DbSet<OrgUnitType> OrgUnitTypes { get; set; }
        public virtual DbSet<Outcome> Outcomes { get; set; }
        public virtual DbSet<OVCType> OVCTypes { get; set; }
        public virtual DbSet<Partner> Partners { get; set; }
        public virtual DbSet<PartnerProject> PartnerProjects { get; set; }
        public virtual DbSet<PartnerProjectSnapshot> PartnerProjectSnapshots { get; set; }
        public virtual DbSet<Project> Projects { get; set; }
        public virtual DbSet<ProjectTrackerItem> ProjectTrackerItems { get; set; }
        public virtual DbSet<ProjectTrackerItemValue> ProjectTrackerItemValues { get; set; }
        public virtual DbSet<Question> Questions { get; set; }
        public virtual DbSet<QuestionCriteria> QuestionsCriteria { get; set; }
        public virtual DbSet<RecipientType> RecipientTypes { get; set; }
        public virtual DbSet<Reference> References { get; set; }
        public virtual DbSet<ReferenceService> ReferenceServices { get; set; }
        public virtual DbSet<ReferenceType> ReferenceTypes { get; set; }
        public virtual DbSet<RegistrationType> RegistrationTypes { get; set; }
        public virtual DbSet<Resource> Resources { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<RoutineVisit> RoutineVisits { get; set; }
        public virtual DbSet<RoutineVisitLog> RoutineVisitLogs { get; set; }
        public virtual DbSet<RoutineVisitMember> RoutineVisitMembers { get; set; }
        public virtual DbSet<RoutineVisitSupport> RoutineVisitSupports { get; set; }
        public virtual DbSet<SchoolLevel> SchoolLevels { get; set; }
        public virtual DbSet<ScoreType> Scores { get; set; }
        public virtual DbSet<Service> Services { get; set; }
        public virtual DbSet<ServiceCategory> ServiceCategorys { get; set; }
        public virtual DbSet<ServiceInstance> ServiceInstances { get; set; }
        public virtual DbSet<ServiceTrack> ServiceTracks { get; set; }
        public virtual DbSet<ServiceTrackSection> ServiceTrackSections { get; set; }
        public virtual DbSet<ServiceProvider> ServiceProviders { get; set; }
        public virtual DbSet<ServiceProviderType> ServiceProviderTypes { get; set; }
        public virtual DbSet<Site> Sites { get; set; }
        public virtual DbSet<SiteConfig> SiteConfigs { get; set; }
        public virtual DbSet<SiteProgress> SiteProgresses { get; set; }
        public virtual DbSet<SiteProgressItem> SiteProgressItems { get; set; }
        public virtual DbSet<SiteProgressItemType> SiteProgressItemTypes { get; set; }
        public virtual DbSet<Snapshot> Snapshots { get; set; }
        public virtual DbSet<Source> Sources { get; set; }
        public virtual DbSet<SupportServiceType> SupportServiceTypes { get; set; }
        public virtual DbSet<TrackerItem> TrackerItems { get; set; }
        public virtual DbSet<Training> Trainings { get; set; }
        public virtual DbSet<TrainingEvent> TrainingEvents { get; set; }
        public virtual DbSet<TrainingItem> TrainingItems { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<UserAction> UserActions { get; set; }
        public virtual DbSet<Village> Villages { get; set; }
        public virtual DbSet<Ward> Wards { get; set; }
        public virtual DbSet<WellbeingXref> WellbeingXrefs { get; set; }
        public virtual DbSet<GeneralInfo> GeneralInfoes { get; set; }
        public virtual DbSet<SimpleEntity> SimpleEntities { get; set; }
        public virtual DbSet<Aid> Aids { get; set; }
        public virtual DbSet<HIVStatus> HIVStatus { get; set; }
        public virtual DbSet<SiteGoal> SiteGoals { get; set; }
        public virtual DbSet<VPPS.CSI.Domain.Task> Tasks { get; set; }
        public virtual DbSet<UserRole> UserRoles { get; set; }
        public virtual DbSet<ReportData> ReportData { get; set; }
        public virtual DbSet<Query> Queries { get; set; }
        public virtual DbSet<HouseholdSupportPlan> HouseholdSupportPlans { get; set; }
        public virtual DbSet<HouseholdStatus> HouseholdStatus { get; set; }
        public virtual DbSet<HouseholdStatusHistory> HouseholdStatusHistories { get; set; }
        public virtual DbSet<Beneficiary> Beneficiaries { get; set; }
        public virtual DbSet<TrimesterDefinition> TrimesterDefinitions { get; set; }
        public virtual DbSet<Trimester> Trimesters { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Add<ForeignKeyNamingConvention>();
            modelBuilder.Entity<GraduationCriteria>().HasMany(c => c.ExcludedQuestions).WithMany(q => q.GraduationCriterias).Map(m =>{ m.MapLeftKey("GraduationCriteriaID"); m.MapRightKey("QuestionID"); m.ToTable("QuestionGraduationCriterias"); });
            modelBuilder.Entity<CareGiverRelation>().Property(e => e.Description).IsUnicode(false);
            modelBuilder.Entity<CarePlanDomain>().Property(e => e.Actions).IsUnicode(false);
            modelBuilder.Entity<CarePlanDomain>().Property(e => e.FamilyReunificationAction).IsFixedLength().IsUnicode(false);
            modelBuilder.Entity<CarePlanDomain>().Property(e => e.ActionCompleted).IsFixedLength().IsUnicode(false);
            modelBuilder.Entity<CarePlanDomain>().Property(e => e.ActionCompletedComments).IsUnicode(false);
            modelBuilder.Entity<CarePlanDomain>().Property(e => e.ResponsibleUser).IsUnicode(false);
            modelBuilder.Entity<CarePlanDomain>().HasMany(e => e.CarePlanDomainSupportServices).WithRequired(e => e.CarePlanDomain).WillCascadeOnDelete(false);
            modelBuilder.Entity<Child>().Property(e => e.FirstName).IsUnicode(false);
            modelBuilder.Entity<Child>().Property(e => e.LastName).IsUnicode(false);
            modelBuilder.Entity<Child>().Property(e => e.Gender).IsFixedLength().IsUnicode(false);
            modelBuilder.Entity<Child>().Property(e => e.Guardian).IsUnicode(false);
            modelBuilder.Entity<Child>().Property(e => e.Notes).IsUnicode(false);
            modelBuilder.Entity<Child>().Property(e => e.PrincipalChief).IsUnicode(false);
            modelBuilder.Entity<Child>().Property(e => e.VillageChief).IsUnicode(false);
            modelBuilder.Entity<Child>().Property(e => e.EducationBursary).IsFixedLength().IsUnicode(false);
            modelBuilder.Entity<Child>().Property(e => e.EligibleFamilyReunification).IsFixedLength().IsUnicode(false);
			modelBuilder.Entity<Child>().Property(e => e.MaterialStationery).IsFixedLength().IsUnicode(false);
			modelBuilder.Entity<Child>().Property(e => e.Literate).IsFixedLength().IsUnicode(false);
            modelBuilder.Entity<Child>().Property(e => e.Numeracy).IsFixedLength().IsUnicode(false);
			modelBuilder.Entity<Child>().Property(e => e.MarketingStarter).IsFixedLength().IsUnicode(false);
            modelBuilder.Entity<Child>().Property(e => e.CapacityIGA).IsFixedLength().IsUnicode(false);
			modelBuilder.Entity<Child>().Property(e => e.EarnIGA).IsFixedLength().IsUnicode(false);
			modelBuilder.Entity<Child>().Property(e => e.SufficientMeals).IsFixedLength().IsUnicode(false);
            modelBuilder.Entity<Child>().Property(e => e.GuardianIdNo).IsUnicode(false);
			modelBuilder.Entity<Child>().Property(e => e.ContactNo).IsUnicode(false);
			modelBuilder.Entity<Child>().Property(e => e.SchoolName).IsUnicode(false);
			modelBuilder.Entity<Child>().Property(e => e.SchoolContactNo).IsUnicode(false);
			modelBuilder.Entity<Child>().Property(e => e.DisabilityNotes).IsUnicode(false);
			modelBuilder.Entity<Child>().Property(e => e.PrincipalName).IsUnicode(false);
			modelBuilder.Entity<Child>().Property(e => e.PrincipalContactNo).IsUnicode(false);
			modelBuilder.Entity<Child>().Property(e => e.TeacherName).IsUnicode(false);
			modelBuilder.Entity<Child>().Property(e => e.TeacherContactNo).IsUnicode(false);
			modelBuilder.Entity<Child>().Property(e => e.HeadName).IsUnicode(false);
			modelBuilder.Entity<Child>().Property(e => e.HeadContactNo).IsUnicode(false);
			modelBuilder.Entity<Child>().Property(e => e.Address).IsUnicode(false);
			modelBuilder.Entity<Child>().Property(e => e.Grade).IsUnicode(false);
			modelBuilder.Entity<Child>().Property(e => e.Class).IsUnicode(false);
			modelBuilder.Entity<Child>().HasMany(e => e.ChildEvents).WithRequired(e => e.Child).WillCascadeOnDelete(false);
			modelBuilder.Entity<Child>().HasMany(e => e.ChildProjects).WithRequired(e => e.Child).WillCascadeOnDelete(false);
			modelBuilder.Entity<Child>().HasMany(e => e.ChildRegistrations).WithRequired(e => e.Child).WillCascadeOnDelete(false);
			modelBuilder.Entity<Child>().HasMany(e => e.ChildTrainingEvents).WithRequired(e => e.Child).WillCascadeOnDelete(false);
			modelBuilder.Entity<Child>().HasMany(e => e.Encounters).WithRequired(e => e.Child).WillCascadeOnDelete(false);
			modelBuilder.Entity<Child>().HasMany(e => e.ChildDisabilities).WithRequired(e => e.Child).WillCascadeOnDelete(false);
			modelBuilder.Entity<Child>().HasMany(e => e.ChildPartners).WithRequired(e => e.Child).WillCascadeOnDelete(false);
			modelBuilder.Entity<Child>().HasMany(e => e.ChildStatusHistories).WithRequired(e => e.Child).WillCascadeOnDelete(false);
			modelBuilder.Entity<Child>().HasMany(e => e.CSIs).WithOptional(e => e.Child).WillCascadeOnDelete(false);
			modelBuilder.Entity<ChildEvent>().Property(e => e.Comments).IsFixedLength().IsUnicode(false);
			modelBuilder.Entity<ChildRegistration>().Property(e => e.Detail).IsUnicode(false);
			modelBuilder.Entity<ChildStatus>().Property(e => e.Description).IsUnicode(false);
			modelBuilder.Entity<ChildTrainingEvent>().Property(e => e.Registered).IsFixedLength().IsUnicode(false);
			modelBuilder.Entity<ChildTrainingEvent>().Property(e => e.Participated).IsFixedLength().IsUnicode(false);
			modelBuilder.Entity<ChildTrainingEvent>().Property(e => e.Completed).IsFixedLength().IsUnicode(false);
			modelBuilder.Entity<ChildTrainingEvent>().HasMany(e => e.ChildTrainingEventItems).WithRequired(e => e.ChildTrainingEvent).WillCascadeOnDelete(false);
			modelBuilder.Entity<ChildTrainingEventItem>().Property(e => e.Proficient).IsFixedLength().IsUnicode(false);
			modelBuilder.Entity<CommunityCouncil>().Property(e => e.Description).IsUnicode(false);
			modelBuilder.Entity<CSI>().Property(e => e.Height).HasPrecision(10, 2);
			modelBuilder.Entity<CSI>().Property(e => e.Weight).HasPrecision(10, 2);
			modelBuilder.Entity<CSI>().Property(e => e.BMI).HasPrecision(10, 2);
			modelBuilder.Entity<CSI>().Property(e => e.MedicationDescription).IsUnicode(false);
			modelBuilder.Entity<CSI>().Property(e => e.Suggestions).IsUnicode(false);
			modelBuilder.Entity<CSI>().Property(e => e.Caregiver).IsUnicode(false);
			modelBuilder.Entity<CSI>().Property(e => e.OutreachBenefit).IsFixedLength().IsUnicode(false);
			modelBuilder.Entity<CSI>().Property(e => e.SocialWorkerName).IsUnicode(false);
			modelBuilder.Entity<CSI>().Property(e => e.SocialWorkerContactNo).IsUnicode(false);
			modelBuilder.Entity<CSI>().Property(e => e.DoctorName).IsUnicode(false);
			modelBuilder.Entity<CSI>().Property(e => e.DoctorContactNo).IsUnicode(false);
			modelBuilder.Entity<CSI>().Property(e => e.AllergyNotes).IsUnicode(false);
			modelBuilder.Entity<CSI>().HasMany(e => e.CarePlans).WithRequired(e => e.CSI).WillCascadeOnDelete(false);
			modelBuilder.Entity<CSIDomainScore>().Property(e => e.Comments).IsUnicode(false);
			modelBuilder.Entity<CSIDomainSupportService>().Property(e => e.OtherService).IsUnicode(false);
			modelBuilder.Entity<CSIDomainSupportService>().Property(e => e.OtherServiceProvider).IsUnicode(false);
			modelBuilder.Entity<CSIEvent>().Property(e => e.Comments).IsUnicode(false);
			modelBuilder.Entity<Disability>().Property(e => e.Description).IsUnicode(false);
			modelBuilder.Entity<District>().Property(e => e.Description).IsUnicode(false);
			modelBuilder.Entity<DomainEntity>().Property(e => e.Description).IsUnicode(false);
			modelBuilder.Entity<Drug>().Property(e => e.DrugName).IsUnicode(false);
			modelBuilder.Entity<Drug>().Property(e => e.Acronym).IsUnicode(false);
			modelBuilder.Entity<Drug>().HasMany(e => e.DrugDoses).WithRequired(e => e.Drug).WillCascadeOnDelete(false);
			modelBuilder.Entity<DrugDose>().Property(e => e.WeightFrom).HasPrecision(3, 1);
			modelBuilder.Entity<DrugDose>().Property(e => e.WeightTo).HasPrecision(3, 1);
			modelBuilder.Entity<DrugDose>().Property(e => e.Dose).HasPrecision(2, 1);
			modelBuilder.Entity<Encounter>().Property(e => e.Height).HasPrecision(10, 2);
			modelBuilder.Entity<Encounter>().Property(e => e.Weight).HasPrecision(10, 2);
			modelBuilder.Entity<Encounter>().Property(e => e.BMI).HasPrecision(10, 2);
			modelBuilder.Entity<Encounter>().Property(e => e.CD4Known).IsFixedLength().IsUnicode(false);
			modelBuilder.Entity<Encounter>().Property(e => e.CD4Requested).IsFixedLength().IsUnicode(false);
			modelBuilder.Entity<Encounter>().Property(e => e.CD4OrderNo).IsUnicode(false);
			modelBuilder.Entity<Encounter>().Property(e => e.AttendCaregiverDay).IsFixedLength().IsUnicode(false);
			modelBuilder.Entity<Encounter>().Property(e => e.FeelFamilySupported).IsFixedLength().IsUnicode(false);
			modelBuilder.Entity<Encounter>().Property(e => e.ExtremeWeatherGearUse).IsFixedLength().IsUnicode(false);
			modelBuilder.Entity<Encounter>().Property(e => e.Hypothermia).IsFixedLength().IsUnicode(false);
			modelBuilder.Entity<Encounter>().Property(e => e.Frostbite).IsFixedLength().IsUnicode(false);
			modelBuilder.Entity<Encounter>().Property(e => e.AllAdherenceRate).HasPrecision(10, 2);
			modelBuilder.Entity<Encounter>().Property(e => e.ARVAdherenceRate).HasPrecision(10, 2);
			modelBuilder.Entity<EncounterDrug>().Property(e => e.AdherenceRate).HasPrecision(10, 2);
			modelBuilder.Entity<EncounterType>().Property(e => e.Description).IsUnicode(false);
			modelBuilder.Entity<Event>().Property(e => e.Description).IsUnicode(false);
			modelBuilder.Entity<Field>().Property(e => e.FieldCode).IsFixedLength().IsUnicode(false);
			modelBuilder.Entity<Field>().Property(e => e.Description).IsUnicode(false);
			modelBuilder.Entity<Form>().Property(e => e.FormName).IsUnicode(false);
			modelBuilder.Entity<Form>().HasMany(e => e.FormFields).WithRequired(e => e.Form).WillCascadeOnDelete(false);
			modelBuilder.Entity<FormField>().Property(e => e.Value).IsUnicode(false);
			modelBuilder.Entity<GuardianRelation>().Property(e => e.Description).IsUnicode(false);
			modelBuilder.Entity<Guideline>().Property(e => e.DescriptionEnglish).IsUnicode(false);
			modelBuilder.Entity<Guideline>().Property(e => e.DescriptionSesotho).IsUnicode(false);
			modelBuilder.Entity<Language>().Property(e => e.Description).IsUnicode(false);
			modelBuilder.Entity<Outcome>().Property(e => e.Code).IsUnicode(false);
			modelBuilder.Entity<Outcome>().Property(e => e.Description).IsUnicode(false);
			modelBuilder.Entity<OVCType>().Property(e => e.Description).IsUnicode(false);
			modelBuilder.Entity<Partner>().Property(e => e.Name).IsUnicode(false);
			modelBuilder.Entity<Partner>().Property(e => e.Address).IsUnicode(false);
			modelBuilder.Entity<Partner>().Property(e => e.ContactNo).IsUnicode(false);
			modelBuilder.Entity<Partner>().Property(e => e.FaxNo).IsUnicode(false);
			modelBuilder.Entity<Partner>().Property(e => e.ContactName).IsUnicode(false);
			modelBuilder.Entity<PartnerProject>().HasMany(e => e.PartnerProjectSnapshots).WithRequired(e => e.PartnerProject).WillCascadeOnDelete(false);
			modelBuilder.Entity<PartnerProject>().HasMany(e => e.SiteProgresses).WithRequired(e => e.PartnerProject).WillCascadeOnDelete(false);
			modelBuilder.Entity<PartnerProject>().HasMany(e => e.TrainingEvents).WithRequired(e => e.PartnerProject).WillCascadeOnDelete(false);
			modelBuilder.Entity<PartnerProjectSnapshot>().HasMany(e => e.Snapshots).WithRequired(e => e.PartnerProjectSnapshot).WillCascadeOnDelete(false);
			modelBuilder.Entity<Project>().Property(e => e.ProjectName).IsUnicode(false);
			modelBuilder.Entity<ProjectTrackerItem>().Property(e => e.Numerator).IsUnicode(false);
			modelBuilder.Entity<ProjectTrackerItem>().Property(e => e.Denominator).IsUnicode(false);
			modelBuilder.Entity<ProjectTrackerItem>().Property(e => e.List).IsUnicode(false);
			modelBuilder.Entity<ProjectTrackerItem>().HasMany(e => e.ProjectTrackerItemValues).WithRequired(e => e.ProjectTrackerItem).WillCascadeOnDelete(false);
			modelBuilder.Entity<ProjectTrackerItemValue>().Property(e => e.Value).HasPrecision(6, 2);
			modelBuilder.Entity<ProjectTrackerItemValue>().Property(e => e.Denominator).HasPrecision(6, 2);
			modelBuilder.Entity<ProjectTrackerItemValue>().Property(e => e.Numerator).HasPrecision(6, 2);
			modelBuilder.Entity<Question>().Property(e => e.Description).IsUnicode(false);
			modelBuilder.Entity<RecipientType>().Property(e => e.Description).IsUnicode(false);
			modelBuilder.Entity<RegistrationType>().Property(e => e.Description).IsUnicode(false);
			modelBuilder.Entity<SchoolLevel>().Property(e => e.Description).IsUnicode(false);
			modelBuilder.Entity<ScoreType>().HasMany(e => e.Answers).WithRequired(e => e.Score).WillCascadeOnDelete(false);
			modelBuilder.Entity<ServiceProvider>().Property(e => e.Description).IsUnicode(false);
			modelBuilder.Entity<ServiceProvider>().Property(e => e.InformationURL).IsUnicode(false);
			modelBuilder.Entity<ServiceProviderType>().Property(e => e.Description).IsUnicode(false);
			modelBuilder.Entity<Site>().Property(e => e.SiteType).IsUnicode(false);
			modelBuilder.Entity<SiteProgress>().Property(e => e.PreparedBy).IsUnicode(false);
			modelBuilder.Entity<SiteProgress>().Property(e => e.ProjectSchedule).IsUnicode(false);
			modelBuilder.Entity<SiteProgress>().Property(e => e.Resources).IsUnicode(false);
			modelBuilder.Entity<SiteProgress>().Property(e => e.Comments).IsUnicode(false);
			modelBuilder.Entity<SiteProgress>().HasMany(e => e.SiteProgressItems).WithRequired(e => e.SiteProgress).WillCascadeOnDelete(false);
			modelBuilder.Entity<SiteProgressItem>().Property(e => e.Detail).IsUnicode(false);
			modelBuilder.Entity<SiteProgressItemType>().Property(e => e.Description).IsUnicode(false);
			modelBuilder.Entity<Snapshot>().Property(e => e.Value).IsUnicode(false);
			modelBuilder.Entity<Source>().Property(e => e.Description).IsUnicode(false);
			modelBuilder.Entity<SupportServiceType>().Property(e => e.Description).IsUnicode(false);
			modelBuilder.Entity<TrackerItem>().Property(e => e.Description).IsUnicode(false);
			modelBuilder.Entity<TrackerItem>().HasMany(e => e.ProjectTrackerItems).WithRequired(e => e.TrackerItem).WillCascadeOnDelete(false);
			modelBuilder.Entity<TrackerItem>().HasMany(e => e.Snapshots).WithRequired(e => e.TrackerItem).WillCascadeOnDelete(false);
			modelBuilder.Entity<Training>().Property(e => e.Description).IsUnicode(false);
			modelBuilder.Entity<Training>().HasMany(e => e.TrainingEvents).WithRequired(e => e.Training).WillCascadeOnDelete(false);
			modelBuilder.Entity<Training>().HasMany(e => e.TrainingItems).WithRequired(e => e.Training).WillCascadeOnDelete(false);
			modelBuilder.Entity<TrainingItem>().Property(e => e.Description).IsUnicode(false);
			modelBuilder.Entity<Village>().Property(e => e.Name).IsUnicode(false);
			modelBuilder.Entity<Ward>().Property(e => e.Description).IsUnicode(false);
			modelBuilder.Entity<Household>().HasRequired(h => h.FamilyOriginRef).WithMany().HasForeignKey(h => h.FamilyOriginRefId).WillCascadeOnDelete(false);
			modelBuilder.Entity<Household>().HasRequired(h => h.FamilyHead).WithMany().HasForeignKey(h => h.FamilyHeadId).WillCascadeOnDelete(false);
            modelBuilder.Entity<ChildStatusHistory>().HasOptional(c => c.ChildStatus).WithMany().HasForeignKey(c => c.ChildStatusID);
			modelBuilder.Entity<ChildStatusHistory>().HasOptional(c => c.CreatedUser).WithMany().HasForeignKey(c => c.CreatedUserID);
			modelBuilder.Entity<Adult>().HasRequired(a => a.KinshipToFamilyHead).WithMany().HasForeignKey(a => a.KinshipToFamilyHeadID).WillCascadeOnDelete(false);
			modelBuilder.Entity<Child>().HasRequired(a => a.KinshipToFamilyHead).WithMany().HasForeignKey(a => a.KinshipToFamilyHeadID).WillCascadeOnDelete(false);
            modelBuilder.Entity<Child>().HasRequired(a => a.OVCType).WithMany().HasForeignKey(a => a.OVCTypeID).WillCascadeOnDelete(false);
            modelBuilder.Entity<ChildStatusHistory>().HasOptional(c => c.LastUpdatedUser).WithMany().HasForeignKey(c => c.LastUpdatedUserID);
            modelBuilder.Entity<Household>().HasOptional(c => c.CreatedUser).WithMany().HasForeignKey(c => c.CreatedUserID);
            modelBuilder.Entity<Household>().HasOptional(c => c.LastUpdatedUser).WithMany().HasForeignKey(c => c.LastUpdatedUserID);
            modelBuilder.Entity<Child>().HasOptional(c => c.CreatedUser).WithMany().HasForeignKey(c => c.CreatedUserID);
            modelBuilder.Entity<Child>().HasOptional(c => c.LastUpdatedUser).WithMany().HasForeignKey(c => c.LastUpdatedUserID);
            modelBuilder.Entity<Adult>().HasOptional(c => c.CreatedUser).WithMany().HasForeignKey(c => c.CreatedUserID);
            modelBuilder.Entity<Adult>().HasOptional(c => c.LastUpdatedUser).WithMany().HasForeignKey(c => c.LastUpdatedUserID);
            modelBuilder.Entity<CSI>().HasOptional(c => c.CreatedUser).WithMany().HasForeignKey(c => c.CreatedUserID);
            modelBuilder.Entity<CSI>().HasOptional(c => c.LastUpdatedUser).WithMany().HasForeignKey(c => c.LastUpdatedUserID);
            modelBuilder.Entity<CSIDomainScore>().HasOptional(c => c.CreatedUser).WithMany().HasForeignKey(c => c.CreatedUserID);
            modelBuilder.Entity<CSIDomainScore>().HasOptional(c => c.LastUpdatedUser).WithMany().HasForeignKey(c => c.LastUpdatedUserID);
            modelBuilder.Entity<CSIDomain>().HasOptional(c => c.CreatedUser).WithMany().HasForeignKey(c => c.CreatedUserID);
            modelBuilder.Entity<CSIDomain>().HasOptional(c => c.LastUpdatedUser).WithMany().HasForeignKey(c => c.LastUpdatedUserID);
            modelBuilder.Entity<CarePlan>().HasOptional(c => c.CreatedUser).WithMany().HasForeignKey(c => c.CreatedUserID);
            modelBuilder.Entity<CarePlan>().HasOptional(c => c.LastUpdatedUser).WithMany().HasForeignKey(c => c.LastUpdatedUserID);
            modelBuilder.Entity<CarePlanDomain>().HasOptional(c => c.CreatedUser).WithMany().HasForeignKey(c => c.CreatedUserID);
            modelBuilder.Entity<CarePlanDomain>().HasOptional(c => c.LastUpdatedUser).WithMany().HasForeignKey(c => c.LastUpdatedUserID);
            modelBuilder.Entity<CarePlanDomainSupportService>().HasOptional(c => c.CreatedUser).WithMany().HasForeignKey(c => c.CreatedUserID);
            modelBuilder.Entity<CarePlanDomainSupportService>().HasOptional(c => c.LastUpdatedUser).WithMany().HasForeignKey(c => c.LastUpdatedUserID);
            modelBuilder.Entity<Partner>().HasOptional(c => c.CreatedUser).WithMany().HasForeignKey(c => c.CreatedUserID);
            modelBuilder.Entity<Partner>().HasOptional(c => c.LastUpdatedUser).WithMany().HasForeignKey(c => c.LastUpdatedUserID);
            modelBuilder.Entity<Reference>().HasOptional(c => c.CreatedUser).WithMany().HasForeignKey(c => c.CreatedUserID);
            modelBuilder.Entity<Reference>().HasOptional(c => c.LastUpdatedUser).WithMany().HasForeignKey(c => c.LastUpdatedUserID);
            modelBuilder.Entity<ReferenceService>().HasOptional(c => c.CreatedUser).WithMany().HasForeignKey(c => c.CreatedUserID);
            modelBuilder.Entity<ReferenceService>().HasOptional(c => c.LastUpdatedUser).WithMany().HasForeignKey(c => c.LastUpdatedUserID);
            modelBuilder.Entity<RoutineVisit>().HasOptional(c => c.CreatedUser).WithMany().HasForeignKey(c => c.CreatedUserID);
            modelBuilder.Entity<RoutineVisit>().HasOptional(c => c.LastUpdatedUser).WithMany().HasForeignKey(c => c.LastUpdatedUserID);
            modelBuilder.Entity<RoutineVisitMember>().HasOptional(c => c.CreatedUser).WithMany().HasForeignKey(c => c.CreatedUserID);
            modelBuilder.Entity<RoutineVisitMember>().HasOptional(c => c.LastUpdatedUser).WithMany().HasForeignKey(c => c.LastUpdatedUserID);
            modelBuilder.Entity<RoutineVisitSupport>().HasOptional(c => c.CreatedUser).WithMany().HasForeignKey(c => c.CreatedUserID);
            modelBuilder.Entity<RoutineVisitSupport>().HasOptional(c => c.LastUpdatedUser).WithMany().HasForeignKey(c => c.LastUpdatedUserID);
            modelBuilder.Entity<SimpleEntity>().HasOptional(c => c.CreatedUser).WithMany().HasForeignKey(c => c.CreatedUserID);
            modelBuilder.Entity<SimpleEntity>().HasOptional(c => c.LastUpdatedUser).WithMany().HasForeignKey(c => c.LastUpdatedUserID);
            modelBuilder.Entity<SiteGoal>().HasOptional(c => c.CreatedUser).WithMany().HasForeignKey(c => c.CreatedUserID);
            modelBuilder.Entity<SiteGoal>().HasOptional(c => c.LastUpdatedUser).WithMany().HasForeignKey(c => c.LastUpdatedUserID);
            modelBuilder.Entity<VPPS.CSI.Domain.Task>().HasOptional(c => c.CreatedUser).WithMany().HasForeignKey(c => c.CreatedUserID);
            modelBuilder.Entity<VPPS.CSI.Domain.Task>().HasOptional(c => c.LastUpdatedUser).WithMany().HasForeignKey(c => c.LastUpdatedUserID);
            modelBuilder.Entity<Aid>().HasOptional(c => c.CreatedUser).WithMany().HasForeignKey(c => c.CreatedUserID);
            modelBuilder.Entity<Aid>().HasOptional(c => c.LastUpdatedUser).WithMany().HasForeignKey(c => c.LastUpdatedUserID);
            modelBuilder.Entity<ChildStatus>().HasOptional(c => c.CreatedUser).WithMany().HasForeignKey(c => c.CreatedUserID);
            modelBuilder.Entity<ChildStatus>().HasOptional(c => c.LastUpdatedUser).WithMany().HasForeignKey(c => c.LastUpdatedUserID);
            modelBuilder.Entity<OrgUnit>().HasOptional(c => c.CreatedUser).WithMany().HasForeignKey(c => c.CreatedUserID);
            modelBuilder.Entity<OrgUnit>().HasOptional(c => c.LastUpdatedUser).WithMany().HasForeignKey(c => c.LastUpdatedUserID);
            modelBuilder.Entity<OrgUnitType>().HasOptional(c => c.CreatedUser).WithMany().HasForeignKey(c => c.CreatedUserID);
            modelBuilder.Entity<OrgUnitType>().HasOptional(c => c.LastUpdatedUser).WithMany().HasForeignKey(c => c.LastUpdatedUserID);
            modelBuilder.Entity<ReportData>().HasOptional(c => c.CreatedUser).WithMany().HasForeignKey(c => c.CreatedUserID);
            modelBuilder.Entity<ReportData>().HasOptional(c => c.LastUpdatedUser).WithMany().HasForeignKey(c => c.LastUpdatedUserID);
            modelBuilder.Entity<Query>().HasOptional(c => c.CreatedUser).WithMany().HasForeignKey(c => c.CreatedUserID);
            modelBuilder.Entity<Query>().HasOptional(c => c.LastUpdatedUser).WithMany().HasForeignKey(c => c.LastUpdatedUserID);
            modelBuilder.Entity<HouseholdSupportPlan>().HasOptional(c => c.CreatedUser).WithMany().HasForeignKey(c => c.CreatedUserID);
            modelBuilder.Entity<HouseholdSupportPlan>().HasOptional(c => c.LastUpdatedUser).WithMany().HasForeignKey(c => c.LastUpdatedUserID);
            modelBuilder.Entity<HouseholdStatus>().HasOptional(c => c.CreatedUser).WithMany().HasForeignKey(c => c.CreatedUserID);
            modelBuilder.Entity<HouseholdStatus>().HasOptional(c => c.LastUpdatedUser).WithMany().HasForeignKey(c => c.LastUpdatedUserID);
            modelBuilder.Entity<HouseholdStatusHistory>().HasOptional(c => c.HouseholdStatus).WithMany().HasForeignKey(c => c.HouseholdStatusID);
            modelBuilder.Entity<HouseholdStatusHistory>().HasOptional(c => c.CreatedUser).WithMany().HasForeignKey(c => c.CreatedUserID);
            modelBuilder.Entity<HouseholdStatusHistory>().HasOptional(c => c.LastUpdatedUser).WithMany().HasForeignKey(c => c.LastUpdatedUserID);
            modelBuilder.Entity<Beneficiary>().HasRequired(a => a.KinshipToFamilyHead).WithMany().HasForeignKey(a => a.KinshipToFamilyHeadID).WillCascadeOnDelete(false);
            modelBuilder.Entity<Beneficiary>().HasOptional(a => a.OVCType).WithMany().HasForeignKey(a => a.OVCTypeID).WillCascadeOnDelete(false);
            modelBuilder.Entity<Beneficiary>().HasOptional(c => c.CreatedUser).WithMany().HasForeignKey(c => c.CreatedUserID);
            modelBuilder.Entity<Beneficiary>().HasOptional(c => c.LastUpdatedUser).WithMany().HasForeignKey(c => c.LastUpdatedUserID);
            modelBuilder.Entity<Beneficiary>().HasOptional(b => b.ServicesStatus).WithMany().HasForeignKey(b => b.ServicesStatusID).WillCascadeOnDelete(false);
            modelBuilder.Entity<RoutineVisitSupport>().HasOptional(h => h.Support).WithMany().HasForeignKey(h => h.SupportID).WillCascadeOnDelete(false);
            modelBuilder.Entity<CSIDomain>().HasOptional(h => h.Domain).WithMany().HasForeignKey(h => h.DomainID).WillCascadeOnDelete(false);
        }
    }
}
