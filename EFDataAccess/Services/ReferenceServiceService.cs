using EFDataAccess.UOW;
using System;
using System.Collections.Generic;
using System.Linq;
using VPPS.CSI.Domain;
using System.Data;
using EFDataAccess.Logging;
using System.Threading.Tasks;
using System.Collections;
using System.Data.Entity;
using EFDataAccess.DTO;
using System.Data.SqlClient;

namespace EFDataAccess.Services
{
    public class ReferenceServiceService : BaseService
    {
        private UserService UserService;
        private PartnerService PartnerService;
        private RoutineVisitService RoutineVisitService;
        private Hashtable ReferenceServiceAll = null;
        private Hashtable ReferenceAll = null;

        public ReferenceServiceService(UnitOfWork uow) : base(uow)
        {
            UserService = new UserService(uow);
            PartnerService = new PartnerService(uow);
            RoutineVisitService = new RoutineVisitService(uow);
        }

        private EFDataAccess.Repository.IRepository<ReferenceService> ReferenceServiceRepository
        {
            get { return UnitOfWork.Repository<ReferenceService>(); }
        }

        public ReferenceService Reload(ReferenceService entity)
        {
            ReferenceServiceRepository.FullReload(entity);
            return entity;
        }

        public ReferenceService findReferenceServiceBySyncGuid(Guid ReferenceServiceSyncGuid)
        {
            return ReferenceServiceRepository.GetAll().Where(e => e.SyncGuid == ReferenceServiceSyncGuid).FirstOrDefault();
        }

        public Reference findReferenceBySyncGuid(Guid ReferenceSyncGuid)
        {
            return UnitOfWork.Repository<Reference>().Get().Where(e => e.SyncGuid == ReferenceSyncGuid).FirstOrDefault();
        }

        public RoutineVisitMember findRoutineVisitMemberByGuid(Guid RoutineVisitGuid)
        {
            return UnitOfWork.Repository<RoutineVisitMember>().Get().Where(x => x.SyncGuid == RoutineVisitGuid).FirstOrDefault();
        }

        public ReferenceService fetchReferenceServiceByID(int ReferenceServiceID)
        {
            ReferenceService referenceService = UnitOfWork.Repository<ReferenceService>().GetAll()
                .Include(j => j.References.Select(k => k.ReferenceType))
                .Include(j => j.Child)
                .Include(j => j.Adult)
                .Include(j => j.Beneficiary)
                .Where(j => j.ReferenceServiceID == ReferenceServiceID)
                .FirstOrDefault();
            return referenceService;
        }
        
        public List<Reference> fetchReferencesByReferenceServiceByIDNoTracking(int ReferenceServiceID)
        {
            return UnitOfWork.Repository<Reference>().GetAll()
                .Include(j => j.ReferenceType)
                .Where(j => j.ReferenceServiceID == ReferenceServiceID)
                .AsNoTracking().ToList();
        }

        public ReferenceService fetchReferenceServiceByBeneficiaryID(int id)
        {
            ReferenceService referenceService = UnitOfWork.Repository<ReferenceService>().GetAll()
                .Include(j => j.References.Select(k => k.ReferenceType))
                .Include(j => j.Beneficiary)
                .Where(j => j.BeneficiaryID == id)
                .FirstOrDefault();
            referenceService.BeneficiaryType = (referenceService.Beneficiary.AgeInYears > 18) ? "adult" : "child" ;
            return referenceService;
        }

        public ReferenceService fetchReferenceServiceByBeneficiaryIDAndType(int id, string type)
        {
            ReferenceService referenceService = UnitOfWork.Repository<ReferenceService>().GetAll()
                .Include(j => j.References.Select(k => k.ReferenceType))
                .Include(j => j.Child)
                .Include(j => j.Adult)
                .Where(j => j.ChildID == id && type == "child" || j.AdultId == id && type == "adult")
                .FirstOrDefault();
            referenceService.BeneficiaryType = type;
            return referenceService;
        }

        public List<UniqueEntity> FindAllBeneficiaryUniqueEntities() => UnitOfWork.DbContext.Database.SqlQuery<UniqueEntity>("Select ISNULL(SyncGuid, cast(cast(0 as binary) as uniqueidentifier)) As SyncGuid, BeneficiaryID As ID from Beneficiary").ToList();

        public List<UniqueEntity> FindAllReferenceServiceUniqueEntity() => UnitOfWork.DbContext.Database.SqlQuery<UniqueEntity>("Select ISNULL(SyncGuid, cast(cast(0 as binary) as uniqueidentifier)) As SyncGuid, ReferenceServiceID As ID from ReferenceService").ToList();
        
        public List<UniqueEntity> FindAllReferenceUniqueEntity() => UnitOfWork.DbContext.Database.SqlQuery<UniqueEntity>("Select ISNULL(SyncGuid, cast(cast(0 as binary) as uniqueidentifier)) As SyncGuid, ReferenceID As ID from Reference").ToList();

        public void Delete(ReferenceService ReferenceService) { ReferenceServiceRepository.Delete(ReferenceService); }

        public void SaveOrUpdateReferenceService(ReferenceService ReferenceService)
        {
            if (ReferenceService.ReferenceServiceID == 0) { UnitOfWork.Repository<ReferenceService>().Add(ReferenceService); }
            else { UnitOfWork.Repository<ReferenceService>().Update(ReferenceService); }
        }

        public void SaveOrUpdateReference(Reference Reference)
        {
            if (Reference.ReferenceID == 0) { UnitOfWork.Repository<Reference>().Add(Reference); }
            else { UnitOfWork.Repository<Reference>().Update(Reference); }
        }

        public ReferenceService CreateReferenceServiceMemberWithoutSave(int beneficiaryID, string type)
        {
            ReferenceService referenceService = new ReferenceService();
            referenceService.ReferenceDate = null;
            referenceService.ReferenceNumber = 0;
            referenceService.HealthWorker = "";
            referenceService.SocialWorker = "";
            referenceService.BeneficiaryType = type;

            if (type.Equals("child"))
            {
                referenceService.ChildID = beneficiaryID;
                referenceService.AdultId = null;
            }
            else if (type.Equals("adult"))
            {
                referenceService.ChildID = null;
                referenceService.AdultId = beneficiaryID;
            }

            Reference reference;
            foreach (ReferenceType rType in UnitOfWork.Repository<ReferenceType>().GetAll().ToList())
            {
                reference = new Reference { ReferenceTypeID = rType.ReferenceTypeID, ReferenceType = rType, Value = "CheckBox".Equals(rType.FieldType) ? "0" : "" };
                referenceService.ReferencesList.Add(reference);
            }

            return referenceService;
        }

        public ReferenceService CreateReferenceServiceMemberWithoutSave(int beneficiaryID)
        {
            ReferenceService referenceService = new ReferenceService();
            referenceService.ReferenceDate = null;
            referenceService.ReferenceNumber = 0;
            referenceService.HealthWorker = "";
            referenceService.SocialWorker = "";
            referenceService.BeneficiaryID = beneficiaryID;

            Reference reference;
            foreach (ReferenceType rType in UnitOfWork.Repository<ReferenceType>().GetAll().OrderBy(c => c.ReferenceCategory).ThenBy(n => n.ReferenceRow).ThenBy(n => n.ReferenceColumn).ToList())
            {
                reference = new Reference { ReferenceTypeID = rType.ReferenceTypeID, ReferenceType = rType, Value = "CheckBox".Equals(rType.FieldType) ? "0" : "" };
                referenceService.ReferencesList.Add(reference);
            }

            return referenceService;
        }

        public void CreateReferenceServiceFull(ReferenceService referenceService)
        {
            UnitOfWork.Repository<ReferenceService>().Add(referenceService);
            referenceService.References.ToList().ForEach(x => UnitOfWork.Repository<Reference>().Add(x));
            Commit();
        }

        public void UpdateReferenceServiceFull(ReferenceService referenceService)
        {
            referenceService.References.ToList().ForEach(x => UnitOfWork.Repository<Reference>().Update(x));
            referenceService.References = null;
            UnitOfWork.Repository<ReferenceService>().Update(referenceService);
            Commit();
        }

        public void DeleteReferenceServiceFull(ReferenceService referenceService)
        {
            referenceService.References.ToList().ForEach(x => UnitOfWork.Repository<Reference>().Delete(x));
            UnitOfWork.Repository<ReferenceService>().Delete(referenceService);
            Commit();
        }

        public int ImportData(string path)
        {
            _logger.Information("IMPORTACAO DE GUIAS DE REFERENCIA ...");

            UnitOfWork.DbContext.Configuration.AutoDetectChangesEnabled = false;

            string lastGuidToImport = null;
            int ImportedReferencesObjects = 0;
            UsersDB = ConvertListToHashtableUsers(findAllUsersUniqueEntities());
            List<UniqueEntity> PartnersDB = PartnerService.findAllPartnerUniqueEntities();
            List<UniqueEntity> BeneficiariesDB = FindAllBeneficiaryUniqueEntities();
            List<ReferenceService> ReferenceServiceToPersist = new List<ReferenceService>();
            List<Guid> excludedObjects = new List<Guid>();
            FileImporter imp = new FileImporter();

            try
            {
                string fullPathRS = path + @"\ReferenceServices.csv";
                Hashtable PartnerAll = ConvertListToHashtable(PartnersDB);
                ReferenceAll = ConvertListToHashtable(FindAllReferenceUniqueEntity());
                ReferenceServiceAll = ConvertListToHashtable(FindAllReferenceServiceUniqueEntity());
                List<UniqueEntity> RoutineVisitMembersDB = RoutineVisitService.FindAllRoutineVisitMemberUniqueEntity();
                IEnumerable<DataRow> rowsRS = imp.ImportFromCSV(fullPathRS).Rows.Cast<DataRow>();

                Parallel.ForEach(rowsRS, new ParallelOptions { MaxDegreeOfParallelism = MAXTHREADS }, row =>
                {
                    Guid BeneficiaryGuid = new Guid(row["BeneficiaryGuid"].ToString());
                    Guid PartnerGuid = new Guid(row["PartnerGuid"].ToString());
                    Guid ReferenceServiceGuid = new Guid(row["ReferenceServiceGuid"].ToString());
                    int partnerID = ParseStringToIntSafe(PartnerAll[PartnerGuid]);
                    lastGuidToImport = ReferenceServiceGuid.ToString();

                    ReferenceService ReferenceService = (ReferenceServiceAll[ReferenceServiceGuid] == null) ? new ReferenceService() : findReferenceServiceBySyncGuid(ReferenceServiceGuid);
                    ReferenceService.ReferenceNumber = int.Parse(row["ReferenceNumber"].ToString().Length == 0 ? "0" : row["ReferenceNumber"].ToString());
                    ReferenceService.ReferencedBy = row["ReferencedBy"].ToString();
                    ReferenceService.HealthWorker = row["HealthWorker"].ToString();
                    ReferenceService.HealthUnitName = row["HealthUnitName"].ToString();
                    ReferenceService.SocialWorker = row["SocialWorker"].ToString();
                    ReferenceService.SelectedReference = row["SelectedReference"].ToString();
                    if (partnerID > 0) ReferenceService.PartnerID = partnerID;
                    ReferenceService.SocialAttendedDate = (row["SocialAttendedDate"].ToString()).Length == 0 ? ReferenceService.SocialAttendedDate : DateTime.Parse(row["SocialAttendedDate"].ToString());
                    ReferenceService.ReferenceDate = (row["ReferenceDate"].ToString()).Length == 0 ? ReferenceService.ReferenceDate : DateTime.Parse(row["ReferenceDate"].ToString());
                    ReferenceService.HealthAttendedDate = (row["HealthAttendedDate"].ToString()).Length == 0 ? ReferenceService.HealthAttendedDate : DateTime.Parse(row["HealthAttendedDate"].ToString());
                    SetCreationDataFields(ReferenceService, row, ReferenceServiceGuid);
                    SetUpdatedDataFields(ReferenceService, row);

                    UniqueEntity BeneficiaryUE = isZerosGuid(BeneficiaryGuid.ToString()) ? null : FindBySyncGuid(BeneficiariesDB, BeneficiaryGuid);

                    if(BeneficiaryUE != null && !isZerosGuid(BeneficiaryGuid.ToString())) 
                    { 
                        ReferenceService.BeneficiaryID = BeneficiaryUE.ID;
                        ReferenceServiceToPersist.Add(ReferenceService);
                        ImportedReferencesObjects++;
                    }
                    else
                    {
                        _logger.Error("Não existe nem Beneficiario com o Guid '{0}'. A referencia com Guid '{1}' nao sera importada.", BeneficiaryGuid, ReferenceServiceGuid);
                        excludedObjects.Add(ReferenceServiceGuid);
                    }

                    if (ImportedReferencesObjects % 100 == 0)
                    { _logger.Information(ImportedReferencesObjects + " de " + rowsRS.Count() + " Objectos de Referencias (ReferenceService) importados."); }
                });
                ReferenceServiceToPersist.ForEach(x => SaveOrUpdateReferenceService(x));
                UnitOfWork.Commit();
                Rename(fullPathRS, fullPathRS + IMPORTED);


                ImportedReferencesObjects = 0;
                string fullPathR = path + @"\References.csv";
                ReferenceServiceAll = ConvertListToHashtable(FindAllReferenceServiceUniqueEntity());
                ReferenceAll = ConvertListToHashtable(FindAllReferenceUniqueEntity());
                List<Reference> ReferenceList = new List<Reference>();
                IEnumerable<DataRow> rowsR = imp.ImportFromCSV(fullPathR).Rows.Cast<DataRow>();


                Parallel.ForEach(rowsR, new ParallelOptions { MaxDegreeOfParallelism = MAXTHREADS }, row =>
                {
                    Guid ReferenceGuid = new Guid(row["Reference_guid"].ToString());
                    Guid ReferenceServiceGuid = new Guid(row["ReferenceServiceGuid"].ToString());

                    if (excludedObjects.Contains(ReferenceServiceGuid))
                    {
                        _logger.Information("Objecto Reference '{0}' nao importado porque o Objecto ReferenceService '{1}' foi excluido da importacao.", ReferenceGuid, ReferenceServiceGuid);
                    }
                    else
                    {
                        lastGuidToImport = ReferenceGuid.ToString();
                        Reference Reference = (ReferenceAll[ReferenceGuid] == null) ? new Reference() : findReferenceBySyncGuid(ReferenceGuid);
                        Reference.ReferenceServiceID = ParseStringToIntSafe(ReferenceServiceAll[ReferenceServiceGuid]);
                        Reference.ReferenceTypeID = int.Parse(row["ReferenceTypeID"].ToString());
                        Reference.Value = row["Value"].ToString();
                        SetCreationDataFields(Reference, row, ReferenceGuid);
                        SetUpdatedDataFields(Reference, row);

                        ReferenceList.Add(Reference);
                        ImportedReferencesObjects++;
                    }

                    if (ImportedReferencesObjects % 500 == 0)
                    { _logger.Information(ImportedReferencesObjects + " de " + rowsR.Count() + " Objectos de Referencias (Reference) importados."); }
                });
                ReferenceList.ForEach(x => SaveOrUpdateReference(x));
                UnitOfWork.Commit();
                Rename(fullPathR, fullPathR + IMPORTED);
            }
            catch (Exception e)
            {
                _logger.Information("Erro ao importar o Guid : " + lastGuidToImport);
                _logger.Error(e, "Erro ao importar Guias de Referência", null);
                throw e;
            }
            finally
            {
                UnitOfWork.Dispose();
            }

            return ReferenceServiceToPersist.Count();
        }


