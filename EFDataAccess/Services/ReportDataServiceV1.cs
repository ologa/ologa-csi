using EFDataAccess.UOW;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using EFDataAccess.DTO;
using System.Data;
using VPPS.CSI.Domain;
using System.Collections;
using EFDataAccess.DTO.AgreggatedDTO;

namespace EFDataAccess.Services
{
    public class ReportDataServiceV1 : BaseService
    {
        public ReportDataServiceV1(UnitOfWork uow) : base(uow) { }
        protected ApplicationDbContext db = new ApplicationDbContext();
        /*
        * ###################################################################################################################
        * ###### 1. Beneficiarios Activos T3 FY18(Os que receberam pelo menos um servico neste trimestre +graduados)  #######
        * ###################################################################################################################
        */

        // 1.1. Total de Beneficiarios que receberam pelo menos um servico neste trimestre
        public List<AgeGroupDTO> getGroupOne_OneReport(DateTime initialDateForAnteriorTrimester, DateTime initialDate, DateTime lastDate)
        {
            String query = @"SELECT
	                            age_obj.Partner,
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
		                            ben.Partner,
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
	                            FROM
	                            (
		                            SELECT
			                            cp.Name AS ChiefPartner
			                            ,p.[Name] AS Partner
			                            ,hh.HouseholdName AS HouseholdName
			                            ,ben.RegistrationDate AS RegistrationDate
			                            ,'CRIANÇA' AS BeneficiaryType
			                            ,c.FirstName AS FirstName
			                            ,c.LastName AS LastName
			                            ,c.Gender AS Gender
			                            ,c.DateOfBirth
		                            FROM  [Partner] p
		                            inner join  [Partner] cp on (p.SuperiorId = cp.PartnerID) 
		                            inner join  [HouseHold] hh on (p.PartnerID = hh.PartnerID) 
		                            inner join  [Beneficiary] c on (hh.HouseHoldID = c.HouseholdID and DATEDIFF(year, CAST(c.DateOfBirth As Date), GETDATE()) <= 17)		
		                            inner join  [ChildStatusHistory] csh 
					                            ON (csh.ChildStatusHistoryID = (SELECT max(csh2.ChildStatusHistoryID) FROM  [ChildStatusHistory] csh2 WHERE csh2.ChildID = c.ChildID 
					                            AND (csh2.EffectiveDate BETWEEN @initialDateForAnteriorTrimester AND  @lastDate)))
		                            inner join  [ChildStatus] cs ON (csh.childStatusID = cs.StatusID AND cs.Description = 'Inicial')
		                            inner join  [vw_beneficiary_details] ben ON ben.Guid = c.Beneficiary_guid AND type='child'
		                            AND ben.RegistrationDate between @initialDateForAnteriorTrimester AND @lastDate
		                            --Beneficiarios que tenham recebido mais de um serviço entre a data inicial e data final
		                            AND ben.Guid IN
		                            (
			                            SELECT rvs.Guid
			                            FROM  [Routine_Visit_Summary] rvs
			                            WHERE rvs.RoutineVisitDate BETWEEN @initialDate AND @lastDate AND rvs.BeneficiaryHasServices = 1
		                            )
		                            UNION ALL

		                            SELECT
				                            cp.Name AS ChiefPartner
				                            ,p.[Name] AS Partner
				                            ,hh.HouseholdName AS HouseholdName
				                            ,ben.RegistrationDate AS RegistrationDate
				                            ,'ADULT' AS BeneficiaryType
				                            ,a.FirstName AS FirstName
				                            ,a.LastName AS LastName
				                            ,a.Gender AS Gender
				                            ,a.DateOfBirth
		                            FROM  [Partner] p
		                            inner join  [Partner] cp on (p.SuperiorId = cp.PartnerID) 
		                            inner join  [HouseHold] hh on (p.PartnerID = hh.PartnerID)
		                            inner join  [Beneficiary] a on (hh.HouseHoldID = a.HouseholdID and DATEDIFF(year, CAST(a.DateOfBirth As Date), GETDATE()) > 17)		
		                            inner join  [ChildStatusHistory] csh 
					                            ON (csh.ChildStatusHistoryID = (SELECT max(csh2.ChildStatusHistoryID) FROM  [ChildStatusHistory] csh2 WHERE csh2.AdultID = a.AdultID 
					                            AND (csh2.EffectiveDate BETWEEN @initialDateForAnteriorTrimester AND  @lastDate)))
		                            inner join  [ChildStatus] cs ON (csh.childStatusID = cs.StatusID AND cs.Description = 'Inicial')
		                            inner join  [vw_beneficiary_details] ben ON ben.Guid = a.Beneficiary_guid AND type='adult'
		                            AND ben.RegistrationDate between @initialDateForAnteriorTrimester AND @lastDate
		                            --Beneficiarios que tenham recebido mais de um serviço entre a data inicial e data final
		                            AND ben.Guid IN
		                            (
			                            SELECT rvs.Guid
			                            FROM  [Routine_Visit_Summary] rvs
			                            WHERE rvs.RoutineVisitDate BETWEEN @initialDate AND @lastDate AND rvs.BeneficiaryHasServices = 1
		                            )
	                            )ben
                            ) age_obj
                            group by 
	                            age_obj.Partner";

            return UnitOfWork.DbContext.Database.SqlQuery<AgeGroupDTO>(query,
                                                            new SqlParameter("initialDate", initialDate),
                                                            new SqlParameter("lastDate", lastDate),
                                                            new SqlParameter("initialDateForAnteriorTrimester", initialDateForAnteriorTrimester)).ToList();
        }

        // 1.2. Total de Graduados  ate final do T3
        public List<AgeGroupDTO> getGroupOne_TwoReport(DateTime initialDate, DateTime lastDate)
        {
            String query = @"SELECT
	                            age_obj.Partner,
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
		                            ben.Partner,
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
	                            FROM
	                            (
		                            SELECT
			                            cp.Name AS ChiefPartner
			                            ,p.[Name] AS Partner
			                            ,hh.HouseholdName AS HouseholdName
			                            ,ben.RegistrationDate AS RegistrationDate
			                            ,'CRIANÇA' AS BeneficiaryType
			                            ,cs.Description AS BeneficiaryStatus
			                            ,csh.EffectiveDate AS EffectiveDate
			                            ,ben.FirstName AS FirstName
			                            ,ben.LastName AS LastName
			                            ,ben.Gender AS Gender
			                            ,ben.DateOfBirth
		                            FROM  [Partner] p
		                            inner join  [Partner] cp on (p.SuperiorId = cp.PartnerID) 
		                            inner join  [HouseHold] hh on (p.PartnerID = hh.PartnerID) 
									inner join  [vw_beneficiary_details] ben ON ben.HouseholdID = hh.HouseHoldID AND type='child'
		                            inner join  [ChildStatusHistory] csh 
					                            ON (csh.ChildStatusHistoryID = (SELECT max(csh2.ChildStatusHistoryID) FROM  [ChildStatusHistory] csh2 WHERE csh2.BeneficiaryGuid = ben.Guid
					                            AND (csh2.EffectiveDate BETWEEN @initialDate AND  @lastDate)))
		                            inner join  [ChildStatus] cs ON (csh.childStatusID = cs.StatusID AND cs.Description = 'Graduação')

		                            UNION ALL

		                            SELECT
				                            cp.Name AS ChiefPartner
				                            ,p.[Name] AS Partner
				                            ,hh.HouseholdName AS HouseholdName
				                            ,ben.RegistrationDate AS RegistrationDate
				                            ,'ADULT' AS BeneficiaryType
				                            ,cs.Description AS BeneficiaryStatus
				                            ,csh.EffectiveDate AS EffectiveDate
				                            ,ben.FirstName AS FirstName
				                            ,ben.LastName AS LastName
				                            ,ben.Gender AS Gender
				                            ,ben.DateOfBirth
		                            FROM  [Partner] p
		                            inner join  [Partner] cp on (p.SuperiorId = cp.PartnerID) 
		                            inner join  [HouseHold] hh on (p.PartnerID = hh.PartnerID)
									inner join  [vw_beneficiary_details] ben ON ben.HouseholdID = hh.HouseHoldID AND type='adult'
		                            inner join  [ChildStatusHistory] csh 
					                            ON (csh.ChildStatusHistoryID = (SELECT max(csh2.ChildStatusHistoryID) FROM  [ChildStatusHistory] csh2 WHERE csh2.BeneficiaryGuid = ben.Guid 
					                            AND (csh2.EffectiveDate BETWEEN @initialDate AND  @lastDate)))
		                            inner join  [ChildStatus] cs ON (csh.childStatusID = cs.StatusID AND cs.Description = 'Graduação')
		                            
		                            --ORDER BY cp.Name, p.Name, hh.HouseholdName, BeneficiaryType, FirstName
	                            )ben
                            ) age_obj
                            group by 
	                            age_obj.Partner";

