using EFDataAccess.CustomQuery;
using EFDataAccess.DTO;
using EFDataAccess.UOW;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using VPPS.CSI.Domain;

namespace EFDataAccess.Services
{
    public class HouseholdService : BaseService
    {
        private PartnerService partnerService;

        public HouseholdService(UnitOfWork uow) : base (uow)
        {
            partnerService = new PartnerService(uow);
        }

        private EFDataAccess.Repository.IRepository<Household> HouseholdRepository
        {
            get { return UnitOfWork.Repository<Household>(); }
        }
        
        public List<UniqueEntity> findAllHouseholdUniqueEntities() => UnitOfWork.DbContext.Database.SqlQuery<UniqueEntity>("Select ISNULL(SyncGuid, cast(cast(0 as binary) as uniqueidentifier)) As SyncGuid, HouseHoldID As ID from Household").ToList();

        public List<UniqueEntity> findAllAidUniqueEntities() => UnitOfWork.DbContext.Database.SqlQuery<UniqueEntity>("Select ISNULL(SyncGuid, cast(cast(0 as binary) as uniqueidentifier)) As SyncGuid, AidID As ID from Aid").ToList();

        private List<UniqueEntity> findlAllOrgUnitUniqueEntities() => UnitOfWork.DbContext.Database.SqlQuery<UniqueEntity>("Select ISNULL(OrgUnit_Guid, cast(cast(0 as binary) as uniqueidentifier)) As SyncGuid, OrgUnitID As ID from OrgUnit").ToList();

        public Household fetchHouseholdById(int householdID)
        {
            Household h = UnitOfWork.Repository<Household>().GetAll()
                .Where(e => e.HouseHoldID == householdID)
                .Include(e => e.Partner.site.orgUnit)
                .Include(e => e.RoutineVisits)
                .Include(e => e.Adults.Select(c => c.ChildStatusHistories.Select(x => x.ChildStatus)))
                .Include(e => e.Children.Select(c => c.ChildStatusHistories.Select(x => x.ChildStatus)))
                .Include(e => e.Beneficiaries.Select(c => c.ChildStatusHistories.Select(x => x.ChildStatus)))
                .Include(e => e.Partner)
                .Include(e => e.OrgUnit)
                .Include(e => e.FamilyHead)
                .Include(e => e.FamilyOriginRef)
                .Include(e => e.CreatedUser)
                .Include(e => e.LastUpdatedUser)
                .Include(e => e.Aid)
                .Include(e => e.HouseholdSupportPlans)
                .Include(e => e.HouseholdStatusHistories.Select(c => c.HouseholdStatus))
                .FirstOrDefault();

            HouseholdRepository.GetDbEntry(h)
                .Collection(e => e.Children).Query().Include(e => e.ChildStatusHistories.Select(x => x.ChildStatus))
                .Where(e => e.ChildStatusHistories.OrderByDescending(x => x.EffectiveDate).ThenByDescending(x => x.ChildStatusHistoryID).FirstOrDefault().ChildStatus.StatusID != 5)
                .Load();

            return h;
        }

        public Aid findAidByID(int ID) => UnitOfWork.Repository<Aid>().GetById(ID);

        public Household findHouseHoldBySyncGuid(Guid guid) => HouseholdRepository.GetAll().Where(e => e.SyncGuid == guid).FirstOrDefault();

        public Aid findAidBySyncGuid(Guid syncGuid) => UnitOfWork.Repository<Aid>().GetAll().Where(a => a.SyncGuid == syncGuid).FirstOrDefault();

        public Household findHouseHoldByGuid(Guid guid) => HouseholdRepository.GetAll().Where(e => e.HouseholdUniqueIdentifier == guid).FirstOrDefault();

        public IQueryable<Household> findHouseholdByHouseholdNameAndPartnerNameAndCode(string householdName, string partnerName, string householdCode)
        {
            return UnitOfWork.Repository<Household>().GetAll()
                .Where( e => (householdName == null || householdName.Equals("") || e.HouseholdName.Contains(householdName)) 
                && (partnerName == null || partnerName.Equals("") || e.Partner.Name.Contains(partnerName))
                && (householdCode == null || householdCode.Equals("") || e.Code.Contains(householdCode)))
                .Include(e => e.Beneficiaries)
                .Include(e => e.Partner)
                .Include(e => e.CreatedUser)
                .Include(e => e.LastUpdatedUser);
        }

        public List<Household> findAll()
        {
            return UnitOfWork.Repository<Household>().GetAll().Include(e => e.Adults)
                .Include(e => e.Partner)
                .Include(e => e.CreatedUser)
                .Include(e => e.OrgUnit)
                .Include(e => e.LastUpdatedUser)
                .Include(e => e.Aid)
                .Include(e => e.FamilyHead)
                .Include(e => e.FamilyOriginRef)
                .ToList();
        }