        /*
         * #######################################################
         * ###### Activist References ChiefPartner for Child #####
         * #######################################################
         */

        public List<ActivistReferencesSummaryByRefTypeReportDTO> getActivistReferencesForChildReportSummaryChiefPartner(DateTime initialDate, DateTime lastDate)
        {
            return getActivistReferencesForChildReportSummary(initialDate, lastDate, "chiefpartner");
        }

        /*
         * #######################################################
         * ######### Activist References Partner  for Child ######
         * #######################################################
         */

        public List<ActivistReferencesSummaryByRefTypeReportDTO> getActivistReferencesForChildReportSummaryPartner(DateTime initialDate, DateTime lastDate)
        {
            return getActivistReferencesForChildReportSummary(initialDate, lastDate, "partner");
        }


        /*
         * ##############################################################
         * ####### Activist References for Child Report Summary #########
         * ##############################################################
         */

        public List<ActivistReferencesSummaryByRefTypeReportDTO> getActivistReferencesForChildReportSummary(DateTime initialDate, DateTime lastDate, string partnerType)
        {

            String query = @"SELECT  
                                p.ChiefPartner--<<ReplaceColumn<<--
                                As Partner,
                                ISNULL(partner_references_obj.ATS_Male,0) As ATS_Male,
	                            ISNULL(partner_references_obj.ATS_Female,0) As ATS_Female,
	                            ISNULL(partner_references_obj.TARV_Male,0) As TARV_Male,
	                            ISNULL(partner_references_obj.TARV_Female,0) As TARV_Female,
	                            ISNULL(partner_references_obj.CCR_Male,0) As CCR_Male,
	                            ISNULL(partner_references_obj.CCR_Female,0) As CCR_Female,
	                            ISNULL(partner_references_obj.SSR_Male,0) As SSR_Male,
	                            ISNULL(partner_references_obj.SSR_Female,0) As SSR_Female,
	                            ISNULL(partner_references_obj.VGB_Male,0) As VGB_Male,
	                            ISNULL(partner_references_obj.VGB_Female,0) As VGB_Female,
	                            ISNULL(partner_references_obj.Poverty_Proof_Male,0) As Poverty_Proof_Male,
	                            ISNULL(partner_references_obj.Poverty_Proof_Female,0) As Poverty_Proof_Female,
	                            ISNULL(partner_references_obj.Birth_Registration_Male,0) As Birth_Registration_Male,
	                            ISNULL(partner_references_obj.Birth_Registration_Female,0) As Birth_Registration_Female,
	                            ISNULL(partner_references_obj.Identification_Card_Male,0) As Identification_Card_Male,
	                            ISNULL(partner_references_obj.Identification_Card_Female,0) As Identification_Card_Female,
	                            ISNULL(partner_references_obj.School_Integration_Male,0) As School_Integration_Male,
	                            ISNULL(partner_references_obj.School_Integration_Female,0) As School_Integration_Female,
	                            ISNULL(partner_references_obj.Vocational_Courses_Male,0) As Vocational_Courses_Male,
	                            ISNULL(partner_references_obj.Vocational_Courses_Female,0) As Vocational_Courses_Female,
	                            ISNULL(partner_references_obj.School_Material_Male,0) As School_Material_Male,
	                            ISNULL(partner_references_obj.School_Material_Female,0) As School_Material_Female,
	                            ISNULL(partner_references_obj.Basic_Basket_Male,0) As Basic_Basket_Male,
	                            ISNULL(partner_references_obj.Basic_Basket_Female,0) As Basic_Basket_Female,
	                            ISNULL(partner_references_obj.INAS_Benefit_Male,0) As INAS_Benefit_Male,
	                            ISNULL(partner_references_obj.INAS_Benefit_Female,0) As INAS_Benefit_Female,
	                            ISNULL(partner_references_obj.SAAJ_Male,0) As SAAJ_Male,
	                            ISNULL(partner_references_obj.SAAJ_Female,0) As SAAJ_Female,
	                            ISNULL(partner_references_obj.Desnutrition_Male,0) As Desnutrition_Male,
	                            ISNULL(partner_references_obj.Desnutrition_Female,0) As Desnutrition_Female,
	                            ISNULL(partner_references_obj.Delay_in_Development_Male,0) As Delay_in_Development_Male,
	                            ISNULL(partner_references_obj.Delay_in_Development_Female,0) As Delay_in_Development_Female,
	                            ISNULL(partner_references_obj.Others_Male,0) As Others_Male,
	                            ISNULL(partner_references_obj.Others_Female,0) As Others_Female
                                FROM
                            (
	                            SELECT
	                            ChiefPartner--<<ReplaceColumn<<--
	                            FROM
	                            (
		                            SELECT cp.Name As ChiefPartner, p.Name As Partner
		                            FROM  [Partner] p
		                            inner join  [Partner] cp on (p.SuperiorId = cp.PartnerID)
		                            Where p.CollaboratorRoleID = 1
	                            ) p
	                            group by p.ChiefPartner--<<ReplaceColumn<<--
                            ) p
                            left join
                            (	
	                            SELECT  --  Group By Child
	                            ref_obj.ChiefPartner--<<ReplaceColumn<<--
	                            ,
	                            SUM(ref_obj.ATS_Male) As ATS_Male,
	                            SUM(ref_obj.ATS_Female) As ATS_Female,
	                            SUM(ref_obj.TARV_Male) As TARV_Male,
	                            SUM(ref_obj.TARV_Female) As TARV_Female,
	                            SUM(ref_obj.CCR_Male) As CCR_Male,
	                            SUM(ref_obj.CCR_Female) As CCR_Female,
	                            SUM(ref_obj.SSR_Male) As SSR_Male,
	                            SUM(ref_obj.SSR_Female) As SSR_Female,
	                            SUM(ref_obj.VGB_Male) As VGB_Male,
	                            SUM(ref_obj.VGB_Female) As VGB_Female,
	                            SUM(ref_obj.Poverty_Proof_Male) As Poverty_Proof_Male,
	                            SUM(ref_obj.Poverty_Proof_Female) As Poverty_Proof_Female,
	                            SUM(ref_obj.Birth_Registration_Male) As Birth_Registration_Male,
	                            SUM(ref_obj.Birth_Registration_Female) As Birth_Registration_Female,
	                            SUM(ref_obj.Identification_Card_Male) As Identification_Card_Male,
	                            SUM(ref_obj.Identification_Card_Female) As Identification_Card_Female,
	                            SUM(ref_obj.School_Integration_Male) As School_Integration_Male,
	                            SUM(ref_obj.School_Integration_Female) As School_Integration_Female,
	                            SUM(ref_obj.Vocational_Courses_Male) As Vocational_Courses_Male,
	                            SUM(ref_obj.Vocational_Courses_Female) As Vocational_Courses_Female,
	                            SUM(ref_obj.School_Material_Male) As School_Material_Male,
	                            SUM(ref_obj.School_Material_Female) As School_Material_Female,
	                            SUM(ref_obj.Basic_Basket_Male) As Basic_Basket_Male,
	                            SUM(ref_obj.Basic_Basket_Female) As Basic_Basket_Female,
	                            SUM(ref_obj.INAS_Benefit_Male) As INAS_Benefit_Male,
	                            SUM(ref_obj.INAS_Benefit_Female) As INAS_Benefit_Female,
	                            SUM(ref_obj.SAAJ_Male) As SAAJ_Male,
	                            SUM(ref_obj.SAAJ_Female) As SAAJ_Female,
	                            SUM(ref_obj.Desnutrition_Male) As Desnutrition_Male,
	                            SUM(ref_obj.Desnutrition_Female) As Desnutrition_Female,
	                            SUM(ref_obj.Delay_in_Development_Male) As Delay_in_Development_Male,
	                            SUM(ref_obj.Delay_in_Development_Female) As Delay_in_Development_Female,
	                            SUM(ref_obj.Others_Male) As Others_Male,
	                            SUM(ref_obj.Others_Female) As Others_Female,
	                            SUM(ref_obj.Total_Male) As Total_Male,
	                            SUM(ref_obj.Total_Female) As Total_Female
	                            FROM
	                            (
		                            SELECT
		                            cp.Name As ChiefPartner,
                                    p.Name AS [Partner],
		                            c.FirstName,
		                            c.LastName,
		                            ATS_Male = CASE WHEN c.Gender = 'M' AND rt.ReferenceName = 'ATS' THEN 1 ELSE 0 END,
		                            ATS_Female = CASE WHEN c.Gender = 'F' AND rt.ReferenceName = 'ATS' THEN 1 ELSE 0 END,
		                            TARV_Male = CASE WHEN c.Gender = 'M' AND rt.ReferenceName in ('PTV','Testado HIV+','Pré-TARV/IO','Abandono TARV','PPE') THEN 1 ELSE 0 END,
		                            TARV_Female = CASE WHEN c.Gender = 'F' AND rt.ReferenceName  in ('PTV','Testado HIV+','Pré-TARV/IO','Abandono TARV','PPE') THEN 1 ELSE 0 END,
		                            CCR_Male = CASE WHEN c.Gender = 'M' AND rt.ReferenceName = 'CCR' THEN 1 ELSE 0 END,
		                            CCR_Female = CASE WHEN c.Gender = 'F' AND rt.ReferenceName = 'CCR' THEN 1 ELSE 0 END,
		                            SSR_Male = CASE WHEN c.Gender = 'M' AND rt.ReferenceName in ('Maternidade p/ Parto','CPN','CPN Familiar','Consulta Pós-Parto','ITS','Circuncisao Masculina') THEN 1 ELSE 0 END,
		                            SSR_Female = CASE WHEN c.Gender = 'F' AND rt.ReferenceName in ('Maternidade p/ Parto','CPN','CPN Familiar','Consulta Pós-Parto','ITS','Circuncisao Masculina') THEN 1 ELSE 0 END,
		                            VGB_Male = CASE WHEN c.Gender = 'M' AND rt.ReferenceName in ('GAVV','Apoio Psico-Social','Posto Policial') THEN 1 ELSE 0 END,
		                            VGB_Female = CASE WHEN c.Gender = 'F' AND rt.ReferenceName in ('GAVV','Apoio Psico-Social','Posto Policial') THEN 1 ELSE 0 END,
		                            Poverty_Proof_Male = CASE WHEN c.Gender = 'M' AND rt.ReferenceName = 'Atestado de Pobreza' THEN 1 ELSE 0 END,
		                            Poverty_Proof_Female = CASE WHEN c.Gender = 'F' AND rt.ReferenceName = 'Atestado de Pobreza' THEN 1 ELSE 0 END,
		                            Birth_Registration_Male = CASE WHEN c.Gender = 'M' AND rt.ReferenceName = 'Registo de Nascimento/Cédula' THEN 1 ELSE 0 END,
		                            Birth_Registration_Female = CASE WHEN c.Gender = 'F' AND rt.ReferenceName = 'Registo de Nascimento/Cédula' THEN 1 ELSE 0 END,
		                            Identification_Card_Male = CASE WHEN c.Gender = 'M' AND rt.ReferenceName = 'Bilhete de Identidade (B.I)' THEN 1 ELSE 0 END,
		                            Identification_Card_Female = CASE WHEN c.Gender = 'F' AND rt.ReferenceName = 'Bilhete de Identidade (B.I)' THEN 1 ELSE 0 END,
		                            School_Integration_Male = CASE WHEN c.Gender = 'M' AND rt.ReferenceName = 'Integração Escolar' THEN 1 ELSE 0 END,
		                            School_Integration_Female = CASE WHEN c.Gender = 'F' AND rt.ReferenceName = 'Integração Escolar' THEN 1 ELSE 0 END,
		                            Vocational_Courses_Male = CASE WHEN c.Gender = 'M' AND rt.ReferenceName = 'Curso de Formação Vocacional' THEN 1 ELSE 0 END,
		                            Vocational_Courses_Female = CASE WHEN c.Gender = 'F' AND rt.ReferenceName = 'Curso de Formação Vocacional' THEN 1 ELSE 0 END,
		                            School_Material_Male = CASE WHEN c.Gender = 'M' AND rt.ReferenceName = 'Material Escolar' THEN 1 ELSE 0 END,
		                            School_Material_Female = CASE WHEN c.Gender = 'F' AND rt.ReferenceName = 'Material Escolar' THEN 1 ELSE 0 END,
		                            Basic_Basket_Male = CASE WHEN c.Gender = 'M' AND rt.ReferenceName = 'Cesta Básica' THEN 1 ELSE 0 END,
		                            Basic_Basket_Female = CASE WHEN c.Gender = 'F' AND rt.ReferenceName = 'Cesta Básica' THEN 1 ELSE 0 END,
		                            INAS_Benefit_Male = CASE WHEN c.Gender = 'M' AND rt.ReferenceName = 'Subsídios Sociais do INAS' THEN 1 ELSE 0 END,
		                            INAS_Benefit_Female = CASE WHEN c.Gender = 'F' AND rt.ReferenceName = 'Subsídios Sociais do INAS' THEN 1 ELSE 0 END,
		                            SAAJ_Male = CASE WHEN c.Gender = 'M' AND rt.ReferenceName = 'SAAJ' THEN 1 ELSE 0 END,
		                            SAAJ_Female = CASE WHEN c.Gender = 'F' AND rt.ReferenceName = 'SAAJ' THEN 1 ELSE 0 END,
		                            Desnutrition_Male = CASE WHEN c.Gender = 'M' AND rt.ReferenceName = 'Desnutrição' THEN 1 ELSE 0 END,
		                            Desnutrition_Female = CASE WHEN c.Gender = 'F' AND rt.ReferenceName = 'Desnutrição' THEN 1 ELSE 0 END,
		                            Delay_in_Development_Male = CASE WHEN c.Gender = 'M' AND rt.ReferenceName = 'Atraso no Desenvolvimento' THEN 1 ELSE 0 END,
		                            Delay_in_Development_Female = CASE WHEN c.Gender = 'F' AND rt.ReferenceName = 'Atraso no Desenvolvimento' THEN 1 ELSE 0 END,
                                    Others_Male = CASE WHEN c.Gender = 'M' AND rt.ReferenceName not in ('ATS','PTV','Testado HIV+','Pré-TARV/IO','Abandono TARV','PPE','CCR','Maternidade p/ Parto','CPN'
                                    ,'CPN Familiar','Consulta Pós-Parto','ITS','Circuncisao Masculina','GAVV','Apoio Psico-Social','Posto Policial'
                                    ,'Atestado de Pobreza','Registo de Nascimento/Cédula','Bilhete de Identidade (B.I)','Integração Escolar'
                                    ,'Curso de Formação Vocacional','Material Escolar','Cesta Básica','Subsídios Sociais do INAS'
		                            ,'SAAJ','Desnutrição', 'Atraso no Desenvolvimento') THEN 1 ELSE 0 END,
                                    Others_Female = CASE WHEN c.Gender = 'F' AND rt.ReferenceName not in ('ATS','PTV','Testado HIV+','Pré-TARV/IO','Abandono TARV','PPE','CCR','Maternidade p/ Parto','CPN'
                                    ,'CPN Familiar','Consulta Pós-Parto','ITS','Circuncisao Masculina','GAVV','Apoio Psico-Social','Posto Policial'
                                    ,'Atestado de Pobreza','Registo de Nascimento/Cédula','Bilhete de Identidade (B.I)','Integração Escolar'
                                    ,'Curso de Formação Vocacional','Material Escolar','Cesta Básica','Subsídios Sociais do INAS'
		                            ,'SAAJ','Desnutrição', 'Atraso no Desenvolvimento') THEN 1 ELSE 0 END,
		                            Total_Male = CASE WHEN c.Gender = 'M' THEN 1 ELSE 0 END,
		                            Total_Female = CASE WHEN c.Gender = 'F' THEN 1 ELSE 0 END
		                            FROM  [Partner] p
		                            inner join  [Partner] cp on (p.SuperiorId = cp.PartnerID) 
		                            inner join  [HouseHold] hh on (p.PartnerID = hh.PartnerID)
		                            inner join  [Beneficiary] c on (c.HouseHoldID = hh.HouseHoldID)
                                    inner join  [CSI_PROD].[dbo].[vw_beneficiary_details] ben ON ben.ID = c.BeneficiaryID AND type='child'
		                            inner join  [ReferenceService] rs on (c.BeneficiaryID = rs.BeneficiaryID)
		                            inner join  [Reference] r on (r.ReferenceServiceID = rs.ReferenceServiceID)
		                            inner join  [ReferenceType] rt on (rt.ReferenceTypeID = r.ReferenceTypeID)
                                    WHERE p.CollaboratorRoleID = 1 AND c.BeneficiaryID IS NOT NULL
                                    AND rs.ReferenceDate between @initialDate AND @lastDate
		                            AND rt.ReferenceCategory = 'Activist' AND r.Value <> '0' AND r.Value <> '' AND r.Value IS NOT NULL
		                            AND NOT LOWER(r.Value) LIKE LOWER('%test%')--Testagem, teste, ATS
		                            AND NOT LOWER(r.Value) LIKE LOWER('%ats%')--ATS
		                            AND NOT LOWER(r.Value) LIKE LOWER('%its%') --ITS
		                            AND NOT LOWER(r.Value) LIKE LOWER('%tarv%')--TARV
		                            AND NOT LOWER(r.Value) LIKE LOWER('%profila%')--PPE
		                            AND NOT LOWER(r.Value) LIKE LOWER('%ppe%')--PPE
		                            AND NOT LOWER(r.Value) LIKE LOWER('%natal%')--Testagem, teste
		                            AND NOT LOWER(r.Value) LIKE LOWER('%ccr%') --CCR
		                            AND NOT LOWER(r.Value) LIKE LOWER('%risco%') --CCR
		                            AND NOT LOWER(r.Value) LIKE LOWER('%viol%')--GAAV
		                            AND NOT LOWER(r.Value) LIKE LOWER('%gaav%')--GAAV
		                            AND NOT LOWER(r.Value) LIKE LOWER('%parto%')--CPN Maternidade p/ Parto,Consulta Pós-Parto
		                            AND NOT LOWER(r.Value) LIKE LOWER('%cpn%')--CPN
		                            AND NOT LOWER(r.Value) LIKE LOWER('%planeamento familiar%')--CPF
		                            AND NOT LOWER(r.Value) LIKE LOWER('%oportuni%')--IO
		                            AND NOT LOWER(r.Value) LIKE LOWER('%domic%')--CD
		                            AND NOT LOWER(r.Value) LIKE LOWER('%sadia%')--CCS
		                            AND NOT LOWER(r.Value) LIKE LOWER('%ptv%')--PTV
		                            AND NOT LOWER(r.Value) LIKE LOWER('%vertical%')--PTV
		                            AND NOT LOWER(r.Value) LIKE LOWER('%bk%')--BK
		                            AND NOT LOWER(r.Value) LIKE LOWER('%mal%')--malária
		                            AND NOT LOWER(r.Value) LIKE LOWER('%circun%')--Circuncisão
		                            AND NOT LOWER(r.Value) LIKE LOWER('%pscico%')--Apoio Psico-Social
		                            AND NOT LOWER(r.Value) LIKE LOWER('%social%')--Acção Social,Apoio Psico-Social
		                            AND NOT LOWER(r.Value) LIKE LOWER('%polic%')--policial
	                            ) ref_obj
	                            group by ref_obj.ChiefPartner--<<ReplaceColumn<<--
                            ) partner_references_obj 
                            on 
                            (
	                            p.ChiefPartner--<<ReplaceColumn<<--
	                            =
	                            partner_references_obj.ChiefPartner--<<ReplaceColumn<<--
                            )";


            query = (partnerType.Equals("partner")) ? query.Replace("ChiefPartner--<<ReplaceColumn<<--", "Partner") : query;


            return UnitOfWork.DbContext.Database.SqlQuery<ActivistReferencesSummaryByRefTypeReportDTO>(query,
                                                            new SqlParameter("initialDate", initialDate),
                                                            new SqlParameter("lastDate", lastDate)).ToList();
        }


