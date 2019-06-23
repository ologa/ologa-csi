using EFDataAccess.UOW;
using System;
using System.Collections.Generic;
using System.Linq;
using VPPS.CSI.Domain;
using EFDataAccess.DTO;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;

namespace EFDataAccess.Services
{
    public class AdultService : BaseService
    {
        private UserService UserService;
        private HouseholdService HouseHoldservice;
        private HIVStatusService HivStatusService;
        private HIVStatusQueryService HIVStatusQueryService;

        public AdultService(UnitOfWork uow) : base(uow)
        {
            UserService = new UserService(uow);
            HouseHoldservice = new HouseholdService(uow);
            HivStatusService = new HIVStatusService(uow);
            HIVStatusQueryService = new HIVStatusQueryService(uow);
        }

        private EFDataAccess.Repository.IRepository<Adult> AdultRepository => UnitOfWork.Repository<Adult>();

        public Adult FindAdultByID(int AdultId) => UnitOfWork.Repository<Adult>().Get().Where(x => x.AdultId == AdultId).SingleOrDefault();

        public Adult FetchById(int id)
        {
            return AdultRepository
                .GetAll()
                .Where(x => x.AdultId == id)
                .Include(x => x.HIVStatus)
                .Include(x => x.Household)
                .Include(x => x.KinshipToFamilyHead)
                .FirstOrDefault();
        }


        public Adult findByGuid(Guid guid) => AdultRepository.GetAll().Where(a => a.AdultGuid == guid).FirstOrDefault();

        public Adult findbySyncGuid(Guid adultGuid) => AdultRepository.GetAll().Where(a => a.SyncGuid == adultGuid).FirstOrDefault();

        public List<UniqueEntity> FindAllAdultUniqueEntity() => UnitOfWork.DbContext.Database.SqlQuery<UniqueEntity>("Select ISNULL(SyncGuid, cast(cast(0 as binary) as uniqueidentifier)) As SyncGuid, AdultID As ID from Adult").ToList();

        public IQueryable<Adult> findByname(string name)
        {
            return AdultRepository.GetAll().Where(e => e.FirstName.Contains(name) || e.LastName.Contains(name))
                .Include(e => e.Household);
        }

        public IQueryable<Adult> findBynameAndPartnerAndCode(string name, string partnerName, string beneficiaryCode)
        {
            return AdultRepository.GetAll()
                .Where(e =>
                (name == null || name.Equals("") || e.FirstName.Contains(name) || e.LastName.Contains(name))
                &&
                (partnerName == null || partnerName.Equals("") || e.Household.Partner.Name.Contains(partnerName))
                &&
                (beneficiaryCode == null || beneficiaryCode.Equals("") || e.Code.Contains(beneficiaryCode)))
                .Include(e => e.Household);
        }

        public List<Adult> FetchByHouseholdIDandStatusID(int householdID, int StatusID)
        {
            return AdultRepository.GetAll()
                .Include(e => e.ChildStatusHistories.Select(x => x.ChildStatus))
                .Where(e => e.ChildStatusHistories.OrderByDescending(x => x.EffectiveDate)
                .ThenByDescending(x => x.ChildStatusHistoryID).FirstOrDefault().ChildStatus.StatusID != StatusID)
                .Where(x => x.HouseholdID == householdID).ToList();
        }

        public Adult FetchByAdultIDandStatusID(int adultdID, int StatusID)
        {
            return AdultRepository.GetAll()
                .Include(e => e.ChildStatusHistories.Select(x => x.ChildStatus))
                .Where(e => e.ChildStatusHistories.OrderByDescending(x => x.EffectiveDate)
                .ThenByDescending(x => x.ChildStatusHistoryID).FirstOrDefault().ChildStatus.StatusID != StatusID)
                .Where(x => x.AdultId == adultdID).SingleOrDefault();
        }