        public List<Household> findAllActiveHouseholds()
        {
            List<int> serviceStatusIDs = UnitOfWork.Repository<SimpleEntity>()
                .GetAll()
                .Where(x => x.Type == "ben-services-status" && x.Code == "03")
                .Select(x => x.SimpleEntityID)
                .ToList();

            return UnitOfWork.Repository<Household>()
                .GetAll()
                .Include(h => h.Beneficiaries.Select(x => x.Household))
                .Where(h => h.Beneficiaries.Any(x => serviceStatusIDs.Contains(x.ServicesStatusID.Value)))
                .ToList();
        }

        public List<Household> findAllByPartner(int partnerID)
        {
            return UnitOfWork.Repository<Household>().GetAll()
                .Where(e => e.PartnerID == partnerID)
                .ToList();
        }

        public List<Household> findByOCB(Site site)
        {
            return UnitOfWork.Repository<Household>().GetAll()
            .Include(h => h.Partner)
            .Where(h => site.SiteID == 0 || h.Partner.siteID == site.SiteID).ToList();
        }

        public string findHouseholdNameByID(int householdID)
        {
            Household household = UnitOfWork.Repository<Household>().GetAll().Where(e => e.HouseHoldID == householdID).SingleOrDefault();
            return household.HouseholdName;
        }

        public DateTime getHouseholdRegistrationDate(int HouseholdID)
        {
            DateTime regDate = new DateTime();

            var household = HouseholdRepository.GetAll().Where(b => b.HouseHoldID == HouseholdID).FirstOrDefault();
            regDate = household.RegistrationDate.Value;

            return regDate;
        }
        

        public IEnumerable<Object> searchHouseholds(string householdName, int partnerID)
        {
            CustomQueryExecutor queryExecutor = new CustomQueryExecutor(UnitOfWork);
            return queryExecutor.SearchHouseholds(householdName, partnerID);
        }

        public Household Reload(Household entity)
        {
            HouseholdRepository.FullReload(entity);
            return entity;
        }

        public void Delete(Household household)
        {
            HouseholdRepository.Delete(household);
        }

        public void Attach(Household household)
        {
            UnitOfWork.DbContext.Households.Attach(household);
        }

        public void Save(Household household, User user, Aid aid)
        {
            if (household.Aid == null || (household.Aid != null && household.Aid.AidID == 0))
            {
                household.Aid = aid;
                UnitOfWork.Repository<Aid>().Add(aid);
            }
            else
            {
                household.Aid.communityAid = aid.communityAid;
                household.Aid.InstitutionalAid = aid.InstitutionalAid;
                household.Aid.individualAid = aid.individualAid;
                UnitOfWork.Repository<Aid>().Update(aid);
            }

            if (household.HouseHoldID > 0)
            {
                household.LastUpdatedUser = user;
                HouseholdRepository.Update(household);
            } 
            else
            {
                household.CreatedUser = user;
                household.LastUpdatedUser = user;
                HouseholdRepository.Add(household);
            }    
        }

        public void CreateHouseholdFull(Household household)
        {
            UnitOfWork.Repository<Aid>().Add(household.Aid);
            HouseholdRepository.Add(household);
        }

        public void UpdateHouseholdFull(Household household)
        {
            bool saveFailed = false;
            do
            {
                try
                {
                    SaveOrUpdate(household);
                    SaveOrUpdateAid(household.Aid);
                    Commit();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    saveFailed = true;
                    // Update the values of the entity that failed to save from the store 
                    ex.Entries.Single().Reload();
                }
            }
            while (saveFailed);
        }

        public void SaveOrUpdate(Household household)
        {
            if (household.HouseHoldID == 0)
            { UnitOfWork.Repository<Household>().Add(household); }
            else
            { UnitOfWork.Repository<Household>().Update(household); }
        }

        public void UpdateHouseholdWithNewPartner(Household household, int partnerID)
        {
            if (partnerID != 0)
            {
                household.PartnerID = partnerID;
                UnitOfWork.Repository<Household>().Update(household);
                Commit();
            }
               
        }

        public void SaveOrUpdateAid(Aid aid)
        {
            if(aid.AidID == 0)
            { UnitOfWork.Repository<Aid>().Add(aid); }
            else
            { UnitOfWork.Repository<Aid>().Update(aid); }
        }

        public List<Partner> findAllPartners()
        {
            return UnitOfWork.Repository<Partner>().GetAll()
                .Include(p => p.site)
                .ToList();
        }

