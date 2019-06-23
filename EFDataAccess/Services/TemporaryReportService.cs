using EFDataAccess.UOW;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using EFDataAccess.DTO;
using EFDataAccess.DTO.TemporaryReportListingsDTO;

namespace EFDataAccess.Services
{
    public class TemporaryReportService : BaseService
    {
        public TemporaryReportService(UnitOfWork uow) : base(uow) {}

        public List<SeriesData> getData(String sqlQuery)
        {
            return UnitOfWork.DbContext.Database.SqlQuery<SeriesData>(sqlQuery).ToList();
        }

        public List<SeriesDataComulative> getDataComulative(String sqlQuery)
        {
            return UnitOfWork.DbContext.Database.SqlQuery<SeriesDataComulative>(sqlQuery).ToList();
        }

        public List<SeriesDataChildsTotalInitialGraduatedAdultsNonGraduated> getDataChildsTMG(String sqlQuery)
        {
            return UnitOfWork.DbContext.Database.SqlQuery<SeriesDataChildsTotalInitialGraduatedAdultsNonGraduated>(sqlQuery).ToList();
        }


        /********************************************************************************************************************************************************************
         ************************************************************* TEMPORARY REPORTS **********************************************************************************
         ********************************************************************************************************************************************************************/

        /*
         * #################################################################################
         * ############## PartnersWithSameCollabRolesLevelReport ########################
         * #################################################################################
         */

        public List<PartnersWithSameCollabRolesLevelReportDTO> getPartnersWithSameCollabRolesLevelReport()
        {
            String query = @"SELECT
	                            --superior.[PartnerID] AS 'ID do Superior'
	                            superior.[Name] AS SuperiorName
	                            ,CASE superior.CollaboratorRoleID WHEN 3 THEN 'Supervior' WHEN 2 THEN 'Activista Chefe' ELSE 'Activista Simples' END AS SuperiorLevel
	                            --,activista.[PartnerID] AS 'ID do Activista'
	                            ,activista.[Name] AS PartnerName
	                            ,CASE activista.CollaboratorRoleID WHEN 3 THEN 'Supervior' WHEN 2 THEN 'Activista Chefe' ELSE 'Activista Simples' END AS PartnerLevel
                            FROM  [Partner] activista
                            JOIN  [Partner] superior ON activista.SuperiorId = superior.PartnerID
                            AND activista.CollaboratorRoleID = superior.CollaboratorRoleID";
            return UnitOfWork.DbContext.Database.SqlQuery<PartnersWithSameCollabRolesLevelReportDTO>(query).ToList();
        }


        /*
         * ####################################################################################
         * ############## BeneficiaryListingForInitialReport #################
         * ####################################################################################
         */

