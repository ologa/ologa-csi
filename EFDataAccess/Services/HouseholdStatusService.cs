using EFDataAccess.UOW;
using System;
using System.Linq;
using VPPS.CSI.Domain;
using System.Data;
using System.Data.Entity;
using System.Collections.Generic;
using EFDataAccess.Logging;

namespace EFDataAccess.Services
{
    public class HouseholdStatusService : BaseService
    {
        private UserService UserService;

        public HouseholdStatusService(UnitOfWork uow) : base(uow) => UserService = new UserService(uow);

        private Repository.IRepository<HouseholdStatusHistory> HouseholdStatusHistoryRepository => UnitOfWork.Repository<HouseholdStatusHistory>();

        private Repository.IRepository<HouseholdStatus> HouseholdStatusRepository => UnitOfWork.Repository<HouseholdStatus>();

        public HouseholdStatus getHouseholdStatusByID(int statusID) => HouseholdStatusRepository.GetAll().Where(x => x.HouseholdStatusID == statusID).FirstOrDefault();

        public HouseholdStatus getHouseholdStatusByDescription(string description) => HouseholdStatusRepository.GetAll().Where(x => x.Description == description).FirstOrDefault();

        public HouseholdStatusHistory findHouseholdStatusHistoryByID(int householdStatusHistoryByID) => HouseholdStatusHistoryRepository.GetAll().Where(h => h.HouseholdStatusHistoryID == householdStatusHistoryByID).FirstOrDefault();

        public List<UniqueEntity> FindAllHouseholdUniqueEntities() => UnitOfWork.DbContext.Database.SqlQuery<UniqueEntity>("Select ISNULL(SyncGuid, cast(cast(0 as binary) as uniqueidentifier)) As SyncGuid, HouseholdID As ID from Household").ToList();

        public List<UniqueEntity> FindAllHouseholdStatusHistoryUniqueEntities() => UnitOfWork.DbContext.Database.SqlQuery<UniqueEntity>("Select ISNULL(SyncGuid, cast(cast(0 as binary) as uniqueidentifier)) As SyncGuid, HouseholdStatusHistoryID As ID from HouseholdStatusHistory").ToList();

        public void SaveOrUpdateHouseholdStatusHistory(HouseholdStatusHistory hhStatusHistory)
        {
            if (hhStatusHistory.HouseholdStatusHistoryID == 0)
            {
                UnitOfWork.Repository<HouseholdStatusHistory>().Add(hhStatusHistory);
                Commit();
            }
            else
            {
                UnitOfWork.Repository<HouseholdStatusHistory>().Update(hhStatusHistory);
                Commit();
            }
        }

        public void DeleteHouseholdStatusHistory(HouseholdStatusHistory hhStatusHistory)
        {
            UnitOfWork.Repository<HouseholdStatusHistory>().Delete(hhStatusHistory);
        }

        public int ImportData(string path)
        {
            _logger.Information("IMPORTACAO DOS ESTADOS DOS AGREGADOS FAMILIARES ...");

            UnitOfWork.DbContext.Configuration.AutoDetectChangesEnabled = false;
            FileImporter imp = new FileImporter();
            string fullPah = path + @"\HouseholdStatusHistories.csv";

            string lastGuidToImport = null;
            int ImportedHouseholdStatusHistory = 0;
            List<User> UDB = UnitOfWork.Repository<User>().GetAll().ToList();
            List<UniqueEntity> HouseholdDB = FindAllHouseholdUniqueEntities();
            List<UniqueEntity> HouseholdStatusHistoriesDB = FindAllHouseholdStatusHistoryUniqueEntities();
            List<HouseholdStatusHistory> HouseholdStatusHistoryToPersist = new List<HouseholdStatusHistory>();
            DataTable HouseholdStatusHistorysDataTable = imp.ImportFromCSV(fullPah);
            IEnumerable<DataRow> HouseholdStatusRows = HouseholdStatusHistorysDataTable.Rows.Cast<DataRow>();

            try
            {
                foreach (DataRow row in HouseholdStatusRows)
                {
                    Guid HouseholdStatusHistoryGuid = new Guid(row["HouseholdStatusHistory_Guid"].ToString());
                    if (FindBySyncGuid(HouseholdStatusHistoriesDB, HouseholdStatusHistoryGuid) == null)
                    {                        
                        UniqueEntity HouseholdUE = null;
                        lastGuidToImport = HouseholdStatusHistoryGuid.ToString();
                        User user = UDB.Where(x => x.SyncGuid == new Guid(row["CreatedUserGuid"].ToString())).SingleOrDefault();

                        HouseholdStatusHistory householdStatusHistory = new HouseholdStatusHistory
                        {
                            RegistrationDate = DateTime.Parse(row["RegistrationDate"].ToString()),
                            CreatedDate = DateTime.Parse(row["CreatedDate"].ToString()),
                            HouseholdStatusID = int.Parse(row["HouseholdStatusID"].ToString()),
                            CreatedUserID = user == null ? 1 : user.UserID,
                            SyncGuid = HouseholdStatusHistoryGuid,
                            SyncDate = DateTime.Now
                        };

                        if (!isZerosGuid(row["HouseholdGuid"].ToString()))
                        {
                            Guid HouseholdGuid = new Guid(row["HouseholdGuid"].ToString());
                            HouseholdUE = FindBySyncGuid(HouseholdDB, HouseholdGuid);
                            if (HouseholdUE != null) { householdStatusHistory.HouseholdID = HouseholdUE.ID; }
                        }

                        if (isZerosGuid(row["CreatedUserGuid"].ToString()))
                        { _logger.Error("Não foi encontrado nenhum usuário com Guid '{0}'. A autoria da criacao deste estado sera do usuario com ID = 1", row["CreatedUserGuid"].ToString()); }
                        if (HouseholdUE == null)
                        { _logger.Error("Nao existe nenhum Household com o Guid '{0}'. O Estado com Guid '{1}' nao sera importado.", row["HouseholdGuid"].ToString(), HouseholdStatusHistoryGuid); }
                        else
                        {
                            HouseholdStatusHistoryToPersist.Add(householdStatusHistory);
                            ImportedHouseholdStatusHistory++;
                        }
                    }

                    if (ImportedHouseholdStatusHistory % 100 == 0)
                    { _logger.Information(ImportedHouseholdStatusHistory + " de " + HouseholdStatusRows.Count() + " Estados do Agregado importados."); }
                }
                HouseholdStatusHistoryToPersist.ForEach(csh => UnitOfWork.Repository<HouseholdStatusHistory>().Add(csh));
                UnitOfWork.Commit();
                Rename(fullPah, fullPah + IMPORTED);
            }
            catch (Exception e)
            {
                _logger.Information("Erro ao importar o Guid : " + lastGuidToImport);
                _logger.Error(e, "Erro ao importar Agregados", null);
                throw e;
            }

            return HouseholdStatusHistoryToPersist.Count();
        }
    }
}