        public int ImportData(string path)
        {
            _logger.Information("IMPORTACAO DE AGREGADOS ...");

            UnitOfWork.DbContext.Configuration.AutoDetectChangesEnabled = false;
            FileImporter imp = new FileImporter();

            int ImportedAids = 0;
            int ImportedHouseholds = 0;
            string lastGuidToImport = null;

            string fullPathAids = path + @"\Aids.csv";
            DataTable aidsDataTable = imp.ImportFromCSV(fullPathAids);
            Hashtable AidsDB = ConvertListToHashtable(findAllAidUniqueEntities());
            UsersDB = ConvertListToHashtableUsers(findAllUsersUniqueEntities());

            foreach (DataRow row in aidsDataTable.Rows)
            {
                Guid aidGuid = new Guid(row["Aid_guid"].ToString());
                Guid CreatedUserGuid = new Guid(row["CreatedUserGuid"].ToString());
                lastGuidToImport = aidGuid.ToString();
                int AidID = ParseStringToIntSafe(AidsDB[aidGuid]);
                Aid aid = (AidID > 0) ? findAidByID(AidID) : new Aid();
                aid.InstitutionalAid = row["InstitutionalAidStr"].ToString().Equals("1") ? true : false;
                aid.communityAid = row["communityAidStr"].ToString().Equals("1") ? true : false;
                aid.individualAid = row["individualAidStr"].ToString().Equals("1") ? true : false;
                aid.InstitutionalAidDetail = (row["InstitutionalAidDetail"].ToString()).Length == 0 ? null : row["InstitutionalAidDetail"].ToString();
                aid.communityAidDetail = (row["communityAidDetail"].ToString()).Length == 0 ? null : row["communityAidDetail"].ToString();
                SetCreationDataFields(aid, row, aidGuid);
                SetUpdatedDataFields(aid, row);
                SaveOrUpdateAid(aid);
                ImportedAids++;

                if (ImportedAids % 100 == 0)
                { _logger.Information(ImportedAids + " de " + aidsDataTable.Rows.Count + " Ajudas dos Agregados importadas."); }
            }
            UnitOfWork.Commit();
            Rename(fullPathAids, fullPathAids + IMPORTED);

            string fullPathHHs = path + @"\Households.csv";
            DataTable householdsDataTable = imp.ImportFromCSV(fullPathHHs);
            Hashtable HHDB = new Hashtable();
            Hashtable PartnersDB = new Hashtable();
            Hashtable OrgUnitsDB = new Hashtable();

            if (householdsDataTable.Rows.Count > 0)
            {
                HHDB = ConvertListToHashtable(findAllHouseholdUniqueEntities());
                PartnersDB = ConvertListToHashtable(partnerService.findAllPartnerUniqueEntities());
                AidsDB = ConvertListToHashtable(findAllAidUniqueEntities());
                OrgUnitsDB = ConvertListToHashtable(findlAllOrgUnitUniqueEntities());
            }
            try
            {
                foreach (DataRow row in householdsDataTable.Rows)
                {
                    Guid hh_guid = new Guid(row["HouseholdUniqueIdentifier"].ToString());
                    lastGuidToImport = hh_guid.ToString();

                    int FamilyOriginRefID = row["FamilyOriginRefId"].ToString().Length == 0 ? 0 : int.Parse(row["FamilyOriginRefId"].ToString());
                    int FamilyHeadID = row["FamilyHeadId"].ToString().Length == 0 ? 0 : int.Parse(row["FamilyHeadId"].ToString());
                    int PartnerID = ParseStringToIntSafe(PartnersDB[new Guid(row["PartnerGuid"].ToString())]);
                    int AidID = ParseStringToIntSafe(AidsDB[new Guid(row["AidGuid"].ToString())]);

                    if (FamilyHeadID != 0 && FamilyOriginRefID != 0 && PartnerID != 0 && AidID != 0)
                    {
                        Household household = (HHDB[hh_guid] == null) ? new Household() : findHouseHoldBySyncGuid(hh_guid);
                        household.Code = row.Table.Columns.Contains("Code") ? row["Code"].ToString() : "";
                        household.HouseNumber = row["HouseNumber"].ToString();
                        household.OtherFamilyHead = row["OtherFamilyHead"].ToString();
                        household.Address = row["Address"].ToString();
                        household.Block = row["Block"].ToString();
                        household.FamilyPhoneNumber = row["FamilyPhoneNumber"].ToString();
                        household.ClosePlaceToHome = row["ClosePlaceToHome"].ToString();
                        household.NeighborhoodName = row["NeighborhoodName"].ToString();
                        household.HouseholdName = row["HouseholdName"].ToString();
                        household.OfficialHouseholdIdentifierNumber = int.Parse(row["OfficialHouseholdIdentifierNumber"].ToString());
                        household.OtherFamilyOriginRef = row["OtherFamilyOriginRef"].ToString();
                        household.PrincipalChiefName = row["PrincipalChiefName"].ToString();
                        household.AnyoneBedridden = row["AnyoneBedriddenInt"].ToString().Equals("1") ? true : false;
                        household.AidID = AidID;
                        household.RegistrationDate = (row["RegistrationDate"].ToString()).Length == 0 ? (DateTime?) null : DateTime.Parse(row["RegistrationDate"].ToString());
                        household.FamilyOriginRefId = FamilyOriginRefID;
                        household.FamilyHeadId = FamilyHeadID;
                        household.OrgUnitID = getNonDuplicatedOrgUnitID(OrgUnitsDB, int.Parse(row["OrgUnitID"].ToString()), lastGuidToImport);
                        household.PartnerID = PartnerID;
                        SetCreationDataFields(household, row, hh_guid);
                        SetUpdatedDataFields(household, row);
                        SaveOrUpdate(household);
                        ImportedHouseholds++;
                    }
                    else
                    { _logger.Information("Agregado nao importado por falta de dados. Guid : " + lastGuidToImport); }

                    if (ImportedHouseholds != 0 && ImportedHouseholds % 100 == 0)
                    { _logger.Information(ImportedHouseholds + " de " + householdsDataTable.Rows.Count + " Agregados importados."); }
                }
                UnitOfWork.Commit();
                Rename(fullPathHHs, fullPathHHs + IMPORTED);
            }
            catch (Exception e)
            {
                _logger.Information("Erro ao importar o Guid : " + lastGuidToImport);
                _logger.Error(e, "Erro ao importar Agregados", null);
                throw e;
            }
            finally
            {
                UnitOfWork.Dispose();
            }

            return ImportedHouseholds;
        }