            return UnitOfWork.DbContext.Database.SqlQuery<AgeGroupDTO>(query,
                                                            new SqlParameter("initialDate", initialDate),
                                                            new SqlParameter("lastDate", lastDate)).ToList();
        }


        /*
        * ###########################################################
        * ###### 2. Somente para os identificados no T3 FY18  #######
        * ###########################################################
        */

        // 2.1. Novos Beneficiarios no T3 (Identificados e que receberam pelo menos 1 servico) 
        public List<AgeGroupDTO> getGroupTwo_OneReport(DateTime initialDate, DateTime lastDate)
        {
            String query = @"SELECT
	                            age_obj.Partner,
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
		                            ben.Partner,
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
	                            FROM
	                            (
		                            SELECT
			                            cp.Name AS ChiefPartner
			                            ,p.[Name] AS Partner
			                            ,hh.HouseholdName AS HouseholdName
			                            ,ben.RegistrationDate AS RegistrationDate
			                            ,'CRIANÇA' AS BeneficiaryType
			                            ,ben.FirstName AS FirstName
			                            ,ben.LastName AS LastName
			                            ,ben.Gender AS Gender
			                            ,ben.DateOfBirth
		                            FROM  [Partner] p
		                            inner join  [Partner] cp on (p.SuperiorId = cp.PartnerID) 
		                            inner join  [HouseHold] hh on (p.PartnerID = hh.PartnerID) 
									inner join  [vw_beneficiary_details] ben ON ben.HouseholdID = hh.HouseHoldID AND type='child'
		                            inner join  [ChildStatusHistory] csh 
			                            ON (csh.ChildStatusHistoryID = (SELECT max(csh2.ChildStatusHistoryID) FROM  [ChildStatusHistory] csh2 WHERE csh2.BeneficiaryGuid = ben.Guid 
			                            AND (csh2.EffectiveDate < @lastDate)))
		                            inner join  [ChildStatus] cs ON (csh.childStatusID = cs.StatusID AND cs.Description not in ('Adulto'))
		                            AND ben.RegistrationDate between @initialDate AND @lastDate
		                            --Beneficiarios que tenham recebido mais de um serviço entre a data inicial e data final
		                            AND ben.Guid IN
		                            (
			                            SELECT rvs.Guid
			                            FROM  [Routine_Visit_Summary] rvs
			                            WHERE rvs.RoutineVisitDate BETWEEN @initialDate AND @lastDate AND rvs.BeneficiaryHasServices = 1
		                            )
		                            UNION ALL

		                            SELECT
				                            cp.Name AS ChiefPartner
				                            ,p.[Name] AS Partner
				                            ,hh.HouseholdName AS HouseholdName
				                            ,ben.RegistrationDate AS RegistrationDate
				                            ,'ADULT' AS BeneficiaryType
				                            ,ben.FirstName AS FirstName
				                            ,ben.LastName AS LastName
				                            ,ben.Gender AS Gender
				                            ,ben.DateOfBirth
		                            FROM  [Partner] p
		                            inner join  [Partner] cp on (p.SuperiorId = cp.PartnerID) 
		                            inner join  [HouseHold] hh on (p.PartnerID = hh.PartnerID)
									inner join  [vw_beneficiary_details] ben ON hh.HouseHoldID = ben.HouseholdID AND type='adult'
		                            inner join  [ChildStatusHistory] csh 
						                            ON (csh.ChildStatusHistoryID = (SELECT max(csh2.ChildStatusHistoryID) FROM  [ChildStatusHistory] csh2 WHERE csh2.BeneficiaryGuid = ben.Guid 
						                            AND (csh2.EffectiveDate <  @lastDate)))
		                            inner join  [ChildStatus] cs ON (csh.childStatusID = cs.StatusID AND cs.Description not in ('Adulto'))
		                            
		                            AND ben.RegistrationDate between @initialDate AND @lastDate
		                            --Beneficiarios que tenham recebido mais de um serviço entre a data inicial e data final
		                            AND ben.Guid IN
		                            (
			                            SELECT rvs.Guid
			                            FROM  [Routine_Visit_Summary] rvs
			                            WHERE rvs.RoutineVisitDate BETWEEN @initialDate AND @lastDate AND rvs.BeneficiaryHasServices = 1
		                            )
	                            )ben
                            ) age_obj
                            group by 
	                            age_obj.Partner";

            return UnitOfWork.DbContext.Database.SqlQuery<AgeGroupDTO>(query,
                                                            new SqlParameter("initialDate", initialDate),
                                                            new SqlParameter("lastDate", lastDate)).ToList();
        }


        // 2.2. Origem de Referencia do beneficiario (somente os que receberam servicos)
        public List<ReferenceOriginDTO> getGroupTwo_TwoReport(DateTime initialDate, DateTime lastDate)
        {
            String query = @"SELECT
	                            ben.Partner,
	                            SUM(ben.Com) As Comunidade,
	                            SUM(ben.US) As UnidadeSanitaria,
	                            SUM(ben.Parceiro_Clínico) As ParceiroClinico,
	                            SUM(ben.Nenhuma) As Nenhuma,
	                            SUM(ben.Others) As Outro
                            FROM
                            (
	                            SELECT
		                            cp.Name AS ChiefPartner
		                            ,p.[Name] AS Partner
		                            ,hh.HouseholdName AS HouseholdName
		                            ,ben.RegistrationDate AS RegistrationDate
		                            ,'CRIANÇA' AS BeneficiaryType
		                            ,ben.FirstName AS FirstName
		                            ,ben.LastName AS LastName
		                            ,ben.Gender AS Gender
		                            ,ben.DateOfBirth
		                            ,US = CASE WHEN se.Description = 'Unidade Sanitária' THEN 1 ELSE 0 END
		                            ,Com = CASE WHEN se.Description = 'Comunidade' THEN 1 ELSE 0 END
		                            ,Parceiro_Clínico = CASE WHEN se.Description = 'Parceiro Clínico' THEN 1 ELSE 0 END
		                            ,Nenhuma = CASE WHEN se.Description = 'Nenhuma' THEN 1 ELSE 0 END
		                            ,Others = CASE WHEN se.Description = 'Outra' THEN 1 ELSE 0 END
	                            FROM  [Partner] p
	                            inner join  [Partner] cp on (p.SuperiorId = cp.PartnerID) 
	                            inner join  [HouseHold] hh on (p.PartnerID = hh.PartnerID) 
	                            inner join  [SimpleEntity] se on (se.SimpleEntityID = hh.FamilyOriginRefID)
								inner join  [vw_beneficiary_details] ben ON hh.HouseHoldID = ben.HouseholdID AND ben.type='child'
	                            inner join  [ChildStatusHistory] csh 
		                            ON (csh.ChildStatusHistoryID = (SELECT max(csh2.ChildStatusHistoryID) FROM  [ChildStatusHistory] csh2 WHERE csh2.BeneficiaryGuid = ben.Guid 
		                            AND (csh2.EffectiveDate < @lastDate)))
	                            inner join  [ChildStatus] cs ON (csh.childStatusID = cs.StatusID AND cs.Description not in ('Adulto'))
	                            
	                            AND ben.RegistrationDate between @initialDate AND @lastDate
	                            --Beneficiarios que tenham recebido mais de um serviço entre a data inicial e data final
	                            AND ben.Guid IN
	                            (
		                            SELECT rvs.Guid
		                            FROM  [Routine_Visit_Summary] rvs
		                            WHERE rvs.RoutineVisitDate BETWEEN @initialDate AND @lastDate AND rvs.BeneficiaryHasServices = 1
	                            )
	                            UNION ALL

	                            SELECT
			                            cp.Name AS ChiefPartner
			                            ,p.[Name] AS Partner
			                            ,hh.HouseholdName AS HouseholdName
			                            ,ben.RegistrationDate AS RegistrationDate
			                            ,'ADULT' AS BeneficiaryType
			                            ,ben.FirstName AS FirstName
			                            ,ben.LastName AS LastName
			                            ,ben.Gender AS Gender
			                            ,ben.DateOfBirth
			                            ,US = CASE WHEN se.Description = 'Unidade Sanitária' THEN 1 ELSE 0 END
			                            ,Com = CASE WHEN se.Description = 'Comunidade' THEN 1 ELSE 0 END
			                            ,Parceiro_Clínico = CASE WHEN se.Description = 'Parceiro Clínico' THEN 1 ELSE 0 END
			                            ,Nenhuma = CASE WHEN se.Description = 'Nenhuma' THEN 1 ELSE 0 END
			                            ,Others = CASE WHEN se.Description = 'Outra' THEN 1 ELSE 0 END
	                            FROM  [Partner] p
	                            inner join  [Partner] cp on (p.SuperiorId = cp.PartnerID) 
	                            inner join  [HouseHold] hh on (p.PartnerID = hh.PartnerID)
								inner join  [vw_beneficiary_details] ben ON hh.HouseHoldID = ben.HouseholdID AND ben.type='adult'
	                            inner join  [SimpleEntity] se on (se.SimpleEntityID = hh.FamilyOriginRefID)
	                            inner join  [ChildStatusHistory] csh 
					                            ON (csh.ChildStatusHistoryID = (SELECT max(csh2.ChildStatusHistoryID) FROM  [ChildStatusHistory] csh2 WHERE csh2.BeneficiaryGuid = ben.Guid 
					                            AND (csh2.EffectiveDate <  @lastDate)))
	                            inner join  [ChildStatus] cs ON (csh.childStatusID = cs.StatusID AND cs.Description not in ('Adulto'))
	                            AND ben.RegistrationDate between @initialDate AND @lastDate
	                            --Beneficiarios que tenham recebido mais de um serviço entre a data inicial e data final
	                            AND ben.Guid IN
	                            (
		                            SELECT rvs.Guid
		                            FROM  [Routine_Visit_Summary] rvs
		                            WHERE rvs.RoutineVisitDate BETWEEN @initialDate AND @lastDate AND rvs.BeneficiaryHasServices = 1
	                            )
                            )ben
                            group by ben.Partner";

            return UnitOfWork.DbContext.Database.SqlQuery<ReferenceOriginDTO>(query,
                                                            new SqlParameter("initialDate", initialDate),
                                                            new SqlParameter("lastDate", lastDate)).ToList();
        }


        // 2.3. Percentagem de OVC com seroestado reportado ao parceiro de implementação
        public List<HIVGroupDTO> getGroupTwo_ThreeReport(DateTime initialDate, DateTime lastDate)
        {
            String query = @"SELECT
	                            ben.Partner,
	                            SUM(ben.HIV_P_IN_TARV) As HIV_P_IN_TARV,
	                            SUM(ben.HIV_P_NOT_TARV) As HIV_P_NOT_TARV,
	                            SUM(ben.HIV_N) As HIV_N,
	                            SUM(ben.HIV_KNOWN_NREVEAL) As HIV_KNOWN_NREVEAL,
	                            SUM(ben.HIV_UNKNOWN) As HIV_UNKNOWN,
	                            SUM(ben.HIV_NOT_RECOMMENDED) As HIV_NOT_RECOMMENDED
                            FROM
                            (
	                            SELECT
		                            cp.Name AS ChiefPartner
		                            ,p.[Name] AS Partner
		                            ,hh.HouseholdName AS HouseholdName
		                            ,ben.RegistrationDate AS RegistrationDate
		                            ,'CRIANÇA' AS BeneficiaryType
		                            ,ben.FirstName AS FirstName
		                            ,ben.LastName AS LastName
		                            ,ben.Gender AS Gender
		                            ,ben.DateOfBirth,
		                            HIV_N = CASE WHEN hs.HIV = 'N' THEN 1 ELSE 0 END,
		                            HIV_P_IN_TARV = CASE WHEN hs.HIV = 'P' AND hs.HIVInTreatment = 0 THEN 1 ELSE 0 END,
		                            HIV_P_NOT_TARV = CASE WHEN hs.HIV = 'P' AND hs.HIVInTreatment = 1 THEN 1 ELSE 0 END,
		                            HIV_KNOWN_NREVEAL = CASE WHEN hs.HIV = 'U' AND hs.HIVUndisclosedReason = 0 THEN 1 ELSE 0 END,
		                            HIV_NOT_RECOMMENDED = CASE WHEN hs.HIV = 'U' AND hs.HIVUndisclosedReason = 2 AND DATEDIFF(month, CAST(ben.DateOfBirth As Date), @lastDate) > 24 THEN 1 ELSE 0 END,
		                            HIV_UNKNOWN = CASE WHEN hs.HIV = 'U' AND hs.HIVUndisclosedReason = 1 THEN 1 ELSE 0 END
	                            FROM  [Partner] p
	                            inner join  [Partner] cp on (p.SuperiorId = cp.PartnerID) 
	                            inner join  [HouseHold] hh on (p.PartnerID = hh.PartnerID)
								inner join  [vw_beneficiary_details] ben ON hh.HouseHoldID = ben.HouseholdID AND ben.type='child' 
	                            inner join  [ChildStatusHistory] csh 
		                            ON (csh.ChildStatusHistoryID = (SELECT max(csh2.ChildStatusHistoryID) FROM  [ChildStatusHistory] csh2 WHERE csh2.BeneficiaryGuid = ben.Guid 
		                            AND (csh2.EffectiveDate < @lastDate)))
	                            inner join  [ChildStatus] cs ON (csh.childStatusID = cs.StatusID AND cs.Description not in ('Adulto'))
	                            inner join  [HIVStatus] hs 
		                                    ON (hs.HIVStatusID = (SELECT max(hs2.HIVStatusID) FROM  [HIVStatus] hs2 WHERE hs2.BeneficiaryGuid = ben.Guid AND hs2.[InformationDate]<= @lastDate))
	                            AND ben.RegistrationDate between @initialDate AND @lastDate
	                            UNION ALL

	                            SELECT
			                            cp.Name AS ChiefPartner
			                            ,p.[Name] AS Partner
			                            ,hh.HouseholdName AS HouseholdName
			                            ,ben.RegistrationDate AS RegistrationDate
			                            ,'ADULT' AS BeneficiaryType
			                            ,ben.FirstName AS FirstName
			                            ,ben.LastName AS LastName
			                            ,ben.Gender AS Gender
			                            ,ben.DateOfBirth,
			                            HIV_N = 0,
			                            HIV_P_IN_TARV = CASE WHEN hs.HIV = 'P' AND hs.HIVInTreatment = 0 THEN 1 ELSE 0 END,
			                            HIV_P_NOT_TARV = CASE WHEN hs.HIV = 'P' AND hs.HIVInTreatment = 1 THEN 1 ELSE 0 END,
			                            HIV_KNOWN_NREVEAL = 0,
			                            HIV_NOT_RECOMMENDED = 0,
			                            HIV_UNKNOWN = 0
	                            FROM  [Partner] p
	                            inner join  [Partner] cp on (p.SuperiorId = cp.PartnerID) 
	                            inner join  [HouseHold] hh on (p.PartnerID = hh.PartnerID)
								inner join  [vw_beneficiary_details] ben ON hh.HouseHoldID = ben.HouseholdID AND ben.type='adult'
	                            inner join  [ChildStatusHistory] csh 
				                            ON (csh.ChildStatusHistoryID = (SELECT max(csh2.ChildStatusHistoryID) FROM  [ChildStatusHistory] csh2 WHERE csh2.BeneficiaryGuid = ben.Guid 
				                            AND (csh2.EffectiveDate <  @lastDate)))
	                            inner join  [ChildStatus] cs ON (csh.childStatusID = cs.StatusID AND cs.Description not in ('Adulto'))
	                            inner join  [HIVStatus] hs 
		                                    ON (hs.HIVStatusID = (SELECT max(hs2.HIVStatusID) FROM  [HIVStatus] hs2 WHERE hs2.BeneficiaryGuid = ben.Guid AND hs2.[InformationDate]<= @lastDate))
	                            AND ben.RegistrationDate between @initialDate AND @lastDate
                            )ben
                            group by ben.Partner";

            return UnitOfWork.DbContext.Database.SqlQuery<HIVGroupDTO>(query,
                                                            new SqlParameter("initialDate", initialDate),
                                                            new SqlParameter("lastDate", lastDate)).ToList();
        }


        /*
        * ##################################################################
        * ###### 3. Para todos Activos (T2+T3) (Activos +Graduados)  #######
        * ##################################################################
        */

        // 3.1. Agrupamento Rastreio HIV e Seguimento
        public List<RoutineVisitGroupDTO> getGroupThree_OneReport(DateTime initialDateForAnteriorTrimester, DateTime initialDate, DateTime lastDate)
        {
            String query = @"SELECT
	                            ben.Partner,
	                            SUM(ben.HIVTracked) As HIVTracked,
	                            SUM(ben.HIVRisk) AS HIVRisk,
	                            SUM(ben.FinanceAid) AS FinanceAid, 
	                            SUM(ben.Food) AS Food,
	                            SUM(ben.Housing) AS Housing,
	                            SUM(ben.Education) AS Education,
	                            SUM(ben.Health) AS Health, 
	                            SUM(ben.SocialAid) AS SocialAid,
	                            SUM(ben.LegalAdvice) AS LegalAdvice,
	                            SUM(ben.DPI) AS DPI,
	                            SUM(ben.MUACGreen) AS MUACGreen,
	                            SUM(ben.MUACYellow) AS MUACYellow,
	                            SUM(ben.MUACRed) AS MUACRed
                            FROM
                            (
	                            SELECT
		                            p.[Name] AS Partner,
		                            HIVTracked = CASE WHEN ben.HIVTracked = '1' AND DATEDIFF(month, CAST(ben.DateOfBirth As Date), @lastDate) > 24 THEN 1 ELSE 0 END,
		                            ISNULL(SUM(childrenRoutineVisit.FinaceAid),0) As FinanceAid, 
		                            ISNULL(SUM(childrenRoutineVisit.Health),0) As Health, 
		                            ISNULL(SUM(childrenRoutineVisit.Food),0) As Food,
		                            ISNULL(SUM(childrenRoutineVisit.Education),0) As Education,
		                            ISNULL(SUM(childrenRoutineVisit.LegalAdvice),0) As LegalAdvice,
		                            ISNULL(SUM(childrenRoutineVisit.Housing),0) As Housing,
		                            ISNULL(SUM(childrenRoutineVisit.SocialAid),0) As SocialAid,
		                            DPI = CASE WHEN DATEDIFF(YEAR, CAST(ben.DateOfBirth As Date), childrenRoutineVisit.RoutineVisitDate) < 6 THEN SUM(childrenRoutineVisit.DPI) ELSE 0 END,
		                            HIVRisk = CASE WHEN DATEDIFF(month, CAST(ben.DateOfBirth As Date), childrenRoutineVisit.RoutineVisitDate) > 24 THEN SUM(childrenRoutineVisit.HIVRisk) ELSE 0 END,
		                            MUACGreen = CASE WHEN DATEDIFF(month, CAST(ben.DateOfBirth As Date), childrenRoutineVisit.RoutineVisitDate) BETWEEN 6 AND 59 THEN SUM(childrenRoutineVisit.MUACGreen) ELSE 0 END,
		                            MUACYellow = CASE WHEN DATEDIFF(month, CAST(ben.DateOfBirth As Date), childrenRoutineVisit.RoutineVisitDate) BETWEEN 6 AND 59 THEN SUM(childrenRoutineVisit.MUACYellow) ELSE 0 END,
		                            MUACRed = CASE WHEN DATEDIFF(month, CAST(ben.DateOfBirth As Date), childrenRoutineVisit.RoutineVisitDate) BETWEEN 6 AND 59 THEN SUM(childrenRoutineVisit.MUACRed) ELSE 0 END
	                            FROM  [Partner] p
	                            inner join  [Partner] cp on (p.SuperiorId = cp.PartnerID) 
	                            inner join  [HouseHold] hh on (p.PartnerID = hh.PartnerID) 
								inner join  [vw_beneficiary_details] ben ON hh.HouseHoldID = ben.HouseholdID AND ben.type='child'
	                            inner join  [ChildStatusHistory] csh 
				                            ON (csh.ChildStatusHistoryID = (SELECT max(csh2.ChildStatusHistoryID) FROM  [ChildStatusHistory] csh2 WHERE csh2.BeneficiaryGuid = ben.Guid 
				                            AND (csh2.EffectiveDate BETWEEN @initialDateForAnteriorTrimester AND  @lastDate)))
	                            inner join  [ChildStatus] cs ON (csh.childStatusID = cs.StatusID AND cs.Description in ('Inicial','Graduação'))
	                            inner join [CSI_PROD].[dbo].[Children_Routine_Visit_Summary] childrenRoutineVisit ON childrenRoutineVisit.Guid = ben.Guid
	                            inner join [CSI_PROD].[dbo].[RoutineVisitSupport] rvs on (rvs.RoutineVisitSupportID = childrenRoutineVisit.RoutineVisitSupportID)
	                            inner join [CSI_PROD].[dbo].[RoutineVisitMember] rvm on (rvm.RoutineVisitMemberID = rvs.RoutineVisitMemberID)
	                            inner join [CSI_PROD].[dbo].[RoutineVisit] rv on (rv.RoutineVisitID = rvm.RoutineVisitID)
	                            AND ben.RegistrationDate between @initialDateForAnteriorTrimester AND @lastDate
	                            group by p.Name, ben.HIVTracked, ben.DateOfBirth, childrenRoutineVisit.RoutineVisitDate

	                            UNION ALL

	                            SELECT
			                            p.[Name] AS Partner,
			                            HIVTracked = CASE WHEN ben.HIVTracked = '1' AND DATEDIFF(month, CAST(ben.DateOfBirth As Date), @lastDate) > 24 THEN 1 ELSE 0 END,
			                            ISNULL(SUM(adultsRoutineVisit.FinaceAid),0) As FinanceAid, 
			                            ISNULL(SUM(adultsRoutineVisit.Health),0) As Health, 
			                            ISNULL(SUM(adultsRoutineVisit.Food),0) As Food,
			                            ISNULL(SUM(adultsRoutineVisit.Education),0) As Education,
			                            ISNULL(SUM(adultsRoutineVisit.LegalAdvice),0) As LegalAdvice,
			                            ISNULL(SUM(adultsRoutineVisit.Housing),0) As Housing,
			                            ISNULL(SUM(adultsRoutineVisit.SocialAid),0) As SocialAid,
			                            DPI = CASE WHEN DATEDIFF(YEAR, CAST(ben.DateOfBirth As Date), adultsRoutineVisit.RoutineVisitDate) < 6 THEN SUM(adultsRoutineVisit.DPI) ELSE 0 END,
			                            HIVRisk = CASE WHEN DATEDIFF(month, CAST(ben.DateOfBirth As Date), adultsRoutineVisit.RoutineVisitDate) > 24 THEN SUM(adultsRoutineVisit.HIVRisk) ELSE 0 END,
			                            MUACGreen = CASE WHEN DATEDIFF(month, CAST(ben.DateOfBirth As Date), adultsRoutineVisit.RoutineVisitDate) BETWEEN 6 AND 59 THEN SUM(adultsRoutineVisit.MUACGreen) ELSE 0 END,
			                            MUACYellow = CASE WHEN DATEDIFF(month, CAST(ben.DateOfBirth As Date), adultsRoutineVisit.RoutineVisitDate) BETWEEN 6 AND 59 THEN SUM(adultsRoutineVisit.MUACYellow) ELSE 0 END,
			                            MUACRed = CASE WHEN DATEDIFF(month, CAST(ben.DateOfBirth As Date), adultsRoutineVisit.RoutineVisitDate) BETWEEN 6 AND 59 THEN SUM(adultsRoutineVisit.MUACRed) ELSE 0 END
	                            FROM  [Partner] p
	                            inner join  [Partner] cp on (p.SuperiorId = cp.PartnerID) 
	                            inner join  [HouseHold] hh on (p.PartnerID = hh.PartnerID)
								inner join  [vw_beneficiary_details] ben ON hh.HouseHoldID = ben.HouseholdID AND ben.type='adult'
	                            inner join  [ChildStatusHistory] csh 
					                            ON (csh.ChildStatusHistoryID = (SELECT max(csh2.ChildStatusHistoryID) FROM  [ChildStatusHistory] csh2 WHERE csh2.BeneficiaryGuid = ben.Guid 
					                            AND (csh2.EffectiveDate BETWEEN @initialDateForAnteriorTrimester AND  @lastDate)))
	                            inner join  [ChildStatus] cs ON (csh.childStatusID = cs.StatusID AND cs.Description in ('Inicial','Graduação'))
	                            inner join [CSI_PROD].[dbo].[Adults_Routine_Visit_Summary] adultsRoutineVisit ON adultsRoutineVisit.Guid = ben.Guid
	                            inner join [CSI_PROD].[dbo].[RoutineVisitSupport] rvs on (rvs.RoutineVisitSupportID = adultsRoutineVisit.RoutineVisitSupportID)
	                            inner join [CSI_PROD].[dbo].[RoutineVisitMember] rvm on (rvm.RoutineVisitMemberID = rvs.RoutineVisitMemberID)
	                            inner join [CSI_PROD].[dbo].[RoutineVisit] rv on (rv.RoutineVisitID = rvm.RoutineVisitID)
	                            AND ben.RegistrationDate between @initialDateForAnteriorTrimester AND @lastDate
	                            group by p.Name, ben.HIVTracked, ben.DateOfBirth, adultsRoutineVisit.RoutineVisitDate
                            )ben
                            group by ben.Partner";

            return UnitOfWork.DbContext.Database.SqlQuery<RoutineVisitGroupDTO>(query,
                                                            new SqlParameter("initialDate", initialDate),
                                                            new SqlParameter("lastDate", lastDate),
                                                            new SqlParameter("initialDateForAnteriorTrimester", initialDateForAnteriorTrimester)).ToList();
        }


        // 3.2. ATS													
        public List<AgeGroupDTO> getGroupThree_TwoReport(DateTime initialDateForAnteriorTrimester, DateTime initialDate, DateTime lastDate)
        {
            String query = @"SELECT
	                            age_obj.Partner,
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
		                            ben.Partner,
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
	                            FROM
	                            (
		                            SELECT
			                            cp.Name AS ChiefPartner
			                            ,p.[Name] AS Partner
			                            ,hh.HouseholdName AS HouseholdName
			                            ,ben.RegistrationDate AS RegistrationDate
			                            ,'CRIANÇA' AS BeneficiaryType
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
					                            AND (csh2.EffectiveDate BETWEEN @initialDateForAnteriorTrimester AND  @lastDate)))
		                            inner join  [ChildStatus] cs ON (csh.childStatusID = cs.StatusID AND cs.Description in ('Inicial','Graduação'))
		                            inner join [CSI_PROD].[dbo].[ReferenceService] rs on (c.ChildID = rs.ChildID)
		                            inner join [CSI_PROD].[dbo].[Reference] r on (r.ReferenceServiceID = rs.ReferenceServiceID)
		                            inner join [CSI_PROD].[dbo].[ReferenceType] rt on (rt.ReferenceTypeID = r.ReferenceTypeID)
		                            inner join  [Beneficiary] ben ON ben.ID = c.ChildID AND ben.type='child'
		                            AND ben.RegistrationDate between @initialDateForAnteriorTrimester AND @lastDate
		                            AND c.ChildID IS NOT NULL
		                            AND rt.ReferenceName in ('ATS')

		                            UNION ALL

		                            SELECT
				                            cp.Name AS ChiefPartner
				                            ,p.[Name] AS Partner
				                            ,hh.HouseholdName AS HouseholdName
				                            ,ben.RegistrationDate AS RegistrationDate
				                            ,'ADULT' AS BeneficiaryType
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
						                            AND (csh2.EffectiveDate BETWEEN @initialDateForAnteriorTrimester AND  @lastDate)))
		                            inner join  [ChildStatus] cs ON (csh.childStatusID = cs.StatusID AND cs.Description in ('Inicial','Graduação'))
		                            inner join [CSI_PROD].[dbo].[ReferenceService] rs on (a.AdultID = rs.AdultID)
		                            inner join [CSI_PROD].[dbo].[Reference] r on (r.ReferenceServiceID = rs.ReferenceServiceID)
		                            inner join [CSI_PROD].[dbo].[ReferenceType] rt on (rt.ReferenceTypeID = r.ReferenceTypeID)
		                            inner join  [Beneficiary] ben ON ben.ID = a.AdultID AND ben.type='adult'
		                            AND ben.RegistrationDate between @initialDateForAnteriorTrimester AND @lastDate
		                            AND a.AdultID IS NOT NULL
		                            AND rt.ReferenceName in ('ATS')
	                            )ben
                            ) age_obj
                            group by 
	                            age_obj.Partner";

            return UnitOfWork.DbContext.Database.SqlQuery<AgeGroupDTO>(query,
                                                            new SqlParameter("initialDate", initialDate),
                                                            new SqlParameter("lastDate", lastDate),
                                                            new SqlParameter("initialDateForAnteriorTrimester", initialDateForAnteriorTrimester)).ToList();
        }


        // 3.3. TARV													
        public List<AgeGroupDTO> getGroupThree_ThreeReport(DateTime initialDateForAnteriorTrimester, DateTime initialDate, DateTime lastDate)
        {
            String query = @"SELECT
	                            age_obj.Partner,
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
		                            ben.Partner,
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
	                            FROM
	                            (
		                            SELECT
			                            cp.Name AS ChiefPartner
			                            ,p.[Name] AS Partner
			                            ,hh.HouseholdName AS HouseholdName
			                            ,ben.RegistrationDate AS RegistrationDate
			                            ,'CRIANÇA' AS BeneficiaryType
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
					                            AND (csh2.EffectiveDate BETWEEN @initialDateForAnteriorTrimester AND  @lastDate)))
		                            inner join  [ChildStatus] cs ON (csh.childStatusID = cs.StatusID AND cs.Description in ('Inicial','Graduação'))
		                            inner join [CSI_PROD].[dbo].[ReferenceService] rs on (c.ChildID = rs.ChildID)
		                            inner join [CSI_PROD].[dbo].[Reference] r on (r.ReferenceServiceID = rs.ReferenceServiceID)
		                            inner join [CSI_PROD].[dbo].[ReferenceType] rt on (rt.ReferenceTypeID = r.ReferenceTypeID)
		                            inner join  [Beneficiary] ben ON ben.ID = c.ChildID AND ben.type='child'
		                            AND ben.RegistrationDate between @initialDateForAnteriorTrimester AND @lastDate
		                            AND c.ChildID IS NOT NULL
		                            AND rt.ReferenceName in ('PTV','Testado HIV+','Pré-TARV/IO','Abandono TARV','PPE')

		                            UNION ALL

		                            SELECT
				                            cp.Name AS ChiefPartner
				                            ,p.[Name] AS Partner
				                            ,hh.HouseholdName AS HouseholdName
				                            ,ben.RegistrationDate AS RegistrationDate
				                            ,'ADULT' AS BeneficiaryType
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
						                            AND (csh2.EffectiveDate BETWEEN @initialDateForAnteriorTrimester AND  @lastDate)))
		                            inner join  [ChildStatus] cs ON (csh.childStatusID = cs.StatusID AND cs.Description in ('Inicial','Graduação'))
		                            inner join [CSI_PROD].[dbo].[ReferenceService] rs on (a.AdultID = rs.AdultID)
		                            inner join [CSI_PROD].[dbo].[Reference] r on (r.ReferenceServiceID = rs.ReferenceServiceID)
		                            inner join [CSI_PROD].[dbo].[ReferenceType] rt on (rt.ReferenceTypeID = r.ReferenceTypeID)
		                            inner join  [Beneficiary] ben ON ben.ID = a.AdultID AND ben.type='adult'
		                            AND ben.RegistrationDate between @initialDateForAnteriorTrimester AND @lastDate
		                            AND a.AdultID IS NOT NULL
		                            AND rt.ReferenceName in ('PTV','Testado HIV+','Pré-TARV/IO','Abandono TARV','PPE')
	                            )ben
                            ) age_obj
                            group by 
	                            age_obj.Partner";

            return UnitOfWork.DbContext.Database.SqlQuery<AgeGroupDTO>(query,
                                                            new SqlParameter("initialDate", initialDate),
                                                            new SqlParameter("lastDate", lastDate),
                                                            new SqlParameter("initialDateForAnteriorTrimester", initialDateForAnteriorTrimester)).ToList();
        }


        // 3.4. Number of people referred to health services and other social services by community-based organizations												
        public List<ReferenceGroupDTO> getGroupThree_FourReport(DateTime initialDateForAnteriorTrimester, DateTime initialDate, DateTime lastDate)
        {
            String query = @"SELECT
	                            age_obj.Partner,
	                            ISNULL(age_obj.ATS_Male,0) As ATS_Male,
	                            ISNULL(age_obj.ATS_Female,0) As ATS_Female,
	                            ISNULL(age_obj.TARV_Male,0) As TARV_Male,
	                            ISNULL(age_obj.TARV_Female,0) As TARV_Female,
	                            ISNULL(age_obj.CCR_Male,0) As CCR_Male,
	                            ISNULL(age_obj.CCR_Female,0) As CCR_Female,
	                            ISNULL(age_obj.SSR_Male,0) As SSR_Male,
	                            ISNULL(age_obj.SSR_Female,0) As SSR_Female,
	                            ISNULL(age_obj.VGB_Male,0) As VGB_Male,
	                            ISNULL(age_obj.VGB_Female,0) As VGB_Female,
	                            ISNULL(age_obj.Poverty_Proof_Male,0) As Poverty_Proof_Male,
	                            ISNULL(age_obj.Poverty_Proof_Female,0) As Poverty_Proof_Female,
	                            ISNULL(age_obj.Birth_Registration_Male,0) As Birth_Registration_Male,
	                            ISNULL(age_obj.Birth_Registration_Female,0) As Birth_Registration_Female,
	                            ISNULL(age_obj.Identification_Card_Male,0) As Identification_Card_Male,
	                            ISNULL(age_obj.Identification_Card_Female,0) As Identification_Card_Female,
	                            ISNULL(age_obj.School_Integration_Male,0) As School_Integration_Male,
	                            ISNULL(age_obj.School_Integration_Female,0) As School_Integration_Female,
	                            ISNULL(age_obj.Vocational_Courses_Male,0) As Vocational_Courses_Male,
	                            ISNULL(age_obj.Vocational_Courses_Female,0) As Vocational_Courses_Female,
	                            ISNULL(age_obj.School_Material_Male,0) As School_Material_Male,
	                            ISNULL(age_obj.School_Material_Female,0) As School_Material_Female,
	                            ISNULL(age_obj.Basic_Basket_Male,0) As Basic_Basket_Male,
	                            ISNULL(age_obj.Basic_Basket_Female,0) As Basic_Basket_Female,
	                            ISNULL(age_obj.INAS_Benefit_Male,0) As INAS_Benefit_Male,
	                            ISNULL(age_obj.INAS_Benefit_Female,0) As INAS_Benefit_Female,
	                            ISNULL(age_obj.Others_Male,0) As Others_Male,
	                            ISNULL(age_obj.Others_Female,0) As Others_Female
                            FROM
                            (
	                            SELECT
		                            ben.Partner,
		                            SUM(ben.ATS_Male) As ATS_Male,
	                                SUM(ben.ATS_Female) As ATS_Female,
	                                SUM(ben.TARV_Male) As TARV_Male,
	                                SUM(ben.TARV_Female) As TARV_Female,
	                                SUM(ben.CCR_Male) As CCR_Male,
	                                SUM(ben.CCR_Female) As CCR_Female,
	                                SUM(ben.SSR_Male) As SSR_Male,
	                                SUM(ben.SSR_Female) As SSR_Female,
	                                SUM(ben.VGB_Male) As VGB_Male,
	                                SUM(ben.VGB_Female) As VGB_Female,
	                                SUM(ben.Poverty_Proof_Male) As Poverty_Proof_Male,
	                                SUM(ben.Poverty_Proof_Female) As Poverty_Proof_Female,
	                                SUM(ben.Birth_Registration_Male) As Birth_Registration_Male,
	                                SUM(ben.Birth_Registration_Female) As Birth_Registration_Female,
	                                SUM(ben.Identification_Card_Male) As Identification_Card_Male,
	                                SUM(ben.Identification_Card_Female) As Identification_Card_Female,
	                                SUM(ben.School_Integration_Male) As School_Integration_Male,
	                                SUM(ben.School_Integration_Female) As School_Integration_Female,
	                                SUM(ben.Vocational_Courses_Male) As Vocational_Courses_Male,
	                                SUM(ben.Vocational_Courses_Female) As Vocational_Courses_Female,
	                                SUM(ben.School_Material_Male) As School_Material_Male,
	                                SUM(ben.School_Material_Female) As School_Material_Female,
	                                SUM(ben.Basic_Basket_Male) As Basic_Basket_Male,
	                                SUM(ben.Basic_Basket_Female) As Basic_Basket_Female,
	                                SUM(ben.INAS_Benefit_Male) As INAS_Benefit_Male,
	                                SUM(ben.INAS_Benefit_Female) As INAS_Benefit_Female,
	                                SUM(ben.Others_Male) As Others_Male,
	                                SUM(ben.Others_Female) As Others_Female
	                            FROM
	                            (
		                            SELECT
			                            p.[Name] AS Partner,
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
                                        Others_Male = CASE WHEN c.Gender = 'M' AND rt.ReferenceName not in ('ATS','PTV','Testado HIV+','Pré-TARV/IO','Abandono TARV','PPE','CCR','Maternidade p/ Parto','CPN'
                                        ,'CPN Familiar','Consulta Pós-Parto','ITS','Circuncisao Masculina','GAVV','Apoio Psico-Social','Posto Policial'
                                        ,'Atestado de Pobreza','Registo de Nascimento/Cédula','Bilhete de Identidade (B.I)','Integração Escolar'
                                        ,'Curso de Formação Vocacional','Material Escolar','Cesta Básica','Subsídios Sociais do INAS') THEN 1 ELSE 0 END,
                                        Others_Female = CASE WHEN c.Gender = 'F' AND rt.ReferenceName not in ('ATS','PTV','Testado HIV+','Pré-TARV/IO','Abandono TARV','PPE','CCR','Maternidade p/ Parto','CPN'
                                        ,'CPN Familiar','Consulta Pós-Parto','ITS','Circuncisao Masculina','GAVV','Apoio Psico-Social','Posto Policial'
                                        ,'Atestado de Pobreza','Registo de Nascimento/Cédula','Bilhete de Identidade (B.I)','Integração Escolar'
                                        ,'Curso de Formação Vocacional','Material Escolar','Cesta Básica','Subsídios Sociais do INAS') THEN 1 ELSE 0 END
		                            FROM  [Partner] p
		                            inner join  [Partner] cp on (p.SuperiorId = cp.PartnerID) 
		                            inner join  [HouseHold] hh on (p.PartnerID = hh.PartnerID) 
		                            inner join  [Child] c on (hh.HouseHoldID = c.HouseholdID)
		                            inner join  [ChildStatusHistory] csh 
					                            ON (csh.ChildStatusHistoryID = (SELECT max(csh2.ChildStatusHistoryID) FROM  [ChildStatusHistory] csh2 WHERE csh2.ChildID = c.ChildID 
					                            AND (csh2.EffectiveDate BETWEEN @initialDateForAnteriorTrimester AND  @lastDate)))
		                            inner join  [ChildStatus] cs ON (csh.childStatusID = cs.StatusID AND cs.Description in ('Inicial','Graduação'))
		                            inner join [CSI_PROD].[dbo].[ReferenceService] rs on (c.ChildID = rs.ChildID)
		                            inner join [CSI_PROD].[dbo].[Reference] r on (r.ReferenceServiceID = rs.ReferenceServiceID)
		                            inner join [CSI_PROD].[dbo].[ReferenceType] rt on (rt.ReferenceTypeID = r.ReferenceTypeID)
		                            inner join  [Beneficiary] ben ON ben.ID = c.ChildID AND ben.type='child'
		                            AND ben.RegistrationDate between @initialDateForAnteriorTrimester AND @lastDate
		                            AND c.ChildID IS NOT NULL
		                            --AND rs.ReferenceDate between @initialDate AND @lastDate
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
				                            p.[Name] AS Partner,
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
		                                    Others_Male = CASE WHEN a.Gender = 'M' AND rt.ReferenceName not in ('ATS','PTV','Testado HIV+','Pré-TARV/IO','Abandono TARV','PPE','CCR','Maternidade p/ Parto','CPN'
                                            ,'CPN Familiar','Consulta Pós-Parto','ITS','Circuncisao Masculina','GAVV','Apoio Psico-Social','Posto Policial'
                                            ,'Atestado de Pobreza','Registo de Nascimento/Cédula','Bilhete de Identidade (B.I)','Integração Escolar'
                                            ,'Curso de Formação Vocacional','Material Escolar','Cesta Básica','Subsídios Sociais do INAS') THEN 1 ELSE 0 END,
                                            Others_Female = CASE WHEN a.Gender = 'F' AND rt.ReferenceName not in ('ATS','PTV','Testado HIV+','Pré-TARV/IO','Abandono TARV','PPE','CCR','Maternidade p/ Parto','CPN'
                                            ,'CPN Familiar','Consulta Pós-Parto','ITS','Circuncisao Masculina','GAVV','Apoio Psico-Social','Posto Policial'
                                            ,'Atestado de Pobreza','Registo de Nascimento/Cédula','Bilhete de Identidade (B.I)','Integração Escolar'
                                            ,'Curso de Formação Vocacional','Material Escolar','Cesta Básica','Subsídios Sociais do INAS') THEN 1 ELSE 0 END
		                            FROM  [Partner] p
		                            inner join  [Partner] cp on (p.SuperiorId = cp.PartnerID) 
		                            inner join  [HouseHold] hh on (p.PartnerID = hh.PartnerID)
		                            inner join  [Adult] a on (hh.HouseHoldID = a.HouseholdID)
		                            inner join  [ChildStatusHistory] csh 
						                            ON (csh.ChildStatusHistoryID = (SELECT max(csh2.ChildStatusHistoryID) FROM  [ChildStatusHistory] csh2 WHERE csh2.AdultID = a.AdultID 
						                            AND (csh2.EffectiveDate BETWEEN @initialDateForAnteriorTrimester AND  @lastDate)))
		                            inner join  [ChildStatus] cs ON (csh.childStatusID = cs.StatusID AND cs.Description in ('Inicial','Graduação'))
		                            inner join [CSI_PROD].[dbo].[ReferenceService] rs on (a.AdultID = rs.AdultID)
		                            inner join [CSI_PROD].[dbo].[Reference] r on (r.ReferenceServiceID = rs.ReferenceServiceID)
		                            inner join [CSI_PROD].[dbo].[ReferenceType] rt on (rt.ReferenceTypeID = r.ReferenceTypeID)
		                            inner join  [Beneficiary] ben ON ben.ID = a.AdultID AND ben.type='adult'
		                            AND ben.RegistrationDate between @initialDateForAnteriorTrimester AND @lastDate
		                            AND a.AdultID IS NOT NULL
		                            --AND rs.ReferenceDate between @initialDate AND @lastDate
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
	                            )ben
	                            group by ben.Partner
                            ) age_obj";

            return UnitOfWork.DbContext.Database.SqlQuery<ReferenceGroupDTO>(query,
                                                            new SqlParameter("initialDate", initialDate),
                                                            new SqlParameter("lastDate", lastDate),
                                                            new SqlParameter("initialDateForAnteriorTrimester", initialDateForAnteriorTrimester)).ToList();
        }


        // 3.5. Number of referrals from a community-based organization known to be completed													
        public List<ReferenceGroupDTO> getGroupThree_FiveReport(DateTime initialDateForAnteriorTrimester, DateTime initialDate, DateTime lastDate)
        {
            String query = @"SELECT
	                            age_obj.Partner,
	                            ISNULL(age_obj.ATS_Male,0) As ATS_Male,
	                            ISNULL(age_obj.ATS_Female,0) As ATS_Female,
	                            ISNULL(age_obj.TARV_Male,0) As TARV_Male,
	                            ISNULL(age_obj.TARV_Female,0) As TARV_Female,
	                            ISNULL(age_obj.CCR_Male,0) As CCR_Male,
	                            ISNULL(age_obj.CCR_Female,0) As CCR_Female,
	                            ISNULL(age_obj.SSR_Male,0) As SSR_Male,
	                            ISNULL(age_obj.SSR_Female,0) As SSR_Female,
	                            ISNULL(age_obj.VGB_Male,0) As VGB_Male,
	                            ISNULL(age_obj.VGB_Female,0) As VGB_Female,
	                            ISNULL(age_obj.Poverty_Proof_Male,0) As Poverty_Proof_Male,
	                            ISNULL(age_obj.Poverty_Proof_Female,0) As Poverty_Proof_Female,
	                            ISNULL(age_obj.Birth_Registration_Male,0) As Birth_Registration_Male,
	                            ISNULL(age_obj.Birth_Registration_Female,0) As Birth_Registration_Female,
	                            ISNULL(age_obj.Identification_Card_Male,0) As Identification_Card_Male,
	                            ISNULL(age_obj.Identification_Card_Female,0) As Identification_Card_Female,
	                            ISNULL(age_obj.School_Integration_Male,0) As School_Integration_Male,
	                            ISNULL(age_obj.School_Integration_Female,0) As School_Integration_Female,
	                            ISNULL(age_obj.Vocational_Courses_Male,0) As Vocational_Courses_Male,
	                            ISNULL(age_obj.Vocational_Courses_Female,0) As Vocational_Courses_Female,
	                            ISNULL(age_obj.School_Material_Male,0) As School_Material_Male,
	                            ISNULL(age_obj.School_Material_Female,0) As School_Material_Female,
	                            ISNULL(age_obj.Basic_Basket_Male,0) As Basic_Basket_Male,
	                            ISNULL(age_obj.Basic_Basket_Female,0) As Basic_Basket_Female,
	                            ISNULL(age_obj.INAS_Benefit_Male,0) As INAS_Benefit_Male,
	                            ISNULL(age_obj.INAS_Benefit_Female,0) As INAS_Benefit_Female,
	                            ISNULL(age_obj.Others_Male,0) As Others_Male,
	                            ISNULL(age_obj.Others_Female,0) As Others_Female
                            FROM
                            (
	                            SELECT
		                            ben.Partner,
		                            SUM(ben.ATS_Male) As ATS_Male,
	                                SUM(ben.ATS_Female) As ATS_Female,
	                                SUM(ben.TARV_Male) As TARV_Male,
	                                SUM(ben.TARV_Female) As TARV_Female,
	                                SUM(ben.CCR_Male) As CCR_Male,
	                                SUM(ben.CCR_Female) As CCR_Female,
	                                SUM(ben.SSR_Male) As SSR_Male,
	                                SUM(ben.SSR_Female) As SSR_Female,
	                                SUM(ben.VGB_Male) As VGB_Male,
	                                SUM(ben.VGB_Female) As VGB_Female,
	                                SUM(ben.Poverty_Proof_Male) As Poverty_Proof_Male,
	                                SUM(ben.Poverty_Proof_Female) As Poverty_Proof_Female,
	                                SUM(ben.Birth_Registration_Male) As Birth_Registration_Male,
	                                SUM(ben.Birth_Registration_Female) As Birth_Registration_Female,
	                                SUM(ben.Identification_Card_Male) As Identification_Card_Male,
	                                SUM(ben.Identification_Card_Female) As Identification_Card_Female,
	                                SUM(ben.School_Integration_Male) As School_Integration_Male,
	                                SUM(ben.School_Integration_Female) As School_Integration_Female,
	                                SUM(ben.Vocational_Courses_Male) As Vocational_Courses_Male,
	                                SUM(ben.Vocational_Courses_Female) As Vocational_Courses_Female,
	                                SUM(ben.School_Material_Male) As School_Material_Male,
	                                SUM(ben.School_Material_Female) As School_Material_Female,
	                                SUM(ben.Basic_Basket_Male) As Basic_Basket_Male,
	                                SUM(ben.Basic_Basket_Female) As Basic_Basket_Female,
	                                SUM(ben.INAS_Benefit_Male) As INAS_Benefit_Male,
	                                SUM(ben.INAS_Benefit_Female) As INAS_Benefit_Female,
	                                SUM(ben.Others_Male) As Others_Male,
	                                SUM(ben.Others_Female) As Others_Female
	                            FROM
	                            (
		                            SELECT
			                            p.[Name] AS Partner,
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
		                                Others_Male = CASE WHEN c.Gender = 'M' AND rt.ReferenceName not in ('ATS','Pré TARV/IO','Testado HIV+','PPE','PTV','TARV','CD','CCR','Maternidade p/ parto'
	                                    ,'CPN','CPN Familiar','Consulta Pós-parto','ITS','GAVV','Atestado de Pobreza','Registo de Nascimento/Cédula','Bilhete de Identidade (B.I)'
	                                    ,'Integração Escolar','Curso de Formação Vocacional','Material Escolar','Cesta Básica','Subsídios Sociais do INAS') THEN 1 ELSE 0 END,
		                                Others_Female = CASE WHEN c.Gender = 'F' AND rt.ReferenceName not in ('ATS','Pré TARV/IO','Testado HIV+','PPE','PTV','TARV','CD','CCR','Maternidade p/ parto'
	                                    ,'CPN','CPN Familiar','Consulta Pós-parto','ITS','GAVV','Atestado de Pobreza','Registo de Nascimento/Cédula','Bilhete de Identidade (B.I)'
	                                    ,'Integração Escolar','Curso de Formação Vocacional','Material Escolar','Cesta Básica','Subsídios Sociais do INAS') THEN 1 ELSE 0 END
		                            FROM  [Partner] p
		                            inner join  [Partner] cp on (p.SuperiorId = cp.PartnerID) 
		                            inner join  [HouseHold] hh on (p.PartnerID = hh.PartnerID) 
		                            inner join  [Child] c on (hh.HouseHoldID = c.HouseholdID)
		                            inner join  [ChildStatusHistory] csh 
					                            ON (csh.ChildStatusHistoryID = (SELECT max(csh2.ChildStatusHistoryID) FROM  [ChildStatusHistory] csh2 WHERE csh2.ChildID = c.ChildID 
					                            AND (csh2.EffectiveDate BETWEEN @initialDateForAnteriorTrimester AND  @lastDate)))
		                            inner join  [ChildStatus] cs ON (csh.childStatusID = cs.StatusID AND cs.Description in ('Inicial','Graduação'))
		                            inner join [CSI_PROD].[dbo].[ReferenceService] rs on (c.ChildID = rs.ChildID)
		                            inner join [CSI_PROD].[dbo].[Reference] r on (r.ReferenceServiceID = rs.ReferenceServiceID)
		                            inner join [CSI_PROD].[dbo].[ReferenceType] rt on (rt.ReferenceTypeID = r.ReferenceTypeID)
		                            inner join  [Beneficiary] ben ON ben.ID = c.ChildID AND ben.type='child'
		                            AND ben.RegistrationDate between @initialDateForAnteriorTrimester AND @lastDate
		                            AND c.ChildID IS NOT NULL
		                            --AND rs.ReferenceDate between @initialDate AND @lastDate
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

		                            UNION ALL

		                            SELECT
				                            p.[Name] AS Partner,
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
		                                    Others_Male = CASE WHEN a.Gender = 'M' AND rt.ReferenceName not in ('ATS','Pré TARV/IO','Testado HIV+','PPE','PTV','TARV','CD','CCR','Maternidade p/ parto'
	                                        ,'CPN','CPN Familiar','Consulta Pós-parto','ITS','GAVV','Atestado de Pobreza','Registo de Nascimento/Cédula','Bilhete de Identidade (B.I)'
	                                        ,'Integração Escolar','Curso de Formação Vocacional','Material Escolar','Cesta Básica','Subsídios Sociais do INAS') THEN 1 ELSE 0 END,
		                                    Others_Female = CASE WHEN a.Gender = 'F' AND rt.ReferenceName not in ('ATS','Pré TARV/IO','Testado HIV+','PPE','PTV','TARV','CD','CCR','Maternidade p/ parto'
	                                        ,'CPN','CPN Familiar','Consulta Pós-parto','ITS','GAVV','Atestado de Pobreza','Registo de Nascimento/Cédula','Bilhete de Identidade (B.I)'
	                                        ,'Integração Escolar','Curso de Formação Vocacional','Material Escolar','Cesta Básica','Subsídios Sociais do INAS') THEN 1 ELSE 0 END
		                            FROM  [Partner] p
		                            inner join  [Partner] cp on (p.SuperiorId = cp.PartnerID) 
		                            inner join  [HouseHold] hh on (p.PartnerID = hh.PartnerID)
		                            inner join  [Adult] a on (hh.HouseHoldID = a.HouseholdID)
		                            inner join  [ChildStatusHistory] csh 
						                            ON (csh.ChildStatusHistoryID = (SELECT max(csh2.ChildStatusHistoryID) FROM  [ChildStatusHistory] csh2 WHERE csh2.AdultID = a.AdultID 
						                            AND (csh2.EffectiveDate BETWEEN @initialDateForAnteriorTrimester AND  @lastDate)))
		                            inner join  [ChildStatus] cs ON (csh.childStatusID = cs.StatusID AND cs.Description in ('Inicial','Graduação'))
		                            inner join [CSI_PROD].[dbo].[ReferenceService] rs on (a.AdultID = rs.AdultID)
		                            inner join [CSI_PROD].[dbo].[Reference] r on (r.ReferenceServiceID = rs.ReferenceServiceID)
		                            inner join [CSI_PROD].[dbo].[ReferenceType] rt on (rt.ReferenceTypeID = r.ReferenceTypeID)
		                            inner join  [Beneficiary] ben ON ben.ID = a.AdultID AND ben.type='adult'
		                            AND ben.RegistrationDate between @initialDateForAnteriorTrimester AND @lastDate
		                            AND a.AdultID IS NOT NULL
		                            --AND rs.ReferenceDate between @initialDate AND @lastDate
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
	                            )ben
	                            group by ben.Partner
                            ) age_obj";

            return UnitOfWork.DbContext.Database.SqlQuery<ReferenceGroupDTO>(query,
                                                            new SqlParameter("initialDate", initialDate),
                                                            new SqlParameter("lastDate", lastDate),
                                                            new SqlParameter("initialDateForAnteriorTrimester", initialDateForAnteriorTrimester)).ToList();
        }


        // 4. Numero Beneficiarios Total e saidas sem graduação								
        public List<NonGraduatedBeneficiaryDTO> getGroupFourReport(DateTime initialDate, DateTime lastDate)
        {
            String query = @"SELECT
	                            obj.Partner,
	                            SUM(obj.ID) AS Total,
	                            SUM(obj.Death) As Death, 
	                            SUM(obj.Lost) As Lost, 
	                            SUM(obj.GaveUp) As GaveUp, 
	                            SUM(obj.Others) As Others
                            FROM
                            (
	                            SELECT
		                            cp.Name As ChiefPartner,
		                            p.Name As Partner,
		                            count(c.ChildID) as ID,
		                            ben.FirstName As FirstName, ben.LastName As LastName, cs.Description,
		                            Death = CASE WHEN cs.Description = 'Óbito' THEN 1 ELSE 0 END,
		                            Lost = CASE WHEN cs.Description = 'Perdido' THEN 1 ELSE 0 END,
		                            GaveUp = CASE WHEN cs.Description = 'Desistência' THEN 1 ELSE 0 END,
		                            Others = CASE WHEN cs.Description = 'Outras Saídas' THEN 1 ELSE 0 END
	                            FROM  [Partner] p
	                            inner join  [Partner] cp on (p.SuperiorId = cp.PartnerID) 
	                            inner join  [HouseHold] hh on (p.PartnerID = hh.PartnerID)
	                            inner join  [Child] c on (hh.HouseHoldID = c.HouseholdID)
	                            inner join  [ChildStatusHistory] csh 
		                            ON (csh.ChildStatusHistoryID = (SELECT max(csh2.ChildStatusHistoryID) FROM  [ChildStatusHistory] csh2 WHERE csh2.ChildID = c.ChildID 
		                            AND (csh2.EffectiveDate < @lastDate)))
	                            inner join  [ChildStatus] cs ON (csh.childStatusID = cs.StatusID AND cs.Description not in ('Adulto'))
	                            inner join  [Beneficiary] ben ON ben.ID = c.ChildID AND ben.type='child'
	                            WHERE p.CollaboratorRoleID = 1
	                            group by cp.Name, p.Name, ben.ID, ben.FirstName, ben.LastName, cs.Description

	                            UNION ALL

		                            SELECT
		                            cp.Name As ChiefPartner,
		                            p.Name As Partner,
		                            count(a.AdultId) as ID,
		                            ben.FirstName As FirstName, ben.LastName As LastName, cs.Description,
		                            Death = CASE WHEN cs.Description = 'Óbito' THEN 1 ELSE 0 END,
		                            Lost = CASE WHEN cs.Description = 'Perdido' THEN 1 ELSE 0 END,
		                            GaveUp = CASE WHEN cs.Description = 'Desistência' THEN 1 ELSE 0 END,
		                            Others = CASE WHEN cs.Description = 'Outras Saídas' THEN 1 ELSE 0 END
	                            FROM  [Partner] p
	                            inner join  [Partner] cp on (p.SuperiorId = cp.PartnerID) 
	                            inner join  [HouseHold] hh on (p.PartnerID = hh.PartnerID)
	                            inner join  [Adult] a on (hh.HouseHoldID = a.HouseholdID)
	                            inner join  [ChildStatusHistory] csh 
					                            ON (csh.ChildStatusHistoryID = (SELECT max(csh2.ChildStatusHistoryID) FROM  [ChildStatusHistory] csh2 WHERE csh2.AdultID = a.AdultID 
					                            AND (csh2.EffectiveDate <  @lastDate)))
	                            inner join  [ChildStatus] cs ON (csh.childStatusID = cs.StatusID AND cs.Description not in ('Adulto'))
	                            inner join  [Beneficiary] ben ON ben.ID = a.AdultID AND ben.type='adult'
	                            WHERE p.CollaboratorRoleID = 1
	                            group by cp.Name, p.Name, ben.ID, ben.FirstName, ben.LastName, cs.Description
                            )
                            obj
                            group by obj.Partner";

            return UnitOfWork.DbContext.Database.SqlQuery<NonGraduatedBeneficiaryDTO>(query,
                                                            new SqlParameter("initialDate", initialDate),
                                                            new SqlParameter("lastDate", lastDate)).ToList();
        }

        public List<UniqueEntity> findAllReportDataUniqueEntities() => UnitOfWork.DbContext.Database.SqlQuery<UniqueEntity>("Select SyncGuid, ReportDataID As ID from ReportData").ToList();

        public ReportData findByID(int ID) => UnitOfWork.Repository<ReportData>().GetById(ID);

        public void SaveOrUpdate(ReportData reportData)
        {
            if (reportData.ReportDataID == 0)
            { UnitOfWork.Repository<ReportData>().Add(reportData); }
            else
            { UnitOfWork.Repository<ReportData>().Update(reportData); }
        }

        public int ImportData(string fullPath)
        {
            _logger.Information("IMPORTACAO DE DADOS DOS RELATÓRIOS...");

            UnitOfWork.DbContext.Configuration.AutoDetectChangesEnabled = false;

            string lastGuidToImport = null;
            int ReportDataCount = 0;
            FileImporter imp = new FileImporter();
            DataTable dt2 = imp.ImportFromCSV(fullPath);

            UsersDB = ConvertListToHashtableUsers(findAllUsersUniqueEntities());
            Hashtable ReportDataDB = ConvertListToHashtable(findAllReportDataUniqueEntities());
            ClearMEReportTableBeforeImport(dt2);

            try
            {

                foreach (DataRow row in dt2.Rows)
                {
                    Guid ReportData_guid = new Guid(row["ReportData_guid"].ToString());
                    lastGuidToImport = ReportData_guid.ToString();

                    int ReportDataID = ParseStringToIntSafe(ReportDataDB[ReportData_guid]);
                    ReportData ReportData = (ReportDataID > 0) ? findByID(ReportDataID) : new ReportData();
                    ReportData.ExecutionNumber = int.Parse(row["ExecutionNumber"].ToString());
                    ReportData.QueryCode = row["QueryCode"].ToString();
                    ReportData.SiteName = row["SiteName"].ToString();
                    ReportData.Province = row["Province"].ToString();
                    ReportData.District = row["District"].ToString();
                    ReportData.Field1 = row["Field1"].ToString().Equals("") ? "0" : row["Field1"].ToString();
                    ReportData.Field2 = row["Field2"].ToString().Equals("") ? "0" : row["Field2"].ToString();
                    ReportData.Field3 = row["Field3"].ToString().Equals("") ? "0" : row["Field3"].ToString();
                    ReportData.Field4 = row["Field4"].ToString().Equals("") ? "0" : row["Field4"].ToString();
                    ReportData.Field5 = row["Field5"].ToString().Equals("") ? "0" : row["Field5"].ToString();
                    ReportData.Field6 = row["Field6"].ToString().Equals("") ? "0" : row["Field6"].ToString();
                    ReportData.Field7 = row["Field7"].ToString().Equals("") ? "0" : row["Field7"].ToString();
                    ReportData.Field8 = row["Field8"].ToString().Equals("") ? "0" : row["Field8"].ToString();
                    ReportData.Field9 = row["Field9"].ToString().Equals("") ? "0" : row["Field9"].ToString();
                    ReportData.Field10 = row["Field10"].ToString().Equals("") ? "0" : row["Field10"].ToString();
                    ReportData.Field11 = row["Field11"].ToString().Equals("") ? "0" : row["Field11"].ToString();
                    ReportData.Field12 = row["Field12"].ToString().Equals("") ? "0" : row["Field12"].ToString();
                    ReportData.Field13 = row["Field13"].ToString().Equals("") ? "0" : row["Field13"].ToString();
                    ReportData.Field14 = row["Field14"].ToString().Equals("") ? "0" : row["Field14"].ToString();
                    ReportData.Field15 = row["Field15"].ToString().Equals("") ? "0" : row["Field15"].ToString();
                    ReportData.Field16 = row["Field16"].ToString().Equals("") ? "0" : row["Field16"].ToString();
                    ReportData.Field17 = row["Field17"].ToString().Equals("") ? "0" : row["Field17"].ToString();
                    ReportData.Field18 = row["Field18"].ToString().Equals("") ? "0" : row["Field18"].ToString();
                    ReportData.Field19 = row["Field19"].ToString().Equals("") ? "0" : row["Field19"].ToString();
                    ReportData.Field20 = row["Field20"].ToString().Equals("") ? "0" : row["Field20"].ToString();
                    ReportData.Field21 = row["Field21"].ToString().Equals("") ? "0" : row["Field21"].ToString();
                    ReportData.Field22 = row["Field22"].ToString().Equals("") ? "0" : row["Field22"].ToString();
                    ReportData.Field23 = row["Field23"].ToString().Equals("") ? "0" : row["Field23"].ToString();
                    ReportData.Field24 = row["Field24"].ToString().Equals("") ? "0" : row["Field24"].ToString();
                    ReportData.Field25 = row["Field25"].ToString().Equals("") ? "0" : row["Field25"].ToString();
                    ReportData.Field26 = row["Field26"].ToString().Equals("") ? "0" : row["Field26"].ToString();
                    ReportData.Field27 = row["Field27"].ToString().Equals("") ? "0" : row["Field27"].ToString();
                    ReportData.Field28 = row["Field28"].ToString().Equals("") ? "0" : row["Field28"].ToString();
                    ReportData.Field29 = row["Field29"].ToString().Equals("") ? "0" : row["Field29"].ToString();
                    ReportData.Field30 = row["Field30"].ToString().Equals("") ? "0" : row["Field30"].ToString();
                    ReportData.Field30 = row["Field31"].ToString().Equals("") ? "0" : row["Field31"].ToString();
                    ReportData.Field30 = row["Field32"].ToString().Equals("") ? "0" : row["Field32"].ToString();
                    ReportData.Field30 = row["Field33"].ToString().Equals("") ? "0" : row["Field33"].ToString();
                    ReportData.Field30 = row["Field34"].ToString().Equals("") ? "0" : row["Field34"].ToString();
                    ReportData.Field30 = row["Field35"].ToString().Equals("") ? "0" : row["Field35"].ToString();
                    ReportData.Field30 = row["Field36"].ToString().Equals("") ? "0" : row["Field36"].ToString();
                    ReportData.Field30 = row["Field37"].ToString().Equals("") ? "0" : row["Field37"].ToString();
                    ReportData.Field30 = row["Field38"].ToString().Equals("") ? "0" : row["Field38"].ToString();
                    ReportData.Field30 = row["Field39"].ToString().Equals("") ? "0" : row["Field39"].ToString();
                    ReportData.Field30 = row["Field40"].ToString().Equals("") ? "0" : row["Field40"].ToString();
                    ReportData.Field30 = row["Field41"].ToString().Equals("") ? "0" : row["Field41"].ToString();
                    ReportData.Field30 = row["Field42"].ToString().Equals("") ? "0" : row["Field42"].ToString();
                    ReportData.Field30 = row["Field43"].ToString().Equals("") ? "0" : row["Field43"].ToString();
                    ReportData.Field30 = row["Field44"].ToString().Equals("") ? "0" : row["Field44"].ToString();
                    ReportData.Field30 = row["Field45"].ToString().Equals("") ? "0" : row["Field45"].ToString();
                    ReportData.InitialPositionDate = (row["InitialPositionDate"].ToString()).Length == 0 ? (DateTime?) null : DateTime.Parse(row["InitialPositionDate"].ToString());
                    ReportData.FinalPositionDate = (row["FinalPositionDate"].ToString()).Length == 0 ? (DateTime?) null : DateTime.Parse(row["FinalPositionDate"].ToString());
                    SetCreationDataFields(ReportData, row, ReportData_guid);
                    SetUpdatedDataFields(ReportData, row);
                    SaveOrUpdate(ReportData);
                    ReportDataCount++;
                }
                UnitOfWork.Commit();
                Rename(fullPath, fullPath + IMPORTED);
                return ReportDataCount;
            }
            catch (Exception e)
            {
                _logger.Information("Erro ao importar o Guid : " + lastGuidToImport);
                _logger.Error(e, "Erro ao importar Dados dos Relatórios", null);
                throw e;
            }
            finally
            {
                UnitOfWork.Dispose();
            }

            return ReportDataCount;
        }

        private void ClearMEReportTableBeforeImport(DataTable dt2)
        {
            DataRow FileFirstRow = dt2.Rows[0];
            string SiteName = FileFirstRow["SiteName"].ToString();
            DateTime initialDate = DateTime.Parse(FileFirstRow["InitialPositionDate"].ToString());
            DateTime lastDate = DateTime.Parse(FileFirstRow["FinalPositionDate"].ToString());
            List<string> QueryCodes = new List<string>
            { "1.1", "1.2", "2.1", "2.2", "2.3", "3.1", "3.2", "3.3", "3.4", "3.5", "4", };
            QueryCodes.ForEach(queryCode =>
            {
                List<ReportData> existingRecords = db.ReportData.Where(data =>
                data.InitialPositionDate == initialDate &&
                data.FinalPositionDate == lastDate &&
                data.QueryCode == queryCode && data.SiteName == SiteName)
                .ToList();

                if (existingRecords.Count() > 0)
                {
                    existingRecords.ForEach(record => db.ReportData.Remove(record));
                    db.SaveChanges();
                }
            });
        }
    }
}
