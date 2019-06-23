using EFDataAccess.UOW;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using EFDataAccess.DTO;

namespace EFDataAccess.Services
{
    public class ReportService : BaseService
    {
        public ReportService(UnitOfWork uow) : base(uow) { }

        public List<SeriesData> getData(String sqlQuery)
        {
            return UnitOfWork.DbContext.Database.SqlQuery<SeriesData>(sqlQuery).ToList();
        }

        public List<SeriesDecimalData> getDecimalData(String sqlQuery)
        {
            return UnitOfWork.DbContext.Database.SqlQuery<SeriesDecimalData>(sqlQuery).ToList();
        }

        public List<SeriesDataComulative> getDataComulative(String sqlQuery)
        {
            return UnitOfWork.DbContext.Database.SqlQuery<SeriesDataComulative>(sqlQuery).ToList();
        }

        public List<SeriesDataComulative2> getDataComulative2(String sqlQuery)
        {
            return UnitOfWork.DbContext.Database.SqlQuery<SeriesDataComulative2>(sqlQuery).ToList();
        }

        public List<SeriesDataChildsTotalInitialGraduatedAdultsNonGraduated> getDataChildsTMG(String sqlQuery)
        {
            return UnitOfWork.DbContext.Database.SqlQuery<SeriesDataChildsTotalInitialGraduatedAdultsNonGraduated>(sqlQuery).ToList();
        }

        public List<SeriesDataBeneficiaryStates> getDataBeneficiaryStates(String sqlQuery, DateTime initialDate, DateTime finalDate)
        {
            return GetResultsFilteredByDatesInterval(sqlQuery, initialDate, finalDate)
                    .ToList();
        }

        public List<SeriesDecimalData> GetDataGetCHASSandHealthFacilityFamilyOriginReferencesByOCB(String sqlQuery, DateTime initialDate, DateTime finalDate)
        {
            return GetResultsFilteredByDatesIntervalSeriesDecimalData(sqlQuery, initialDate, finalDate)
                    .ToList();
        }

        public List<SeriesData> GetDateBeneficiariesBySex(String sqlQuery, DateTime initialDate, DateTime finalDate)
        {
            return GetResultsFilteredByDatesIntervalSeriesData(sqlQuery, initialDate, finalDate)
                    .ToList();
        }

        public List<SeriesDataComulative> GetDateBeneficiariesByAge(String sqlQuery, DateTime initialDate, DateTime finalDate)
        {
            return GetResultsFilteredByDatesIntervalSeriesDataComulative(sqlQuery, initialDate, finalDate)
                    .ToList();
        }

        public List<SeriesDataComulative> GetDateChildsFrom0to5WhoReceivedDPI(String sqlQuery, DateTime initialDate, DateTime finalDate)
        {
            return GetResultsFilteredByDatesIntervalSeriesDataComulative(sqlQuery, initialDate, finalDate)
                    .ToList();
        }

        public List<SeriesData> GetDateHIVCascade(String sqlQuery, DateTime initialDate, DateTime finalDate)
        {
            return GetResultsFilteredByDatesIntervalSeriesData(sqlQuery, initialDate, finalDate)
                    .ToList();
        }

        private System.Data.Entity.Infrastructure.DbRawSqlQuery<SeriesDataBeneficiaryStates> GetResultsFilteredByDatesInterval(string sqlQuery, DateTime initialDate, DateTime finalDate)
        {
            return UnitOfWork.DbContext.Database.SqlQuery<SeriesDataBeneficiaryStates>
                (sqlQuery, new SqlParameter("initialDate", initialDate), new SqlParameter("lastDate", finalDate));
        }

        private System.Data.Entity.Infrastructure.DbRawSqlQuery<SeriesDecimalData> GetResultsFilteredByDatesIntervalSeriesDecimalData(string sqlQuery, DateTime initialDate, DateTime finalDate)
        {
            return UnitOfWork.DbContext.Database.SqlQuery<SeriesDecimalData>
                (sqlQuery, new SqlParameter("initialDate", initialDate), new SqlParameter("lastDate", finalDate));
        }

        private System.Data.Entity.Infrastructure.DbRawSqlQuery<SeriesData> GetResultsFilteredByDatesIntervalSeriesData(string sqlQuery, DateTime initialDate, DateTime finalDate)
        {
            return UnitOfWork.DbContext.Database.SqlQuery<SeriesData>
                (sqlQuery, new SqlParameter("initialDate", initialDate), new SqlParameter("lastDate", finalDate));
        }

        private System.Data.Entity.Infrastructure.DbRawSqlQuery<SeriesDataComulative> GetResultsFilteredByDatesIntervalSeriesDataComulative(string sqlQuery, DateTime initialDate, DateTime finalDate)
        {
            return UnitOfWork.DbContext.Database.SqlQuery<SeriesDataComulative>
                (sqlQuery, new SqlParameter("initialDate", initialDate), new SqlParameter("lastDate", finalDate));
        }
        /*
         * #######################################################
         * ###### InitialRecordSummaryReport ChiefPartner  #######
         * #######################################################
         */

        public List<InitialRecordSummaryReportDTO> getInitialRecordSummaryChiefPartner(DateTime initialDate, DateTime lastDate)
        {
            return getInitialRecordSummary(initialDate, lastDate, "chiefpartner");
        }

        /*
         * #######################################################
         * ######### InitialRecordSummaryReport Partner ##########
         * #######################################################
         */

        public List<InitialRecordSummaryReportDTO> getInitialRecordSummaryPartner(DateTime initialDate, DateTime lastDate)
        {
            return getInitialRecordSummary(initialDate, lastDate, "partner");
        }

        /*
         * #####################################################################
         * #################### InitialRecordSummaryReport #####################
         * #####################################################################
         */

        public List<InitialRecordSummaryReportDTO> getInitialRecordSummary(DateTime initialDate, DateTime lastDate, string partnerType)
        {
            String query = @"SELECT
	                            p.ChiefPartner--<<ReplaceColumn<<--
	                            As Partner,
	                            ISNULL(age_obj.btw_x_1_M, 0) As btw_x_1_M,
	                            ISNULL(age_obj.btw_1_4_M, 0) As btw_1_4_M,
	                            ISNULL(age_obj.btw_5_9_M, 0) As btw_5_9_M,
	                            ISNULL(age_obj.btw_10_14_M, 0) As btw_10_14_M,
	                            ISNULL(age_obj.btw_15_17_M, 0) As btw_15_17_M,
	                            ISNULL(age_obj.btw_18_24_M, 0) As btw_18_24_M,
	                            ISNULL(age_obj.btw_25_x_M, 0) As btw_25_x_M,
	                            ISNULL(age_obj.btw_x_1_F, 0) As btw_x_1_F,
	                            ISNULL(age_obj.btw_1_4_F, 0) As btw_1_4_F,
	                            ISNULL(age_obj.btw_5_9_F, 0) As btw_5_9_F,
	                            ISNULL(age_obj.btw_10_14_F, 0) As btw_10_14_F,
	                            ISNULL(age_obj.btw_15_17_F, 0) As btw_15_17_F,
	                            ISNULL(age_obj.btw_18_24_F, 0) As btw_18_24_F,
	                            ISNULL(age_obj.btw_25_x_F, 0) As btw_25_x_F,
	                            ISNULL(age_obj.ovc_father, 0) As ovc_father,
	                            ISNULL(age_obj.ovc_mother, 0) As ovc_mother,
	                            ISNULL(age_obj.ovc_both, 0) As ovc_both,
	                            ISNULL(age_obj.IsPartSavingGroup, 0) As IsPartSavingGroup,
	                            ISNULL(hiv_obj.HIV_N, 0) As HIV_N,
	                            ISNULL(hiv_obj.HIV_P_IN_TARV, 0) As HIV_P_IN_TARV,
	                            ISNULL(hiv_obj.HIV_P_NOT_TARV, 0) As HIV_P_NOT_TARV,
	                            ISNULL(hiv_obj.HIV_KNOWN_NREVEAL, 0) As HIV_KNOWN_NREVEAL,
	                            ISNULL(hiv_obj.HIV_NOT_RECOMMENDED, 0) As HIV_NOT_RECOMMENDED,
	                            ISNULL(hiv_obj.HIV_UNKNOWN, 0) As HIV_UNKNOWN,
	                            ISNULL(hiv_obj.HIVTracked, 0) As HIVTracked,
	                            ISNULL(ref_origin_obj.US, 0) As US,
	                            ISNULL(ref_origin_obj.Com, 0) As Com,
	                            ISNULL(ref_origin_obj.Parceiro_Clínico, 0) As Parceiro_Clínico,
	                            ISNULL(ref_origin_obj.OCBS, 0) As OCBS,
	                            ISNULL(ref_origin_obj.Others, 0) As Others
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
	                            SELECT  --  Total de beneficiarios dos activistas (Por Idade), agrupaddo pelo activista chefe
		                            age_obj.ChiefPartner--<<ReplaceColumn<<--
		                            ,
		                            SUM(age_obj.btw_x_1_M) As btw_x_1_M,
		                            SUM(age_obj.btw_1_4_M) As btw_1_4_M,
		                            SUM(age_obj.btw_5_9_M) As btw_5_9_M,
		                            SUM(age_obj.btw_10_14_M) As btw_10_14_M,
		                            SUM(age_obj.btw_15_17_M) As btw_15_17_M,
		                            SUM(age_obj.btw_18_24_M) As btw_18_24_M,
		                            SUM(age_obj.btw_25_x_M) As btw_25_x_M,
		                            SUM(age_obj.btw_x_1_F) As btw_x_1_F,
		                            SUM(age_obj.btw_1_4_F) As btw_1_4_F,
		                            SUM(age_obj.btw_5_9_F) As btw_5_9_F,
		                            SUM(age_obj.btw_10_14_F) As btw_10_14_F,
		                            SUM(age_obj.btw_15_17_F) As btw_15_17_F,
		                            SUM(age_obj.btw_18_24_F) As btw_18_24_F,
		                            SUM(age_obj.btw_25_x_F) As btw_25_x_F,
		                            SUM(age_obj.ovc_father) As ovc_father,
		                            SUM(age_obj.ovc_mother) As ovc_mother,
		                            SUM(age_obj.ovc_both) As ovc_both,
		                            SUM(age_obj.IsPartSavingGroup) As IsPartSavingGroup
	                            FROM
	                            (
		                            SELECT
			                            cp.Name As ChiefPartner, 
			                            ChiefPartnerRole = CASE cp.CollaboratorRoleID WHEN 3 THEN 'Supervior' WHEN 2 THEN 'Activista Chefe' ELSE 'Activista' END,
			                            p.[Name] As [Partner], 
			                            PartnerRole = CASE p.CollaboratorRoleID WHEN 3 THEN 'Supervior' WHEN 2 THEN 'Activista Chefe' ELSE 'Activista' END,
			                            ben.FirstName As FirstName,
			                            ben.LastName As LastName,
			                            DATEDIFF(year, CAST(ben.DateOfBirth As Date), @lastDate) As Age,
			                            btw_x_1_M = CASE WHEN DATEDIFF(month, CAST(ben.DateOfBirth As Date), @lastDate) < 12 AND ben.Gender = 'M' THEN 1 ELSE 0 END,
			                            btw_1_4_M = CASE WHEN DATEDIFF(month, CAST(ben.DateOfBirth As Date), @lastDate) BETWEEN 12 AND 59 AND ben.Gender = 'M' THEN 1 ELSE 0 END,
			                            btw_5_9_M = CASE WHEN DATEDIFF(month, CAST(ben.DateOfBirth As Date), @lastDate) BETWEEN 60 AND 119 AND ben.Gender = 'M' THEN 1 ELSE 0 END,
			                            btw_10_14_M = CASE WHEN DATEDIFF(month, CAST(ben.DateOfBirth As Date), @lastDate) BETWEEN 120 AND 179 AND ben.Gender = 'M' THEN 1 ELSE 0 END,
			                            btw_15_17_M = CASE WHEN DATEDIFF(month, CAST(ben.DateOfBirth As Date), @lastDate) BETWEEN 180 AND 215 AND ben.Gender = 'M' THEN 1 ELSE 0 END,
			                            btw_18_24_M = CASE WHEN DATEDIFF(month, CAST(ben.DateOfBirth As Date), @lastDate) BETWEEN 216 AND 299 AND ben.Gender = 'M' THEN 1 ELSE 0 END,
			                            btw_25_x_M = CASE WHEN DATEDIFF(month, CAST(ben.DateOfBirth As Date), @lastDate) >= 300 AND ben.Gender = 'M' THEN 1 ELSE 0 END,
			                            btw_x_1_F = CASE WHEN DATEDIFF(month, CAST(ben.DateOfBirth As Date), @lastDate) < 12 AND ben.Gender = 'F' THEN 1 ELSE 0 END,
			                            btw_1_4_F = CASE WHEN DATEDIFF(month, CAST(ben.DateOfBirth As Date), @lastDate) BETWEEN 12 AND 59 AND ben.Gender = 'F' THEN 1 ELSE 0 END,
			                            btw_5_9_F = CASE WHEN DATEDIFF(month, CAST(ben.DateOfBirth As Date), @lastDate) BETWEEN 60 AND 119 AND ben.Gender = 'F' THEN 1 ELSE 0 END,
			                            btw_10_14_F = CASE WHEN DATEDIFF(month, CAST(ben.DateOfBirth As Date), @lastDate) BETWEEN 120 AND 179 AND ben.Gender = 'F' THEN 1 ELSE 0 END,
			                            btw_15_17_F = CASE WHEN DATEDIFF(month, CAST(ben.DateOfBirth As Date), @lastDate) BETWEEN 180 AND 215 AND ben.Gender = 'F' THEN 1 ELSE 0 END,
			                            btw_18_24_F = CASE WHEN DATEDIFF(month, CAST(ben.DateOfBirth As Date), @lastDate) BETWEEN 216 AND 299 AND ben.Gender = 'F' THEN 1 ELSE 0 END,
			                            btw_25_x_F = CASE WHEN DATEDIFF(month, CAST(ben.DateOfBirth As Date), @lastDate) >= 300 AND ben.Gender = 'F' THEN 1 ELSE 0 END,
			                            ovc_father = CASE WHEN ovc.Description = 'Orfão de Pai' THEN 1 ELSE 0 END,
			                            ovc_mother = CASE WHEN ovc.Description = 'Orfão de mãe' THEN 1 ELSE 0 END,
			                            ovc_both = CASE WHEN ovc.Description = 'Orfão de ambos' THEN 1 ELSE 0 END,
			                            IsPartSavingGroup = CASE WHEN DATEDIFF(year, CAST(ben.DateOfBirth As Date), @lastDate) >=12 AND ben.IsPartSavingGroup = 1 THEN 1 ELSE 0 END
		                            FROM 
			                             [Partner] p
		                            inner join  [Partner] cp ON (p.SuperiorId = cp.PartnerID) 
		                            inner join  [HouseHold] hh ON (p.PartnerID = hh.PartnerID)
		                            inner join  [Beneficiary] ben ON (ben.HouseholdID = hh.HouseHoldID)
		                            --inner join  [ChildStatusHistory] csh 
		                            --ON (csh.ChildStatusHistoryID = (SELECT max(csh2.ChildStatusHistoryID) FROM  [ChildStatusHistory] csh2 WHERE csh2.BeneficiaryGuid = ben.Guid AND csh2.EffectiveDate <= @lastDate))
		                            --inner join  [ChildStatus] cs ON (csh.childStatusID = cs.StatusID AND cs.Description in ('Inicial')) 
		                            inner join  [HIVStatus] hs 
		                            ON (hs.HIVStatusID = (SELECT max(hs2.HIVStatusID) FROM  [HIVStatus] hs2 WHERE hs2.BeneficiaryGuid = ben.Guid AND hs2.[InformationDate]<= @lastDate))
		                            left join  [OVCType] ovc ON (ovc.OVCTypeID = ben.OVCTypeID)
		                            Where p.CollaboratorRoleID = 1
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
		                            --Beneficiarios que não tenham recebido mais de um serviço entre 21/09 até data inicial do intervalo
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

	                            ) age_obj
	                            group by 
		                            age_obj.ChiefPartner--<<ReplaceColumn<<--
                            ) age_obj 
                            on 
                            (
	                            p.ChiefPartner--<<ReplaceColumn<<--
	                            =
	                            age_obj.ChiefPartner--<<ReplaceColumn<<--
                            ) 
                            left join
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
		                            inner join  [Partner] cp ON (p.SuperiorId = cp.PartnerID) 
		                            inner join  [HouseHold] hh ON (p.PartnerID = hh.PartnerID)
		                            inner join  [Beneficiary] ben ON (ben.HouseholdID = hh.HouseHoldID AND type='child')
		                            --inner join  [ChildStatusHistory] csh 
		                            --ON (csh.ChildStatusHistoryID = (SELECT max(csh2.ChildStatusHistoryID) FROM  [ChildStatusHistory] csh2 WHERE csh2.BeneficiaryGuid = ben.Guid AND csh2.EffectiveDate<= @lastDate))
		                            --inner join  [ChildStatus] cs ON (csh.childStatusID = cs.StatusID AND cs.Description in ('Inicial')) 
		                            inner join  [HIVStatus] hs 
		                            ON (hs.HIVStatusID = (SELECT max(hs2.HIVStatusID) FROM  [HIVStatus] hs2 WHERE hs2.BeneficiaryGuid = ben.Guid AND hs2.[InformationDate]<= @lastDate))
		                            Where p.CollaboratorRoleID = 1 
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
		                            --Crianças que tenham recebido mais de um serviço entre a data inicial e data final
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
                            ) hiv_obj 
                            on 
                            (
	                            p.ChiefPartner--<<ReplaceColumn<<--
	                            =
	                            hiv_obj.ChiefPartner--<<ReplaceColumn<<--
                            )
                            left join
                            (
	                            SELECT  --  Total de familias de acordo com a referencia
		                            ref_origin_obj.ChiefPartner--<<ReplaceColumn<<--
		                            ,
		                            SUM(ref_origin_obj.US) As US,
		                            SUM(ref_origin_obj.Com) As Com,
		                            SUM(ref_origin_obj.Parceiro_Clínico) As Parceiro_Clínico,
		                            SUM(ref_origin_obj.OCBS) As OCBS,
		                            SUM(ref_origin_obj.Others) As Others
	                            FROM
	                            (
		                            SELECT
			                            cp.Name As ChiefPartner, 
			                            ChiefPartnerRole = CASE cp.CollaboratorRoleID WHEN 3 THEN 'Supervior' WHEN 2 THEN 'Activista Chefe' ELSE 'Activista' END,
			                            p.Name As Partner, 
			                            PartnerRole = CASE p.CollaboratorRoleID WHEN 3 THEN 'Supervior' WHEN 2 THEN 'Activista Chefe' ELSE 'Activista' END,
			                            hh.HouseholdName,
			                            ben.Guid, ben.FirstName, ben.LastName,
			                            US = CASE WHEN se.Description = 'Unidade Sanitária' THEN 1 ELSE 0 END,
			                            Com = CASE WHEN se.Description = 'Comunidade' THEN 1 ELSE 0 END,
			                            Parceiro_Clínico = CASE WHEN se.Description = 'Parceiro Clínico' THEN 1 ELSE 0 END,
			                            OCBS = CASE WHEN se.Description = 'Nenhuma' THEN 1 ELSE 0 END,
			                            Others = CASE WHEN se.Description = 'Outra' THEN 1 ELSE 0 END
		                            FROM 
			                             [Partner] p
		                            inner join  [Partner] cp on (p.SuperiorId = cp.PartnerID) 
		                            inner join  [HouseHold] hh on (p.PartnerID = hh.PartnerID)
		                            inner join  [SimpleEntity] se on (se.SimpleEntityID = hh.FamilyOriginRefID)
		                            inner join  [Beneficiary] ben on (ben.HouseholdID = hh.HouseHoldID)
		                            --inner join  [ChildStatusHistory] csh 
		                            --ON (csh.ChildStatusHistoryID = (SELECT max(csh2.ChildStatusHistoryID) FROM  [ChildStatusHistory] csh2 WHERE csh2.BeneficiaryGuid = b.Guid AND csh2.EffectiveDate<= @lastDate))
		                            --inner join  [ChildStatus] cs ON (csh.childStatusID = cs.StatusID AND cs.Description in ('Inicial')) 
		                            inner join  [HIVStatus] hs 
		                            ON (hs.HIVStatusID = (SELECT max(hs2.HIVStatusID) FROM  [HIVStatus] hs2 WHERE hs2.BeneficiaryGuid = ben.Guid AND hs2.[InformationDate]<= @lastDate))
		                            Where p.CollaboratorRoleID = 1
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
	                            ) ref_origin_obj
	                            group by 
		                            ref_origin_obj.ChiefPartner--<<ReplaceColumn<<--
                            ) ref_origin_obj 
                            on 
                            (
	                            p.ChiefPartner--<<ReplaceColumn<<--
	                            =
	                            ref_origin_obj.ChiefPartner--<<ReplaceColumn<<--
                            )";

            query = (partnerType.Equals("partner")) ? query.Replace("ChiefPartner--<<ReplaceColumn<<--", "Partner") : query;