        public int LockData() => LockAndUnlockData(3, new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, 20));

        public int UnlockData(DateTime unlockDate) => LockAndUnlockData(0, unlockDate);

        public int LockAndUnlockData(int state, DateTime lockOrUnlockDate)
        {
            UnitOfWork.DbContext.Configuration.AutoDetectChangesEnabled = false;
            List<Household> householdsToLock = UnitOfWork.Repository<Household>().GetAll().Include(x => x.Aid).Where(x => x.RegistrationDate <= lockOrUnlockDate).ToList();

            foreach (Household hh in householdsToLock)
            {
                hh.State = state;
                hh.Aid.State = state;
                SaveOrUpdate(hh);
                SaveOrUpdateAid(hh.Aid);
            }

            Commit();
            UnitOfWork.DbContext.Configuration.AutoDetectChangesEnabled = true;
            return householdsToLock.Count();
        }

        public void LockByHouseholdStatus(int state, int householdID)
        {
            UnitOfWork.DbContext.Configuration.AutoDetectChangesEnabled = false;
            Household householdToLock = UnitOfWork.Repository<Household>().GetAll().Include(x => x.Aid).Where(x => x.HouseHoldID == householdID).SingleOrDefault();

            householdToLock.State = state;
            householdToLock.Aid.State = state;
            SaveOrUpdate(householdToLock);
            SaveOrUpdateAid(householdToLock.Aid);
            
            List<Beneficiary> BeneficiaryToLockOrUnlock = UnitOfWork.Repository<Beneficiary>().GetAll().Where(x => x.HouseholdID == householdID).ToList();
            Commit();
            UnitOfWork.DbContext.Configuration.AutoDetectChangesEnabled = true;
        }

        public bool householdValidationback(string hhname, string hhphone, string hhNeighborhood, string hhNumber, string hhBlock)
        {
            //Se Bairro, Número da casa e Quarteirao forem iguais e nome for diferente retorna true
            return (from house in UnitOfWork.DbContext.Households where house.HouseholdName == hhname
                    && house.NeighborhoodName == hhNeighborhood 
                    && house.HouseNumber == hhNumber
                    && house.Block == hhBlock
                    select house.HouseholdName).Any();
        }

         public bool householdValidation(string hhname, string hhphone)
        {
            if (string.IsNullOrWhiteSpace(hhphone))
            {
                return (from house in UnitOfWork.DbContext.Households
                        where house.HouseholdName == hhname
                        && house.FamilyPhoneNumber == hhphone
                        && !house.FamilyPhoneNumber.Equals(string.Empty)
                        select house.HouseholdName).Any();
            }
            else
            {
                return (from house in UnitOfWork.DbContext.Households
                        where house.HouseholdName == hhname
                        && house.FamilyPhoneNumber == hhphone
                        select house.HouseholdName).Any();
            }
            
        }
    }
}