        public List<BeneficiaryListingForInitialReportDTO> getBeneficiaryListingForInitialReport(DateTime initialDate, DateTime lastDate)
        {
            String query = @"SELECT
	                            cp.Name AS ChiefPartner
	                            ,p.[Name] AS Partner
	                            ,hh.HouseholdName AS HouseholdName
	                            ,CONVERT(varchar,hh.RegistrationDate,103) AS HouseholdRegistrationDate
	                            ,'CRIANÇA' AS BeneficiaryType
	                            ,cs.Description AS BeneficiaryStatus
	                            ,CONVERT(varchar,csh.EffectiveDate,103) AS StatusDate
	                            ,c.FirstName AS FirstName
	                            ,c.LastName AS LastName
	                            ,c.Gender AS Gender
	                            ,CONVERT(varchar,c.DateOFBirth,103) AS DateOFBirth
	                            ,DATEDIFF(year, CAST(c.DateOfBirth As Date), @lastDate) AS Age
	                            ,ISNULL(ovc.Description,' ') AS OVCType
	                            ,CASE WHEN DATEDIFF(year, CAST(c.DateOfBirth As Date), @lastDate) >= 12 AND c.IsPartSavingGroup = 1 THEN 'Faz parte' ELSE 'Não faz parte' END AS SavingGroupMember_Age12
	                            ,(CASE hs.HIV WHEN 'P' THEN 'POSITIVO' WHEN 'N' THEN 'NEGATIVO' WHEN 'U' THEN 'DESCONHECIDO'END) AS GeralHIVStatus
	                            ,(CASE  
		                            WHEN hs.HIV='P' AND hs.HIVInTreatment = 0 THEN 'ESTÁ EM TARV'
		                            WHEN hs.HIV='P' AND hs.HIVInTreatment=1 THEN 'NÃO ESTÁ EM TARV'
		                            WHEN hs.HIV='U' AND hs.HIVUndisclosedReason=0 THEN 'NÃO REVELADO'
		                            WHEN hs.HIV='U' AND hs.HIVUndisclosedReason=1 THEN 'NÃO CONHECE' 
		                            WHEN hs.HIV='U' AND hs.HIVUndisclosedReason=2 THEN 'NÃO RECOMENDADO'
		                            ELSE '' 
		                            END) AS DetailedHIVStatus
	                            ,(CASE WHEN c.HIVTracked = 'TRUE' THEN 'RASTREADO' ELSE '' END) AS HIVTracked
	                            ,(CASE 
		                            WHEN se.Description = 'Unidade Sanitária' THEN 'Unidade Sanitária' 
		                            WHEN se.Description = 'Comunidade' THEN 'Comunidade'
		                            WHEN se.Description = 'Parceiro Clínico' THEN 'Parceiro Clínico'
		                            WHEN se.Description = 'Nenhuma' THEN 'Nenhuma'
		                            WHEN se.Description = 'Outra' THEN 'Outra' 
		                            ELSE '' 
		                            END) AS FamilyOrigin
                            FROM 
	                             [Partner] p
	                            inner join  [Partner] cp ON (p.SuperiorId = cp.PartnerID) 
	                            inner join  [HouseHold] hh ON (p.PartnerID = hh.PartnerID)
	                            left join  [SimpleEntity] se on (se.SimpleEntityID = hh.FamilyOriginRefID)
	                            inner join  [Child] c ON (hh.HouseHoldID = c.HouseholdID AND DATEDIFF(year, CAST(c.DateOfBirth As Date), @lastDate) < 18)
	                            inner join  [ChildStatusHistory] csh 
	                            ON (csh.ChildStatusHistoryID = (SELECT max(csh2.ChildStatusHistoryID) FROM  [ChildStatusHistory] csh2 WHERE csh2.ChildID = c.ChildID AND csh2.EffectiveDate<= @lastDate))
	                            inner join  [ChildStatus] cs ON (csh.childStatusID = cs.StatusID) 
	                            inner join  [HIVStatus] hs 
	                            ON (hs.HIVStatusID = (SELECT max(hs2.HIVStatusID) FROM  [HIVStatus] hs2 WHERE hs2.ChildID = c.ChildID AND hs2.[InformationDate]<= @lastDate))
	                            left join  [OVCType] ovc ON (ovc.OVCTypeID = c.OVCTypeID)
	                            inner join  [Beneficiary] ben ON ben.ID = c.ChildID AND ben.type='child'
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
	                            and c.childID IN
	                            ( 
		                            SELECT
			                            c.ChildID
		                            FROM  [Child] c 
		                            INNER JOIN  [RoutineVisitMember] rvm ON rvm.ChildID = c.ChildID AND rvm.BeneficiaryHasServices=1
		                            WHERE rvm.RoutineVisitDate BETWEEN @initialDate AND @lastDate
	                            )
	                            --Crianças que não tenham recebido mais de um serviço entre 21/09 até data inicial do intervalo
	                            AND c.ChildID not in 
	                            ( 
			                            SELECT
				                            c.ChildID
			                            FROM  [Child] c 
			                            INNER JOIN  [RoutineVisitMember] rvm ON rvm.ChildID = c.ChildID AND rvm.BeneficiaryHasServices=1
			                            WHERE rvm.RoutineVisitDate BETWEEN 
			                            (
				                            SELECT GetFiscalInitialDate = 
				                            CASE WHEN MONTH(@initialDate) in (1,2,3,4,5,6,7,8) AND YEAR(@initialDate) = YEAR(@lastDate) THEN DATEFROMPARTS(YEAR(@initialDate)-1,09,21)
				                            WHEN MONTH(@initialDate) in (9,10,11,12) AND YEAR(@initialDate) = YEAR(@lastDate) THEN DATEFROMPARTS(YEAR(@initialDate),09,21) 
				                            WHEN MONTH(@initialDate) in (9,10,11,12) AND YEAR(@initialDate) <> YEAR(@lastDate) THEN DATEFROMPARTS(YEAR(@initialDate),09,21)
				                            --WHEN MONTH(@initialDate) in (1,2,3,4,5,6,7,8,9) AND YEAR(@initialDate) <> YEAR(@lastDate) THEN DATEFROMPARTS(YEAR(@initialDate),09,21) 
				                            END
			                            ) 
			                            AND @initialDate
	                            )

                            UNION ALL

                            SELECT
	                            cp.Name AS ChiefPartner
	                            ,p.[Name] AS Partner
	                            ,hh.HouseholdName AS HouseholdName
	                            ,CONVERT(varchar,hh.RegistrationDate,103) AS HouseholdRegistrationDate
	                            ,'ADULTO' AS BeneficiaryType
	                            ,cs.Description AS BeneficiaryStatus
	                            ,CONVERT(varchar,csh.EffectiveDate,103) AS StatusDate
	                            ,a.FirstName AS FirstName
	                            ,a.LastName AS LastName
	                            ,a.Gender AS Gender
	                            ,CONVERT(varchar,a.DateOFBirth,103) AS DateOFBirth
	                            ,DATEDIFF(year, CAST(a.DateOfBirth As Date), @lastDate) AS Age
	                            ,'ADULTO' AS OVCType
	                            ,CASE WHEN DATEDIFF(year, CAST(a.DateOfBirth As Date), @lastDate) >= 12 AND a.IsPartSavingGroup = 1 THEN 'Faz parte' ELSE 'Não faz parte' END AS SavingGroupMember_Age12
	                            ,(CASE hs.HIV WHEN 'P' THEN 'POSITIVO' WHEN 'N' THEN 'NEGATIVO' WHEN 'U' THEN 'DESCONHECIDO'END) AS GeralHIVStatus
	                            ,(CASE  
		                            WHEN hs.HIV='P' AND hs.HIVInTreatment = 0 THEN 'ESTÁ EM TARV'
		                            WHEN hs.HIV='P' AND hs.HIVInTreatment=1 THEN 'NÃO ESTÁ EM TARV'
		                            WHEN hs.HIV='U' AND hs.HIVUndisclosedReason=0 THEN 'NÃO REVELADO'
		                            WHEN hs.HIV='U' AND hs.HIVUndisclosedReason=1 THEN 'NÃO CONHECE'
		                            WHEN hs.HIV='U' AND hs.HIVUndisclosedReason=2 THEN 'NÃO RECOMENDADO'
		                            ELSE '' 
		                            END) AS DetailedHIVStatus
	                            ,(CASE WHEN a.HIVTracked = 'TRUE' THEN 'RASTREADO' ELSE '' END) AS HIVTracked
	                            ,(CASE 
		                            WHEN se.Description = 'Unidade Sanitária' THEN 'Unidade Sanitária' 
		                            WHEN se.Description = 'Comunidade' THEN 'Comunidade'
		                            WHEN se.Description = 'Parceiro Clínico' THEN 'Parceiro Clínico'
		                            WHEN se.Description = 'Nenhuma' THEN 'Nenhuma'
		                            WHEN se.Description = 'Outra' THEN 'Outra' 
		                            ELSE '' 
		                            END) AS FamilyOrigin
                            FROM 
	                             [Partner] p
	                            inner join  [Partner] cp ON (p.SuperiorId = cp.PartnerID) 
	                            inner join  [HouseHold] hh ON (p.PartnerID = hh.PartnerID)
	                            left join  [SimpleEntity] se on (se.SimpleEntityID = hh.FamilyOriginRefID)
	                            inner join  [Adult] a on (hh.HouseHoldID = a.HouseholdID)
	                            inner join  [ChildStatusHistory] csh 
	                            ON (csh.ChildStatusHistoryID = (SELECT max(csh2.ChildStatusHistoryID) FROM  [ChildStatusHistory] csh2 WHERE csh2.AdultID = a.AdultID AND csh2.EffectiveDate <= @lastDate))
	                            inner join  [ChildStatus] cs ON (csh.childStatusID = cs.StatusID) 
	                            inner join  [HIVStatus] hs 
	                            ON (hs.HIVStatusID = (SELECT max(hs2.HIVStatusID) FROM  [HIVStatus] hs2 WHERE hs2.AdultID = a.AdultID AND hs2.[InformationDate] <= @lastDate))
	                            inner join  [Beneficiary] ben ON ben.ID = a.AdultID AND ben.type='adult'
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
	                            --Adultos que tenham recebido mais de um serviço entre a data inicial e data final
	                            and a.AdultID IN
	                            ( 
		                            SELECT
			                            a.AdultID
		                            FROM  [Adult] a 
		                            INNER JOIN  [RoutineVisitMember] rvm ON rvm.AdultID = a.AdultID AND rvm.BeneficiaryHasServices=1
		                            WHERE rvm.RoutineVisitDate BETWEEN @initialDate AND @lastDate
	                            )
	                            --Adultos que não tenham recebido mais de um serviço entre 21/09 até data inicial do intervalo
	                            AND a.AdultID not in 
	                            ( 
			                            SELECT
				                            a.AdultID
			                            FROM  [Adult] a  
			                            INNER JOIN  [RoutineVisitMember] rvm ON rvm.AdultID = a.AdultID AND rvm.BeneficiaryHasServices=1
			                            WHERE rvm.RoutineVisitDate BETWEEN 
			                            (
				                            SELECT GetFiscalInitialDate = 
				                            CASE WHEN MONTH(@initialDate) in (1,2,3,4,5,6,7,8) AND YEAR(@initialDate) = YEAR(@lastDate) THEN DATEFROMPARTS(YEAR(@initialDate)-1,09,21)
				                            WHEN MONTH(@initialDate) in (9,10,11,12) AND YEAR(@initialDate) = YEAR(@lastDate) THEN DATEFROMPARTS(YEAR(@initialDate),09,21) 
				                            WHEN MONTH(@initialDate) in (9,10,11,12) AND YEAR(@initialDate) <> YEAR(@lastDate) THEN DATEFROMPARTS(YEAR(@initialDate),09,21)
				                            --WHEN MONTH(@initialDate) in (1,2,3,4,5,6,7,8,9) AND YEAR(@initialDate) <> YEAR(@lastDate) THEN DATEFROMPARTS(YEAR(@initialDate),09,21) 
				                            END
			                            ) 
			                            AND @initialDate
	                            )
                            ORDER BY cp.Name, p.Name, hh.HouseholdName";
            return UnitOfWork.DbContext.Database.SqlQuery<BeneficiaryListingForInitialReportDTO>(query,
                                                            new SqlParameter("initialDate", initialDate),
                                                            new SqlParameter("lastDate", lastDate)).ToList();
        }



