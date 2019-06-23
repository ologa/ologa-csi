using EFDataAccess.UOW;
using System;
using System.Linq;
using VPPS.CSI.Domain;
using System.Data;
using System.Data.Entity;
using System.Collections.Generic;
using EFDataAccess.Logging;
using EFDataAccess.DTO;
using System.Data.SqlClient;

namespace EFDataAccess.Services
{
    public class HouseholdSupportPlanService : BaseService
    {
        private UserService UserService;
        private HouseholdService HouseholdService;

        public HouseholdSupportPlanService(UnitOfWork uow) : base(uow)
        {
            UserService = new UserService(uow);
            HouseholdService = new HouseholdService(uow);
        }

        private Repository.IRepository<HouseholdSupportPlan> HouseholdSupportPlanRepository => UnitOfWork.Repository<HouseholdSupportPlan>();
        public List<UniqueEntity> FindAllHouseholdSupportPlanEntities() => UnitOfWork.DbContext.Database.SqlQuery<UniqueEntity>("Select ISNULL(SyncGuid, cast(cast(0 as binary) as uniqueidentifier)) As SyncGuid, HouseHoldSupportPlanID As ID from HouseholdSupportPlan").ToList();

        public void SaveOrUpdateHouseholdSupportPlan(HouseholdSupportPlan supportPlan)
        {
            if (supportPlan.HouseHoldSupportPlanID == 0)
            {
                UnitOfWork.Repository<HouseholdSupportPlan>().Add(supportPlan);
                Commit();
            }
            else
            {
                UnitOfWork.Repository<HouseholdSupportPlan>().Update(supportPlan);
                Commit();
            }
        }

        public void DeleteHouseholdSupportPlan(HouseholdSupportPlan supportPlan)
        {
            UnitOfWork.Repository<HouseholdSupportPlan>().Delete(supportPlan);
            Commit();
        }

        public HouseholdSupportPlan findHouseholdSupportPlanByID(int householdSupportPlanByID)
        {
            return UnitOfWork.Repository<HouseholdSupportPlan>()
                .GetAll().Where(h => h.HouseHoldSupportPlanID == householdSupportPlanByID)
                .Include(h => h.Household)
                .FirstOrDefault();
        }

        public List<HouseholdSupportPlanDTO> FindHouseholdSupportPlanByDates(DateTime initialDate, DateTime lastDate)
        {
            String query = @"select
	                            b.FirstName, 
	                            b.LastName, 
	                            DATEDIFF(year, CAST(b.DateOfBirth As Date), GETDATE()) As Age, 
	                            b.Gender,
	                            h.HouseholdName, 
	                            p.Name As PartnerName,
	                            CASE WHEN b.RegistrationDate IS NULL THEN h.RegistrationDate ELSE b.RegistrationDate END As RegistrationDate,
                                hsp.SupportPlanInitialDate,
                                hsp.SupportPlanFinalDate                         
                            from Beneficiary b
                            inner join Household h on (b.HouseholdID = h.HouseholdID)
                            inner join [Partner] p on (p.PartnerID = h.PartnerID)
                            inner join HouseholdSupportPlan hsp on (hsp.HouseholdID = hsp.HouseholdID)
                            where (hsp.SupportPlanInitialDate > @initialDate and hsp.SupportPlanInitialDate < @lastDate)
                            or (hsp.SupportPlanInitialDate > @initialDate and @lastDate is null)";

            return UnitOfWork.DbContext.Database.SqlQuery<HouseholdSupportPlanDTO>(query,
                                                new SqlParameter("initialDate", initialDate),
                                                new SqlParameter("lastDate", lastDate)).ToList();
        }