        /*
         * #######################################################
         * ###### Activist References ChiefPartner for Adult #####
         * #######################################################
         */

        public List<ActivistReferencesSummaryByRefTypeReportDTO> getActivistReferencesForAdultReportSummaryChiefPartner(DateTime initialDate, DateTime lastDate)
        {
            return getActivistReferencesForAdultReportSummary(initialDate, lastDate, "chiefpartner");
        }

        /*
         * #######################################################
         * ######### Activist References Partner  for Adult ######
         * #######################################################
         */

        public List<ActivistReferencesSummaryByRefTypeReportDTO> getActivistReferencesForAdultReportSummaryPartner(DateTime initialDate, DateTime lastDate)
        {
            return getActivistReferencesForAdultReportSummary(initialDate, lastDate, "partner");
        }


        /*
         * ##############################################################
         * ####### Activist References for Adult Report Summary #########
         * ##############################################################
         */

        public List<ActivistReferencesSummaryByRefTypeReportDTO> getActivistReferencesForAdultReportSummary(DateTime initialDate, DateTime lastDate, string partnerType)
        {
           
            String query = @"SELECT  
                                p.ChiefPartner--<<ReplaceColumn<<--
                                As Partner,
                                ISNULL(partner_references_obj.ATS_Male,0) As ATS_Male,
	                            ISNULL(partner_references_obj.ATS_Female,0) As ATS_Female,
	                            ISNULL(partner_references_obj.TARV_Male,0) As TARV_Male,
	                            ISNULL(partner_references_obj.TARV_Female,0) As TARV_Female,
	                            ISNULL(partner_references_obj.CCR_Male,0) As CCR_Male,
	                            ISNULL(partner_references_obj.CCR_Female,0) As CCR_Female,
	                            ISNULL(partner_references_obj.SSR_Male,0) As SSR_Male,
	                            ISNULL(partner_references_obj.SSR_Female,0) As SSR_Female,
	                            ISNULL(partner_references_obj.VGB_Male,0) As VGB_Male,
	                            ISNULL(partner_references_obj.VGB_Female,0) As VGB_Female,
	                            ISNULL(partner_references_obj.Poverty_Proof_Male,0) As Poverty_Proof_Male,
	                            ISNULL(partner_references_obj.Poverty_Proof_Female,0) As Poverty_Proof_Female,
	                            ISNULL(partner_references_obj.Birth_Registration_Male,0) As Birth_Registration_Male,
	                            ISNULL(partner_references_obj.Birth_Registration_Female,0) As Birth_Registration_Female,
	                            ISNULL(partner_references_obj.Identification_Card_Male,0) As Identification_Card_Male,
	                            ISNULL(partner_references_obj.Identification_Card_Female,0) As Identification_Card_Female,
	                            ISNULL(partner_references_obj.School_Integration_Male,0) As School_Integration_Male,
	                            ISNULL(partner_references_obj.School_Integration_Female,0) As School_Integration_Female,
	                            ISNULL(partner_references_obj.Vocational_Courses_Male,0) As Vocational_Courses_Male,
	                            ISNULL(partner_references_obj.Vocational_Courses_Female,0) As Vocational_Courses_Female,
	                            ISNULL(partner_references_obj.School_Material_Male,0) As School_Material_Male,
	                            ISNULL(partner_references_obj.School_Material_Female,0) As School_Material_Female,
	                            ISNULL(partner_references_obj.Basic_Basket_Male,0) As Basic_Basket_Male,
	                            ISNULL(partner_references_obj.Basic_Basket_Female,0) As Basic_Basket_Female,
	                            ISNULL(partner_references_obj.INAS_Benefit_Male,0) As INAS_Benefit_Male,
	                            ISNULL(partner_references_obj.INAS_Benefit_Female,0) As INAS_Benefit_Female,
	                            ISNULL(partner_references_obj.SAAJ_Male,0) As SAAJ_Male,
	                            ISNULL(partner_references_obj.SAAJ_Female,0) As SAAJ_Female,
	                            ISNULL(partner_references_obj.Desnutrition_Male,0) As Desnutrition_Male,
	                            ISNULL(partner_references_obj.Desnutrition_Female,0) As Desnutrition_Female,
	                            ISNULL(partner_references_obj.Delay_in_Development_Male,0) As Delay_in_Development_Male,
	                            ISNULL(partner_references_obj.Delay_in_Development_Female,0) As Delay_in_Development_Female,
	                            ISNULL(partner_references_obj.Others_Male,0) As Others_Male,
	                            ISNULL(partner_references_obj.Others_Female,0) As Others_Female
                            FROM
                            (
	                            SELECT
	                            ChiefPartner--<<ReplaceColumn<<--
	                            FROM
	                            (
		                            SELECT cp.Name As ChiefPartner, p.Name As Partner
		                            FROM  [Partner] p
		                            inner join  [Partner] cp on (p.SuperiorId = cp.PartnerID)
		                            Where p.CollaboratorRoleID = 1
	                            ) p
	                            group by p.ChiefPartner--<<ReplaceColumn<<--
                            ) p
                            left join
                            (	
	                            SELECT  --  Group By Adult
	                            ref_obj.ChiefPartner--<<ReplaceColumn<<--
                                ,
	                            SUM(ref_obj.ATS_Male) As ATS_Male,
	                            SUM(ref_obj.ATS_Female) As ATS_Female,
	                            SUM(ref_obj.TARV_Male) As TARV_Male,
	                            SUM(ref_obj.TARV_Female) As TARV_Female,
	                            SUM(ref_obj.CCR_Male) As CCR_Male,
	                            SUM(ref_obj.CCR_Female) As CCR_Female,
	                            SUM(ref_obj.SSR_Male) As SSR_Male,
	                            SUM(ref_obj.SSR_Female) As SSR_Female,
	                            SUM(ref_obj.VGB_Male) As VGB_Male,
	                            SUM(ref_obj.VGB_Female) As VGB_Female,
	                            SUM(ref_obj.Poverty_Proof_Male) As Poverty_Proof_Male,
	                            SUM(ref_obj.Poverty_Proof_Female) As Poverty_Proof_Female,
	                            SUM(ref_obj.Birth_Registration_Male) As Birth_Registration_Male,
	                            SUM(ref_obj.Birth_Registration_Female) As Birth_Registration_Female,
	                            SUM(ref_obj.Identification_Card_Male) As Identification_Card_Male,
	                            SUM(ref_obj.Identification_Card_Female) As Identification_Card_Female,
	                            SUM(ref_obj.School_Integration_Male) As School_Integration_Male,
	                            SUM(ref_obj.School_Integration_Female) As School_Integration_Female,
	                            SUM(ref_obj.Vocational_Courses_Male) As Vocational_Courses_Male,
	                            SUM(ref_obj.Vocational_Courses_Female) As Vocational_Courses_Female,
	                            SUM(ref_obj.School_Material_Male) As School_Material_Male,
	                            SUM(ref_obj.School_Material_Female) As School_Material_Female,
	                            SUM(ref_obj.Basic_Basket_Male) As Basic_Basket_Male,
	                            SUM(ref_obj.Basic_Basket_Female) As Basic_Basket_Female,
	                            SUM(ref_obj.INAS_Benefit_Male) As INAS_Benefit_Male,
	                            SUM(ref_obj.INAS_Benefit_Female) As INAS_Benefit_Female,
	                            SUM(ref_obj.SAAJ_Male) As SAAJ_Male,
	                            SUM(ref_obj.SAAJ_Female) As SAAJ_Female,
	                            SUM(ref_obj.Desnutrition_Male) As Desnutrition_Male,
	                            SUM(ref_obj.Desnutrition_Female) As Desnutrition_Female,
	                            SUM(ref_obj.Delay_in_Development_Male) As Delay_in_Development_Male,
	                            SUM(ref_obj.Delay_in_Development_Female) As Delay_in_Development_Female,
	                            SUM(ref_obj.Others_Male) As Others_Male,
	                            SUM(ref_obj.Others_Female) As Others_Female,
	                            SUM(ref_obj.Total_Male) As RC_Total_Male,
	                            SUM(ref_obj.Total_Female) As RC_Total_Female
	                            FROM
	                            (
		                            SELECT
		                            cp.Name As ChiefPartner,
                                    p.Name AS [Partner],
		                            a.FirstName,
		                            a.LastName,
		                            ATS_Male = CASE WHEN a.Gender = 'M' AND rt.ReferenceName = 'ATS' THEN 1 ELSE 0 END,
		                            ATS_Female = CASE WHEN a.Gender = 'F' AND rt.ReferenceName = 'ATS' THEN 1 ELSE 0 END,
		                            TARV_Male = CASE WHEN a.Gender = 'M' AND rt.ReferenceName in ('PTV','Testado HIV+','Pré-TARV/IO','Abandono TARV','PPE') THEN 1 ELSE 0 END,
		                            TARV_Female = CASE WHEN a.Gender = 'F' AND rt.ReferenceName  in ('PTV','Testado HIV+','Pré-TARV/IO','Abandono TARV','PPE') THEN 1 ELSE 0 END,
		                            CCR_Male = CASE WHEN a.Gender = 'M' AND rt.ReferenceName = 'CCR' THEN 1 ELSE 0 END,
		                            CCR_Female = CASE WHEN a.Gender = 'F' AND rt.ReferenceName = 'CCR' THEN 1 ELSE 0 END,
		                            SSR_Male = CASE WHEN a.Gender = 'M' AND rt.ReferenceName in ('Maternidade p/ Parto','CPN','CPN Familiar','Consulta Pós-Parto','ITS','Circuncisao Masculina') THEN 1 ELSE 0 END,
		                            SSR_Female = CASE WHEN a.Gender = 'F' AND rt.ReferenceName in ('Maternidade p/ Parto','CPN','CPN Familiar','Consulta Pós-Parto','ITS','Circuncisao Masculina') THEN 1 ELSE 0 END,
		                            VGB_Male = CASE WHEN a.Gender = 'M' AND rt.ReferenceName in ('GAVV','Apoio Psico-Social','Posto Policial') THEN 1 ELSE 0 END,
		                            VGB_Female = CASE WHEN a.Gender = 'F' AND rt.ReferenceName in ('GAVV','Apoio Psico-Social','Posto Policial') THEN 1 ELSE 0 END,
		                            Poverty_Proof_Male = CASE WHEN a.Gender = 'M' AND rt.ReferenceName = 'Atestado de Pobreza' THEN 1 ELSE 0 END,
		                            Poverty_Proof_Female = CASE WHEN a.Gender = 'F' AND rt.ReferenceName = 'Atestado de Pobreza' THEN 1 ELSE 0 END,
		                            Birth_Registration_Male = CASE WHEN a.Gender = 'M' AND rt.ReferenceName = 'Registo de Nascimento/Cédula' THEN 1 ELSE 0 END,
		                            Birth_Registration_Female = CASE WHEN a.Gender = 'F' AND rt.ReferenceName = 'Registo de Nascimento/Cédula' THEN 1 ELSE 0 END,
		                            Identification_Card_Male = CASE WHEN a.Gender = 'M' AND rt.ReferenceName = 'Bilhete de Identidade (B.I)' THEN 1 ELSE 0 END,
		                            Identification_Card_Female = CASE WHEN a.Gender = 'F' AND rt.ReferenceName = 'Bilhete de Identidade (B.I)' THEN 1 ELSE 0 END,
		                            School_Integration_Male = CASE WHEN a.Gender = 'M' AND rt.ReferenceName = 'Integração Escolar' THEN 1 ELSE 0 END,
		                            School_Integration_Female = CASE WHEN a.Gender = 'F' AND rt.ReferenceName = 'Integração Escolar' THEN 1 ELSE 0 END,
		                            Vocational_Courses_Male = CASE WHEN a.Gender = 'M' AND rt.ReferenceName = 'Curso de Formação Vocacional' THEN 1 ELSE 0 END,
		                            Vocational_Courses_Female = CASE WHEN a.Gender = 'F' AND rt.ReferenceName = 'Curso de Formação Vocacional' THEN 1 ELSE 0 END,
		                            School_Material_Male = CASE WHEN a.Gender = 'M' AND rt.ReferenceName = 'Material Escolar' THEN 1 ELSE 0 END,
		                            School_Material_Female = CASE WHEN a.Gender = 'F' AND rt.ReferenceName = 'Material Escolar' THEN 1 ELSE 0 END,
		                            Basic_Basket_Male = CASE WHEN a.Gender = 'M' AND rt.ReferenceName = 'Cesta Básica' THEN 1 ELSE 0 END,
		                            Basic_Basket_Female = CASE WHEN a.Gender = 'F' AND rt.ReferenceName = 'Cesta Básica' THEN 1 ELSE 0 END,
		                            INAS_Benefit_Male = CASE WHEN a.Gender = 'M' AND rt.ReferenceName = 'Subsídios Sociais do INAS' THEN 1 ELSE 0 END,
		                            INAS_Benefit_Female = CASE WHEN a.Gender = 'F' AND rt.ReferenceName = 'Subsídios Sociais do INAS' THEN 1 ELSE 0 END,
		                            SAAJ_Male = CASE WHEN a.Gender = 'M' AND rt.ReferenceName = 'SAAJ' THEN 1 ELSE 0 END,
		                            SAAJ_Female = CASE WHEN a.Gender = 'F' AND rt.ReferenceName = 'SAAJ' THEN 1 ELSE 0 END,
		                            Desnutrition_Male = CASE WHEN a.Gender = 'M' AND rt.ReferenceName = 'Desnutrição' THEN 1 ELSE 0 END,
		                            Desnutrition_Female = CASE WHEN a.Gender = 'F' AND rt.ReferenceName = 'Desnutrição' THEN 1 ELSE 0 END,
		                            Delay_in_Development_Male = CASE WHEN a.Gender = 'M' AND rt.ReferenceName = 'Atraso no Desenvolvimento' THEN 1 ELSE 0 END,
		                            Delay_in_Development_Female = CASE WHEN a.Gender = 'F' AND rt.ReferenceName = 'Atraso no Desenvolvimento' THEN 1 ELSE 0 END,
		                            Others_Male = CASE WHEN a.Gender = 'M' AND rt.ReferenceName not in ('ATS','PTV','Testado HIV+','Pré-TARV/IO','Abandono TARV','PPE','CCR','Maternidade p/ Parto','CPN'
                                    ,'CPN Familiar','Consulta Pós-Parto','ITS','Circuncisao Masculina','GAVV','Apoio Psico-Social','Posto Policial'
                                    ,'Atestado de Pobreza','Registo de Nascimento/Cédula','Bilhete de Identidade (B.I)','Integração Escolar'
                                    ,'Curso de Formação Vocacional','Material Escolar','Cesta Básica','Subsídios Sociais do INAS'
		                            ,'SAAJ','Desnutrição', 'Atraso no Desenvolvimento') THEN 1 ELSE 0 END,
                                    Others_Female = CASE WHEN a.Gender = 'F' AND rt.ReferenceName not in ('ATS','PTV','Testado HIV+','Pré-TARV/IO','Abandono TARV','PPE','CCR','Maternidade p/ Parto','CPN'
                                    ,'CPN Familiar','Consulta Pós-Parto','ITS','Circuncisao Masculina','GAVV','Apoio Psico-Social','Posto Policial'
                                    ,'Atestado de Pobreza','Registo de Nascimento/Cédula','Bilhete de Identidade (B.I)','Integração Escolar'
                                    ,'Curso de Formação Vocacional','Material Escolar','Cesta Básica','Subsídios Sociais do INAS'
		                            ,'SAAJ','Desnutrição', 'Atraso no Desenvolvimento') THEN 1 ELSE 0 END,
		                            Total_Male = CASE WHEN a.Gender = 'M' THEN 1 ELSE 0 END,
		                            Total_Female = CASE WHEN a.Gender = 'F' THEN 1 ELSE 0 END
		                            FROM  [Partner] p
		                            inner join  [Partner] cp on (p.SuperiorId = cp.PartnerID) 
		                            inner join  [HouseHold] hh on (p.PartnerID = hh.PartnerID)
		                            inner join  [Beneficiary] a on (a.HouseHoldID = hh.HouseHoldID)
                                    inner join  [CSI_PROD].[dbo].[vw_beneficiary_details] ben ON ben.ID = a.BeneficiaryID AND type='adult'
		                            inner join  [ReferenceService] rs on (a.BeneficiaryID = rs.BeneficiaryID)
		                            inner join  [Reference] r on (r.ReferenceServiceID = rs.ReferenceServiceID)
		                            inner join  [ReferenceType] rt on (rt.ReferenceTypeID = r.ReferenceTypeID)
		                            --Where rt.FieldType = 'CheckBox' 
                                    WHERE p.CollaboratorRoleID = 1 AND a.BeneficiaryID IS NOT NULL
                                    AND rs.ReferenceDate between @initialDate AND @lastDate
		                            AND rt.ReferenceCategory = 'Activist' AND r.Value <> '0' AND r.Value <> '' AND r.Value IS NOT NULL
		                            AND NOT LOWER(r.Value) LIKE LOWER('%test%')--Testagem, teste, ATS
		                            AND NOT LOWER(r.Value) LIKE LOWER('%ats%')--ATS
		                            AND NOT LOWER(r.Value) LIKE LOWER('%its%') --ITS
		                            AND NOT LOWER(r.Value) LIKE LOWER('%tarv%')--TARV
		                            AND NOT LOWER(r.Value) LIKE LOWER('%profila%')--PPE
		                            AND NOT LOWER(r.Value) LIKE LOWER('%ppe%')--PPE
		                            AND NOT LOWER(r.Value) LIKE LOWER('%natal%')--Testagem, teste
		                            AND NOT LOWER(r.Value) LIKE LOWER('%ccr%') --CCR
		                            AND NOT LOWER(r.Value) LIKE LOWER('%risco%') --CCR
		                            AND NOT LOWER(r.Value) LIKE LOWER('%viol%')--GAAV
		                            AND NOT LOWER(r.Value) LIKE LOWER('%gaav%')--GAAV
		                            AND NOT LOWER(r.Value) LIKE LOWER('%parto%')--CPN Maternidade p/ Parto,Consulta Pós-Parto
		                            AND NOT LOWER(r.Value) LIKE LOWER('%cpn%')--CPN
		                            AND NOT LOWER(r.Value) LIKE LOWER('%planeamento familiar%')--CPF
		                            AND NOT LOWER(r.Value) LIKE LOWER('%oportuni%')--IO
		                            AND NOT LOWER(r.Value) LIKE LOWER('%domic%')--CD
		                            AND NOT LOWER(r.Value) LIKE LOWER('%sadia%')--CCS
		                            AND NOT LOWER(r.Value) LIKE LOWER('%ptv%')--PTV
		                            AND NOT LOWER(r.Value) LIKE LOWER('%vertical%')--PTV
		                            AND NOT LOWER(r.Value) LIKE LOWER('%bk%')--BK
		                            AND NOT LOWER(r.Value) LIKE LOWER('%mal%')--malária
		                            AND NOT LOWER(r.Value) LIKE LOWER('%circun%')--Circuncisão
		                            AND NOT LOWER(r.Value) LIKE LOWER('%pscico%')--Apoio Psico-Social
		                            AND NOT LOWER(r.Value) LIKE LOWER('%social%')--Acção Social,Apoio Psico-Social
		                            AND NOT LOWER(r.Value) LIKE LOWER('%polic%')--policial
	                            ) ref_obj
	                            group by ref_obj.ChiefPartner--<<ReplaceColumn<<--
                            ) partner_references_obj 
                            on 
                            (
	                            p.ChiefPartner--<<ReplaceColumn<<--
	                            =
	                            partner_references_obj.ChiefPartner--<<ReplaceColumn<<--
                            )";


            query = (partnerType.Equals("partner")) ? query.Replace("ChiefPartner--<<ReplaceColumn<<--", "Partner") : query;


            return UnitOfWork.DbContext.Database.SqlQuery<ActivistReferencesSummaryByRefTypeReportDTO>(query,
                                                            new SqlParameter("initialDate", initialDate),
                                                            new SqlParameter("lastDate", lastDate)).ToList();
        }