            return UnitOfWork.DbContext.Database.SqlQuery<InitialRecordSummaryReportDTO>(query,
                    new SqlParameter("initialDate", initialDate),
                    new SqlParameter("lastDate", lastDate)).ToList();
        }

        /*
        * #######################################################
        * ###### RoutineVisitSummaryReport ChiefPartner  ########
        * #######################################################
        */

        public List<RoutineVisitSummaryReportDTO> getRoutineVisitSummaryChiefPartner(DateTime initialDate, DateTime lastDate)
        {
            return getRoutineVisitSummary(initialDate, lastDate, "chiefpartner");
        }

        /*
         * #######################################################
         * ########## RoutineVisitSummaryReport Partner ##########
         * #######################################################
         */

        public List<RoutineVisitSummaryReportDTO> getRoutineVisitSummaryPartner(DateTime initialDate, DateTime lastDate)
        {
            return getRoutineVisitSummary(initialDate, lastDate, "partner");
        }

        /*
         * #######################################################
         * ############## RoutineVisitSummaryReport ##############
         * #######################################################
         */

        public List<RoutineVisitSummaryReportDTO> getRoutineVisitSummary(DateTime initialDate, DateTime lastDate, string partnerType)
        {
            String query = @"SELECT
                                prt.ChiefPartner--<<ReplaceColumn<<--
								As Partner,
	                            ISNULL(age_obj.btw_x_1_M,0) As btw_x_1_M,
								ISNULL(age_obj.btw_1_4_M,0) As btw_1_4_M,
	                            ISNULL(age_obj.btw_5_9_M,0) As btw_5_9_M,
								ISNULL(age_obj.btw_10_14_M,0) As btw_10_14_M,
	                            ISNULL(age_obj.btw_15_17_M,0) As btw_15_17_M,
								ISNULL(age_obj.btw_18_24_M,0) As btw_18_24_M,
	                            ISNULL(age_obj.btw_25_x_M,0) As btw_25_x_M,
								ISNULL(age_obj.btw_x_1_F,0) As btw_x_1_F,
	                            ISNULL(age_obj.btw_1_4_F,0) As btw_1_4_F, 
								ISNULL(age_obj.btw_5_9_F,0) As btw_5_9_F,
	                            ISNULL(age_obj.btw_10_14_F,0) As btw_10_14_F, 
								ISNULL(age_obj.btw_15_17_F,0) As btw_15_17_F,
	                            ISNULL(age_obj.btw_18_24_F,0) As btw_18_24_F, 
								ISNULL(age_obj.btw_25_x_F,0) As btw_25_x_F,
	                            ISNULL(routv_obj.NewFinaceAid,0) As NewFinaceAid, 
								ISNULL(routv_obj.RptFinaceAid,0) As RptFinaceAid,
	                            ISNULL(routv_obj.NewHealth,0) As NewHealth, 
								ISNULL(routv_obj.RptHealth,0) As RptHealth,
	                            ISNULL(routv_obj.NewFood,0) As NewFood, 
								ISNULL(routv_obj.RptFood,0) As RptFood,
	                            ISNULL(routv_obj.NewEducation,0) As NewEducation, 
								ISNULL(routv_obj.RptEducation,0) As RptEducation,
	                            ISNULL(routv_obj.NewLegalAdvice,0) As NewLegalAdvice, 
								ISNULL(routv_obj.RptLegalAdvice,0) As RptLegalAdvice,
	                            ISNULL(routv_obj.NewHousing,0) As NewHousing, 
								ISNULL(routv_obj.RptHousing,0) As RptHousing,
	                            ISNULL(routv_obj.NewSocialAid,0) As NewSocialAid, 
								ISNULL(routv_obj.RptSocialAid,0) As RptSocialAid,
	                            ISNULL(famKit.FamilyKitReceived,0) As FamilyKitReceived, 
								ISNULL(routv_obj.NewDPI,0) As NewDPI, 
								ISNULL(routv_obj.RptDPI,0) As RptDPI,
								ISNULL(routv_obj.HIVRisk,0) As HIVRisk,
	                            ISNULL(hiv_obj.HIV_P_IN_TARV, 0) As HIV_P_IN_TARV,
	                            ISNULL(hiv_obj.HIV_P_NOT_TARV, 0) As HIV_P_NOT_TARV,
								ISNULL(hiv_obj.HIV_N, 0) As HIV_N,
	                            ISNULL(hiv_obj.HIV_UNKNOWN_NREVEAL, 0) As HIV_KNOWN_NREVEAL,
								ISNULL(hiv_obj.HIV_UNKNOWN_NRECOMMENDED, 0) As HIV_UNKNOWN_NRECOMMENDED,
	                            ISNULL(routv_obj.MUACGreen,0) As MUACGreen, 
								ISNULL(routv_obj.MUACYellow,0) As MUACYellow, 
								ISNULL(routv_obj.MUACRed,0) As MUACRed,
	                            ISNULL(nograduation_exits.Death,0) As Death, 
								ISNULL(nograduation_exits.Lost,0) As Lost, 
								ISNULL(nograduation_exits.GaveUp,0) As GaveUp, 
								ISNULL(nograduation_exits.Others,0) As Others
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
							)
							prt LEFT JOIN
                            (
								SELECT	--------------  Total de beneficiarios apoiados (Por Idade)
								age_obj.ChiefPartner--<<ReplaceColumn<<--
								,
								SUM(age_obj.btw_x_1_M) As btw_x_1_M, 
								SUM(age_obj.btw_1_4_M) As btw_1_4_M,
								SUM(age_obj.btw_5_9_M) As btw_5_9_M, 
								SUM(age_obj.btw_10_14_M) As btw_10_14_M,
								SUM(age_obj.btw_15_17_M) As btw_15_17_M, 
								SUM(age_obj.btw_18_24_M) As btw_18_24_M,
								SUM(age_obj.btw_25_x_M) As btw_25_x_M, 
								SUM(age_obj.btw_x_1_F) As btw_x_1_F, 
								SUM(age_obj.btw_1_4_F) As btw_1_4_F, 
								SUM(age_obj.btw_5_9_F) As btw_5_9_F, 
								SUM(age_obj.btw_10_14_F) As btw_10_14_F, 
								SUM(age_obj.btw_15_17_F) As btw_15_17_F, 
								SUM(age_obj.btw_18_24_F) As btw_18_24_F, 
								SUM(age_obj.btw_25_x_F) As btw_25_x_F
								FROM  
								(
									SELECT
									cp.Name As ChiefPartner, p.[Name] As [Partner], ben.FirstName As FirstName,  ben.LastName As LastName,
									DATEDIFF(year, CAST(ben.DateOfBirth As Date), @lastDate) As Age,
			                        btw_x_1_M = CASE WHEN DATEDIFF(month, CAST(ben.DateOfBirth As Date), @lastDate) < 12 AND ben.Gender = 'M' THEN 1 ELSE 0 END,
			                        btw_1_4_M = CASE WHEN DATEDIFF(month, CAST(ben.DateOfBirth As Date), @lastDate) BETWEEN 12 AND 59 AND ben.Gender = 'M' THEN 1 ELSE 0 END,
			                        btw_5_9_M = CASE WHEN DATEDIFF(month, CAST(ben.DateOfBirth As Date), @lastDate) BETWEEN 60 AND 119 AND ben.Gender = 'M' THEN 1 ELSE 0 END,
			                        btw_10_14_M = CASE WHEN DATEDIFF(month, CAST(ben.DateOfBirth As Date), @lastDate) BETWEEN 120 AND 179 AND ben.Gender = 'M' THEN 1 ELSE 0 END,
			                        btw_15_17_M = CASE WHEN DATEDIFF(month, CAST(ben.DateOfBirth As Date), @lastDate) BETWEEN 180 AND 215 AND ben.Gender = 'M' THEN 1 ELSE 0 END,
			                        btw_18_24_M = CASE WHEN DATEDIFF(month, CAST(ben.DateOfBirth As Date), @lastDate) BETWEEN 216 AND 299 AND ben.Gender = 'M' THEN 1 ELSE 0 END,
			                        btw_25_x_M = CASE WHEN DATEDIFF(month, CAST(ben.DateOfBirth As Date), @lastDate) >= 300 AND ben.Gender = 'M' THEN 1 ELSE 0 END,
			                        btw_x_1_F = CASE WHEN DATEDIFF(month, CAST(ben.DateOfBirth As Date), @lastDate) < 12 AND ben.Gender = 'F' THEN 1 ELSE 0 END,
			                        btw_1_4_F = CASE WHEN DATEDIFF(month, CAST(ben.DateOfBirth As Date), @lastDate) BETWEEN 12 AND 59 AND ben.Gender = 'F' THEN 1 ELSE 0 END,
			                        btw_5_9_F = CASE WHEN DATEDIFF(month, CAST(ben.DateOfBirth As Date), @lastDate) BETWEEN 60 AND 119 AND ben.Gender = 'F' THEN 1 ELSE 0 END,
			                        btw_10_14_F = CASE WHEN DATEDIFF(month, CAST(ben.DateOfBirth As Date), @lastDate) BETWEEN 120 AND 179 AND ben.Gender = 'F' THEN 1 ELSE 0 END,
			                        btw_15_17_F = CASE WHEN DATEDIFF(month, CAST(ben.DateOfBirth As Date), @lastDate) BETWEEN 180 AND 215 AND ben.Gender = 'F' THEN 1 ELSE 0 END,
			                        btw_18_24_F = CASE WHEN DATEDIFF(month, CAST(ben.DateOfBirth As Date), @lastDate) BETWEEN 216 AND 299 AND ben.Gender = 'F' THEN 1 ELSE 0 END,
			                        btw_25_x_F = CASE WHEN DATEDIFF(month, CAST(ben.DateOfBirth As Date), @lastDate) >= 300 AND ben.Gender = 'F' THEN 1 ELSE 0 END
									FROM  [Partner] p
									inner join  [Partner] cp on (p.SuperiorId = cp.PartnerID) 
									inner join  [HouseHold] hh on (p.PartnerID = hh.PartnerID)
									inner join  [Beneficiary] ben on (hh.HouseHoldID = ben.HouseholdID)
									Where p.CollaboratorRoleID = 1 
									AND ben.[Guid] in 
									(
										SELECT d.[Guid] FROM
										(
											SELECT
											routv_all.[Guid] As [Guid],
											ISNULL(SUM(routv_all.FinaceAid),0) As AllFinanceAid, 
											ISNULL(SUM(routv_dt.FinaceAid),0) As DtFinanceAid,
											ISNULL(SUM(routv_all.Health),0) As AllHealth, 
											ISNULL(SUM(routv_dt.Health),0) As DtHealth,
											ISNULL(SUM(routv_all.Food),0) As AllFood,
											ISNULL(SUM(routv_dt.Food),0) As DtFood,
											ISNULL(SUM(routv_all.Education),0) As AllEducation,
											ISNULL(SUM(routv_dt.Education),0) As DtEducation,
											ISNULL(SUM(routv_all.LegalAdvice),0) As AllLegalAdvice,
											ISNULL(SUM(routv_dt.LegalAdvice),0) As DtLegalAdvice,
											ISNULL(SUM(routv_all.Housing),0) As AllHousing,
											ISNULL(SUM(routv_dt.Housing),0) As DtHousing,
											ISNULL(SUM(routv_all.SocialAid),0) As AllSocialAid,
											ISNULL(SUM(routv_dt.SocialAid),0) As DtSocialAid,
											AllDPI = CASE WHEN DATEDIFF(YEAR, CAST(routv_dt.DateOfBirth As Date), routv_all.RoutineVisitDate) < 6 THEN SUM(routv_all.DPI) ELSE 0 END,
											DtDPI = CASE WHEN DATEDIFF(YEAR, CAST(routv_dt.DateOfBirth As Date), routv_all.RoutineVisitDate) < 6 THEN SUM(routv_dt.DPI) ELSE 0 END,
											MUACGreen = CASE WHEN DATEDIFF(month, CAST(routv_dt.DateOfBirth As Date), routv_all.RoutineVisitDate) BETWEEN 6 AND 59 THEN SUM(routv_dt.MUACGreen) ELSE 0 END,
											MUACYellow = CASE WHEN DATEDIFF(month, CAST(routv_dt.DateOfBirth As Date), routv_all.RoutineVisitDate) BETWEEN 6 AND 59 THEN SUM(routv_dt.MUACYellow) ELSE 0 END,
											MUACRed = CASE WHEN DATEDIFF(month, CAST(routv_dt.DateOfBirth As Date), routv_all.RoutineVisitDate) BETWEEN 6 AND 59 THEN SUM(routv_dt.MUACRed) ELSE 0 END
											FROM
											(
												SELECT c.*  from  [Children_Routine_Visit_Summary] c
												--inner join  [ChildStatusHistory] csh on (csh.ChildStatusHistoryID = 
												--(SELECT max(csh2.ChildStatusHistoryID) FROM  [ChildStatusHistory] csh2 WHERE csh2.EffectiveDate<= @lastDate AND (csh2.ChildID = c.ChildID)))
												--inner join  [ChildStatus] ct on (csh.childStatusID = ct.StatusID and ct.Description = 'Inicial')
												WHERE c.RoutineVisitDate BETWEEN @initialDate AND @lastDate

												union all

												SELECT a.* from  [Adults_Routine_Visit_Summary] a
												--inner join  [ChildStatusHistory] csh on (csh.ChildStatusHistoryID = 
												--(SELECT max(csh2.ChildStatusHistoryID) FROM  [ChildStatusHistory] csh2 WHERE csh2.EffectiveDate<= @lastDate AND (csh2.AdultID = a.AdultID)))
												--inner join  [ChildStatus] ct on (csh.childStatusID = ct.StatusID and ct.Description = 'Inicial')
												WHERE a.RoutineVisitDate BETWEEN @initialDate AND @lastDate
											) 
											routv_all left join
											(
												SELECT rvs.* FROM  [Routine_Visit_Summary] rvs
												WHERE rvs.RoutineVisitDate BETWEEN @initialDate AND @lastDate
											) routv_dt on (routv_all.RoutineVisitSupportID = routv_dt.RoutineVisitSupportID)
											group by routv_all.[Guid], routv_dt.[Guid], routv_dt.DateOfBirth,routv_all.RoutineVisitDate
										) d
										WHERE
										((d.AllFinanceAid + d.DtFinanceAid + d.AllHealth  + d.DtHealth + d.AllFood  + d.DtFood  +
										d.AllEducation + d.DtEducation + d.AllLegalAdvice + d.DtLegalAdvice + d.AllHousing + d.DtHousing  +
										d.AllSocialAid + d.DtSocialAid  + d.AllDPI + d.DtDPI + d.MUACGreen + d.MUACYellow + d.MUACRed) > 0)
									)
									OR ben.[Guid] in 
									(
										SELECT b.Guid from  [Beneficiary] b
										inner join  [HIVStatus] hs on (hs.HIVStatusID != b.HIVStatusID and hs.HIVStatusID =
										(SELECT max(hs2.HIVStatusID) FROM  [HIVStatus] hs2 WHERE hs2.[InformationDate] BETWEEN @initialDate AND @lastDate AND (hs2.BeneficiaryGuid = b.Guid)))  
									)
								) age_obj
								group by age_obj.ChiefPartner--<<ReplaceColumn<<--
                            ) age_obj
							on
							(
								prt.ChiefPartner--<<ReplaceColumn<<--
								=
								age_obj.ChiefPartner--<<ReplaceColumn<<--
							)
                            left join
                            (
	                            SELECT --  Total de beneficiarios de acordo com o tipo de Serviço prestado a criança (Por apoio prestado)
	                            routv_obj.ChiefPartner--<<ReplaceColumn<<--
	                            ,
								SUM(routv_obj.NewFinaceAid) As NewFinaceAid, SUM(routv_obj.RptFinaceAid) As RptFinaceAid,
	                            SUM(routv_obj.NewHealth) As NewHealth, SUM(routv_obj.RptHealth) As RptHealth,
	                            SUM(routv_obj.NewFood) As NewFood, SUM(routv_obj.RptFood) As RptFood,
	                            SUM(routv_obj.NewEducation) As NewEducation, SUM(routv_obj.RptEducation) As RptEducation,
	                            SUM(routv_obj.NewLegalAdvice) As NewLegalAdvice, SUM(routv_obj.RptLegalAdvice) As RptLegalAdvice,
	                            SUM(routv_obj.NewHousing) As NewHousing, SUM(routv_obj.RptHousing) As RptHousing,
	                            SUM(routv_obj.NewSocialAid) As NewSocialAid, SUM(routv_obj.RptSocialAid) As RptSocialAid,
								SUM(routv_obj.HIVRisk) As HIVRisk,
	                            SUM(routv_obj.NewDPI) As NewDPI, SUM(routv_obj.RptDPI) As RptDPI,
	                            SUM(routv_obj.MUACGreen) As MUACGreen, SUM(routv_obj.MUACYellow) As MUACYellow, SUM(routv_obj.MUACRed) As MUACRed
	                            FROM
	                            (
		                            SELECT  -- Eliminar os registos novos dos repetidos
			                            data.ChiefPartner, data.[Partner], data.[Guid],
			                            NewFinaceAid, RptFinaceAid = CASE WHEN NewFinaceAid = 1 AND RptFinaceAid > 0 THEN (RptFinaceAid-1) ELSE RptFinaceAid END,
			                            NewHealth, RptHealth = CASE WHEN NewHealth = 1 AND RptHealth > 0 THEN (RptHealth-1) ELSE RptHealth END, 
			                            NewFood, RptFood = CASE WHEN NewFood = 1 AND RptFood > 0 THEN (RptFood-1) ELSE RptFood END,
			                            NewEducation, RptEducation = CASE WHEN NewEducation = 1 AND RptEducation > 0 THEN (RptEducation-1) ELSE RptEducation END, 
			                            NewLegalAdvice, RptLegalAdvice = CASE WHEN NewLegalAdvice = 1 AND RptLegalAdvice > 0 THEN (RptLegalAdvice-1) ELSE RptLegalAdvice END, 
			                            NewHousing, RptHousing = CASE WHEN NewHousing = 1 AND RptHousing > 0 THEN (RptHousing-1) ELSE RptHousing END, 
			                            NewSocialAid, RptSocialAid = CASE WHEN NewSocialAid = 1 AND RptSocialAid > 0 THEN (RptSocialAid-1) ELSE RptSocialAid END,
			                            NewDPI, RptDPI = CASE WHEN NewDPI = 1 AND RptDPI > 0 THEN (RptDPI-1) ELSE RptDPI END, 
			                            HIVRisk, MUACGreen, MUACYellow, MUACRed
		                            FROM
		                            (
			                            SELECT  --  Group By Partner
			                            data.ChiefPartner, data.[Partner], data.[Guid],
			                            NewFinaceAid = CASE WHEN (SUM(data.AllFinanceAid) = 1 AND SUM(data.DtFinanceAid) = 1) OR (SUM(data.AllFinanceAid) > 0 AND SUM(data.AllFinanceAid)-SUM(data.DtFinanceAid) = 0) THEN 1 ELSE 0 END,
			                            RptFinaceAid = CASE WHEN SUM(data.AllFinanceAid) > 1 THEN SUM(data.DtFinanceAid) ELSE 0 END,
			                            NewHealth = CASE WHEN (SUM(data.AllHealth) = 1 AND SUM(data.DtHealth) = 1) OR (SUM(data.AllHealth) > 0 AND SUM(data.AllHealth)-SUM(data.DtHealth) = 0) THEN 1 ELSE 0 END,
			                            RptHealth = CASE WHEN SUM(data.AllHealth) > 1 THEN SUM(data.DtHealth) ELSE 0 END,
			                            NewFood = CASE WHEN SUM(data.AllFood) = 1  AND SUM(data.DtFood) = 1 OR SUM(data.AllFood) > 0 AND SUM(data.AllFood)-SUM(data.DtFood) = 0 THEN 1 ELSE 0 END,
			                            RptFood = CASE WHEN SUM(data.AllFood) > 1 THEN SUM(data.DtFood) ELSE 0 END,
			                            NewEducation = CASE WHEN SUM(data.AllEducation) = 1 AND SUM(data.DtEducation) = 1 OR SUM(data.AllEducation) > 0 AND SUM(data.AllEducation)-SUM(data.DtEducation) = 0 THEN 1 ELSE 0 END,
			                            RptEducation = CASE WHEN SUM(data.AllEducation) > 1 THEN SUM(data.DtEducation) ELSE 0 END,
			                            NewLegalAdvice = CASE WHEN SUM(data.AllLegalAdvice) = 1 AND SUM(data.DtLegalAdvice) = 1 OR SUM(data.AllLegalAdvice) > 0 AND SUM(data.AllLegalAdvice)-SUM(data.DtLegalAdvice) = 0 THEN 1 ELSE 0 END,
			                            RptLegalAdvice = CASE WHEN SUM(data.AllLegalAdvice) > 1 THEN SUM(data.DtLegalAdvice) ELSE 0 END,
			                            NewHousing = CASE WHEN SUM(data.AllHousing) = 1  AND SUM(data.DtHousing) = 1 OR SUM(data.AllHousing) > 0 AND SUM(data.AllHousing)-SUM(data.DtHousing) = 0 THEN 1 ELSE 0 END,
			                            RptHousing = CASE WHEN SUM(data.AllHousing) > 1 THEN SUM(data.DtHousing) ELSE 0 END,
			                            NewSocialAid = CASE WHEN SUM(data.AllSocialAid) = 1 AND SUM(data.DtSocialAid) = 1 OR SUM(data.AllSocialAid) > 0 AND SUM(data.AllSocialAid)-SUM(data.DtSocialAid) = 0 THEN 1 ELSE 0 END,
			                            RptSocialAid = CASE WHEN SUM(data.AllSocialAid) > 1 THEN SUM(data.DtSocialAid) ELSE 0 END,
			                            NewDPI = CASE WHEN SUM(data.AllDPI) = 1 AND SUM(data.DtDPI) = 1 OR SUM(data.AllDPI) > 0 AND SUM(data.AllDPI)-SUM(data.DtDPI) = 0 THEN 1 ELSE 0 END,
			                            RptDPI = CASE WHEN SUM(data.AllDPI) > 1 THEN SUM(data.DtDPI) ELSE 0 END,
			                            SUM(data.HIVRisk) As HIVRisk, SUM(data.MUACGreen) As MUACGreen, SUM(data.MUACYellow) As MUACYellow, SUM(data.MUACRed) As MUACRed
			                            FROM
			                            (
				                            SELECT  --  Group By Beneficiary
				                            routv_all.ChiefPartner, routv_all.[Partner], routv_all.[Guid],
				                            ISNULL(SUM(routv_all.FinaceAid),0) As AllFinanceAid, ISNULL(SUM(routv_dt.FinaceAid),0) As DtFinanceAid,
				                            ISNULL(SUM(routv_all.Health),0) As AllHealth, ISNULL(SUM(routv_dt.Health),0) As DtHealth,
				                            ISNULL(SUM(routv_all.Food),0) As AllFood, ISNULL(SUM(routv_dt.Food),0) As DtFood,
				                            ISNULL(SUM(routv_all.Education),0) As AllEducation, ISNULL(SUM(routv_dt.Education),0) As DtEducation,
				                            ISNULL(SUM(routv_all.LegalAdvice),0) As AllLegalAdvice, ISNULL(SUM(routv_dt.LegalAdvice),0) As DtLegalAdvice,
				                            ISNULL(SUM(routv_all.Housing),0) As AllHousing, ISNULL(SUM(routv_dt.Housing),0) As DtHousing,
				                            ISNULL(SUM(routv_all.SocialAid),0) As AllSocialAid, ISNULL(SUM(routv_dt.SocialAid),0) As DtSocialAid,
				                            AllDPI = CASE WHEN DATEDIFF(YEAR, CAST(routv_dt.DateOfBirth As Date), routv_all.RoutineVisitDate) < 6 THEN SUM(routv_all.DPI) ELSE 0 END,
				                            DtDPI = CASE WHEN DATEDIFF(YEAR, CAST(routv_dt.DateOfBirth As Date), routv_all.RoutineVisitDate) < 6 THEN SUM(routv_dt.DPI) ELSE 0 END,
				                            HIVRisk = CASE WHEN routv_all.RegistrationDate BETWEEN @initialDate and @lastDate AND routv_all.HIVTracked = 1 THEN 1 ELSE SUM(routv_all.HIVRisk) END,
											MUACGreen = CASE WHEN DATEDIFF(month, CAST(routv_dt.DateOfBirth As Date), routv_all.RoutineVisitDate) BETWEEN 6 AND 59 THEN SUM(routv_dt.MUACGreen) ELSE 0 END,
				                            MUACYellow = CASE WHEN DATEDIFF(month, CAST(routv_dt.DateOfBirth As Date), routv_all.RoutineVisitDate) BETWEEN 6 AND 59 THEN SUM(routv_dt.MUACYellow) ELSE 0 END,
				                            MUACRed = CASE WHEN DATEDIFF(month, CAST(routv_dt.DateOfBirth As Date), routv_all.RoutineVisitDate) BETWEEN 6 AND 59 THEN SUM(routv_dt.MUACRed) ELSE 0 END
					                        FROM
					                        (
												SELECT c.*  from  [Children_Routine_Visit_Summary] c
												--inner join  [ChildStatusHistory] csh on (csh.ChildStatusHistoryID = 
												--(SELECT max(csh2.ChildStatusHistoryID) FROM  [ChildStatusHistory] csh2 WHERE csh2.EffectiveDate<= @lastDate AND (csh2.ChildID = c.ChildID)))
												--inner join  [ChildStatus] ct on (csh.childStatusID = ct.StatusID and ct.Description = 'Inicial')
												WHERE c.RoutineVisitDate BETWEEN
						                        (
							                        SELECT GetFiscalInitialDate = 
							                        CASE WHEN MONTH(@initialDate) in (1,2,3,4,5,6,7,8) AND YEAR(@initialDate) = YEAR(@lastDate) THEN DATEFROMPARTS(YEAR(@initialDate)-1,09,21)
							                        WHEN MONTH(@initialDate) in (9,10,11,12) AND YEAR(@initialDate) = YEAR(@lastDate) THEN DATEFROMPARTS(YEAR(@initialDate),09,21) 
							                        WHEN MONTH(@initialDate) in (9,10,11,12) AND YEAR(@initialDate) <> YEAR(@lastDate) THEN DATEFROMPARTS(YEAR(@initialDate),09,21)
							                        END
						                        )
						                        AND @lastDate

												union all

												SELECT a.* from  [Adults_Routine_Visit_Summary] a
												--inner join  [ChildStatusHistory] csh on (csh.ChildStatusHistoryID = 
												--(SELECT max(csh2.ChildStatusHistoryID) FROM  [ChildStatusHistory] csh2 WHERE csh2.EffectiveDate<= @lastDate AND (csh2.AdultID = a.AdultID)))
												--inner join  [ChildStatus] ct on (csh.childStatusID = ct.StatusID and ct.Description = 'Inicial')
												WHERE a.RoutineVisitDate BETWEEN
						                        (
							                        SELECT GetFiscalInitialDate = 
							                        CASE WHEN MONTH(@initialDate) in (1,2,3,4,5,6,7,8) AND YEAR(@initialDate) = YEAR(@lastDate) THEN DATEFROMPARTS(YEAR(@initialDate)-1,09,21)
							                        WHEN MONTH(@initialDate) in (9,10,11,12) AND YEAR(@initialDate) = YEAR(@lastDate) THEN DATEFROMPARTS(YEAR(@initialDate),09,21) 
							                        WHEN MONTH(@initialDate) in (9,10,11,12) AND YEAR(@initialDate) <> YEAR(@lastDate) THEN DATEFROMPARTS(YEAR(@initialDate),09,21)
							                        END
						                        )
						                        AND @lastDate
					                        ) 
					                        routv_all left join
					                        (
						                        SELECT rvs.* from  [Routine_Visit_Summary] rvs
						                        WHERE rvs.RoutineVisitDate BETWEEN @initialDate AND @lastDate
					                        ) routv_dt on (routv_all.RoutineVisitSupportID = routv_dt.RoutineVisitSupportID)
					                        group by routv_all.ChiefPartner, routv_all.[Partner], routv_all.[Guid], 
											routv_dt.ChiefPartner, routv_dt.[Partner], routv_dt.DateOfBirth, routv_all.RegistrationDate, routv_all.RoutineVisitDate, routv_all.HIVTracked
				                        ) data
			                            group by data.ChiefPartner, data.Partner, data.[Guid]
		                            ) data
	                            ) routv_obj
	                            group by routv_obj.ChiefPartner--<<ReplaceColumn<<--
                            ) routv_obj
                            on 
							( 
								prt.ChiefPartner--<<ReplaceColumn<<--
								= 
								routv_obj.ChiefPartner--<<ReplaceColumn<<--
							)
							left join
							(
								Select
								data.ChiefPartner--<<ReplaceColumn<<--
								,
								Sum(data.FamilyKitReceived) As FamilyKitReceived 
								from
								(
									select cp.Name As ChiefPartner, p.Name As Partner, hh.HouseholdID,
									Sum(CASE WHEN rv.FamilyKitReceived = 1 THEN 1 ELSE 0 END) As FamilyKitReceived
									from  [RoutineVisit] rv 
									inner join  HouseHold hh on (rv.HouseholdID = hh.HouseHoldID)
									inner join  [Partner] p on (hh.PartnerID = p.PartnerID)
									inner join  [Partner] cp on (p.SuperiorId = cp.PartnerID)
									where rv.RoutineVisitDate BETWEEN @initialDate AND @lastDate
									group by cp.Name, p.Name, hh.HouseholdID
								) data
								group by data.ChiefPartner--<<ReplaceColumn<<--
							) famKit 
							on 
							(
								prt.ChiefPartner--<<ReplaceColumn<<--
								= 
								famKit.ChiefPartner--<<ReplaceColumn<<--
							)
                            left join
                            (
								SELECT --  Saidas sem graduacao	
	                            obj.ChiefPartner--<<ReplaceColumn<<--
								,
								SUM(obj.Death) As Death, SUM(obj.Lost) As Lost, SUM(obj.GaveUp) As GaveUp, SUM(obj.Others) As Others
	                            FROM
	                            (
		                            SELECT
		                            cp.Name As ChiefPartner,
		                            p.Name As Partner,
		                            b.FirstName As FirstName, b.LastName As LastName, b.BeneficiaryState,
		                            Death = CASE WHEN b.BeneficiaryState = 'Óbito' THEN 1 ELSE 0 END,
		                            Lost = CASE WHEN b.BeneficiaryState = 'Perdido' THEN 1 ELSE 0 END,
		                            GaveUp = CASE WHEN b.BeneficiaryState = 'Desistência' THEN 1 ELSE 0 END,
		                            Others = CASE WHEN b.BeneficiaryState = 'Outras Saídas' THEN 1 ELSE 0 END
		                            FROM  [Partner] p
		                            inner join  [Partner] cp on (p.SuperiorId = cp.PartnerID) 
		                            inner join  [HouseHold] hh on (p.PartnerID = hh.PartnerID)
		                            inner join  [Beneficiary] b on (hh.HouseHoldID = b.HouseholdID)
									inner join  [ChildStatusHistory] csh on (csh.ChildStatusHistoryID = 
									(SELECT max(csh2.ChildStatusHistoryID) FROM  [ChildStatusHistory] csh2 WHERE csh2.EffectiveDate<= @lastDate AND (csh2.BeneficiaryGuid = b.Guid)))
		                            inner join  [Routine_Visit_Summary] rvs on (rvs.Guid = b.Guid)
									WHERE rvs.RoutineVisitDate BETWEEN @initialDate AND @lastDate AND p.CollaboratorRoleID = 1
		                            group by cp.Name, p.Name, b.FirstName, b.LastName, b.BeneficiaryState
	                            ) obj
	                            group by obj.ChiefPartner--<<ReplaceColumn<<--
                            ) nograduation_exits on 
							(
								prt.ChiefPartner--<<ReplaceColumn<<--
								= 
								nograduation_exits.ChiefPartner--<<ReplaceColumn<<--
							)
							left join
                            (
	                            SELECT  --  Estado de HIV das Crianças dos Activistas, agrupado por activista chefe
	                            hiv_obj.ChiefPartner--<<ReplaceColumn<<--
	                            ,
								SUM(hiv_obj.HIV_P_IN_TARV) As HIV_P_IN_TARV,
	                            SUM(hiv_obj.HIV_P_NOT_TARV) As HIV_P_NOT_TARV,
								SUM(hiv_obj.HIV_N) As HIV_N,
	                            SUM(hiv_obj.HIV_UNKNOWN_NREVEAL) As HIV_UNKNOWN_NREVEAL,
	                            SUM(hiv_obj.HIV_UNKNOWN_NRECOMMENDED) As HIV_UNKNOWN_NRECOMMENDED
	                            FROM
	                            (
									SELECT
			                            cp.Name As ChiefPartner, cp.PartnerID As ChiefPartnerID,
										p.Name As Partner, p.PartnerID As PartnerID,
			                            c.FirstName As FirstName, c.LastName As LastName,
										c. child_guid As Guid,
			                            hs.HIV,
			                            HIV_N = CASE WHEN hs.HIV = 'N' THEN 1 ELSE 0 END,
			                            HIV_P_IN_TARV = CASE WHEN hs.HIV = 'P' AND hs.HIVInTreatment = 0 THEN 1 ELSE 0 END,
			                            HIV_P_NOT_TARV = CASE WHEN hs.HIV = 'P' AND hs.HIVInTreatment = 1 THEN 1 ELSE 0 END,
			                            HIV_UNKNOWN_NREVEAL = CASE WHEN hs.HIV = 'U' AND hs.HIVUndisclosedReason = 0 THEN 1 ELSE 0 END,
			                            HIV_UNKNOWN_NRECOMMENDED = CASE WHEN hs.HIV = 'U' AND hs.HIVUndisclosedReason = 2 THEN 1 ELSE 0 END
		                            FROM 
									 Child c
									inner join  [HouseHold] hh ON (c.HouseholdID = hh.HouseholdID)
									inner join  [Partner] p on (p.PartnerID = hh.PartnerID)
		                            inner join  [Partner] cp ON (p.SuperiorId = cp.PartnerID) 
									inner join  [HIVStatus] hs on (hs.HIVStatusID != c.HIVStatusID and hs.HIVStatusID =
									(SELECT max(hs2.HIVStatusID) FROM  [HIVStatus] hs2 WHERE hs2.[InformationDate] <= @lastDate AND (hs2.ChildID = c.ChildID))) 

									union all

									SELECT
			                            cp.Name As ChiefPartner, cp.PartnerID As ChiefPartnerID,
										p.Name As Partner, p.PartnerID As PartnerID,
			                            a.FirstName As FirstName, a.LastName As LastName,
										a.AdultGuid As Guid,
			                            hs.HIV,
			                            HIV_N = CASE WHEN hs.HIV = 'N' THEN 1 ELSE 0 END,
			                            HIV_P_IN_TARV = CASE WHEN hs.HIV = 'P' AND hs.HIVInTreatment = 0 THEN 1 ELSE 0 END,
			                            HIV_P_NOT_TARV = CASE WHEN hs.HIV = 'P' AND hs.HIVInTreatment = 1 THEN 1 ELSE 0 END,
			                            HIV_UNKNOWN_NREVEAL = CASE WHEN hs.HIV = 'U' AND hs.HIVUndisclosedReason = 0 THEN 1 ELSE 0 END,
			                            HIV_UNKNOWN_NRECOMMENDED = CASE WHEN hs.HIV = 'U' AND hs.HIVUndisclosedReason = 2 THEN 1 ELSE 0 END
		                            FROM 
									 Adult a
									inner join  [HouseHold] hh ON (a.HouseholdID = hh.HouseholdID)
									inner join  [Partner] p on (p.PartnerID = hh.PartnerID)
		                            inner join  [Partner] cp ON (p.SuperiorId = cp.PartnerID) 
									inner join  [HIVStatus] hs on (hs.HIVStatusID != a.HIVStatusID and hs.HIVStatusID = 
									(SELECT max(hs2.HIVStatusID) FROM  [HIVStatus] hs2 WHERE hs2.[InformationDate] <= @lastDate AND (hs2.AdultID = a.AdultId)))
	                            ) hiv_obj
								Where hiv_obj.Guid in 
								(
									SELECT d.[Guid] FROM
									(
										SELECT
										routv_all.[Guid] As [Guid],
										ISNULL(SUM(routv_all.FinaceAid),0) As AllFinanceAid, 
										ISNULL(SUM(routv_dt.FinaceAid),0) As DtFinanceAid,
										ISNULL(SUM(routv_all.Health),0) As AllHealth, 
										ISNULL(SUM(routv_dt.Health),0) As DtHealth,
										ISNULL(SUM(routv_all.Food),0) As AllFood,
										ISNULL(SUM(routv_dt.Food),0) As DtFood,
										ISNULL(SUM(routv_all.Education),0) As AllEducation,
										ISNULL(SUM(routv_dt.Education),0) As DtEducation,
										ISNULL(SUM(routv_all.LegalAdvice),0) As AllLegalAdvice,
										ISNULL(SUM(routv_dt.LegalAdvice),0) As DtLegalAdvice,
										ISNULL(SUM(routv_all.Housing),0) As AllHousing,
										ISNULL(SUM(routv_dt.Housing),0) As DtHousing,
										ISNULL(SUM(routv_all.SocialAid),0) As AllSocialAid,
										ISNULL(SUM(routv_dt.SocialAid),0) As DtSocialAid,
										AllDPI = CASE WHEN DATEDIFF(YEAR, CAST(routv_dt.DateOfBirth As Date), routv_all.RoutineVisitDate) < 6 THEN SUM(routv_all.DPI) ELSE 0 END,
										DtDPI = CASE WHEN DATEDIFF(YEAR, CAST(routv_dt.DateOfBirth As Date), routv_all.RoutineVisitDate) < 6 THEN SUM(routv_dt.DPI) ELSE 0 END,
										MUACGreen = CASE WHEN DATEDIFF(month, CAST(routv_dt.DateOfBirth As Date), routv_all.RoutineVisitDate) BETWEEN 6 AND 59 THEN SUM(routv_dt.MUACGreen) ELSE 0 END,
										MUACYellow = CASE WHEN DATEDIFF(month, CAST(routv_dt.DateOfBirth As Date), routv_all.RoutineVisitDate) BETWEEN 6 AND 59 THEN SUM(routv_dt.MUACYellow) ELSE 0 END,
										MUACRed = CASE WHEN DATEDIFF(month, CAST(routv_dt.DateOfBirth As Date), routv_all.RoutineVisitDate) BETWEEN 6 AND 59 THEN SUM(routv_dt.MUACRed) ELSE 0 END
										FROM
										(
											SELECT c.*  from  [Children_Routine_Visit_Summary] c
											--inner join  [ChildStatusHistory] csh on (csh.ChildStatusHistoryID = 
											--(SELECT max(csh2.ChildStatusHistoryID) FROM  [ChildStatusHistory] csh2 WHERE csh2.EffectiveDate<= @lastDate AND (csh2.ChildID = c.ChildID)))
											--inner join  [ChildStatus] ct on (csh.childStatusID = ct.StatusID and ct.Description = 'Inicial')
											WHERE c.RoutineVisitDate BETWEEN @initialDate AND @lastDate

											union all

											SELECT a.* from  [Adults_Routine_Visit_Summary] a
											--inner join  [ChildStatusHistory] csh on (csh.ChildStatusHistoryID = 
											--(SELECT max(csh2.ChildStatusHistoryID) FROM  [ChildStatusHistory] csh2 WHERE csh2.EffectiveDate<= @lastDate AND (csh2.AdultID = a.AdultID)))
											--inner join  [ChildStatus] ct on (csh.childStatusID = ct.StatusID and ct.Description = 'Inicial')
											WHERE a.RoutineVisitDate BETWEEN @initialDate AND @lastDate
										) 
										routv_all left join
										(
											SELECT rvs.* FROM  [Routine_Visit_Summary] rvs
											WHERE rvs.RoutineVisitDate BETWEEN @initialDate AND @lastDate
										) routv_dt on (routv_all.RoutineVisitSupportID = routv_dt.RoutineVisitSupportID)
										group by routv_all.[Guid], routv_dt.[Guid], routv_dt.DateOfBirth, routv_all.RoutineVisitDate
									) d
									WHERE
									((d.AllHealth + d.DtHealth) > 0)
								)
	                            group by
		                            hiv_obj.ChiefPartner--<<ReplaceColumn<<--
                            ) hiv_obj on
							(
								prt.ChiefPartner--<<ReplaceColumn<<--
								=
								hiv_obj.ChiefPartner--<<ReplaceColumn<<--
							)";

            query = (partnerType.Equals("partner")) ? query.Replace("ChiefPartner--<<ReplaceColumn<<--", "Partner") : query;

            return UnitOfWork.DbContext.Database.SqlQuery<RoutineVisitSummaryReportDTO>(query,
                    new SqlParameter("initialDate", initialDate),
                    new SqlParameter("lastDate", lastDate)).ToList();
        }

        public List<GlobalReportDTO> getGlobalReport(DateTime initialDate, DateTime lastDate)
        {
            String query = @"SELECT
                                agregado_obj.personType AS personType
                                ,agregado_obj.FirstName --NEW PRIMEIRO NOME
                                ,agregado_obj.LastName --NEW APELIDO
                                ,agregado_obj.HouseholdName AS HouseholdName
                                ,agregado_obj.ChiefPartner AS ChiefPartner
                                ,agregado_obj.Partner AS Partner
                                ,ISNULL(agregado_obj.Age,-1) AS Age
        
                                --,ISNULL(agregado_obj.DateOFBirth,-1) AS DateOFBirth
		                        ,CONVERT(varchar,agregado_obj.DateOFBirth,103) AS DateOFBirth
                                ,agregado_obj.DateOfBirthUnknown
                      
                                ,agregado_obj.Gender AS Gender
                                ,agregado_obj.District AS District
                                ,agregado_obj.AdministrativePost AS AdministrativePost
                                ,agregado_obj.NeighborhoodName AS NeighborhoodName

                                ,agregado_obj.Block --NEW QUARTEIRÃO
                                ,agregado_obj.ClosePlaceToHome --NEW LUGAR PROXIMO DE CASA

                                ,agregado_obj.PrincipalChiefName AS PrincipalChiefName

                                ,agregado_obj.FamilyHeadDescription --NEW RESPONSAVEL PELA FAMILIA

                                --,agregado_obj.RegistrationDate AS RegistrationDate
		                        ,CONVERT(varchar,agregado_obj.RegistrationDate,103) AS RegistrationDate
                                ,agregado_obj.InstitutionalAid AS InstitutionalAid
                                ,agregado_obj.InstitutionalAidDetail --NEW DETALHES APOIO INSTITUCIONAL
                                ,agregado_obj.communityAid
                                ,agregado_obj.communityAidDetail --NEW DETALHES APOIO COMUNIDADE
                                ,agregado_obj.individualAid --NEW APOIO INDIVIDUAL
                                ,agregado_obj.FamilyPhoneNumber
                                ,agregado_obj.AnyoneBedridden
                                ,agregado_obj.FamilyOriginReference
                                ,agregado_obj.OtherFamilyOriginRef --NEW NOME DE ORIGEM DA FAMILIA
                                ,agregado_obj.ovcDescription
                                ,agregado_obj.degreeOfKingshipDescription
                                ,agregado_obj.IsPartSavingGroup
                                ,agregado_obj.HIVStatus
                                ,agregado_obj.HIVStatusDetails --NEW DETALHES HIV
                                ,agregado_obj.NID

                                ,ISNULL(mac_obj.P1,-1) AS P1
                                ,ISNULL(mac_obj.P2,-1) AS P2
                                ,ISNULL(mac_obj.P3,-1) AS P3
                                ,ISNULL(mac_obj.P4,-1) AS P4
                                ,ISNULL(mac_obj.P5,-1) AS P5
                                ,ISNULL(mac_obj.P6,-1) AS P6
                                ,ISNULL(mac_obj.P7,-1) AS P7
                                ,ISNULL(mac_obj.P8,-1) AS P8
                                ,ISNULL(mac_obj.P9,-1) AS P9
                                ,ISNULL(mac_obj.P10,-1) AS P10
                                ,ISNULL(mac_obj.P11,-1) AS P11
                                ,ISNULL(mac_obj.P12,-1) AS P12
                                ,ISNULL(mac_obj.P13,-1) AS P13
                                ,ISNULL(mac_obj.P14,-1) AS P14
                                ,ISNULL(mac_obj.P15,-1) AS P15
                                ,ISNULL(mac_obj.P16,-1) AS P16
                                ,ISNULL(mac_obj.P17,-1) AS P17
                                ,ISNULL(mac_obj.P18,-1) AS P18
                                ,ISNULL(mac_obj.P19,-1) AS P19
                                ,ISNULL(mac_obj.P20,-1) AS P20
                                ,ISNULL(mac_obj.P21,-1) AS P21
                                ,ISNULL(mac_obj.P22,-1) AS P22
                                ,ISNULL(mac_obj.P23,-1) AS P23
                                ,ISNULL(mac_obj.P24,-1) AS P24
                                ,ISNULL(mac_obj.P25,-1) AS P25
                                ,ISNULL(mac_obj.P26,-1) AS P26
                                ,ISNULL(mac_obj.P27,-1) AS P27
                                ,ISNULL(mac_obj.P28,-1) AS P28
                                ,ISNULL(mac_obj.P29,-1) AS P29
                                ,ISNULL(mac_obj.P30,-1) AS P30
                                ,ISNULL(mac_obj.P31,-1) AS P31
                                ,ISNULL(mac_obj.P32,-1) AS P32
                                ,ISNULL(mac_obj.P33,-1) AS P33
                                ,ISNULL(pAccao_obj.A1,'') AS A1
                                ,ISNULL(pAccao_obj.A2,'') AS A2
                                ,ISNULL(pAccao_obj.A3,'') AS A3
                                ,ISNULL(pAccao_obj.A4,'') AS A4
                                ,ISNULL(pAccao_obj.A5,'') AS A5
                                ,ISNULL(pAccao_obj.A6,'') AS A6
                                ,ISNULL(pAccao_obj.A7,'') AS A7
                                ,ISNULL(pAccao_obj.A8,'') AS A8
                                ,ISNULL(pAccao_obj.A9,'') AS A9
                                ,ISNULL(pAccao_obj.A10,'') AS A10
                                ,ISNULL(pAccao_obj.A11,'') AS A11
                                ,ISNULL(pAccao_obj.A12,'') AS A12
                                ,ISNULL(pAccao_obj.A13,'') AS A13
                                ,ISNULL(pAccao_obj.A14,'') AS A14
                                ,ISNULL(pAccao_obj.A15,'') AS A15
                                ,ISNULL(pAccao_obj.A16,'') AS A16
                                ,ISNULL(pAccao_obj.A17,'') AS A17
                                ,ISNULL(pAccao_obj.A18,'') AS A18
                                ,ISNULL(pAccao_obj.A19,'') AS A19
                                ,ISNULL(pAccao_obj.A20,'') AS A20
                                ,ISNULL(pAccao_obj.A21,'') AS A21
                                ,ISNULL(pAccao_obj.A22,'') AS A22
                                ,ISNULL(pAccao_obj.A23,'') AS A23
                                ,ISNULL(pAccao_obj.A24,'') AS A24
                                ,ISNULL(pAccao_obj.A25,'') AS A25
                                ,ISNULL(pAccao_obj.A26,'') AS A26
                                ,ISNULL(pAccao_obj.A27,'') AS A27
                                ,ISNULL(pAccao_obj.A28,'') AS A28
                                ,ISNULL(pAccao_obj.A29,'') AS A29
                                ,ISNULL(pAccao_obj.A30,'') AS A30
                                ,ISNULL(pAccao_obj.A31,'') AS A31
                                ,ISNULL(pAccao_obj.A32,'') AS A32
                                ,ISNULL(pAccao_obj.A33,'') AS A33
           
                                ,agregado_obj.ChildStatusDescription
		                        ,fichaseguimento_obj.FirstTimeSavingGroup
                                ,fichaseguimento_obj.FE
                                ,fichaseguimento_obj.AN
                                ,fichaseguimento_obj.HAB
                                ,fichaseguimento_obj.ED
                                ,fichaseguimento_obj.SD
                                ,fichaseguimento_obj.APS
                                ,fichaseguimento_obj.PL
                                ,fichaseguimento_obj.DPI
                                ,fichaseguimento_obj.MUACGREEN
                                ,fichaseguimento_obj.MUACYELLOW
                                ,fichaseguimento_obj.MUACRED

		                        ,ISNULL(ref_obj.ATS,'') AS ATS
                                ,ISNULL(ref_obj.TARV,'') AS TARV
                                ,ISNULL(ref_obj.CCR,'') AS CCR
                                ,ISNULL(ref_obj.SSR,'') AS SSR
                                ,ISNULL(ref_obj.VGB,'') AS VGB
                                ,ISNULL(ref_obj.Others,'') AS Others

		                        --,ref_obj.ReferenceDate
		                        --,CONVERT(varchar,ref_obj.ReferenceDate,103) AS ReferenceDate
		                        ,ISNULL(CONVERT(varchar,ref_obj.ReferenceDate,103),'') AS ReferenceDate

                                ,ISNULL(ref_obj.RC_ATS,'') AS RC_ATS
                                ,ISNULL(ref_obj.RC_TARV,'') AS RC_TARV
                                ,ISNULL(ref_obj.RC_CCR,'') AS RC_CCR
                                ,ISNULL(ref_obj.RC_SSR,'') AS RC_SSR
                                ,ISNULL(ref_obj.RC_VGB,'') AS RC_VGB
		
                                --,ref_obj.HealthAttendedDate
		                        --,CONVERT(varchar,ref_obj.HealthAttendedDate,103) AS HealthAttendedDate
		                        ,ISNULL(CONVERT(varchar,ref_obj.HealthAttendedDate,103),'') AS HealthAttendedDate

                                --,ref_obj.SocialAttendedDate
		                        --,CONVERT(varchar,ref_obj.SocialAttendedDate,103) AS SocialAttendedDate
		                        ,ISNULL(ref_obj.SocialAttendedDate,'') AS SocialAttendedDate
		
                                FROM
                                (
			                        SELECT
			                        beneficiarios.personType
			                        ,beneficiarios.FirstName  
			                        ,beneficiarios.LastName
			                        ,beneficiarios.HouseholdName
			                        ,beneficiarios.ChiefPartner
			                        ,beneficiarios.Partner
			                        ,beneficiarios.Age
			                        ,beneficiarios.DateOFBirth
			                        ,beneficiarios.DateOfBirthUnknown
			                        ,beneficiarios.Gender
			                        ,beneficiarios.District
			                        ,beneficiarios.AdministrativePost
			                        ,beneficiarios.NeighborhoodName
			                        ,beneficiarios.Block
			                        ,beneficiarios.ClosePlaceToHome
			                        ,beneficiarios.PrincipalChiefName
			                        ,beneficiarios.FamilyHeadDescription
			                        ,beneficiarios.RegistrationDate
			                        ,beneficiarios.InstitutionalAid
			                        ,beneficiarios.InstitutionalAidDetail
			                        ,beneficiarios.communityAid
			                        ,beneficiarios.communityAidDetail
			                        ,beneficiarios.individualAid
			                        ,beneficiarios.FamilyPhoneNumber
			                        ,beneficiarios.AnyoneBedridden
			                        ,beneficiarios.FamilyOriginReference
			                        ,beneficiarios.OtherFamilyOriginRef
			                        ,beneficiarios.ovcDescription
			                        ,beneficiarios.degreeOfKingshipDescription
			                        ,beneficiarios.IsPartSavingGroup
			                        ,beneficiarios.HIVStatus
			                        ,beneficiarios.HIVStatusDetails
			                        ,beneficiarios.NID
			                        ,beneficiarios.ChildStatusDescription
			                        FROM
                                    (
                                    SELECT	
			                                    'CRIANÇA' as personType
			                                    --,(c.FirstName) + ' ' + (c.LastName) As FullName
			                                    ,c.FirstName
			                                    ,c.LastName
			                                    ,hh.HouseholdName
			                                    ,cp.Name AS ChiefPartner
			                                    ,p.Name AS Partner
			                                    ,DATEDIFF(YEAR, c.DateOFBirth, GETDATE()) AS Age
			                                    ,c.DateOFBirth
			                                    ,(CASE WHEN c.DateOfBirthUnknown = 1 THEN 'DATA DESCONHECIDA' 
			                                    WHEN c.DateOfBirthUnknown = 0 THEN '' END) as DateOfBirthUnknown
			                                    ,c.Gender AS Gender
			                                    ,(CASE ounitparent.OrgUnitTypeID WHEN 3 THEN ounitparent.Name END) as District
			                                    ,(CASE ounit.OrgUnitTypeID WHEN 4 THEN ounit.Name END) as AdministrativePost
			                                    ,hh.NeighborhoodName
			                                    ,hh.Block
			                                    ,hh.ClosePlaceToHome
			                                    ,hh.PrincipalChiefName
			                                    ,familyhead.Description as FamilyHeadDescription
			                                    ,hh.RegistrationDate
			                                    ,(CASE aid.InstitutionalAid WHEN 0 THEN 'NÃO' WHEN 1 THEN 'SIM' END) as InstitutionalAid
			                                    ,aid.InstitutionalAidDetail
			                                    ,(CASE aid.communityAid	WHEN 0 THEN 'NÃO' WHEN 1 THEN 'SIM' END) as communityAid
			                                    ,aid.communityAidDetail
			                                    ,(CASE aid.individualAid WHEN 0 THEN 'NÃO' WHEN 1 THEN 'SIM' END) as individualAid
			                                    ,hh.FamilyPhoneNumber
			                                    ,(CASE hh.AnyoneBedridden WHEN 0 THEN 'NÃO' WHEN 1 THEN 'SIM' END) as AnyoneBedridden
			                                    ,familyorigin.Description as FamilyOriginReference
			                                    ,hh.OtherFamilyOriginRef
			                                    ,ovc.Description as ovcDescription
			                                    ,degreeofkingship.Description as degreeOfKingshipDescription
			                                    ,(CASE c.IsPartSavingGroup WHEN 0 THEN 'NÃO'  WHEN 1 THEN 'SIM' END) as IsPartSavingGroup
			                                    ,(CASE hiv.HIV WHEN 'P' THEN 'POSITIVO' WHEN 'N' THEN 'NEGATIVO'WHEN 'U' THEN 'DESCONHECIDO'END) as HIVStatus
			                                    ,(CASE  
				                                    WHEN hiv.HIV='P' AND hiv.HIVInTreatment = 0 THEN 'ESTÁ EM TARV'
				                                    WHEN hiv.HIV='P' AND hiv.HIVInTreatment=1 THEN 'NÃO ESTÁ EM TARV'
				                                    WHEN hiv.HIV='U' AND hiv.HIVUndisclosedReason=0 THEN 'NÃO REVELADO'
				                                    WHEN hiv.HIV='U' AND hiv.HIVUndisclosedReason=1 THEN 'NÃO CONHECE' ELSE '' 
				                                    END) as HIVStatusDetails
			                                    --,HIVStatusDetails = 'CCR' AND rt.ReferenceCategory in ('Health','Social') AND r.Value = 1 THEN 1 ELSE 0 END), -1) As RC_CCR
			                                    --(CASE WHEN rt.ReferenceName = 'CCR' AND rt.ReferenceCategory in ('Health','Social') AND r.Value = 1 THEN 1 ELSE 0 END), -1) As RC_CCR
			                                    ,c.NID
			                                    ,MAX(cs.Description) As ChildStatusDescription
                                    FROM 
			                                     [Partner] AS cp
	                                    LEFT JOIN 
			                                     [Partner] AS p ON cp.PartnerID = p.SuperiorID 
	                                    LEFT JOIN
			                                     [HouseHold] AS hh ON hh.PartnerID = p.PartnerID
	                                    LEFT JOIN
			                                     [Child] AS c ON hh.HouseHoldID = c.HouseholdID
	                                    LEFT JOIN 
			                                     [OrgUnit] AS ounit ON ounit.OrgUnitID = hh.OrgUnitID 
	                                    LEFT JOIN
			                                     [OrgUnitType] AS outype ON outype.OrgUnitTypeID = ounit.OrgUnitTypeID
	                                    LEFT JOIN
			                                     [OrgUnit] AS ounitparent ON ounit.ParentOrgUnitId = ounitparent.OrgUnitID
	                                    LEFT JOIN
			                                     [OrgUnit] AS ounitparent2 ON ounitparent.ParentOrgUnitId = ounitparent2.OrgUnitID
	                                    LEFT JOIN
			                                     Aid AS aid ON aid.AidID = hh.AidID
	                                    LEFT JOIN
			                                     SimpleEntity AS familyorigin ON hh.[FamilyOriginRefID] = familyorigin.SimpleEntityID
	                                    LEFT JOIN
			                                     SimpleEntity AS familyhead ON hh.FamilyHeadID = familyhead.SimpleEntityID
	                                    LEFT JOIN
			                                     [OVCType] AS ovc ON c.OVCTypeID = ovc.OVCTypeID
	                                    LEFT JOIN
			                                     SimpleEntity AS degreeofkingship ON c.KinshipToFamilyHeadID = degreeofkingship.SimpleEntityID
	                                    LEFT JOIN
			                                     [HIVStatus] AS hiv ON hiv.HIVStatusID = c.HIVStatusID
	                                    LEFT JOIN 
		                                     [ChildStatusHistory] csh on  (csh.ChildID = c.ChildID)
	                                    LEFT JOIN 
		                                     [ChildStatus] cs on  (cs.StatusID = csh.ChildStatusID)
                                    WHERE
		                                    cp.CollaboratorRoleID = 2 AND c.HouseholdID IS NOT NULL
		                                    AND hh.RegistrationDate >= @initialDate AND hh.RegistrationDate <= @lastDate
		                                    --AND hh.RegistrationDate >= '2017/08/01' AND hh.RegistrationDate <= '2017/09/28'
                                    GROUP BY 
		                                    cp.Name
		                                    ,hh.HouseholdName
		                                    ,p.Name
		                                    ,c.FirstNamE
		                                    ,c.LastName
		                                    ,c.DateOfBirth
		                                    ,C.DateOfBirthUnknown
		                                    ,c.gender
		                                    ,ounit.Name
		                                    ,ounit.OrgUnitTypeID
		                                    ,ounitparent.Name
		                                    ,ounitparent.OrgUnitTypeID
		                                    ,hh.NeighborhoodName
		                                    ,hh.Block
		                                    ,hh.ClosePlaceToHome
		                                    ,hh.[PrincipalChiefName]
		                                    ,familyhead.Description
		                                    ,hh.RegistrationDate
		                                    ,aid.InstitutionalAid
		                                    ,aid.InstitutionalAidDetail
		                                    ,aid.communityAid
		                                    ,aid.communityAidDetail
		                                    ,aid.individualAid
		                                    ,hh.FamilyPhoneNumber
		                                    ,hh.AnyoneBedridden
		                                    ,familyorigin.Description
		                                    ,hh.OtherFamilyOriginRef
		                                    ,ovc.Description
		                                    ,degreeofkingship.Description
		                                    ,c.IsPartSavingGroup
		                                    ,hiv.HIV
		                                    ,hiv.HIVInTreatment
		                                    ,hiv.HIVUndisclosedReason
		                                    ,c.NID
		                                    ,cs.Description

                                    UNION ALL

                                    SELECT	
			                                    'ADULTO' as personType
			                                    --,(c.FirstName) + ' ' + (c.LastName) As FullName
			                                    ,a.FirstName
			                                    ,a.LastName
			                                    ,hh.HouseholdName
			                                    ,cp.Name AS ChiefPartner
			                                    ,p.Name AS Partner
			                                    ,DATEDIFF(YEAR, a.DateOFBirth, GETDATE()) AS Age
			                                    ,a.DateOFBirth
			                                    ,'' as DateOfBirthUnknown
			                                    ,a.Gender AS Gender
			                                    ,(CASE ounitparent.OrgUnitTypeID WHEN 3 THEN ounitparent.Name END) as District
			                                    ,(CASE ounit.OrgUnitTypeID WHEN 4 THEN ounit.Name END) as AdministrativePost
			                                    ,hh.NeighborhoodName
			                                    ,hh.Block
			                                    ,hh.ClosePlaceToHome
			                                    ,hh.PrincipalChiefName
			                                    ,familyhead.Description as FamilyHeadDescription
			                                    ,hh.RegistrationDate
			                                    ,(CASE aid.InstitutionalAid WHEN 0 THEN 'NÃO' WHEN 1 THEN 'SIM' END) as InstitutionalAid
			                                    ,aid.InstitutionalAidDetail
			                                    ,(CASE aid.communityAid	WHEN 0 THEN 'NÃO' WHEN 1 THEN 'SIM' END) as communityAid
			                                    ,aid.communityAidDetail
			                                    ,(CASE aid.individualAid WHEN 0 THEN 'NÃO' WHEN 1 THEN 'SIM' END) as individualAid
			                                    ,hh.FamilyPhoneNumber
			                                    ,(CASE hh.AnyoneBedridden	WHEN 0 THEN 'NÃO' WHEN 1 THEN 'SIM' END) as AnyoneBedridden
			                                    ,familyorigin.Description as FamilyOriginReference
			                                    ,hh.OtherFamilyOriginRef
			                                    ,NULL as ovcDescription
			                                    ,degreeofkingship.Description as degreeOfKingshipDescription
			                                    ,(CASE a.IsPartSavingGroup WHEN 0 THEN 'NÃO'  WHEN 1 THEN 'SIM' END) as IsPartSavingGroup
			                                    ,(CASE hiv.HIV WHEN 'P' THEN 'POSITIVO' WHEN 'N' THEN 'NEGATIVO'WHEN 'U' THEN 'DESCONHECIDO'END) as HIVStatus
			                                    ,(CASE  
				                                    WHEN hiv.HIV='P' AND hiv.HIVInTreatment = 0 THEN 'ESTÁ EM TARV'
				                                    WHEN hiv.HIV='P' AND hiv.HIVInTreatment=1 THEN 'NÃO ESTÁ EM TARV'
				                                    WHEN hiv.HIV='U' AND hiv.HIVUndisclosedReason=0 THEN 'NÃO REVELADO'
				                                    WHEN hiv.HIV='U' AND hiv.HIVUndisclosedReason=1 THEN 'NÃO CONHECE' ELSE '' 
				                                    END) as HIVStatusDetails
			                                    ,a.NID
			                                    ,'' As ChildStatusDescription
                                    FROM 
			                                     [Partner] AS cp
	                                    LEFT JOIN 
			                                     [Partner] AS p ON cp.PartnerID = p.SuperiorID 
	                                    LEFT JOIN
			                                     [HouseHold] AS hh ON hh.PartnerID = p.PartnerID
	                                    LEFT JOIN
			                                     [Adult] AS a ON hh.HouseHoldID = a.HouseholdID
	                                    LEFT JOIN 
			                                     [OrgUnit] AS ounit ON ounit.OrgUnitID = hh.OrgUnitID 
	                                    LEFT JOIN
			                                     [OrgUnitType] AS outype ON outype.OrgUnitTypeID = ounit.OrgUnitTypeID
	                                    LEFT JOIN
			                                     [OrgUnit] AS ounitparent ON ounit.ParentOrgUnitId = ounitparent.OrgUnitID
	                                    LEFT JOIN
			                                     [OrgUnit] AS ounitparent2 ON ounitparent.ParentOrgUnitId = ounitparent2.OrgUnitID
	                                    LEFT JOIN
			                                     Aid AS aid ON aid.AidID = hh.AidID
	                                    LEFT JOIN
			                                     SimpleEntity AS familyorigin ON hh.[FamilyOriginRefID] = familyorigin.SimpleEntityID
	                                    LEFT JOIN
			                                     SimpleEntity AS familyhead ON hh.FamilyHeadID = familyhead.SimpleEntityID
	                                    --LEFT JOIN
			                                    -- [OVCType] AS ovc ON a.OVCTypeID = ova.OVCTypeID
	                                    LEFT JOIN
			                                     SimpleEntity AS degreeofkingship ON a.KinshipToFamilyHeadID = degreeofkingship.SimpleEntityID
	                                    LEFT JOIN
			                                     [HIVStatus] AS hiv ON hiv.HIVStatusID = a.HIVStatusID
                                    WHERE
		                                    cp.CollaboratorRoleID = 2 AND a.HouseholdID IS NOT NULL
		                                    AND hh.RegistrationDate >= @initialDate AND hh.RegistrationDate <= @lastDate
		                                    --AND hh.RegistrationDate >= '2017/08/01' AND hh.RegistrationDate <= '2017/09/28'
	                                    GROUP BY 
		                                    cp.Name
		                                    ,hh.HouseholdName
		                                    ,p.Name
		                                    ,a.FirstName
		                                    ,a.LastName
		                                    ,a.DateOfBirth
		                                    ,a.gender
		                                    ,ounit.Name
		                                    ,ounit.OrgUnitTypeID
		                                    ,ounitparent.Name
		                                    ,ounitparent.OrgUnitTypeID
		                                    ,hh.NeighborhoodName
		                                    ,hh.Block
		                                    ,hh.ClosePlaceToHome
		                                    ,hh.[PrincipalChiefName]
		                                    ,familyhead.Description
		                                    ,hh.RegistrationDate
		                                    ,aid.InstitutionalAid 
		                                    ,aid.InstitutionalAidDetail
		                                    ,aid.communityAid
		                                    ,aid.communityAidDetail
		                                    ,aid.individualAid
		                                    ,hh.FamilyPhoneNumber
		                                    ,hh.AnyoneBedridden
		                                    ,familyorigin.Description
		                                    ,hh.OtherFamilyOriginRef
		                                    --,ova.Description
		                                    ,degreeofkingship.Description
		                                    ,a.IsPartSavingGroup
		                                    ,hiv.HIV
		                                    ,hiv.HIV
		                                    ,hiv.HIVInTreatment
		                                    ,hiv.HIVUndisclosedReason
		                                    ,a.NID
                                    )beneficiarios 
                                    GROUP BY
	                                    beneficiarios.personType
	                                    ,beneficiarios.FirstName
	                                    ,beneficiarios.LastName
	                                    ,beneficiarios.HouseholdName
	                                    ,beneficiarios.ChiefPartner
	                                    ,beneficiarios.Partner
	                                    ,beneficiarios.Age
	                                    ,beneficiarios.DateOFBirth
	                                    ,beneficiarios.DateOfBirthUnknown
	                                    ,beneficiarios.Gender
	                                    ,beneficiarios.District
	                                    ,beneficiarios.AdministrativePost
	                                    ,beneficiarios.NeighborhoodName
	                                    ,beneficiarios.Block
	                                    ,beneficiarios.ClosePlaceToHome
	                                    ,beneficiarios.PrincipalChiefName
	                                    ,beneficiarios.FamilyHeadDescription
	                                    ,beneficiarios.RegistrationDate
	                                    ,beneficiarios.InstitutionalAid
	                                    ,beneficiarios.InstitutionalAidDetail
	                                    ,beneficiarios.communityAid
	                                    ,beneficiarios.communityAidDetail
	                                    ,beneficiarios.individualAid
	                                    ,beneficiarios.FamilyPhoneNumber
	                                    ,beneficiarios.AnyoneBedridden
	                                    ,beneficiarios.FamilyOriginReference
	                                    ,beneficiarios.OtherFamilyOriginRef
	                                    ,beneficiarios.ovcDescription
	                                    ,beneficiarios.degreeOfKingshipDescription
	                                    ,beneficiarios.IsPartSavingGroup
	                                    ,beneficiarios.HIVStatus
	                                    ,beneficiarios.HIVStatusDetails
	                                    ,beneficiarios.NID
	                                    ,beneficiarios.ChildStatusDescription	
                                )agregado_obj
                                LEFT JOIN
                                (
                                    SELECT 
	                                    --,(c.FirstName) + ' ' + (c.LastName) As FullName
	                                    c.FirstName
	                                    ,c.LastName
	                                    ,ISNULL(MAX(CASE WHEN SUBSTRING (question.Description,1,3)='1-' THEN ScoreType.Score  END), -1) As P1
	                                    ,ISNULL(MAX(CASE WHEN SUBSTRING (question.Description,1,3)='2-' THEN ScoreType.Score  END), -1) As P2
	                                    ,ISNULL(MAX(CASE WHEN SUBSTRING (question.Description,1,3)='3-' THEN ScoreType.Score  END), -1) As P3
	                                    ,ISNULL(MAX(CASE WHEN SUBSTRING (question.Description,1,3)='4-' THEN ScoreType.Score  END), -1) As P4
	                                    ,ISNULL(MAX(CASE WHEN SUBSTRING (question.Description,1,3)='5-' THEN ScoreType.Score  END), -1) As P5
	                                    ,ISNULL(MAX(CASE WHEN SUBSTRING (question.Description,1,3)='6-' THEN ScoreType.Score  END), -1) As P6
	                                    ,ISNULL(MAX(CASE WHEN SUBSTRING (question.Description,1,3)='7-' THEN ScoreType.Score  END), -1) As P7
	                                    ,ISNULL(MAX(CASE WHEN SUBSTRING (question.Description,1,3)='8-' THEN ScoreType.Score  END), -1) As P8
	                                    ,ISNULL(MAX(CASE WHEN SUBSTRING (question.Description,1,3)='9-' THEN ScoreType.Score  END), -1) As P9
	                                    ,ISNULL(MAX(CASE WHEN SUBSTRING (question.Description,1,3)='10-' THEN ScoreType.Score  END), -1) As P10
	                                    ,ISNULL(MAX(CASE WHEN SUBSTRING (question.Description,1,3)='11-' THEN ScoreType.Score  END), -1) As P11
	                                    ,ISNULL(MAX(CASE WHEN SUBSTRING (question.Description,1,3)='12-' THEN ScoreType.Score  END), -1) As P12
	                                    ,ISNULL(MAX(CASE WHEN SUBSTRING (question.Description,1,3)='13-' THEN ScoreType.Score  END), -1) As P13
	                                    ,ISNULL(MAX(CASE WHEN SUBSTRING (question.Description,1,3)='14-' THEN ScoreType.Score  END), -1) As P14
	                                    ,ISNULL(MAX(CASE WHEN SUBSTRING (question.Description,1,3)='15-' THEN ScoreType.Score  END), -1) As P15
	                                    ,ISNULL(MAX(CASE WHEN SUBSTRING (question.Description,1,3)='16-' THEN ScoreType.Score  END), -1) As P16
	                                    ,ISNULL(MAX(CASE WHEN SUBSTRING (question.Description,1,3)='17-' THEN ScoreType.Score  END), -1) As P17
	                                    ,ISNULL(MAX(CASE WHEN SUBSTRING (question.Description,1,3)='18-' THEN ScoreType.Score  END), -1) As P18
	                                    ,ISNULL(MAX(CASE WHEN SUBSTRING (question.Description,1,3)='19-' THEN ScoreType.Score  END), -1) As P19
	                                    ,ISNULL(MAX(CASE WHEN SUBSTRING (question.Description,1,3)='20-' THEN ScoreType.Score  END), -1) As P20
	                                    ,ISNULL(MAX(CASE WHEN SUBSTRING (question.Description,1,3)='21-' THEN ScoreType.Score  END), -1) As P21
	                                    ,ISNULL(MAX(CASE WHEN SUBSTRING (question.Description,1,3)='22-' THEN ScoreType.Score  END), -1) As P22
	                                    ,ISNULL(MAX(CASE WHEN SUBSTRING (question.Description,1,3)='23-' THEN ScoreType.Score  END), -1) As P23
	                                    ,ISNULL(MAX(CASE WHEN SUBSTRING (question.Description,1,3)='24-' THEN ScoreType.Score  END), -1) As P24
	                                    ,ISNULL(MAX(CASE WHEN SUBSTRING (question.Description,1,3)='25-' THEN ScoreType.Score  END), -1) As P25
	                                    ,ISNULL(MAX(CASE WHEN SUBSTRING (question.Description,1,3)='26-' THEN ScoreType.Score  END), -1) As P26
	                                    ,ISNULL(MAX(CASE WHEN SUBSTRING (question.Description,1,3)='27-' THEN ScoreType.Score  END), -1) As P27
	                                    ,ISNULL(MAX(CASE WHEN SUBSTRING (question.Description,1,3)='28-' THEN ScoreType.Score  END), -1) As P28
	                                    ,ISNULL(MAX(CASE WHEN SUBSTRING (question.Description,1,3)='29-' THEN ScoreType.Score  END), -1) As P29
	                                    ,ISNULL(MAX(CASE WHEN SUBSTRING (question.Description,1,3)='30-' THEN ScoreType.Score  END), -1) As P30
	                                    ,ISNULL(MAX(CASE WHEN SUBSTRING (question.Description,1,3)='31-' THEN ScoreType.Score  END), -1) As P31
	                                    ,ISNULL(MAX(CASE WHEN SUBSTRING (question.Description,1,3)='32-' THEN ScoreType.Score  END), -1) As P32
	                                    ,ISNULL(MAX(CASE WHEN SUBSTRING (question.Description,1,3)='33-' THEN ScoreType.Score  END), -1) As P33
                                    FROM 
	                                     [Partner] AS cp
                                    LEFT JOIN 
	                                     [Partner] AS p ON cp.PartnerID = p.SuperiorID 
                                    LEFT JOIN
	                                     [HouseHold] AS hh ON hh.PartnerID = p.PartnerID
                                    LEFT JOIN
	                                     [Child] AS c ON hh.HouseHoldID = c.HouseholdID 
                                    LEFT JOIN 
                                     CSI AS csi ON c.ChildID = csi.ChildID 
                                    LEFT JOIN 
                                     CSIDomain AS csidomain ON csi.CSIID = csidomain.CSIID
                                    LEFT JOIN 
                                     Domain AS domain ON domain.DomainID = csidomain.DomainID
                                    LEFT JOIN 
                                     CSIDomainScore AS csidomainscore ON csidomain.CSIDomainID = csidomainscore.CSIDomainID
                                    LEFT JOIN 
                                     Question AS question ON csidomainscore.QuestionID = question.QuestionID
                                    LEFT JOIN 
                                     Answer AS answer ON csidomainscore.AnswerID = answer.AnswerID
                                    LEFT JOIN 
                                     ScoreType AS scoretype ON scoretype.ScoreTypeID = answer.ScoreID
                                    WHERE
                                    cp.CollaboratorRoleID = 2 AND c.HouseholdID IS NOT NULL
                                    AND csi.IndexDate >= @initialDate AND csi.IndexDate <= @lastDate
                                    --AND csi.IndexDate >= '2017/08/01' AND csi.IndexDate <= '2017/09/28'
                                    GROUP BY 
                                    c.FirstName
                                    ,c.LastName
                                )mac_obj
                                ON
                                (
                                    agregado_obj.FirstName = mac_obj.FirstName
                                    AND
                                    agregado_obj.LastName = mac_obj.LastName

                                )
                                LEFT JOIN
                                (
                                    SELECT 
                                    --,(c.FirstName) + ' ' + (c.LastName) As FullName
                                    c.FirstName
                                    ,c.LastName	
                                    ,ISNULL(MAX(CASE WHEN SUBSTRING (question.Description,1,3)='1-' THEN cplandomainss.Description  END), '') As A1
                                    ,ISNULL(MAX(CASE WHEN SUBSTRING (question.Description,1,3)='2-' THEN cplandomainss.Description  END), '') As A2
                                    ,ISNULL(MAX(CASE WHEN SUBSTRING (question.Description,1,3)='3-' THEN cplandomainss.Description  END), '') As A3
                                    ,ISNULL(MAX(CASE WHEN SUBSTRING (question.Description,1,3)='4-' THEN cplandomainss.Description  END), '') As A4
                                    ,ISNULL(MAX(CASE WHEN SUBSTRING (question.Description,1,3)='5-' THEN cplandomainss.Description  END), '') As A5
                                    ,ISNULL(MAX(CASE WHEN SUBSTRING (question.Description,1,3)='6-' THEN cplandomainss.Description  END), '') As A6
                                    ,ISNULL(MAX(CASE WHEN SUBSTRING (question.Description,1,3)='7-' THEN cplandomainss.Description  END), '') As A7
                                    ,ISNULL(MAX(CASE WHEN SUBSTRING (question.Description,1,3)='8-' THEN cplandomainss.Description  END), '') As A8
                                    ,ISNULL(MAX(CASE WHEN SUBSTRING (question.Description,1,3)='9-' THEN cplandomainss.Description  END), '') As A9
                                    ,ISNULL(MAX(CASE WHEN SUBSTRING (question.Description,1,3)='10-' THEN cplandomainss.Description  END), '') As A10
                                    ,ISNULL(MAX(CASE WHEN SUBSTRING (question.Description,1,3)='11-' THEN cplandomainss.Description  END), '') As A11
                                    ,ISNULL(MAX(CASE WHEN SUBSTRING (question.Description,1,3)='12-' THEN cplandomainss.Description  END), '') As A12
                                    ,ISNULL(MAX(CASE WHEN SUBSTRING (question.Description,1,3)='13-' THEN cplandomainss.Description  END), '') As A13
                                    ,ISNULL(MAX(CASE WHEN SUBSTRING (question.Description,1,3)='14-' THEN cplandomainss.Description  END), '') As A14
                                    ,ISNULL(MAX(CASE WHEN SUBSTRING (question.Description,1,3)='15-' THEN cplandomainss.Description  END), '') As A15
                                    ,ISNULL(MAX(CASE WHEN SUBSTRING (question.Description,1,3)='16-' THEN cplandomainss.Description  END), '') As A16
                                    ,ISNULL(MAX(CASE WHEN SUBSTRING (question.Description,1,3)='17-' THEN cplandomainss.Description  END), '') As A17
                                    ,ISNULL(MAX(CASE WHEN SUBSTRING (question.Description,1,3)='18-' THEN cplandomainss.Description  END), '') As A18
                                    ,ISNULL(MAX(CASE WHEN SUBSTRING (question.Description,1,3)='19-' THEN cplandomainss.Description  END), '') As A19
                                    ,ISNULL(MAX(CASE WHEN SUBSTRING (question.Description,1,3)='20-' THEN cplandomainss.Description  END), '') As A20
                                    ,ISNULL(MAX(CASE WHEN SUBSTRING (question.Description,1,3)='21-' THEN cplandomainss.Description  END), '') As A21
                                    ,ISNULL(MAX(CASE WHEN SUBSTRING (question.Description,1,3)='22-' THEN cplandomainss.Description  END), '') As A22
                                    ,ISNULL(MAX(CASE WHEN SUBSTRING (question.Description,1,3)='23-' THEN cplandomainss.Description  END), '') As A23
                                    ,ISNULL(MAX(CASE WHEN SUBSTRING (question.Description,1,3)='24-' THEN cplandomainss.Description  END), '') As A24
                                    ,ISNULL(MAX(CASE WHEN SUBSTRING (question.Description,1,3)='25-' THEN cplandomainss.Description  END), '') As A25
                                    ,ISNULL(MAX(CASE WHEN SUBSTRING (question.Description,1,3)='26-' THEN cplandomainss.Description  END), '') As A26
                                    ,ISNULL(MAX(CASE WHEN SUBSTRING (question.Description,1,3)='27-' THEN cplandomainss.Description  END), '') As A27
                                    ,ISNULL(MAX(CASE WHEN SUBSTRING (question.Description,1,3)='28-' THEN cplandomainss.Description  END), '') As A28
                                    ,ISNULL(MAX(CASE WHEN SUBSTRING (question.Description,1,3)='29-' THEN cplandomainss.Description  END), '') As A29
                                    ,ISNULL(MAX(CASE WHEN SUBSTRING (question.Description,1,3)='30-' THEN cplandomainss.Description  END), '') As A30
                                    ,ISNULL(MAX(CASE WHEN SUBSTRING (question.Description,1,3)='31-' THEN cplandomainss.Description  END), '') As A31
                                    ,ISNULL(MAX(CASE WHEN SUBSTRING (question.Description,1,3)='32-' THEN cplandomainss.Description  END), '') As A32
                                    ,ISNULL(MAX(CASE WHEN SUBSTRING (question.Description,1,3)='33-' THEN cplandomainss.Description  END), '') As A33	FROM 
		                                     [Partner] AS cp
                                    LEFT JOIN 
		                                     [Partner] AS p ON cp.PartnerID = p.SuperiorID 
                                    LEFT JOIN
		                                     [HouseHold] AS hh ON hh.PartnerID = p.PartnerID
                                    LEFT JOIN
		                                     [Child] AS c ON hh.HouseHoldID = c.HouseholdID 
                                    LEFT JOIN 
	                                     CSI AS csi ON c.ChildID = csi.ChildID 
                                    LEFT JOIN 
	                                     CSIDomain AS csidomain ON csi.CSIID = csidomain.CSIID
                                    LEFT JOIN 
	                                     Domain AS domain ON domain.DomainID = csidomain.DomainID
                                    LEFT JOIN 
	                                     CSIDomainScore AS csidomainscore ON csidomain.CSIDomainID = csidomainscore.CSIDomainID
                                    LEFT JOIN 
	                                     Question AS question ON csidomainscore.QuestionID = question.QuestionID
                                    LEFT JOIN 
	                                     Answer AS answer ON csidomainscore.AnswerID = answer.AnswerID
                                    LEFT JOIN 
	                                     ScoreType AS scoretype ON scoretype.ScoreTypeID = answer.ScoreID
                                    LEFT JOIN 
	                                     CarePlan AS cplan ON cplan.CSIID = csi.CSIID
                                    LEFT JOIN
	                                     [CarePlanDomain] AS cplandomain ON cplandomain.CarePlanID = cplan.CarePlanID
                                    LEFT JOIN
	                                     [CarePlanDomainSupportService] AS cplandomainss ON cplandomainss.CarePlanDomainID = cplandomain.CarePlanDomainID
	                                    AND cplandomainss.QuestionID = question.QuestionID
                                    WHERE
	                                    cp.CollaboratorRoleID = 2 AND c.HouseholdID IS NOT NULL
	                                    AND cplan.CarePlanDate >= @initialDate AND cplan.CarePlanDate <= @lastDate
	                                    --AND cplan.CreatedDate >= '2017/08/01' AND cplan.CreatedDate <= '2017/09/28'
                                    GROUP BY 
	                                    c.FirstName
	                                    ,c.LastName
                                )pAccao_obj
                                ON
                                (
                                    mac_obj.FirstName = pAccao_obj.FirstName
                                    AND
                                    mac_obj.LastName = pAccao_obj.LastName
                                )
		                        LEFT JOIN
                                (
                                    SELECT
                                    --,(c.FirstName) + ' ' + (c.LastName) As FullName
                                    c.FirstName
                                    ,c.LastName
                                    --,MAX(cs.Description) As ChildStatus
			                        ,ISNULL(MAX(CASE WHEN rv.FirstTimeSavingGroupMember = 1  THEN 'SIM' else 'NÃO' END), '') As FirstTimeSavingGroup
                                    ,ISNULL(MAX(CASE WHEN rvs.SupportType = 'Domain' AND rvs.SupportID = 6 THEN rvs.SupportValue END), '') As FE
                                    ,ISNULL(MAX(CASE WHEN rvs.SupportType = 'Domain' AND rvs.SupportID = 1 THEN rvs.SupportValue END), '') As AN
                                    ,ISNULL(MAX(CASE WHEN rvs.SupportType = 'Domain' AND rvs.SupportID = 7 THEN rvs.SupportValue END), '') As HAB
                                    ,ISNULL(MAX(CASE WHEN rvs.SupportType = 'Domain' AND rvs.SupportID = 2 THEN rvs.SupportValue END), '') As ED
                                    ,ISNULL(MAX(CASE WHEN rvs.SupportType = 'Domain' AND rvs.SupportID = 3 THEN rvs.SupportValue END), '') As SD
                                    ,ISNULL(MAX(CASE WHEN rvs.SupportType = 'Domain' AND rvs.SupportID = 5 THEN rvs.SupportValue END), '') As APS
                                    ,ISNULL(MAX(CASE WHEN rvs.SupportType = 'Domain' AND rvs.SupportID = 4 THEN rvs.SupportValue END), '') As PL
                                    ,ISNULL(MAX(CASE WHEN rvs.SupportType = 'DPI' AND rvs.SupportID = 1 THEN rvs.SupportValue END), '') As DPI
                                    ,ISNULL(MAX(CASE WHEN rvs.SupportType = 'MUAC' AND rvs.SupportID = 1 THEN  rvs.SupportValue END), '') As MUACGREEN
                                    ,ISNULL(MAX(CASE WHEN rvs.SupportType = 'MUAC' AND rvs.SupportID = 2 THEN  rvs.SupportValue END), '') As MUACYELLOW
                                    ,ISNULL(MAX(CASE WHEN rvs.SupportType = 'MUAC' AND rvs.SupportID = 3 THEN  rvs.SupportValue END), '') As MUACRED

                                    FROM 	 [Partner] AS cp
	                                    LEFT JOIN 
		                                     [Partner] AS p ON cp.PartnerID = p.SuperiorID
	                                    LEFT JOIN 
		                                     [HouseHold] hh on (p.PartnerID = hh.PartnerID)
	                                    LEFT JOIN 
		                                     [RoutineVisit] rv on (hh.HouseHoldID = rv.HouseholdID)
	                                    LEFT JOIN 
		                                     [RoutineVisitMember] rvm on (rvm.RoutineVisitID = rv.RoutineVisitID)
	                                    LEFT JOIN 
		                                     Child c on (c.ChildID = rvm.ChildID)
	                                    --LEFT JOIN 
		                                   --  [ReferenceService] rs on (rs.RoutineVisitMemberID = rvm.RoutineVisitMemberID)
	                                    LEFT JOIN 
		                                     [RoutineVisitSupport] rvs on (rvs.RoutineVisitMemberID = rvm.RoutineVisitMemberID)
	                                    LEFT JOIN
		                                     [Domain] dom on dom.[DomainID] = rvs.SupportID
	                                    --LEFT JOIN 
		                                --     [Reference] r on (r.ReferenceServiceID = rs.ReferenceServiceID)
	                                    --LEFT JOIN 
		                                   -- [ReferenceType] rt on (rt.ReferenceTypeID = r.ReferenceTypeID)
                                    WHERE 
	                                    cp.CollaboratorRoleID = 2  AND c.HouseholdID IS NOT NULL
	                                    --AND rt.FieldType = 'CheckBox' AND rvm.ChildID IS NOT NULL
	                                    --AND rv.RoutineVisitDate >= @initialDate AND rv.RoutineVisitDate <= @lastDate
	                                    --AND rs.ReferenceDate >= @initialDate AND rs.ReferenceDate  <= @lastDate
	                                    --AND rs.HealthAttendedDate >= @initialDate AND rs.HealthAttendedDate  <= @lastDate
                                    group by
	                                    c.FirstName, c.LastName--, rs.ReferenceDate, rs.HealthAttendedDate, rs.SocialAttendedDate
                                )fichaseguimento_obj
                                ON
                                (
                                    pAccao_obj.FirstName = fichaseguimento_obj.FirstName
                                    AND
                                    pAccao_obj.LastName = fichaseguimento_obj.LastName
                                )

                                LEFT JOIN
                                (
                                    SELECT
                                    c.FirstName
                                    ,c.LastName
                                    ,ISNULL(MAX(CASE WHEN rt.ReferenceName = 'ATS' AND rt.ReferenceCategory = 'Activist' AND r.Value = 1 THEN 'SIM' ELSE 'NÃO' END), '')  As ATS
                                    ,ISNULL(MAX(CASE WHEN rt.ReferenceName = 'TARV' AND rt.ReferenceCategory = 'Activist' AND r.Value = 1 THEN 'SIM' ELSE 'NÃO' END), '')  As TARV
                                    ,ISNULL(MAX(CASE WHEN rt.ReferenceName = 'CCR' AND rt.ReferenceCategory = 'Activist' AND r.Value = 1 THEN 'SIM' ELSE 'NÃO' END), '')  As CCR
                                    ,ISNULL(MAX(CASE WHEN rt.ReferenceName in ('Consulta Pós-Parto','CPN','PTV') AND rt.ReferenceCategory = 'Activist' AND r.Value = 1 THEN 'SIM' ELSE 'NÃO' END), '')  As SSR
                                    ,ISNULL(MAX(CASE WHEN rt.ReferenceName in ('GAVV','Posto Policial') AND rt.ReferenceCategory = 'Activist' AND r.Value = 1 THEN 'SIM' ELSE 'NÃO' END), '') As VGB
                                    ,ISNULL(MAX(CASE WHEN rt.ReferenceName not in ('ATS','TARV','CCR','Consulta Pós-Parto','CPN','PTV','GAVV','Posto Policial') AND rt.ReferenceCategory = 'Activist' AND r.Value = 1 THEN 'SIM' ELSE 'NÃO' END), '') As Others
                                    ,rs.ReferenceDate
			                        --,(CASE WHEN rs.ReferenceDate >= '2016/09/01' THEN rs.ReferenceDate ELSE NULL END) AS ReferenceDate

                                    ,ISNULL(MAX(CASE WHEN rt.ReferenceName = 'ATS' AND rt.ReferenceCategory in ('Health','Social') AND r.Value = 1 THEN 'SIM' ELSE 'NÃO' END), '') As RC_ATS
                                    ,ISNULL(MAX(CASE WHEN rt.ReferenceName = 'TARV' AND rt.ReferenceCategory in ('Health','Social') AND r.Value = 1 THEN 'SIM' ELSE 'NÃO' END), '') As RC_TARV
                                    ,ISNULL(MAX(CASE WHEN rt.ReferenceName = 'CCR' AND rt.ReferenceCategory in ('Health','Social') AND r.Value = 1 THEN 'SIM' ELSE 'NÃO' END), '') As RC_CCR
                                    ,ISNULL(MAX(CASE WHEN rt.ReferenceName in ('Consulta Pós-Parto','CPN','PTV') AND rt.ReferenceCategory in ('Health','Social') AND r.Value = 1 THEN 'SIM' ELSE 'NÃO' END), '') As RC_SSR
                                    ,ISNULL(MAX(CASE WHEN rt.ReferenceName in ('GAVV','Posto Policial') AND rt.ReferenceCategory in ('Health','Social') AND r.Value = 1 THEN 'SIM' ELSE 'NÃO' END), '') As RC_VGB
                                    ,ISNULL(MAX(CASE WHEN rt.ReferenceName not in ('ATS','TARV','CCR','Consulta Pós-Parto','CPN','PTV','GAVV','Posto Policial') AND rt.ReferenceCategory in ('Health','Social') AND r.Value = 1 THEN 'SIM' ELSE 'NÃO' END), '') As RC_Others
                                    ,rs.HealthAttendedDate
			                        --,(CASE WHEN rs.HealthAttendedDate >= '2016/09/01' THEN rs.HealthAttendedDate ELSE NULL END) AS HealthAttendedDate
                                    ,rs.SocialAttendedDate
			                        --,(CASE WHEN rs.SocialAttendedDate >= '2016/09/01' THEN rs.SocialAttendedDate ELSE NULL END) AS SocialAttendedDate
			
                                    FROM 	 [Partner] AS cp
	                                    LEFT JOIN 
		                                     [Partner] AS p ON cp.PartnerID = p.SuperiorID
	                                    LEFT JOIN 
		                                     [HouseHold] hh on (p.PartnerID = hh.PartnerID)
	                                    LEFT JOIN 
		                                     [RoutineVisit] rv on (hh.HouseHoldID = rv.HouseholdID)
	                                    LEFT JOIN 
		                                     [RoutineVisitMember] rvm on (rvm.RoutineVisitID = rv.RoutineVisitID)
	                                    LEFT JOIN 
		                                     Child c on (c.ChildID = rvm.ChildID)
				                        LEFT JOIN 
		                                     [ReferenceService] rs on (rs.RoutineVisitMemberID = rvm.RoutineVisitMemberID)
	                                    --LEFT JOIN 
		                                    -- [RoutineVisitSupport] rvs on (rvs.RoutineVisitMemberID = rvm.RoutineVisitMemberID)
	                                    --LEFT JOIN
		                                    -- [Domain] dom on dom.[DomainID] = rvs.SupportID
	                                    LEFT JOIN 
		                                     [Reference] r on (r.ReferenceServiceID = rs.ReferenceServiceID)
	                                    LEFT JOIN 
		                                     [ReferenceType] rt on (rt.ReferenceTypeID = r.ReferenceTypeID)
                                    WHERE 
	                                    cp.CollaboratorRoleID = 2  AND c.HouseholdID IS NOT NULL
	                                    AND rt.FieldType = 'CheckBox' --AND rvm.ChildID IS NOT NULL
	                                    --AND rv.RoutineVisitDate >= @initialDate AND rv.RoutineVisitDate <= @lastDate
	                                    --AND rs.ReferenceDate >= @initialDate AND rs.ReferenceDate  <= @lastDate
	                                    --AND rs.HealthAttendedDate >= @initialDate AND rs.HealthAttendedDate  <= @lastDate
                                    group by
	                                    c.FirstName, c.LastName, rs.ReferenceDate, rs.HealthAttendedDate, rs.SocialAttendedDate
                                )ref_obj
                                ON
                                (
                                    fichaseguimento_obj.FirstName = ref_obj.FirstName
                                    AND
                                    fichaseguimento_obj.LastName = ref_obj.LastName
                                )
		
		                        /*LEFT JOIN
                                (
                                    SELECT
                                    c.FirstName
                                    ,c.LastName
                                    ,ISNULL(MAX(CASE WHEN rt.ReferenceName = 'ATS' AND rt.ReferenceCategory in ('Health','Social') AND r.Value = 1 THEN 'SIM' ELSE 'NÃO' END), '') As RC_ATS
                                    ,ISNULL(MAX(CASE WHEN rt.ReferenceName = 'TARV' AND rt.ReferenceCategory in ('Health','Social') AND r.Value = 1 THEN 'SIM' ELSE 'NÃO' END), '') As RC_TARV
                                    ,ISNULL(MAX(CASE WHEN rt.ReferenceName = 'CCR' AND rt.ReferenceCategory in ('Health','Social') AND r.Value = 1 THEN 'SIM' ELSE 'NÃO' END), '') As RC_CCR
                                    ,ISNULL(MAX(CASE WHEN rt.ReferenceName in ('Consulta Pós-Parto','CPN','PTV') AND rt.ReferenceCategory in ('Health','Social') AND r.Value = 1 THEN 'SIM' ELSE 'NÃO' END), '') As RC_SSR
                                    ,ISNULL(MAX(CASE WHEN rt.ReferenceName in ('GAVV','Posto Policial') AND rt.ReferenceCategory in ('Health','Social') AND r.Value = 1 THEN 'SIM' ELSE 'NÃO' END), '') As RC_VGB
                                    ,ISNULL(MAX(CASE WHEN rt.ReferenceName not in ('ATS','TARV','CCR','Consulta Pós-Parto','CPN','PTV','GAVV','Posto Policial') AND rt.ReferenceCategory in ('Health','Social') AND r.Value = 1 THEN 'SIM' ELSE 'NÃO' END), '') As RC_Others
                                    ,rs.HealthAttendedDate
			                        --,(CASE WHEN rs.HealthAttendedDate >= '2016/09/01' THEN rs.HealthAttendedDate ELSE NULL END) AS HealthAttendedDate
                                    ,rs.SocialAttendedDate
			                        --,(CASE WHEN rs.SocialAttendedDate >= '2016/09/01' THEN rs.SocialAttendedDate ELSE NULL END) AS SocialAttendedDate

                                    FROM 	 [Partner] AS cp
	                                    LEFT JOIN 
		                                     [Partner] AS p ON cp.PartnerID = p.SuperiorID
	                                    LEFT JOIN 
		                                     [HouseHold] hh on (p.PartnerID = hh.PartnerID)
	                                    LEFT JOIN 
		                                     [RoutineVisit] rv on (hh.HouseHoldID = rv.HouseholdID)
	                                    LEFT JOIN 
		                                     [RoutineVisitMember] rvm on (rvm.RoutineVisitID = rv.RoutineVisitID)
	                                    LEFT JOIN 
		                                     Child c on (c.ChildID = rvm.ChildID)
				                        LEFT JOIN 
		                                     [ReferenceService] rs on (rs.RoutineVisitMemberID = rvm.RoutineVisitMemberID)
	                                    --LEFT JOIN 
		                                    -- [RoutineVisitSupport] rvs on (rvs.RoutineVisitMemberID = rvm.RoutineVisitMemberID)
	                                    --LEFT JOIN
		                                    -- [Domain] dom on dom.[DomainID] = rvs.SupportID
	                                    LEFT JOIN 
		                                     [Reference] r on (r.ReferenceServiceID = rs.ReferenceServiceID)
	                                    LEFT JOIN 
		                                     [ReferenceType] rt on (rt.ReferenceTypeID = r.ReferenceTypeID)
                                    WHERE 
	                                    cp.CollaboratorRoleID = 2  AND c.HouseholdID IS NOT NULL
	                                    AND rt.FieldType = 'CheckBox' --AND rvm.ChildID IS NOT NULL
	                                    --AND rv.RoutineVisitDate >= @initialDate AND rv.RoutineVisitDate <= @lastDate
	                                    --AND rs.ReferenceDate >= @initialDate AND rs.ReferenceDate  <= @lastDate
	                                    --AND rs.HealthAttendedDate >= @initialDate AND rs.HealthAttendedDate  <= @lastDate
                                    group by
	                                    c.FirstName, c.LastName, rs.ReferenceDate, rs.HealthAttendedDate, rs.SocialAttendedDate
                                )contra_ref_obj
                                ON
                                (
                                    contra_ref_obj.FirstName = ref_obj.FirstName
                                    AND
                                    contra_ref_obj.LastName = ref_obj.LastName
                                )
		                        */
                                --WHERE
                                    --agregado_obj.communityAidDetail IS NOT NULL

                                ORDER BY
                                    agregado_obj.personType DESC";

            return UnitOfWork.DbContext.Database.SqlQuery<GlobalReportDTO>(query,
                                                new SqlParameter("initialDate", initialDate),
                                                new SqlParameter("lastDate", lastDate)).ToList();
        }

        public List<BeneficiariesWithoutServicesDTO> getBeneficiariesWithoutServices(DateTime initialDate, DateTime lastDate)
        {
            String query = @"SELECT
		                            cp.Name AS ChiefPartner
		                            ,p.[Name] AS Partner
		                            ,hh.HouseholdName AS HouseholdName
		                            --,CONVERT(varchar,hh.RegistrationDate,103) AS HouseholdRegistrationDate
                                    ,hh.RegistrationDate AS HouseholdRegistrationDate
		                            ,'CRIANÇA' AS BeneficiaryType
		                            ,cs.Description AS BeneficiaryStatus
		                            -- ,c.childID
		                            ,c.FirstName AS FirstName
		                            ,c.LastName AS LastName
		                            --,c.FirstName+' '+ c.LastName AS FullName
		                            ,c.Gender AS Gender
		                            --,CONVERT(varchar,c.DateOfBirth,103) AS DateOfBirth
                                    ,c.DateOfBirth
		                            ,DATEDIFF(year, CAST(c.DateOfBirth As Date), @lastDate) AS YearAge
		                            ,DATEDIFF(month, CAST(c.DateOfBirth As Date), @lastDate) AS MonthAge
                            FROM  [Partner] p
                            inner join  [Partner] cp on (p.SuperiorId = cp.PartnerID) 
                            inner join  [HouseHold] hh on (p.PartnerID = hh.PartnerID) 
                            inner join  [Child] c on (hh.HouseHoldID = c.HouseholdID)
                            inner join  [ChildStatusHistory] csh 
			                            ON (csh.ChildStatusHistoryID = (SELECT max(csh2.ChildStatusHistoryID) FROM  [ChildStatusHistory] csh2 WHERE csh2.ChildID = c.ChildID AND csh2.EffectiveDate <= @lastDate))
                            inner join  [ChildStatus] cs ON (csh.childStatusID = cs.StatusID AND cs.Description in ('Inicial'))
                            inner join  [Beneficiary] ben ON ben.ID = c.ChildID AND type='child'
                            Where p.CollaboratorRoleID = 1 
                            AND ben.RegistrationDate <= @lastDate 
                            AND c.ChildID NOT IN
                            ( 
		                            SELECT
			                            c.ChildID
		                            FROM
		                            (

			                            SELECT  --  Group By Child
				                            routv_dt.ChildID
				                            ,ISNULL(SUM(routv_dt.FinaceAid),0) As FinanceAid
				                            ,ISNULL(SUM(routv_dt.Health),0) As Health
				                            ,ISNULL(SUM(routv_dt.Food),0) As Food
				                            ,ISNULL(SUM(routv_dt.Education),0) As Education
				                            ,ISNULL(SUM(routv_dt.LegalAdvice),0) As LegalAdvice
				                            ,ISNULL(SUM(routv_dt.Housing),0) As Housing 
				                            ,ISNULL(SUM(routv_dt.SocialAid),0) As SocialAid
				                            ,DPI = CASE WHEN DATEDIFF(YEAR, CAST(routv_dt.DateOfBirth As Date), routv_dt.RoutineVisitDate) < 6 THEN SUM(routv_dt.DPI) ELSE 0 END
				                            ,ISNULL(routv_dt.HIVRisk,0) As HIVRisk
				                            ,MUACGreen = CASE WHEN DATEDIFF(m, CAST(routv_dt.DateOfBirth As Date), routv_dt.RoutineVisitDate) BETWEEN 6 AND 59 THEN SUM(routv_dt.MUACGreen) ELSE 0 END
				                            ,MUACYellow = CASE WHEN DATEDIFF(m, CAST(routv_dt.DateOfBirth As Date), routv_dt.RoutineVisitDate) BETWEEN 6 AND 59 THEN SUM(routv_dt.MUACYellow) ELSE 0 END
				                            ,MUACRed = CASE WHEN DATEDIFF(m, CAST(routv_dt.DateOfBirth As Date), routv_dt.RoutineVisitDate) BETWEEN 6 AND 59 THEN SUM(routv_dt.MUACRed) ELSE 0 END
			                            FROM
			                            (
				                            SELECT 
					                            childRVSummart.*
				                            FROM  [Routine_Visit_Summary] childRVSummart
				                            inner join  [Child] c ON childRVSummart.ChildID = c.ChildID
				                            WHERE childRVSummart.RoutineVisitDate BETWEEN @initialDate AND @lastDate AND childRVSummart.[ChildID] IS NOT NULL
			                            ) routv_dt
			                            group by routv_dt.ChiefPartner, routv_dt.Partner ,routv_dt.ChildID, routv_dt.DateOfBirth,routv_dt.RoutineVisitDate, routv_dt.HIVRisk
		                            )c
		                            WHERE (c.FinanceAid + c.Health  + c.Food + c.Education + c.LegalAdvice + c.Housing  + c.SocialAid 
		                            + c.DPI + c.HIVRisk +  c.MUACGreen + c.MUACYellow + c.MUACRed) > 0
                            )


                            UNION ALL

                            SELECT
		                            cp.Name AS ChiefPartner
		                            ,p.[Name] AS Partner
		                            ,hh.HouseholdName AS HouseholdName
		                            --,CONVERT(varchar,hh.RegistrationDate,103) AS HouseholdRegistrationDate
                                    ,hh.RegistrationDate AS HouseholdRegistrationDate
		                            ,'ADULT' AS BeneficiaryType
		                            ,cs.Description AS BeneficiaryStatus
		                            -- ,a.adultID
		                            ,a.FirstName AS FirstName
		                            ,a.LastName AS LastName
		                            --,a.FirstName+' '+ a.LastName AS FullName
		                            ,a.Gender AS Gender
		                            --,CONVERT(varchar,a.DateOfBirth,103) AS DateOfBirth
                                    ,a.DateOfBirth
		                            ,DATEDIFF(year, CAST(a.DateOfBirth As Date), @lastDate) AS YearAge
		                            ,DATEDIFF(month, CAST(a.DateOfBirth As Date), @lastDate) AS MonthAge
                            FROM  [Partner] p
                            inner join  [Partner] cp on (p.SuperiorId = cp.PartnerID) 
                            inner join  [HouseHold] hh on (p.PartnerID = hh.PartnerID)
                            inner join  [Adult] a on (hh.HouseHoldID = a.HouseholdID)
                            inner join  [ChildStatusHistory] csh 
			                            ON (csh.ChildStatusHistoryID = (SELECT max(csh2.ChildStatusHistoryID) FROM  [ChildStatusHistory] csh2 WHERE csh2.AdultID = a.AdultID AND csh2.EffectiveDate <= @lastDate))
                            inner join  [ChildStatus] cs ON (csh.childStatusID = cs.StatusID AND cs.Description in ('Inicial'))
                            inner join  [Beneficiary] ben ON ben.ID = a.AdultID AND type='adult'
                            Where p.CollaboratorRoleID = 1 
                            AND ben.RegistrationDate <= @lastDate 
                            AND a.AdultID NOT IN
                            ( 
		                            SELECT
			                            a.AdultID
		                            FROM
		                            (
			                            SELECT  --  Group By Child
				                            routv_dt.AdultID 
				                            ,ISNULL(SUM(routv_dt.FinaceAid),0) As FinanceAid
				                            ,ISNULL(SUM(routv_dt.Health),0) As Health
				                            ,ISNULL(SUM(routv_dt.Food),0) As Food
				                            ,ISNULL(SUM(routv_dt.Education),0) As Education
				                            ,ISNULL(SUM(routv_dt.LegalAdvice),0) As LegalAdvice
				                            ,ISNULL(SUM(routv_dt.Housing),0) As Housing 
				                            ,ISNULL(SUM(routv_dt.SocialAid),0) As SocialAid
				                            ,DPI = CASE WHEN DATEDIFF(YEAR, CAST(routv_dt.DateOfBirth As Date), routv_dt.RoutineVisitDate) < 6 THEN SUM(routv_dt.DPI) ELSE 0 END
				                            ,ISNULL(routv_dt.HIVRisk,0) As HIVRisk
				                            ,MUACGreen = CASE WHEN DATEDIFF(month, CAST(routv_dt.DateOfBirth As Date), routv_dt.RoutineVisitDate) BETWEEN 6 AND 59 THEN SUM(routv_dt.MUACGreen) ELSE 0 END
				                            ,MUACYellow = CASE WHEN DATEDIFF(month, CAST(routv_dt.DateOfBirth As Date), routv_dt.RoutineVisitDate) BETWEEN 6 AND 59 THEN SUM(routv_dt.MUACYellow) ELSE 0 END
				                            ,MUACRed = CASE WHEN DATEDIFF(month, CAST(routv_dt.DateOfBirth As Date), routv_dt.RoutineVisitDate) BETWEEN 6 AND 59 THEN SUM(routv_dt.MUACRed) ELSE 0 END
			                            FROM
			                            (
				                            SELECT 
					                            adultRVSummart.*
				                            FROM  [Routine_Visit_Summary] adultRVSummart
				                            inner join  [Adult] a ON adultRVSummart.AdultID = a.AdultID
				                            WHERE adultRVSummart.RoutineVisitDate BETWEEN @initialDate AND @lastDate AND adultRVSummart.AdultID IS NOT NULL
			                            ) routv_dt
			                            group by routv_dt.ChiefPartner, routv_dt.Partner ,routv_dt.AdultID, routv_dt.DateOfBirth,routv_dt.RoutineVisitDate, routv_dt.HIVRisk
		                            )a
		                            WHERE (a.FinanceAid + a.Health  + a.Food + a.Education + a.LegalAdvice + a.Housing  + a.SocialAid 
			                            + a.DPI + a.HIVRisk + a.MUACGreen + a.MUACYellow + a.MUACRed) > 0
                            )
                            ORDER BY cp.Name, p.Name, hh.HouseholdName";

            return UnitOfWork.DbContext.Database.SqlQuery<BeneficiariesWithoutServicesDTO>(query,
                                                new SqlParameter("initialDate", initialDate),
                                                new SqlParameter("lastDate", lastDate)).ToList();
        }


        public List<BeneficiaryStatusDTO> getBeneficiariesByStatus(DateTime initialDate, DateTime lastDate, int statusID)
        {
            String query = @"SELECT
		                            cp.Name AS ChiefPartner
		                            ,p.[Name] AS Partner
		                            ,hh.HouseholdName AS HouseholdName
                                    ,ben.RegistrationDate AS RegistrationDate
		                            ,'CRIANÇA' AS BeneficiaryType
		                            ,cs.Description AS BeneficiaryStatus
		                            ,csh.EffectiveDate AS EffectiveDate
		                            ,c.FirstName AS FirstName
		                            ,c.LastName AS LastName
		                            ,c.Gender AS Gender
                                    ,c.DateOfBirth
                            FROM  [Partner] p
                            inner join  [Partner] cp on (p.SuperiorId = cp.PartnerID) 
                            inner join  [HouseHold] hh on (p.PartnerID = hh.PartnerID) 
                            inner join  [Child] c on (hh.HouseHoldID = c.HouseholdID)
                            inner join  [ChildStatusHistory] csh 
			                            ON (csh.ChildStatusHistoryID = (SELECT max(csh2.ChildStatusHistoryID) FROM  [ChildStatusHistory] csh2 WHERE csh2.ChildID = c.ChildID 
			                            AND (csh2.EffectiveDate BETWEEN @initialDate AND  @lastDate)))
                            inner join  [ChildStatus] cs ON (csh.childStatusID = cs.StatusID AND cs.StatusID = @statusID)
                            inner join  [Beneficiary] ben ON ben.ID = c.ChildID AND type='child'

                            UNION ALL

                            SELECT
		                            cp.Name AS ChiefPartner
		                            ,p.[Name] AS Partner
		                            ,hh.HouseholdName AS HouseholdName
                                    ,ben.RegistrationDate AS RegistrationDate
		                            ,'ADULT' AS BeneficiaryType
		                            ,cs.Description AS BeneficiaryStatus
		                            ,csh.EffectiveDate AS EffectiveDate
		                            ,a.FirstName AS FirstName
		                            ,a.LastName AS LastName
		                            ,a.Gender AS Gender
                                    ,a.DateOfBirth
                            FROM  [Partner] p
                            inner join  [Partner] cp on (p.SuperiorId = cp.PartnerID) 
                            inner join  [HouseHold] hh on (p.PartnerID = hh.PartnerID)
                            inner join  [Adult] a on (hh.HouseHoldID = a.HouseholdID)
                            inner join  [ChildStatusHistory] csh 
			                            ON (csh.ChildStatusHistoryID = (SELECT max(csh2.ChildStatusHistoryID) FROM  [ChildStatusHistory] csh2 WHERE csh2.AdultID = a.AdultID 
			                            AND (csh2.EffectiveDate BETWEEN @initialDate AND  @lastDate)))
                            inner join  [ChildStatus] cs ON (csh.childStatusID = cs.StatusID AND cs.StatusID = @statusID)
                            inner join  [Beneficiary] ben ON ben.ID = a.AdultID AND type='adult'
                            ORDER BY cp.Name, p.Name, hh.HouseholdName, BeneficiaryType, FirstName";

            query = (statusID == 0) ? query.Replace("AND cs.StatusID = @statusID", " ") : query;

            return UnitOfWork.DbContext.Database.SqlQuery<BeneficiaryStatusDTO>(query,
                                                new SqlParameter("initialDate", initialDate),
                                                new SqlParameter("lastDate", lastDate),
                                                new SqlParameter("statusID", statusID)).ToList();
        }


        /*
         * ########################################################################
         * ############## Before and Actual Beneficiary Status Report #############
         * ########################################################################
         */

        public List<BeforeAndActualChildStatusReportDTO> getBeforeAndActualBeneficiaryStatusReport(DateTime initialDate, DateTime lastDate)
        {
            String query = @"SELECT 
	                            estagioAnterior.PartnerName AS Partner
	                            ,estagioAnterior.BeneficiaryType
	                            ,estagioAnterior.FirstName
	                            ,estagioAnterior.LastName
	                            ,estagioAnterior.Gender
	                            ,estagioAnterior.Age
	                            ,estagioAnterior.Description AS BeforeActualStatus
	                            ,estagioAnterior.StatusDate AS BeforeActualStatusDate
	                            ,estagioActual.Description AS ActualStatus
	                            ,estagioActual.StatusDate AS ActualStatusDate
                            FROM
                            (
	                            SELECT	p.Name AS PartnerName
			                            ,'CRIANÇA' as BeneficiaryType
			                            ,c.FirstName
			                            ,c.LastName
			                            ,c.Gender AS Gender
			                            ,DATEDIFF(YEAR, c.DateOFBirth, GETDATE()) AS Age
			                            ,row_number() over(partition by csh.[ChildID] order by csh.[EffectiveDate] DESC) as LastStatusRow
			                            ,ct.Description
			                            ,csh.[EffectiveDate] AS StatusDate
	                            FROM [ChildStatusHistory] AS csh
	                            INNER JOIN [Child] AS c ON c.[ChildID] = csh.[ChildID]
	                            INNER JOIN [HouseHold] AS hh ON hh.HouseHoldID = c.HouseholdID
	                            INNER JOIN [ChildStatus] AS ct ON csh.[ChildStatusID] = ct.[StatusID]
	                            INNER JOIN [Partner] AS p ON hh.PartnerID = p.PartnerID
	                            WHERE  c.[ChildID] IN
	                            (SELECT [ChildStatusHistory].[ChildID]
	                            FROM [ChildStatusHistory]
	                            GROUP BY [ChildStatusHistory].[ChildID]
	                            HAVING count([ChildStatusHistory].[ChildID]) > 1)

	                            UNION ALL

	                            SELECT	p.Name AS PartnerName
			                            ,'ADULTO' as BeneficiaryType
			                            ,a.FirstName
			                            ,a.LastName
			                            ,a.Gender AS Gender
			                            ,DATEDIFF(YEAR, a.DateOFBirth, GETDATE()) AS Age
			                            ,row_number() over(partition by csh.[adultID] order by csh.[EffectiveDate] DESC) as LastStatusRow
			                            ,ct.Description
			                            ,csh.[EffectiveDate] AS StatusDate
	                            FROM [ChildStatusHistory] AS csh
	                            INNER JOIN [Adult] AS a ON a.[adultID] = csh.[adultID]
	                            INNER JOIN [HouseHold] AS hh ON hh.HouseHoldID = a.HouseholdID
	                            INNER JOIN [ChildStatus] AS ct ON csh.[ChildStatusID] = ct.[StatusID]
	                            INNER JOIN [Partner] AS p ON hh.PartnerID = p.PartnerID
	                            WHERE a.[ChildID] IN
	                            (SELECT [ChildStatusHistory].[adultID]
	                            FROM [ChildStatusHistory]
	                            GROUP BY [ChildStatusHistory].[adultID]
	                            HAVING count([ChildStatusHistory].[adultID]) > 1)
                            )estagioAnterior
                            LEFT JOIN
                            (
	                            SELECT	
			                            'CRIANÇA' as BeneficiaryType
			                            ,c.FirstName
			                            ,c.LastName
			                            ,c.Gender AS Gender
			                            ,DATEDIFF(YEAR, c.DateOFBirth, GETDATE()) AS Age
			                            ,row_number() over(partition by csh.[ChildID] order by csh.[EffectiveDate] DESC) as LastStatusRow
			                            ,ct.Description
			                            ,csh.[EffectiveDate] AS StatusDate
	                            FROM [ChildStatusHistory] AS csh
	                            INNER JOIN [Child] AS c ON c.[ChildID] = csh.[ChildID]
	                            INNER JOIN [HouseHold] AS hh ON hh.HouseHoldID = c.HouseholdID
	                            INNER JOIN [ChildStatus] AS ct ON csh.[ChildStatusID] = ct.[StatusID]
	                            INNER JOIN [Partner] AS p ON hh.PartnerID = p.PartnerID
	                            WHERE  c.[ChildID] IN
	                            (SELECT [ChildStatusHistory].[ChildID]
	                            FROM [ChildStatusHistory]
	                            GROUP BY [ChildStatusHistory].[ChildID]
	                            HAVING count([ChildStatusHistory].[ChildID]) > 1)

	                            UNION ALL

	                            SELECT	'ADULTO' as BeneficiaryType
			                            ,a.FirstName
			                            ,a.LastName
			                            ,a.Gender AS Gender
			                            ,DATEDIFF(YEAR, a.DateOFBirth, GETDATE()) AS Age
			                            ,row_number() over(partition by csh.[adultID] order by csh.[EffectiveDate] DESC) as LastStatusRow
			                            ,ct.Description
			                            ,csh.[EffectiveDate] AS StatusDate
	                            FROM [ChildStatusHistory] AS csh
	                            INNER JOIN [Adult] AS a ON a.[adultID] = csh.[adultID]
	                            INNER JOIN [HouseHold] AS hh ON hh.HouseHoldID = a.HouseholdID
	                            INNER JOIN [ChildStatus] AS ct ON csh.[ChildStatusID] = ct.[StatusID]
	                            INNER JOIN [Partner] AS p ON hh.PartnerID = p.PartnerID
	                            WHERE a.[ChildID] IN
	                            (SELECT [ChildStatusHistory].[adultID]
	                            FROM [ChildStatusHistory]
	                            GROUP BY [ChildStatusHistory].[adultID]
	                            HAVING count([ChildStatusHistory].[adultID]) > 1)
                            )estagioActual
                            ON estagioAnterior.FirstName=estagioActual.FirstName
                            AND estagioAnterior.LastName=estagioActual.LastName
                            WHERE
                            estagioActual.LastStatusRow = 1 AND estagioAnterior.LastStatusRow = 2 
                            AND estagioActual.StatusDate BETWEEn @initialDate AND @lastDate";

            return UnitOfWork.DbContext.Database.SqlQuery<BeforeAndActualChildStatusReportDTO>(query,
                                                            new SqlParameter("initialDate", initialDate),
                                                            new SqlParameter("lastDate", lastDate)).ToList();
        }


        // ===========================================================================================


        public List<BeneficiaryHIVStatusDTO> getBeneficiariesWithNotInTARVandUnknownHIVStatus(DateTime initialDate, DateTime lastDate)
        {
            String query = @"SELECT
		                        cp.Name AS ChiefPartner
		                        ,p.[Name] AS Partner
		                        ,hh.HouseholdName AS HouseholdName
                                ,ben.RegistrationDate AS RegistrationDate
		                        ,'CRIANÇA' AS BeneficiaryType
		                        ,HIVStatus = 
		                        CASE WHEN hs.HIV = 'P' AND hs.HIVInTreatment = 1 THEN 'serostado HIV+ não TARV' 
		                        WHEN hs.HIV = 'U' THEN 'Desconhecido' 
		                        ELSE '' END
		                        ,hs.[InformationDate] AS HIVStatusDate
		                        ,c.FirstName AS FirstName
		                        ,c.LastName AS LastName
		                        ,c.Gender AS Gender
                                ,c.DateOfBirth
                        FROM  [Partner] p
                        inner join [Partner] cp on (p.SuperiorId = cp.PartnerID) 
                        inner join [HouseHold] hh on (p.PartnerID = hh.PartnerID) 
                        inner join [Child] c on (hh.HouseHoldID = c.HouseholdID)
                        inner join [HIVStatus] hs on (hs.HIVStatusID = 
	                        (SELECT max(hs1.HIVStatusID) FROM  [HIVStatus] hs1 WHERE hs1.ChildID = c.ChildID
	                        AND (hs1.[InformationDate] BETWEEN @initialDate AND  @lastDate)
	                        AND ((hs1.HIV = 'P' AND hs1.HIVInTreatment = 1) OR (hs1.HIV = 'U'))))
                        inner join [Beneficiary] ben ON ben.ID = c.ChildID AND type='child'

                        UNION ALL

                        SELECT
		                        cp.Name AS ChiefPartner
		                        ,p.[Name] AS Partner
		                        ,hh.HouseholdName AS HouseholdName
                                ,ben.RegistrationDate AS RegistrationDate
		                        ,'ADULT' AS BeneficiaryType
		                        ,HIVStatus = 
		                        CASE WHEN hs.HIV = 'P' AND hs.HIVInTreatment = 1 THEN 'serostado HIV+ não TARV' 
		                        WHEN hs.HIV = 'U' THEN 'Desconhecido' 
		                        ELSE '' END
		                        ,hs.[InformationDate] AS HIVStatusDate
		                        ,a.FirstName AS FirstName
		                        ,a.LastName AS LastName
		                        ,a.Gender AS Gender
                                ,a.DateOfBirth
                        FROM  [Partner] p
                        inner join [Partner] cp on (p.SuperiorId = cp.PartnerID) 
                        inner join [HouseHold] hh on (p.PartnerID = hh.PartnerID)
                        inner join [Adult] a on (hh.HouseHoldID = a.HouseholdID)
                        inner join [HIVStatus] hs on (hs.HIVStatusID = 
	                        (SELECT max(hs1.HIVStatusID) FROM  [HIVStatus] hs1 WHERE hs1.AdultID = a.AdultId
	                        AND (hs1.[InformationDate] BETWEEN @initialDate AND  @lastDate)
	                        AND ((hs1.HIV = 'P' AND hs1.HIVInTreatment = 1) OR (hs1.HIV = 'U'))))
                        inner join [Beneficiary] ben ON ben.ID = a.AdultID AND type='adult'
                        ORDER BY cp.Name, p.Name, hh.HouseholdName, BeneficiaryType, FirstName";

            return UnitOfWork.DbContext.Database.SqlQuery<BeneficiaryHIVStatusDTO>(query,
                                                new SqlParameter("initialDate", initialDate),
                                                new SqlParameter("lastDate", lastDate)).ToList();
        }

        public List<ReferencesListWithStatusDTO> GetReferencesListWithStatus(DateTime initialDate, DateTime lastDate)
        {
            var query = @"SELECT 
                        p.Name AS PartnerName, 
                        cp.Name AS ChiefPartnerName, 
                        CONCAT(ben.FirstName,' ' , ben.LastName) AS BeneficiaryName,
                        CASE 
	                        WHEN rt.ReferenceName IN (
								                        'Maternidade p/ Parto', 'CPN', 'CPN Familiar', 'Suspeito de TB',
								                        'Consulta Pós-Parto', 'CCR', 'PTV', 'ATS', 'ITS', 'Pré-TARV/IO',
								                        'Testado HIV+', 'Abandono TARV', 'PPE', 'Circuncisao Masculina',
								                        'Contacto de TB', 'Controlo de BK', 'Abandono de TTB', 'Reacções do TTB',
								                        'Suspeito de Malária', 'Suspeito de Malnutrição', 'Banco de Socorro/Controle de triagem',
								                        'Controlo da Dor'
							                        ) THEN 'Saude'
	                        ELSE 'Accao Social'
                        END AS ReferenceProvider,
                        rt.ReferenceName,
                        CONVERT(varchar, rs.ReferenceDate, 23) AS ReferenceDate,
                        CASE 
	                        WHEN rs.CounterReferenceDate is null THEN 'Sim'
	                        ELSE ''
                        END AS InProgress,
                        CASE 
	                        WHEN rs.CounterReferenceDate is not null THEN  CONVERT(varchar, rs.CounterReferenceDate, 23)
	                        ELSE ''
                        END AS IsComplete
                        FROM            
                        dbo.Partner AS p 
                        INNER JOIN dbo.Partner AS cp ON p.SuperiorId = cp.PartnerID
                        INNER JOIN dbo.HouseHold AS hh ON p.PartnerID = hh.PartnerID
                        INNER JOIN dbo.Beneficiary AS ben ON ben.HouseholdID = hh.HouseHoldID
                        INNER JOIN dbo.ReferenceService AS rs ON (rs.BeneficiaryID = ben.BeneficiaryID)
                        INNER JOIN dbo.Reference AS r ON (r.ReferenceServiceID = rs.ReferenceServiceID and r.Value = '1')
                        INNER JOIN dbo.ReferenceType rt ON (r.ReferenceTypeID = rt.ReferenceTypeID and rt.ReferenceCategory = 'Activist')
                        WHERE rs.ReferenceDate BETWEEN @initialDate AND @lastDate";

            return UnitOfWork.DbContext.Database.SqlQuery<ReferencesListWithStatusDTO>(query,
                                                new SqlParameter("initialDate", initialDate),
                                                new SqlParameter("lastDate", lastDate)).ToList();
        }

        public List<RoutineVisitMonthlyDTO> GetRoutineVisitMonthlyReport(DateTime initialDate, DateTime lastDate)
        {
            var query = @"SELECT  
	                        DomainOrder,
	                        Domain,
	                        Question,
	                        SUM(btw_x_1_M) AS btw_x_1_M,
	                        SUM(btw_1_4_M) AS btw_1_4_M,
	                        SUM(btw_5_9_M) AS btw_5_9_M,
	                        SUM(btw_10_14_M) AS btw_10_14_M,
	                        SUM(btw_15_17_M) AS btw_15_17_M,
	                        SUM(btw_18_24_M) AS btw_18_24_M,
	                        SUM(btw_25_x_M) AS btw_25_x_M,
	                        SUM(btw_x_1_F) AS btw_x_1_F,
	                        SUM(btw_1_4_F) AS btw_1_4_F,
	                        SUM(btw_5_9_F) AS btw_5_9_F,
	                        SUM(btw_10_14_F) AS btw_10_14_F,
	                        SUM(btw_15_17_F) AS btw_15_17_F,
	                        SUM(btw_18_24_F) AS btw_18_24_F,
	                        SUM(btw_25_x_F) AS btw_25_x_F,
	                        SupportServiceOrderInDomain
                        FROM
	                        (SELECT 
	                        sst.TypeDescription AS Domain,
	                        sst.Description AS Question,
	                        sst.SupportServiceOrderInDomain,
	                        sst.DomainOrder,
	                        btw_x_1_M = CASE WHEN ben.Gender = 'M' and DATEDIFF(MONTH, CAST(ben.DateOfBirth AS Date), GETDATE()) < 12 AND rvs.Checked = '1' THEN 1 ELSE 0 END,
	                        btw_1_4_M = CASE WHEN ben.Gender = 'M' and DATEDIFF(YEAR, CAST(ben.DateOfBirth AS Date), GETDATE()) BETWEEN 1 AND 4 AND rvs.Checked = '1' THEN 1 ELSE 0 END,
	                        btw_5_9_M = CASE WHEN ben.Gender = 'M' and DATEDIFF(YEAR, CAST(ben.DateOfBirth AS Date), GETDATE())  BETWEEN 5 AND 9 AND rvs.Checked = '1' THEN 1 ELSE 0 END,
	                        btw_10_14_M = CASE WHEN ben.Gender = 'M' and DATEDIFF(YEAR, CAST(ben.DateOfBirth AS Date), GETDATE()) BETWEEN 10 AND 14 AND rvs.Checked = '1' THEN 1 ELSE 0 END,
	                        btw_15_17_M = CASE WHEN ben.Gender = 'M' and DATEDIFF(YEAR, CAST(ben.DateOfBirth AS Date), GETDATE()) BETWEEN 15 AND 17 AND rvs.Checked = '1' THEN 1 ELSE 0 END,
	                        btw_18_24_M = CASE WHEN ben.Gender = 'M' and DATEDIFF(YEAR, CAST(ben.DateOfBirth AS Date), GETDATE()) BETWEEN 18 AND 24 AND rvs.Checked = '1' THEN 1 ELSE 0 END,
	                        btw_25_x_M = CASE WHEN ben.Gender = 'M' and DATEDIFF(YEAR, CAST(ben.DateOfBirth AS Date), GETDATE()) > 24 AND rvs.Checked = '1' THEN 1 ELSE 0 END,
	                        btw_x_1_F = CASE WHEN ben.Gender = 'F' and DATEDIFF(MONTH, CAST(ben.DateOfBirth AS Date), GETDATE()) < 12 AND rvs.Checked = '1' THEN 1 ELSE 0 END,
	                        btw_1_4_F = CASE WHEN ben.Gender = 'F' and DATEDIFF(YEAR, CAST(ben.DateOfBirth AS Date), GETDATE()) BETWEEN 1 AND 4 AND rvs.Checked = '1' THEN 1 ELSE 0 END,
	                        btw_5_9_F = CASE WHEN ben.Gender = 'F' and DATEDIFF(YEAR, CAST(ben.DateOfBirth AS Date), GETDATE())  BETWEEN 5 AND 9 AND rvs.Checked = '1' THEN 1 ELSE 0 END,
	                        btw_10_14_F = CASE WHEN ben.Gender = 'F' and DATEDIFF(YEAR, CAST(ben.DateOfBirth AS Date), GETDATE()) BETWEEN 10 AND 14 AND rvs.Checked = '1' THEN 1 ELSE 0 END,
	                        btw_15_17_F = CASE WHEN ben.Gender = 'F' and DATEDIFF(YEAR, CAST(ben.DateOfBirth AS Date), GETDATE()) BETWEEN 15 AND 17 AND rvs.Checked = '1' THEN 1 ELSE 0 END,
	                        btw_18_24_F = CASE WHEN ben.Gender = 'F' and DATEDIFF(YEAR, CAST(ben.DateOfBirth AS Date), GETDATE()) BETWEEN 18 AND 24 AND rvs.Checked = '1' THEN 1 ELSE 0 END,
	                        btw_25_x_F = CASE WHEN ben.Gender = 'F' and DATEDIFF(YEAR, CAST(ben.DateOfBirth AS Date), GETDATE()) > 24 AND rvs.Checked = '1' THEN 1 ELSE 0 END
	                        from SupportServiceType sst
	                        inner join RoutineVisitSupport rvs on sst.SupportServiceTypeID = rvs.SupportID
	                        inner join RoutineVisitMember rvm on (rvm.RoutineVisitMemberID = rvs.RoutineVisitMemberID)
	                        inner join Beneficiary ben on ben.BeneficiaryID = rvm.BeneficiaryID
	                        left join RoutineVisit rv on rv.RoutineVisitID = rvm.RoutineVisitID
	                        and rv.RoutineVisitDate BETWEEN @initialDate and @lastDate
	                        and sst.Description not in ('HIV-', 'HIV+ -> Em TARV', 'HIV+ -> Não TARV', 'Conhece mas não revelou', 'Não conhece') ) q
                        GROUP BY DomainOrder, Domain, Question, SupportServiceOrderInDomain
                        ORDER BY DomainOrder, SupportServiceOrderInDomain";

            return UnitOfWork.DbContext.Database.SqlQuery<RoutineVisitMonthlyDTO>(query,
                                                new SqlParameter("initialDate", initialDate),
                                                new SqlParameter("lastDate", lastDate)).ToList();
        }



        /*
         * ####################################################################
         * ###### MonthlyActiveBeneficiariesSummaryReport ChiefPartner  #######
         * ####################################################################
         */

        public List<MonthlyActiveBeneficiariesSummaryReportDTO> getMonthlyActiveBeneficiariesSummaryChiefPartner(DateTime initialDate, DateTime lastDate)
        {
            return GetMonthlyActiveBeneficiariesSummaryReport(initialDate, lastDate, "chiefpartner");
        }

        /*
         * ####################################################################
         * ######### MonthlyActiveBeneficiariesSummaryReport Partner ##########
         * ####################################################################
         */

        public List<MonthlyActiveBeneficiariesSummaryReportDTO> getMonthlyActiveBeneficiariesSummaryPartner(DateTime initialDate, DateTime lastDate)
        {
            return GetMonthlyActiveBeneficiariesSummaryReport(initialDate, lastDate, "partner");
        }

        /*
         * ##################################################################################
         * #################### MonthlyActiveBeneficiariesSummaryReport #####################
         * ##################################################################################
         */

        public List<MonthlyActiveBeneficiariesSummaryReportDTO> GetMonthlyActiveBeneficiariesSummaryReport(DateTime initialDate, DateTime lastDate, string partnerType)
        {
            var query = @"SELECT
	                            p.ChiefPartner--<<ReplaceColumn<<--
	                            As Partner,
	                            ISNULL(obj_Age.btw_x_1_M, 0) As btw_x_1_M,
	                            ISNULL(obj_Age.btw_1_4_M, 0) AS btw_1_4_M,
	                            ISNULL(obj_Age.btw_5_9_M, 0) AS btw_5_9_M,
	                            ISNULL(obj_Age.btw_10_14_M, 0) AS btw_10_14_M,
	                            ISNULL(obj_Age.btw_15_17_M, 0) AS btw_15_17_M,
	                            ISNULL(obj_Age.btw_18_24_M, 0) AS btw_18_24_M,
	                            ISNULL(obj_Age.btw_25_x_M, 0) AS btw_25_x_M,
	                            ISNULL(obj_Age.btw_x_1_F, 0) AS btw_x_1_F,
	                            ISNULL(obj_Age.btw_1_4_F, 0) AS btw_1_4_F,
	                            ISNULL(obj_Age.btw_5_9_F, 0) AS btw_5_9_F,
	                            ISNULL(obj_Age.btw_10_14_F, 0) AS btw_10_14_F,
	                            ISNULL(obj_Age.btw_15_17_F, 0) AS btw_15_17_F,
	                            ISNULL(obj_Age.btw_18_24_F, 0) AS btw_18_24_F,
	                            ISNULL(obj_Age.btw_25_x_F, 0) AS btw_25_x_F,

	                            ISNULL(HIV_P_IN_TARV_Child, 0)  AS HIV_P_IN_TARV_Child,
	                            ISNULL(HIV_P_NOT_TARV_Child, 0)  AS HIV_P_NOT_TARV_Child,
	                            ISNULL(HIV_N_Child, 0)  AS HIV_N_Child,
	                            ISNULL(HIV_KNOWN_NREVEAL_Child, 0)  AS HIV_KNOWN_NREVEAL_Child,
	                            ISNULL(HIV_NOT_RECOMMENDED_Child, 0)  AS  HIV_NOT_RECOMMENDED_Child,
	                            ISNULL(HIV_UNKNOWN_Child, 0)  AS HIV_UNKNOWN_Child,
	                            ISNULL(HIV_P_IN_TARV_Adult, 0)  AS HIV_P_IN_TARV_Adult,
	                            ISNULL(HIV_P_NOT_TARV_Adult, 0)  AS HIV_P_NOT_TARV_Adult,
	                            ISNULL(HIV_N_Adult, 0)  AS HIV_N_Adult,
	                            ISNULL(HIV_KNOWN_NREVEAL_Adult, 0)  AS  HIV_KNOWN_NREVEAL_Adult,
	                            ISNULL(HIV_UNKNOWN_Adult, 0)  AS HIV_UNKNOWN_Adult,

	                            ISNULL(obj_NonGratuated.DeathChild, 0) AS DeathChild, 
	                            ISNULL(obj_NonGratuated.DeathAdult, 0) AS DeathAdult, 
	                            ISNULL(obj_NonGratuated.LostChild, 0) AS LostChild, 
	                            ISNULL(obj_NonGratuated.LostAdult, 0) AS LostAdult, 
	                            ISNULL(obj_NonGratuated.GaveUpChild, 0) AS GaveUpChild, 
	                            ISNULL(obj_NonGratuated.GaveUpAdult, 0) AS GaveUpAdult, 
	                            ISNULL(obj_NonGratuated.TransfersPEPFARChild, 0) AS TransfersPEPFARChild, 
	                            ISNULL(obj_NonGratuated.TransfersPEPFARAdult, 0) AS TransfersPEPFARAdult, 
	                            ISNULL(obj_NonGratuated.TransfersNotPEPFARChild, 0) AS TransfersNotPEPFARChild, 
	                            ISNULL(obj_NonGratuated.TransfersNotPEPFARAdult, 0) AS TransfersNotPEPFARAdult, 
	                            ISNULL(obj_NonGratuated.BecameAdult, 0) AS BecameAdult, 
	                            ISNULL(obj_NonGratuated.ChildHasServices, 0) AS ChildHasServices, 
	                            ISNULL(obj_NonGratuated.AdultHasServices, 0) AS AdultHasServices
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
		                            obj_age.ChiefPartner--<<ReplaceColumn<<--
	                                ,
		                            SUM(obj_Age.btw_x_1_M) As btw_x_1_M,
		                            SUM(obj_Age.btw_1_4_M) AS btw_1_4_M,
		                            SUM(obj_Age.btw_5_9_M) AS btw_5_9_M,
		                            SUM(obj_Age.btw_10_14_M) AS btw_10_14_M,
		                            SUM(obj_Age.btw_15_17_M) AS btw_15_17_M,
		                            SUM(obj_Age.btw_18_24_M) AS btw_18_24_M,
		                            SUM(obj_Age.btw_25_x_M) AS btw_25_x_M,
		                            SUM(obj_Age.btw_x_1_F) AS btw_x_1_F,
		                            SUM(obj_Age.btw_1_4_F) AS btw_1_4_F,
		                            SUM(obj_Age.btw_5_9_F) AS btw_5_9_F,
		                            SUM(obj_Age.btw_10_14_F) AS btw_10_14_F,
		                            SUM(obj_Age.btw_15_17_F) AS btw_15_17_F,
		                            SUM(obj_Age.btw_18_24_F) AS btw_18_24_F,
		                            SUM(obj_Age.btw_25_x_F) AS btw_25_x_F

	                            FROM
	                            (
		                            SELECT
			                            cp.Name As ChiefPartner,
			                            p.Name As Partner,
			                            count(ben.BeneficiaryID) as ID,
			                            ben.FirstName As FirstName, 
			                            ben.LastName As LastName, 
			                            cs.Description,
			                            btw_x_1_M = CASE WHEN DATEDIFF(month, CAST(ben.DateOfBirth As Date), @lastDate) < 12 AND ben.Gender = 'M' THEN 1 ELSE 0 END,
			                            btw_1_4_M = CASE WHEN DATEDIFF(month, CAST(ben.DateOfBirth As Date), @lastDate) BETWEEN 12 AND 59 AND ben.Gender = 'M' THEN 1 ELSE 0 END,
			                            btw_5_9_M = CASE WHEN DATEDIFF(month, CAST(ben.DateOfBirth As Date), @lastDate) BETWEEN 60 AND 119 AND ben.Gender = 'M' THEN 1 ELSE 0 END,
			                            btw_10_14_M = CASE WHEN DATEDIFF(month, CAST(ben.DateOfBirth As Date), @lastDate) BETWEEN 120 AND 179 AND ben.Gender = 'M' THEN 1 ELSE 0 END,
			                            btw_15_17_M = CASE WHEN DATEDIFF(month, CAST(ben.DateOfBirth As Date), @lastDate) BETWEEN 180 AND 215 AND ben.Gender = 'M' THEN 1 ELSE 0 END,
			                            btw_18_24_M = CASE WHEN DATEDIFF(month, CAST(ben.DateOfBirth As Date), @lastDate) BETWEEN 216 AND 299 AND ben.Gender = 'M' THEN 1 ELSE 0 END,
			                            btw_25_x_M = CASE WHEN DATEDIFF(month, CAST(ben.DateOfBirth As Date), @lastDate) >= 300 AND ben.Gender = 'M' THEN 1 ELSE 0 END,
			                            btw_x_1_F = CASE WHEN DATEDIFF(month, CAST(ben.DateOfBirth As Date), @lastDate) < 12 AND ben.Gender = 'F' THEN 1 ELSE 0 END,
			                            btw_1_4_F = CASE WHEN DATEDIFF(month, CAST(ben.DateOfBirth As Date), @lastDate) BETWEEN 12 AND 59 AND ben.Gender = 'F' THEN 1 ELSE 0 END,
			                            btw_5_9_F = CASE WHEN DATEDIFF(month, CAST(ben.DateOfBirth As Date), @lastDate) BETWEEN 60 AND 119 AND ben.Gender = 'F' THEN 1 ELSE 0 END,
			                            btw_10_14_F = CASE WHEN DATEDIFF(month, CAST(ben.DateOfBirth As Date), @lastDate) BETWEEN 120 AND 179 AND ben.Gender = 'F' THEN 1 ELSE 0 END,
			                            btw_15_17_F = CASE WHEN DATEDIFF(month, CAST(ben.DateOfBirth As Date), @lastDate) BETWEEN 180 AND 215 AND ben.Gender = 'F' THEN 1 ELSE 0 END,
			                            btw_18_24_F = CASE WHEN DATEDIFF(month, CAST(ben.DateOfBirth As Date), @lastDate) BETWEEN 216 AND 299 AND ben.Gender = 'F' THEN 1 ELSE 0 END,
			                            btw_25_x_F = CASE WHEN DATEDIFF(month, CAST(ben.DateOfBirth As Date), @lastDate) >= 300 AND ben.Gender = 'F' THEN 1 ELSE 0 END
		                            FROM  [Partner] p
		                            inner join [Partner] cp on (p.SuperiorId = cp.PartnerID) 
		                            inner join [HouseHold] hh on (p.PartnerID = hh.PartnerID)
		                            inner join [Beneficiary] ben ON ben.HouseholdID = hh.HouseHoldID
		                            inner join [vw_beneficiary_details] benView ON benView.ID = ben.[BeneficiaryID] and ben.[ServicesStatusForReportDate] between @initialDate and @lastDate
		                            inner join [SimpleEntity] se ON ben.[ServicesStatusForReportID] = se.SimpleEntityID and se.Type='ben-services-status'
			                            and se.Code= '03'
		                            inner join  [ChildStatusHistory] csh 
			                            ON (csh.ChildStatusHistoryID = (SELECT max(csh2.ChildStatusHistoryID) FROM  [ChildStatusHistory] csh2 WHERE csh2.BeneficiaryID = ben.BeneficiaryID 
			                            AND (csh2.EffectiveDate < @lastDate)))
		                            inner join  [ChildStatus] cs ON (csh.childStatusID = cs.StatusID AND cs.Description = 'Inicial')
		                            WHERE p.CollaboratorRoleID = 1
		                            group by cp.Name, p.Name, ben.BeneficiaryID, ben.FirstName, ben.LastName, cs.Description, Ben.DateOfBirth, ben.Gender
	                            )obj_age 
	                            group by obj_age.ChiefPartner--<<ReplaceColumn<<--
                            )
                            obj_Age
                            ON 
                            (
	                            p.ChiefPartner--<<ReplaceColumn<<--
	                            =
	                            obj_Age.ChiefPartner--<<ReplaceColumn<<--
                            ) 
                            LEFT JOIN
                            (
	                            SELECT
		                            obj_NonGratuated.ChiefPartner--<<ReplaceColumn<<--
	                                ,
		                            SUM(obj_NonGratuated.DeathChild) AS DeathChild, 
		                            SUM(obj_NonGratuated.DeathAdult) AS DeathAdult, 
		                            SUM(obj_NonGratuated.LostChild) AS LostChild, 
		                            SUM(obj_NonGratuated.LostAdult) AS LostAdult, 
		                            SUM(obj_NonGratuated.GaveUpChild) AS GaveUpChild, 
		                            SUM(obj_NonGratuated.GaveUpAdult) AS GaveUpAdult, 
		                            SUM(obj_NonGratuated.TransfersPEPFARChild) AS TransfersPEPFARChild, 
		                            SUM(obj_NonGratuated.TransfersPEPFARAdult) AS TransfersPEPFARAdult, 
		                            SUM(obj_NonGratuated.TransfersNotPEPFARChild) AS TransfersNotPEPFARChild, 
		                            SUM(obj_NonGratuated.TransfersNotPEPFARAdult) AS TransfersNotPEPFARAdult, 
		                            SUM(obj_NonGratuated.BecameAdult) AS BecameAdult, 
		                            SUM(obj_NonGratuated.ChildHasServices) AS ChildHasServices, 
		                            SUM(obj_NonGratuated.AdultHasServices) AS AdultHasServices
	                            FROM
	                            (
		                            SELECT
			                            cp.Name As ChiefPartner,
			                            p.Name As Partner,
			                            count(ben.BeneficiaryID) as ID,
			                            ben.FirstName As FirstName, ben.LastName As LastName, cs.Description,
			                            DeathChild = CASE WHEN cs.Description = 'Óbito' AND DATEDIFF(year,CAST(Ben.DateOfBirth AS Date), @lastDate) < 18 THEN 1 ELSE 0 END,
			                            DeathAdult = CASE WHEN cs.Description = 'Óbito' AND DATEDIFF(year,CAST(Ben.DateOfBirth AS Date), @lastDate) >= 18 THEN 1 ELSE 0 END,
			                            LostChild = CASE WHEN cs.Description = 'Perdido' AND DATEDIFF(year,CAST(Ben.DateOfBirth AS Date), @lastDate) < 18 THEN 1 ELSE 0 END,
			                            LostAdult = CASE WHEN cs.Description = 'Perdido' AND DATEDIFF(year,CAST(Ben.DateOfBirth AS Date), @lastDate) >= 18 THEN 1 ELSE 0 END,
			                            GaveUpChild = CASE WHEN cs.Description = 'Desistência' AND DATEDIFF(year,CAST(Ben.DateOfBirth AS Date), @lastDate) < 18 THEN 1 ELSE 0 END,
			                            GaveUpAdult = CASE WHEN cs.Description = 'Desistência' AND DATEDIFF(year,CAST(Ben.DateOfBirth AS Date), @lastDate) >= 18 THEN 1 ELSE 0 END,
			                            TransfersPEPFARChild = CASE WHEN cs.Description = 'Transferido p/ programas de PEPFAR' AND DATEDIFF(year,CAST(Ben.DateOfBirth AS Date), @lastDate) < 18 THEN 1 ELSE 0 END,
			                            TransfersPEPFARAdult = CASE WHEN cs.Description = 'Transferido p/ programas de PEPFAR' AND DATEDIFF(year,CAST(Ben.DateOfBirth AS Date), @lastDate) >= 18 THEN 1 ELSE 0 END,
			                            TransfersNotPEPFARChild = CASE WHEN cs.Description = 'Transferido p/ programas NÃO de PEPFAR)' AND DATEDIFF(year,CAST(Ben.DateOfBirth AS Date), @lastDate) < 18 THEN 1 ELSE 0 END,
			                            TransfersNotPEPFARAdult = CASE WHEN cs.Description = 'Transferido p/ programas NÃO de PEPFAR)' AND DATEDIFF(year,CAST(Ben.DateOfBirth AS Date), @lastDate) >= 18 THEN 1 ELSE 0 END,
			                            BecameAdult = CASE WHEN DATEDIFF(year,CAST(Ben.DateOfBirth AS Date), @lastDate) = 18 THEN 1 ELSE 0 END,
			                            ChildHasServices = CASE WHEN DATEDIFF(year,CAST(Ben.DateOfBirth AS Date), @lastDate) >= 18 AND rvs.BeneficiaryHasServices <> 1 THEN 1 ELSE 0 END,
			                            AdultHasServices = CASE WHEN DATEDIFF(year,CAST(Ben.DateOfBirth AS Date), @lastDate) < 18 AND rvs.BeneficiaryHasServices <> 1 THEN 1 ELSE 0 END
		                            FROM [Partner] p
		                            inner join [Partner] cp on (p.SuperiorId = cp.PartnerID) 
		                            inner join [HouseHold] hh on (p.PartnerID = hh.PartnerID)
		                            inner join [Beneficiary] ben ON ben.HouseholdID = hh.HouseHoldID
		                            inner join [vw_beneficiary_details] benView ON benView.ID = ben.[BeneficiaryID] and ben.[ServicesStatusForReportDate] between @initialDate and @lastDate
		                            inner join [SimpleEntity] se ON ben.[ServicesStatusForReportID] = se.SimpleEntityID and se.Type='ben-services-status'
		                            inner join [ChildStatusHistory] csh 
			                            ON (csh.ChildStatusHistoryID = (SELECT max(csh2.ChildStatusHistoryID) FROM  [ChildStatusHistory] csh2 WHERE csh2.BeneficiaryID = ben.BeneficiaryID 
			                            AND (csh2.EffectiveDate < @lastDate)))
		                            inner join [ChildStatus] cs ON (csh.childStatusID = cs.StatusID AND cs.Description = 'Inicial')
		                            left join [Routine_Visit_Summary] rvs on rvs.BeneficiaryID = ben.BeneficiaryID and rvs.BeneficiaryHasServices <> 1 and rvs.RoutineVisitDate between @initialDate and @lastDate
		                            WHERE p.CollaboratorRoleID = 1
		                            group by cp.Name, p.Name, ben.BeneficiaryID, ben.FirstName, ben.LastName, cs.Description, Ben.DateOfBirth, rvs.BeneficiaryHasServices
	                            )obj_NonGratuated
	                            group by obj_NonGratuated.ChiefPartner--<<ReplaceColumn<<--
                            )
                            obj_NonGratuated
                            ON 
                            (
	                            p.ChiefPartner--<<ReplaceColumn<<--
	                            =
	                            obj_NonGratuated.ChiefPartner--<<ReplaceColumn<<--
                            ) 
                            LEFT JOIN
                            (
	                            SELECT
		                            hiv_obj.ChiefPartner--<<ReplaceColumn<<--
	                                ,
		                            SUM(HIV_P_IN_TARV_Child) AS HIV_P_IN_TARV_Child,
		                            SUM(HIV_P_IN_TARV_Adult) AS HIV_P_IN_TARV_Adult,
		                            SUM(HIV_P_NOT_TARV_Child) AS HIV_P_NOT_TARV_Child,
		                            SUM(HIV_P_NOT_TARV_Adult) AS HIV_P_NOT_TARV_Adult,
		                            SUM(HIV_N_Child) AS HIV_N_Child,
		                            SUM(HIV_N_Adult) AS HIV_N_Adult,
		                            SUM(HIV_KNOWN_NREVEAL_Child) AS HIV_KNOWN_NREVEAL_Child,
		                            SUM(HIV_KNOWN_NREVEAL_Adult) AS  HIV_KNOWN_NREVEAL_Adult,
		                            SUM(HIV_NOT_RECOMMENDED_Child) AS  HIV_NOT_RECOMMENDED_Child,
		                            SUM(HIV_NOT_RECOMMENDED_Adult) AS  HIV_NOT_RECOMMENDED_Adult,
		                            SUM(HIV_UNKNOWN_Child) AS HIV_UNKNOWN_Child,
		                            SUM(HIV_UNKNOWN_Adult) AS HIV_UNKNOWN_Adult
	                            FROM
	                            (
		                            SELECT
			                            cp.Name AS ChiefPartner
			                            ,p.[Name] AS Partner
			                            ,hh.HouseholdName AS HouseholdName
			                            ,ben.RegistrationDate AS RegistrationDate
			                            ,ben.FirstName AS FirstName
			                            ,ben.LastName AS LastName
			                            ,ben.Gender AS Gender
			                            ,ben.DateOfBirth,
			                            HIV_P_IN_TARV_Child = CASE WHEN hs.HIV = 'P' AND hs.HIVInTreatment = 0 AND DATEDIFF(year,CAST(Ben.DateOfBirth AS Date), @lastDate) < 18 THEN 1 ELSE 0 END,
			                            HIV_P_IN_TARV_Adult = CASE WHEN hs.HIV = 'P' AND hs.HIVInTreatment = 0 AND DATEDIFF(year,CAST(Ben.DateOfBirth AS Date), @lastDate) >= 18 THEN 1 ELSE 0 END,
			                            HIV_P_NOT_TARV_Child = CASE WHEN hs.HIV = 'P' AND hs.HIVInTreatment = 1 AND hs.HIVInTreatment = 0 AND DATEDIFF(year,CAST(Ben.DateOfBirth AS Date), @lastDate) < 18 THEN 1 ELSE 0 END,
			                            HIV_P_NOT_TARV_Adult = CASE WHEN hs.HIV = 'P' AND hs.HIVInTreatment = 1 AND DATEDIFF(year,CAST(Ben.DateOfBirth AS Date), @lastDate) >= 18 THEN 1 ELSE 0 END,
			                            HIV_N_Child = CASE WHEN hs.HIV = 'N' AND hs.HIVInTreatment = 0  AND DATEDIFF(year,CAST(Ben.DateOfBirth AS Date), @lastDate) < 18 THEN 1 ELSE 0 END,
			                            HIV_N_Adult = CASE WHEN hs.HIV = 'N' AND DATEDIFF(year,CAST(Ben.DateOfBirth AS Date), @lastDate) >= 18 THEN 1 ELSE 0 END,
			                            HIV_KNOWN_NREVEAL_Child = CASE WHEN hs.HIV = 'U' AND hs.HIVUndisclosedReason = 0 AND hs.HIVInTreatment = 0 AND DATEDIFF(year,CAST(Ben.DateOfBirth AS Date), @lastDate) < 18 THEN 1 ELSE 0 END,
			                            HIV_KNOWN_NREVEAL_Adult = CASE WHEN hs.HIV = 'U' AND hs.HIVUndisclosedReason = 0 AND DATEDIFF(year,CAST(Ben.DateOfBirth AS Date), @lastDate) >= 18 THEN 1 ELSE 0 END,
			                            HIV_NOT_RECOMMENDED_Child = CASE WHEN hs.HIV = 'U' AND hs.HIVUndisclosedReason = 2 AND hs.HIVInTreatment = 0 AND DATEDIFF(year,CAST(Ben.DateOfBirth AS Date), @lastDate) < 18 THEN 1 ELSE 0 END,
			                            HIV_NOT_RECOMMENDED_Adult = CASE WHEN hs.HIV = 'U' AND hs.HIVUndisclosedReason = 2 AND DATEDIFF(year,CAST(Ben.DateOfBirth AS Date), @lastDate) >= 18 THEN 1 ELSE 0 END,
			                            HIV_UNKNOWN_Child = CASE WHEN hs.HIV = 'U' AND hs.HIVUndisclosedReason = 1 AND hs.HIVInTreatment = 0 AND DATEDIFF(year,CAST(Ben.DateOfBirth AS Date), @lastDate) < 18 THEN 1 ELSE 0 END,
			                            HIV_UNKNOWN_Adult = CASE WHEN hs.HIV = 'U' AND hs.HIVUndisclosedReason = 1 AND DATEDIFF(year,CAST(Ben.DateOfBirth AS Date), @lastDate) >= 18 THEN 1 ELSE 0 END
		                            FROM [Partner] p
		                            inner join [Partner] cp on (p.SuperiorId = cp.PartnerID) 
		                            inner join [HouseHold] hh on (p.PartnerID = hh.PartnerID)
		                            inner join [Beneficiary] ben ON ben.HouseholdID = hh.HouseHoldID
		                            inner join [vw_beneficiary_details] benView ON benView.ID = ben.[BeneficiaryID] and ben.[ServicesStatusForReportDate] between @initialDate and @lastDate
		                            inner join [SimpleEntity] se ON ben.[ServicesStatusForReportID] = se.SimpleEntityID and se.Type='ben-services-status'
		                            inner join [ChildStatusHistory] csh 
			                            ON (csh.ChildStatusHistoryID = (SELECT max(csh2.ChildStatusHistoryID) FROM  [ChildStatusHistory] csh2 WHERE csh2.BeneficiaryID = ben.BeneficiaryID
			                            AND (csh2.EffectiveDate < @lastDate)))
		                            inner join [ChildStatus] cs ON (csh.childStatusID = cs.StatusID AND cs.Description = 'Inicial')
		                            inner join [HIVStatus] hs 
					                            ON (hs.HIVStatusID = (SELECT max(hs2.HIVStatusID) FROM  [HIVStatus] hs2 WHERE hs2.BeneficiaryGuid = ben.Beneficiary_guid AND hs2.[InformationDate]<= @lastDate))
	                            )hiv_obj 
	                            group by hiv_obj.ChiefPartner--<<ReplaceColumn<<--
                            )hiv_obj
                            ON 
                            (
	                            p.ChiefPartner--<<ReplaceColumn<<--
	                            =
	                            hiv_obj.ChiefPartner--<<ReplaceColumn<<--
                            )";

            query = (partnerType.Equals("partner")) ? query.Replace("ChiefPartner--<<ReplaceColumn<<--", "Partner") : query;

            return UnitOfWork.DbContext.Database.SqlQuery<MonthlyActiveBeneficiariesSummaryReportDTO>(query,
                    new SqlParameter("initialDate", initialDate),
                    new SqlParameter("lastDate", lastDate)).ToList();
        }

    }
}
