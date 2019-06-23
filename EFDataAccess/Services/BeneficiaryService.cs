using EFDataAccess.UOW;
using System;
using System.Collections.Generic;
using System.Linq;
using VPPS.CSI.Domain;
using EFDataAccess.DTO;
using System.Data.SqlClient;
using System.Data;
using System.Data.Entity;
using EFDataAccess.Services.TrimesterServices;

namespace EFDataAccess.Services
{
    public class BeneficiaryService : BaseService
    {
        private UserService UserService;
        private AdultService AdultService;
        private HouseholdService HouseholdService;
        private HIVStatusService HIVStatusService;
        private HIVStatusQueryService HIVStatusQueryService;
        private readonly TrimesterQueryService TrimesterQueryService;
        private Repository.IRepository<ReferenceService> ReferenceServiceRepository;
        private Repository.IRepository<RoutineVisitMember> RoutineVisitMemberRepository;
        private Repository.IRepository<SimpleEntity> SimpleEntityRepository;

        public BeneficiaryService(UnitOfWork uow) : base(uow)
        {
            UserService = new UserService(uow);
            AdultService = new AdultService(uow);
            HouseholdService = new HouseholdService(uow);
            HIVStatusService = new HIVStatusService(uow);
            HIVStatusQueryService = new HIVStatusQueryService(uow);
            ReferenceServiceRepository = uow.Repository<ReferenceService>();
            RoutineVisitMemberRepository = uow.Repository<RoutineVisitMember>();
            SimpleEntityRepository = uow.Repository<SimpleEntity>();
            TrimesterQueryService = new TrimesterQueryService(uow);
        }

        private EFDataAccess.Repository.IRepository<Beneficiary> BeneficiaryRepository => UnitOfWork.Repository<Beneficiary>();

        public int CountBySite(Site site)
        {
            return UnitOfWork.DbContext.Database.SqlQuery<int>(
                @"select Count(*) from Beneficiary b 
                inner join HouseHold hh on (hh.HouseHoldID = b.HouseHoldID)
                inner join Partner p on (hh.PartnerID = p.PartnerID)
                where b.Type = 'Beneficiary' and BeneficiaryState != 'Adulto' 
                and (p.SiteID = @SiteID or @SiteID = 0)",
                new SqlParameter("SiteID", site.SiteID)).SingleOrDefault();
        }

        public int countBeneficiariesByType(List<Beneficiary> benList, string ageGroup)
        {
            List<Beneficiary> beneficiaryList = benList;

            int count = 0;
            foreach (Beneficiary b in beneficiaryList)
            {
                if (b.AgeInYears <= 17 && ageGroup.Equals("child")) { count++; }
                else if (b.AgeInYears > 17 && ageGroup.Equals("adult")) { count++; }
            }
            return count;
        }

        public Beneficiary findBeneficiaryBySyncGuid(Guid BeneficiaryGuid) { return BeneficiaryRepository.GetAll().Where(c => c.SyncGuid == BeneficiaryGuid).FirstOrDefault(); }

        public List<UniqueEntity> FindAllBeneficiaryUniqueEntities() => UnitOfWork.DbContext.Database.SqlQuery<UniqueEntity>("Select ISNULL(SyncGuid, cast(cast(0 as binary) as uniqueidentifier)) As SyncGuid, BeneficiaryID As ID from Beneficiary").ToList();

        public List<UniqueEntity> FindAllServicesStatusUniqueEntity() => UnitOfWork.DbContext.Database.SqlQuery<UniqueEntity>("Select ISNULL(SyncGuid, cast(cast(0 as binary) as uniqueidentifier)) As SyncGuid, ServicesStatusID As ID from ServicesStatus").ToList();

        public List<Beneficiary> FindAllBeneficiaries() { return BeneficiaryRepository.GetAll().AsNoTracking().ToList(); }

        public int countBeneficiariesByServiceStatusCodeAndAgeGroup(string serviceStatusCode, string ageGroup, List<Beneficiary> benList)
        {
            SimpleEntity serviceStatus = SimpleEntityRepository.Get().Where(x => x.Code == serviceStatusCode && x.Type == "ben-services-status").SingleOrDefault();

            List<Beneficiary> beneficiaryList = benList.Where(x => x.ServicesStatusID == serviceStatus.SimpleEntityID).ToList();

            int count = 0;
            foreach (Beneficiary b in beneficiaryList)
            {
                if (b.AgeInYears <= 17 && ageGroup.Equals("child")) { count++; }
                else if(b.AgeInYears > 17 && ageGroup.Equals("adult")) { count++; }
                else if (ageGroup.Trim() == "") { count++; }
            }
            return count;
        }

        public int CountBeneficiariesByAgeGroup(string ageGroup, List<Beneficiary> benList)
        {
            List<Beneficiary> beneficiaryList = benList;

            int count = 0;
            foreach (Beneficiary b in beneficiaryList)
            {
                if (b.AgeInYears <= 17 && ageGroup.Equals("child")) { count++; }
                else if (b.AgeInYears > 17 && ageGroup.Equals("adult")) { count++; }
                else if (ageGroup.Trim() == "") { count++; }
            }
            return count;
        }

        public Beneficiary FetchById(int id) { return BeneficiaryRepository.GetAll().Where(x => x.BeneficiaryID == id).Include(x => x.HIVStatus).Include(x => x.Household).Include(x => x.KinshipToFamilyHead).Include(x => x.OVCType).FirstOrDefault(); }

        public IQueryable<Beneficiary> findBeneficiaryByname(string name)
        {
            return UnitOfWork.Repository<Beneficiary>()
                .GetAll()
                .Where(c => c.FirstName.Contains(name) || c.LastName.Contains(name))
                .Include(e => e.Household);
        }

        public List<Beneficiary> getAllBeneficiariesInInitialStatus(DateTime date)
        {
            List<Beneficiary> benList = BeneficiaryRepository
                .GetAll().Where(x => x.HouseholdID != null)
                .Include(e => e.ChildStatusHistories.Select(x => x.ChildStatus))
                .Where(e => e.ChildStatusHistories.OrderByDescending(x => x.EffectiveDate < date)
                .ThenByDescending(x => x.ChildStatusHistoryID).FirstOrDefault().ChildStatus.Description == "Inicial").ToList();

            return benList;
        }

        public int findAllBeneficiariesByStatus(DateTime date, string status, string ageGroup)
        {
            List<Beneficiary> beneficiaryList = 
                BeneficiaryRepository.GetAll()
                .Where(x => x.HouseholdID != null)
                .Include(e => e.ChildStatusHistories.Select(x => x.ChildStatus))
                .Where(e => e.ChildStatusHistories.OrderByDescending(x => x.EffectiveDate <= date)
                .ThenByDescending(x => x.ChildStatusHistoryID).FirstOrDefault().ChildStatus.Description == status).ToList();

            int count = 0;
            foreach (Beneficiary b in beneficiaryList)
            {
                if (b.AgeInYears <= 17 && ageGroup.Equals("child")) { count++; }
                else if (b.AgeInYears > 17 && ageGroup.Equals("adult")) { count++; }
                else if (ageGroup.Trim() == "") { count++; }
            }
            return count;

        }

        public List<Beneficiary> findAllHIVPositiveInTreatmentBeneficiariesByStatus(DateTime date, string status)
        {
            return BeneficiaryRepository
                    .GetAll()
                    .Where(x => x.HouseholdID != null
                            && x.HIVStatus.HIV == HIVStatus.POSITIVE
                            && x.HIVStatus.HIVInTreatment == HIVStatus.IN_TARV
                    )
                    .Include(e => e.ChildStatusHistories.Select(x => x.ChildStatus))
                    .Where(e => e.ChildStatusHistories
                                    .OrderByDescending(x => x.EffectiveDate < date)
                                    .ThenByDescending(x => x.ChildStatusHistoryID)
                                    .FirstOrDefault().ChildStatus.Description == status
                    )
                    .ToList();
        }

        public List<Beneficiary> findAllHIVPositiveBeneficiariesBy(DateTime date)
        {
            return BeneficiaryRepository
                    .GetAll()
                    .Where(x => x.HouseholdID != null
                            && x.HIVStatus.HIV == HIVStatus.POSITIVE
                    )
                    .ToList();
        }

        public List<Beneficiary> findAllBeneficiariesThatKnowTheirHIVStatusByStatus(DateTime date, string status)
        {
            return BeneficiaryRepository
                    .GetAll()
                    .Where(x => x.HouseholdID != null
                            && (x.HIVStatus.HIV == HIVStatus.POSITIVE
                                    || x.HIVStatus.HIV == HIVStatus.NEGATIVE
                                    || x.HIVStatus.HIVUndisclosedReason == HIVStatus.NOT_RECOMMENDED)
                    )
                    .Include(e => e.ChildStatusHistories.Select(x => x.ChildStatus))
                    .Where(e => e.ChildStatusHistories
                                    .OrderByDescending(x => x.EffectiveDate <= date)
                                    .ThenByDescending(x => x.ChildStatusHistoryID)
                                    .FirstOrDefault().ChildStatus.Description == status
                    )
                    .ToList();
        }

        public List<Beneficiary> getAllBeneficiariesWithServiceStatus()
        {
            List<Beneficiary> benList = BeneficiaryRepository
                .GetAll().Where(x => x.ServicesStatusID != null).ToList();

            return benList;
        }

        public List<Beneficiary> getAllBeneficiariesWithServiceStatusForReports()
        {
            List<Beneficiary> benList = BeneficiaryRepository
                .GetAll().Where(x => x.ServicesStatusID != null).ToList();

            return benList;
        }


        public IQueryable<Beneficiary> findBeneficiaryBynameAndPartnerAndCode(string name, string partnerName, string beneficiaryCode)
        {
            return UnitOfWork.Repository<Beneficiary>()
                .GetAll()
                .Where(c => c.FirstName.Contains(name) || c.LastName.Contains(name))
                .Where(e => (name == null || name.Equals("") || e.FirstName.Contains(name) || e.LastName.Contains(name))
                && (partnerName == null || partnerName.Equals("") || e.Household.Partner.Name.Contains(partnerName))
                && (beneficiaryCode == null || beneficiaryCode.Equals("") || e.Code.Contains(beneficiaryCode)))
                .Include(e => e.Household)
                .Include(e => e.ChildStatusHistories.Select(x => x.ChildStatus));

        }

        public List<Beneficiary> FetchByHouseholdIDandStatusID(int householdID, int StatusID)
        {
            return BeneficiaryRepository.GetAll()
                .Include(e => e.ChildStatusHistories.Select(x => x.ChildStatus))
                .Where(e => e.ChildStatusHistories.OrderByDescending(x => x.EffectiveDate)
                .ThenByDescending(x => x.ChildStatusHistoryID).FirstOrDefault().ChildStatus.Description != "Adulto")
                .Where(e => e.ChildStatusHistories.OrderByDescending(x => x.EffectiveDate)
                .ThenByDescending(x => x.ChildStatusHistoryID).FirstOrDefault().ChildStatus.StatusID != StatusID)
                .Where(x => x.HouseholdID == householdID).ToList();
        }

        public Beneficiary FetchByBeneficiaryIDandStatusID(int BeneficiaryID, int StatusID)
        {
            return BeneficiaryRepository.GetAll()
                .Include(e => e.ChildStatusHistories.Select(x => x.ChildStatus))
                .Where(e => e.ChildStatusHistories.OrderByDescending(x => x.EffectiveDate)
                .ThenByDescending(x => x.ChildStatusHistoryID).FirstOrDefault().ChildStatus.Description != "Adulto")
                .Where(e => e.ChildStatusHistories.OrderByDescending(x => x.EffectiveDate)
                .ThenByDescending(x => x.ChildStatusHistoryID).FirstOrDefault().ChildStatus.StatusID != StatusID)
                .Where(x => x.BeneficiaryID == BeneficiaryID).FirstOrDefault();
        }

        public Beneficiary Reload(Beneficiary entity) { BeneficiaryRepository.FullReload(entity); return entity; }

        public void Delete(Beneficiary Beneficiary)
        {
            UnitOfWork.Repository<HIVStatus>().GetAll().Where(x => x.BeneficiaryID == Beneficiary.BeneficiaryID).ToList().ForEach(x => UnitOfWork.Repository<HIVStatus>().Delete(x));
            UnitOfWork.Repository<ChildStatusHistory>().GetAll().Where(x => x.BeneficiaryID == Beneficiary.BeneficiaryID).ToList().ForEach(x => UnitOfWork.Repository<ChildStatusHistory>().Delete(x));
            BeneficiaryRepository.Delete(Beneficiary);
        }

        public Beneficiary Get(int BeneficiaryID)
        {
            return UnitOfWork.Repository<Beneficiary>().GetAll()
                .Where(c => c.BeneficiaryID == BeneficiaryID)
                .Include(e => e.Household).SingleOrDefault();
        }

        public void Save(Beneficiary Beneficiary)
        {
            if (Beneficiary.BeneficiaryID > 0)
            { BeneficiaryRepository.Update(Beneficiary); }
            else
            { BeneficiaryRepository.Add(Beneficiary); }
        }

