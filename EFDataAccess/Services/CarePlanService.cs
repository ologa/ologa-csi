using EFDataAccess.UOW;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using VPPS.CSI.Domain;

namespace EFDataAccess.Services
{
    public class CarePlanService : BaseService
    {
        public CarePlanService(UnitOfWork uow) : base(uow) {}

        private EFDataAccess.Repository.IRepository<CarePlan> CarePlanRepository => UnitOfWork.Repository<CarePlan>();

        public List<UniqueEntity> FindAllCSIUniqueEntity() => UnitOfWork.DbContext.Database.SqlQuery<UniqueEntity>("Select SyncGuid, CSIID As ID from CSI").ToList();

        public List<UniqueEntity> FindAllAnswerUniqueEntities() => UnitOfWork.DbContext.Database.SqlQuery<UniqueEntity>("Select AnswerGUID As SyncGuid, AnswerID As ID from Answer").ToList();

        public List<UniqueEntity> FindAllSupportServiceTypeUniqueEntities() => UnitOfWork.DbContext.Database.SqlQuery<UniqueEntity>("Select supportservicetype_guid As SyncGuid, SupportServiceTypeID As ID from SupportServiceType").ToList();

        public List<UniqueEntity> FindAllCarePlanUniqueEntity() => UnitOfWork.DbContext.Database.SqlQuery<UniqueEntity>("Select SyncGuid, CarePlanID As ID from CarePlan").ToList();

        public List<UniqueEntity> FindAllCarePlanDomainUniqueEntity() => UnitOfWork.DbContext.Database.SqlQuery<UniqueEntity>("Select SyncGuid, CarePlanDomainID As ID from CarePlanDomain").ToList();

        public List<UniqueEntity> FindAllCarePlanDomainSupportServiceUniqueEntity() => UnitOfWork.DbContext.Database.SqlQuery<UniqueEntity>("Select SyncGuid, CarePlanDomainSupportServiceID As ID from CarePlanDomainSupportService").ToList();

        public List<UniqueEntity> FindAllTasksUniqueEntity() => UnitOfWork.DbContext.Database.SqlQuery<UniqueEntity>("Select SyncGuid, TaskID As ID from Tasks").ToList();

        public DomainEntity findDomainByCode(string DomainCode) => UnitOfWork.Repository<DomainEntity>().GetAll().Where(e => e.DomainCode == DomainCode).FirstOrDefault();

        public Question findQuestionByCode(string questionCode) => UnitOfWork.Repository<Question>().GetAll().Where(e => e.Code == questionCode).FirstOrDefault();

        public Resource findResourceByID(int ResourceID) => UnitOfWork.Repository<Resource>().GetAll().Where(e => e.ResourceID == ResourceID).FirstOrDefault();

        public Answer findAnswerByID(int answerID) => UnitOfWork.Repository<Answer>().GetAll().Where(e => e.AnswerID == answerID).FirstOrDefault();

        public CarePlan findCarePlanBySyncGuid(Guid CarePlan_sync_guid) => UnitOfWork.Repository<CarePlan>().Get().Where(x => x.SyncGuid == CarePlan_sync_guid).SingleOrDefault();

        public CarePlanDomain findCarePlanDomainBySyncGuid(Guid SyncGuid) => UnitOfWork.Repository<CarePlanDomain>().Get().Where(x => x.SyncGuid == SyncGuid).SingleOrDefault();

        public CarePlanDomainSupportService findCarePlanDomainSupportServiceBySyncGuid(Guid SyncGuid) => UnitOfWork.Repository<CarePlanDomainSupportService>().Get().Where(x => x.SyncGuid == SyncGuid).SingleOrDefault();

        public VPPS.CSI.Domain.Task findTaskBySyncGuid(Guid taskGuid) => UnitOfWork.Repository<VPPS.CSI.Domain.Task>().Get().Where(x => x.SyncGuid == taskGuid).SingleOrDefault();

        public VPPS.CSI.Domain.Task findTaskByID(int taskID) => UnitOfWork.Repository<VPPS.CSI.Domain.Task>().Get().Where(x => x.TaskID == taskID).SingleOrDefault();

