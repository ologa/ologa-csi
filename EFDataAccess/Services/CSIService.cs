using EFDataAccess.UOW;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using VPPS.CSI.Domain;
using System.Web;
using System.Globalization;

namespace EFDataAccess.Services
{
    public class CSIService : BaseService
    {
        private UserService UserService;
        private BeneficiaryService BeneficiaryService;
        private CarePlanService CarePlanService;

        public CSIService(UnitOfWork uow) : base(uow)
        {
            UserService = new UserService(uow);
            BeneficiaryService = new BeneficiaryService(uow);
            CarePlanService = new CarePlanService(uow);
        }

        private EFDataAccess.Repository.IRepository<CSI> CSIRepository
        {
            get { return UnitOfWork.Repository<CSI>(); }
        }

        public VPPS.CSI.Domain.DomainEntity FindDomainByCode(string DomainCode)
        {
            return UnitOfWork.Repository<VPPS.CSI.Domain.DomainEntity>().GetAll().Where(e => e.DomainCode == DomainCode).FirstOrDefault();
        }

        public Question findQuestionByCode(string questionCode)
        {
            return UnitOfWork.Repository<Question>().GetAll().Where(e => e.Code == questionCode).FirstOrDefault();
        }

        public Answer findAnswerByID(int answerID)
        {
            return UnitOfWork.Repository<Answer>().GetAll().Where(e => e.AnswerID == answerID).FirstOrDefault();
        }

        public CSI findCSIBySyncGuid(Guid csi_sync_guid)
        {
            return UnitOfWork.Repository<CSI>().GetAll().Where(x => x.SyncGuid == csi_sync_guid)
                .Include(x => x.Child).Include(x => x.CreatedUser).Include(x => x.LastUpdatedUser).FirstOrDefault();
        }

        public CSIDomain findCSIDomainBySyncGuid(Guid SyncGuid)
        {
            return UnitOfWork.Repository<CSIDomain>().Get().Where(x => x.SyncGuid == SyncGuid).FirstOrDefault();
        }

        public List<CSI> findByChild(Child child)
        {
            return CSIRepository
                .GetAll()
                .Where(csi => csi.ChildID == child.ChildID)
                .ToList();
        }

        public List<CSI> findByBeneficiary(Beneficiary beneficiary)
        {
            return CSIRepository
                .GetAll()
                .Where(csi => csi.BeneficiaryID == beneficiary.BeneficiaryID)
                .ToList();
        }

        public List<CSIDomain> findAllCSIDomains()
        {
            return UnitOfWork.Repository<CSIDomain>().GetAll().Include(x => x.CSI).Include(x => x.Domain).ToList();
        }

        public CSIDomainScore findCSIDomainScoreBySyncGuid(Guid SyncGuid)
        {
            return UnitOfWork.Repository<CSIDomainScore>().Get().Where(x => x.SyncGuid == SyncGuid).FirstOrDefault();
        }

        public List<CSIDomainScore> findAllCSIDomainScores()
        {
            return UnitOfWork.Repository<CSIDomainScore>().GetAll().Include(x => x.CSIDomain).Include(x => x.Question).Include(x => x.Answer).ToList();
        }

        public CSI fetchCSIByID(int CSIID)
        {
            return UnitOfWork.Repository<CSI>().GetAll()
                .Where(x => x.CSIID == CSIID)
                .Include(x => x.CSIDomains.Select(y => y.Domain))
                .Include(x => x.CSIDomains.Select(y => y.CSIDomainScores.Select(z => z.Question)))
                .Include(x => x.CSIDomains.Select(y => y.CSIDomainScores.Select(z => z.Question.Domain)))
                .Include(x => x.CSIDomains.Select(y => y.CSIDomainScores.Select(z => z.Answer)))
                .Include(x => x.CSIDomains.Select(y => y.CSIDomainScores.Select(z => z.Answer.Score)))
                .Include(x => x.Beneficiary)
                .Include(x => x.CarePlans)
                .FirstOrDefault();
        }

        public List<CSIDomainScore> fetchAllCSIDomainScoresByCSIID(int CSIID)
        {
            return UnitOfWork.Repository<CSIDomainScore>().GetAll()
                .Include(x => x.Question.File)
                .Include(x => x.Question.Answers.Select(a => a.File))
                .Include(x => x.Question.Answers.Select(a => a.Score))
                .Where(x => x.CSIDomain.CSI.CSIID == CSIID).ToList();
        }

