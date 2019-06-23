using EFDataAccess.UOW;
using System;
using System.Collections.Generic;
using System.Linq;
using VPPS.CSI.Domain;
using System.Data;
using System.Data.Entity;

namespace EFDataAccess.Services
{
    public class BeneficiaryService : BaseService
    {
        private UserService UserService;
        private HIVStatusService HIVStatusService;
        private HIVStatusQueryService HIVStatusQueryService;
        private Repository.IRepository<SimpleEntity> SimpleEntityRepository;

        public BeneficiaryService(UnitOfWork uow) : base(uow)
        {
            UserService = new UserService(uow);
            HIVStatusService = new HIVStatusService(uow);
            HIVStatusQueryService = new HIVStatusQueryService(uow);
            SimpleEntityRepository = uow.Repository<SimpleEntity>();
        }

        private EFDataAccess.Repository.IRepository<Beneficiary> BeneficiaryRepository => UnitOfWork.Repository<Beneficiary>();

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

            List<Beneficiary> beneficiaryList = benList.ToList();

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

        public Beneficiary FetchById(int id) { return BeneficiaryRepository.GetAll().Where(x => x.BeneficiaryID == id).Include(x => x.HIVStatus).Include(x => x.KinshipToFamilyHead).Include(x => x.OVCType).FirstOrDefault(); }

        public IQueryable<Beneficiary> findBeneficiaryByname(string name)
        {
            return UnitOfWork.Repository<Beneficiary>()
                .GetAll()
                .Where(c => c.FirstName.Contains(name) || c.LastName.Contains(name));
        }

        public List<Beneficiary> getAllBeneficiariesInInitialStatus(DateTime date)
        {
            List<Beneficiary> benList = BeneficiaryRepository
                .GetAll().Include(e => e.ChildStatusHistories.Select(x => x.ChildStatus))
                .Where(e => e.ChildStatusHistories.OrderByDescending(x => x.EffectiveDate < date)
                .ThenByDescending(x => x.ChildStatusHistoryID).FirstOrDefault().ChildStatus.Description == "Inicial").ToList();

            return benList;
        }

        public int findAllBeneficiariesByStatus(DateTime date, string status, string ageGroup)
        {
            List<Beneficiary> beneficiaryList = 
                BeneficiaryRepository.GetAll()
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
                    .Where(x => x.HIVStatus.HIV == HIVStatus.POSITIVE
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

        public IQueryable<Beneficiary> findBeneficiaryBynameAndPartnerAndCode(string name, string partnerName, string beneficiaryCode)
        {
            return UnitOfWork.Repository<Beneficiary>()
                .GetAll()
                .Where(c => c.FirstName.Contains(name) || c.LastName.Contains(name))
                .Where(e => (name == null || name.Equals("") || e.FirstName.Contains(name) || e.LastName.Contains(name))
                && (partnerName == null || partnerName.Equals(""))
                && (beneficiaryCode == null || beneficiaryCode.Equals("") || e.Code.Contains(beneficiaryCode)))
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
                .ToList();
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
                .Where(c => c.BeneficiaryID == BeneficiaryID).SingleOrDefault();
        }

        public void Save(Beneficiary Beneficiary)
        {
            if (Beneficiary.BeneficiaryID > 0)
            { BeneficiaryRepository.Update(Beneficiary); }
            else
            { BeneficiaryRepository.Add(Beneficiary); }
        }
    }
}