        public CarePlan fetchCarePlanByID(int CarePlanID)
        {
            return UnitOfWork.Repository<CarePlan>().GetAll()
                .Where(x => x.CarePlanID == CarePlanID)
                .Include(x => x.CSI.CSIDomains.Select(y => y.CSIDomainScores.Select(z => z.Question)))
                .Include(x => x.CSI.CSIDomains.Select(y => y.CSIDomainScores.Select(z => z.Question.Domain)))
                .Include(x => x.CSI.CSIDomains.Select(y => y.CSIDomainScores.Select(z => z.Answer)))
                .Include(x => x.CSI.CSIDomains.Select(y => y.CSIDomainScores.Select(z => z.Answer.Score)))
                .Include(x => x.CarePlanDomains.Select(y => y.Domain))
                .Include(x => x.CarePlanDomains.Select(y => y.CarePlanDomainSupportServices))
                .Include(x => x.CarePlanDomains.Select(y => y.CarePlanDomainSupportServices.Select(z => z.SupportServiceType)))
                .Include(x => x.CarePlanDomains.Select(y => y.CarePlanDomainSupportServices.Select(z => z.Question)))
                .Include(x => x.CarePlanDomains.Select(y => y.CarePlanDomainSupportServices.Select(z => z.Answer.Score)))
                .Include(x => x.CarePlanDomains.Select(y => y.CarePlanDomainSupportServices.Select(z => z.Tasks)))
                .FirstOrDefault();
        }

        public VPPS.CSI.Domain.Task fetchTaskID(int TaskID)
        {
            return UnitOfWork.Repository<VPPS.CSI.Domain.Task>().GetAll().Where(x => x.TaskID == TaskID).Include(x => x.CarePlanDomainSupportService.CarePlanDomain.Domain).FirstOrDefault();
        }

        public void SaveOrUpdateCarePlan(CarePlan CarePlan)
        {
            if (CarePlan.CarePlanID == 0)
            { CarePlanRepository.Add(CarePlan); }
            else
            { CarePlanRepository.Update(CarePlan); }
        }

        public void DeleteTask(VPPS.CSI.Domain.Task task)
        {
            UnitOfWork.Repository<VPPS.CSI.Domain.Task>().Delete(task);
        }

        public void SaveOrUpdateCarePlanDomain(CarePlanDomain CarePlanDomain)
        {
            if (CarePlanDomain.CarePlanDomainID == 0)
            { UnitOfWork.Repository<CarePlanDomain>().Add(CarePlanDomain); }
            else
            { UnitOfWork.Repository<CarePlanDomain>().Update(CarePlanDomain); }
        }

        public void SaveOrUpdateCarePlanDomainSupportService(CarePlanDomainSupportService CarePlanDomainSupportService)
        {
            if (CarePlanDomainSupportService.CarePlanDomainSupportServiceID == 0)
            { UnitOfWork.Repository<CarePlanDomainSupportService>().Add(CarePlanDomainSupportService); }
            else
            { } // UnitOfWork.DbContext.Entry(CarePlanDomainSupportService).State = EntityState.Modified; }
        }

        public void SaveOrUpdateCPDSS(CarePlanDomainSupportService CarePlanDomainSupportService)
        {
            if (CarePlanDomainSupportService.CarePlanDomainSupportServiceID == 0)
            { UnitOfWork.Repository<CarePlanDomainSupportService>().Add(CarePlanDomainSupportService); }
            else
            { UnitOfWork.Repository<CarePlanDomainSupportService>().Update(CarePlanDomainSupportService); }
        }

        public void SaveOrUpdateTask(VPPS.CSI.Domain.Task task)
        {
            if (task.TaskID == 0)
            { UnitOfWork.Repository<VPPS.CSI.Domain.Task>().Add(task); }
            else
            { } // UnitOfWork.DbContext.Entry(task).State = EntityState.Modified; }
        }

        public void CreateTask(VPPS.CSI.Domain.Task task)
        {
            UnitOfWork.Repository<VPPS.CSI.Domain.Task>().Add(task);
            Commit();
        }

        public void UpdateTask(VPPS.CSI.Domain.Task task)
        {
            UnitOfWork.DbContext.Entry(task).State = EntityState.Modified;
            UnitOfWork.Repository<VPPS.CSI.Domain.Task>().Update(task);
            Commit();
        }