        /*
        * ###################################################################################################
        * ######################## BeneficiaryListingWihtAtLeastOneServiceReport ############################
        * ###################################################################################################
        */

        public List<BasicBeneficiaryListingDTO> getBeneficiaryListingWihtAtLeastOneServiceReport(DateTime initialDate, DateTime lastDate)
        {
            String query = @"SELECT
                            cp.Name As ChiefPartner
                            , p.[Name] As [Partner]
                            , hh.HouseholdName AS HouseholdName
                            , CONVERT(varchar,ben.RegistrationDate,103) AS BeneficiaryRegistrationDate
                            , ben.FirstName As FirstName
                            , ben.LastName As LastName
                            , ben.Gender AS Gender
                            , CONVERT(varchar,ben.DateOfBirth,103) AS DateOfBirth
                            ,DATEDIFF(year, CAST(ben.DateOfBirth As Date), @lastDate) AS YearAge
                            ,DATEDIFF(month, CAST(ben.DateOfBirth As Date), @lastDate) AS MonthAge
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
                            )";

            return UnitOfWork.DbContext.Database.SqlQuery<BasicBeneficiaryListingDTO>(query,
                                                            new SqlParameter("initialDate", initialDate),
                                                            new SqlParameter("lastDate", lastDate)).ToList();
        }


        /*
        * ##################################################################################################
        * ######################## BeneficiaryListingThatChangedHIVStatusReport ############################
        * ##################################################################################################
        */

        public List<BasicBeneficiaryListingDTO> getBeneficiaryListingThatChangedHIVStatusReport(DateTime initialDate, DateTime lastDate)
        {
            String query = @"SELECT
                                cp.Name As ChiefPartner
                                , p.[Name] As [Partner]
                                , hh.HouseholdName AS HouseholdName
                                , CONVERT(varchar,ben.RegistrationDate,103) AS BeneficiaryRegistrationDate
                                , ben.FirstName As FirstName
                                , ben.LastName As LastName
                                , ben.Gender AS Gender
                                , CONVERT(varchar,ben.DateOfBirth,103) AS DateOfBirth
                                ,DATEDIFF(year, CAST(ben.DateOfBirth As Date), @lastDate) AS YearAge
                                ,DATEDIFF(month, CAST(ben.DateOfBirth As Date), @lastDate) AS MonthAge
                            FROM  [Partner] p
                            inner join  [Partner] cp on (p.SuperiorId = cp.PartnerID) 
                            inner join  [HouseHold] hh on (p.PartnerID = hh.PartnerID)
                            inner join  [Beneficiary] ben on (hh.HouseHoldID = ben.HouseholdID)
                            Where p.CollaboratorRoleID = 1 
                            AND ben.[Guid] in 
                            (
	                            SELECT b.Guid from  [Beneficiary] b
	                            inner join  [HIVStatus] hs on (hs.HIVStatusID != b.HIVStatusID and hs.HIVStatusID =
	                            (SELECT max(hs2.HIVStatusID) FROM  [HIVStatus] hs2 WHERE hs2.[InformationDate] BETWEEN @initialDate AND @lastDate AND (hs2.BeneficiaryGuid = b.Guid)))  
                            )";

            return UnitOfWork.DbContext.Database.SqlQuery<BasicBeneficiaryListingDTO>(query,
                                                            new SqlParameter("initialDate", initialDate),
                                                            new SqlParameter("lastDate", lastDate)).ToList();
        }



        /*
         * #########################################################################################
         * ############## BeneficiaryListingWithServicesForRoutineVisitReport ######################
         * #########################################################################################
         */