        public void Delete(Adult Adult)
        {
            UnitOfWork.Repository<HIVStatus>().GetAll().Where(x => x.AdultID == Adult.AdultId).ToList().ForEach(x => UnitOfWork.Repository<HIVStatus>().Delete(x));
            UnitOfWork.Repository<ChildStatusHistory>().GetAll().Where(x => x.AdultID == Adult.AdultId).ToList().ForEach(x => UnitOfWork.Repository<ChildStatusHistory>().Delete(x));
            AdultRepository.Delete(Adult);
        }

        public int CountBySite(Site site)
        {
            return 
                UnitOfWork.DbContext.Database.SqlQuery<int>(
                    @"select Count(*) from Beneficiary b 
                    inner join HouseHold hh on (hh.HouseHoldID = b.HouseHoldID)
                    inner join Partner p on (hh.PartnerID = p.PartnerID)
                    where (p.SiteID = @SiteID or @SiteID = 0) and DATEDIFF(year, CAST(b.DateOfBirth As Date), GETDATE()) >= 18", 
                    new SqlParameter("SiteID", site.SiteID)).SingleOrDefault();
        }

        public Adult Reload(Adult entity)
        {
            AdultRepository.FullReload(entity);
            return entity;
        }

        public void Save(Adult Adult)
        {           
            if (Adult.AdultId > 0)
            { AdultRepository.Update(Adult); } 
            else
            { AdultRepository.Add(Adult); }
        }

        public int ImportAdults(string path)
        {
            _logger.Information("IMPORTACAO DE ADULTOS ...");
            UnitOfWork.DbContext.Configuration.AutoDetectChangesEnabled = false;
            FileImporter imp = new FileImporter();
            string fullPath = path + @"\Adults.csv";

            int ImportedAdults = 0;
            string lastGuidToImport = null;
            UsersDB = ConvertListToHashtableUsers(findAllUsersUniqueEntities());
            List<Adult> AdultsToPersist = new List<Adult>();
            List<UniqueEntity> AdultsDB = FindAllAdultUniqueEntity();
            List<UniqueEntity> HIVStatusDB = HIVStatusQueryService.FindAllHIVStatusUniqueEntity();
            List<UniqueEntity> HouseholdsDB = HouseHoldservice.findAllHouseholdUniqueEntities();
            IEnumerable<DataRow> AdultRows = imp.ImportFromCSV(fullPath).Rows.Cast<DataRow>();

            try
            {
                foreach (DataRow row in AdultRows)
                {
                    Guid adultGuid = new Guid(row["AdultGuid"].ToString());
                    lastGuidToImport = adultGuid.ToString();
                    UniqueEntity Household = FindBySyncGuid(HouseholdsDB, new Guid(row["HouseHoldGuid"].ToString()));
                    UniqueEntity HIVStatus = FindBySyncGuid(HIVStatusDB, new Guid(row["HIVStatusGuid"].ToString()));

                    if (Household == null)
                    { _logger.Error("Adulto com o Guid '{0}' tem HouseHold com Guid '{1}' em falta. Este nao sera importado.", adultGuid, row["HouseHoldGuid"].ToString()); }
                    else if (HIVStatus == null)
                    { _logger.Error("Adulto com o Guid '{0}' tem HIVStatus com Guid '{1}' em falta. Este nao sera importado.", adultGuid, row["HIVStatusGuid"].ToString()); }
                    else
                    {
                        Adult adult = (FindBySyncGuid(AdultsDB, adultGuid) == null) ? new Adult() : findbySyncGuid(adultGuid);
                        adult.Code = row["Code"].ToString();
                        adult.FirstName = row["FirstName"].ToString();
                        adult.LastName = row["LastName"].ToString();
                        adult.IsHouseHoldChef = row["IsHouseHoldChefInt"].ToString().Equals("1") ? true : false;
                        if (row["MaritalStatusID"].ToString().Length > 0) { adult.MaritalStatusID = int.Parse(row["MaritalStatusID"].ToString()); }
                        adult.DateOfBirth = (row["DateOfBirth"].ToString()).Length == 0 ? (DateTime?) null : DateTime.Parse(row["DateOfBirth"].ToString());
                        adult.DateOfBirthUnknown = row["IsDateOfBirthUnknownInt"].ToString().Equals("1") ? true : false;
                        adult.RegistrationDate = (row["RegistrationDate"].ToString()).Length == 0 ? (DateTime?) null : DateTime.Parse(row["RegistrationDate"].ToString());
                        adult.RegistrationDateDifferentFromHouseholdDate = row["IsRegistrationDateDifferentFromHouseholdDateInt"].ToString().Equals("1") ? true : false;
                        adult.HIVTracked = row["IsHIVTrackedInt"].ToString().Equals("1") ? true : false;
                        adult.ContactNo = row["ContactNo"].ToString();
                        adult.IsPartSavingGroup = row["IsPartSavingGroupInt"].ToString().Equals("1") ? true : false;
                        adult.HIV = row["HIV"].ToString();
                        adult.HIVInTreatment = int.Parse(row["HIVInTreatment"].ToString());
                        adult.HIVUndisclosedReason = int.Parse(row["HIVUndisclosedReason"].ToString());
                        if (row["ChildID"].ToString().Length > 0) { adult.ChildID = int.Parse(row["ChildID"].ToString()); }
                        adult.Gender = row["Gender"].ToString();
                        adult.IDNumber = row["IDNumber"].ToString();
                        adult.HouseholdID = Household.ID;
                        adult.HIVStatusID = HIVStatus.ID;
                        if (row["KinshipToFamilyHeadID"].ToString().Length > 0) { adult.KinshipToFamilyHeadID = int.Parse(row["KinshipToFamilyHeadID"].ToString()); }
                        if (row["OtherKinshipToFamilyHead"].ToString().Length > 0) { adult.OtherKinshipToFamilyHead = row["OtherKinshipToFamilyHead"].ToString(); }
                        SetCreationDataFields(adult, row, adultGuid);
                        SetUpdatedDataFields(adult, row);
                        AdultsToPersist.Add(adult);
                        ImportedAdults++;
                    }


                    if (ImportedAdults % 100 == 0)
                    { _logger.Information(ImportedAdults + " de " + AdultRows.Count() + " Adultos importados."); }
                }

                AdultsToPersist.ForEach(a => Save(a));
                UnitOfWork.Commit();
                Rename(fullPath, fullPath + IMPORTED);
            }
            catch (Exception e)
            {
                _logger.Information("Erro ao importar o Guid : " + lastGuidToImport);
                _logger.Error(e, "Erro ao importar Adultos", null);
                throw e;
            }
            finally
            {
                UnitOfWork.Dispose();
            }

            return ImportedAdults;
        }