        public CarePlan CreateCarePlanFromCSI(CSI csi, User user)
        {
            CarePlan carePlan = new CarePlan();
            carePlan.CSI = csi;
            carePlan.CSIID = csi.CSIID;
            carePlan.CreatedUser = user;
            carePlan.LastUpdatedUser = user;
            carePlan.CarePlanDate = DateTime.Now;
            carePlan.CarePlanDomains = new HashSet<CarePlanDomain>();

            foreach (CSIDomain csiDomain in csi.CSIDomains)
            {
                CarePlanDomain carePlanDomain = new CarePlanDomain();
                carePlanDomain.CarePlanID = carePlan.CarePlanID;
                carePlanDomain.CreatedUser = user;
                carePlanDomain.LastUpdatedUser = user;
                carePlanDomain.DomainID = csiDomain.Domain.DomainID;
                carePlan.CarePlanDomains.Add(carePlanDomain);
            }

            return carePlan;
        }

        public void CreateCarePlanFull(CarePlan carePlan)
        {
            UnitOfWork.Repository<CarePlan>().Add(carePlan);
            carePlan.CarePlanDomains.ToList().ForEach(x => UnitOfWork.Repository<CarePlanDomain>().Add(x));
            carePlan.CarePlanDomains.ToList().ForEach(x => x.CarePlanDomainSupportServices.ToList().ForEach(y => UnitOfWork.Repository<CarePlanDomainSupportService>().Add(y)));
            //carePlan.CarePlanDomains.ToList().ForEach(x => x.CarePlanDomainSupportServices.ToList().ForEach(y => y.Tasks.ToList().ForEach(z => UnitOfWork.Repository<VPPS.CSI.Domain.Task>().Add(z))));
            Commit();
        }

        public void UpdateCarePlanFull(CarePlan carePlan)
        {
            UnitOfWork.Repository<CarePlan>().Update(carePlan);
            carePlan.CarePlanDomains.ToList().ForEach(x => UnitOfWork.Repository<CarePlanDomain>().Update(x));
            carePlan.CarePlanDomains.ToList().ForEach(x => x.CarePlanDomainSupportServices.ToList().ForEach(y => SaveOrUpdateCPDSS(y)));
            //carePlan.CarePlanDomains.ToList().ForEach(x => x.CarePlanDomainSupportServices.ToList().ForEach(y => y.Tasks.ToList().ForEach(z => UnitOfWork.Repository<VPPS.CSI.Domain.Task>().Update(z))));
            Commit();
        }

        public void DeleteCarePlanFull(CarePlan carePlan)
        {
            carePlan.CarePlanDomains.ToList().ForEach(x => x.CarePlanDomainSupportServices.ToList().ForEach(y => y.Tasks.ToList().ForEach(z => UnitOfWork.Repository<VPPS.CSI.Domain.Task>().Delete(z))));
            carePlan.CarePlanDomains.ToList().ForEach(x => x.CarePlanDomainSupportServices.ToList().ForEach(y => UnitOfWork.Repository<CarePlanDomainSupportService>().Delete(y)));
            carePlan.CarePlanDomains.ToList().ForEach(x => UnitOfWork.Repository<CarePlanDomain>().Delete(x));
            UnitOfWork.Repository<CarePlan>().Delete(carePlan);
            Commit();
        }