        public List<Question> fetchAllQuestions()
        {
            return UnitOfWork.Repository<Question>().GetAll()
                .Include(x => x.File)
                .Include(x => x.Domain)
                .Include(x => x.Answers.Select(a => a.File))
                .Include(x => x.Answers.Select(a => a.Score))
                .OrderBy(x => x.Code)
                .ToList();
        }

        public List<Question> fetchVersion2Questions()
        {
            return UnitOfWork.Repository<Question>().GetAll()
                .Where(x => x.QuestionVersion == 2)
                .Include(x => x.File)
                .Include(x => x.Domain)
                .Include(x => x.Answers.Select(a => a.File))
                .Include(x => x.Answers.Select(a => a.Score))
                .OrderBy(x => x.QuestionOrder)
                .ToList();
        }

        public List<Question> fetchVersion3Questions()
        {
            return UnitOfWork.Repository<Question>().GetAll()
                .Where(x => x.QuestionVersion == 3)
                .Include(x => x.File)
                .Include(x => x.Domain)
                .Include(x => x.Answers.Select(a => a.File))
                .Include(x => x.Answers.Select(a => a.Score))
                .OrderBy(x => x.QuestionOrder)
                .ToList();
        }

        public CSI findCSIByID(int CSIID)
        {
            return UnitOfWork.Repository<CSI>().GetById(CSIID);
        }

        public List<UniqueEntity> FindAllCSIUniqueEntity() => UnitOfWork.DbContext.Database.SqlQuery<UniqueEntity>("Select ISNULL(SyncGuid, cast(cast(0 as binary) as uniqueidentifier)) As SyncGuid, CSIID As ID from CSI").ToList();

        public CSI Reload(CSI entity)
        {
            CSIRepository.FullReload(entity);
            return entity;
        }

        public void Delete(CSI CSI) => CSIRepository.Delete(CSI);

        public void SaveOrUpdateCSI(CSI CSI)
        {
            if (CSI.CSIID == 0)
            { CSIRepository.Add(CSI); }
            else
            { CSIRepository.Update(CSI); }
        }

        public void SaveOrUpdateCSIDomain(CSIDomain CSIDomain)
        {
            if (CSIDomain.CSIDomainID == 0)
            { UnitOfWork.Repository<CSIDomain>().Add(CSIDomain); }
        }

        public void SaveOrUpdateCSIDomainScore(CSIDomainScore csiDomainScore)
        {
            if (csiDomainScore.CSIDomainScoreID == 0)
            { UnitOfWork.Repository<CSIDomainScore>().Add(csiDomainScore); }
            else
            { UnitOfWork.Repository<CSIDomainScore>().Update(csiDomainScore); }
        }

        public void UpdateCSIDomainScore(CSIDomainScore csiDomainScore)
        {
            UnitOfWork.Repository<CSIDomainScore>().Update(csiDomainScore);
        }

        public void CreateCSIFull(CSI csi)
        {
            UnitOfWork.Repository<CSI>().Add(csi);
            csi.CSIDomains.ForEach(x => UnitOfWork.Repository<CSIDomain>().Add(x));
            foreach (CSIDomain csiDomain in csi.CSIDomains) { if (csiDomain.CSIDomainScores != null)
                { csiDomain.CSIDomainScores.ToList().ForEach(y => UnitOfWork.Repository<CSIDomainScore>().Add(y)); } }
            Commit();
        }

        public void UpdateCSIFull(CSI csi)
        {
            csi.CSIDomainScores.ForEach(y => UnitOfWork.Repository<CSIDomainScore>().Update(y));
            UnitOfWork.Repository<CSI>().Update(csi);
            Commit();
        }

        public void DeleteCSIFull(CSI csi)
        {
            foreach (CarePlan carePlan in csi.CarePlans.ToList())
            { CarePlanService.DeleteCarePlanFull(CarePlanService.fetchCarePlanByID(carePlan.CarePlanID)); }

            UnitOfWork.Repository<CSI>().Delete(csi);
            csi.CSIDomains.ToList().ForEach(x => UnitOfWork.Repository<CSIDomain>().Delete(x));
            csi.CSIDomains.ToList().ForEach(x => x.CSIDomainScores.ToList().ForEach(y => UnitOfWork.Repository<CSIDomainScore>().Delete(y)));
            Commit();
        }