        public int ImportData(string path)
        {
            _logger.Information("IMPORTACAO DE BENEFICIARIOS ...");

            UnitOfWork.DbContext.Configuration.AutoDetectChangesEnabled = false;
            FileImporter imp = new FileImporter();
            string fullPath = path + @"\Beneficiaries.csv";

            int ImportedBeneficiarys = 0;
            string lastGuidToImport = null;
            List<Beneficiary> BeneficiariesToPersist = new List<Beneficiary>();
            List<UniqueEntity> BeneficiaryDB = FindAllBeneficiaryUniqueEntities();
            List<UniqueEntity> HouseholdsDB = HouseholdService.findAllHouseholdUniqueEntities();
            List<UniqueEntity> HIVStatusDB = HIVStatusQueryService.FindAllHIVStatusUniqueEntity();
            List<UniqueEntity> ServicesStatusDB = FindAllServicesStatusUniqueEntity();
            List<SimpleEntity> BenServiceStatusDB = UnitOfWork.DbContext.SimpleEntities.Where(x => x.Type.Equals("ben-services-status")).ToList();
            List<SimpleEntity> KinshipsToFamilyHeadDB = UnitOfWork.DbContext.SimpleEntities.Where(x => x.Type.Equals("degree-of-kinship")).ToList();
            IEnumerable<DataRow> BeneficiaryRows = imp.ImportFromCSV(fullPath).Rows.Cast<DataRow>();
            UsersDB = ConvertListToHashtableUsers(findAllUsersUniqueEntities());

            try
            {
                foreach (DataRow row in BeneficiaryRows)
                {
                    Guid BeneficiaryGuid = new Guid(row["Beneficiary_guid"].ToString());
                    UniqueEntity Household = FindBySyncGuid(HouseholdsDB, new Guid(row["HouseHoldGuid"].ToString()));
                    UniqueEntity HIVStatus = FindBySyncGuid(HIVStatusDB, new Guid(row["HIVStatusGuid"].ToString()));
                    UniqueEntity ServicesStatus = FindBySyncGuid(ServicesStatusDB, new Guid(row["ServicesStatusGuid"].ToString()));

                    if (Household == null)
                    { _logger.Error("Beneficiario com o Guid '{0}' tem HouseHold com Guid '{1}' em falta. Este nao sera importado.", BeneficiaryGuid, row["HouseHoldGuid"].ToString()); }
                    else if (HIVStatus == null)
                    { _logger.Error("Beneficiario com o Guid '{0}' tem HIVStatus com Guid '{1}' em falta. Este nao sera importado.", BeneficiaryGuid, row["HIVStatusGuid"].ToString()); }
                    else
                    {
                        Beneficiary Beneficiary = FindBySyncGuid(BeneficiaryDB, BeneficiaryGuid) == null ? Beneficiary = new Beneficiary() : findBeneficiaryBySyncGuid(BeneficiaryGuid);
                        lastGuidToImport = BeneficiaryGuid.ToString();
                        Beneficiary.Code = row["Code"].ToString();
                        Beneficiary.IDNumber = row["IDNumber"].ToString();
                        Beneficiary.FirstName = row["FirstName"].ToString();
                        Beneficiary.LastName = row["LastName"].ToString();
                        Beneficiary.DateOfBirth = (row["DateOfBirth"].ToString()).Length == 0 ? (DateTime?)null : DateTime.Parse(row["DateOfBirth"].ToString());
                        Beneficiary.DateOfBirthUnknown = row["IsDateOfBirthUnknownInt"].ToString().Equals("1") ? true : false;
                        Beneficiary.RegistrationDate = (row["RegistrationDate"].ToString()).Length == 0 ? (DateTime?)null : DateTime.Parse(row["RegistrationDate"].ToString());
                        Beneficiary.RegistrationDateDifferentFromHouseholdDate = row["IsRegistrationDateDifferentFromHouseholdDateInt"].ToString().Equals("1") ? true : false;
                        Beneficiary.IsPartSavingGroup = row["IsPartSavingGroupInt"].ToString().Equals("1") ? true : false;
                        Beneficiary.HIVTracked = row["IsHIVTrackedInt"].ToString().Equals("1") ? true : false;
                        Beneficiary.Gender = row["Gender"].ToString();
                        Beneficiary.IsHouseHoldChef = row["IsHouseHoldChefInt"].ToString().Equals("1") ? true : false;
                        Beneficiary.NID = row["NID"].ToString();
                        Beneficiary.HouseholdID = Household.ID;
                        Beneficiary.HIVStatusID = HIVStatus.ID;
                        SimpleEntity BenServiceStatus = BenServiceStatusDB.Where(x => x.Code.Equals(row["ServicesStatusCode"])).SingleOrDefault();
                        Beneficiary.ServicesStatusID = ServicesStatus != null ? ServicesStatus.ID : Beneficiary.ServicesStatusID;
                        SimpleEntity KinshipsToFamilyHead = KinshipsToFamilyHeadDB.Where(x => x.Code.Equals(row["KinshipToFamilyHeadCode"])).SingleOrDefault();
                        Beneficiary.KinshipToFamilyHeadID = KinshipsToFamilyHead == null ? Beneficiary.KinshipToFamilyHeadID : KinshipsToFamilyHead.SimpleEntityID;
                        if (row["OtherKinshipToFamilyHead"].ToString().Length > 0) { Beneficiary.OtherKinshipToFamilyHead = row["OtherKinshipToFamilyHead"].ToString(); }
                        if (row["OVCTypeID"].ToString().Length > 0) { Beneficiary.OVCTypeID = int.Parse(row["OVCTypeID"].ToString()); }
                        if (row["OrgUnitID"].ToString().Length > 0) { Beneficiary.OrgUnitID = int.Parse(row["OrgUnitID"].ToString()); }
                        SetCreationDataFields(Beneficiary, row, BeneficiaryGuid);
                        SetUpdatedDataFields(Beneficiary, row);
                        BeneficiariesToPersist.Add(Beneficiary);
                        ImportedBeneficiarys++;
                    }

                    if (ImportedBeneficiarys % 100 == 0)
                    { _logger.Information(ImportedBeneficiarys + " de " + BeneficiaryRows.Count() + " Beneficiarios importados."); }
                }

                BeneficiariesToPersist.ForEach(c => Save(c));
                UnitOfWork.Commit();
                Rename(fullPath, fullPath + IMPORTED);
            }
            catch (Exception e)
            {
                _logger.Information("Erro ao importar o Guid : " + lastGuidToImport);
                _logger.Error(e, "Erro ao importar Criancas", null);
                throw e;
            }

            UnitOfWork.Dispose();
            return BeneficiariesToPersist.Count();
        }

        public bool BeneficiaryValidation(string firstname, string lastname, DateTime? birthdate)
        {
            return (from Beneficiary in UnitOfWork.DbContext.Beneficiaries
                    where Beneficiary.FirstName == firstname
                    && Beneficiary.LastName == lastname
                    && Beneficiary.DateOfBirth == birthdate
                    select Beneficiary.FirstName).Any();
        }

        public int LockData() => LockAndUnlockData(3, new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, 20));

        public int UnlockData(DateTime unlockDate) => LockAndUnlockData(0, unlockDate);

        public int LockAndUnlockData(int state, DateTime lockOrUnlockDate)
        {
            UnitOfWork.DbContext.Configuration.AutoDetectChangesEnabled = false;
            List<Beneficiary> BeneficiaryToLockOrUnlock = UnitOfWork.Repository<Beneficiary>().GetAll().Where(x => x.Household.RegistrationDate <= lockOrUnlockDate).ToList();
            foreach (Beneficiary c in BeneficiaryToLockOrUnlock) { c.State = state; Save(c); }

            Commit();
            UnitOfWork.DbContext.Configuration.AutoDetectChangesEnabled = true;
            return BeneficiaryToLockOrUnlock.Count();
        }

        public void LockByHouseholdStatus(int state, int householdID)
        {
            UnitOfWork.DbContext.Configuration.AutoDetectChangesEnabled = false;

            List<Beneficiary> BeneficiaryToLockOrUnlock = UnitOfWork.Repository<Beneficiary>().GetAll().Where(x => x.HouseholdID == householdID).ToList();

            foreach (Beneficiary c in BeneficiaryToLockOrUnlock) { c.State = state; Save(c); }

            Commit();
            UnitOfWork.DbContext.Configuration.AutoDetectChangesEnabled = true;
            // return BeneficiaryToLockOrUnlock.Count();
        }

        public bool MoreThanTwoAdultsOnHousehold(int householdID)
        {
            List<Beneficiary> beneficiaryList = BeneficiaryRepository.GetAll().Where(x => x.HouseholdID == householdID).AsNoTracking().ToList();
            int countAdults = 0;
            foreach (Beneficiary b in beneficiaryList)
            {
                if (b.AgeInYears >= 18)
                {
                    countAdults++;
                }
            }

            if (countAdults >= 2)
            {
                return true;
            }
            return false;
        }

        public bool MoreThanOneHouseholdChief(string type, int householdID, int beneficiaryID, int NewHouseholdChiefStatus)
        {
            List<Beneficiary> beneficiaryList = BeneficiaryRepository.GetAll()
                .Where(x => x.HouseholdID == householdID
                && x.IsHouseHoldChef == true)
                .AsNoTracking().ToList();

            int beneficiaryCount = beneficiaryList.Count;

            if (type == "create")
            {
                if (beneficiaryCount >= 1 && NewHouseholdChiefStatus == 1)
                {
                    beneficiaryCount++;
                }
            }
            else if (type == "edit")
            {
                foreach (Beneficiary b in beneficiaryList)
                {
                    if (b.BeneficiaryID == beneficiaryID && b.IsHouseHoldChefInt == NewHouseholdChiefStatus)
                    {
                        beneficiaryCount++;
                    }
                    else if ((b.BeneficiaryID == beneficiaryID && b.IsHouseHoldChefInt != NewHouseholdChiefStatus))
                    {
                        beneficiaryCount--;
                    }
                    else if (b.BeneficiaryID != beneficiaryID && NewHouseholdChiefStatus == 1)
                    {
                        beneficiaryCount++;
                    }
                }
            }

            return (beneficiaryCount > 1) ? true : false;

        }


        /*
         * #######################################################
         * ############## BeneficiaryQuestionReport ##############
         * #######################################################
         */