        public List<BeneficiaryListingForRoutineVisitReportDTO> getBeneficiaryListingWithServicesForRoutineVisitReport(DateTime initialDate, DateTime lastDate)
        {
            String query = @"SELECT  -- Eliminar os registos novos dos repetidos
	                            data.ChiefPartner
	                            , data.[Partner]
	                            ,hh.HouseholdName AS HouseholdName
	                            , CONVERT(varchar,ben.RegistrationDate,103) AS BeneficiaryRegistrationDate
	                            ,data.FirstName
	                            ,data.LastName
	                            ,ben.[Gender]
	                            , CONVERT(varchar,ben.DateOfBirth,103) AS DateOfBirth
	                            ,DATEDIFF(year, CAST(ben.DateOfBirth As Date), @lastDate) AS YearAge
	                            ,DATEDIFF(month, CAST(ben.DateOfBirth As Date), @lastDate) AS MonthAge
                                , CONVERT(varchar,data.RoutineVisitAll,103) AS RoutineVisitAll
                                , CONVERT(varchar,data.RoutineVisitInterval,103) AS RoutineVisitInterval,
	                            NewFinaceAid, RptFinaceAid = CASE WHEN NewFinaceAid = 1 AND RptFinaceAid > 0 THEN (RptFinaceAid-1) ELSE RptFinaceAid END,
	                            NewHealth, RptHealth = CASE WHEN NewHealth = 1 AND RptHealth > 0 THEN (RptHealth-1) ELSE RptHealth END, 
	                            NewFood, RptFood = CASE WHEN NewFood = 1 AND RptFood > 0 THEN (RptFood-1) ELSE RptFood END,
	                            NewEducation, RptEducation = CASE WHEN NewEducation = 1 AND RptEducation > 0 THEN (RptEducation-1) ELSE RptEducation END, 
	                            NewLegalAdvice, RptLegalAdvice = CASE WHEN NewLegalAdvice = 1 AND RptLegalAdvice > 0 THEN (RptLegalAdvice-1) ELSE RptLegalAdvice END, 
	                            NewHousing, RptHousing = CASE WHEN NewHousing = 1 AND RptHousing > 0 THEN (RptHousing-1) ELSE RptHousing END, 
	                            NewSocialAid, RptSocialAid = CASE WHEN NewSocialAid = 1 AND RptSocialAid > 0 THEN (RptSocialAid-1) ELSE RptSocialAid END,
	                            NewDPI, RptDPI = CASE WHEN NewDPI = 1 AND RptDPI > 0 THEN (RptDPI-1) ELSE RptDPI END, 
	                            HIVRisk,
	                            HIVRisk_in_Household
	                            , MUACGreen, MUACYellow, MUACRed
                            FROM
                            (
	                            SELECT  --  Group By Partner
	                            data.ChiefPartner, data.[Partner], data.[Guid]
	                            ,data.FirstName
	                            ,data.LastName
	                            ,data.DateOfBirth
	                            ,data.Age
	                            ,data.RoutineVisitAll
	                            ,data.RoutineVisitInterval,
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
	                            SUM(data.HIVRisk) As HIVRisk, 
	                            SUM(HIVRisk_in_Household) As HIVRisk_in_Household
	                            ,SUM(data.MUACGreen) As MUACGreen, SUM(data.MUACYellow) As MUACYellow, SUM(data.MUACRed) As MUACRed
	                            FROM
	                            (
		                            SELECT  --  Group By Beneficiary
		                            routv_all.ChiefPartner
		                            , routv_all.[Partner]
		                            , routv_all.[Guid]
		                            , routv_all.BeneficiaryFirstName AS FirstName
		                            , routv_all.BeneficiaryLastName AS LastName
		                            , routv_all.DateOfBirth AS DateOfBirth
		                            , routv_all.Age AS Age
		                            , routv_all.RoutineVisitDate AS RoutineVisitAll
		                            , routv_dt.RoutineVisitDate AS RoutineVisitInterval,
		                            ISNULL(SUM(routv_all.FinaceAid),0) As AllFinanceAid, ISNULL(SUM(routv_dt.FinaceAid),0) As DtFinanceAid,
		                            ISNULL(SUM(routv_all.Health),0) As AllHealth, ISNULL(SUM(routv_dt.Health),0) As DtHealth,
		                            ISNULL(SUM(routv_all.Food),0) As AllFood, ISNULL(SUM(routv_dt.Food),0) As DtFood,
		                            ISNULL(SUM(routv_all.Education),0) As AllEducation, ISNULL(SUM(routv_dt.Education),0) As DtEducation,
		                            ISNULL(SUM(routv_all.LegalAdvice),0) As AllLegalAdvice, ISNULL(SUM(routv_dt.LegalAdvice),0) As DtLegalAdvice,
		                            ISNULL(SUM(routv_all.Housing),0) As AllHousing, ISNULL(SUM(routv_dt.Housing),0) As DtHousing,
		                            ISNULL(SUM(routv_all.SocialAid),0) As AllSocialAid, ISNULL(SUM(routv_dt.SocialAid),0) As DtSocialAid,
		                            ISNULL(SUM(routv_all.HIVRisk),0) AS HIVRisk,
		                            HIVRisk_in_Household = CASE WHEN routv_all.RegistrationDate BETWEEN @initialDate and @lastDate AND routv_all.HIVTracked = 1 THEN 1 ELSE 0 END,
		                            ISNULL(SUM(routv_all.DPI),0) As AllDPI,
		                            ISNULL(SUM(routv_dt.DPI),0) As DtDPI,
		                            ISNULL(SUM(routv_dt.MUACGreen),0) As MUACGreen,
		                            ISNULL(SUM(routv_dt.MUACYellow),0) As MUACYellow,
		                            ISNULL(SUM(routv_dt.MUACRed),0) As MUACRed
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
		                            routv_dt.ChiefPartner, routv_dt.[Partner], routv_all.BeneficiaryFirstName ,routv_all.BeneficiaryLastName 
		                            , routv_all.DateOfBirth, routv_all.Age, routv_all.RegistrationDate, routv_all.RoutineVisitDate, routv_dt.RoutineVisitDate, routv_all.HIVTracked
	                            ) data
	                            group by data.ChiefPartner, data.Partner, data.[Guid], data.FirstName
	                            ,data.LastName
	                            ,data.DateOfBirth
	                            ,data.Age
	                            ,data.RoutineVisitAll
	                            ,data.RoutineVisitInterval
                            ) data
                            inner join  [Beneficiary] ben ON (ben.Guid = data.Guid)
                            inner join  [Household] hh ON (hh.HouseHoldID = ben.HouseholdID)";
            return UnitOfWork.DbContext.Database.SqlQuery<BeneficiaryListingForRoutineVisitReportDTO>(query,
                                                            new SqlParameter("initialDate", initialDate),
                                                            new SqlParameter("lastDate", lastDate)).ToList();
        }

        /*
         * #########################################################################################
        //-----------------------------------------REFERENCIAS--------------------------------------
        * ##########################################################################################
        */


        /*
         * ################################################################################
         * ############## ChildListingForActivistReferencesReport ######################
         * ################################################################################
         */

        public List<BeneficiaryListingForReferenceServiceReportDTO> getChildListingForActivistReferencesReport(DateTime initialDate, DateTime lastDate)
        {
            String query = @"SELECT
	                        cp.Name As ChiefPartner,
                            p.Name AS [Partner],
	                        hh.HouseholdName,
	                        c.FirstName,
	                        c.LastName,
	                        c.Gender,
	                        CONVERT(varchar,c.DateOfBirth,103) AS DateOfBirth,
	                        DATEDIFF(year, CAST(c.DateOfBirth As Date), @lastDate) AS YearAge,
	                        DATEDIFF(month, CAST(c.DateOfBirth As Date), @lastDate) AS MonthAge,
	                        CONVERT(varchar,rs.ReferenceDate,103) AS ReferenceDate,
	                        Grouping_Type_Name = (CASE 
		                        WHEN rt.ReferenceName = 'ATS' THEN 'ATS'
		                        WHEN rt.ReferenceName in ('PTV','Testado HIV+','Pré-TARV/IO','Abandono TARV','PPE') THEN 'TARV'
		                        WHEN rt.ReferenceName = 'CCR' THEN 'CCR'
		                        WHEN rt.ReferenceName in ('Maternidade p/ Parto','CPN','CPN Familiar','Consulta Pós-Parto','ITS','Circuncisao Masculina') THEN 'SSR'
		                        WHEN rt.ReferenceName in ('GAVV','Apoio Psico-Social','Posto Policial') THEN 'VGB'
		                        WHEN rt.ReferenceName = 'Atestado de Pobreza' THEN 'Poverty_Proof'
		                        WHEN rt.ReferenceName = 'Registo de Nascimento/Cédula' THEN 'Birth_Registration'
		                        WHEN rt.ReferenceName = 'Bilhete de Identidade (B.I)' THEN 'Identification_Card'
		                        WHEN rt.ReferenceName = 'Integração Escolar' THEN 'School_Integration'
		                        WHEN rt.ReferenceName = 'Curso de Formação Vocacional' THEN 'Vocational_Courses'
		                        WHEN rt.ReferenceName = 'Material Escolar' THEN 'School_Material'
		                        WHEN rt.ReferenceName = 'Cesta Básica' THEN 'Basic_Basket'
		                        WHEN rt.ReferenceName = 'Subsídios Sociais do INAS' THEN 'INAS_Benefit'
                                WHEN rt.ReferenceName not in ('ATS','PTV','Testado HIV+','Pré-TARV/IO','Abandono TARV','PPE','CCR','Maternidade p/ Parto','CPN'
                                ,'CPN Familiar','Consulta Pós-Parto','ITS','Circuncisao Masculina','GAVV','Apoio Psico-Social','Posto Policial'
                                ,'Atestado de Pobreza','Registo de Nascimento/Cédula','Bilhete de Identidade (B.I)','Integração Escolar'
                                ,'Curso de Formação Vocacional','Material Escolar','Cesta Básica','Subsídios Sociais do INAS') THEN 'Others'
	                        ELSE ''
	                        END),
                            rt.ReferenceName,
	                        FieldType = CASE WHEN r.value = '1' THEN 'CheckField' ELSE 'TextField' END,
	                        r.Value
                        FROM  [Partner] p
                        inner join  [Partner] cp on (p.SuperiorId = cp.PartnerID) 
                        inner join  [HouseHold] hh on (p.PartnerID = hh.PartnerID)
                        inner join  [Child] c on (c.HouseHoldID = hh.HouseHoldID)
                        inner join  [ReferenceService] rs on (c.ChildID = rs.ChildID)
                        inner join  [Reference] r on (r.ReferenceServiceID = rs.ReferenceServiceID)
                        inner join  [ReferenceType] rt on (rt.ReferenceTypeID = r.ReferenceTypeID)
                        WHERE p.CollaboratorRoleID = 1 AND c.ChildID IS NOT NULL
                        AND rs.ReferenceDate between @initialDate AND @lastDate
                        AND rt.ReferenceCategory = 'Activist' AND r.Value <> '0' AND r.Value <> '' AND r.Value IS NOT NULL";

            return UnitOfWork.DbContext.Database.SqlQuery<BeneficiaryListingForReferenceServiceReportDTO>(query,
                                                            new SqlParameter("initialDate", initialDate),
                                                            new SqlParameter("lastDate", lastDate)).ToList();
        }