        public int ImportData(string path)
        {
            _logger.Information("IMPORTACAO DOS PLANOS DE ACCAO DOS AGREGADOS FAMILIARES ...");

            UnitOfWork.DbContext.Configuration.AutoDetectChangesEnabled = false;
            FileImporter imp = new FileImporter();
            string fullPah = path + @"\HouseholdSupportPlans.csv";

            string lastGuidToImport = null;
            int ImportedHouseholdPlanHistory = 0;
            List<User> UDB = UserService.findAll();
            List<UniqueEntity> HouseholdDB = HouseholdService.findAllHouseholdUniqueEntities();
            List<UniqueEntity> HouseholdSupportPlansDB = FindAllHouseholdSupportPlanEntities();
            List<HouseholdSupportPlan> HouseholdSupportPlanToPersist = new List<HouseholdSupportPlan>();
            DataTable HouseholdSupportPlanHistorysDataTable = imp.ImportFromCSV(fullPah);
            IEnumerable<DataRow> HouseholdPlanRows = HouseholdSupportPlanHistorysDataTable.Rows.Cast<DataRow>();

            try
            {
                foreach (DataRow row in HouseholdPlanRows)
                {
                    Guid HouseholdSupportPlanGuid = new Guid(row["Householdsupportplan_Guid"].ToString());
                    if (FindBySyncGuid(HouseholdSupportPlansDB, HouseholdSupportPlanGuid) == null)
                    {                       
                        UniqueEntity HouseholdUE = null;
                        lastGuidToImport = HouseholdSupportPlanGuid.ToString();
                        User user = UDB.Where(x => x.SyncGuid == new Guid(row["CreatedUserGuid"].ToString())).SingleOrDefault();

                        HouseholdSupportPlan householdSupportPlan = new HouseholdSupportPlan
                        {
                            SupportPlanInitialDate = (row["SupportPlanInitialDate"].ToString()).Length == 0 ? (DateTime?)null : DateTime.Parse(row["SupportPlanInitialDate"].ToString()),
                            SupportPlanFinalDate = (row["SupportPlanFinalDate"].ToString()).Length == 0 ? (DateTime?)null : DateTime.Parse(row["SupportPlanFinalDate"].ToString()),
                            CreatedDate = (row["CreatedDate"].ToString()).Length == 0 ? (DateTime?)null : DateTime.Parse(row["CreatedDate"].ToString()),
                            HouseHoldSupportPlanStatus = int.Parse(row["HouseHoldSupportPlanStatus"].ToString()),
                            CreatedUserID = user == null ? 1 : user.UserID,
                            SyncGuid = HouseholdSupportPlanGuid,
                            SyncDate = DateTime.Now
                        };

                        if (!isZerosGuid(row["HouseholdGuid"].ToString()))
                        {
                            Guid HouseholdGuid = new Guid(row["HouseholdGuid"].ToString());
                            HouseholdUE = FindBySyncGuid(HouseholdDB, HouseholdGuid);
                            if (HouseholdUE != null) { householdSupportPlan.HouseholdID = HouseholdUE.ID; }
                        }

                        if (isZerosGuid(row["CreatedUserGuid"].ToString()))
                        { _logger.Error("Não foi encontrado nenhum usuário com Guid '{0}'. A autoria da criacao deste Plano de Suporte do Agregado sera do usuario com ID = 1", row["CreatedUserGuid"]); }
                        if (HouseholdUE == null)
                        { _logger.Error("Nao existe nenhum Household com o Guid '{0}'. O Plano de Suporte do Agregado com Guid '{1}' nao sera importado.", row["HouseholdGuid"], row["Householdsupportplan_Guid"]); }
                        else
                        {
                            HouseholdSupportPlanToPersist.Add(householdSupportPlan);
                            ImportedHouseholdPlanHistory++;
                        }
                    }

                    if (ImportedHouseholdPlanHistory % 100 == 0)
                    { _logger.Information(ImportedHouseholdPlanHistory + " de " + HouseholdPlanRows.Count() + " Estados do Agregado importados."); }
                }

                HouseholdSupportPlanToPersist.ForEach(csh => UnitOfWork.Repository<HouseholdSupportPlan>().Add(csh));
                UnitOfWork.Commit();
                Rename(fullPah, fullPah + IMPORTED);
            }
            catch (Exception e)
            {
                _logger.Information("Erro ao importar o Guid : " + lastGuidToImport);
                _logger.Error(e, "Erro ao importar Agregados", null);
                throw e;
            }

            return HouseholdSupportPlanToPersist.Count();
        }
    }
}