        /*
         * #####################################################
         * ########## ReferencesSummaryReport By Ages ##########
         * #####################################################
         */

        public ReferencesSummaryByAgeReportDTO getReferencesSummaryByAgeEspecific(DateTime initialDate, DateTime lastDate, List<String> referenceTypes, String OP)
        {
            String query = @"SELECT
							-- ref_obj.ReferenceName,
							ISNULL(SUM(ref_obj.Male_x_1),0) As Male_x_1,
							ISNULL(SUM(ref_obj.Female_x_1),0) As Female_x_1,
							ISNULL(SUM(ref_obj.Male_1_4),0) As Male_1_4,
							ISNULL(SUM(ref_obj.Female_1_4),0) As Female_1_4,
							ISNULL(SUM(ref_obj.Male_5_9),0) As Male_5_9,
							ISNULL(SUM(ref_obj.Female_5_9),0) As Female_5_9,
							ISNULL(SUM(ref_obj.Male_10_14),0) As Male_10_14,
							ISNULL(SUM(ref_obj.Female_10_14),0) As Female_10_14,
							ISNULL(SUM(ref_obj.Male_15_17),0) As Male_15_17,
							ISNULL(SUM(ref_obj.Female_15_17),0) As Female_15_17,
							ISNULL(SUM(ref_obj.Male_18_24),0) As Male_18_24,
							ISNULL(SUM(ref_obj.Female_18_24),0) As Female_18_24,
							ISNULL(SUM(ref_obj.Male_25_x),0) As Male_25_x,
							ISNULL(SUM(ref_obj.Female_25_x),0) As Female_25_x
							FROM
							(
                                SELECT
								rt.ReferenceName,
                                Male_x_1 = CASE WHEN DATEDIFF(year, CAST(ben.DateOfBirth As Date), GETDATE()) < 1 AND ben.Gender = 'M' THEN 1 ELSE 0 END,
                                Female_x_1 = CASE WHEN DATEDIFF(year, CAST(ben.DateOfBirth As Date), GETDATE()) < 1 AND ben.Gender = 'F' THEN 1 ELSE 0 END,
                                Male_1_4 = CASE WHEN DATEDIFF(year, CAST(ben.DateOfBirth As Date), GETDATE()) BETWEEN 1 AND 4 AND  ben.Gender = 'M'  THEN 1 ELSE 0 END,
                                Female_1_4 = CASE WHEN DATEDIFF(year, CAST(ben.DateOfBirth As Date), GETDATE()) BETWEEN 1 AND 4 AND ben.Gender = 'F' THEN 1 ELSE 0 END,
                                Male_5_9 = CASE WHEN DATEDIFF(year, CAST(ben.DateOfBirth As Date), GETDATE()) BETWEEN 5 AND 9 AND ben.Gender = 'M' THEN 1 ELSE 0 END,
                                Female_5_9 = CASE WHEN DATEDIFF(year, CAST(ben.DateOfBirth As Date), GETDATE()) BETWEEN 5 AND 9 AND ben.Gender = 'F' THEN 1 ELSE 0 END,
                                Male_10_14 = CASE WHEN DATEDIFF(year, CAST(ben.DateOfBirth As Date), GETDATE()) BETWEEN 10 AND 14 AND ben.Gender = 'M' THEN 1 ELSE 0 END,
                                Female_10_14 = CASE WHEN DATEDIFF(year, CAST(ben.DateOfBirth As Date), GETDATE()) BETWEEN 10 AND 14 AND ben.Gender = 'F' THEN 1 ELSE 0 END,
                                Male_15_17 = CASE WHEN DATEDIFF(year, CAST(ben.DateOfBirth As Date), GETDATE()) BETWEEN 15 AND 17 AND ben.Gender = 'M' THEN 1 ELSE 0 END,
                                Female_15_17 = CASE WHEN DATEDIFF(year, CAST(ben.DateOfBirth As Date), GETDATE()) BETWEEN 15 AND 17 AND ben.Gender = 'F' THEN 1 ELSE 0 END,
                                Male_18_24 = CASE WHEN DATEDIFF(year, CAST(ben.DateOfBirth As Date), GETDATE()) BETWEEN 18 AND 24 AND ben.Gender = 'M' THEN 1 ELSE 0 END,
                                Female_18_24 = CASE WHEN DATEDIFF(year, CAST(ben.DateOfBirth As Date), GETDATE()) BETWEEN 18 AND 24 AND ben.Gender = 'F' THEN 1 ELSE 0 END,
                                Male_25_x = CASE WHEN DATEDIFF(year, CAST(ben.DateOfBirth As Date), GETDATE()) >= 25 AND ben.Gender = 'M' THEN 1 ELSE 0 END,
                                Female_25_x = CASE WHEN DATEDIFF(year, CAST(ben.DateOfBirth As Date), GETDATE()) >= 25 AND ben.Gender = 'F' THEN 1 ELSE 0 END
                                FROM  [ReferenceService] rs
                                inner join  [Reference] r on (r.ReferenceServiceID = rs.ReferenceServiceID)
                                inner join  [ReferenceType] rt on (rt.ReferenceTypeID = r.ReferenceTypeID)
						        inner join [CSI_PROD].[dbo].[vw_beneficiary_details] ben ON ben.ID = rs.BeneficiaryID AND type='child'

                                WHERE rt.ReferenceName X ({0})
                                AND rs.ReferenceDate bETWEEN @initialDate AND @lastDate
                                AND rt.ReferenceCategory = 'Activist' AND r.Value <> '0' AND r.Value <> '' AND r.Value IS NOT NULL 
					            AND NOT LOWER(r.Value) LIKE LOWER('%test%')--Testagem, teste, ATS
					            AND NOT LOWER(r.Value) LIKE LOWER('%ats%')--ATS
					            AND NOT LOWER(r.Value) LIKE LOWER('%its%') --ITS
					            AND NOT LOWER(r.Value) LIKE LOWER('%tarv%')--TARV
					            AND NOT LOWER(r.Value) LIKE LOWER('%profila%')--PPE
					            AND NOT LOWER(r.Value) LIKE LOWER('%ppe%')--PPE
					            AND NOT LOWER(r.Value) LIKE LOWER('%natal%')--Testagem, teste
					            AND NOT LOWER(r.Value) LIKE LOWER('%ccr%') --CCR
					            AND NOT LOWER(r.Value) LIKE LOWER('%risco%') --CCR
					            AND NOT LOWER(r.Value) LIKE LOWER('%viol%')--GAAV
					            AND NOT LOWER(r.Value) LIKE LOWER('%gaav%')--GAAV
					            AND NOT LOWER(r.Value) LIKE LOWER('%parto%')--CPN Maternidade p/ Parto,Consulta Pós-Parto
					            AND NOT LOWER(r.Value) LIKE LOWER('%cpn%')--CPN
					            AND NOT LOWER(r.Value) LIKE LOWER('%planeamento familiar%')--CPF
					            AND NOT LOWER(r.Value) LIKE LOWER('%oportuni%')--IO
					            AND NOT LOWER(r.Value) LIKE LOWER('%domic%')--CD
					            AND NOT LOWER(r.Value) LIKE LOWER('%sadia%')--CCS
					            AND NOT LOWER(r.Value) LIKE LOWER('%ptv%')--PTV
					            AND NOT LOWER(r.Value) LIKE LOWER('%vertical%')--PTV
					            AND NOT LOWER(r.Value) LIKE LOWER('%bk%')--BK
					            AND NOT LOWER(r.Value) LIKE LOWER('%mal%')--malária
					            AND NOT LOWER(r.Value) LIKE LOWER('%circun%')--Circuncisão
					            AND NOT LOWER(r.Value) LIKE LOWER('%pscico%')--Apoio Psico-Social
					            AND NOT LOWER(r.Value) LIKE LOWER('%social%')--Acção Social,Apoio Psico-Social
					            AND NOT LOWER(r.Value) LIKE LOWER('%polic%')--policial

                                UNION ALL

                                SELECT
								rt.ReferenceName,
                                Male_x_1 = CASE WHEN DATEDIFF(year, CAST(ben.DateOfBirth As Date), GETDATE()) < 1 AND ben.Gender = 'M' THEN 1 ELSE 0 END,
                                Female_x_1 = CASE WHEN DATEDIFF(year, CAST(ben.DateOfBirth As Date), GETDATE()) < 1 AND ben.Gender = 'F' THEN 1 ELSE 0 END,
                                Male_1_4 = CASE WHEN DATEDIFF(year, CAST(ben.DateOfBirth As Date), GETDATE()) BETWEEN 1 AND 4 AND  ben.Gender = 'M'  THEN 1 ELSE 0 END,
                                Female_1_4 = CASE WHEN DATEDIFF(year, CAST(ben.DateOfBirth As Date), GETDATE()) BETWEEN 1 AND 4 AND ben.Gender = 'F' THEN 1 ELSE 0 END,
                                Male_5_9 = CASE WHEN DATEDIFF(year, CAST(ben.DateOfBirth As Date), GETDATE()) BETWEEN 5 AND 9 AND ben.Gender = 'M' THEN 1 ELSE 0 END,
                                Female_5_9 = CASE WHEN DATEDIFF(year, CAST(ben.DateOfBirth As Date), GETDATE()) BETWEEN 5 AND 9 AND ben.Gender = 'F' THEN 1 ELSE 0 END,
                                Male_10_14 = CASE WHEN DATEDIFF(year, CAST(ben.DateOfBirth As Date), GETDATE()) BETWEEN 10 AND 14 AND ben.Gender = 'M' THEN 1 ELSE 0 END,
                                Female_10_14 = CASE WHEN DATEDIFF(year, CAST(ben.DateOfBirth As Date), GETDATE()) BETWEEN 10 AND 14 AND ben.Gender = 'F' THEN 1 ELSE 0 END,
                                Male_15_17 = CASE WHEN DATEDIFF(year, CAST(ben.DateOfBirth As Date), GETDATE()) BETWEEN 15 AND 17 AND ben.Gender = 'M' THEN 1 ELSE 0 END,
                                Female_15_17 = CASE WHEN DATEDIFF(year, CAST(ben.DateOfBirth As Date), GETDATE()) BETWEEN 15 AND 17 AND ben.Gender = 'F' THEN 1 ELSE 0 END,
                                Male_18_24 = CASE WHEN DATEDIFF(year, CAST(ben.DateOfBirth As Date), GETDATE()) BETWEEN 18 AND 24 AND ben.Gender = 'M' THEN 1 ELSE 0 END,
                                Female_18_24 = CASE WHEN DATEDIFF(year, CAST(ben.DateOfBirth As Date), GETDATE()) BETWEEN 18 AND 24 AND ben.Gender = 'F' THEN 1 ELSE 0 END,
                                Male_25_x = CASE WHEN DATEDIFF(year, CAST(ben.DateOfBirth As Date), GETDATE()) >= 25 AND ben.Gender = 'M' THEN 1 ELSE 0 END,
                                Female_25_x = CASE WHEN DATEDIFF(year, CAST(ben.DateOfBirth As Date), GETDATE()) >= 25 AND ben.Gender = 'F' THEN 1 ELSE 0 END
                                FROM  [ReferenceService] rs
                                inner join  [Reference] r on (r.ReferenceServiceID = rs.ReferenceServiceID)
                                inner join  [ReferenceType] rt on (rt.ReferenceTypeID = r.ReferenceTypeID)
						        inner join [CSI_PROD].[dbo].[vw_beneficiary_details] ben ON ben.ID = rs.BeneficiaryID AND type='adult'
                                WHERE rt.ReferenceName X ({0})
                                AND rs.ReferenceDate bETWEEN @initialDate AND @lastDate
                                AND rt.ReferenceCategory = 'Activist' AND r.Value <> '0' AND r.Value <> '' AND r.Value IS NOT NULL 
					            AND NOT LOWER(r.Value) LIKE LOWER('%test%')--Testagem, teste, ATS
					            AND NOT LOWER(r.Value) LIKE LOWER('%ats%')--ATS
					            AND NOT LOWER(r.Value) LIKE LOWER('%its%') --ITS
					            AND NOT LOWER(r.Value) LIKE LOWER('%tarv%')--TARV
					            AND NOT LOWER(r.Value) LIKE LOWER('%profila%')--PPE
					            AND NOT LOWER(r.Value) LIKE LOWER('%ppe%')--PPE
					            AND NOT LOWER(r.Value) LIKE LOWER('%natal%')--Testagem, teste
					            AND NOT LOWER(r.Value) LIKE LOWER('%ccr%') --CCR
					            AND NOT LOWER(r.Value) LIKE LOWER('%risco%') --CCR
					            AND NOT LOWER(r.Value) LIKE LOWER('%viol%')--GAAV
					            AND NOT LOWER(r.Value) LIKE LOWER('%gaav%')--GAAV
					            AND NOT LOWER(r.Value) LIKE LOWER('%parto%')--CPN Maternidade p/ Parto,Consulta Pós-Parto
					            AND NOT LOWER(r.Value) LIKE LOWER('%cpn%')--CPN
					            AND NOT LOWER(r.Value) LIKE LOWER('%planeamento familiar%')--CPF
					            AND NOT LOWER(r.Value) LIKE LOWER('%oportuni%')--IO
					            AND NOT LOWER(r.Value) LIKE LOWER('%domic%')--CD
					            AND NOT LOWER(r.Value) LIKE LOWER('%sadia%')--CCS
					            AND NOT LOWER(r.Value) LIKE LOWER('%ptv%')--PTV
					            AND NOT LOWER(r.Value) LIKE LOWER('%vertical%')--PTV
					            AND NOT LOWER(r.Value) LIKE LOWER('%bk%')--BK
					            AND NOT LOWER(r.Value) LIKE LOWER('%mal%')--malária
					            AND NOT LOWER(r.Value) LIKE LOWER('%circun%')--Circuncisão
					            AND NOT LOWER(r.Value) LIKE LOWER('%pscico%')--Apoio Psico-Social
					            AND NOT LOWER(r.Value) LIKE LOWER('%social%')--Acção Social,Apoio Psico-Social
					            AND NOT LOWER(r.Value) LIKE LOWER('%polic%')--policial
							) ref_obj
							--group by ref_obj.ReferenceName";

            query = query.Replace("X", OP);
            List<SqlParameter> parametes = new List<SqlParameter>();
            string[] paramNames = referenceTypes.Select((s, i) => "@ref" + i.ToString()).ToArray();
            string inClause = string.Join(",", paramNames);

            for (int i = 0; i < paramNames.Length; i++)
            {
                parametes.Add(new SqlParameter(paramNames[i], referenceTypes[i]));
            }

            parametes.Add(new SqlParameter("initialDate", initialDate));
            parametes.Add(new SqlParameter("lastDate", lastDate));

            return UnitOfWork.DbContext.Database.SqlQuery<ReferencesSummaryByAgeReportDTO>(string.Format(query, inClause), parametes.ToArray()).SingleOrDefault();
        }