        /*
         * ################################################################################
         * ############## AdultListingForActivistReferencesReport ######################
         * ################################################################################
         */

        public List<BeneficiaryListingForReferenceServiceReportDTO> getAdultListingForActivistReferencesReport(DateTime initialDate, DateTime lastDate)
        {
            String query = @"SELECT
	                        cp.Name As ChiefPartner,
                            p.Name AS [Partner],
	                        hh.HouseholdName,
	                        a.FirstName,
	                        a.LastName,
	                        a.Gender,
	                        CONVERT(varchar,a.DateOfBirth,103) AS DateOfBirth,
	                        DATEDIFF(year, CAST(a.DateOfBirth As Date), @lastDate) AS YearAge,
	                        DATEDIFF(month, CAST(a.DateOfBirth As Date), @lastDate) AS MonthAge,
	                        CONVERT(varchar,rs.ReferenceDate,103) AS ReferenceDate,
	                        Grouping_Type_Name = (CASE 
		                        WHEN rt.ReferenceName = 'ATS' THEN 'ATS'
		                        WHEN rt.ReferenceName in ('PTV','Testado HIV+','Pré-TARV/IO','Abandono TARV','PPE') THEN 'TARV'
		                        WHEN rt.ReferenceName = 'CCR' THEN 'CCR'
		                        WHEN rt.ReferenceName in ('Maternidade p/ Parto','CPN','CPN Familiar','Consulta Pós-Parto','ITS','Circuncisao Masculina') THEN 'SSR'
		                        WHEN rt.ReferenceName in ('GAVV','Apoio Psico-Social','Posto Policial') THEN 'VGB'
		                        WHEN rt.ReferenceName = 'Atestado de Pobreza' THEN 'Poverty_Proof'
		                        WHEN rt.ReferenceName = 'Registo de Nascimento/Cédula' THEN 'Birth_Registration'
		                        WHEN rt.ReferenceName = 'Bilhete de Identidade (B.I)' THEN 'Identification_Card'
		                        WHEN rt.ReferenceName = 'Integração Escolar' THEN 'School_Integration'
		                        WHEN rt.ReferenceName = 'Curso de Formação Vocacional' THEN 'Vocational_Courses'
		                        WHEN rt.ReferenceName = 'Material Escolar' THEN 'School_Material'
		                        WHEN rt.ReferenceName = 'Cesta Básica' THEN 'Basic_Basket'
		                        WHEN rt.ReferenceName = 'Subsídios Sociais do INAS' THEN 'INAS_Benefit'
                                WHEN rt.ReferenceName not in ('ATS','PTV','Testado HIV+','Pré-TARV/IO','Abandono TARV','PPE','CCR','Maternidade p/ Parto','CPN'
                                ,'CPN Familiar','Consulta Pós-Parto','ITS','Circuncisao Masculina','GAVV','Apoio Psico-Social','Posto Policial'
                                ,'Atestado de Pobreza','Registo de Nascimento/Cédula','Bilhete de Identidade (B.I)','Integração Escolar'
                                ,'Curso de Formação Vocacional','Material Escolar','Cesta Básica','Subsídios Sociais do INAS') THEN 'Others'
	                        ELSE ''
	                        END),
                            rt.ReferenceName,
	                        FieldType = CASE WHEN r.value = '1' THEN 'CheckField' ELSE 'TextField' END,
	                        r.Value
                        FROM  [Partner] p
                        inner join  [Partner] cp on (p.SuperiorId = cp.PartnerID) 
                        inner join  [HouseHold] hh on (p.PartnerID = hh.PartnerID)
                        inner join  [Adult] a on (a.HouseHoldID = hh.HouseHoldID)
                        inner join  [ReferenceService] rs on (a.AdultID = rs.AdultID)
                        inner join  [Reference] r on (r.ReferenceServiceID = rs.ReferenceServiceID)
                        inner join  [ReferenceType] rt on (rt.ReferenceTypeID = r.ReferenceTypeID)
                        WHERE p.CollaboratorRoleID = 1 AND a.AdultID IS NOT NULL
                        AND rs.ReferenceDate between @initialDate AND @lastDate
                        AND rt.ReferenceCategory = 'Activist' AND r.Value <> '0' AND r.Value <> '' AND r.Value IS NOT NULL";

            return UnitOfWork.DbContext.Database.SqlQuery<BeneficiaryListingForReferenceServiceReportDTO>(query,
                                                            new SqlParameter("initialDate", initialDate),
                                                            new SqlParameter("lastDate", lastDate)).ToList();
        }





        /*
         * ################################################################################
         * ############## ChildListingForCounterReferencesReport ######################
         * ################################################################################
         */

