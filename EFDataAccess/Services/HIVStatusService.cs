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

        public List<HIVStatus> findAllChildStatuses(Child child) { return HIVStatusRepository.GetAll().Where(x => x.ChildID == child.ChildID).ToList(); }

        public List<HIVStatus> findAllBeneficiaryStatuses(Beneficiary beneficiary) { return HIVStatusRepository.GetAll().Where(x => x.BeneficiaryID == beneficiary.BeneficiaryID).ToList(); }

        public List<HIVStatus> findAllAdultStatuses(Adult adult) { return HIVStatusRepository.GetAll().Where(x => x.AdultID == adult.AdultId).ToList(); }

        public HIVStatus findByGuid(Guid guid) { return HIVStatusRepository.GetAll().Where(h => h.HIVStatus_guid == guid).FirstOrDefault(); }

        public HIVStatus findByID(int hivstatusID) { return HIVStatusRepository.GetAll().Where(h => h.HIVStatusID == hivstatusID).FirstOrDefault(); }

        public HIVStatus findHIVStatusBySyncGuid(Guid guid) { return HIVStatusRepository.GetAll().Where(h => h.SyncGuid == guid).FirstOrDefault(); }

        public Adult findAdultBySyncGuid(Guid AdultGuid) { return UnitOfWork.Repository<Adult>().GetAll().Where(a => a.SyncGuid == AdultGuid).FirstOrDefault(); }

        public Child findChildBySyncGuid(Guid ChildGuid) { return UnitOfWork.Repository<Child>().GetAll().Where(a => a.SyncGuid == ChildGuid).FirstOrDefault(); }

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


        /*
         * #######################################################
         * ###### InitialRecordSummaryReport ChiefPartner  #######
         * #######################################################
         */

        public List<HIVStatusByPartnerReportDTO> getHIVStatusByChiefPartnerReport(DateTime initialDate, DateTime lastDate)
        {
            return getHIVStatusReport(initialDate, lastDate, "chiefpartner");
        }

        /*
         * #######################################################
         * ######### InitialRecordSummaryReport Partner ##########
         * #######################################################
         */

        public List<HIVStatusByPartnerReportDTO> getHIVStatusByPartnerReport(DateTime initialDate, DateTime lastDate)
        {
            return getHIVStatusReport(initialDate, lastDate, "partner");
        }

        /*
         * #####################################################################
         * #################### InitialRecordSummaryReport #####################
         * #####################################################################
         */

        public List<HIVStatusByPartnerReportDTO> getHIVStatusReport(DateTime initialDate, DateTime lastDate, string partnerType)
        {
            String query = @"SELECT
	                            p.ChiefPartner--<<ReplaceColumn<<--
	                            As Partner,
	                            --NOVOS CRIANÇAS
	                            ISNULL(new_child_hiv.HIV_P_IN_TARV, 0) As NewChildInTARV,
	                            ISNULL(new_child_hiv.HIV_P_NOT_TARV, 0) As NewChildNotInTARV,
	                            ISNULL(new_child_hiv.HIV_N, 0) As NewChildNegative,
	                            ISNULL(new_child_hiv.HIV_KNOWN_NREVEAL, 0) As NewChildNotRevealed,
	                            ISNULL(new_child_hiv.HIV_NOT_RECOMMENDED, 0) As NewChildNotRecommended,
	                            ISNULL(new_child_hiv.HIV_UNKNOWN, 0) As NewChildUnknown,
	                            --NOVOS ADULTOS
	                            ISNULL(new_adult_hiv.HIV_P_IN_TARV, 0) As NewAdultInTARV,
	                            ISNULL(new_adult_hiv.HIV_P_NOT_TARV, 0) As NewAdultNotInTARV,
	                            ISNULL(new_adult_hiv.HIV_N, 0) As NewAdultNegative,
	                            ISNULL(new_adult_hiv.HIV_KNOWN_NREVEAL, 0) As NewAdultNotRevealed,
	                            ISNULL(new_adult_hiv.HIV_UNKNOWN, 0) As NewAdultUnknown,
	                            --NOVOS CRIANÇAS GRADUADAS
	                            ISNULL(graduated_child_hiv.HIV_P_IN_TARV, 0) As  GraduatedChildInTARV,
	                            ISNULL(graduated_child_hiv.HIV_P_NOT_TARV, 0) As GraduatedChildNotInTARV,
	                            ISNULL(graduated_child_hiv.HIV_N, 0) As GraduatedChildNegative,
	                            ISNULL(graduated_child_hiv.HIV_KNOWN_NREVEAL, 0) As GraduatedChildNotRevealed,
	                            ISNULL(graduated_child_hiv.HIV_NOT_RECOMMENDED, 0) As GraduatedChildNotRecommended,
	                            ISNULL(graduated_child_hiv.HIV_UNKNOWN, 0) As GraduatedChildUnknown,
	                            --NOVOS ADULTOS GRADUADOS
	                            ISNULL(graduated_adult_hiv.HIV_P_IN_TARV, 0) As GraduatedAdultInTARV,
	                            ISNULL(graduated_adult_hiv.HIV_P_NOT_TARV, 0) As GraduatedAdultNotInTARV,
	                            ISNULL(graduated_adult_hiv.HIV_N, 0) As GraduatedAdultNegative,
	                            ISNULL(graduated_adult_hiv.HIV_KNOWN_NREVEAL, 0) As GraduatedAdultNotRevealed,
	                            ISNULL(graduated_adult_hiv.HIV_UNKNOWN, 0) As GraduatedAdultUnknown,
	                            --NOVOS CRIANÇAS EM ESTADO INICIAL
	                            ISNULL(initial_child_hiv.HIV_P_IN_TARV, 0) As InitialChildInTARV,
	                            ISNULL(initial_child_hiv.HIV_P_NOT_TARV, 0) As InitialChildNotInTARV,
	                            ISNULL(initial_child_hiv.HIV_N, 0) As InitialChildNegative,
	                            ISNULL(initial_child_hiv.HIV_KNOWN_NREVEAL, 0) As InitialChildNotRevealed,
	                            ISNULL(initial_child_hiv.HIV_NOT_RECOMMENDED, 0) As InitialChildNotRecommended,
	                            ISNULL(initial_child_hiv.HIV_UNKNOWN, 0) As InitialChildUnknown,
	                            --NOVOS ADULTOS EM ESTADO INICIAL
	                            ISNULL(initial_adult_hiv.HIV_P_IN_TARV, 0) As InitialAdultInTARV,
	                            ISNULL(initial_adult_hiv.HIV_P_NOT_TARV, 0) As InitialAdultNotInTARV,
	                            ISNULL(initial_adult_hiv.HIV_N, 0) As InitialAdultNegative,
	                            ISNULL(initial_adult_hiv.HIV_KNOWN_NREVEAL, 0) As InitialAdultNotRevealed,
	                            ISNULL(initial_adult_hiv.HIV_UNKNOWN, 0) As InitialAdultUnknown
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
                            LEFT JOIN
                            (
	                            SELECT
	                                hiv_obj.ChiefPartner--<<ReplaceColumn<<--
	                                ,
	                                SUM(hiv_obj.HIV_N) As HIV_N,
	                                SUM(hiv_obj.HIV_P_IN_TARV) As HIV_P_IN_TARV,
	                                SUM(hiv_obj.HIV_P_NOT_TARV) As HIV_P_NOT_TARV,
	                                SUM(hiv_obj.HIV_KNOWN_NREVEAL) As HIV_KNOWN_NREVEAL,
	                                SUM(hiv_obj.HIV_NOT_RECOMMENDED) As HIV_NOT_RECOMMENDED,
	                                SUM(hiv_obj.HIV_UNKNOWN) As HIV_UNKNOWN,
	                                SUM(hiv_obj.HIVTracked) As HIVTracked
	                            FROM
	                            (
		                            SELECT
			                            cp.Name As ChiefPartner, 
			                            ChiefPartnerRole = CASE cp.CollaboratorRoleID WHEN 3 THEN 'Supervior' WHEN 2 THEN 'Activista Chefe' ELSE 'Activista' END,
			                            p.Name As Partner, 
			                            PartnerRole = CASE p.CollaboratorRoleID WHEN 3 THEN 'Supervior' WHEN 2 THEN 'Activista Chefe' ELSE 'Activista' END,
			                            ben.FirstName As FirstName,
			                            ben.LastName As LastName,
			                            hs.HIV,
			                            HIV_N = CASE WHEN hs.HIV = 'N' THEN 1 ELSE 0 END,
			                            HIV_P_IN_TARV = CASE WHEN hs.HIV = 'P' AND hs.HIVInTreatment = 0 THEN 1 ELSE 0 END,
			                            HIV_P_NOT_TARV = CASE WHEN hs.HIV = 'P' AND hs.HIVInTreatment = 1 THEN 1 ELSE 0 END,
			                            HIV_KNOWN_NREVEAL = CASE WHEN hs.HIV = 'U' AND hs.HIVUndisclosedReason = 0 THEN 1 ELSE 0 END,
			                            HIV_NOT_RECOMMENDED = CASE WHEN hs.HIV = 'U' AND hs.HIVUndisclosedReason = 2 AND ben.HIVTracked = 1 THEN 1 ELSE 0 END,
			                            HIV_UNKNOWN = CASE WHEN hs.HIV = 'U' AND hs.HIVUndisclosedReason = 1 THEN 1 ELSE 0 END,
			                            HIVTracked = CASE WHEN ben.HIVTracked = 'TRUE' THEN 1 ELSE 0 END
		                            FROM [Partner] p
		                            inner join [Partner] cp ON (p.SuperiorId = cp.PartnerID) 
		                            inner join [HouseHold] hh ON (p.PartnerID = hh.PartnerID)
		                            inner join [Child] c on (hh.HouseHoldID = c.HouseholdID)
		                            --inner join [ChildStatusHistory] csh 
		                            --		ON (csh.ChildStatusHistoryID = (SELECT max(csh2.ChildStatusHistoryID) FROM  [ChildStatusHistory] csh2 WHERE csh2.ChildID = c.ChildID 
		                            --		AND (csh2.EffectiveDate BETWEEN @initialDate AND  @lastDate)))
		                            --inner join [ChildStatus] cs ON (csh.childStatusID = cs.StatusID AND cs.Description = 'Inicial')
		                            inner join [Beneficiary] ben ON ben.ID = c.ChildID AND type='child'
		                            inner join [HIVStatus] hs 
		                            ON (hs.HIVStatusID = (SELECT max(hs2.HIVStatusID) FROM [HIVStatus] hs2 WHERE hs2.BeneficiaryGuid = ben.Guid AND hs2.[InformationDate]<= @lastDate))
		                            AND ben.RegistrationDate --between @initialDate2 and @lastDate2
		                            BETWEEN 
		                            (
			                            SELECT GetFiscalInitialDate = 
			                            CASE WHEN MONTH(@initialDate) in (1,2,3,4,5,6,7,8) AND YEAR(@initialDate) = YEAR(@lastDate) THEN DATEFROMPARTS(YEAR(@initialDate)-1,09,21)
			                            WHEN MONTH(@initialDate) in (9,10,11,12) AND YEAR(@initialDate) = YEAR(@lastDate) THEN DATEFROMPARTS(YEAR(@initialDate),09,21) 
			                            WHEN MONTH(@initialDate) in (9,10,11,12) AND YEAR(@initialDate) <> YEAR(@lastDate) THEN DATEFROMPARTS(YEAR(@initialDate),09,21)
			                            --WHEN MONTH(@initialDate) in (1,2,3,4,5,6,7,8,9) AND YEAR(@initialDate) <> YEAR(@lastDate) THEN DATEFROMPARTS(YEAR(@initialDate),09,21) 
			                            END
		                            ) 
		                            AND @lastDate
		                            --Beneficiarios que tenham recebido mais de um serviço entre a data inicial e data final
		                            AND ben.Guid IN
		                            (
			                            SELECT rvs.Guid
			                            FROM  [Routine_Visit_Summary] rvs
			                            WHERE rvs.RoutineVisitDate BETWEEN @initialDate AND @lastDate AND rvs.BeneficiaryHasServices = 1
		                            )
		                            --Crianças que não tenham recebido mais de um serviço entre 21/09 até data inicial do intervalo
		                            AND ben.Guid not in
		                            ( 
			                            SELECT rvs.Guid
			                            FROM  [Routine_Visit_Summary] rvs
			                            WHERE rvs.RoutineVisitDate BETWEEN
			                            (
				                            SELECT GetFiscalInitialDate = 
				                            CASE WHEN MONTH(@initialDate) in (1,2,3,4,5,6,7,8) AND YEAR(@initialDate) = YEAR(@lastDate) THEN DATEFROMPARTS(YEAR(@initialDate)-1,09,21)
				                            WHEN MONTH(@initialDate) in (9,10,11,12) AND YEAR(@initialDate) = YEAR(@lastDate) THEN DATEFROMPARTS(YEAR(@initialDate),09,21) 
				                            WHEN MONTH(@initialDate) in (9,10,11,12) AND YEAR(@initialDate) <> YEAR(@lastDate) THEN DATEFROMPARTS(YEAR(@initialDate),09,21)
				                            END
			                            ) 
			                            AND @initialDate AND rvs.BeneficiaryHasServices = 1
		                            )
	                            ) hiv_obj
	                            group by 
		                            hiv_obj.ChiefPartner--<<ReplaceColumn<<--
                            )new_child_hiv
                            ON 
                            (
	                            p.ChiefPartner--<<ReplaceColumn<<--
	                            =
	                            new_child_hiv.ChiefPartner--<<ReplaceColumn<<--
                            )
                            LEFT JOIN
                            (
	                            SELECT
	                                hiv_obj.ChiefPartner--<<ReplaceColumn<<--
	                                ,
	                                SUM(hiv_obj.HIV_N) As HIV_N,
	                                SUM(hiv_obj.HIV_P_IN_TARV) As HIV_P_IN_TARV,
	                                SUM(hiv_obj.HIV_P_NOT_TARV) As HIV_P_NOT_TARV,
	                                SUM(hiv_obj.HIV_KNOWN_NREVEAL) As HIV_KNOWN_NREVEAL,
	                                SUM(hiv_obj.HIV_NOT_RECOMMENDED) As HIV_NOT_RECOMMENDED,
	                                SUM(hiv_obj.HIV_UNKNOWN) As HIV_UNKNOWN,
	                                SUM(hiv_obj.HIVTracked) As HIVTracked
	                            FROM
	                            (
		                            SELECT
			                            cp.Name As ChiefPartner, 
			                            ChiefPartnerRole = CASE cp.CollaboratorRoleID WHEN 3 THEN 'Supervior' WHEN 2 THEN 'Activista Chefe' ELSE 'Activista' END,
			                            p.Name As Partner, 
			                            PartnerRole = CASE p.CollaboratorRoleID WHEN 3 THEN 'Supervior' WHEN 2 THEN 'Activista Chefe' ELSE 'Activista' END,
			                            ben.FirstName As FirstName,
			                            ben.LastName As LastName,
			                            hs.HIV,
			                            HIV_N = CASE WHEN hs.HIV = 'N' THEN 1 ELSE 0 END,
			                            HIV_P_IN_TARV = CASE WHEN hs.HIV = 'P' AND hs.HIVInTreatment = 0 THEN 1 ELSE 0 END,
			                            HIV_P_NOT_TARV = CASE WHEN hs.HIV = 'P' AND hs.HIVInTreatment = 1 THEN 1 ELSE 0 END,
			                            HIV_KNOWN_NREVEAL = CASE WHEN hs.HIV = 'U' AND hs.HIVUndisclosedReason = 0 THEN 1 ELSE 0 END,
			                            HIV_NOT_RECOMMENDED = CASE WHEN hs.HIV = 'U' AND hs.HIVUndisclosedReason = 2 AND ben.HIVTracked = 1 THEN 1 ELSE 0 END,
			                            HIV_UNKNOWN = CASE WHEN hs.HIV = 'U' AND hs.HIVUndisclosedReason = 1 THEN 1 ELSE 0 END,
			                            HIVTracked = CASE WHEN ben.HIVTracked = 'TRUE' THEN 1 ELSE 0 END
		                            FROM 
				                            [Partner] p
		                            inner join [Partner] cp ON (p.SuperiorId = cp.PartnerID) 
		                            inner join [HouseHold] hh ON (p.PartnerID = hh.PartnerID)
		                            inner join  [Adult] a on (hh.HouseHoldID = a.HouseholdID)
		                            --inner join  [ChildStatusHistory] csh 
					                            --ON (csh.ChildStatusHistoryID = (SELECT max(csh2.ChildStatusHistoryID) FROM  [ChildStatusHistory] csh2 WHERE csh2.AdultID = a.AdultID 
					                            --AND (csh2.EffectiveDate BETWEEN @initialDate AND  @lastDate)))
		                            --inner join  [ChildStatus] cs ON (csh.childStatusID = cs.StatusID AND cs.Description = 'Inicial')
		                            inner join  [Beneficiary] ben ON ben.ID = a.AdultID AND type='adult'
		                            inner join [HIVStatus] hs 
		                            ON (hs.HIVStatusID = (SELECT max(hs2.HIVStatusID) FROM [HIVStatus] hs2 WHERE hs2.BeneficiaryGuid = ben.Guid AND hs2.[InformationDate]<= @lastDate))
		                            AND ben.RegistrationDate --between @initialDate2 and @lastDate2
		                            BETWEEN 
		                            (
			                            SELECT GetFiscalInitialDate = 
			                            CASE WHEN MONTH(@initialDate) in (1,2,3,4,5,6,7,8) AND YEAR(@initialDate) = YEAR(@lastDate) THEN DATEFROMPARTS(YEAR(@initialDate)-1,09,21)
			                            WHEN MONTH(@initialDate) in (9,10,11,12) AND YEAR(@initialDate) = YEAR(@lastDate) THEN DATEFROMPARTS(YEAR(@initialDate),09,21) 
			                            WHEN MONTH(@initialDate) in (9,10,11,12) AND YEAR(@initialDate) <> YEAR(@lastDate) THEN DATEFROMPARTS(YEAR(@initialDate),09,21)
			                            --WHEN MONTH(@initialDate) in (1,2,3,4,5,6,7,8,9) AND YEAR(@initialDate) <> YEAR(@lastDate) THEN DATEFROMPARTS(YEAR(@initialDate),09,21) 
			                            END
		                            ) 
		                            AND @lastDate
		                            --Beneficiarios que tenham recebido mais de um serviço entre a data inicial e data final
		                            AND ben.Guid IN
		                            (
			                            SELECT rvs.Guid
			                            FROM  [Routine_Visit_Summary] rvs
			                            WHERE rvs.RoutineVisitDate BETWEEN @initialDate AND @lastDate AND rvs.BeneficiaryHasServices = 1
		                            )
		                            --Crianças que não tenham recebido mais de um serviço entre 21/09 até data inicial do intervalo
		                            AND ben.Guid not in
		                            ( 
			                            SELECT rvs.Guid
			                            FROM  [Routine_Visit_Summary] rvs
			                            WHERE rvs.RoutineVisitDate BETWEEN
			                            (
				                            SELECT GetFiscalInitialDate = 
				                            CASE WHEN MONTH(@initialDate) in (1,2,3,4,5,6,7,8) AND YEAR(@initialDate) = YEAR(@lastDate) THEN DATEFROMPARTS(YEAR(@initialDate)-1,09,21)
				                            WHEN MONTH(@initialDate) in (9,10,11,12) AND YEAR(@initialDate) = YEAR(@lastDate) THEN DATEFROMPARTS(YEAR(@initialDate),09,21) 
				                            WHEN MONTH(@initialDate) in (9,10,11,12) AND YEAR(@initialDate) <> YEAR(@lastDate) THEN DATEFROMPARTS(YEAR(@initialDate),09,21)
				                            END
			                            ) 
			                            AND @initialDate AND rvs.BeneficiaryHasServices = 1
		                            )
	                            ) hiv_obj
	                            group by 
		                            hiv_obj.ChiefPartner--<<ReplaceColumn<<--
                            ) new_adult_hiv
                            ON 
                            (
	                            p.ChiefPartner--<<ReplaceColumn<<--
	                            =
	                            new_adult_hiv.ChiefPartner--<<ReplaceColumn<<--
                            )
                            LEFT JOIN
                            (
	                            SELECT
	                                hiv_obj.ChiefPartner--<<ReplaceColumn<<--
	                                ,
	                                SUM(hiv_obj.HIV_N) As HIV_N,
	                                SUM(hiv_obj.HIV_P_IN_TARV) As HIV_P_IN_TARV,
	                                SUM(hiv_obj.HIV_P_NOT_TARV) As HIV_P_NOT_TARV,
	                                SUM(hiv_obj.HIV_KNOWN_NREVEAL) As HIV_KNOWN_NREVEAL,
	                                SUM(hiv_obj.HIV_NOT_RECOMMENDED) As HIV_NOT_RECOMMENDED,
	                                SUM(hiv_obj.HIV_UNKNOWN) As HIV_UNKNOWN,
	                                SUM(hiv_obj.HIVTracked) As HIVTracked
	                            FROM
	                            (
		                            SELECT
			                            cp.Name As ChiefPartner, 
			                            ChiefPartnerRole = CASE cp.CollaboratorRoleID WHEN 3 THEN 'Supervior' WHEN 2 THEN 'Activista Chefe' ELSE 'Activista' END,
			                            p.Name As Partner, 
			                            PartnerRole = CASE p.CollaboratorRoleID WHEN 3 THEN 'Supervior' WHEN 2 THEN 'Activista Chefe' ELSE 'Activista' END,
			                            ben.FirstName As FirstName,
			                            ben.LastName As LastName,
			                            hs.HIV,
			                            HIV_N = CASE WHEN hs.HIV = 'N' THEN 1 ELSE 0 END,
			                            HIV_P_IN_TARV = CASE WHEN hs.HIV = 'P' AND hs.HIVInTreatment = 0 THEN 1 ELSE 0 END,
			                            HIV_P_NOT_TARV = CASE WHEN hs.HIV = 'P' AND hs.HIVInTreatment = 1 THEN 1 ELSE 0 END,
			                            HIV_KNOWN_NREVEAL = CASE WHEN hs.HIV = 'U' AND hs.HIVUndisclosedReason = 0 THEN 1 ELSE 0 END,
			                            HIV_NOT_RECOMMENDED = CASE WHEN hs.HIV = 'U' AND hs.HIVUndisclosedReason = 2 AND ben.HIVTracked = 1 THEN 1 ELSE 0 END,
			                            HIV_UNKNOWN = CASE WHEN hs.HIV = 'U' AND hs.HIVUndisclosedReason = 1 THEN 1 ELSE 0 END,
			                            HIVTracked = CASE WHEN ben.HIVTracked = 'TRUE' THEN 1 ELSE 0 END
		                            FROM [Partner] p
		                            inner join [Partner] cp ON (p.SuperiorId = cp.PartnerID) 
		                            inner join [HouseHold] hh ON (p.PartnerID = hh.PartnerID)
		                            inner join [Child] c on (hh.HouseHoldID = c.HouseholdID)
		                            inner join [ChildStatusHistory] csh 
				                            ON (csh.ChildStatusHistoryID = (SELECT max(csh2.ChildStatusHistoryID) FROM  [ChildStatusHistory] csh2 WHERE csh2.ChildID = c.ChildID 
				                            AND (csh2.EffectiveDate BETWEEN @initialDate AND  @lastDate)))
		                            inner join [ChildStatus] cs ON (csh.childStatusID = cs.StatusID AND cs.Description = 'Graduação')
		                            inner join [Beneficiary] ben ON ben.ID = c.ChildID AND type='child'
		                            inner join [HIVStatus] hs 
		                            ON (hs.HIVStatusID = (SELECT max(hs2.HIVStatusID) FROM [HIVStatus] hs2 WHERE hs2.BeneficiaryGuid = ben.Guid AND hs2.[InformationDate]<= @lastDate))
		                            AND ben.RegistrationDate --between @initialDate2 and @lastDate2
		                            BETWEEN 
		                            (
			                            SELECT GetFiscalInitialDate = 
			                            CASE WHEN MONTH(@initialDate) in (1,2,3,4,5,6,7,8) AND YEAR(@initialDate) = YEAR(@lastDate) THEN DATEFROMPARTS(YEAR(@initialDate)-1,09,21)
			                            WHEN MONTH(@initialDate) in (9,10,11,12) AND YEAR(@initialDate) = YEAR(@lastDate) THEN DATEFROMPARTS(YEAR(@initialDate),09,21) 
			                            WHEN MONTH(@initialDate) in (9,10,11,12) AND YEAR(@initialDate) <> YEAR(@lastDate) THEN DATEFROMPARTS(YEAR(@initialDate),09,21)
			                            --WHEN MONTH(@initialDate) in (1,2,3,4,5,6,7,8,9) AND YEAR(@initialDate) <> YEAR(@lastDate) THEN DATEFROMPARTS(YEAR(@initialDate),09,21) 
			                            END
		                            ) 
		                            AND @lastDate
		                            --Beneficiarios que tenham recebido mais de um serviço entre a data inicial e data final
		                            AND ben.Guid IN
		                            (
			                            SELECT rvs.Guid
			                            FROM  [Routine_Visit_Summary] rvs
			                            WHERE rvs.RoutineVisitDate BETWEEN @initialDate AND @lastDate AND rvs.BeneficiaryHasServices = 1
		                            )
		                            --Crianças que não tenham recebido mais de um serviço entre 21/09 até data inicial do intervalo
		                            AND ben.Guid not in
		                            ( 
			                            SELECT rvs.Guid
			                            FROM  [Routine_Visit_Summary] rvs
			                            WHERE rvs.RoutineVisitDate BETWEEN
			                            (
				                            SELECT GetFiscalInitialDate = 
				                            CASE WHEN MONTH(@initialDate) in (1,2,3,4,5,6,7,8) AND YEAR(@initialDate) = YEAR(@lastDate) THEN DATEFROMPARTS(YEAR(@initialDate)-1,09,21)
				                            WHEN MONTH(@initialDate) in (9,10,11,12) AND YEAR(@initialDate) = YEAR(@lastDate) THEN DATEFROMPARTS(YEAR(@initialDate),09,21) 
				                            WHEN MONTH(@initialDate) in (9,10,11,12) AND YEAR(@initialDate) <> YEAR(@lastDate) THEN DATEFROMPARTS(YEAR(@initialDate),09,21)
				                            END
			                            ) 
			                            AND @initialDate AND rvs.BeneficiaryHasServices = 1
		                            )
	                            ) hiv_obj
	                            group by 
		                            hiv_obj.ChiefPartner--<<ReplaceColumn<<--
                            )graduated_child_hiv
                            ON 
                            (
	                            p.ChiefPartner--<<ReplaceColumn<<--
	                            =
	                            graduated_child_hiv.ChiefPartner--<<ReplaceColumn<<--
                            )
                            LEFT JOIN
                            (
	                            SELECT
	                                hiv_obj.ChiefPartner--<<ReplaceColumn<<--
	                                ,
	                                SUM(hiv_obj.HIV_N) As HIV_N,
	                                SUM(hiv_obj.HIV_P_IN_TARV) As HIV_P_IN_TARV,
	                                SUM(hiv_obj.HIV_P_NOT_TARV) As HIV_P_NOT_TARV,
	                                SUM(hiv_obj.HIV_KNOWN_NREVEAL) As HIV_KNOWN_NREVEAL,
	                                SUM(hiv_obj.HIV_NOT_RECOMMENDED) As HIV_NOT_RECOMMENDED,
	                                SUM(hiv_obj.HIV_UNKNOWN) As HIV_UNKNOWN,
	                                SUM(hiv_obj.HIVTracked) As HIVTracked
	                            FROM
	                            (
		                            SELECT
			                            cp.Name As ChiefPartner, 
			                            ChiefPartnerRole = CASE cp.CollaboratorRoleID WHEN 3 THEN 'Supervior' WHEN 2 THEN 'Activista Chefe' ELSE 'Activista' END,
			                            p.Name As Partner, 
			                            PartnerRole = CASE p.CollaboratorRoleID WHEN 3 THEN 'Supervior' WHEN 2 THEN 'Activista Chefe' ELSE 'Activista' END,
			                            ben.FirstName As FirstName,
			                            ben.LastName As LastName,
			                            hs.HIV,
			                            HIV_N = CASE WHEN hs.HIV = 'N' THEN 1 ELSE 0 END,
			                            HIV_P_IN_TARV = CASE WHEN hs.HIV = 'P' AND hs.HIVInTreatment = 0 THEN 1 ELSE 0 END,
			                            HIV_P_NOT_TARV = CASE WHEN hs.HIV = 'P' AND hs.HIVInTreatment = 1 THEN 1 ELSE 0 END,
			                            HIV_KNOWN_NREVEAL = CASE WHEN hs.HIV = 'U' AND hs.HIVUndisclosedReason = 0 THEN 1 ELSE 0 END,
			                            HIV_NOT_RECOMMENDED = CASE WHEN hs.HIV = 'U' AND hs.HIVUndisclosedReason = 2 AND ben.HIVTracked = 1 THEN 1 ELSE 0 END,
			                            HIV_UNKNOWN = CASE WHEN hs.HIV = 'U' AND hs.HIVUndisclosedReason = 1 THEN 1 ELSE 0 END,
			                            HIVTracked = CASE WHEN ben.HIVTracked = 'TRUE' THEN 1 ELSE 0 END
		                            FROM 
				                            [Partner] p
		                            inner join [Partner] cp ON (p.SuperiorId = cp.PartnerID) 
		                            inner join [HouseHold] hh ON (p.PartnerID = hh.PartnerID)
		                            inner join  [Adult] a on (hh.HouseHoldID = a.HouseholdID)
		                            inner join  [ChildStatusHistory] csh 
					                            ON (csh.ChildStatusHistoryID = (SELECT max(csh2.ChildStatusHistoryID) FROM  [ChildStatusHistory] csh2 WHERE csh2.AdultID = a.AdultID 
					                            AND (csh2.EffectiveDate BETWEEN @initialDate AND  @lastDate)))
		                            inner join  [ChildStatus] cs ON (csh.childStatusID = cs.StatusID AND cs.Description = 'Graduação')
		                            inner join  [Beneficiary] ben ON ben.ID = a.AdultID AND type='adult'
		                            inner join [HIVStatus] hs 
		                            ON (hs.HIVStatusID = (SELECT max(hs2.HIVStatusID) FROM [HIVStatus] hs2 WHERE hs2.BeneficiaryGuid = ben.Guid AND hs2.[InformationDate]<= @lastDate))
		                            AND ben.RegistrationDate --between @initialDate2 and @lastDate2
		                            BETWEEN 
		                            (
			                            SELECT GetFiscalInitialDate = 
			                            CASE WHEN MONTH(@initialDate) in (1,2,3,4,5,6,7,8) AND YEAR(@initialDate) = YEAR(@lastDate) THEN DATEFROMPARTS(YEAR(@initialDate)-1,09,21)
			                            WHEN MONTH(@initialDate) in (9,10,11,12) AND YEAR(@initialDate) = YEAR(@lastDate) THEN DATEFROMPARTS(YEAR(@initialDate),09,21) 
			                            WHEN MONTH(@initialDate) in (9,10,11,12) AND YEAR(@initialDate) <> YEAR(@lastDate) THEN DATEFROMPARTS(YEAR(@initialDate),09,21)
			                            --WHEN MONTH(@initialDate) in (1,2,3,4,5,6,7,8,9) AND YEAR(@initialDate) <> YEAR(@lastDate) THEN DATEFROMPARTS(YEAR(@initialDate),09,21) 
			                            END
		                            ) 
		                            AND @lastDate
		                            --Beneficiarios que tenham recebido mais de um serviço entre a data inicial e data final
		                            AND ben.Guid IN
		                            (
			                            SELECT rvs.Guid
			                            FROM  [Routine_Visit_Summary] rvs
			                            WHERE rvs.RoutineVisitDate BETWEEN @initialDate AND @lastDate AND rvs.BeneficiaryHasServices = 1
		                            )
		                            --Crianças que não tenham recebido mais de um serviço entre 21/09 até data inicial do intervalo
		                            AND ben.Guid not in
		                            ( 
			                            SELECT rvs.Guid
			                            FROM  [Routine_Visit_Summary] rvs
			                            WHERE rvs.RoutineVisitDate BETWEEN
			                            (
				                            SELECT GetFiscalInitialDate = 
				                            CASE WHEN MONTH(@initialDate) in (1,2,3,4,5,6,7,8) AND YEAR(@initialDate) = YEAR(@lastDate) THEN DATEFROMPARTS(YEAR(@initialDate)-1,09,21)
				                            WHEN MONTH(@initialDate) in (9,10,11,12) AND YEAR(@initialDate) = YEAR(@lastDate) THEN DATEFROMPARTS(YEAR(@initialDate),09,21) 
				                            WHEN MONTH(@initialDate) in (9,10,11,12) AND YEAR(@initialDate) <> YEAR(@lastDate) THEN DATEFROMPARTS(YEAR(@initialDate),09,21)
				                            END
			                            ) 
			                            AND @initialDate AND rvs.BeneficiaryHasServices = 1
		                            )
	                            ) hiv_obj
	                            group by 
		                            hiv_obj.ChiefPartner--<<ReplaceColumn<<--
                            ) graduated_adult_hiv
                            ON 
                            (
	                            p.ChiefPartner--<<ReplaceColumn<<--
	                            =
	                            graduated_adult_hiv.ChiefPartner--<<ReplaceColumn<<--
                            )
                            LEFT JOIN
                            (
	                            SELECT
	                                hiv_obj.ChiefPartner--<<ReplaceColumn<<--
	                                ,
	                                SUM(hiv_obj.HIV_N) As HIV_N,
	                                SUM(hiv_obj.HIV_P_IN_TARV) As HIV_P_IN_TARV,
	                                SUM(hiv_obj.HIV_P_NOT_TARV) As HIV_P_NOT_TARV,
	                                SUM(hiv_obj.HIV_KNOWN_NREVEAL) As HIV_KNOWN_NREVEAL,
	                                SUM(hiv_obj.HIV_NOT_RECOMMENDED) As HIV_NOT_RECOMMENDED,
	                                SUM(hiv_obj.HIV_UNKNOWN) As HIV_UNKNOWN,
	                                SUM(hiv_obj.HIVTracked) As HIVTracked
	                            FROM
	                            (
		                            SELECT
			                            cp.Name As ChiefPartner, 
			                            ChiefPartnerRole = CASE cp.CollaboratorRoleID WHEN 3 THEN 'Supervior' WHEN 2 THEN 'Activista Chefe' ELSE 'Activista' END,
			                            p.Name As Partner, 
			                            PartnerRole = CASE p.CollaboratorRoleID WHEN 3 THEN 'Supervior' WHEN 2 THEN 'Activista Chefe' ELSE 'Activista' END,
			                            ben.FirstName As FirstName,
			                            ben.LastName As LastName,
			                            hs.HIV,
			                            HIV_N = CASE WHEN hs.HIV = 'N' THEN 1 ELSE 0 END,
			                            HIV_P_IN_TARV = CASE WHEN hs.HIV = 'P' AND hs.HIVInTreatment = 0 THEN 1 ELSE 0 END,
			                            HIV_P_NOT_TARV = CASE WHEN hs.HIV = 'P' AND hs.HIVInTreatment = 1 THEN 1 ELSE 0 END,
			                            HIV_KNOWN_NREVEAL = CASE WHEN hs.HIV = 'U' AND hs.HIVUndisclosedReason = 0 THEN 1 ELSE 0 END,
			                            HIV_NOT_RECOMMENDED = CASE WHEN hs.HIV = 'U' AND hs.HIVUndisclosedReason = 2 AND ben.HIVTracked = 1 THEN 1 ELSE 0 END,
			                            HIV_UNKNOWN = CASE WHEN hs.HIV = 'U' AND hs.HIVUndisclosedReason = 1 THEN 1 ELSE 0 END,
			                            HIVTracked = CASE WHEN ben.HIVTracked = 'TRUE' THEN 1 ELSE 0 END
		                            FROM [Partner] p
		                            inner join [Partner] cp ON (p.SuperiorId = cp.PartnerID) 
		                            inner join [HouseHold] hh ON (p.PartnerID = hh.PartnerID)
		                            inner join [Child] c on (hh.HouseHoldID = c.HouseholdID)
		                            inner join [ChildStatusHistory] csh 
				                            ON (csh.ChildStatusHistoryID = (SELECT max(csh2.ChildStatusHistoryID) FROM  [ChildStatusHistory] csh2 WHERE csh2.ChildID = c.ChildID 
				                            AND (csh2.EffectiveDate BETWEEN @initialDate AND  @lastDate)))
		                            inner join [ChildStatus] cs ON (csh.childStatusID = cs.StatusID AND cs.Description = 'Inicial')
		                            inner join [Beneficiary] ben ON ben.ID = c.ChildID AND type='child'
		                            inner join [HIVStatus] hs 
		                            ON (hs.HIVStatusID = (SELECT max(hs2.HIVStatusID) FROM [HIVStatus] hs2 WHERE hs2.BeneficiaryGuid = ben.Guid AND hs2.[InformationDate]<= @lastDate))
		                            AND ben.RegistrationDate --between @initialDate2 and @lastDate2
		                            BETWEEN 
		                            (
			                            SELECT GetFiscalInitialDate = 
			                            CASE WHEN MONTH(@initialDate) in (1,2,3,4,5,6,7,8) AND YEAR(@initialDate) = YEAR(@lastDate) THEN DATEFROMPARTS(YEAR(@initialDate)-1,09,21)
			                            WHEN MONTH(@initialDate) in (9,10,11,12) AND YEAR(@initialDate) = YEAR(@lastDate) THEN DATEFROMPARTS(YEAR(@initialDate),09,21) 
			                            WHEN MONTH(@initialDate) in (9,10,11,12) AND YEAR(@initialDate) <> YEAR(@lastDate) THEN DATEFROMPARTS(YEAR(@initialDate),09,21)
			                            --WHEN MONTH(@initialDate) in (1,2,3,4,5,6,7,8,9) AND YEAR(@initialDate) <> YEAR(@lastDate) THEN DATEFROMPARTS(YEAR(@initialDate),09,21) 
			                            END
		                            ) 
		                            AND @lastDate
		                            --Beneficiarios que tenham recebido mais de um serviço entre a data inicial e data final
		                            AND ben.Guid IN
		                            (
			                            SELECT rvs.Guid
			                            FROM  [Routine_Visit_Summary] rvs
			                            WHERE rvs.RoutineVisitDate BETWEEN @initialDate AND @lastDate AND rvs.BeneficiaryHasServices = 1
		                            )
		                            --Crianças que não tenham recebido mais de um serviço entre 21/09 até data inicial do intervalo
		                            AND ben.Guid not in
		                            ( 
			                            SELECT rvs.Guid
			                            FROM  [Routine_Visit_Summary] rvs
			                            WHERE rvs.RoutineVisitDate BETWEEN
			                            (
				                            SELECT GetFiscalInitialDate = 
				                            CASE WHEN MONTH(@initialDate) in (1,2,3,4,5,6,7,8) AND YEAR(@initialDate) = YEAR(@lastDate) THEN DATEFROMPARTS(YEAR(@initialDate)-1,09,21)
				                            WHEN MONTH(@initialDate) in (9,10,11,12) AND YEAR(@initialDate) = YEAR(@lastDate) THEN DATEFROMPARTS(YEAR(@initialDate),09,21) 
				                            WHEN MONTH(@initialDate) in (9,10,11,12) AND YEAR(@initialDate) <> YEAR(@lastDate) THEN DATEFROMPARTS(YEAR(@initialDate),09,21)
				                            END
			                            ) 
			                            AND @initialDate AND rvs.BeneficiaryHasServices = 1
		                            )
	                            ) hiv_obj
	                            group by 
		                            hiv_obj.ChiefPartner--<<ReplaceColumn<<--
                            )initial_child_hiv
                            ON 
                            (
	                            p.ChiefPartner--<<ReplaceColumn<<--
	                            =
	                            initial_child_hiv.ChiefPartner--<<ReplaceColumn<<--
                            )
                            LEFT JOIN
                            (
	                            SELECT
	                                hiv_obj.ChiefPartner--<<ReplaceColumn<<--
	                                ,
	                                SUM(hiv_obj.HIV_N) As HIV_N,
	                                SUM(hiv_obj.HIV_P_IN_TARV) As HIV_P_IN_TARV,
	                                SUM(hiv_obj.HIV_P_NOT_TARV) As HIV_P_NOT_TARV,
	                                SUM(hiv_obj.HIV_KNOWN_NREVEAL) As HIV_KNOWN_NREVEAL,
	                                SUM(hiv_obj.HIV_NOT_RECOMMENDED) As HIV_NOT_RECOMMENDED,
	                                SUM(hiv_obj.HIV_UNKNOWN) As HIV_UNKNOWN,
	                                SUM(hiv_obj.HIVTracked) As HIVTracked
	                            FROM
	                            (
		                            SELECT
			                            cp.Name As ChiefPartner, 
			                            ChiefPartnerRole = CASE cp.CollaboratorRoleID WHEN 3 THEN 'Supervior' WHEN 2 THEN 'Activista Chefe' ELSE 'Activista' END,
			                            p.Name As Partner, 
			                            PartnerRole = CASE p.CollaboratorRoleID WHEN 3 THEN 'Supervior' WHEN 2 THEN 'Activista Chefe' ELSE 'Activista' END,
			                            ben.FirstName As FirstName,
			                            ben.LastName As LastName,
			                            hs.HIV,
			                            HIV_N = CASE WHEN hs.HIV = 'N' THEN 1 ELSE 0 END,
			                            HIV_P_IN_TARV = CASE WHEN hs.HIV = 'P' AND hs.HIVInTreatment = 0 THEN 1 ELSE 0 END,
			                            HIV_P_NOT_TARV = CASE WHEN hs.HIV = 'P' AND hs.HIVInTreatment = 1 THEN 1 ELSE 0 END,
			                            HIV_KNOWN_NREVEAL = CASE WHEN hs.HIV = 'U' AND hs.HIVUndisclosedReason = 0 THEN 1 ELSE 0 END,
			                            HIV_NOT_RECOMMENDED = CASE WHEN hs.HIV = 'U' AND hs.HIVUndisclosedReason = 2 AND ben.HIVTracked = 1 THEN 1 ELSE 0 END,
			                            HIV_UNKNOWN = CASE WHEN hs.HIV = 'U' AND hs.HIVUndisclosedReason = 1 THEN 1 ELSE 0 END,
			                            HIVTracked = CASE WHEN ben.HIVTracked = 'TRUE' THEN 1 ELSE 0 END
		                            FROM 
				                            [Partner] p
		                            inner join [Partner] cp ON (p.SuperiorId = cp.PartnerID) 
		                            inner join [HouseHold] hh ON (p.PartnerID = hh.PartnerID)
		                            inner join  [Adult] a on (hh.HouseHoldID = a.HouseholdID)
		                            inner join  [ChildStatusHistory] csh 
					                            ON (csh.ChildStatusHistoryID = (SELECT max(csh2.ChildStatusHistoryID) FROM  [ChildStatusHistory] csh2 WHERE csh2.AdultID = a.AdultID 
					                            AND (csh2.EffectiveDate BETWEEN @initialDate AND  @lastDate)))
		                            inner join  [ChildStatus] cs ON (csh.childStatusID = cs.StatusID AND cs.Description = 'Inicial')
		                            inner join  [Beneficiary] ben ON ben.ID = a.AdultID AND type='adult'
		                            inner join [HIVStatus] hs 
		                            ON (hs.HIVStatusID = (SELECT max(hs2.HIVStatusID) FROM [HIVStatus] hs2 WHERE hs2.BeneficiaryGuid = ben.Guid AND hs2.[InformationDate]<= @lastDate))
		                            AND ben.RegistrationDate --between @initialDate2 and @lastDate2
		                            BETWEEN 
		                            (
			                            SELECT GetFiscalInitialDate = 
			                            CASE WHEN MONTH(@initialDate) in (1,2,3,4,5,6,7,8) AND YEAR(@initialDate) = YEAR(@lastDate) THEN DATEFROMPARTS(YEAR(@initialDate)-1,09,21)
			                            WHEN MONTH(@initialDate) in (9,10,11,12) AND YEAR(@initialDate) = YEAR(@lastDate) THEN DATEFROMPARTS(YEAR(@initialDate),09,21) 
			                            WHEN MONTH(@initialDate) in (9,10,11,12) AND YEAR(@initialDate) <> YEAR(@lastDate) THEN DATEFROMPARTS(YEAR(@initialDate),09,21)
			                            --WHEN MONTH(@initialDate) in (1,2,3,4,5,6,7,8,9) AND YEAR(@initialDate) <> YEAR(@lastDate) THEN DATEFROMPARTS(YEAR(@initialDate),09,21) 
			                            END
		                            ) 
		                            AND @lastDate
		                            --Beneficiarios que tenham recebido mais de um serviço entre a data inicial e data final
		                            AND ben.Guid IN
		                            (
			                            SELECT rvs.Guid
			                            FROM  [Routine_Visit_Summary] rvs
			                            WHERE rvs.RoutineVisitDate BETWEEN @initialDate AND @lastDate AND rvs.BeneficiaryHasServices = 1
		                            )
		                            --Crianças que não tenham recebido mais de um serviço entre 21/09 até data inicial do intervalo
		                            AND ben.Guid not in
		                            ( 
			                            SELECT rvs.Guid
			                            FROM  [Routine_Visit_Summary] rvs
			                            WHERE rvs.RoutineVisitDate BETWEEN
			                            (
				                            SELECT GetFiscalInitialDate = 
				                            CASE WHEN MONTH(@initialDate) in (1,2,3,4,5,6,7,8) AND YEAR(@initialDate) = YEAR(@lastDate) THEN DATEFROMPARTS(YEAR(@initialDate)-1,09,21)
				                            WHEN MONTH(@initialDate) in (9,10,11,12) AND YEAR(@initialDate) = YEAR(@lastDate) THEN DATEFROMPARTS(YEAR(@initialDate),09,21) 
				                            WHEN MONTH(@initialDate) in (9,10,11,12) AND YEAR(@initialDate) <> YEAR(@lastDate) THEN DATEFROMPARTS(YEAR(@initialDate),09,21)
				                            END
			                            ) 
			                            AND @initialDate AND rvs.BeneficiaryHasServices = 1
		                            )
	                            ) hiv_obj
	                            group by 
		                            hiv_obj.ChiefPartner--<<ReplaceColumn<<--
                            ) initial_adult_hiv
                            ON 
                            (
	                            p.ChiefPartner--<<ReplaceColumn<<--
	                            =
	                            initial_adult_hiv.ChiefPartner--<<ReplaceColumn<<--
                            )";

            query = (partnerType.Equals("partner")) ? query.Replace("ChiefPartner--<<ReplaceColumn<<--", "Partner") : query;

            return UnitOfWork.DbContext.Database.SqlQuery<HIVStatusByPartnerReportDTO>(query,
                    new SqlParameter("initialDate", initialDate),
                    new SqlParameter("lastDate", lastDate)).ToList();
        }
    }
}