        /*
         * #######################################################
         * ###### counter References ChiefPartner for Child ######
         * #######################################################
         */

        public List<CounterReferencesSummaryByRefTypeReportDTO> getCounterReferencesForChildReportSummaryChiefPartner(DateTime initialDate, DateTime lastDate)
        {
            return getCounterReferencesForChildReportSummary(initialDate, lastDate, "chiefpartner");
        }

        /*
         * #######################################################
         * ######### counter References Partner  for Child #######
         * #######################################################
         */

        public List<CounterReferencesSummaryByRefTypeReportDTO> getCounterReferencesForChildReportSummaryPartner(DateTime initialDate, DateTime lastDate)
        {
            return getCounterReferencesForChildReportSummary(initialDate, lastDate, "partner");
        }



        /*
         * ##############################################################
         * ####### counter References for Child Report Summary ##########
         * ##############################################################
         */

        public List<CounterReferencesSummaryByRefTypeReportDTO> getCounterReferencesForChildReportSummary(DateTime initialDate, DateTime lastDate, string partnerType)
        {
          
            String query = @"SELECT  
                                p.ChiefPartner--<<ReplaceColumn<<--
                                As Partner,
                                ISNULL(partner_references_obj.ATS_Male,0) As ATS_Male,
	                            ISNULL(partner_references_obj.ATS_Female,0) As ATS_Female,
	                            ISNULL(partner_references_obj.TARV_Male,0) As TARV_Male,
	                            ISNULL(partner_references_obj.TARV_Female,0) As TARV_Female,
	                            ISNULL(partner_references_obj.CCR_Male,0) As CCR_Male,
	                            ISNULL(partner_references_obj.CCR_Female,0) As CCR_Female,
	                            ISNULL(partner_references_obj.SSR_Male,0) As SSR_Male,
	                            ISNULL(partner_references_obj.SSR_Female,0) As SSR_Female,
	                            ISNULL(partner_references_obj.VGB_Male,0) As VGB_Male,
	                            ISNULL(partner_references_obj.VGB_Female,0) As VGB_Female,
	                            ISNULL(partner_references_obj.Poverty_Proof_Male,0) As Poverty_Proof_Male,
	                            ISNULL(partner_references_obj.Poverty_Proof_Female,0) As Poverty_Proof_Female,
	                            ISNULL(partner_references_obj.Birth_Registration_Male,0) As Birth_Registration_Male,
	                            ISNULL(partner_references_obj.Birth_Registration_Female,0) As Birth_Registration_Female,
	                            ISNULL(partner_references_obj.Identification_Card_Male,0) As Identification_Card_Male,
	                            ISNULL(partner_references_obj.Identification_Card_Female,0) As Identification_Card_Female,
	                            ISNULL(partner_references_obj.School_Integration_Male,0) As School_Integration_Male,
	                            ISNULL(partner_references_obj.School_Integration_Female,0) As School_Integration_Female,
	                            ISNULL(partner_references_obj.Vocational_Courses_Male,0) As Vocational_Courses_Male,
	                            ISNULL(partner_references_obj.Vocational_Courses_Female,0) As Vocational_Courses_Female,
	                            ISNULL(partner_references_obj.School_Material_Male,0) As School_Material_Male,
	                            ISNULL(partner_references_obj.School_Material_Female,0) As School_Material_Female,
	                            ISNULL(partner_references_obj.Basic_Basket_Male,0) As Basic_Basket_Male,
	                            ISNULL(partner_references_obj.Basic_Basket_Female,0) As Basic_Basket_Female,
	                            ISNULL(partner_references_obj.INAS_Benefit_Male,0) As INAS_Benefit_Male,
	                            ISNULL(partner_references_obj.INAS_Benefit_Female,0) As INAS_Benefit_Female,
	                            ISNULL(partner_references_obj.SAAJ_Male,0) As SAAJ_Male,
	                            ISNULL(partner_references_obj.SAAJ_Female,0) As SAAJ_Female,
	                            ISNULL(partner_references_obj.Desnutrition_Male,0) As Desnutrition_Male,
	                            ISNULL(partner_references_obj.Desnutrition_Female,0) As Desnutrition_Female,
	                            ISNULL(partner_references_obj.Delay_in_Development_Male,0) As Delay_in_Development_Male,
	                            ISNULL(partner_references_obj.Delay_in_Development_Female,0) As Delay_in_Development_Female,
	                            ISNULL(partner_references_obj.Others_Male,0) As Others_Male,
	                            ISNULL(partner_references_obj.Others_Female,0) As Others_Female
                            FROM
                            (
	                            SELECT
	                            ChiefPartner--<<ReplaceColumn<<--
	                            FROM
	                            (
		                            SELECT cp.Name As ChiefPartner, p.Name As Partner
		                            FROM  [Partner] p
		                            inner join  [Partner] cp on (p.SuperiorId = cp.PartnerID)
		                            Where p.CollaboratorRoleID = 1
	                            ) p
	                            group by p.ChiefPartner--<<ReplaceColumn<<--
                            ) p
                            left join
                            (	
	                            SELECT  --  Group By Child
	                            ref_obj.ChiefPartner--<<ReplaceColumn<<--
	                            ,
	                            SUM(ref_obj.ATS_Male) As ATS_Male,
	                            SUM(ref_obj.ATS_Female) As ATS_Female,
	                            SUM(ref_obj.TARV_Male) As TARV_Male,
	                            SUM(ref_obj.TARV_Female) As TARV_Female,
	                            SUM(ref_obj.CCR_Male) As CCR_Male,
	                            SUM(ref_obj.CCR_Female) As CCR_Female,
	                            SUM(ref_obj.SSR_Male) As SSR_Male,
	                            SUM(ref_obj.SSR_Female) As SSR_Female,
	                            SUM(ref_obj.VGB_Male) As VGB_Male,
	                            SUM(ref_obj.VGB_Female) As VGB_Female,
	                            SUM(ref_obj.Poverty_Proof_Male) As Poverty_Proof_Male,
	                            SUM(ref_obj.Poverty_Proof_Female) As Poverty_Proof_Female,
	                            SUM(ref_obj.Birth_Registration_Male) As Birth_Registration_Male,
	                            SUM(ref_obj.Birth_Registration_Female) As Birth_Registration_Female,
	                            SUM(ref_obj.Identification_Card_Male) As Identification_Card_Male,
	                            SUM(ref_obj.Identification_Card_Female) As Identification_Card_Female,
	                            SUM(ref_obj.School_Integration_Male) As School_Integration_Male,
	                            SUM(ref_obj.School_Integration_Female) As School_Integration_Female,
	                            SUM(ref_obj.Vocational_Courses_Male) As Vocational_Courses_Male,
	                            SUM(ref_obj.Vocational_Courses_Female) As Vocational_Courses_Female,
	                            SUM(ref_obj.School_Material_Male) As School_Material_Male,
	                            SUM(ref_obj.School_Material_Female) As School_Material_Female,
	                            SUM(ref_obj.Basic_Basket_Male) As Basic_Basket_Male,
	                            SUM(ref_obj.Basic_Basket_Female) As Basic_Basket_Female,
	                            SUM(ref_obj.INAS_Benefit_Male) As INAS_Benefit_Male,
	                            SUM(ref_obj.INAS_Benefit_Female) As INAS_Benefit_Female,
	                            SUM(ref_obj.SAAJ_Male) As SAAJ_Male,
	                            SUM(ref_obj.SAAJ_Female) As SAAJ_Female,
	                            SUM(ref_obj.Desnutrition_Male) As Desnutrition_Male,
	                            SUM(ref_obj.Desnutrition_Female) As Desnutrition_Female,
	                            SUM(ref_obj.Delay_in_Development_Male) As Delay_in_Development_Male,
	                            SUM(ref_obj.Delay_in_Development_Female) As Delay_in_Development_Female,
	                            SUM(ref_obj.Others_Male) As Others_Male,
	                            SUM(ref_obj.Others_Female) As Others_Female,
	                            SUM(ref_obj.Total_Male) As Total_Male,
	                            SUM(ref_obj.Total_Female) As Total_Female
	                            FROM
	                            (
		                            SELECT
		                            cp.Name As ChiefPartner,
                                    p.Name AS [Partner],
		                            c.FirstName,
		                            c.LastName,
		                            ATS_Male = CASE WHEN c.Gender = 'M' AND rt.ReferenceName = 'ATS' THEN 1 ELSE 0 END,
		                            ATS_Female = CASE WHEN c.Gender = 'F' AND rt.ReferenceName = 'ATS' THEN 1 ELSE 0 END,
		                            TARV_Male = CASE WHEN c.Gender = 'M' AND rt.ReferenceName in ('Pré TARV/IO','Testado HIV+','PPE','PTV','TARV','CD') THEN 1 ELSE 0 END,
		                            TARV_Female = CASE WHEN c.Gender = 'F' AND rt.ReferenceName  in ('Pré TARV/IO','Testado HIV+','PPE','PTV','TARV','CD') THEN 1 ELSE 0 END,
		                            CCR_Male = CASE WHEN c.Gender = 'M' AND rt.ReferenceName = 'CCR' THEN 1 ELSE 0 END,
		                            CCR_Female = CASE WHEN c.Gender = 'F' AND rt.ReferenceName = 'CCR' THEN 1 ELSE 0 END,
		                            SSR_Male = CASE WHEN c.Gender = 'M' AND rt.ReferenceName in ('Maternidade p/ parto','CPN','CPN Familiar','Consulta Pós-parto','ITS') THEN 1 ELSE 0 END,
		                            SSR_Female = CASE WHEN c.Gender = 'F' AND rt.ReferenceName in ('Maternidade p/ parto','CPN','CPN Familiar','Consulta Pós-parto','ITS') THEN 1 ELSE 0 END,
		                            VGB_Male = CASE WHEN c.Gender = 'M' AND rt.ReferenceName in ('GAVV') THEN 1 ELSE 0 END,
		                            VGB_Female = CASE WHEN c.Gender = 'F' AND rt.ReferenceName in ('GAVV') THEN 1 ELSE 0 END,
		                            Poverty_Proof_Male = CASE WHEN c.Gender = 'M' AND rt.ReferenceName = 'Atestado de Pobreza' THEN 1 ELSE 0 END,
		                            Poverty_Proof_Female = CASE WHEN c.Gender = 'F' AND rt.ReferenceName = 'Atestado de Pobreza' THEN 1 ELSE 0 END,
		                            Birth_Registration_Male = CASE WHEN c.Gender = 'M' AND rt.ReferenceName = 'Registo de Nascimento/Cédula' THEN 1 ELSE 0 END,
		                            Birth_Registration_Female = CASE WHEN c.Gender = 'F' AND rt.ReferenceName = 'Registo de Nascimento/Cédula' THEN 1 ELSE 0 END,
		                            Identification_Card_Male = CASE WHEN c.Gender = 'M' AND rt.ReferenceName = 'Bilhete de Identidade (B.I)' THEN 1 ELSE 0 END,
		                            Identification_Card_Female = CASE WHEN c.Gender = 'F' AND rt.ReferenceName = 'Bilhete de Identidade (B.I)' THEN 1 ELSE 0 END,
		                            School_Integration_Male = CASE WHEN c.Gender = 'M' AND rt.ReferenceName = 'Integração Escolar' THEN 1 ELSE 0 END,
		                            School_Integration_Female = CASE WHEN c.Gender = 'F' AND rt.ReferenceName = 'Integração Escolar' THEN 1 ELSE 0 END,
		                            Vocational_Courses_Male = CASE WHEN c.Gender = 'M' AND rt.ReferenceName = 'Curso de Formação Vocacional' THEN 1 ELSE 0 END,
		                            Vocational_Courses_Female = CASE WHEN c.Gender = 'F' AND rt.ReferenceName = 'Curso de Formação Vocacional' THEN 1 ELSE 0 END,
		                            School_Material_Male = CASE WHEN c.Gender = 'M' AND rt.ReferenceName = 'Material Escolar' THEN 1 ELSE 0 END,
		                            School_Material_Female = CASE WHEN c.Gender = 'F' AND rt.ReferenceName = 'Material Escolar' THEN 1 ELSE 0 END,
		                            Basic_Basket_Male = CASE WHEN c.Gender = 'M' AND rt.ReferenceName = 'Cesta Básica' THEN 1 ELSE 0 END,
		                            Basic_Basket_Female = CASE WHEN c.Gender = 'F' AND rt.ReferenceName = 'Cesta Básica' THEN 1 ELSE 0 END,
		                            INAS_Benefit_Male = CASE WHEN c.Gender = 'M' AND rt.ReferenceName = 'Subsídios Sociais do INAS' THEN 1 ELSE 0 END,
		                            INAS_Benefit_Female = CASE WHEN c.Gender = 'F' AND rt.ReferenceName = 'Subsídios Sociais do INAS' THEN 1 ELSE 0 END,
		                            SAAJ_Male = CASE WHEN c.Gender = 'M' AND rt.ReferenceName = 'SAAJ' THEN 1 ELSE 0 END,
		                            SAAJ_Female = CASE WHEN c.Gender = 'F' AND rt.ReferenceName = 'SAAJ' THEN 1 ELSE 0 END,
		                            Desnutrition_Male = CASE WHEN c.Gender = 'M' AND rt.ReferenceName = 'Desnutrição' THEN 1 ELSE 0 END,
		                            Desnutrition_Female = CASE WHEN c.Gender = 'F' AND rt.ReferenceName = 'Desnutrição' THEN 1 ELSE 0 END,
		                            Delay_in_Development_Male = CASE WHEN c.Gender = 'M' AND rt.ReferenceName = 'Atraso no Desenvolvimento' THEN 1 ELSE 0 END,
		                            Delay_in_Development_Female = CASE WHEN c.Gender = 'F' AND rt.ReferenceName = 'Atraso no Desenvolvimento' THEN 1 ELSE 0 END,
		                            Others_Male = CASE WHEN c.Gender = 'M' AND rt.ReferenceName not in ('ATS','Pré TARV/IO','Testado HIV+','PPE','PTV','TARV','CD','CCR','Maternidade p/ parto'
	                                ,'CPN','CPN Familiar','Consulta Pós-parto','ITS','GAVV','Atestado de Pobreza','Registo de Nascimento/Cédula','Bilhete de Identidade (B.I)'
	                                ,'Integração Escolar','Curso de Formação Vocacional','Material Escolar','Cesta Básica','Subsídios Sociais do INAS'
		                            ,'SAAJ','Desnutrição', 'Atraso no Desenvolvimento') THEN 1 ELSE 0 END,
		                            Others_Female = CASE WHEN c.Gender = 'F' AND rt.ReferenceName not in ('ATS','Pré TARV/IO','Testado HIV+','PPE','PTV','TARV','CD','CCR','Maternidade p/ parto'
	                                ,'CPN','CPN Familiar','Consulta Pós-parto','ITS','GAVV','Atestado de Pobreza','Registo de Nascimento/Cédula','Bilhete de Identidade (B.I)'
	                                ,'Integração Escolar','Curso de Formação Vocacional','Material Escolar','Cesta Básica','Subsídios Sociais do INAS'
		                            ,'SAAJ','Desnutrição', 'Atraso no Desenvolvimento') THEN 1 ELSE 0 END,
		                            Total_Male = CASE WHEN c.Gender = 'M' THEN 1 ELSE 0 END,
		                            Total_Female = CASE WHEN c.Gender = 'F' THEN 1 ELSE 0 END
		                            FROM  [Partner] p
		                            inner join  [Partner] cp on (p.SuperiorId = cp.PartnerID) 
		                            inner join  [HouseHold] hh on (p.PartnerID = hh.PartnerID)
						            inner join [CSI_PROD].[dbo].[Beneficiary] AS c ON hh.HouseHoldID = c.HouseholdID
						            inner join [CSI_PROD].[dbo].[vw_beneficiary_details] ben ON ben.ID = c.BeneficiaryID AND type='child'
		                            inner join  [ReferenceService] rs on (c.BeneficiaryID = rs.BeneficiaryID)
		                            inner join  [Reference] r on (r.ReferenceServiceID = rs.ReferenceServiceID)
		                            inner join  [ReferenceType] rt on (rt.ReferenceTypeID = r.ReferenceTypeID)
                                    WHERE p.CollaboratorRoleID = 1 AND rs.BeneficiaryID IS NOT NULL AND ben.Type = 'child'
                                    AND ((rs.HealthAttendedDate between @initialDate AND @lastDate)
                                    OR (rs.SocialAttendedDate between @initialDate AND @lastDate))
                                    AND rt.ReferenceCategory in ('Health','Social') AND r.Value <> '0' AND r.Value <> '' AND r.Value IS NOT NULL
		                            AND NOT LOWER(r.Value) LIKE LOWER('%test%')--Testagem, teste, ATS
		                            AND NOT LOWER(r.Value) LIKE LOWER('%ats%')--ATS
		                            AND NOT LOWER(r.Value) LIKE LOWER('%its%') --ITS
		                            AND NOT LOWER(r.Value) LIKE LOWER('%tarv%')--TARV
		                            AND NOT LOWER(r.Value) LIKE LOWER('%profila%')--PPE
		                            AND NOT LOWER(r.Value) LIKE LOWER('%ppe%')--PPE
		                            AND NOT LOWER(r.Value) LIKE LOWER('%natal%')--Testagem, teste
		                            AND NOT LOWER(r.Value) LIKE LOWER('%ccr%') --CCR
		                            AND NOT LOWER(r.Value) LIKE LOWER('%risco%') --CCR
		                            AND NOT LOWER(r.Value) LIKE LOWER('%viol%')--GAAV
		                            AND NOT LOWER(r.Value) LIKE LOWER('%gaav%')--GAAV
		                            AND NOT LOWER(r.Value) LIKE LOWER('%parto%')--CPN Maternidade p/ Parto,Consulta Pós-Parto
		                            AND NOT LOWER(r.Value) LIKE LOWER('%cpn%')--CPN
		                            AND NOT LOWER(r.Value) LIKE LOWER('%planeamento familiar%')--CPF
		                            AND NOT LOWER(r.Value) LIKE LOWER('%oportuni%')--IO
		                            AND NOT LOWER(r.Value) LIKE LOWER('%domic%')--CD
		                            AND NOT LOWER(r.Value) LIKE LOWER('%sadia%')--CCS
		                            AND NOT LOWER(r.Value) LIKE LOWER('%ptv%')--PTV
		                            AND NOT LOWER(r.Value) LIKE LOWER('%vertical%')--PTV
		                            AND NOT LOWER(r.Value) LIKE LOWER('%bk%')--BK
		                            AND NOT LOWER(r.Value) LIKE LOWER('%mal%')--malária
		                            AND NOT LOWER(r.Value) LIKE LOWER('%circun%')--Circuncisão
		                            AND NOT LOWER(r.Value) LIKE LOWER('%pscico%')--Apoio Psico-Social
		                            AND NOT LOWER(r.Value) LIKE LOWER('%social%')--Acção Social,Apoio Psico-Social
		                            AND NOT LOWER(r.Value) LIKE LOWER('%polic%')--policial
	                            ) ref_obj
	                            group by ref_obj.ChiefPartner--<<ReplaceColumn<<--
                            ) partner_references_obj 
                            on 
                            (
	                            p.ChiefPartner--<<ReplaceColumn<<--
	                            =
	                            partner_references_obj.ChiefPartner--<<ReplaceColumn<<--
                            )";

            query = (partnerType.Equals("partner")) ? query.Replace("ChiefPartner--<<ReplaceColumn<<--", "Partner") : query;

            return UnitOfWork.DbContext.Database.SqlQuery<CounterReferencesSummaryByRefTypeReportDTO>(query,
                                                            new SqlParameter("initialDate", initialDate),
                                                            new SqlParameter("lastDate", lastDate)).ToList();
        }