        public List<BeneficiaryListingForReferenceServiceReportDTO> getChildListingForCounterReferencesReport(DateTime initialDate, DateTime lastDate)
        {
            String query = @"SELECT
	                        cp.Name As ChiefPartner,
                            p.Name AS [Partner],
	                        hh.HouseholdName,
	                        c.FirstName,
	                        c.LastName,
	                        c.Gender,
	                        CONVERT(varchar,c.DateOfBirth,103) AS DateOfBirth,
	                        DATEDIFF(year, CAST(c.DateOfBirth As Date), @lastDate) AS YearAge,
	                        DATEDIFF(month, CAST(c.DateOfBirth As Date), @lastDate) AS MonthAge,
	                        CONVERT(varchar,rs.HealthAttendedDate,103) AS HealthAttendedDate,
	                        CONVERT(varchar,rs.SocialAttendedDate,103) AS SocialAttendedDate,
	                        Grouping_Type_Name = (CASE 
		                        WHEN rt.ReferenceName = 'ATS' THEN 'ATS'
		                        WHEN rt.ReferenceName in ('Pré TARV/IO','Testado HIV+','PPE','PTV','TARV','CD') THEN 'TARV'
		                        WHEN rt.ReferenceName = 'CCR' THEN 'CCR'
		                        WHEN rt.ReferenceName in ('Maternidade p/ parto','CPN','CPN Familiar','Consulta Pós-parto','ITS') THEN 'SSR'
		                        WHEN rt.ReferenceName in ('GAVV') THEN 'VGB'
		                        WHEN rt.ReferenceName = 'Atestado de Pobreza' THEN 'Poverty_Proof'
		                        WHEN rt.ReferenceName = 'Registo de Nascimento/Cédula' THEN 'Birth_Registration'
		                        WHEN rt.ReferenceName = 'Bilhete de Identidade (B.I)' THEN 'Identification_Card'
		                        WHEN rt.ReferenceName = 'Integração Escolar' THEN 'School_Integration'
		                        WHEN rt.ReferenceName = 'Curso de Formação Vocacional' THEN 'Vocational_Courses'
		                        WHEN rt.ReferenceName = 'Material Escolar' THEN 'School_Material'
		                        WHEN rt.ReferenceName = 'Cesta Básica' THEN 'Basic_Basket'
		                        WHEN rt.ReferenceName = 'Subsídios Sociais do INAS' THEN 'INAS_Benefit'
                                WHEN rt.ReferenceName not in ('ATS','Pré TARV/IO','Testado HIV+','PPE','PTV','TARV','CD','CCR','Maternidade p/ parto'
	                            ,'CPN','CPN Familiar','Consulta Pós-parto','ITS','GAVV','Atestado de Pobreza','Registo de Nascimento/Cédula','Bilhete de Identidade (B.I)'
	                            ,'Integração Escolar','Curso de Formação Vocacional','Material Escolar','Cesta Básica','Subsídios Sociais do INAS') THEN 'Others'
	                        ELSE ''
	                        END),
                            rt.ReferenceName,
	                        FieldType = CASE WHEN r.value = '1' THEN 'CheckField' ELSE 'TextField' END,
	                        r.Value
                        FROM  [Partner] p
                        inner join  [Partner] cp on (p.SuperiorId = cp.PartnerID) 
                        inner join  [HouseHold] hh on (p.PartnerID = hh.PartnerID)
                        inner join  [Child] c on (c.HouseHoldID = hh.HouseHoldID)
                        inner join  [ReferenceService] rs on (c.ChildID = rs.ChildID)
                        inner join  [Reference] r on (r.ReferenceServiceID = rs.ReferenceServiceID)
                        inner join  [ReferenceType] rt on (rt.ReferenceTypeID = r.ReferenceTypeID)
                        WHERE p.CollaboratorRoleID = 1 AND c.ChildID IS NOT NULL
                        AND ((rs.HealthAttendedDate between @initialDate AND @lastDate)
                        OR (rs.SocialAttendedDate between @initialDate AND @lastDate))
                        AND rt.ReferenceCategory in ('Health','Social') AND r.Value <> '0' AND r.Value <> '' AND r.Value IS NOT NULL";

            return UnitOfWork.DbContext.Database.SqlQuery<BeneficiaryListingForReferenceServiceReportDTO>(query,
                                                            new SqlParameter("initialDate", initialDate),
                                                            new SqlParameter("lastDate", lastDate)).ToList();
        }




        /*
         * ################################################################################
         * ############## AdultListingForCounterReferencesReport ######################
         * ################################################################################
         */

        public List<BeneficiaryListingForReferenceServiceReportDTO> getAdultListingForCounterReferencesReport(DateTime initialDate, DateTime lastDate)
        {
            String query = @"SELECT
	                        cp.Name As ChiefPartner,
                            p.Name AS [Partner],
	                        hh.HouseholdName,
	                        a.FirstName,
	                        a.LastName,
	                        a.Gender,
	                        CONVERT(varchar,a.DateOfBirth,103) AS DateOfBirth,
	                        DATEDIFF(year, CAST(a.DateOfBirth As Date), @lastDate) AS YearAge,
	                        DATEDIFF(month, CAST(a.DateOfBirth As Date), @lastDate) AS MonthAge,
	                        CONVERT(varchar,rs.HealthAttendedDate,103) AS HealthAttendedDate,
	                        CONVERT(varchar,rs.SocialAttendedDate,103) AS SocialAttendedDate,
	                        Grouping_Type_Name = (CASE 
		                        WHEN rt.ReferenceName = 'ATS' THEN 'ATS'
		                        WHEN rt.ReferenceName in ('Pré TARV/IO','Testado HIV+','PPE','PTV','TARV','CD') THEN 'TARV'
		                        WHEN rt.ReferenceName = 'CCR' THEN 'CCR'
		                        WHEN rt.ReferenceName in ('Maternidade p/ parto','CPN','CPN Familiar','Consulta Pós-parto','ITS') THEN 'SSR'
		                        WHEN rt.ReferenceName in ('GAVV') THEN 'VGB'
		                        WHEN rt.ReferenceName = 'Atestado de Pobreza' THEN 'Poverty_Proof'
		                        WHEN rt.ReferenceName = 'Registo de Nascimento/Cédula' THEN 'Birth_Registration'
		                        WHEN rt.ReferenceName = 'Bilhete de Identidade (B.I)' THEN 'Identification_Card'
		                        WHEN rt.ReferenceName = 'Integração Escolar' THEN 'School_Integration'
		                        WHEN rt.ReferenceName = 'Curso de Formação Vocacional' THEN 'Vocational_Courses'
		                        WHEN rt.ReferenceName = 'Material Escolar' THEN 'School_Material'
		                        WHEN rt.ReferenceName = 'Cesta Básica' THEN 'Basic_Basket'
		                        WHEN rt.ReferenceName = 'Subsídios Sociais do INAS' THEN 'INAS_Benefit'
                                WHEN rt.ReferenceName not in ('ATS','Pré TARV/IO','Testado HIV+','PPE','PTV','TARV','CD','CCR','Maternidade p/ parto'
	                            ,'CPN','CPN Familiar','Consulta Pós-parto','ITS','GAVV','Atestado de Pobreza','Registo de Nascimento/Cédula','Bilhete de Identidade (B.I)'
	                            ,'Integração Escolar','Curso de Formação Vocacional','Material Escolar','Cesta Básica','Subsídios Sociais do INAS') THEN 'Others'
	                        ELSE ''
	                        END),
                            rt.ReferenceName,
	                        FieldType = CASE WHEN r.value = '1' THEN 'CheckField' ELSE 'TextField' END,
	                        r.Value
                        FROM  [Partner] p
                        inner join  [Partner] cp on (p.SuperiorId = cp.PartnerID) 
                        inner join  [HouseHold] hh on (p.PartnerID = hh.PartnerID)
                        inner join  [Adult] a on (a.HouseHoldID = hh.HouseHoldID)
                        inner join  [ReferenceService] rs on (a.AdultID = rs.AdultID)
                        inner join  [Reference] r on (r.ReferenceServiceID = rs.ReferenceServiceID)
                        inner join  [ReferenceType] rt on (rt.ReferenceTypeID = r.ReferenceTypeID)
                        WHERE p.CollaboratorRoleID = 1 AND a.AdultID IS NOT NULL
                        AND ((rs.HealthAttendedDate between @initialDate AND @lastDate)
                        OR (rs.SocialAttendedDate between @initialDate AND @lastDate))
                        AND rt.ReferenceCategory in ('Health','Social') AND r.Value <> '0' AND r.Value <> '' AND r.Value IS NOT NULL";

            return UnitOfWork.DbContext.Database.SqlQuery<BeneficiaryListingForReferenceServiceReportDTO>(query,
                                                            new SqlParameter("initialDate", initialDate),
                                                            new SqlParameter("lastDate", lastDate)).ToList();
        }