        public CSI CreateCSIWithoutSave(int BeneficiaryID, User user)
        {
            CSI csi = new CSI();

            ApplicationDbContext context = new ApplicationDbContext();
            Beneficiary B = context.Beneficiaries.Find(BeneficiaryID);

            var Age = B.AgeInYears;

            if (Age <= 17)
            {
                csi.BeneficiaryID = BeneficiaryID;
                csi.Beneficiary = BeneficiaryService.FetchById(BeneficiaryID);
                csi.IndexDate = DateTime.Now;
                csi.CSIDomains = new List<CSIDomain>();
                csi.CSIDomainScores = new List<CSIDomainScore>();

                foreach (VPPS.CSI.Domain.DomainEntity domain in UnitOfWork.Repository<VPPS.CSI.Domain.DomainEntity>().GetAll())
                {
                    CSIDomain csiDomain = new CSIDomain();
                    csiDomain.DomainID = domain.DomainID;
                    csiDomain.Domain = domain;
                    csiDomain.CSIDomainScores = new List<CSIDomainScore>();

                    foreach (Question q in fetchVersion2Questions())
                    {
                        if (q.Domain.DomainID == domain.DomainID)
                        {
                            CSIDomainScore csiDomainScore = new CSIDomainScore();
                            csiDomainScore.QuestionID = q.QuestionID;
                            csiDomainScore.Question = q;
                            csiDomainScore.CreatedUserID = user.UserID;
                            csiDomain.CSIDomainScores.Add(csiDomainScore);
                            csi.CSIDomainScores.Add(csiDomainScore);
                        }
                    }

                    csi.CSIDomains.Add(csiDomain);
                }
            }
            else if (Age >= 18)
            {
                csi.BeneficiaryID = BeneficiaryID;
                csi.Beneficiary = BeneficiaryService.FetchById(BeneficiaryID);
                csi.IndexDate = DateTime.Now;
                csi.CSIDomains = new List<CSIDomain>();
                csi.CSIDomainScores = new List<CSIDomainScore>();

                foreach (VPPS.CSI.Domain.DomainEntity domain in UnitOfWork.Repository<VPPS.CSI.Domain.DomainEntity>().GetAll())
                {
                    CSIDomain csiDomain = new CSIDomain();
                    csiDomain.DomainID = domain.DomainID;
                    csiDomain.Domain = domain;
                    csiDomain.CSIDomainScores = new List<CSIDomainScore>();

                    foreach (Question q in fetchVersion3Questions())
                    {
                        if (q.Domain.DomainID == domain.DomainID)
                        {
                            CSIDomainScore csiDomainScore = new CSIDomainScore();
                            csiDomainScore.QuestionID = q.QuestionID;
                            csiDomainScore.Question = q;
                            csiDomainScore.CreatedUserID = user.UserID;
                            csiDomain.CSIDomainScores.Add(csiDomainScore);
                            csi.CSIDomainScores.Add(csiDomainScore);
                        }
                    }

                    csi.CSIDomains.Add(csiDomain);
                }
            }




            return csi;
        }

        public List<UniqueEntity> FindAllCSIUniqueEntities()
        {
            return UnitOfWork.DbContext.Database
                       .SqlQuery<UniqueEntity>("Select ISNULL(SyncGuid, cast(cast(0 as binary) as uniqueidentifier)) As SyncGuid, CSIID As ID from CSI")
                       .ToList();
        }

        public List<UniqueEntity> FindAllCSIDomainUniqueEntities() 
        {
            return UnitOfWork.DbContext.Database
                       .SqlQuery<UniqueEntity>("Select ISNULL(SyncGuid, cast(cast(0 as binary) as uniqueidentifier)) As SyncGuid, CSIDomainID As ID from CSIDomain")
                       .ToList();
        }

        public List<UniqueEntity> FindAllCSIDomainScoreUniqueEntities()
        {
            return UnitOfWork.DbContext.Database
                       .SqlQuery<UniqueEntity>("Select ISNULL(SyncGuid, cast(cast(0 as binary) as uniqueidentifier)) As SyncGuid, CSIDomainScoreID As ID from CSIDomainScore")
                       .ToList();
        }