        public bool AdultValidation(string firstname, string lastname, DateTime? birthdate)
        {
            return (from adult in UnitOfWork.DbContext.Adults
                    where adult.FirstName == firstname
                    && adult.LastName == lastname
                    && adult.DateOfBirth == birthdate
                    select adult.FirstName).Any();
        }

        public bool MoreThanTwoAdultsOnHousehold(int householdID)
        {
            int countAdults = (from adults in UnitOfWork.DbContext.Adults
                               where adults.HouseholdID == householdID
                               select adults.AdultId).Count();

            if (countAdults >= 2)
            {
                return true;
            }
            return false;
        }

        public int LockData() => LockAndUnlockData(3, new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, 20));

        public int UnlockData(DateTime unlockDate) => LockAndUnlockData(0, unlockDate);

        public int LockAndUnlockData(int state, DateTime lockOrUnlockDate)
        {
            UnitOfWork.DbContext.Configuration.AutoDetectChangesEnabled = false;
            List<Adult> adultsToLockOrUnlock = UnitOfWork.Repository<Adult>().GetAll().Where(x => x.Household.RegistrationDate <= lockOrUnlockDate).ToList();

            foreach (Adult a in adultsToLockOrUnlock)
            {
                a.State = state;
                Save(a);
            }

            Commit();

            UnitOfWork.DbContext.Configuration.AutoDetectChangesEnabled = true;
            return adultsToLockOrUnlock.Count();
        }

