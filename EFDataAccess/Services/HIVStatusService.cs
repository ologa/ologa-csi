using EFDataAccess.UOW;
using System;
using System.Linq;
using VPPS.CSI.Domain;
using System.Data;
using System.Collections.Generic;
using EFDataAccess.Logging;
using System.Data.SqlClient;
using EFDataAccess.DTO;

namespace EFDataAccess.Services
{
    public class HIVStatusService : BaseService
    {
        private UserService UserService;
        private HIVStatusQueryService HIVStatusQueryService;

        public HIVStatusService(UnitOfWork uow) : base(uow)
        {
            UserService = new UserService(uow);
            HIVStatusQueryService = new HIVStatusQueryService(uow);
        }

        private EFDataAccess.Repository.IRepository<HIVStatus> HIVStatusRepository { get { return UnitOfWork.Repository<HIVStatus>(); } }

        public List<UniqueEntity> FindAllBeneficiaryUniqueEntities() => UnitOfWork.DbContext.Database.SqlQuery<UniqueEntity>("Select ISNULL(SyncGuid, cast(cast(0 as binary) as uniqueidentifier)) As SyncGuid, BeneficiaryID As ID from Beneficiary").ToList();

        public List<HIVStatus> findAllBeneficiaryStatuses(Beneficiary beneficiary) { return HIVStatusRepository.GetAll().Where(x => x.BeneficiaryID == beneficiary.BeneficiaryID).ToList(); }

        public HIVStatus findByGuid(Guid guid) { return HIVStatusRepository.GetAll().Where(h => h.HIVStatus_guid == guid).FirstOrDefault(); }

        public HIVStatus findByID(int hivstatusID) { return HIVStatusRepository.GetAll().Where(h => h.HIVStatusID == hivstatusID).FirstOrDefault(); }

        public HIVStatus findHIVStatusBySyncGuid(Guid guid) { return HIVStatusRepository.GetAll().Where(h => h.SyncGuid == guid).FirstOrDefault(); }

        public HIVStatus findHIVStatusByHIVAndInTreatmentAndUndisclosedReasonAndDate(String HIV, int InTreatment, int UndisclosedReason, int ChildID, DateTime InformationDate)
        {
            HIVStatus h = UnitOfWork.Repository<HIVStatus>().GetAll().Where(
                e => e.HIV == HIV && e.HIVInTreatment == InTreatment &&
                e.HIVUndisclosedReason == UndisclosedReason &&
                e.InformationDate.Equals(InformationDate) && e.ChildID == ChildID).FirstOrDefault();
            return h;
        }

        public void Delete(HIVStatus hivstatus) { HIVStatusRepository.Delete(hivstatus); }

        public void SaveOrUpdate(HIVStatus HIVStatus)
        {
            if (HIVStatus.HIVStatusID == 0) { UnitOfWork.Repository<HIVStatus>().Add(HIVStatus); }
            else { UnitOfWork.Repository<HIVStatus>().Update(HIVStatus); }
        }

        public int ImportData(string path, int time)
        {
            _logger.Information("IMPORTACAO ESTADOS DE HIV ...");

            UnitOfWork.DbContext.Configuration.AutoDetectChangesEnabled = false;

            FileImporter imp = new FileImporter();
            string fullPath = path + @"\HIVStatus.csv";
            DataTable data = imp.ImportFromCSV(fullPath);

            String HIVStatusGuidString = null;
            List<UniqueEntity> BeneficiariesDB = FindAllBeneficiaryUniqueEntities();
            List<UniqueEntity> HIVStatusDB = HIVStatusQueryService.FindAllHIVStatusUniqueEntity();
            UsersDB = ConvertListToHashtableUsers(findAllUsersUniqueEntities());

            int ImportedHIVStatus = 0;

            try
            {
                foreach (DataRow row in data.Rows)
                {
                    UniqueEntity BeneficiaryUE = null;
                    HIVStatusGuidString = row["HIVStatus_guid"].ToString();
                    Guid HIVStatusGuid = new Guid(HIVStatusGuidString);
                    HIVStatus HIVStatus = FindBySyncGuid(HIVStatusDB, new Guid(HIVStatusGuidString)) == null ? new HIVStatus() : findHIVStatusBySyncGuid(HIVStatusGuid);
                    User user = (User) UsersDB[new Guid(row["CreatedUserGuid"].ToString())];
                    HIVStatus.CreatedAt = DateTime.Parse(row["CreatedAt"].ToString());
                    HIVStatus.SyncGuid = HIVStatusGuid;
                    HIVStatus.HIV = row["HIV"].ToString();
                    HIVStatus.NID = row["NID"] != null ? row["NID"].ToString() : HIVStatus.NID;
                    HIVStatus.HIVInTreatment = int.Parse(row["HIVInTreatment"].ToString());
                    HIVStatus.HIVUndisclosedReason = int.Parse(row["HIVUndisclosedReason"].ToString());
                    HIVStatus.InformationDate = DateTime.Parse(row["InformationDate"].ToString());
                    HIVStatus.SyncDate = DateTime.Now;
                    HIVStatus.UserID = user == null ? 1 : user.UserID;

                    if (!isZerosGuid(row["BeneficiaryGuid"].ToString()))
                    {
                        HIVStatus.BeneficiaryGuid = new Guid(row["BeneficiaryGuid"].ToString());
                        BeneficiaryUE = FindBySyncGuid(BeneficiariesDB, HIVStatus.BeneficiaryGuid);
                        if (BeneficiaryUE != null) { HIVStatus.BeneficiaryID = BeneficiaryUE.ID; }
                    }

                    if (isZerosGuid(row["CreatedUserGuid"].ToString()))
                    { _logger.Error("Nao foi encontrado nenhum usuario com Guid '{0}'. A autoria da criacao deste estado de HIV sera do usuario com ID = 1", row["CreatedUserGuid"].ToString()); }

                    // Importamos HIVStatus sem Beneficiarios, pois na criação
                    // ainda não existem beneficiarios importados
                    SaveOrUpdate(HIVStatus);
                    ImportedHIVStatus++;

                    if (ImportedHIVStatus % 100 == 0)
                    { _logger.Information(ImportedHIVStatus + " de " + data.Rows.Count + " estados de HIV importados." + ((time == 1) ? " [Criacao]" : " [Actualizacao]")); }
                }

                UnitOfWork.Commit();
            }
            catch (Exception e)
            {
                _logger.Information("Erro ao importar o Guid {0} ", HIVStatusGuidString);
                _logger.Error(e, "Erro ao importar Estados de HIV", null);
                throw e;
            }
            finally
            {
                if (time == 2)
                {
                    Rename(fullPath, fullPath + IMPORTED);
                    UnitOfWork.Dispose();
                }
            }

            return ImportedHIVStatus;
        }
    }
}