        public int ImportData(string path)
        {
            _logger.Information("IMPORTACAO MACs ...");

            UnitOfWork.DbContext.Configuration.AutoDetectChangesEnabled = false;

            int ImportedCSIObjects = 0;
            string lastGuidToImport = null;
            FileImporter imp = new FileImporter();
            List<CSI> CSIsToPersist = new List<CSI>();
            List<Guid> excludedObjects = new List<Guid>();
            UsersDB = ConvertListToHashtableUsers(findAllUsersUniqueEntities());

            // Garantir que todos objectos importantes para 
            // o CSI com Zeros no Guid não sejam importados
            excludedObjects.Add(new Guid(ZEROSGUID));

            try
            {
                string fullPathCSIs = path + @"\CSIs.csv";
                List<UniqueEntity> BeneficiariesDB = BeneficiaryService.FindAllBeneficiaryUniqueEntities();
                Hashtable AllCSIUniqueEntities = ConvertListToHashtable(FindAllCSIUniqueEntities());
                IEnumerable<DataRow> rows = imp.ImportFromCSV(fullPathCSIs).Rows.Cast<DataRow>();

                Parallel.ForEach(rows, new ParallelOptions { MaxDegreeOfParallelism = MAXTHREADS }, row =>
                {
                    Guid csi_guid = new Guid(row["csi_guid"].ToString());
                    lastGuidToImport = csi_guid.ToString();

                    CSI csi = AllCSIUniqueEntities[csi_guid] == null ? new CSI() : findCSIBySyncGuid(csi_guid);
                    csi.Height = decimal.Parse(("".Equals(row["Height"].ToString())) ? "-32000.00" : row["Height"].ToString());
                    csi.Weight = decimal.Parse(("".Equals(row["Weight"].ToString())) ? "-32000.00" : row["Weight"].ToString());
                    csi.BMI = decimal.Parse(("".Equals(row["BMI"].ToString())) ? "-32000.00" : row["BMI"].ToString());
                    Guid BeneficiaryGuid = new Guid(row["beneficiary_guid"].ToString());
                    UniqueEntity BeneficiaryUE = FindBySyncGuid(BeneficiariesDB, BeneficiaryGuid);

                    if (BeneficiaryUE == null)
                    {
                        _logger.Error("CSI '{0}' nao importado, porque o Beneficiario '{0}' nao existe na BD", csi_guid, BeneficiaryGuid);
                        excludedObjects.Add(csi_guid);
                    }
                    else
                    {
                        csi.BeneficiaryID = BeneficiaryUE.ID;
                        csi.IndexDate = (row["IndexDate"].ToString()).Length == 0 ? csi.IndexDate : DateTime.Parse(row["IndexDate"].ToString());
                        csi.StatusID = int.Parse(row["StatusID"].ToString());
                        SetCreationDataFields(csi, row, csi_guid);
                        SetUpdatedDataFields(csi, row);
                        CSIsToPersist.Add(csi);
                        ImportedCSIObjects++;
                    }

                    if (ImportedCSIObjects % 100 == 0)
                    { _logger.Information(ImportedCSIObjects + " de " + (rows.Count()) + " Objectos de MACs (CSI) importados."); }

                });
                CSIsToPersist.ForEach(c => SaveOrUpdateCSI(c));
                UnitOfWork.Commit();
                Rename(fullPathCSIs, fullPathCSIs + IMPORTED);


                ImportedCSIObjects = 0;
                string fullPathCSIDs = path + @"\CSIDomains.csv";
                AllCSIUniqueEntities = ConvertListToHashtable(FindAllCSIUniqueEntities());
                Hashtable AllCSIDomainUniqueEntities = ConvertListToHashtable(FindAllCSIDomainUniqueEntities());
                List<VPPS.CSI.Domain.DomainEntity> DomainsDB = UnitOfWork.Repository<VPPS.CSI.Domain.DomainEntity>().GetAll().ToList();
                List<CSIDomain> CSIDomainToPersist = new List<CSIDomain>();
                rows = imp.ImportFromCSV(fullPathCSIDs).Rows.Cast<DataRow>();

                Parallel.ForEach(rows, new ParallelOptions { MaxDegreeOfParallelism = MAXTHREADS }, row => 
                {
                    Guid csi_guid = new Guid(row["csi_guid"].ToString());
                    Guid csidomain_guid = new Guid(row["csidomain_guid"].ToString());
                    lastGuidToImport = csidomain_guid.ToString();
                    string CSIDomainCode = row["DomainCode"].ToString();

                    if (excludedObjects.Contains(csi_guid))
                    {
                        _logger.Information("CSIDomain '{0}' nao importado porque o CSI '{1}' nao existe na BD.", csidomain_guid, csi_guid);
                        excludedObjects.Add(csidomain_guid);
                    }
                    else
                    {
                        if (AllCSIDomainUniqueEntities[csidomain_guid] == null)
                        {
                            CSIDomain csiDomain = new CSIDomain();
                            csiDomain.CSIID = ParseStringToIntSafe(AllCSIUniqueEntities[csi_guid]);
                            csiDomain.Domain = DomainsDB.Where(x => x.DomainCode == CSIDomainCode).SingleOrDefault();
                            SetCreationDataFields(csiDomain, row, csidomain_guid);
                            SetUpdatedDataFields(csiDomain, row);
                            CSIDomainToPersist.Add(csiDomain);
                            ImportedCSIObjects++;
                        }
                    }

                    if (ImportedCSIObjects != 0 && ImportedCSIObjects % 100 == 0)
                    { _logger.Information(ImportedCSIObjects + " de " + rows.Count() + " Objectos de MACs (CSIDomain) importados."); }
                });
                CSIDomainToPersist.ForEach(cd => SaveOrUpdateCSIDomain(cd));
                UnitOfWork.Commit();
                Rename(fullPathCSIDs, fullPathCSIDs + IMPORTED);


                ImportedCSIObjects = 0;
                string fullPathCSIDSs = path + @"\CSIDomainScores.csv";
                List<Answer> AnswersDB = UnitOfWork.Repository<Answer>().GetAll().ToList();
                List<Question> QuestionsDB = UnitOfWork.Repository<Question>().GetAll().ToList();
                Hashtable AllCSIDomainUniqueEntitiesHT = ConvertListToHashtable(FindAllCSIDomainUniqueEntities());
                Hashtable AllCSIDomainScoreUniqueEntities = ConvertListToHashtable(FindAllCSIDomainScoreUniqueEntities());
                List<CSIDomainScore> CSIDomainScoreToPersist = new List<CSIDomainScore>();
                rows = imp.ImportFromCSV(fullPathCSIDSs).Rows.Cast<DataRow>();

                Parallel.ForEach(rows, new ParallelOptions { MaxDegreeOfParallelism = MAXTHREADS }, row =>
                {
                    Guid csidomainscore_guid = new Guid(row["csidomainscore_guid"].ToString());
                    Guid csidomain_guid = new Guid(row["csidomain_guid"].ToString());
                    string DomainCode = row["DomainCode"].ToString();
                    int QuestionID = int.Parse(row["QuestionID"].ToString().Length == 0 ? "0" : row["QuestionID"].ToString());
                    int AnswerID = int.Parse(row["AnswerID"].ToString().Length == 0 ? "0" : row["AnswerID"].ToString());
                    lastGuidToImport = csidomainscore_guid.ToString();

                    if (excludedObjects.Contains(csidomain_guid))
                    {
                        _logger.Information("CSIDomainScore '{0}' nao importado porque o CSIDomain '{1}' nao existe na BD.", csidomainscore_guid, csidomain_guid);
                        excludedObjects.Add(csidomainscore_guid);
                    }
                    else
                    {
                        CSIDomainScore csiDomainScore = AllCSIDomainScoreUniqueEntities[csidomainscore_guid] == null ? new CSIDomainScore() : findCSIDomainScoreBySyncGuid(csidomainscore_guid);
                        csiDomainScore.CSIDomainID = ParseStringToIntSafe(AllCSIDomainUniqueEntitiesHT[csidomain_guid]);
                        csiDomainScore.Question = QuestionsDB.Where(x => x.QuestionID == QuestionID).SingleOrDefault();
                        csiDomainScore.Answer = AnswersDB.Where(x => x.AnswerID == AnswerID).SingleOrDefault();
                        SetCreationDataFields(csiDomainScore, row, csidomainscore_guid);
                        SetUpdatedDataFields(csiDomainScore, row);
                        CSIDomainScoreToPersist.Add(csiDomainScore);
                        ImportedCSIObjects++;
                    }

                    if (ImportedCSIObjects != 0 && ImportedCSIObjects % 500 == 0)
                    { _logger.Information(ImportedCSIObjects + " de " + rows.Count() + " Objectos de MACs (CSIDomainScore) importados."); }
                });
                CSIDomainScoreToPersist.ForEach(cds => SaveOrUpdateCSIDomainScore(cds));
                UnitOfWork.Commit();
                Rename(fullPathCSIDSs, fullPathCSIDSs + IMPORTED);
            }
            catch (Exception e)
            {
                _logger.Information("Erro ao importar o Guid : " + lastGuidToImport);
                _logger.Error(e, "Erro ao importar Fichas de Visita", null);
                throw e;
            }
            finally
            {
                UnitOfWork.Dispose();
            }

            return CSIsToPersist.Count();
        }
    }
}