        public void CreateAdultFromChild(Child child, User user)
        {
            DateTime currentDate = DateTime.Today;
            ChildStatusHistory lastStatusOfGrownChild = child.ChildStatusHistories.OrderByDescending(x => x.EffectiveDate).ThenByDescending(x => x.ChildStatusHistoryID).FirstOrDefault();

            // Creating the new Adult From Grown Child
            Adult newAdult = new Adult
            {
                AdultGuid = Guid.NewGuid(),
                Code = child.Code,
                FirstName = child.FirstName,
                LastName = child.LastName,
                DateOfBirth = child.DateOfBirth,
                Household = child.Household,
                CreatedDate = currentDate,
                CreatedUser = user,
                LastUpdatedDate = currentDate,
                LastUpdatedUser = user,
                Gender = child.Gender,
                HIVStatus = child.HIVStatus,
                KinshipToFamilyHeadID = child.KinshipToFamilyHeadID,
                OtherKinshipToFamilyHead = child.OtherKinshipToFamilyHead,
                ChildID = child.ChildID
            };
            UnitOfWork.Repository<Adult>().Add(newAdult);
            UnitOfWork.Commit();

            // Creating the new HIVStatus for new Adult
            HIVStatus newHIV = new HIVStatus
            {
                AdultID = newAdult.AdultId,
                ChildID = 0,
                InformationDate = child.HIVStatus.InformationDate,
                HIV = child.HIVStatus.HIV,
                HIVStatusID = child.HIVStatus.HIVStatusID,
                HIVInTreatment = child.HIVStatus.HIVInTreatment,
                HIVUndisclosedReason = child.HIVStatus.HIVUndisclosedReason,
                CreatedAt = currentDate,
                BeneficiaryGuid = newAdult.AdultGuid,
                User = user
            };
            UnitOfWork.Repository<HIVStatus>().Add(newHIV);

            // Creating new status for new Adult
            ChildStatusHistory newStatusAdult = new ChildStatusHistory
            {
                Adult = newAdult,
                ChildStatus = lastStatusOfGrownChild.ChildStatus,
                childstatushistory_guid = Guid.NewGuid(),
                EffectiveDate = currentDate,
                BeneficiaryGuid = newAdult.AdultGuid,
                CreatedDate = currentDate,
                CreatedUser = user,
            };
            UnitOfWork.Repository<ChildStatusHistory>().Add(newStatusAdult);
            UnitOfWork.Commit();
        }

        /*
         * #######################################################
         * ############## Graduated Adult Report #################
         * #######################################################
         */

        public List<GraduatedAdultReportDTO> getGraduatedAdultReport(DateTime initialDate, DateTime lastDate, int PartnerID, int ChildID, int ChildStatusID)
        {
            
            String query = @"SELECT	
                                    --csh.[ChildStatusHistoryID] AS ChildStatusID
		                            --p.PartnerID As PartnerID
		                            p.Name AS PartnerName
		                            --, (csh.[ChildID]) AS ChildID
		                            , (a.FirstName) + ' ' + (a.LastName) As FullNameAdult
		                            , c.Gender AS Gender
		                            , DATEDIFF(YEAR, a.DateOFBirth, GETDATE()) AS Age
		                            --,csh.[EffectiveDate] AS ChildStatusDate
		                            --,row_number() over(partition by csh.[ChildID] order by csh.[EffectiveDate] desc) as LastChildStatusDate
		                            --,csh.[ChildStatusID] AS ChildStatusID
		                            ,ct.Description

                            FROM 
	                             [ChildStatusHistory] AS csh
                            INNER JOIN 
	                             [Child] AS c ON c.[ChildID] = csh.[ChildID]
                            INNER JOIN
	                             [HouseHold] AS hh ON hh.HouseHoldID = c.HouseholdID
                            INNER JOIN
	                             Adult AS a ON a.HouseHoldID = hh.HouseholdID
                            INNER JOIN
	                             [Partner] AS p ON hh.PartnerID = p.PartnerID
                            INNER JOIN
	                             [ChildStatus] AS ct ON csh.[ChildStatusID] = ct.[StatusID]
                            WHERE
	                            ct.Description = 'Saiu devido a graduação'";

            return UnitOfWork.DbContext.Database.SqlQuery<GraduatedAdultReportDTO>(query).ToList(); 
        }
    }
}