        public List<ChildQuestionSummaryReportDTO> getChildQuestionReport(DateTime initialDate, DateTime lastDate, int partnerID)
        {
            String query = @"SELECT 
		                        Partner.Name AS Partner
		                        ,(FirstName) + ' ' + (LastName) As FullName
		                        , Beneficiary.Gender
		                        , DATEDIFF(YEAR, DateOFBirth, GETDATE()) AS Age
		                        ,CONVERT(varchar,CSI.IndexDate,103) AS CSIDate
		                        --, Beneficiary.BeneficiaryID AS BeneficiaryID
		                        --,Question.Description
		                        --,t.col1 as a,
                                ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='1-' THEN ScoreType.Score  END), -1) As P1
                                ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='2-' THEN ScoreType.Score  END), -1) As P2
                                ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='3-' THEN ScoreType.Score  END), -1) As P3
                                ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='4-' THEN ScoreType.Score  END), -1) As P4
                                ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='5-' THEN ScoreType.Score  END), -1) As P5
                                ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='6-' THEN ScoreType.Score  END), -1) As P6
                                ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='7-' THEN ScoreType.Score  END), -1) As P7
                                ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='8-' THEN ScoreType.Score  END), -1) As P8
                                ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='9-' THEN ScoreType.Score  END), -1) As P9
                                ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='10-' THEN ScoreType.Score  END), -1) As P10
                                ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='11-' THEN ScoreType.Score  END), -1) As P11
                                ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='12-' THEN ScoreType.Score  END), -1) As P12
                                ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='13-' THEN ScoreType.Score  END), -1) As P13
                                ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='14-' THEN ScoreType.Score  END), -1) As P14
                                ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='15-' THEN ScoreType.Score  END), -1) As P15
                                ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='16-' THEN ScoreType.Score  END), -1) As P16
                                ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='17-' THEN ScoreType.Score  END), -1) As P17
                                ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='18-' THEN ScoreType.Score  END), -1) As P18
                                ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='19-' THEN ScoreType.Score  END), -1) As P19
                                ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='20-' THEN ScoreType.Score  END), -1) As P20
                                ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='21-' THEN ScoreType.Score  END), -1) As P21
                                ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='22-' THEN ScoreType.Score  END), -1) As P22
                                ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='23-' THEN ScoreType.Score  END), -1) As P23
                                ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='24-' THEN ScoreType.Score  END), -1) As P24
                                ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='25-' THEN ScoreType.Score  END), -1) As P25
                                ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='26-' THEN ScoreType.Score  END), -1) As P26
                                ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='27-' THEN ScoreType.Score  END), -1) As P27
                                ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='28-' THEN ScoreType.Score  END), -1) As P28
                                ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='29-' THEN ScoreType.Score  END), -1) As P29
                                ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='30-' THEN ScoreType.Score  END), -1) As P30
                                ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='31-' THEN ScoreType.Score  END), -1) As P31
                                ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='32-' THEN ScoreType.Score  END), -1) As P32
                                ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='33-' THEN ScoreType.Score  END), -1) As P33
		                        --, DATENAME(d, CSI.IndexDate) + ' ' + DATENAME(m, CSI.IndexDate) + ' ' + DATENAME(YEAR, CSI.IndexDate) AS CSIDate
		                        --, Domain.Description as Domain
                        FROM 
	                         [Partner]
                        INNER JOIN
	                         [HouseHold] ON [HouseHold].PartnerID = [Partner].PartnerID
                        INNER JOIN
	                         Beneficiary ON [HouseHold].HouseHoldID = Beneficiary.HouseHoldID
                        INNER JOIN 
	                         CSI ON Beneficiary.BeneficiaryID = CSI.BeneficiaryID 
                        INNER JOIN 
	                         CSIDomain ON CSI.CSIID = CSIDomain.CSIID
                        INNER JOIN 
	                         Domain ON Domain.DomainID = CSIDomain.DomainID
                        INNER JOIN 
	                         CSIDomainScore ON CSIDomain.CSIDomainID = CSIDomainScore.CSIDomainID
                        INNER JOIN 
	                         Question ON CSIDomainScore.QuestionID = Question.QuestionID
                        INNER JOIN 
	                         Answer ON CSIDomainScore.AnswerID = Answer.AnswerID
                        INNER JOIN 
	                         ScoreType ON ScoreType.ScoreTypeID = Answer.ScoreID
                        --WHERE CSI.IndexDate >= @initialDate AND CSI.IndexDate <= @lastDate
                        WHERE ([Partner].PartnerID = @partnerID or @partnerID = 0)
                        AND 
                        CSI.CSIID IN
                        (
	
		                        SELECT
			                        obj.CSIID
		                        FROM
		                        (
			                        SELECT 
				                        row_number() OVER (PARTITION BY [BeneficiaryID] ORDER BY IndexDate DESC) AS numeroLinha
				                        --Obter o número da linha de acordo com BeneficiaryID, e ordenado pelo ID do Histórico de forma DESCENDENTE(Último ao Primeiro)
				                        ,CSIID
				                        ,[BeneficiaryID]
				                        ,IndexDate
			                        FROM 
				                         CSI
			                        WHERE CSI.IndexDate >= @initialDate AND CSI.IndexDate <= @lastDate
		                        )
		                        obj
		                        WHERE 
			                        obj.numeroLinha=1
                        )
                        GROUP BY 
	                        Partner.Name
	                        ,Beneficiary.FirstName
	                        ,Beneficiary.LastName
	                        ,Beneficiary.Gender
	                        ,Beneficiary.DateOfBirth
	                        , CSI.IndexDate
                        ORDER BY
	                        Partner.Name";

            return UnitOfWork.DbContext.Database.SqlQuery<ChildQuestionSummaryReportDTO>(query,
                                                            new SqlParameter("initialDate", initialDate),
                                                            new SqlParameter("lastDate", lastDate),
                                                            new SqlParameter("partnerID", partnerID)).ToList();
        }


        /*
         * #######################################################
         * ############## AgreggatedChildQuestionReport ##########
         * #######################################################
         */

        public List<ChildQuestionSummaryReportDTO> getAgreggatedChildQuestionReport(int ProvID, int DistID, int SiteID, DateTime initialDate, DateTime lastDate, int partnerID)
        {
            String query = @"SELECT
	                            prov.Name As Province
	                            ,dist.Name As District
	                            ,s.SiteName As SiteName
	                            ,Beneficiaryquestionobj.Partner
	                            ,Beneficiaryquestionobj.FullName
	                            ,Beneficiaryquestionobj.Gender
	                            ,Beneficiaryquestionobj.Age
	                            ,Beneficiaryquestionobj.CSIDate
	                            ,Beneficiaryquestionobj.P1
	                            ,Beneficiaryquestionobj.P2
	                            ,Beneficiaryquestionobj.P3
	                            ,Beneficiaryquestionobj.P4
	                            ,Beneficiaryquestionobj.P5
	                            ,Beneficiaryquestionobj.P6
	                            ,Beneficiaryquestionobj.P7
	                            ,Beneficiaryquestionobj.P8
	                            ,Beneficiaryquestionobj.P9
	                            ,Beneficiaryquestionobj.P10
	                            ,Beneficiaryquestionobj.P11
	                            ,Beneficiaryquestionobj.P12
	                            ,Beneficiaryquestionobj.P13
	                            ,Beneficiaryquestionobj.P14
	                            ,Beneficiaryquestionobj.P15
	                            ,Beneficiaryquestionobj.P16
	                            ,Beneficiaryquestionobj.P17
	                            ,Beneficiaryquestionobj.P18
	                            ,Beneficiaryquestionobj.P19
	                            ,Beneficiaryquestionobj.P20
	                            ,Beneficiaryquestionobj.P21
	                            ,Beneficiaryquestionobj.P22
	                            ,Beneficiaryquestionobj.P23
	                            ,Beneficiaryquestionobj.P24
	                            ,Beneficiaryquestionobj.P25
	                            ,Beneficiaryquestionobj.P26
	                            ,Beneficiaryquestionobj.P27
	                            ,Beneficiaryquestionobj.P28
	                            ,Beneficiaryquestionobj.P29
	                            ,Beneficiaryquestionobj.P30
	                            ,Beneficiaryquestionobj.P31
	                            ,Beneficiaryquestionobj.P32
	                            ,Beneficiaryquestionobj.P33
                            FROM
	                             [Partner] part
	                            inner join  [Site] s on (part.siteID = s.SiteID)
	                            inner join  [OrgUnit] dist on (dist.OrgUnitID = s.OrgUnitID)
	                            inner join  [OrgUnit] prov on (dist.ParentOrgUnitId = prov.OrgUnitID)
                            INNER JOIN 
                            (
		                            SELECT 
				                            Partner.Name AS Partner
				                            ,(FirstName) + ' ' + (LastName) As FullName
				                            , Beneficiary.Gender
				                            , DATEDIFF(YEAR, DateOFBirth, GETDATE()) AS Age
				                            ,CONVERT(varchar,CSI.IndexDate,103) AS CSIDate
				                            --, Beneficiary.BeneficiaryID AS BeneficiaryID
				                            --,Question.Description
				                            --,t.col1 as a,
				                            ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='1-' THEN ScoreType.Score  END), -1) As P1
				                            ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='2-' THEN ScoreType.Score  END), -1) As P2
				                            ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='3-' THEN ScoreType.Score  END), -1) As P3
				                            ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='4-' THEN ScoreType.Score  END), -1) As P4
				                            ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='5-' THEN ScoreType.Score  END), -1) As P5
				                            ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='6-' THEN ScoreType.Score  END), -1) As P6
				                            ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='7-' THEN ScoreType.Score  END), -1) As P7
				                            ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='8-' THEN ScoreType.Score  END), -1) As P8
				                            ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='9-' THEN ScoreType.Score  END), -1) As P9
				                            ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='10-' THEN ScoreType.Score  END), -1) As P10
				                            ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='11-' THEN ScoreType.Score  END), -1) As P11
				                            ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='12-' THEN ScoreType.Score  END), -1) As P12
				                            ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='13-' THEN ScoreType.Score  END), -1) As P13
				                            ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='14-' THEN ScoreType.Score  END), -1) As P14
				                            ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='15-' THEN ScoreType.Score  END), -1) As P15
				                            ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='16-' THEN ScoreType.Score  END), -1) As P16
				                            ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='17-' THEN ScoreType.Score  END), -1) As P17
				                            ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='18-' THEN ScoreType.Score  END), -1) As P18
				                            ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='19-' THEN ScoreType.Score  END), -1) As P19
				                            ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='20-' THEN ScoreType.Score  END), -1) As P20
				                            ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='21-' THEN ScoreType.Score  END), -1) As P21
				                            ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='22-' THEN ScoreType.Score  END), -1) As P22
				                            ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='23-' THEN ScoreType.Score  END), -1) As P23
				                            ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='24-' THEN ScoreType.Score  END), -1) As P24
				                            ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='25-' THEN ScoreType.Score  END), -1) As P25
				                            ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='26-' THEN ScoreType.Score  END), -1) As P26
				                            ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='27-' THEN ScoreType.Score  END), -1) As P27
				                            ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='28-' THEN ScoreType.Score  END), -1) As P28
				                            ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='29-' THEN ScoreType.Score  END), -1) As P29
				                            ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='30-' THEN ScoreType.Score  END), -1) As P30
				                            ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='31-' THEN ScoreType.Score  END), -1) As P31
				                            ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='32-' THEN ScoreType.Score  END), -1) As P32
				                            ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='33-' THEN ScoreType.Score  END), -1) As P33
				                            --, DATENAME(d, CSI.IndexDate) + ' ' + DATENAME(m, CSI.IndexDate) + ' ' + DATENAME(YEAR, CSI.IndexDate) AS CSIDate
				                            --, Domain.Description as Domain
		                            FROM 
			                             [Partner]
		                            INNER JOIN
			                             [HouseHold] ON [HouseHold].PartnerID = [Partner].PartnerID
		                            INNER JOIN
			                             Beneficiary ON [HouseHold].HouseHoldID = Beneficiary.HouseHoldID
		                            INNER JOIN 
			                             CSI ON Beneficiary.BeneficiaryID = CSI.BeneficiaryID 
		                            INNER JOIN 
			                             CSIDomain ON CSI.CSIID = CSIDomain.CSIID
		                            INNER JOIN 
			                             Domain ON Domain.DomainID = CSIDomain.DomainID
		                            INNER JOIN 
			                             CSIDomainScore ON CSIDomain.CSIDomainID = CSIDomainScore.CSIDomainID
		                            INNER JOIN 
			                             Question ON CSIDomainScore.QuestionID = Question.QuestionID
		                            INNER JOIN 
			                             Answer ON CSIDomainScore.AnswerID = Answer.AnswerID
		                            INNER JOIN 
			                             ScoreType ON ScoreType.ScoreTypeID = Answer.ScoreID
		                            --WHERE CSI.IndexDate >= @initialDate AND CSI.IndexDate <= @lastDate
		                            WHERE ([Partner].PartnerID = @partnerID or @partnerID = 0)
		                            AND 
		                            CSI.CSIID IN
		                            (
	
				                            SELECT
					                            obj.CSIID
				                            FROM
				                            (
					                            SELECT 
						                            row_number() OVER (PARTITION BY [BeneficiaryID] ORDER BY IndexDate DESC) AS numeroLinha
						                            --Obter o número da linha de acordo com BeneficiaryID, e ordenado pelo ID do Histórico de forma DESCENDENTE(Último ao Primeiro)
						                            ,CSIID
						                            ,[BeneficiaryID]
						                            ,IndexDate
					                            FROM 
						                             CSI
					                            WHERE CSI.IndexDate >= @initialDate AND CSI.IndexDate <= @lastDate
				                            )
				                            obj
				                            WHERE 
					                            obj.numeroLinha=1
		                            )
		                            GROUP BY 
			                            Partner.Name
			                            ,Beneficiary.FirstName
			                            ,Beneficiary.LastName
			                            ,Beneficiary.Gender
			                            ,Beneficiary.DateOfBirth
			                            , CSI.IndexDate
		                            --ORDER BY
			                            --Partner.Name
                            )Beneficiaryquestionobj
                            ON Beneficiaryquestionobj.Partner = part.Name
                            WHERE
	                            ((s.SiteID = @SiteID AND prov.OrgUnitID = @ProvID AND dist.OrgUnitID = @DistID) OR
	                            (s.SiteID = @SiteID AND @ProvID = 0 AND @DistID = 0) OR
	                            (@SiteID = 0 AND dist.OrgUnitID = @DistID AND @ProvID = 0) OR 
	                            (@SiteID = 0 AND @DistID = 0 AND prov.OrgUnitID = @ProvID) OR
	                            (@SiteID = 0 AND @DistID = 0 AND @ProvID = 0))
                            GROUP BY 
	                            prov.Name
	                            ,dist.Name
	                            ,s.SiteName
	                            ,Beneficiaryquestionobj.Partner
	                            ,Beneficiaryquestionobj.FullName
	                            ,Beneficiaryquestionobj.Gender
	                            ,Beneficiaryquestionobj.Age
	                            ,Beneficiaryquestionobj.CSIDate
	                            ,Beneficiaryquestionobj.P1
	                            ,Beneficiaryquestionobj.P2
	                            ,Beneficiaryquestionobj.P3
	                            ,Beneficiaryquestionobj.P4
	                            ,Beneficiaryquestionobj.P5
	                            ,Beneficiaryquestionobj.P6
	                            ,Beneficiaryquestionobj.P7
	                            ,Beneficiaryquestionobj.P8
	                            ,Beneficiaryquestionobj.P9
	                            ,Beneficiaryquestionobj.P10
	                            ,Beneficiaryquestionobj.P11
	                            ,Beneficiaryquestionobj.P12
	                            ,Beneficiaryquestionobj.P13
	                            ,Beneficiaryquestionobj.P14
	                            ,Beneficiaryquestionobj.P15
	                            ,Beneficiaryquestionobj.P16
	                            ,Beneficiaryquestionobj.P17
	                            ,Beneficiaryquestionobj.P18
	                            ,Beneficiaryquestionobj.P19
	                            ,Beneficiaryquestionobj.P20
	                            ,Beneficiaryquestionobj.P21
	                            ,Beneficiaryquestionobj.P22
	                            ,Beneficiaryquestionobj.P23
	                            ,Beneficiaryquestionobj.P24
	                            ,Beneficiaryquestionobj.P25
	                            ,Beneficiaryquestionobj.P26
	                            ,Beneficiaryquestionobj.P27
	                            ,Beneficiaryquestionobj.P28
	                            ,Beneficiaryquestionobj.P29
	                            ,Beneficiaryquestionobj.P30
	                            ,Beneficiaryquestionobj.P31
	                            ,Beneficiaryquestionobj.P32
	                            ,Beneficiaryquestionobj.P33
                            ORDER BY 
	                            prov.Name
	                            ,dist.Name
	                            ,s.SiteName
	                            ,Beneficiaryquestionobj.Partner";

            return UnitOfWork.DbContext.Database.SqlQuery<ChildQuestionSummaryReportDTO>(query,
                                                            new SqlParameter("ProvID", ProvID),
                                                            new SqlParameter("DistID", DistID),
                                                            new SqlParameter("SiteID", SiteID),
                                                            new SqlParameter("initialDate", initialDate),
                                                            new SqlParameter("lastDate", lastDate),
                                                            new SqlParameter("partnerID", partnerID)).ToList();
        }



        /*
         * #######################################################
         * ############## ChildByQuestionReport ##############
         * #######################################################
         */

        public List<ChildByQuestionReportDTO> getChildByQuestionReport(DateTime initialDate, DateTime lastDate, int partnerID, int questionID, int scoreTypeID)
        {

            String query = @"SELECT 
	                            Partner.Name As Partner,
	                            (FirstName) + ' ' + (LastName) As FullName,
	                            DATEDIFF(YEAR, DateOFBirth, GETDATE()) AS Age,
                                Child.Gender,
	                            CONVERT(varchar,CSI.IndexDate,103) AS CSIDate,
                                Domain.Description as Domain,
	                            Question.Description,
	                            ScoreType.Score
                            FROM Child 
	                            INNER JOIN HouseHold ON HouseHold.HouseHoldID = CHild.HouseholdID
	                            INNER JOIN Partner ON HouseHold.PartnerID = [Partner].PartnerID
	                            INNER JOIN CSI ON Child.ChildID = CSI.ChildID 
	                            INNER JOIN CSIDomain ON CSI.CSIID = CSIDomain.CSIID
	                            INNER JOIN Domain ON Domain.DomainID = CSIDomain.DomainID
	                            INNER JOIN CSIDomainScore ON CSIDomain.CSIDomainID = CSIDomainScore.CSIDomainID
	                            INNER JOIN Question ON CSIDomainScore.QuestionID = Question.QuestionID
	                            INNER JOIN Answer ON CSIDomainScore.AnswerID = Answer.AnswerID
	                            INNER JOIN ScoreType ON ScoreType.ScoreTypeID = Answer.ScoreID
                            WHERE CSI.IndexDate >= @initialDate AND CSI.IndexDate <= @lastDate
                            AND (Partner.PartnerID = @partnerID or @partnerID = 0)
                            AND Question.QuestionID = @questionID
                            AND ScoreType.ScoreTypeID = @scoreTypeID
                            ORDER BY Partner.Name";

            return UnitOfWork.DbContext.Database.SqlQuery<ChildByQuestionReportDTO>(query,
                                                            new SqlParameter("initialDate", initialDate),
                                                            new SqlParameter("lastDate", lastDate),
                                                            new SqlParameter("partnerID", partnerID),
                                                            new SqlParameter("questionID", questionID),
                                                            new SqlParameter("scoreTypeID", scoreTypeID)).ToList();
        }


        /*
         * #############################################################
         * ############## AgreggatedChildByQuestionReport ##############
         * #############################################################
         */

        public List<AgreggatedChildByQuestionReportDTO> getAgreggatedChildByQuestionReport(DateTime initialDate, DateTime lastDate, int partnerID, int questionID, int scoreTypeID)
        {

            String query = @"SELECT
	                            prov.Name As Province
	                            ,dist.Name As District
	                            ,s.SiteName As SiteName
	                            ,childbyquestionobj.Partner
	                            ,childbyquestionobj.Domain
	                            ,childbyquestionobj.QuestionDescription
	                            ,childbyquestionobj.Score
	                            ,childbyquestionobj.MaleTotal
	                            ,childbyquestionobj.FemaleTotal
	                            ,childbyquestionobj.Total
                            FROM
	                             [Partner] part
	                            inner join  [Site] s on (part.siteID = s.SiteID)
	                            inner join  [OrgUnit] dist on (dist.OrgUnitID = s.OrgUnitID)
	                            inner join  [OrgUnit] prov on (dist.ParentOrgUnitId = prov.OrgUnitID)
                            INNER JOIN 
                            (
		                            SELECT
			                            childbyquestionobj.Partner
			                            ,childbyquestionobj.Domain
			                            ,childbyquestionobj.QuestionDescription
			                            ,childbyquestionobj.Score
			                            ,SUM(childbyquestionobj.MaleTotal) AS MaleTotal
			                            ,SUM(childbyquestionobj.femaleTotal) AS FemaleTotal
			                            ,SUM(childbyquestionobj.MaleTotal + FemaleTotal) AS Total
		                            FROM
		                            (
				                            SELECT 
					                            Partner.Name AS Partner
					                            ,Domain.Description AS Domain
					                            ,Question.Description AS QuestionDescription
					                            ,ScoreType.Score AS Score
					                            ,MaleTotal = CASE WHEN Child.Gender = 'M' THEN 1 ELSE 0 END
					                            ,femaleTotal = CASE WHEN Child.Gender = 'F' THEN 1 ELSE 0 END
				                            FROM Child 
					                            INNER JOIN HouseHold ON HouseHold.HouseHoldID = CHild.HouseholdID
					                            INNER JOIN Partner ON HouseHold.PartnerID = [Partner].PartnerID
					                            INNER JOIN CSI ON Child.ChildID = CSI.ChildID 
					                            INNER JOIN CSIDomain ON CSI.CSIID = CSIDomain.CSIID
					                            INNER JOIN Domain ON Domain.DomainID = CSIDomain.DomainID
					                            INNER JOIN CSIDomainScore ON CSIDomain.CSIDomainID = CSIDomainScore.CSIDomainID
					                            INNER JOIN Question ON CSIDomainScore.QuestionID = Question.QuestionID
					                            INNER JOIN Answer ON CSIDomainScore.AnswerID = Answer.AnswerID
					                            INNER JOIN ScoreType ON ScoreType.ScoreTypeID = Answer.ScoreID
				                            WHERE CSI.IndexDate >= @initialDate AND CSI.IndexDate <= @lastDate 
				                            AND (Partner.PartnerID = @partnerID or @partnerID = 0)
				                            AND Question.QuestionID = @questionID
				                            AND ScoreType.ScoreTypeID = @scoreTypeID
		                            )childbyquestionobj
		                            GROUP BY 
			                            childbyquestionobj.Partner
			                            ,childbyquestionobj.Domain
			                            ,childbyquestionobj.QuestionDescription
			                            ,childbyquestionobj.Score
		                            --ORDER BY childbyquestionobj.Partner
                            )childbyquestionobj
                            ON childbyquestionobj.Partner = part.Name 
                            GROUP BY
	                            prov.Name
	                            ,dist.Name
	                            ,s.SiteName
	                            ,childbyquestionobj.Partner
	                            ,childbyquestionobj.Domain
	                            ,childbyquestionobj.QuestionDescription
	                            ,childbyquestionobj.Score
	                            ,childbyquestionobj.MaleTotal
	                            ,childbyquestionobj.FemaleTotal
	                            ,childbyquestionobj.Total";

            return UnitOfWork.DbContext.Database.SqlQuery<AgreggatedChildByQuestionReportDTO>(query,
                                                            new SqlParameter("initialDate", initialDate),
                                                            new SqlParameter("lastDate", lastDate),
                                                            new SqlParameter("partnerID", partnerID),
                                                            new SqlParameter("questionID", questionID),
                                                            new SqlParameter("scoreTypeID", scoreTypeID)).ToList();
        }



        /*
         * #######################################################
         * ############## Last Child Status ##############
         * #######################################################
         */

        public List<LastChildStatusReportDTO> getLastChildStatusReport(DateTime initialDate, DateTime lastDate, int partnerID, int childID, int childStatusID)
        {

            String query = @"SELECT
	                            childstatus.PartnerName AS Partner
	                            --,childstatus.ChildID
	                            ,childstatus.FullName
	                            ,childstatus.Gender
	                            ,childstatus.Age
	                            ,childstatus.ChildStatusDate
	                            ,childstatus.Description
                            FROM
                            (
	                            SELECT	
		                            p.PartnerID As PartnerID
		                            ,p.Name AS PartnerName
		                            , (csh.[ChildID]) AS ChildID
		                            , (FirstName) + ' ' + (LastName) As FullName
		                            , c.Gender AS Gender
		                            , DATEDIFF(YEAR, c.DateOFBirth, GETDATE()) AS Age
		                            ,CONVERT(varchar,csh.EffectiveDate,103) AS ChildStatusDate
		                            ,row_number() over(partition by csh.[ChildID] order by csh.[EffectiveDate] desc) as numeroLinha
		                            ,csh.[ChildStatusID] AS ChildStatusID
		                            ,ct.Description
	                            FROM 
		                             [ChildStatusHistory] AS csh
	                            INNER JOIN 
		                             [Child] AS c ON c.[ChildID] = csh.[ChildID]
	                            INNER JOIN
		                             [HouseHold] AS hh ON hh.HouseHoldID = c.HouseholdID
	                            INNER JOIN
		                             [Partner] AS p ON hh.PartnerID = p.PartnerID
	                            INNER JOIN
		                             [ChildStatus] AS ct ON csh.[ChildStatusID] = ct.[StatusID]
	                            WHERE
		                            csh.EffectiveDate >= @initialDate AND csh.EffectiveDate <= @lastDate 
	                            AND (p.PartnerID = @partnerID or @partnerID = 0)
	                            AND (c.ChildID = @childID or @childID =0)
	                            AND (csh.ChildStatusID = @childStatusID or @childStatusID = 0)
                            ) childstatus
                            WHERE
	                            childstatus.numeroLinha = 1
                            ORDER BY childstatus.PartnerName";

            return UnitOfWork.DbContext.Database.SqlQuery<LastChildStatusReportDTO>(query,
                                                            new SqlParameter("initialDate", initialDate),
                                                            new SqlParameter("lastDate", lastDate),
                                                            new SqlParameter("partnerID", partnerID),
                                                            new SqlParameter("childID", childID),
                                                            new SqlParameter("childStatusID", childStatusID)).ToList();
        }


        /*
         * #######################################################
         * ############## Agreggated Last Child Status ###########
         * #######################################################
         */

        public List<AgreggatedLastChildStatusReportDTO> getAgreggatedLastChildStatusReport(int ProvID, int DistID, int SiteID, DateTime initialDate, DateTime lastDate, int partnerID, int childID, int childStatusID)
        {

            String query = @"SELECT
	                            prov.Name As Province
	                            ,dist.Name As District
	                            ,s.SiteName As SiteName
	                            ,lastchildobj.Partner
	                            ,lastchildobj.Description
	                            ,SUM(lastchildobj.MaleTotal) AS MaleTotal
	                            ,SUM(lastchildobj.femaleTotal) AS FemaleTotal
	                            ,SUM(lastchildobj.MaleTotal + FemaleTotal) AS Total
                            FROM
	                             [Partner] part
	                            inner join  [Site] s on (part.siteID = s.SiteID)
	                            inner join  [OrgUnit] dist on (dist.OrgUnitID = s.OrgUnitID)
	                            inner join  [OrgUnit] prov on (dist.ParentOrgUnitId = prov.OrgUnitID)
                            INNER JOIN 
                            (
	                            SELECT
		                            childstatus.PartnerName AS Partner
		                            --,childstatus.ChildID
		                            ,childstatus.FullName
		                            ,MaleTotal = CASE WHEN childstatus.Gender = 'M' THEN 1 ELSE 0 END
		                            ,femaleTotal = CASE WHEN childstatus.Gender = 'F' THEN 1 ELSE 0 END
		                            --,childstatus.Gender
		                            ,childstatus.Age
		                            ,childstatus.ChildStatusDate
		                            ,childstatus.Description
	                            FROM
	                            (
		                            SELECT	
			                            p.PartnerID As PartnerID
			                            ,p.Name AS PartnerName
			                            , (csh.[ChildID]) AS ChildID
			                            , (FirstName) + ' ' + (LastName) As FullName
			                            , c.Gender AS Gender
			                            , DATEDIFF(YEAR, c.DateOFBirth, GETDATE()) AS Age
			                            ,CONVERT(varchar,csh.EffectiveDate,103) AS ChildStatusDate
			                            ,row_number() over(partition by csh.[ChildID] order by csh.[EffectiveDate] desc) as numeroLinha
			                            ,csh.[ChildStatusID] AS ChildStatusID
			                            ,ct.Description
		                            FROM 
			                             [ChildStatusHistory] AS csh
		                            INNER JOIN 
			                             [Child] AS c ON c.[ChildID] = csh.[ChildID]
		                            INNER JOIN
			                             [HouseHold] AS hh ON hh.HouseHoldID = c.HouseholdID
		                            INNER JOIN
			                             [Partner] AS p ON hh.PartnerID = p.PartnerID
		                            INNER JOIN
			                             [ChildStatus] AS ct ON csh.[ChildStatusID] = ct.[StatusID]
		                            WHERE
			                            csh.EffectiveDate >= @initialDate AND csh.EffectiveDate <= @lastDate 
		                            AND (p.PartnerID = @partnerID or @partnerID = 0)
		                            AND (c.ChildID = @childID or @childID =0)
		                            AND (csh.ChildStatusID = @childStatusID or @childStatusID = 0)
	                            ) childstatus
	                            WHERE
		                            childstatus.numeroLinha = 1
                            )lastchildobj
                            ON lastchildobj.Partner = part.Name
                            WHERE
	                            ((s.SiteID = @SiteID AND prov.OrgUnitID = @ProvID AND dist.OrgUnitID = @DistID) OR
	                            (s.SiteID = @SiteID AND @ProvID = 0 AND @DistID = 0) OR
	                            (@SiteID = 0 AND dist.OrgUnitID = @DistID AND @ProvID = 0) OR 
	                            (@SiteID = 0 AND @DistID = 0 AND prov.OrgUnitID = @ProvID) OR
	                            (@SiteID = 0 AND @DistID = 0 AND @ProvID = 0))
                            GROUP BY 
	                            prov.Name
	                            ,dist.Name
	                            ,s.SiteName
	                            ,lastchildobj.Partner
	                            ,lastchildobj.ChildStatusDate
	                            ,lastchildobj.Description
                            ORDER BY 
	                            prov.Name
	                            ,dist.Name
	                            ,s.SiteName
	                            ,lastchildobj.Partner";

            return UnitOfWork.DbContext.Database.SqlQuery<AgreggatedLastChildStatusReportDTO>(query,
                                                            new SqlParameter("ProvID", ProvID),
                                                            new SqlParameter("DistID", DistID),
                                                            new SqlParameter("SiteID", SiteID),
                                                            new SqlParameter("initialDate", initialDate),
                                                            new SqlParameter("lastDate", lastDate),
                                                            new SqlParameter("partnerID", partnerID),
                                                            new SqlParameter("childID", childID),
                                                            new SqlParameter("childStatusID", childStatusID)).ToList();
        }


        /*
         * ############################################################
         * ############## Before and Actual Child Status ##############
         * ############################################################
         */

        public List<BeforeAndActualChildStatusReportDTO> getBeforeAndActualChildStatusReport(DateTime initialDate, DateTime lastDate)
        {
            String query = @"SELECT
	                            estagioAnterior.PartnerName AS Partner
	                            ,estagioAnterior.FullName
	                            ,estagioAnterior.Gender
	                            ,estagioAnterior.Age
	                            ,estagioAnterior.Description AS BeforeActualChildStatus
	                            ,estagioAnterior.ChildStatusDate AS BeforeActualChildStatusDate
	                            ,estagioActual.Description AS ActualChildStatus
	                            ,estagioActual.ChildStatusDate AS ActualChildStatusDate
                            FROM
                            (
		                            SELECT
				                            p.PartnerID As PartnerID, p.Name AS PartnerName
				                            ,(csh.[ChildID]) AS ChildID, (FirstName) + ' ' + (LastName) As FullName
				                            ,c.Gender AS Gender, DATEDIFF(YEAR, c.DateOFBirth, GETDATE()) AS Age
				                            ,csh.[EffectiveDate] AS ChildStatusDate
				                            ,row_number() over(partition by csh.[ChildID] order by csh.[EffectiveDate] DESC) as LastChildStatusDate
				                            ,csh.[ChildStatusID] AS ChildStatusID ,ct.Description
		                            FROM  [ChildStatusHistory] AS csh
		                            INNER JOIN  [Child] AS c ON c.[ChildID] = csh.[ChildID]
		                            INNER JOIN  [HouseHold] AS hh ON hh.HouseHoldID = c.HouseholdID
		                            INNER JOIN  [Partner] AS p ON hh.PartnerID = p.PartnerID
		                            INNER JOIN [ChildStatus] AS ct ON csh.[ChildStatusID] = ct.[StatusID]
		                            WHERE  c.[ChildID] IN
		                            (SELECT [ChildStatusHistory].[ChildID]
		                            FROM  [ChildStatusHistory]
		                            GROUP BY [ChildStatusHistory].[ChildID]
		                            HAVING count([ChildStatusHistory].[ChildID]) > 1)
                            ) estagioAnterior
                            LEFT JOIN
                            (
		                            SELECT	--csh.[ChildStatusHistoryID] AS ChildStatusID
				                            p.PartnerID As PartnerID,p.Name AS PartnerName
				                            , (csh.[ChildID]) AS ChildID, (FirstName) + ' ' + (LastName) As FullName
				                            , c.Gender AS Gender, DATEDIFF(YEAR, c.DateOFBirth, GETDATE()) AS Age
				                            ,csh.[EffectiveDate] AS ChildStatusDate
				                            ,row_number() over(partition by csh.[ChildID] order by csh.[EffectiveDate] DESC) as LastChildStatusDate
				                            ,csh.[ChildStatusID] AS ChildStatusID
				                            ,ct.Description
		                            FROM  [ChildStatusHistory] AS csh
		                            INNER JOIN  [Child] AS c ON c.[ChildID] = csh.[ChildID]
		                            INNER JOIN  [HouseHold] AS hh ON hh.HouseHoldID = c.HouseholdID
		                            INNER JOIN  [Partner] AS p ON hh.PartnerID = p.PartnerID
		                            INNER JOIN  [ChildStatus] AS ct ON csh.[ChildStatusID] = ct.[StatusID]
		                            WHERE  c.[ChildID] IN
		                            (SELECT [ChildStatusHistory].[ChildID]
		                            FROM  [ChildStatusHistory]
		                            GROUP BY [ChildStatusHistory].[ChildID]
		                            HAVING count([ChildStatusHistory].[ChildID]) > 1)
                            ) estagioActual
                            ON (estagioAnterior.FullName=estagioActual.FullName)
                            WHERE
                            estagioActual.LastChildStatusDate = 1 AND estagioAnterior.LastChildStatusDate = 2 AND
                            estagioActual.ChildStatusDate >= @initialDate AND estagioActual.ChildStatusDate <= @lastDate";

            return UnitOfWork.DbContext.Database.SqlQuery<BeforeAndActualChildStatusReportDTO>(query,
                                                            new SqlParameter("initialDate", initialDate),
                                                            new SqlParameter("lastDate", lastDate)).ToList();
        }


        /*
         * #################################################################
         * ############## Agreggated Before and Actual Child Status ########
         * ################################################################
         */

        public List<AgreggatedBeforeAndActualChildStatusReportDTO> getAgreggatedBeforeAndActualChildStatusReport(int ProvID, int DistID, int SiteID, DateTime initialDate, DateTime lastDate, int PartnerID, int BeforeStatusID, int ActualStatusID)
        {
            String query = @"SELECT
	                            prov.Name As Province
	                            ,dist.Name As District
	                            ,s.SiteName As SiteName
	                            ,agreggatedinitiallastobj.Partner
	                            ,agreggatedinitiallastobj.BeforeActualChildStatus
	                            ,agreggatedinitiallastobj.ActualChildStatus
	                            ,agreggatedinitiallastobj.MaleTotal
	                            ,agreggatedinitiallastobj.FemaleTotal
	                            ,agreggatedinitiallastobj.Total
                            FROM
	                             [Partner] part
	                            inner join  [Site] s on (part.siteID = s.SiteID)
	                            inner join  [OrgUnit] dist on (dist.OrgUnitID = s.OrgUnitID)
	                            inner join  [OrgUnit] prov on (dist.ParentOrgUnitId = prov.OrgUnitID)
                            INNER JOIN 
                            (
		                            SELECT
			                            initiallastobj.PartnerID 
			                            ,initiallastobj.Partner
			                            ,initiallastobj.FullName
			                            ,initiallastobj.BeforeActualChildStatus
			                            ,initiallastobj.ActualChildStatus
			                            ,SUM(initiallastobj.MaleTotal) AS MaleTotal
			                            ,SUM(initiallastobj.femaleTotal) AS FemaleTotal
			                            ,SUM(initiallastobj.MaleTotal + FemaleTotal) AS Total
			
		                            FROM
		                            (
			                            SELECT
				                            estagioAnterior.PartnerID
				                            ,estagioAnterior.PartnerName AS Partner
				                            ,estagioAnterior.FullName
				                            ,estagioAnterior.Age
				                            ,estagioAnterior.Description AS BeforeActualChildStatus
				                            ,estagioActual.Description AS ActualChildStatus
				                            ,MaleTotal = CASE WHEN estagioAnterior.Gender = 'M' THEN 1 ELSE 0 END
				                            ,femaleTotal = CASE WHEN estagioAnterior.Gender = 'F' THEN 1 ELSE 0 END
				
			                            FROM
			                            (
					                            SELECT
							                            p.PartnerID As PartnerID, p.Name AS PartnerName
							                            ,(csh.[ChildID]) AS ChildID, (FirstName) + ' ' + (LastName) As FullName
							                            ,c.Gender AS Gender, DATEDIFF(YEAR, c.DateOFBirth, GETDATE()) AS Age
							                            ,csh.[EffectiveDate] AS ChildStatusDate
							                            ,row_number() over(partition by csh.[ChildID] order by csh.[EffectiveDate] DESC) as LastChildStatusDate
							                            ,csh.[ChildStatusID] AS ChildStatusID ,ct.Description
					                            FROM  [ChildStatusHistory] AS csh
					                            INNER JOIN  [Child] AS c ON c.[ChildID] = csh.[ChildID]
					                            INNER JOIN  [HouseHold] AS hh ON hh.HouseHoldID = c.HouseholdID
					                            INNER JOIN  [Partner] AS p ON hh.PartnerID = p.PartnerID
					                            INNER JOIN [ChildStatus] AS ct ON csh.[ChildStatusID] = ct.[StatusID]
					                            WHERE  c.[ChildID] IN
					                            (SELECT [ChildStatusHistory].[ChildID]
					                            FROM  [ChildStatusHistory]
					                            GROUP BY [ChildStatusHistory].[ChildID]
					                            HAVING count([ChildStatusHistory].[ChildID]) > 1)
			                            ) estagioAnterior
			                            LEFT JOIN
			                            (
					                            SELECT	--csh.[ChildStatusHistoryID] AS ChildStatusID
							                            p.PartnerID As PartnerID,p.Name AS PartnerName
							                            , (csh.[ChildID]) AS ChildID, (FirstName) + ' ' + (LastName) As FullName
							                            , c.Gender AS Gender, DATEDIFF(YEAR, c.DateOFBirth, GETDATE()) AS Age
							                            ,csh.[EffectiveDate] AS ChildStatusDate
							                            ,row_number() over(partition by csh.[ChildID] order by csh.[EffectiveDate] DESC) as LastChildStatusDate
							                            ,csh.[ChildStatusID] AS ChildStatusID
							                            ,ct.Description
					                            FROM  [ChildStatusHistory] AS csh
					                            INNER JOIN  [Child] AS c ON c.[ChildID] = csh.[ChildID]
					                            INNER JOIN  [HouseHold] AS hh ON hh.HouseHoldID = c.HouseholdID
					                            INNER JOIN  [Partner] AS p ON hh.PartnerID = p.PartnerID
					                            INNER JOIN  [ChildStatus] AS ct ON csh.[ChildStatusID] = ct.[StatusID]
					                            WHERE  c.[ChildID] IN
					                            (SELECT [ChildStatusHistory].[ChildID]
					                            FROM  [ChildStatusHistory]
					                            GROUP BY [ChildStatusHistory].[ChildID]
					                            HAVING count([ChildStatusHistory].[ChildID]) > 1)
			                            ) estagioActual
			                            ON (estagioAnterior.FullName=estagioActual.FullName)
			                            WHERE
			                            estagioActual.LastChildStatusDate = 1 AND estagioAnterior.LastChildStatusDate = 2 
			                            AND
			                            estagioActual.ChildStatusDate >= @initialDate AND estagioActual.ChildStatusDate <= @lastDate 
			                            AND
			                            (estagioAnterior.ChildStatusID = @BeforeStatusID OR @BeforeStatusID = 0)
			                            AND
			                            (estagioActual.ChildStatusID = @ActualStatusID OR @ActualStatusID = 0)	
			
	                            )initiallastobj
	                            GROUP BY 
		                            initiallastobj.PartnerID
		                            ,initiallastobj.Partner
		                            ,initiallastobj.FullName
		                            ,initiallastobj.BeforeActualChildStatus
		                            ,initiallastobj.ActualChildStatus

                            )agreggatedinitiallastobj
                            ON agreggatedinitiallastobj.Partner = part.Name
                            WHERE  
	                            ((s.SiteID = @SiteID AND @ProvID = 0 AND @DistID = 0) OR
	                            (@SiteID = 0 AND dist.OrgUnitID = @DistID AND @ProvID = 0) OR 
	                            (@SiteID = 0 AND @DistID = 0 AND prov.OrgUnitID = @ProvID) OR
	                            (@SiteID = 0 AND @DistID = 0 AND @ProvID = 0))
                            AND (agreggatedinitiallastobj.PartnerID = @PartnerID OR @PartnerID = 0)
                            GROUP BY 
	                            prov.Name
	                            ,dist.Name
	                            ,s.SiteName
	                            ,agreggatedinitiallastobj.PartnerID
	                            ,agreggatedinitiallastobj.Partner
	                            ,agreggatedinitiallastobj.MaleTotal
	                            ,agreggatedinitiallastobj.FemaleTotal
	                            ,agreggatedinitiallastobj.Total
	                            ,agreggatedinitiallastobj.BeforeActualChildStatus
	                            ,agreggatedinitiallastobj.ActualChildStatus
                            ORDER BY 
	                            prov.Name
	                            ,dist.Name
	                            ,s.SiteName
	                            ,agreggatedinitiallastobj.Partner";

            return UnitOfWork.DbContext.Database.SqlQuery<AgreggatedBeforeAndActualChildStatusReportDTO>(query,
                                                            new SqlParameter("ProvID", ProvID),
                                                            new SqlParameter("DistID", DistID),
                                                            new SqlParameter("SiteID", SiteID),
                                                            new SqlParameter("initialDate", initialDate),
                                                            new SqlParameter("lastDate", lastDate),
                                                            new SqlParameter("PartnerID", PartnerID),
                                                            new SqlParameter("BeforeStatusID", BeforeStatusID),
                                                            new SqlParameter("ActualStatusID", ActualStatusID)).ToList();
        }


        /*
         * ############################################################
         * ############## Initial and Actual Child Status #############
         * ############################################################
         */

        public List<InitialAndActualChildStatusReportDTO> getInitialAndActualChildStatusReport(DateTime initialDate, DateTime lastDate, int PartnerID, int ChildStatusID)
        {
            String query = @"SELECT
                               distinct primeiroEstagio.PartnerName
                               ,primeiroEstagio.FullName
                               ,primeiroEstagio.Gender
                               ,primeiroEstagio.Age
                               ,primeiroEstagio.Description AS InitialChildStatus
                               ,primeiroEstagio.ChildStatusDate AS InitialActualChildStatusDate
                               ,estagioActual.Description AS ActualChildStatus
                               ,estagioActual.ChildStatusDate AS ActualChildStatusDate
                            FROM
                            (
                                   SELECT
                                           p.PartnerID As PartnerID, p.Name AS PartnerName
                                           ,(csh.[ChildID]) AS ChildID, (FirstName) + ' ' + (LastName) As FullName
                                           ,c.Gender AS Gender, DATEDIFF(YEAR, c.DateOFBirth, GETDATE()) AS Age
                                           ,csh.[EffectiveDate] AS ChildStatusDate
                                           ,row_number() over(partition by csh.[ChildID] order by csh.[EffectiveDate] ASC) as InitialChildStatusDate
                                           ,csh.[ChildStatusID] AS ChildStatusID ,ct.Description
                                   FROM  [ChildStatusHistory] AS csh
                                   LEFT JOIN  [Child] AS c ON c.[ChildID] = csh.[ChildID]
                                   LEFT JOIN  [HouseHold] AS hh ON hh.HouseHoldID = c.HouseholdID
                                   LEFT JOIN  [Partner] AS p ON hh.PartnerID = p.PartnerID
                                   LEFT JOIN [ChildStatus] AS ct ON csh.[ChildStatusID] = ct.[StatusID]
                                   WHERE  c.[ChildID] IN
                                   (SELECT [ChildStatusHistory].[ChildID]
                                   FROM  [ChildStatusHistory]
                                   GROUP BY [ChildStatusHistory].[ChildID]
                                   HAVING count([ChildStatusHistory].[ChildID]) > 1)
                            ) primeiroEstagio
                            LEFT JOIN
                            (
                                   SELECT	--csh.[ChildStatusHistoryID] AS ChildStatusID
                                           p.PartnerID As PartnerID,p.Name AS PartnerName
                                           , (csh.[ChildID]) AS ChildID, (FirstName) + ' ' + (LastName) As FullName
                                           , c.Gender AS Gender, DATEDIFF(YEAR, c.DateOFBirth, GETDATE()) AS Age
                                           ,csh.[EffectiveDate] AS ChildStatusDate
                                           ,row_number() over(partition by csh.[ChildID] order by csh.[EffectiveDate] DESC) as LastChildStatusDate
                                           ,csh.[ChildStatusID] AS ChildStatusID
                                           ,ct.Description
                                   FROM  [ChildStatusHistory] AS csh
                                   LEFT JOIN  [Child] AS c ON c.[ChildID] = csh.[ChildID]
                                   LEFT JOIN  [HouseHold] AS hh ON hh.HouseHoldID = c.HouseholdID
                                   LEFT JOIN  [Partner] AS p ON hh.PartnerID = p.PartnerID
                                   LEFT JOIN  [ChildStatus] AS ct ON csh.[ChildStatusID] = ct.[StatusID]
                                   WHERE  c.[ChildID] IN
                                   (SELECT [ChildStatusHistory].[ChildID]
                                   FROM  [ChildStatusHistory]
                                   GROUP BY [ChildStatusHistory].[ChildID]
                                   HAVING count([ChildStatusHistory].[ChildID]) > 1)
                            ) estagioActual
                            ON (primeiroEstagio.FullName=estagioActual.FullName)
                            WHERE
                            primeiroEstagio.InitialChildStatusDate = 1 AND estagioActual.LastChildStatusDate = 1
                            AND
                            estagioActual.ChildStatusDate >= @initialDate AND estagioActual.ChildStatusDate <= @lastDate";// + And;

            return UnitOfWork.DbContext.Database.SqlQuery<InitialAndActualChildStatusReportDTO>(query,
                                                            new SqlParameter("initialDate", initialDate),
                                                            new SqlParameter("lastDate", lastDate)).ToList();
        }


        // ===========================================================================================


        public List<CarePlanChildDTO> getCarePlanChildReport(DateTime initialDate, DateTime finalDate, int partnerID, Boolean maintenanceCandidates)
        {
            String queryCarePlanChild = @"SELECT
							p.Name As Partner,
							c.FirstName As FirstName,
							c.LastName As LastName,
							cp.CarePlanDate										
							FROM  [Partner] p 
							inner join  [HouseHold] hh on (p.PartnerID = hh.PartnerID)
							inner join  [Child] c on (hh.HouseHoldID = c.HouseholdID)
							inner join  [CSI] csi on (csi.ChildID = c.ChildID)
							inner join  [CarePlan] cp on (cp.CSIID = csi.CSIID)
							inner join  [ChildStatusHistory] csh on (csh.ChildID = c.ChildID)
							inner join  [ChildStatus] cs on (cs.StatusID = csh.ChildStatusID)
							Where p.CollaboratorRoleID = 1 
							and DATEDIFF(DAY, CAST(cp.CarePlanDate As Date), GETDATE()) > 75
							and cs.Description in ('Manutenção','Inicial')
							and (@partnerID = 0 or p.partnerID = @partnerID)
                            and ((@initialDate is not NULL and @finalDate is not NULL) and (cp.CarePlanDate Between @initialDate AND @finalDate))
                            or ((@initialDate is not NULL and @finalDate is NULL) and (@initialDate <= cp.CarePlanDate))
                            or ((@initialDate is NULL and @finalDate is not NULL) and (@finalDate >= cp.CarePlanDate))
                            or (@initialDate is NULL and @finalDate is NULL)";

            String queryMaintenanceCandidates = @"SELECT
							obj.Partner, obj.FirstName,	obj.LastName, obj.CarePlanDate
							--,obj.Saude, obj.Alimentacao, obj.Educacao, obj.Proteccao, obj.Apoio, obj.Fortalecimento, obj.TaskCompleted
							FROM
							(
								SELECT
								p.Name As Partner, c.FirstName As FirstName, c.LastName As LastName, cp.CarePlanDate,
								Saude = CASE WHEN d.Description like 'Saúde' and (q.Description not like '2-%' and q.Description not like '5-%' and q.Description not like '6-%') and st.Score = 3 THEN 1 ELSE 0 END,
								Alimentacao = CASE WHEN d.Description like 'Alimentacão/Nutrição' and st.Score = 3 THEN 1 ELSE 0 END,
								Educacao = CASE WHEN d.Description like 'Educação' and (q.Description not like '10-%' and q.Description not like '12-%' ) and st.Score = 3 THEN 1 ELSE 0 END,
								Proteccao = CASE WHEN d.Description like 'Protecção e Apoio Legal' and st.Score = 3 THEN 1 ELSE 0 END,
								--Habitacao = CASE WHEN d.Description like 'Habitação' and (q.Description not like '22-%') and st.Score > 1 THEN 1 ELSE 0 END,
								Apoio = CASE WHEN d.Description like 'Apoio Psico social' and st.Score = 3 THEN 1 ELSE 0 END,
								Fortalecimento = CASE WHEN d.Description like 'Fortalecimento Económico' and st.Score = 3 THEN 1 ELSE 0 END,
								tsk.Completed As TaskCompleted
								--d.Description As Domain_Description, q.Description As Question_Description, st.Score
								FROM  [Partner] p 
								inner join  [HouseHold] hh on (p.PartnerID = hh.PartnerID)
								inner join  [Child] c on (hh.HouseHoldID = c.HouseholdID)
								inner join  [CSI] csi on (csi.ChildID = c.ChildID)
								inner join  [ChildStatusHistory] csh on (csh.ChildID = c.ChildID)
								inner join  [ChildStatus] cs on (cs.StatusID = csh.ChildStatusID)
								left join  [CarePlan] cp on (cp.CSIID = csi.CSIID)
								inner join  [CarePlanDomain] cpd on (cpd.CarePlanID = cp.CarePlanID)
								inner join  [CarePlanDomainSupportService] cpdss on (cpdss.CarePlanDomainID = cpd.CarePlanDomainID)
								inner join  [Tasks] tsk on (tsk.CarePlanDomainSupportServiceID = cpdss.CarePlanDomainSupportServiceID)
								left join  [CSIDomain] csiDom on (csiDom.CSIID = csi.CSIID)
								inner join  [CSIDomainScore] csiDomScr on (csiDomScr.CSIDomainID = csiDom.CSIDomainID)
								inner join  [Domain] d on (csiDom.DomainID = d.DomainID)
								inner join  [Question] q on (q.QuestionID = csiDomScr.QuestionID)
								inner join  [Answer] a on (a.AnswerID = csiDomScr.AnswerID)
								inner join  [ScoreType] st on (st.ScoreTypeID = a.ScoreID)
								Where p.CollaboratorRoleID = 1 
								and DATEDIFF(DAY, CAST(cp.CarePlanDate As Date), GETDATE()) > 75
							    and cs.Description in ('Manutenção','Inicial')
							    and @partnerID = 0 or p.partnerID = @partnerID
                                and ((@initialDate is not NULL and @finalDate is not NULL) and (cp.CarePlanDate Between @initialDate AND @finalDate))
                                or ((@initialDate is not NULL and @finalDate is NULL) and (@initialDate <= cp.CarePlanDate))
                                or ((@initialDate is NULL and @finalDate is not NULL) and (@finalDate >= cp.CarePlanDate))
                                or (@initialDate is NULL and @finalDate is NULL)
							) obj
							WHERE
								obj.Saude + obj.Alimentacao + obj.Educacao  + obj.Proteccao + obj.Apoio + obj.Fortalecimento = 6 or obj.TaskCompleted = 1
							Group by
								obj.Partner, obj.FirstName, obj.LastName, obj.CarePlanDate
								--,obj.Saude, obj.Alimentacao, obj.Educacao, obj.Proteccao, obj.Apoio, obj.Fortalecimento, obj.TaskCompleted";

            return UnitOfWork.DbContext.Database.SqlQuery<CarePlanChildDTO>((maintenanceCandidates ? queryMaintenanceCandidates : queryCarePlanChild),
                                                                            new SqlParameter("initialDate", initialDate),
                                                                            new SqlParameter("finalDate", finalDate),
                                                                            new SqlParameter("partnerID", partnerID)).ToList();
        }

        public List<CarePlanChildDTO> getAgreggatedCarePlanChildReport(DateTime initialDate, DateTime finalDate, int partnerID, Boolean maintenanceCandidates)
        {
            String queryCarePlanChild = @"SELECT
							p.Name As Partner, Count(*)	As ChildCount								
							FROM  [Partner] p 
							inner join  [HouseHold] hh on (p.PartnerID = hh.PartnerID)
							inner join  [Child] c on (hh.HouseHoldID = c.HouseholdID)
							inner join  [CSI] csi on (csi.ChildID = c.ChildID)
							inner join  [CarePlan] cp on (cp.CSIID = csi.CSIID)
							inner join  [ChildStatusHistory] csh on (csh.ChildID = c.ChildID)
							inner join  [ChildStatus] cs on (cs.StatusID = csh.ChildStatusID)
							Where p.CollaboratorRoleID = 1 
							and DATEDIFF(DAY, CAST(cp.CarePlanDate As Date), GETDATE()) > 75
							and cs.Description in ('Manutenção','Inicial')
							and (@partnerID = 0 or p.partnerID = @partnerID)
                            and ((@initialDate is not NULL and @finalDate is not NULL) and (cp.CarePlanDate Between @initialDate AND @finalDate))
                            or ((@initialDate is not NULL and @finalDate is NULL) and (@initialDate <= cp.CarePlanDate))
                            or ((@initialDate is NULL and @finalDate is not NULL) and (@finalDate >= cp.CarePlanDate))
                            or (@initialDate is NULL and @finalDate is NULL) group by p.Name";

            String queryMaintenanceCandidates = @"SELECT 
                            obj.Partner, Count(*) As ChildCount
							FROM
							(
								SELECT
								p.Name As Partner, c.FirstName As FirstName, c.LastName As LastName, cp.CarePlanDate,
								Saude = CASE WHEN d.Description like 'Saúde' and (q.Description not like '2-%' and q.Description not like '5-%' and q.Description not like '6-%') and st.Score = 3 THEN 1 ELSE 0 END,
								Alimentacao = CASE WHEN d.Description like 'Alimentacão/Nutrição' and st.Score = 3 THEN 1 ELSE 0 END,
								Educacao = CASE WHEN d.Description like 'Educação' and (q.Description not like '10-%' and q.Description not like '12-%' ) and st.Score = 3 THEN 1 ELSE 0 END,
								Proteccao = CASE WHEN d.Description like 'Protecção e Apoio Legal' and st.Score = 3 THEN 1 ELSE 0 END,
								--Habitacao = CASE WHEN d.Description like 'Habitação' and (q.Description not like '22-%') and st.Score > 1 THEN 1 ELSE 0 END,
								Apoio = CASE WHEN d.Description like 'Apoio Psico social' and st.Score = 3 THEN 1 ELSE 0 END,
								Fortalecimento = CASE WHEN d.Description like 'Fortalecimento Económico' and st.Score = 3 THEN 1 ELSE 0 END,
								tsk.Completed As TaskCompleted
								--d.Description As Domain_Description, q.Description As Question_Description, st.Score
								FROM  [Partner] p 
								inner join  [HouseHold] hh on (p.PartnerID = hh.PartnerID)
								inner join  [Child] c on (hh.HouseHoldID = c.HouseholdID)
								inner join  [CSI] csi on (csi.ChildID = c.ChildID)
								inner join  [ChildStatusHistory] csh on (csh.ChildID = c.ChildID)
								inner join  [ChildStatus] cs on (cs.StatusID = csh.ChildStatusID)
								left join  [CarePlan] cp on (cp.CSIID = csi.CSIID)
								inner join  [CarePlanDomain] cpd on (cpd.CarePlanID = cp.CarePlanID)
								inner join  [CarePlanDomainSupportService] cpdss on (cpdss.CarePlanDomainID = cpd.CarePlanDomainID)
								inner join  [Tasks] tsk on (tsk.CarePlanDomainSupportServiceID = cpdss.CarePlanDomainSupportServiceID)
								left join  [CSIDomain] csiDom on (csiDom.CSIID = csi.CSIID)
								inner join  [CSIDomainScore] csiDomScr on (csiDomScr.CSIDomainID = csiDom.CSIDomainID)
								inner join  [Domain] d on (csiDom.DomainID = d.DomainID)
								inner join  [Question] q on (q.QuestionID = csiDomScr.QuestionID)
								inner join  [Answer] a on (a.AnswerID = csiDomScr.AnswerID)
								inner join  [ScoreType] st on (st.ScoreTypeID = a.ScoreID)
								Where p.CollaboratorRoleID = 1 
								and DATEDIFF(DAY, CAST(cp.CarePlanDate As Date), GETDATE()) > 75
							    and cs.Description in ('Manutenção','Inicial')
							    and @partnerID = 0 or p.partnerID = @partnerID
                                and ((@initialDate is not NULL and @finalDate is not NULL) and (cp.CarePlanDate Between @initialDate AND @finalDate))
                                or ((@initialDate is not NULL and @finalDate is NULL) and (@initialDate <= cp.CarePlanDate))
                                or ((@initialDate is NULL and @finalDate is not NULL) and (@finalDate >= cp.CarePlanDate))
                                or (@initialDate is NULL and @finalDate is NULL)
							) obj
							WHERE
								obj.Saude + obj.Alimentacao + obj.Educacao  + obj.Proteccao + obj.Apoio + obj.Fortalecimento = 6 or obj.TaskCompleted = 1
							Group by
								obj.Partner";
            return UnitOfWork.DbContext.Database.SqlQuery<CarePlanChildDTO>((maintenanceCandidates ? queryMaintenanceCandidates : queryCarePlanChild),
                                                                            new SqlParameter("initialDate", initialDate),
                                                                            new SqlParameter("finalDate", finalDate),
                                                                            new SqlParameter("partnerID", partnerID)).ToList();
        }

        public List<RoutineVisitChildDTO> getAgreggatedChildrenWithouAssistence(int ProvID, int DistID, int SiteID, DateTime initialDate, DateTime finalDate)
        {
            String query = @"SELECT
	                            agreggatedinfo.Province, agreggatedinfo.District, agreggatedinfo.SiteName, 
	                            SUM(agreggatedinfo.Male) As TotalMale, SUM(agreggatedinfo.Female) As TotalFemale, 
	                            SUM(agreggatedinfo.Female)+SUM(agreggatedinfo.Male) As TotalCount
                            FROM
                            (
	                            SELECT
		                            Province, District, SiteName, Person, 
		                            Male = CASE WHEN ServicesTotal = 0 AND Gender = 'M' THEN 1 ELSE 0 END,
		                            Female = CASE WHEN ServicesTotal = 0 AND Gender = 'F' THEN 1 ELSE 0 END
	                            FROM
	                            (
		                            SELECT
			                            prov.Name As Province, dist.Name As District, s.SiteName, c.FirstName+c.LastName As Person, c.Gender, 0 As ServicesTotal
		                            FROM  [Partner] p
			                            inner join  [Site] s on (p.siteID = s.SiteID)
			                            inner join  [OrgUnit] dist on (dist.OrgUnitID = s.OrgUnitID)
			                            inner join  [OrgUnit] prov on (dist.ParentOrgUnitId = prov.OrgUnitID)
			                            inner join  [HouseHold] hh on (p.PartnerID = hh.PartnerID)
			                            inner join  [RoutineVisit] rv on (hh.HouseHoldID = rv.HouseholdID)
			                            inner join  [RoutineVisitMember] rvm on (rv.RoutineVisitID = rvm.RoutineVisitID)
			                            inner join  [RoutineVisitSupport] rvs on (rvm.RoutineVisitMemberID = rvs.RoutineVisitMemberID)
			                            inner join  [Child] c on (c.ChildID = rvm.ChildID)
			                            inner join  [ChildStatusHistory] csh on (c.ChildID = csh.ChildID and csh.ChildStatusID != 6)
		                            WHERE p.CollaboratorRoleID = 1
			                            -- Criança que não teve nova ficha de visita entre o periodo seleccionado
                                        AND ((@initialDate is not NULL and @finalDate is not NULL) and (rv.RoutineVisitDate NOT Between @initialDate AND @finalDate))
                                        OR ((@initialDate is not NULL and @finalDate is NULL) and (@initialDate >= rv.RoutineVisitDate))
                                        OR ((@initialDate is NULL and @finalDate is not NULL) and (@finalDate <= rv.RoutineVisitDate))
                                        OR (@initialDate is NULL and @finalDate is NULL)
                                        AND ((s.SiteID = @SiteID AND @ProvID = 0 AND @DistID = 0) OR (@SiteID = 0 AND dist.OrgUnitID = @DistID AND @ProvID = 0) OR 
			                            (@SiteID = 0 AND @DistID = 0 AND prov.OrgUnitID = @ProvID) OR (@SiteID = 0 AND @DistID = 0 AND @ProvID = 0)) 
		                            GROUP BY prov.Name, dist.Name, s.SiteName, c.FirstName+c.LastName, c.Gender
		                            UNION all
		                            SELECT
			                            prov.Name As Province, dist.Name As District, s.SiteName, c.FirstName+c.LastName As Person, c.Gender, SUM(CONVERT(INT, rvs.SupportValue)) As ServicesTotal
		                            FROM  [Partner] p
			                            inner join  [Site] s on (p.siteID = s.SiteID)
			                            inner join  [OrgUnit] dist on (dist.OrgUnitID = s.OrgUnitID)
			                            inner join  [OrgUnit] prov on (dist.ParentOrgUnitId = prov.OrgUnitID)
			                            inner join  [HouseHold] hh on (p.PartnerID = hh.PartnerID)
			                            inner join  [RoutineVisit] rv on (hh.HouseHoldID = rv.HouseholdID)
			                            inner join  [RoutineVisitMember] rvm on (rv.RoutineVisitID = rvm.RoutineVisitID)
			                            inner join  [RoutineVisitSupport] rvs on (rvm.RoutineVisitMemberID = rvs.RoutineVisitMemberID and rvs.SupportType not in ('SAVINGS_GROUP_MEMBER', 'COMMENT'))
			                            inner join  [Child] c on (c.ChildID = rvm.ChildID)
			                            inner join  [ChildStatusHistory] csh on (c.ChildID = csh.ChildID and csh.ChildStatusID != 6)
		                            WHERE p.CollaboratorRoleID = 1
			                            -- Crianças que tem ficha no periodo seleccionado, mas na ficha em analise, a Criança não teve nenhum serviço prestado
                                        AND ((@initialDate is not NULL and @finalDate is not NULL) and (rv.RoutineVisitDate Between @initialDate AND @finalDate))
                                        OR ((@initialDate is not NULL and @finalDate is NULL) and (@initialDate >= rv.RoutineVisitDate))
                                        OR ((@initialDate is NULL and @finalDate is not NULL) and (@finalDate <= rv.RoutineVisitDate))
                                        OR (@initialDate is NULL and @finalDate is NULL)
			                            AND ((s.SiteID = @SiteID AND @ProvID = 0 AND @DistID = 0) OR (@SiteID = 0 AND dist.OrgUnitID = @DistID AND @ProvID = 0) OR 
			                            (@SiteID = 0 AND @DistID = 0 AND prov.OrgUnitID = @ProvID) OR (@SiteID = 0 AND @DistID = 0 AND @ProvID = 0)) 
		                            GROUP BY prov.Name, dist.Name, s.SiteName, c.FirstName+c.LastName, c.Gender
	                            ) childinfo

	                            UNION ALL

	                            SELECT
		                            Province, District, SiteName, Person, 
		                            Male = CASE WHEN ServicesTotal = 0 AND Gender = 'M' THEN 1 ELSE 0 END,
		                            Female = CASE WHEN ServicesTotal = 0 AND Gender = 'F' THEN 1 ELSE 0 END
	                            FROM
	                            (
		                            SELECT
			                            prov.Name As Province, dist.Name As District, s.SiteName, a.FirstName+a.LastName As Person, a.Gender, 0 As ServicesTotal
		                            FROM  [Partner] p
			                            inner join  [Site] s on (p.siteID = s.SiteID)
			                            inner join  [OrgUnit] dist on (dist.OrgUnitID = s.OrgUnitID)
			                            inner join  [OrgUnit] prov on (dist.ParentOrgUnitId = prov.OrgUnitID)
			                            inner join  [HouseHold] hh on (p.PartnerID = hh.PartnerID)
			                            inner join  [RoutineVisit] rv on (hh.HouseHoldID = rv.HouseholdID)
			                            inner join  [RoutineVisitMember] rvm on (rv.RoutineVisitID = rvm.RoutineVisitID)
			                            inner join  [RoutineVisitSupport] rvs on (rvm.RoutineVisitMemberID = rvs.RoutineVisitMemberID)
			                            inner join  [Adult] a on (a.AdultID = rvm.AdultID)
			                            inner join  [ChildStatusHistory] csh on (a.AdultID = csh.ChildID and csh.ChildStatusID != 6)
		                            WHERE p.CollaboratorRoleID = 1
			                            -- Criança que não teve nova ficha de visita entre o periodo seleccionado
                                        AND ((@initialDate is not NULL and @finalDate is not NULL) and (rv.RoutineVisitDate NOT Between @initialDate AND @finalDate))
                                        OR ((@initialDate is not NULL and @finalDate is NULL) and (@initialDate >= rv.RoutineVisitDate))
                                        OR ((@initialDate is NULL and @finalDate is not NULL) and (@finalDate <= rv.RoutineVisitDate))
                                        OR (@initialDate is NULL and @finalDate is NULL)
                                        AND ((s.SiteID = @SiteID AND @ProvID = 0 AND @DistID = 0) OR (@SiteID = 0 AND dist.OrgUnitID = @DistID AND @ProvID = 0) OR 
			                            (@SiteID = 0 AND @DistID = 0 AND prov.OrgUnitID = @ProvID) OR (@SiteID = 0 AND @DistID = 0 AND @ProvID = 0)) 
		                            GROUP BY prov.Name, dist.Name, s.SiteName, a.FirstName+a.LastName, a.Gender
		                            UNION all
		                            SELECT
			                            prov.Name As Province, dist.Name As District, s.SiteName, a.FirstName+a.LastName As Person, a.Gender, SUM(CONVERT(INT, rvs.SupportValue)) As ServicesTotal
		                            FROM  [Partner] p
			                            inner join  [Site] s on (p.siteID = s.SiteID)
			                            inner join  [OrgUnit] dist on (dist.OrgUnitID = s.OrgUnitID)
			                            inner join  [OrgUnit] prov on (dist.ParentOrgUnitId = prov.OrgUnitID)
			                            inner join  [HouseHold] hh on (p.PartnerID = hh.PartnerID)
			                            inner join  [RoutineVisit] rv on (hh.HouseHoldID = rv.HouseholdID)
			                            inner join  [RoutineVisitMember] rvm on (rv.RoutineVisitID = rvm.RoutineVisitID)
			                            inner join  [RoutineVisitSupport] rvs on (rvm.RoutineVisitMemberID = rvs.RoutineVisitMemberID and rvs.SupportType not in ('SAVINGS_GROUP_MEMBER', 'COMMENT'))
			                            inner join  [Adult] a on (a.AdultID = rvm.AdultID)
			                            inner join  [ChildStatusHistory] csh on (a.AdultID = csh.ChildID and csh.ChildStatusID != 6)
		                            WHERE p.CollaboratorRoleID = 1
			                            -- Crianças que tem ficha no periodo seleccionado, mas na ficha em analise, a Criança não teve nenhum serviço prestado
                                        AND ((@initialDate is not NULL and @finalDate is not NULL) and (rv.RoutineVisitDate Between @initialDate AND @finalDate))
                                        OR ((@initialDate is not NULL and @finalDate is NULL) and (@initialDate >= rv.RoutineVisitDate))
                                        OR ((@initialDate is NULL and @finalDate is not NULL) and (@finalDate <= rv.RoutineVisitDate))
                                        OR (@initialDate is NULL and @finalDate is NULL)
			                            AND ((s.SiteID = @SiteID AND @ProvID = 0 AND @DistID = 0) OR (@SiteID = 0 AND dist.OrgUnitID = @DistID AND @ProvID = 0) OR 
			                            (@SiteID = 0 AND @DistID = 0 AND prov.OrgUnitID = @ProvID) OR (@SiteID = 0 AND @DistID = 0 AND @ProvID = 0)) 
		                            GROUP BY prov.Name, dist.Name, s.SiteName, a.FirstName+a.LastName, a.Gender
	                            ) adultinfo
	                            GROUP BY Province, District, SiteName, Person, Gender, ServicesTotal
                            ) agreggatedinfo
                            GROUP BY agreggatedinfo.Province, agreggatedinfo.District, agreggatedinfo.SiteName";
            return UnitOfWork.DbContext.Database.SqlQuery<RoutineVisitChildDTO>(query,
                                                new SqlParameter("ProvID", ProvID),
                                                new SqlParameter("DistID", DistID),
                                                new SqlParameter("SiteID", SiteID),
                                                new SqlParameter("initialDate", initialDate),
                                                new SqlParameter("finalDate", finalDate)).ToList();
        }

        public Boolean WasBeneficiaryServedInThePeriod(Beneficiary beneficiary, DateTime startDate, DateTime endDate)
        {
            List<RoutineVisitMember> allRoutineVisitMember = RoutineVisitMemberRepository
                                                                .GetAll()
                                                                .Where(rvm => rvm.BeneficiaryID == beneficiary.BeneficiaryID
                                                                    && (rvm.RoutineVisitDate >= startDate && rvm.RoutineVisitDate <= endDate)
                                                                    && rvm.BeneficiaryHasServices)
                                                                .ToList();

            Trimester Trimester = TrimesterQueryService.GetTrimesterByDate(endDate);

            return allRoutineVisitMember.Count > 0 || (CheckIfHasReferencesActivist(beneficiary, Trimester) == true);
        }

        public void EvaluateBeneficiaryServicesState(Beneficiary beneficiary, string evaluationType, DateTime currentDate)
        {
            Trimester currentTrimester = TrimesterQueryService.GetTrimesterByDate(currentDate);

            List<SimpleEntity> benficiaryServicesStatuses = SimpleEntityRepository
                                                            .GetAll()
                                                            .Where(se => se.Type == "ben-services-status")
                                                            .ToList();

            if (IsBeneficiaryActive(beneficiary, currentTrimester, currentDate))
            {
                beneficiary.ServicesStatus = ServicesStatus.ACTIVEGREEN;
                beneficiary.ServicesStatus.GenerationDate = currentDate;
            }
            else if (IsBeneficiaryActiveOrange(beneficiary, currentTrimester, currentDate))
            {
                beneficiary.ServicesStatus = ServicesStatus.ACTIVEORANGE;
                beneficiary.ServicesStatus.GenerationDate = currentDate;
            }
            else
            {
                beneficiary.ServicesStatus = ServicesStatus.INACTIVE;
                beneficiary.ServicesStatus.GenerationDate = currentDate;
            }

            beneficiary.ServicesStatus.BeneficiaryID = beneficiary.BeneficiaryID;
            beneficiary.ServicesStatus.Beneficiary_guid = beneficiary.Beneficiary_guid;

            // TODO Salvar Service Status. Muito importante
            Save(beneficiary);
        }


        public void cleanEvaluatedBeneficiaries(string evaluationType)
        {
            if (evaluationType == "InitialLoadingData")
            {
                foreach (Beneficiary ben in getAllBeneficiariesWithServiceStatus())
                {
                    ben.ServicesStatusID = null;
                    Save(ben);
                }
                Commit();
            }
        }


        public void EvaluateAllBeneficiariesServicesState(string evaluationType, DateTime date)
        {
            Trimester Trimester = TrimesterQueryService.GetTrimesterByDate(date);
            getAllBeneficiariesInInitialStatus(Trimester.EndDate).ForEach(beneficiary =>
            {
                EvaluateBeneficiaryServicesState(beneficiary, evaluationType, date);
            });

            Commit();
        }

        private bool IsBeneficiaryActive(Beneficiary beneficiary, Trimester currentTrimester, DateTime currentDate)
        {
            DateTime registrationDate = getRegistrationDate(beneficiary); //Data de Registo de Beneficiários

            Trimester beneficiaryRegistrationTrimester = TrimesterQueryService //Trimeste em que a data de registro se enquadra
                                .GetTrimesterByDate(registrationDate);

            var previousTrimester = TrimesterQueryService.GetPreviousTrimesters(1, false, currentTrimester);

            if (beneficiary.AgeInYearsMethod(currentDate) < 18)
            {
                if (this.CheckIfHasPAFinLast4Trimesters(beneficiary, currentTrimester) && CheckIfHasCarePlanMonitoringinTrimester(beneficiary, currentTrimester))
                {
                    return
                    // Beneficiários que foram registados no Trimestre Corrente e receberam Serviço no Trimestre Corrente
                    WasBeneficiaryServedInThePeriod
                    (beneficiary, beneficiaryRegistrationTrimester.StartDate, beneficiaryRegistrationTrimester.EndDate)
                        && currentTrimester == beneficiaryRegistrationTrimester
                    // Ou Beneficiários que receberam serviços no Trimestre Anterior e Corrente
                    ||
                    WasBeneficiaryServedInThePeriod
                    (beneficiary, previousTrimester[0].StartDate, previousTrimester[0].EndDate)
                    &&
                    WasBeneficiaryServedInThePeriod
                    (beneficiary, currentTrimester.StartDate, currentTrimester.EndDate);
                }
            }
            else
            {
                return
                    // Beneficiários que foram registados no Trimestre Corrente
                    WasBeneficiaryServedInThePeriod
                    (beneficiary, beneficiaryRegistrationTrimester.StartDate, beneficiaryRegistrationTrimester.EndDate)
                        && currentTrimester == beneficiaryRegistrationTrimester
                    // Ou Beneficiários que receberam serviços no Trimestre Anterior e Corrente
                    ||
                    WasBeneficiaryServedInThePeriod
                    (beneficiary, previousTrimester[0].StartDate, previousTrimester[0].EndDate)
                    &&
                    WasBeneficiaryServedInThePeriod
                    (beneficiary, currentTrimester.StartDate, currentTrimester.EndDate);
            }

            return false;
        }

        private bool IsBeneficiaryActiveOrange(Beneficiary beneficiary, Trimester currentTrimester, DateTime currentDate)
        {
            var previousTrimester = TrimesterQueryService.GetPreviousTrimesters(1, false, currentTrimester);

            if (beneficiary.AgeInYearsMethod(currentDate) < 18)
            {
                if (this.CheckIfHasPAFinLast4Trimesters(beneficiary, currentTrimester) && CheckIfHasCarePlanMonitoringinTrimester(beneficiary, currentTrimester))
                {
                    // Ou Beneficiários que receberam serviços no Trimestre Anterior, mas não receberam no Trimestre Corrente
                    return WasBeneficiaryServedInThePeriod
                        (beneficiary, previousTrimester[0].StartDate, previousTrimester[0].EndDate)
                        &&
                        WasBeneficiaryServedInThePeriod
                        (beneficiary, currentTrimester.StartDate, currentTrimester.EndDate) == false;
                }
            }
            else
            {
                // Ou Beneficiários que receberam serviços no Trimestre Anterior, mas ainda receberam no Trimestre Corrente
                return WasBeneficiaryServedInThePeriod
                    (beneficiary, previousTrimester[0].StartDate, previousTrimester[0].EndDate)
                    &&
                    WasBeneficiaryServedInThePeriod
                    (beneficiary, currentTrimester.StartDate, currentTrimester.EndDate) == false;
            }
            return false;
        }

        private bool IsBeneficiaryRegistrationDateWithInTrimester(Beneficiary beneficiary, Trimester trimester)
        {
            DateTime registrationDate = getRegistrationDate(beneficiary);

            return registrationDate >= trimester.StartDate
                && trimester.EndDate <= registrationDate;
        }

        public DateTime getRegistrationDate(Beneficiary beneficiary)
        {
            return (beneficiary.RegistrationDate == null) ? HouseholdService.getHouseholdRegistrationDate(beneficiary.HouseholdID.Value) : beneficiary.RegistrationDate.Value;
        }

        public bool CheckIfHasPAFinLast4Trimesters(Beneficiary ben, Trimester currentTrimester)
        {
            List<Trimester> Last4Ttrimesters = TrimesterQueryService.GetPreviousTrimesters(3, true, currentTrimester);
    
            bool result = false;
            foreach(Trimester Trim in Last4Ttrimesters)
            {
                if (CheckIfHasPAFinTrimester(ben, Trim)) { return result = true; }
                return result;
            }

            return result;

        }


        public bool CheckIfHasPAFinTrimester(Beneficiary ben, Trimester currentTrimester)
        {
            return (from Beneficiary in UnitOfWork.DbContext.Beneficiaries
                    join household in UnitOfWork.DbContext.Households on Beneficiary.HouseholdID equals household.HouseHoldID
                    join paf in UnitOfWork.DbContext.HouseholdSupportPlans on household.HouseHoldID equals paf.HouseholdID
                    where Beneficiary.BeneficiaryID == ben.BeneficiaryID
                    && (paf.SupportPlanInitialDate >= currentTrimester.StartDate && paf.SupportPlanInitialDate <= currentTrimester.EndDate)
                    select Beneficiary.FirstName).Any();
        }

        public bool CheckIfHasCarePlanMonitoringinTrimester(Beneficiary ben, Trimester currentTrimester)
        {
            return (from Beneficiary in UnitOfWork.DbContext.Beneficiaries
                    join household in UnitOfWork.DbContext.Households on Beneficiary.HouseholdID equals household.HouseHoldID
                    join rv in UnitOfWork.DbContext.RoutineVisits on household.HouseHoldID equals rv.HouseHoldID
                    join rvm in UnitOfWork.DbContext.RoutineVisitMembers on rv.RoutineVisitID equals rvm.RoutineVisitID
                    join rvs in UnitOfWork.DbContext.RoutineVisitSupports on rvm.RoutineVisitMemberID equals rvs.RoutineVisitMemberID
                    join sst in UnitOfWork.DbContext.SupportServiceTypes on rvs.SupportID equals sst.SupportServiceTypeID
                    where Beneficiary.BeneficiaryID == ben.BeneficiaryID
                    && (rv.RoutineVisitDate >= currentTrimester.StartDate && rv.RoutineVisitDate <= currentTrimester.EndDate)
                    && rv.Version == "v2"
                    && sst.Description == "Monitoria do Plano de Acção da família"
                    && rvs.Checked == true
                    select Beneficiary.FirstName).Any();
        }

        public bool CheckIfHasReferencesActivist(Beneficiary ben, Trimester currentTrimester)
        {
            return (from Beneficiary in UnitOfWork.DbContext.Beneficiaries
                    join household in UnitOfWork.DbContext.Households on Beneficiary.HouseholdID equals household.HouseHoldID
                    join rs in UnitOfWork.DbContext.ReferenceServices on Beneficiary.BeneficiaryID equals rs.BeneficiaryID
                    join r in UnitOfWork.DbContext.References on rs.ReferenceServiceID equals r.ReferenceServiceID
                    join rt in UnitOfWork.DbContext.ReferenceTypes on r.ReferenceTypeID equals rt.ReferenceTypeID
                    where Beneficiary.BeneficiaryID == ben.BeneficiaryID
                    && (rs.ReferenceDate >= currentTrimester.StartDate && rs.ReferenceDate <= currentTrimester.EndDate)
                    && (r.Value != "0" || r.Value != "" || r.Value != null)
                    && rt.ReferenceCategory == "Activist"
                    select Beneficiary.FirstName).Any();
        }
    }
}