        public int ImportData(string path)
        {
            _logger.Information("IMPORTACAO DE PLANOS DE ACCAO ...");

            UnitOfWork.DbContext.Configuration.AutoDetectChangesEnabled = false;

            string lastGuidToImport = null;
            FileImporter imp = new FileImporter();
            string fullPathCPs = path + @"\CarePlans.csv";
            IEnumerable<DataRow> rowsCP = imp.ImportFromCSV(fullPathCPs).Rows.Cast<DataRow>();

            //////////////// CarePlan ////////////////

            int ImportedCarePlanObjects = 0;
            List<Guid> excludedObjects = new List<Guid>();
            List<CarePlan> CarePlansToPersist = new List<CarePlan>();
            UsersDB = ConvertListToHashtableUsers(findAllUsersUniqueEntities());

            try
            {
                Hashtable CSIsDB = ConvertListToHashtable(FindAllCSIUniqueEntity());
                Hashtable CarePlansDB = ConvertListToHashtable(FindAllCarePlanUniqueEntity());

                Parallel.ForEach(rowsCP, new ParallelOptions { MaxDegreeOfParallelism = MAXTHREADS }, row =>
                {
                    Guid CarePlanGuid = new Guid(row["careplan_guid"].ToString());
                    Guid CSIGuid = new Guid(row["CSIGuid"].ToString());
                    lastGuidToImport = CarePlanGuid.ToString();
                    CarePlan careplan = CarePlansDB[CarePlanGuid] == null ? new VPPS.CSI.Domain.CarePlan() : findCarePlanBySyncGuid(CarePlanGuid);
                    int CSIID = ParseStringToIntSafe(CSIsDB[CSIGuid]);
                    if (CSIID == 0)
                    {
                        _logger.Error("CarePlan '{1}' nao importado, porque o CSI '{0}' nao existe na BD.", CarePlanGuid, CSIGuid);
                        excludedObjects.Add(CarePlanGuid);
                    }
                    else
                    {
                        careplan.CSIID = CSIID;
                        SetCreationDataFields(careplan, row, CarePlanGuid);
                        SetUpdatedDataFields(careplan, row);
                        careplan.CarePlanDate = (row["CarePlanDate"].ToString()).Length == 0 ? careplan.CarePlanDate : DateTime.Parse(row["CarePlanDate"].ToString());
                        CarePlansToPersist.Add(careplan);
                        ImportedCarePlanObjects++;
                    }

                    if (ImportedCarePlanObjects % 100 == 0)
                    { _logger.Information(ImportedCarePlanObjects + " de " + rowsCP.Count() + " Objectos de Planos de Accao (CarePlan) importados."); }
                });
                CarePlansToPersist.ForEach(cp => SaveOrUpdateCarePlan(cp));
                UnitOfWork.Commit();
                Rename(fullPathCPs, fullPathCPs + IMPORTED);


                //////////////// CarePlanDomain ////////////////

                ImportedCarePlanObjects = 0;
                CarePlansDB = ConvertListToHashtable(FindAllCarePlanUniqueEntity());
                List<DomainEntity> DomainsDB = UnitOfWork.Repository<DomainEntity>().GetAll().ToList();
                Hashtable CarePlanDomainsDB = ConvertListToHashtable(FindAllCarePlanDomainUniqueEntity());
                List<CarePlanDomain> CarePlanDomainsToPersist = new List<CarePlanDomain>();
                string fullPathCPDs = path + @"\CarePlanDomains.csv";
                IEnumerable<DataRow> rowsCPD = imp.ImportFromCSV(fullPathCPDs).Rows.Cast<DataRow>();

                Parallel.ForEach(rowsCPD, new ParallelOptions { MaxDegreeOfParallelism = MAXTHREADS }, row =>
                {
                    Guid careplandomain_guid = new Guid(row["careplandomain_guid"].ToString());
                    Guid careplan_guid = new Guid(row["CarePlanGuid"].ToString());
                    string CarePlanDomainCode = row["DomainCode"].ToString();
                    lastGuidToImport = careplandomain_guid.ToString();

                    if (excludedObjects.Contains(careplan_guid))
                    {
                        _logger.Information("CarePlanDomain '{0}' nao importado porque o CarePlan '{1}' nao existe na BD.", careplandomain_guid, careplan_guid);
                        excludedObjects.Add(careplandomain_guid);
                    }
                    else
                    {
                        if (CarePlanDomainsDB[careplandomain_guid] == null)
                        {
                            CarePlanDomain carePlanDomain = new CarePlanDomain();
                            carePlanDomain.CarePlanID = ParseStringToIntSafe(CarePlansDB[careplan_guid]);
                            carePlanDomain.DomainID = DomainsDB.Where(x => x.DomainCode == CarePlanDomainCode).SingleOrDefault().DomainID;
                            SetCreationDataFields(carePlanDomain, row, careplandomain_guid);
                            SetUpdatedDataFields(carePlanDomain, row);
                            CarePlanDomainsToPersist.Add(carePlanDomain);
                            ImportedCarePlanObjects++;

                            if (ImportedCarePlanObjects % 100 == 0)
                            { _logger.Information(ImportedCarePlanObjects + " de " + rowsCPD.Count() + " Objectos de Planos de Accao (CarePlanDomain) importados."); }
                        }
                    }
                });
                CarePlanDomainsToPersist.ForEach(cpd => SaveOrUpdateCarePlanDomain(cpd));
                UnitOfWork.Commit();
                Rename(fullPathCPDs, fullPathCPDs + IMPORTED);

                //////////////// CarePlanDomainSupportService ////////////////

                ImportedCarePlanObjects = 0;
                CarePlansDB = ConvertListToHashtable(FindAllCarePlanDomainUniqueEntity());
                CarePlanDomainsDB = ConvertListToHashtable(FindAllCarePlanDomainUniqueEntity());
                Hashtable AnswersDB = ConvertListToHashtable(FindAllAnswerUniqueEntities());
                Hashtable SupportServiceTypesDB = ConvertListToHashtable(FindAllSupportServiceTypeUniqueEntities());
                Hashtable CarePlanDomainSupportServiceDB = ConvertListToHashtable(FindAllCarePlanDomainSupportServiceUniqueEntity());
                List<CarePlanDomainSupportService> CarePlanDomainSupportServicesToPersist = new List<CarePlanDomainSupportService>();
                string fullPathCPDSSs = path + @"\CarePlanDomainSupportServices.csv";
                IEnumerable<DataRow> rowsCPDSS = imp.ImportFromCSV(fullPathCPDSSs).Rows.Cast<DataRow>();

                Parallel.ForEach(rowsCPDSS, new ParallelOptions { MaxDegreeOfParallelism = MAXTHREADS }, row =>
                {
                    Guid careplandomainsupportservice_guid = new Guid(row["careplandomainsupportservice_guid"].ToString());
                    Guid careplandomain_guid = new Guid(row["CarePlanDomainGuid"].ToString());
                    int QuestionID = row["QuestionID"].ToString().Length == 0 ? 0 : int.Parse(row["QuestionID"].ToString());
                    int AnswerID = row["AnswerID"].ToString().Length == 0 ? 0 : int.Parse(row["AnswerID"].ToString());
                    int SupportServiceTypeID = this.getCorrectSupportServiceTypeID(SupportServiceTypesDB, int.Parse(row["SupportServiceTypeID"].ToString()), lastGuidToImport);
                    lastGuidToImport = careplandomainsupportservice_guid.ToString();

                    if (excludedObjects.Contains(careplandomain_guid))
                    {
                        _logger.Information("CarePlanDomainSupportService '{0}' nao importado porque o CarePlanDomain '{1}' nao existe na BD.", careplandomainsupportservice_guid, careplandomain_guid);
                        excludedObjects.Add(careplandomainsupportservice_guid);
                    }
                    else
                    {
                        if (AnswerID != 0 && !AnswersDB.ContainsValue(AnswerID))
                        { _logger.Information("CarePlanDomainSupportService '{0}' possui resposta '{1}' que nao existe na BD", lastGuidToImport, AnswerID); }

                        if (QuestionID != 0 && SupportServiceTypeID != 0)
                        {
                            CarePlanDomainSupportService CarePlanDomainSupportService = new CarePlanDomainSupportService();
                            CarePlanDomainSupportService.CarePlanDomainSupportServiceID = ParseStringToIntSafe(CarePlanDomainSupportServiceDB[careplandomainsupportservice_guid]);
                            CarePlanDomainSupportService.CarePlanDomainID = ParseStringToIntSafe(CarePlanDomainsDB[careplandomain_guid]);
                            CarePlanDomainSupportService.Description = row["Description"].ToString();
                            CarePlanDomainSupportService.order = (row["order"].ToString()).Length == 0 ? 0 : int.Parse(row["order"].ToString());
                            CarePlanDomainSupportService.QuestionID = QuestionID;
                            CarePlanDomainSupportService.AnswerID = AnswerID == 0 ? CarePlanDomainSupportService.AnswerID : AnswerID;
                            CarePlanDomainSupportService.SupportServiceTypeID = SupportServiceTypeID;
                            SetCreationDataFields(CarePlanDomainSupportService, row, careplandomainsupportservice_guid);
                            SetUpdatedDataFields(CarePlanDomainSupportService, row);
                            CarePlanDomainSupportServicesToPersist.Add(CarePlanDomainSupportService);
                            ImportedCarePlanObjects++;
                        }
                        else
                        {
                            _logger.Information("CarePlanDomainSupportService '{0}' nao importado por falta de dados (QuestionID ou SupportServiceTypeID).", lastGuidToImport);
                        }
                    }

                    if (ImportedCarePlanObjects % 100 == 0)
                    { _logger.Information(ImportedCarePlanObjects + " de " + rowsCPDSS.Count() + " Objectos de Planos de Accao (CarePlanDomainSupportService) importados."); }
                });
                CarePlanDomainSupportServicesToPersist.ForEach(cpdss => SaveOrUpdateCarePlanDomainSupportService(cpdss));
                UnitOfWork.Commit();
                Rename(fullPathCPDSSs, fullPathCPDSSs + IMPORTED);

                //////////////// Tasks ////////////////

                ImportedCarePlanObjects = 0;
                Hashtable TasksDB = ConvertListToHashtable(FindAllTasksUniqueEntity());
                CarePlanDomainSupportServiceDB = ConvertListToHashtable(FindAllCarePlanDomainSupportServiceUniqueEntity());
                List<VPPS.CSI.Domain.Task> TasksToPersist = new List<VPPS.CSI.Domain.Task>();
                string fullPathTks = path + @"\Tasks.csv";
                IEnumerable<DataRow> rowsTk = imp.ImportFromCSV(fullPathTks).Rows.Cast<DataRow>();

                Parallel.ForEach(rowsTk, new ParallelOptions { MaxDegreeOfParallelism = MAXTHREADS }, row =>
                {
                    Guid TaskGuid = new Guid(row["TaskGuid"].ToString());
                    Guid CarePlanDomainSupportServiceGuid = new Guid(row["CarePlanDomainSupportServiceGuid"].ToString());
                    VPPS.CSI.Domain.Task Task = new VPPS.CSI.Domain.Task();
                    Task.TaskID = ParseStringToIntSafe(TasksDB[TaskGuid]);
                    Task.CarePlanDomainSupportServiceID = ParseStringToIntSafe(CarePlanDomainSupportServiceDB[CarePlanDomainSupportServiceGuid]);
                    lastGuidToImport = TaskGuid.ToString();

                    if (excludedObjects.Contains(CarePlanDomainSupportServiceGuid))
                    {   _logger.Information("Task '{0}' nao importado porque o CarePlanDomainSupportService '{1}' nao existe na BD.", TaskGuid, CarePlanDomainSupportServiceGuid); }
                    else
                    {
                        // Only commit if task has is not isolated
                        if (Task.CarePlanDomainSupportServiceID != 0)
                        {
                            Task.ResourceID = row["ResourceID"].ToString().Length != 0 ? int.Parse(row["ResourceID"].ToString()) : Task.ResourceID;
                            Task.Description = row["Description"].ToString();
                            Task.Completed = row["CompletedInt"].ToString().Equals("1") ? true : false;
                            Task.Comments = row["Comments"].ToString();
                            Task.CompleteDate = row["CompleteDate"].ToString().Length == 0 ? Task.CompleteDate : DateTime.Parse(row["CompleteDate"].ToString());
                            SetCreationDataFields(Task, row, TaskGuid);
                            SetUpdatedDataFields(Task, row);
                            TasksToPersist.Add(Task);
                            ImportedCarePlanObjects++;
                        }
                    }

                    if (ImportedCarePlanObjects % 100 == 0)
                    { _logger.Information(ImportedCarePlanObjects + " de " + rowsTk.Count() + " Objectos de Planos de Accao (Tasks) importados."); }
                });
                TasksToPersist.ForEach(t => SaveOrUpdateTask(t));
                UnitOfWork.Commit();
                Rename(fullPathTks, fullPathTks + IMPORTED);
            }
            catch (Exception e)
            {
                _logger.Information("Erro ao importar o Guid : " + lastGuidToImport);
                _logger.Error(e, "Erro ao importar Plano de accao", null);
                throw e;
            }
            finally
            {
                UnitOfWork.Dispose();
            }

            return CarePlansToPersist.Count();
        }
    }
}