        /*
         * #######################################################
         * ###### counter References ChiefPartner for Adult ######
         * #######################################################
         */

        public List<CounterReferencesSummaryByRefTypeReportDTO> getCounterReferencesForAdultReportSummaryChiefPartner(DateTime initialDate, DateTime lastDate)
        {
            return getCounterReferencesForAdultReportSummary(initialDate, lastDate, "chiefpartner");
        }

        /*
         * #######################################################
         * ######### counter References Partner  for Adult #######
         * #######################################################
         */

        public List<CounterReferencesSummaryByRefTypeReportDTO> getCounterReferencesForAdultReportSummaryPartner(DateTime initialDate, DateTime lastDate)
        {
            return getCounterReferencesForAdultReportSummary(initialDate, lastDate, "partner");
        }






        /*
         * ##############################################################
         * ####### Counter References for Adult Report Summary ##########
         * ##############################################################
         */

        public List<CounterReferencesSummaryByRefTypeReportDTO> getCounterReferencesForAdultReportSummary(DateTime initialDate, DateTime lastDate, string partnerType)
        {
    
            String query = @"SELECT  
                                p.ChiefPartner--<<ReplaceColumn<<--
                                As Partner,
                                ISNULL(partner_references_obj.ATS_Male,0) As ATS_Male,
	                            ISNULL(partner_references_obj.ATS_Female,0) As ATS_Female,
	                            ISNULL(partner_references_obj.TARV_Male,0) As TARV_Male,
	                            ISNULL(partner_references_obj.TARV_Female,0) As TARV_Female,
	                            ISNULL(partner_references_obj.CCR_Male,0) As CCR_Male,
	                            ISNULL(partner_references_obj.CCR_Female,0) As CCR_Female,
	                            ISNULL(partner_references_obj.SSR_Male,0) As SSR_Male,
	                            ISNULL(partner_references_obj.SSR_Female,0) As SSR_Female,
	                            ISNULL(partner_references_obj.VGB_Male,0) As VGB_Male,
	                            ISNULL(partner_references_obj.VGB_Female,0) As VGB_Female,
	                            ISNULL(partner_references_obj.Poverty_Proof_Male,0) As Poverty_Proof_Male,
	                            ISNULL(partner_references_obj.Poverty_Proof_Female,0) As Poverty_Proof_Female,
	                            ISNULL(partner_references_obj.Birth_Registration_Male,0) As Birth_Registration_Male,
	                            ISNULL(partner_references_obj.Birth_Registration_Female,0) As Birth_Registration_Female,
	                            ISNULL(partner_references_obj.Identification_Card_Male,0) As Identification_Card_Male,
	                            ISNULL(partner_references_obj.Identification_Card_Female,0) As Identification_Card_Female,
	                            ISNULL(partner_references_obj.School_Integration_Male,0) As School_Integration_Male,
	                            ISNULL(partner_references_obj.School_Integration_Female,0) As School_Integration_Female,
	                            ISNULL(partner_references_obj.Vocational_Courses_Male,0) As Vocational_Courses_Male,
	                            ISNULL(partner_references_obj.Vocational_Courses_Female,0) As Vocational_Courses_Female,
	                            ISNULL(partner_references_obj.School_Material_Male,0) As School_Material_Male,
	                            ISNULL(partner_references_obj.School_Material_Female,0) As School_Material_Female,
	                            ISNULL(partner_references_obj.Basic_Basket_Male,0) As Basic_Basket_Male,
	                            ISNULL(partner_references_obj.Basic_Basket_Female,0) As Basic_Basket_Female,
	                            ISNULL(partner_references_obj.INAS_Benefit_Male,0) As INAS_Benefit_Male,
	                            ISNULL(partner_references_obj.INAS_Benefit_Female,0) As INAS_Benefit_Female,
	                            ISNULL(partner_references_obj.SAAJ_Male,0) As SAAJ_Male,
	                            ISNULL(partner_references_obj.SAAJ_Female,0) As SAAJ_Female,
	                            ISNULL(partner_references_obj.Desnutrition_Male,0) As Desnutrition_Male,
	                            ISNULL(partner_references_obj.Desnutrition_Female,0) As Desnutrition_Female,
	                            ISNULL(partner_references_obj.Delay_in_Development_Male,0) As Delay_in_Development_Male,
	                            ISNULL(partner_references_obj.Delay_in_Development_Female,0) As Delay_in_Development_Female,
	                            ISNULL(partner_references_obj.Others_Male,0) As Others_Male,
	                            ISNULL(partner_references_obj.Others_Female,0) As Others_Female
                            FROM
                            (
	                            SELECT
	                            ChiefPartner--<<ReplaceColumn<<--
	                            FROM
	                            (
		                            SELECT cp.Name As ChiefPartner, p.Name As Partner
		                            FROM  [Partner] p
		                            inner join  [Partner] cp on (p.SuperiorId = cp.PartnerID)
		                            Where p.CollaboratorRoleID = 1
	                            ) p
	                            group by p.ChiefPartner--<<ReplaceColumn<<--
                            ) p
                            left join
                            (	
	                            SELECT  --  Group By Adult
	                            ref_obj.ChiefPartner--<<ReplaceColumn<<--
		                        ,
	                            SUM(ref_obj.ATS_Male) As ATS_Male,
	                            SUM(ref_obj.ATS_Female) As ATS_Female,
	                            SUM(ref_obj.TARV_Male) As TARV_Male,
	                            SUM(ref_obj.TARV_Female) As TARV_Female,
	                            SUM(ref_obj.CCR_Male) As CCR_Male,
	                            SUM(ref_obj.CCR_Female) As CCR_Female,
	                            SUM(ref_obj.SSR_Male) As SSR_Male,
	                            SUM(ref_obj.SSR_Female) As SSR_Female,
	                            SUM(ref_obj.VGB_Male) As VGB_Male,
	                            SUM(ref_obj.VGB_Female) As VGB_Female,
	                            SUM(ref_obj.Poverty_Proof_Male) As Poverty_Proof_Male,
	                            SUM(ref_obj.Poverty_Proof_Female) As Poverty_Proof_Female,
	                            SUM(ref_obj.Birth_Registration_Male) As Birth_Registration_Male,
	                            SUM(ref_obj.Birth_Registration_Female) As Birth_Registration_Female,
	                            SUM(ref_obj.Identification_Card_Male) As Identification_Card_Male,
	                            SUM(ref_obj.Identification_Card_Female) As Identification_Card_Female,
	                            SUM(ref_obj.School_Integration_Male) As School_Integration_Male,
	                            SUM(ref_obj.School_Integration_Female) As School_Integration_Female,
	                            SUM(ref_obj.Vocational_Courses_Male) As Vocational_Courses_Male,
	                            SUM(ref_obj.Vocational_Courses_Female) As Vocational_Courses_Female,
	                            SUM(ref_obj.School_Material_Male) As School_Material_Male,
	                            SUM(ref_obj.School_Material_Female) As School_Material_Female,
	                            SUM(ref_obj.Basic_Basket_Male) As Basic_Basket_Male,
	                            SUM(ref_obj.Basic_Basket_Female) As Basic_Basket_Female,
	                            SUM(ref_obj.INAS_Benefit_Male) As INAS_Benefit_Male,
	                            SUM(ref_obj.INAS_Benefit_Female) As INAS_Benefit_Female,
	                            SUM(ref_obj.SAAJ_Male) As SAAJ_Male,
	                            SUM(ref_obj.SAAJ_Female) As SAAJ_Female,
	                            SUM(ref_obj.Desnutrition_Male) As Desnutrition_Male,
	                            SUM(ref_obj.Desnutrition_Female) As Desnutrition_Female,
	                            SUM(ref_obj.Delay_in_Development_Male) As Delay_in_Development_Male,
	                            SUM(ref_obj.Delay_in_Development_Female) As Delay_in_Development_Female,
	                            SUM(ref_obj.Others_Male) As Others_Male,
	                            SUM(ref_obj.Others_Female) As Others_Female,
	                            SUM(ref_obj.Total_Male) As RC_Total_Male,
	                            SUM(ref_obj.Total_Female) As RC_Total_Female
	                            FROM
	                            (
		                            SELECT
		                            cp.Name As ChiefPartner, 
			                        p.[Name] As [Partner], 
			                        a.FirstName,
		                            a.LastName,
		                            ATS_Male = CASE WHEN a.Gender = 'M' AND rt.ReferenceName = 'ATS' THEN 1 ELSE 0 END,
		                            ATS_Female = CASE WHEN a.Gender = 'F' AND rt.ReferenceName = 'ATS' THEN 1 ELSE 0 END,
		                            TARV_Male = CASE WHEN a.Gender = 'M' AND rt.ReferenceName in ('Pré TARV/IO','Testado HIV+','PPE','PTV','TARV','CD') THEN 1 ELSE 0 END,
		                            TARV_Female = CASE WHEN a.Gender = 'F' AND rt.ReferenceName  in ('Pré TARV/IO','Testado HIV+','PPE','PTV','TARV','CD') THEN 1 ELSE 0 END,
		                            CCR_Male = CASE WHEN a.Gender = 'M' AND rt.ReferenceName = 'CCR' THEN 1 ELSE 0 END,
		                            CCR_Female = CASE WHEN a.Gender = 'F' AND rt.ReferenceName = 'CCR' THEN 1 ELSE 0 END,
		                            SSR_Male = CASE WHEN a.Gender = 'M' AND rt.ReferenceName in ('Maternidade p/ parto','CPN','CPN Familiar','Consulta Pós-parto','ITS') THEN 1 ELSE 0 END,
		                            SSR_Female = CASE WHEN a.Gender = 'F' AND rt.ReferenceName in ('Maternidade p/ parto','CPN','CPN Familiar','Consulta Pós-parto','ITS') THEN 1 ELSE 0 END,
		                            VGB_Male = CASE WHEN a.Gender = 'M' AND rt.ReferenceName in ('GAVV') THEN 1 ELSE 0 END,
		                            VGB_Female = CASE WHEN a.Gender = 'F' AND rt.ReferenceName in ('GAVV') THEN 1 ELSE 0 END,
		                            Poverty_Proof_Male = CASE WHEN a.Gender = 'M' AND rt.ReferenceName = 'Atestado de Pobreza' THEN 1 ELSE 0 END,
		                            Poverty_Proof_Female = CASE WHEN a.Gender = 'F' AND rt.ReferenceName = 'Atestado de Pobreza' THEN 1 ELSE 0 END,
		                            Birth_Registration_Male = CASE WHEN a.Gender = 'M' AND rt.ReferenceName = 'Registo de Nascimento/Cédula' THEN 1 ELSE 0 END,
		                            Birth_Registration_Female = CASE WHEN a.Gender = 'F' AND rt.ReferenceName = 'Registo de Nascimento/Cédula' THEN 1 ELSE 0 END,
		                            Identification_Card_Male = CASE WHEN a.Gender = 'M' AND rt.ReferenceName = 'Bilhete de Identidade (B.I)' THEN 1 ELSE 0 END,
		                            Identification_Card_Female = CASE WHEN a.Gender = 'F' AND rt.ReferenceName = 'Bilhete de Identidade (B.I)' THEN 1 ELSE 0 END,
		                            School_Integration_Male = CASE WHEN a.Gender = 'M' AND rt.ReferenceName = 'Integração Escolar' THEN 1 ELSE 0 END,
		                            School_Integration_Female = CASE WHEN a.Gender = 'F' AND rt.ReferenceName = 'Integração Escolar' THEN 1 ELSE 0 END,
		                            Vocational_Courses_Male = CASE WHEN a.Gender = 'M' AND rt.ReferenceName = 'Curso de Formação Vocacional' THEN 1 ELSE 0 END,
		                            Vocational_Courses_Female = CASE WHEN a.Gender = 'F' AND rt.ReferenceName = 'Curso de Formação Vocacional' THEN 1 ELSE 0 END,
		                            School_Material_Male = CASE WHEN a.Gender = 'M' AND rt.ReferenceName = 'Material Escolar' THEN 1 ELSE 0 END,
		                            School_Material_Female = CASE WHEN a.Gender = 'F' AND rt.ReferenceName = 'Material Escolar' THEN 1 ELSE 0 END,
		                            Basic_Basket_Male = CASE WHEN a.Gender = 'M' AND rt.ReferenceName = 'Cesta Básica' THEN 1 ELSE 0 END,
		                            Basic_Basket_Female = CASE WHEN a.Gender = 'F' AND rt.ReferenceName = 'Cesta Básica' THEN 1 ELSE 0 END,
		                            INAS_Benefit_Male = CASE WHEN a.Gender = 'M' AND rt.ReferenceName = 'Subsídios Sociais do INAS' THEN 1 ELSE 0 END,
		                            INAS_Benefit_Female = CASE WHEN a.Gender = 'F' AND rt.ReferenceName = 'Subsídios Sociais do INAS' THEN 1 ELSE 0 END,
		                            SAAJ_Male = CASE WHEN a.Gender = 'M' AND rt.ReferenceName = 'SAAJ' THEN 1 ELSE 0 END,
		                            SAAJ_Female = CASE WHEN a.Gender = 'F' AND rt.ReferenceName = 'SAAJ' THEN 1 ELSE 0 END,
		                            Desnutrition_Male = CASE WHEN a.Gender = 'M' AND rt.ReferenceName = 'Desnutrição' THEN 1 ELSE 0 END,
		                            Desnutrition_Female = CASE WHEN a.Gender = 'F' AND rt.ReferenceName = 'Desnutrição' THEN 1 ELSE 0 END,
		                            Delay_in_Development_Male = CASE WHEN a.Gender = 'M' AND rt.ReferenceName = 'Atraso no Desenvolvimento' THEN 1 ELSE 0 END,
		                            Delay_in_Development_Female = CASE WHEN a.Gender = 'F' AND rt.ReferenceName = 'Atraso no Desenvolvimento' THEN 1 ELSE 0 END,
		                            Others_Male = CASE WHEN a.Gender = 'M' AND rt.ReferenceName not in ('ATS','Pré TARV/IO','Testado HIV+','PPE','PTV','TARV','CD','CCR','Maternidade p/ parto'
	                                ,'CPN','CPN Familiar','Consulta Pós-parto','ITS','GAVV','Atestado de Pobreza','Registo de Nascimento/Cédula','Bilhete de Identidade (B.I)'
	                                ,'Integração Escolar','Curso de Formação Vocacional','Material Escolar','Cesta Básica','Subsídios Sociais do INAS'
		                            ,'SAAJ','Desnutrição', 'Atraso no Desenvolvimento') THEN 1 ELSE 0 END,
		                            Others_Female = CASE WHEN a.Gender = 'F' AND rt.ReferenceName not in ('ATS','Pré TARV/IO','Testado HIV+','PPE','PTV','TARV','CD','CCR','Maternidade p/ parto'
	                                ,'CPN','CPN Familiar','Consulta Pós-parto','ITS','GAVV','Atestado de Pobreza','Registo de Nascimento/Cédula','Bilhete de Identidade (B.I)'
	                                ,'Integração Escolar','Curso de Formação Vocacional','Material Escolar','Cesta Básica','Subsídios Sociais do INAS'
		                            ,'SAAJ','Desnutrição', 'Atraso no Desenvolvimento') THEN 1 ELSE 0 END,
		                            Total_Male = CASE WHEN a.Gender = 'M' THEN 1 ELSE 0 END,
		                            Total_Female = CASE WHEN a.Gender = 'F' THEN 1 ELSE 0 END
		                            FROM  [Partner] p
		                            inner join  [Partner] cp on (p.SuperiorId = cp.PartnerID) 
		                            inner join  [HouseHold] hh on (p.PartnerID = hh.PartnerID)
						            inner join [CSI_PROD].[dbo].[Beneficiary] AS a ON hh.HouseHoldID = a.HouseholdID
						            inner join [CSI_PROD].[dbo].[vw_beneficiary_details] ben ON ben.ID = a.BeneficiaryID AND type='adult'
		                            inner join  [ReferenceService] rs on (a.BeneficiaryID = rs.BeneficiaryID)
		                            inner join  [Reference] r on (r.ReferenceServiceID = rs.ReferenceServiceID)
		                            inner join  [ReferenceType] rt on (rt.ReferenceTypeID = r.ReferenceTypeID)
                                    WHERE p.CollaboratorRoleID = 1 AND rs.BeneficiaryID IS NOT NULL
                                    AND ((rs.HealthAttendedDate between @initialDate AND @lastDate)
                                    OR (rs.SocialAttendedDate between @initialDate AND @lastDate)) AND ben.Type = 'adult'
                                    AND rt.ReferenceCategory in ('Health','Social') AND r.Value <> '0' AND r.Value <> '' AND r.Value IS NOT NULL
		                            AND NOT LOWER(r.Value) LIKE LOWER('%test%')--Testagem, teste, ATS
		                            AND NOT LOWER(r.Value) LIKE LOWER('%ats%')--ATS
		                            AND NOT LOWER(r.Value) LIKE LOWER('%its%') --ITS
		                            AND NOT LOWER(r.Value) LIKE LOWER('%tarv%')--TARV
		                            AND NOT LOWER(r.Value) LIKE LOWER('%profila%')--PPE
		                            AND NOT LOWER(r.Value) LIKE LOWER('%ppe%')--PPE
		                            AND NOT LOWER(r.Value) LIKE LOWER('%natal%')--Testagem, teste
		                            AND NOT LOWER(r.Value) LIKE LOWER('%ccr%') --CCR
		                            AND NOT LOWER(r.Value) LIKE LOWER('%risco%') --CCR
		                            AND NOT LOWER(r.Value) LIKE LOWER('%viol%')--GAAV
		                            AND NOT LOWER(r.Value) LIKE LOWER('%gaav%')--GAAV
		                            AND NOT LOWER(r.Value) LIKE LOWER('%parto%')--CPN Maternidade p/ Parto,Consulta Pós-Parto
		                            AND NOT LOWER(r.Value) LIKE LOWER('%cpn%')--CPN
		                            AND NOT LOWER(r.Value) LIKE LOWER('%planeamento familiar%')--CPF
		                            AND NOT LOWER(r.Value) LIKE LOWER('%oportuni%')--IO
		                            AND NOT LOWER(r.Value) LIKE LOWER('%domic%')--CD
		                            AND NOT LOWER(r.Value) LIKE LOWER('%sadia%')--CCS
		                            AND NOT LOWER(r.Value) LIKE LOWER('%ptv%')--PTV
		                            AND NOT LOWER(r.Value) LIKE LOWER('%vertical%')--PTV
		                            AND NOT LOWER(r.Value) LIKE LOWER('%bk%')--BK
		                            AND NOT LOWER(r.Value) LIKE LOWER('%mal%')--malária
		                            AND NOT LOWER(r.Value) LIKE LOWER('%circun%')--Circuncisão
		                            AND NOT LOWER(r.Value) LIKE LOWER('%pscico%')--Apoio Psico-Social
		                            AND NOT LOWER(r.Value) LIKE LOWER('%social%')--Acção Social,Apoio Psico-Social
		                            AND NOT LOWER(r.Value) LIKE LOWER('%polic%')--policial
	                            ) ref_obj
	                            group by ref_obj.ChiefPartner--<<ReplaceColumn<<--
                            ) partner_references_obj 
                            on 
                            (
	                            p.ChiefPartner--<<ReplaceColumn<<--
	                            =
	                            partner_references_obj.ChiefPartner--<<ReplaceColumn<<--
                            )";

            query = (partnerType.Equals("partner")) ? query.Replace("ChiefPartner--<<ReplaceColumn<<--", "Partner") : query;

            return UnitOfWork.DbContext.Database.SqlQuery<CounterReferencesSummaryByRefTypeReportDTO>(query,
                                                            new SqlParameter("initialDate", initialDate),
                                                            new SqlParameter("lastDate", lastDate)).ToList();
        }

    }
}