        /*
         * ################################################################################
         * ############## HouseholdWithIndeterminatedStatusListingReport ##################
         * ################################################################################
         */

        public List<HouseholdwithIndeterminatedStatusListingDTO> getHouseholdWithIndeterminatedStatusListingReport()
        {
            String query = @"SELECT
	                        cp.Name AS ChiefPartner
	                        ,p.[Name] AS Partner
	                        ,hh.HouseholdName AS HouseholdName
	                        ,hh.RegistrationDate AS HouseholdRegistrationDate
	                        ,(CASE 
		                        WHEN seHHFO.Code = '00' AND seHHFO.Type = 'fam-origin-ref-type' THEN 'Nenhuma' 
		                        WHEN seHHFO.Code = '01' AND seHHFO.Type = 'fam-origin-ref-type' THEN 'Comunidade'
		                        WHEN seHHFO.Code = '02' AND seHHFO.Type = 'fam-origin-ref-type' THEN 'Unidade Sanitária'
		                        WHEN seHHFO.Code = '03' AND seHHFO.Type = 'fam-origin-ref-type' THEN 'Parceiro Clínico'
		                        WHEN seHHFO.Code = '04' AND seHHFO.Type = 'fam-origin-ref-type' THEN 'Outra' 
		                        WHEN seHHFO.Code = '99' AND seHHFO.Type = 'fam-origin-ref-type' THEN 'INDETERMINADO'
		                        ELSE '' 
		                        END) AS FamilyOrigin
		                        ,(CASE 
		                        WHEN seHHFH.Code = '00' AND seHHFH.Type = 'fam-head-type' THEN 'Nenhum' 
		                        WHEN seHHFH.Code = '01' AND seHHFH.Type = 'fam-head-type' THEN 'Avô/Idoso'
		                        WHEN seHHFH.Code = '02' AND seHHFH.Type = 'fam-head-type' THEN 'Criança'
		                        WHEN seHHFH.Code = '03' AND seHHFH.Type = 'fam-head-type' THEN 'Mãe/Pai Solteiro'
		                        WHEN seHHFH.Code = '04' AND seHHFH.Type = 'fam-head-type' THEN 'Doente Crónico Debilitado' 
		                        WHEN seHHFH.Code = '05' AND seHHFH.Type = 'fam-head-type' THEN 'Outro' 
		                        WHEN seHHFH.Code = '99' AND seHHFH.Type = 'fam-head-type' THEN 'INDETERMINADO'
		                        ELSE '' 
		                        END) AS FamilyHead
                        FROM 
	                        [CSI_PROD].[dbo].[Partner] p
	                        inner join [CSI_PROD].[dbo].[Partner] cp ON (p.SuperiorId = cp.PartnerID) 
	                        inner join [CSI_PROD].[dbo].[HouseHold] hh ON (p.PartnerID = hh.PartnerID)
	                        inner join [CSI_PROD].[dbo].[SimpleEntity] seHHFO on (seHHFO.SimpleEntityID = hh.FamilyOriginRefID)
	                        inner join [CSI_PROD].[dbo].[SimpleEntity] seHHFH on (seHHFH.SimpleEntityID = hh.FamilyHeadID)
	                        Where seHHFH.Code = '99' OR seHHFO.Code = '99'
                        ORDER BY cp.Name, p.Name, hh.HouseholdName";

            return UnitOfWork.DbContext.Database.SqlQuery<HouseholdwithIndeterminatedStatusListingDTO>(query).ToList();
        }



        /*
         * ################################################################################
         * ############## BeneficiariesWithIndeterminatedStatusListingReport ##############
         * ################################################################################
         */

