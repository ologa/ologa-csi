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
    public class BeneficiaryStatusService : BaseService
    {
        private UserService UserService;

        public BeneficiaryStatusService(UnitOfWork uow) : base(uow) => UserService = new UserService(uow);

        private Repository.IRepository<ChildStatusHistory> ChildStatusHistoryRepository => UnitOfWork.Repository<ChildStatusHistory>();

        private Repository.IRepository<ChildStatus> ChildStatusRepository => UnitOfWork.Repository<ChildStatus>();

        public ChildStatus FindChildStatusByDescription(string name) => ChildStatusRepository.GetAll().Where(cs => cs.Description.Equals(name)).FirstOrDefault();

        public List<ChildStatusHistory> FetchChildStatusHistory(Child child) => ChildStatusHistoryRepository.GetAll().Where(x => x.ChildID == child.ChildID).Include(x => x.ChildStatus).ToList();

        public List<ChildStatusHistory> FetchAdultStatusHistory(Adult adult) => ChildStatusHistoryRepository.GetAll().Where(x => x.AdultID == adult.AdultId).Include(x => x.ChildStatus).ToList();

        public List<ChildStatus> FindAllChildStatuses() => ChildStatusRepository.GetAll().ToList();

        public List<UniqueEntity> FindAllBeneficiaryUniqueEntities() => UnitOfWork.DbContext.Database.SqlQuery<UniqueEntity>("Select ISNULL(SyncGuid, cast(cast(0 as binary) as uniqueidentifier)) As SyncGuid, BeneficiaryID As ID from Beneficiary").ToList();

        public List<UniqueEntity> FindAllChildStatusHistoryUniqueEntities() => UnitOfWork.DbContext.Database.SqlQuery<UniqueEntity>("Select ISNULL(SyncGuid, cast(cast(0 as binary) as uniqueidentifier)) As SyncGuid, ChildStatusHistoryID As ID from ChildStatusHistory").ToList();

        public ChildStatusHistory findChildStatusHistoryBySyncGuid(Guid ChildStatusHistoryGuid) { return UnitOfWork.Repository<ChildStatusHistory>().GetAll().Where(c => c.SyncGuid == ChildStatusHistoryGuid).FirstOrDefault(); }

        public List<ChildStatusHistory> FetchChildStatusHistoryByChild(Child child) => UnitOfWork.Repository<ChildStatusHistory>().GetAll().Where(x => x.ChildID == child.ChildID).Include(x => x.ChildStatus).ToList();

        public List<ChildStatusHistory> FetchChildStatusHistoryByBeneficiary(Beneficiary beneficiary) => UnitOfWork.Repository<ChildStatusHistory>().GetAll().Where(x => x.BeneficiaryID == beneficiary.BeneficiaryID).Include(x => x.ChildStatus).ToList();

        public ChildStatusHistory findByID(int childStatusHistoryID) { return ChildStatusHistoryRepository.GetAll().Where(h => h.ChildStatusHistoryID == childStatusHistoryID).FirstOrDefault(); }

        public ChildStatusHistory fetchByID(int childStatusHistoryID)
        {
            ChildStatusHistory childStatusHistory = UnitOfWork.Repository<ChildStatusHistory>().GetAll()
                .Include(x => x.ChildStatus)
                .Where(j => j.ChildStatusHistoryID == childStatusHistoryID)
                .FirstOrDefault();
            return childStatusHistory;
        }

        public Int32 FindHoueholdLowerChildStatus(int householdId)
        {
            return UnitOfWork.DbContext.Database.SqlQuery<Int32>
                ("select min (ChildStatusId) as MinChildStatusID " +
                    "from " +
                    "(select hh.HouseHoldID as HouseHold, c.ChildID as Child, MIN(csh.ChildStatusID) as ChildStatusId " +
                    " from HouseHold hh " +
                    "inner join Child c on (c.HouseholdID = hh.HouseHoldID) " +
                    "inner join ChildStatusHistory csh on (csh.ChildID = c.ChildID) " +
                    "where hh.HouseHoldID = " + householdId.ToString() +
                    " group by hh.HouseHoldID, c.ChildID) " +
                    "q").FirstOrDefault();
        }

        public void Delete(ChildStatusHistory csh)
        {
            ChildStatusHistoryRepository.Delete(csh);
            this.Commit();
        }

        public void SaveOrUpdate(ChildStatusHistory csh)
        {
            if (csh.ChildStatusHistoryID == 0) { ChildStatusHistoryRepository.Add(csh); }
            else { ChildStatusHistoryRepository.Update(csh); }
            this.Commit();
        }

        public int ImportData(string path)
        {
            UnitOfWork.DbContext.Configuration.AutoDetectChangesEnabled = false;
            FileImporter imp = new FileImporter();
            string fullPah = path + @"\ChildStatusHistories.csv";

            string lastGuidToImport = null;
            int ImportedChildStatusHistory = 0;
            List<User> UDB = UnitOfWork.Repository<User>().GetAll().ToList();
            List<UniqueEntity> BeneficiariesDB = FindAllBeneficiaryUniqueEntities();
            List<UniqueEntity> ChildStatusHistoriesDB = FindAllChildStatusHistoryUniqueEntities();
            List<ChildStatusHistory> ChildStatusHistoryToPersist = new List<ChildStatusHistory>();
            IEnumerable<DataRow> ChildStatusRows = imp.ImportFromCSV(fullPah).Rows.Cast<DataRow>();
            
            try
            {
                foreach (DataRow row in ChildStatusRows)
                {
                    Guid ChildStatusHistoryGuid = new Guid(row["childstatushistory_guid"].ToString());
                    if (FindBySyncGuid(ChildStatusHistoriesDB, ChildStatusHistoryGuid) == null)
                    {
                        UniqueEntity BeneficiaryUE = null;
                        lastGuidToImport = ChildStatusHistoryGuid.ToString();
                        User user = UDB.Where(x => x.SyncGuid == new Guid(row["CreatedUserGuid"].ToString())).SingleOrDefault();

                        ChildStatusHistory childStatusHistory = new ChildStatusHistory
                        {
                            EffectiveDate = DateTime.Parse(row["EffectiveDate"].ToString()),
                            CreatedDate = DateTime.Parse(row["CreatedDate"].ToString()),
                            ChildStatusID = int.Parse(row["ChildStatusID"].ToString()),
                            CreatedUserID = user == null ? 1 : user.UserID,
                            SyncGuid = ChildStatusHistoryGuid,
                            SyncDate = DateTime.Now
                        };

                        if (!isZerosGuid(row["BeneficiaryGuid"].ToString()))
                        {
                            childStatusHistory.BeneficiaryGuid = new Guid(row["BeneficiaryGuid"].ToString());
                            BeneficiaryUE = FindBySyncGuid(BeneficiariesDB, childStatusHistory.BeneficiaryGuid);
                            if (BeneficiaryUE != null) { childStatusHistory.BeneficiaryID = BeneficiaryUE.ID; }
                        }

                        if (isZerosGuid(row["CreatedUserGuid"].ToString()))
                        { _logger.Error("Não foi encontrado nenhum usuário com Guid '{0}'. A autoria da criacao deste estado sera do usuario com ID = 1", row["CreatedUserGuid"].ToString()); }
                        if (BeneficiaryUE == null)
                        { _logger.Error("Estado do Beneficiarios não tem o Guid do Beneficiario, actualmente o valor é '{0}'. O Estado com Guid '{1}' nao sera importado.", row["BeneficiaryGuid"].ToString(), ChildStatusHistoryGuid); }
                        else
                        {
                            ChildStatusHistoryToPersist.Add(childStatusHistory);
                            ImportedChildStatusHistory++;
                        }
                    }

                    if (ImportedChildStatusHistory % 100 == 0)
                    { _logger.Information(ImportedChildStatusHistory + " de " + ChildStatusRows.Count() + " Estados de Crianca importados."); }
                }
                ChildStatusHistoryToPersist.ForEach(csh => UnitOfWork.Repository<ChildStatusHistory>().Add(csh));
                UnitOfWork.Commit();
                Rename(fullPah, fullPah + IMPORTED);
            }
            catch (Exception e)
            {
                _logger.Information("Erro ao importar o Guid : " + lastGuidToImport);
                _logger.Error(e, "Erro ao importar Criancas", null);
                throw e;
            }

            return ChildStatusHistoryToPersist.Count();
        }
    }
}