        public List<BeneficiariesIndeterminatedStatusListingDTO> getBeneficiariesWithIndeterminatedStatusListingReport()
        {
            String query = @"SELECT
	                        cp.Name AS ChiefPartner
	                        ,p.[Name] AS Partner
	                        ,hh.HouseholdName AS HouseholdName
	                        ,hh.RegistrationDate AS HouseholdRegistrationDate
	                        ,'CRIANÇA' AS BeneficiaryType
	                        ,c.FirstName AS FirstName
	                        ,c.LastName AS LastName
	                        ,c.Gender AS Gender
	                        ,c.DateOFBirth
	                        ,DATEDIFF(year, CAST(c.DateOfBirth As Date), GetDate()) AS Age
	                        ,(CASE  
		                        WHEN ovc.Description = 'Indeterminado' THEN 'INDETERMINADO'
		                        ELSE ovc.Description 
		                        END) AS OVCType
	                        ,(CASE hs.HIV WHEN 'P' THEN 'POSITIVO' WHEN 'N' THEN 'NEGATIVO' WHEN 'U' THEN 'DESCONHECIDO' WHEN 'Z' THEN 'INDETERMINADO' END) AS GeralHIVStatus
	                        ,(CASE  
		                        WHEN hs.HIV='P' AND hs.HIVInTreatment = 0 THEN 'ESTÁ EM TARV'
		                        WHEN hs.HIV='P' AND hs.HIVInTreatment=1 THEN 'NÃO ESTÁ EM TARV'
		                        WHEN hs.HIV='U' AND hs.HIVUndisclosedReason=0 THEN 'NÃO REVELADO'
		                        WHEN hs.HIV='U' AND hs.HIVUndisclosedReason=1 THEN 'NÃO CONHECE' 
		                        WHEN hs.HIV='U' AND hs.HIVUndisclosedReason=2 THEN 'NÃO RECOMENDADO'
		                        WHEN hs.HIV='Z' THEN 'INDETERMINADO'
		                        ELSE '' 
		                        END) AS DetailedHIVStatus
	                        ,(CASE WHEN c.HIVTracked = 'TRUE' THEN 'RASTREADO' ELSE '' END) AS HIVTracked
	                        ,(CASE 
		                        WHEN seBen.Code = '00' AND seBen.Type = 'degree-of-kinship' THEN 'Neto(a)' 
		                        WHEN seBen.Code = '01' AND seBen.Type = 'degree-of-kinship' THEN 'Filho(a)'
		                        WHEN seBen.Code = '02' AND seBen.Type = 'degree-of-kinship' THEN 'Irmão(a)'
		                        WHEN seBen.Code = '03' AND seBen.Type = 'degree-of-kinship' THEN 'Sobrinho(a)'
		                        WHEN seBen.Code = '04' AND seBen.Type = 'degree-of-kinship' THEN 'Avô/Avó' 
		                        WHEN seBen.Code = '05' AND seBen.Type = 'degree-of-kinship' THEN 'Outro' 
		                        WHEN seBen.Code = '06' AND seBen.Type = 'degree-of-kinship' THEN 'Esposo(a)' 
		                        WHEN seBen.Code = '99' AND seBen.Type = 'degree-of-kinship' THEN 'INDETERMINADO'
		                        ELSE '' 
		                        END) AS Degree_of_Kinship
                        FROM 
	                        [CSI_PROD].[dbo].[Partner] p
	                        inner join [CSI_PROD].[dbo].[Partner] cp ON (p.SuperiorId = cp.PartnerID) 
	                        inner join [CSI_PROD].[dbo].[HouseHold] hh ON (p.PartnerID = hh.PartnerID)
	                        inner join [CSI_PROD].[dbo].[Child] c ON (hh.HouseHoldID = c.HouseholdID)
	                        inner join [CSI_PROD].[dbo].[SimpleEntity] seBen on (seBen.SimpleEntityID = c.KinshipToFamilyHeadID)
	                        inner join [CSI_PROD].[dbo].[HIVStatus] hs 
	                        ON (hs.HIVStatusID = (SELECT max(hs2.HIVStatusID) FROM [CSI_PROD].[dbo].[HIVStatus] hs2 WHERE hs2.ChildID = c.ChildID AND hs2.[InformationDate]<= GetDate()))
	                        left join [CSI_PROD].[dbo].[OVCType] ovc ON (ovc.OVCTypeID = c.OVCTypeID)
	                        inner join [CSI_PROD].[dbo].[Beneficiary] ben ON ben.ID = c.ChildID AND ben.type='child'
	                        Where hs.HIV='Z' OR seBen.Code = '99'

                        UNION ALL

                        SELECT
	                        cp.Name AS ChiefPartner
	                        ,p.[Name] AS Partner
	                        ,hh.HouseholdName AS HouseholdName
	                        ,hh.RegistrationDate AS HouseholdRegistrationDate
	                        ,'ADULTO' AS BeneficiaryType
	                        ,a.FirstName AS FirstName
	                        ,a.LastName AS LastName
	                        ,a.Gender AS Gender
	                        ,a.DateOFBirth AS DateOFBirth
	                        ,DATEDIFF(year, CAST(a.DateOfBirth As Date), GetDate()) AS Age
	                        ,'ADULTO' AS OVCType
	                        ,(CASE hs.HIV WHEN 'P' THEN 'POSITIVO' WHEN 'N' THEN 'NEGATIVO' WHEN 'U' THEN 'DESCONHECIDO' WHEN 'Z' THEN 'INDETERMINADO' END) AS GeralHIVStatus
	                        ,(CASE  
		                        WHEN hs.HIV='P' AND hs.HIVInTreatment = 0 THEN 'ESTÁ EM TARV'
		                        WHEN hs.HIV='P' AND hs.HIVInTreatment=1 THEN 'NÃO ESTÁ EM TARV'
		                        WHEN hs.HIV='U' AND hs.HIVUndisclosedReason=0 THEN 'NÃO REVELADO'
		                        WHEN hs.HIV='U' AND hs.HIVUndisclosedReason=1 THEN 'NÃO CONHECE'
		                        WHEN hs.HIV='U' AND hs.HIVUndisclosedReason=2 THEN 'NÃO RECOMENDADO'
		                        WHEN hs.HIV='Z' THEN 'INDETERMINADO'
		                        ELSE '' 
		                        END) AS DetailedHIVStatus
	                        ,(CASE WHEN a.HIVTracked = 'TRUE' THEN 'RASTREADO' ELSE '' END) AS HIVTracked
	                        ,(CASE 
		                        WHEN seBen.Code = '00' AND seBen.Type = 'degree-of-kinship' THEN 'Neto(a)' 
		                        WHEN seBen.Code = '01' AND seBen.Type = 'degree-of-kinship' THEN 'Filho(a)'
		                        WHEN seBen.Code = '02' AND seBen.Type = 'degree-of-kinship' THEN 'Irmão(a)'
		                        WHEN seBen.Code = '03' AND seBen.Type = 'degree-of-kinship' THEN 'Sobrinho(a)'
		                        WHEN seBen.Code = '04' AND seBen.Type = 'degree-of-kinship' THEN 'Avô/Avó' 
		                        WHEN seBen.Code = '05' AND seBen.Type = 'degree-of-kinship' THEN 'Outro' 
		                        WHEN seBen.Code = '06' AND seBen.Type = 'degree-of-kinship' THEN 'Esposo(a)' 
		                        WHEN seBen.Code = '99' AND seBen.Type = 'degree-of-kinship' THEN 'INDETERMINADO'
		                        ELSE '' 
		                        END) AS Degree_of_Kinship
                        FROM 
	                        [CSI_PROD].[dbo].[Partner] p
	                        inner join [CSI_PROD].[dbo].[Partner] cp ON (p.SuperiorId = cp.PartnerID) 
	                        inner join [CSI_PROD].[dbo].[HouseHold] hh ON (p.PartnerID = hh.PartnerID)
	                        inner join [CSI_PROD].[dbo].[Adult] a on (hh.HouseHoldID = a.HouseholdID)
	                        inner join [CSI_PROD].[dbo].[SimpleEntity] seBen on (seBen.SimpleEntityID = a.KinshipToFamilyHeadID)
	                        inner join [CSI_PROD].[dbo].[HIVStatus] hs 
	                        ON (hs.HIVStatusID = (SELECT max(hs2.HIVStatusID) FROM [CSI_PROD].[dbo].[HIVStatus] hs2 WHERE hs2.AdultID = a.AdultID))
	                        inner join [CSI_PROD].[dbo].[Beneficiary] ben ON ben.ID = a.AdultID AND ben.type='adult'
	                        Where hs.HIV='Z' OR seBen.Code = '99'
                        ORDER BY cp.Name, p.Name, hh.HouseholdName";

            return UnitOfWork.DbContext.Database.SqlQuery<BeneficiariesIndeterminatedStatusListingDTO>(query).ToList();
        }

    }
}
