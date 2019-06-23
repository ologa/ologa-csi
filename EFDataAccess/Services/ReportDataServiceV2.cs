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
using System.Reflection;

namespace EFDataAccess.Services
{
    public class ReportDataServiceV2 : BaseService
    {
        public ReportDataServiceV2(UnitOfWork uow) : base(uow) { }

        public static String QueryNewBeneficiariesAgeAndOVCType = 
                                    @"SELECT--  Total de beneficiarios dos activistas(Por Idade), agrupaddo pelo activista chefe
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
                                    SUM(age_obj.IsPartSavingGroup) As IsPartSavingGroup,
									'N/A' As GoToSchool                                   
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
                                    inner join[Partner] cp ON (p.SuperiorId = cp.PartnerID)

                                    inner join[HouseHold] hh ON(p.PartnerID = hh.PartnerID)

                                    inner join[Beneficiary] ben ON(ben.HouseholdID = hh.HouseHoldID)

                                    inner join[SimpleEntity] ss ON(ben.ServicesStatusID = ss.SimpleEntityID)
		                            --inner join[ChildStatusHistory] csh 
		                            --ON(csh.ChildStatusHistoryID = (SELECT max(csh2.ChildStatusHistoryID) FROM  [ChildStatusHistory] csh2 WHERE csh2.BeneficiaryGuid = ben.Guid AND csh2.EffectiveDate <= @lastDate))
		                            --inner join[ChildStatus] cs ON(csh.childStatusID = cs.StatusID AND cs.Description in ('Inicial')) 
		                            inner join[HIVStatus] hs
                                  ON(hs.HIVStatusID = (SELECT max(hs2.HIVStatusID) FROM  [HIVStatus] hs2 WHERE hs2.BeneficiaryID = ben.BeneficiaryID AND hs2.[InformationDate]<= @lastDate))
		                            left join[OVCType] ovc ON(ovc.OVCTypeID = ben.OVCTypeID)

                                    Where p.CollaboratorRoleID = 1
		                            AND (ben.RegistrationDate BETWEEN @initialDate and @lastDate) OR(hh.RegistrationDate BETWEEN @initialDate and @lastDate)

                                    AND ss.Code = '03' and ss.Type = 'ben-services-status'
                                ) age_obj
                                group by
                                    age_obj.ChiefPartner--<<ReplaceColumn<<--";
        

        public String query1 = @"SELECT
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
			                                ,ben.FirstName AS FirstName
			                                ,ben.LastName AS LastName
			                                ,ben.Gender AS Gender
			                                ,ben.DateOfBirth
		                                FROM  [Partner] p
		                                inner join [Partner] cp on (p.SuperiorId = cp.PartnerID) 
		                                inner join [HouseHold] hh on (p.PartnerID = hh.PartnerID) 	
		                                inner join [Beneficiary] ben ON ben.HouseHoldID = hh.HouseHoldID and ben.[ServicesStatusForReportDate] between @initialDate and @lastDate
		                                inner join [SimpleEntity] se ON ben.[ServicesStatusForReportID] = se.SimpleEntityID and se.Type='ben-services-status' and se.Description = 'Activo Verde'
	                                )ben
                                ) age_obj
                                group by 
	                                age_obj.Partner";

        public static String query2 = @"SELECT
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
		                                        ,ben.FirstName AS FirstName
		                                        ,ben.LastName AS LastName
		                                        ,ben.Gender AS Gender
		                                        ,ben.DateOfBirth,
		                                        HIV_N = CASE WHEN hiv.HIV = 'N' THEN 1 ELSE 0 END,
		                                        HIV_P_IN_TARV = CASE WHEN hiv.HIV = 'P' AND hiv.HIVInTreatment = 0 THEN 1 ELSE 0 END,
		                                        HIV_P_NOT_TARV = CASE WHEN hiv.HIV = 'P' AND hiv.HIVInTreatment = 1 THEN 1 ELSE 0 END,
		                                        HIV_KNOWN_NREVEAL = CASE WHEN hiv.HIV = 'U' AND hiv.HIVUndisclosedReason = 0 THEN 1 ELSE 0 END,
		                                        HIV_NOT_RECOMMENDED = CASE WHEN hiv.HIV = 'U' AND hiv.HIVUndisclosedReason = 2 THEN 1 ELSE 0 END,
		                                        HIV_UNKNOWN = CASE WHEN hiv.HIV = 'U' AND hiv.HIVUndisclosedReason = 1 THEN 1 ELSE 0 END
	                                        FROM [Partner] p
	                                        inner join [Partner] cp on (p.SuperiorId = cp.PartnerID) 
	                                        inner join [HouseHold] hh on (p.PartnerID = hh.PartnerID) 
	                                        inner join [Beneficiary] ben ON ben.HouseHoldID = hh.HouseHoldID AND DATEDIFF(year, CAST(ben.DateOfBirth AS Date), @lastDate) < 18
	                                        and ben.[ServicesStatusForReportDate] between @initialDate and @lastDate
	                                        inner join [SimpleEntity] se ON ben.[ServicesStatusForReportID] = se.SimpleEntityID and se.Type='ben-services-status' and se.Description = 'Activo Verde'
	                                        inner join [HIVStatus] hiv ON hiv.BeneficiaryID = ben.BeneficiaryID 
	                                        AND hiv.HIVStatusID IN -- Último estado de HIV no intervalo de datas
	                                        (
		                                        SELECT
			                                        benHIVStatusObj.HIVStatusID
		                                        FROM
		                                        (
			                                        SELECT 
				                                        row_number() OVER (PARTITION BY BeneficiaryID ORDER BY InformationDate DESC) AS LastHIVStatusRow
				                                        --Obter o número da linha de acordo com BeneficiaryID, e ordenado pela Data de Efectividade de forma DESCENDENTE(Último ao Primeiro)
				                                        ,HIVStatusID
				                                        ,BeneficiaryID
			                                        FROM [HIVStatus] hiv
			                                        WHERE hiv.InformationDate BETWEEN @initialDate AND @lastDate
		                                        )benHIVStatusObj
		                                        WHERE benHIVStatusObj.LastHIVStatusRow = 1 
		                                        -- Verifica o último estado de HIV do Intervalo
	                                        )
                                        )ben
                                        group by ben.Partner";

        public String query3 = @"SELECT
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
			                                ,ben.FirstName AS FirstName
			                                ,ben.LastName AS LastName
			                                ,ben.Gender AS Gender
			                                ,ben.DateOfBirth
		                                FROM  [Partner] p
		                                inner join  [Partner] cp on (p.SuperiorId = cp.PartnerID) 
		                                inner join  [HouseHold] hh on (p.PartnerID = hh.PartnerID) 
		                                inner join  [Beneficiary] ben ON ben.HouseholdID = hh.HouseHoldID
		                                AND ben.BeneficiaryID IN 
		                                (-- Beneficiário em que o último estado é Graduado no intervalo de datas
			                                SELECT
				                                benStatusObj.BeneficiaryID
			                                FROM
			                                (
				                                SELECT 
					                                row_number() OVER (PARTITION BY BeneficiaryID ORDER BY EffectiveDate DESC) AS LastStatusRow
					                                --Obter o número da linha de acordo com BeneficiaryID, e ordenado pela Data de Efectividade de forma DESCENDENTE(Último ao Primeiro)
					                                ,[ChildStatusID]
					                                ,BeneficiaryID
				                                FROM [ChildStatusHistory] csh
				                                WHERE csh.[EffectiveDate] <= @lastDate
			                                )benStatusObj
			                                JOIN ChildStatus cs ON cs.StatusID = benStatusObj.ChildStatusID 
			                                AND cs.Description = 'Graduação' AND benStatusObj.LastStatusRow = 1 
			                                -- Verifica se o último estado do Intervalo escolhido é Graduado
		                                )
	                                )ben
                                ) age_obj
                                group by 
	                                age_obj.Partner";

        public String query4 = @"SELECT
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
		                                ,ben.FirstName AS FirstName
		                                ,ben.LastName AS LastName
		                                ,ben.Gender AS Gender
		                                ,ben.DateOfBirth,
		                                HIV_N = CASE WHEN hiv.HIV = 'N' THEN 1 ELSE 0 END,
		                                HIV_P_IN_TARV = CASE WHEN hiv.HIV = 'P' AND hiv.HIVInTreatment = 0 THEN 1 ELSE 0 END,
		                                HIV_P_NOT_TARV = CASE WHEN hiv.HIV = 'P' AND hiv.HIVInTreatment = 1 THEN 1 ELSE 0 END,
		                                HIV_KNOWN_NREVEAL = CASE WHEN hiv.HIV = 'U' AND hiv.HIVUndisclosedReason = 0 THEN 1 ELSE 0 END,
		                                HIV_NOT_RECOMMENDED = CASE WHEN hiv.HIV = 'U' AND hiv.HIVUndisclosedReason = 2 THEN 1 ELSE 0 END,
		                                HIV_UNKNOWN = CASE WHEN hiv.HIV = 'U' AND hiv.HIVUndisclosedReason = 1 THEN 1 ELSE 0 END
	                                FROM  [Partner] p
	                                inner join  [Partner] cp on (p.SuperiorId = cp.PartnerID) 
	                                inner join  [HouseHold] hh on (p.PartnerID = hh.PartnerID) 
	                                inner join  [Beneficiary] ben ON ben.HouseholdID = hh.HouseHoldID AND DATEDIFF(year, CAST(ben.DateOfBirth AS Date), @lastDate) < 18
	                                AND ben.BeneficiaryID IN 
	                                (-- Beneficiário em que o último estado é Graduado no intervalo de datas
		                                SELECT
			                                benStatusObj.BeneficiaryID
		                                FROM
		                                (
			                                SELECT 
				                                row_number() OVER (PARTITION BY BeneficiaryID ORDER BY EffectiveDate DESC) AS LastStatusRow
				                                --Obter o número da linha de acordo com BeneficiaryID, e ordenado pela Data de Efectividade de forma DESCENDENTE(Último ao Primeiro)
				                                ,[ChildStatusID]
				                                ,BeneficiaryID
			                                FROM [ChildStatusHistory] csh
			                                WHERE csh.[EffectiveDate] <= @lastDate
		                                )benStatusObj
		                                JOIN ChildStatus cs ON cs.StatusID = benStatusObj.ChildStatusID 
		                                AND cs.Description = 'Graduação' AND benStatusObj.LastStatusRow = 1 
		                                -- Verifica se o último estado do Intervalo escolhido é Graduado
	                                )
	                                inner join [HIVStatus] hiv ON hiv.BeneficiaryID = ben.BeneficiaryID 
	                                AND hiv.HIVStatusID IN -- Último estado de HIV no intervalo de datas
	                                (
		                                SELECT
			                                benHIVStatusObj.HIVStatusID
		                                FROM
		                                (
			                                SELECT 
				                                row_number() OVER (PARTITION BY BeneficiaryID ORDER BY InformationDate DESC) AS LastHIVStatusRow
				                                --Obter o número da linha de acordo com BeneficiaryID, e ordenado pela Data de Efectividade de forma DESCENDENTE(Último ao Primeiro)
				                                ,HIVStatusID
				                                ,BeneficiaryID
			                                FROM [HIVStatus] hiv
			                                WHERE hiv.InformationDate BETWEEN @initialDate AND @lastDate
		                                )benHIVStatusObj
		                                WHERE benHIVStatusObj.LastHIVStatusRow = 1 
		                                -- Verifica o último estado de HIV do Intervalo
	                                )
                                )ben
                                group by ben.Partner";

        public String query5 = @"SELECT
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
			                                ,ben.FirstName AS FirstName
			                                ,ben.LastName AS LastName
			                                ,ben.Gender AS Gender
			                                ,ben.DateOfBirth
		                                FROM  [Partner] p
		                                inner join  [Partner] cp on (p.SuperiorId = cp.PartnerID) 
		                                inner join  [HouseHold] hh on (p.PartnerID = hh.PartnerID) 
		                                inner join  [Beneficiary] ben ON ben.HouseHoldID = hh.HouseHoldID
		                                AND ben.RegistrationDate between @initialDate AND @lastDate
		                                --Beneficiarios que tenham recebido mais de um serviço entre a data inicial e data final
		                                AND ben.BeneficiaryID IN
		                                (
			                                SELECT rvs.BeneficiaryID
			                                FROM  [Routine_Visit_Summary] rvs
			                                WHERE rvs.RoutineVisitDate BETWEEN @initialDate AND @lastDate AND rvs.BeneficiaryHasServices = 1
		                                )
	                                )ben
                                ) age_obj
                                group by 
	                                age_obj.Partner";

        public static String query6 = @"SELECT
	                                        ben.Partner,
	                                        SUM(ben.Com) As Comunidade,
	                                        SUM(ben.US) As UnidadeSanitaria,
	                                        SUM(ben.Parceiro_Clínico) As ParceiroClinico,
	                                        SUM(ben.Parceiros_Populacoes_Chave) As ParceirosPopulacoesChave,
	                                        SUM(ben.Nenhuma) As Nenhuma,
	                                        SUM(ben.Others) As Outro
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
		                                        ,ben.DateOfBirth
		                                        ,US = CASE WHEN se.Description = 'Unidade Sanitária' THEN 1 ELSE 0 END
		                                        ,Com = CASE WHEN se.Description = 'Comunidade' THEN 1 ELSE 0 END
		                                        ,Parceiro_Clínico = CASE WHEN se.Description = 'Parceiro Clínico' THEN 1 ELSE 0 END
		                                        ,Parceiros_Populacoes_Chave = CASE WHEN se.Description = 'Parceiros de Populacoes-Chave' THEN 1 ELSE 0 END
		                                        ,Nenhuma = CASE WHEN se.Description = 'Nenhuma' THEN 1 ELSE 0 END
		                                        ,Others = CASE WHEN se.Description = 'Outra' THEN 1 ELSE 0 END
	                                        FROM  [Partner] p
	                                        inner join [Partner] cp on (p.SuperiorId = cp.PartnerID) 
	                                        inner join [HouseHold] hh on (p.PartnerID = hh.PartnerID) 
	                                        inner join [SimpleEntity] se on (se.SimpleEntityID = hh.FamilyOriginRefID)
	                                        inner join [Beneficiary] ben ON ben.HouseHoldID = hh.HouseHoldID
	                                        --Beneficiarios que tenham recebido mais de um serviço entre a data inicial e data final
	                                        AND ben.BeneficiaryID IN
	                                        (
		                                        SELECT rvs.BeneficiaryID
		                                        FROM  [Routine_Visit_Summary] rvs
		                                        WHERE rvs.RoutineVisitDate BETWEEN @initialDate AND @lastDate AND rvs.BeneficiaryHasServices = 1
	                                        )
                                        )ben
                                        group by ben.Partner";

        public String query7 = @"SELECT
	                                ben.Partner,
	                                SUM(ben.HIV_P_IN_TARV) As HIV_P_IN_TARV,
	                                SUM(ben.HIV_P_NOT_TARV) As HIV_P_NOT_TARV,
	                                SUM(ben.HIV_N) As HIV_N,
	                                SUM(ben.HIV_KNOWN_NREVEAL) As HIV_KNOWN_NREVEAL,
	                                SUM(ben.HIV_UNKNOWN) As HIV_UNKNOWN,
	                                SUM(ben.HIV_NOT_RECOMMENDED) As HIV_NOT_RECOMMENDED,
	                                SUM(ben.HIVTracked) As HIVTracked
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
		                                HIVTracked = CASE WHEN ben.HIVTracked = '1' AND DATEDIFF(month, CAST(ben.DateOfBirth As Date), @lastDate) > 24 THEN 1 ELSE 0 END,
		                                HIV_N = CASE WHEN hiv.HIV = 'N' AND DATEDIFF(year, CAST(ben.DateOfBirth AS Date), @lastDate) < 18 THEN 1 ELSE 0 END,
		                                HIV_P_IN_TARV = CASE WHEN hiv.HIV = 'P' AND hiv.HIVInTreatment = 0 THEN 1 ELSE 0 END,
		                                HIV_P_NOT_TARV = CASE WHEN hiv.HIV = 'P' AND hiv.HIVInTreatment = 1 THEN 1 ELSE 0 END,
		                                HIV_KNOWN_NREVEAL = CASE WHEN hiv.HIV = 'U' AND hiv.HIVUndisclosedReason = 0 THEN 1 ELSE 0 END,
		                                HIV_NOT_RECOMMENDED = CASE WHEN hiv.HIV = 'U' AND hiv.HIVUndisclosedReason = 2 THEN 1 ELSE 0 END,
		                                HIV_UNKNOWN = CASE WHEN hiv.HIV = 'U' AND hiv.HIVUndisclosedReason = 1 THEN 1 ELSE 0 END
	                                FROM [Partner] p
	                                inner join [Partner] cp on (p.SuperiorId = cp.PartnerID) 
	                                inner join [HouseHold] hh on (p.PartnerID = hh.PartnerID) 
	                                inner join [Beneficiary] ben ON ben.HouseHoldID = hh.HouseHoldID
	                                inner join [HIVStatus] hiv ON hiv.BeneficiaryID = ben.BeneficiaryID 
	                                AND hiv.HIVStatusID IN -- Último estado de HIV no intervalo de datas
	                                (
		                                SELECT
			                                benHIVStatusObj.HIVStatusID
		                                FROM
		                                (
			                                SELECT 
				                                row_number() OVER (PARTITION BY BeneficiaryID ORDER BY InformationDate DESC) AS LastHIVStatusRow
				                                --Obter o número da linha de acordo com BeneficiaryID, e ordenado pela Data de Efectividade de forma DESCENDENTE(Último ao Primeiro)
				                                ,HIVStatusID
				                                ,BeneficiaryID
			                                FROM [HIVStatus] hiv
			                                WHERE hiv.InformationDate BETWEEN @initialDate AND @lastDate
		                                )benHIVStatusObj
		                                WHERE benHIVStatusObj.LastHIVStatusRow = 1 
		                                -- Verifica o último estado de HIV do Intervalo
	                                )
	                                AND ben.RegistrationDate between @initialDate AND @lastDate
                                )ben
                                group by ben.Partner";

        public String query8 = @"SELECT
	                                p.Partner,
	                                ISNULL(SUM(RoutiveVisitDomains.RoutineVisitHIVRiskTracked),0) AS RoutineVisitHIVRiskTracked,
	                                ISNULL(SUM(RoutiveVisitDomains.FinanceAid), 0) AS FinanceAid, 
	                                ISNULL(SUM(RoutiveVisitDomains.Food),0) AS Food,
	                                ISNULL(SUM(RoutiveVisitDomains.Housing),0) AS Housing,
	                                ISNULL(SUM(RoutiveVisitDomains.Education),0) AS Education,
	                                ISNULL(SUM(RoutiveVisitDomains.Health),0) AS Health, 
	                                ISNULL(SUM(RoutiveVisitDomains.SocialAid),0) AS SocialAid,
	                                ISNULL(SUM(RoutiveVisitDomains.LegalAdvice),0) AS LegalAdvice,
	                                ISNULL(SUM(RoutiveVisitDomains.DPI),0) AS DPI,
	                                ISNULL(SUM(MuacDomains.NewMUACGreen),0) AS NewMUACGreen,
	                                ISNULL(SUM(MuacDomains.NewMUACYellow),0) AS NewMUACYellow,
	                                ISNULL(SUM(MuacDomains.NewMUACRed),0) AS NewMUACRed,
	                                ISNULL(SUM(MuacDomains.RptMUACGreen),0) AS RepeatedMUACGreen,
	                                ISNULL(SUM(MuacDomains.RptMUACYellow),0) AS RepeatedMUACYellow,
	                                ISNULL(SUM(MuacDomains.RptMUACRed),0) AS RepeatedMUACRed
                                FROM
                                (
	                                SELECT
		                                Partner
	                                FROM
	                                (
		                                SELECT cp.Name As ChiefPartner, p.Name As Partner
		                                FROM [Partner] p
		                                inner join [Partner] cp on (p.SuperiorId = cp.PartnerID)
		                                Where p.CollaboratorRoleID = 1
	                                ) p
	                                group by p.partner
                                ) p
                                LEFT JOIN
                                (
	                                SELECT
		                                p.[Name] AS Partner,
		                                ISNULL(SUM(merged_Routine_Visit_Summary.FinaceAid),0) As FinanceAid, 
		                                ISNULL(SUM(merged_Routine_Visit_Summary.Health),0) As Health, 
		                                ISNULL(SUM(merged_Routine_Visit_Summary.Food),0) As Food,
		                                ISNULL(SUM(merged_Routine_Visit_Summary.Education),0) As Education,
		                                ISNULL(SUM(merged_Routine_Visit_Summary.LegalAdvice),0) As LegalAdvice,
		                                ISNULL(SUM(merged_Routine_Visit_Summary.Housing),0) As Housing,
		                                ISNULL(SUM(merged_Routine_Visit_Summary.SocialAid),0) As SocialAid,
		                                DPI = CASE WHEN DATEDIFF(YEAR, CAST(ben.DateOfBirth As Date), merged_Routine_Visit_Summary.RoutineVisitDate) < 6 THEN SUM(merged_Routine_Visit_Summary.DPI) ELSE 0 END,
		                                RoutineVisitHIVRiskTracked = CASE WHEN DATEDIFF(month, CAST(ben.DateOfBirth As Date), merged_Routine_Visit_Summary.RoutineVisitDate) > 24 THEN SUM(merged_Routine_Visit_Summary.RoutineVisitHIVRiskTracked) ELSE 0 END
	                                FROM  [Partner] p
	                                inner join [Partner] cp on (p.SuperiorId = cp.PartnerID) 
	                                inner join [HouseHold] hh on (p.PartnerID = hh.PartnerID) 
	                                inner join [Beneficiary] ben ON ben.HouseHoldID = hh.HouseHoldID
	                                inner join [Merged_Routine_Visit_Summary] merged_Routine_Visit_Summary ON merged_Routine_Visit_Summary.BeneficiaryID = ben.BeneficiaryID
	                                AND merged_Routine_Visit_Summary.RoutineVisitDate between @initialDate AND @lastDate
	                                group by p.Name, ben.HIVTracked, ben.DateOfBirth, merged_Routine_Visit_Summary.RoutineVisitDate
                                )RoutiveVisitDomains
                                ON p.Partner = RoutiveVisitDomains.Partner
                                LEFT JOIN
                                (
	                                SELECT --  Total de beneficiarios de acordo com o tipo de Serviço prestado a criança (Por apoio prestado)
		                                routv_obj.Partner As Partner,
		                                SUM(routv_obj.NewMUACGreen) As NewMUACGreen,
		                                SUM(routv_obj.RptMUACGreen) As RptMUACGreen,
		                                SUM(routv_obj.NewMUACYellow) As NewMUACYellow,
		                                SUM(routv_obj.RptMUACYellow) As RptMUACYellow,
		                                SUM(routv_obj.NewMUACRed) As NewMUACRed,
		                                SUM(routv_obj.RptMUACRed) As RptMUACRed
	                                FROM
	                                (
		                                SELECT  -- Eliminar os registos novos dos repetidos
			                                Partner,
			                                NewMUACGreen, 
			                                RptMUACGreen = CASE WHEN NewMUACGreen = 1 AND RptMUACGreen > 0 THEN (RptMUACGreen-1) ELSE RptMUACGreen END,
			                                NewMUACYellow, 
			                                RptMUACYellow = CASE WHEN NewMUACYellow = 1 AND RptMUACYellow > 0 THEN (RptMUACYellow-1) ELSE RptMUACYellow END, 
			                                NewMUACRed, 
			                                RptMUACRed = CASE WHEN NewMUACRed = 1 AND RptMUACRed > 0 THEN (RptMUACRed-1) ELSE RptMUACRed END
		                                FROM
		                                (
			                                SELECT  --  Group By Partner
				                                data.Partner,
				                                NewMUACGreen = CASE WHEN (SUM(data.AllMUACGreen) = 1 AND SUM(data.DtMUACGreen) = 1) OR (SUM(data.AllMUACGreen) > 0 AND SUM(data.AllMUACGreen)-SUM(data.DtMUACGreen) = 0) THEN 1 ELSE 0 END,
				                                RptMUACGreen = CASE WHEN SUM(data.AllMUACGreen) > 1 THEN SUM(data.DtMUACGreen) ELSE 0 END,
				                                NewMUACYellow = CASE WHEN (SUM(data.AllMUACYellow) = 1 AND SUM(data.DtMUACYellow) = 1) OR (SUM(data.AllMUACYellow) > 0 AND SUM(data.AllMUACYellow)-SUM(data.DtMUACYellow) = 0) THEN 1 ELSE 0 END,
				                                RptMUACYellow = CASE WHEN SUM(data.AllMUACYellow) > 1 THEN SUM(data.DtMUACYellow) ELSE 0 END,
				                                NewMUACRed = CASE WHEN SUM(data.AllMUACRed) = 1  AND SUM(data.DtMUACRed) = 1 OR SUM(data.AllMUACRed) > 0 AND SUM(data.AllMUACRed)-SUM(data.DtMUACRed) = 0 THEN 1 ELSE 0 END,
				                                RptMUACRed = CASE WHEN SUM(data.AllMUACRed) > 1 THEN SUM(data.DtMUACRed) ELSE 0 END
			                                FROM
			                                (
				                                SELECT  --  Group By Child
					                                routv_all.Partner As Partner, 
					                                AllMUACGreen = CASE WHEN DATEDIFF(month, CAST(routv_all.DateOfBirth As Date), routv_all.RoutineVisitDate) BETWEEN 6 AND 59 THEN SUM(routv_all.MUACGreen) ELSE 0 END,
					                                AllMUACYellow = CASE WHEN DATEDIFF(month, CAST(routv_all.DateOfBirth As Date), routv_all.RoutineVisitDate) BETWEEN 6 AND 59 THEN SUM(routv_all.MUACYellow) ELSE 0 END,
					                                AllMUACRed = CASE WHEN DATEDIFF(month, CAST(routv_all.DateOfBirth As Date), routv_all.RoutineVisitDate) BETWEEN 6 AND 59 THEN SUM(routv_all.MUACRed) ELSE 0 END,
					                                DtMUACGreen = CASE WHEN DATEDIFF(month, CAST(routv_all.DateOfBirth As Date), routv_all.RoutineVisitDate) BETWEEN 6 AND 59 THEN SUM(routv_all.MUACGreen) ELSE 0 END,
					                                DtMUACYellow = CASE WHEN DATEDIFF(month, CAST(routv_all.DateOfBirth As Date), routv_all.RoutineVisitDate) BETWEEN 6 AND 59 THEN SUM(routv_dt.MUACYellow) ELSE 0 END,
					                                DtMUACRed = CASE WHEN DATEDIFF(month, CAST(routv_all.DateOfBirth As Date), routv_all.RoutineVisitDate) BETWEEN 6 AND 59 THEN SUM(routv_dt.MUACRed) ELSE 0 END
				                                FROM
				                                (
					                                SELECT mrvs.* from [Merged_Routine_Visit_Summary] mrvs
					                                WHERE mrvs.RoutineVisitDate BETWEEN @initialDate AND @lastDate
				                                ) 
				                                routv_all LEFT JOIN
				                                (
					                                SELECT mrvs.* from [Merged_Routine_Visit_Summary] mrvs
					                                WHERE mrvs.RoutineVisitDate BETWEEN @initialDate AND @lastDate
				                                ) routv_dt on (routv_all.RoutineVisitSupportID = routv_dt.RoutineVisitSupportID)
				                                GROUP BY
					                                routv_all.Partner, 
					                                routv_all.DateOfBirth, 
					                                routv_all.RoutineVisitDate
			                                ) data
			                                GROUP BY 
				                                data.Partner
		                                ) data
		                                GROUP BY
			                                data.Partner,
			                                data.NewMUACGreen, 
			                                data.RptMUACGreen,
			                                data.NewMUACYellow, 
			                                data.RptMUACYellow, 
			                                data.NewMUACRed, 
			                                data.RptMUACRed
	                                ) routv_obj 
	                                GROUP BY Partner
                                )MuacDomains
                                ON p.Partner = MuacDomains.Partner
                                GROUP BY p.Partner";

        public String query9 = @"SELECT
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
			                                ,ben.FirstName AS FirstName
			                                ,ben.LastName AS LastName
			                                ,ben.Gender AS Gender
			                                ,ben.DateOfBirth
		                                FROM  [Partner] p
		                                inner join [Partner] cp on (p.SuperiorId = cp.PartnerID) 
		                                inner join [HouseHold] hh on (p.PartnerID = hh.PartnerID) 
		                                inner join [Beneficiary] ben ON ben.HouseHoldID = hh.HouseHoldID and ben.[ServicesStatusForReportDate] between @initialDate and @lastDate
		                                inner join [SimpleEntity] se ON ben.[ServicesStatusForReportID] = se.SimpleEntityID and se.Type='ben-services-status' and se.Description = 'Activo Verde'
		                                inner join [CSI_PROD].[dbo].[ReferenceService] rs on (ben.BeneficiaryID  = rs.BeneficiaryID)
		                                inner join [CSI_PROD].[dbo].[Reference] r on (r.ReferenceServiceID = rs.ReferenceServiceID)
		                                inner join [CSI_PROD].[dbo].[ReferenceType] rt on (rt.ReferenceTypeID = r.ReferenceTypeID)
		                                AND rs.ReferenceDate between @initialDate AND @lastDate
		                                AND ben.BeneficiaryID IS NOT NULL
		                                AND rt.ReferenceName in ('ATS')
	                                )ben
                                ) age_obj
                                group by 
	                                age_obj.Partner";

        public String query10 = @"SELECT
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
			                                ,ben.FirstName AS FirstName
			                                ,ben.LastName AS LastName
			                                ,ben.Gender AS Gender
			                                ,ben.DateOfBirth
		                                FROM  [Partner] p
		                                inner join [Partner] cp on (p.SuperiorId = cp.PartnerID) 
		                                inner join [HouseHold] hh on (p.PartnerID = hh.PartnerID) 
		                                inner join [Beneficiary] ben ON ben.HouseHoldID = hh.HouseHoldID and ben.[ServicesStatusForReportDate] between @initialDate and @lastDate
		                                inner join [SimpleEntity] se ON ben.[ServicesStatusForReportID] = se.SimpleEntityID and se.Type='ben-services-status' and se.Description = 'Activo Verde'
		                                inner join [CSI_PROD].[dbo].[ReferenceService] rs on (ben.BeneficiaryID  = rs.BeneficiaryID)
		                                inner join [CSI_PROD].[dbo].[Reference] r on (r.ReferenceServiceID = rs.ReferenceServiceID)
		                                inner join [CSI_PROD].[dbo].[ReferenceType] rt on (rt.ReferenceTypeID = r.ReferenceTypeID)
		                                AND rs.ReferenceDate between @initialDate AND @lastDate
		                                AND ben.BeneficiaryID IS NOT NULL
		                                AND rt.ReferenceName in ('PTV','Testado HIV+','Pré-TARV/IO','Abandono TARV','PPE')
	                                )ben
                                ) age_obj
                                group by 
	                                age_obj.Partner";

        public String query11 = @"SELECT
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
	                                ISNULL(age_obj.SAAJ_Male,0) As SAAJ_Male,
	                                ISNULL(age_obj.SAAJ_Female,0) As SAAJ_Female,
	                                ISNULL(age_obj.Desnutrition_Male,0) As Desnutrition_Male,
	                                ISNULL(age_obj.Desnutrition_Female,0) As Desnutrition_Female,
	                                ISNULL(age_obj.Development_Delay_Male,0) As Development_Delay_Male,
	                                ISNULL(age_obj.Development_Delay_Female,0) As Development_Delay_Female,
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
		                                SUM(ben.SAAJ_Male) As SAAJ_Male,
		                                SUM(ben.SAAJ_Female) As SAAJ_Female,
		                                SUM(ben.Desnutrition_Male) As Desnutrition_Male,
		                                SUM(ben.Desnutrition_Female) As Desnutrition_Female,
		                                SUM(ben.Development_Delay_Male) As Development_Delay_Male,
		                                SUM(ben.Development_Delay_Female) As Development_Delay_Female,
	                                    SUM(ben.Others_Male) As Others_Male,
	                                    SUM(ben.Others_Female) As Others_Female
	                                FROM
	                                (
		                                SELECT
			                                p.[Name] AS Partner,
			                                ATS_Male = CASE WHEN ben.Gender = 'M' AND rt.ReferenceName = 'ATS' THEN 1 ELSE 0 END,
		                                    ATS_Female = CASE WHEN ben.Gender = 'F' AND rt.ReferenceName = 'ATS' THEN 1 ELSE 0 END,
		                                    TARV_Male = CASE WHEN ben.Gender = 'M' AND rt.ReferenceName in ('PTV','Testado HIV+','Pré-TARV/IO','Abandono TARV','PPE') THEN 1 ELSE 0 END,
		                                    TARV_Female = CASE WHEN ben.Gender = 'F' AND rt.ReferenceName  in ('PTV','Testado HIV+','Pré-TARV/IO','Abandono TARV','PPE') THEN 1 ELSE 0 END,
		                                    CCR_Male = CASE WHEN ben.Gender = 'M' AND rt.ReferenceName = 'CCR' THEN 1 ELSE 0 END,
		                                    CCR_Female = CASE WHEN ben.Gender = 'F' AND rt.ReferenceName = 'CCR' THEN 1 ELSE 0 END,
		                                    SSR_Male = CASE WHEN ben.Gender = 'M' AND rt.ReferenceName in ('Maternidade p/ Parto','CPN','CPN Familiar','Consulta Pós-Parto','ITS','Circuncisao Masculina') THEN 1 ELSE 0 END,
		                                    SSR_Female = CASE WHEN ben.Gender = 'F' AND rt.ReferenceName in ('Maternidade p/ Parto','CPN','CPN Familiar','Consulta Pós-Parto','ITS','Circuncisao Masculina') THEN 1 ELSE 0 END,
		                                    VGB_Male = CASE WHEN ben.Gender = 'M' AND rt.ReferenceName in ('GAVV','Apoio Psico-Social','Posto Policial') THEN 1 ELSE 0 END,
		                                    VGB_Female = CASE WHEN ben.Gender = 'F' AND rt.ReferenceName in ('GAVV','Apoio Psico-Social','Posto Policial') THEN 1 ELSE 0 END,
		                                    Poverty_Proof_Male = CASE WHEN ben.Gender = 'M' AND rt.ReferenceName = 'Atestado de Pobreza' THEN 1 ELSE 0 END,
		                                    Poverty_Proof_Female = CASE WHEN ben.Gender = 'F' AND rt.ReferenceName = 'Atestado de Pobreza' THEN 1 ELSE 0 END,
		                                    Birth_Registration_Male = CASE WHEN ben.Gender = 'M' AND rt.ReferenceName = 'Registo de Nascimento/Cédula' THEN 1 ELSE 0 END,
		                                    Birth_Registration_Female = CASE WHEN ben.Gender = 'F' AND rt.ReferenceName = 'Registo de Nascimento/Cédula' THEN 1 ELSE 0 END,
		                                    Identification_Card_Male = CASE WHEN ben.Gender = 'M' AND rt.ReferenceName = 'Bilhete de Identidade (B.I)' THEN 1 ELSE 0 END,
		                                    Identification_Card_Female = CASE WHEN ben.Gender = 'F' AND rt.ReferenceName = 'Bilhete de Identidade (B.I)' THEN 1 ELSE 0 END,
		                                    School_Integration_Male = CASE WHEN ben.Gender = 'M' AND rt.ReferenceName = 'Integração Escolar' THEN 1 ELSE 0 END,
		                                    School_Integration_Female = CASE WHEN ben.Gender = 'F' AND rt.ReferenceName = 'Integração Escolar' THEN 1 ELSE 0 END,
		                                    Vocational_Courses_Male = CASE WHEN ben.Gender = 'M' AND rt.ReferenceName = 'Curso de Formação Vocacional' THEN 1 ELSE 0 END,
		                                    Vocational_Courses_Female = CASE WHEN ben.Gender = 'F' AND rt.ReferenceName = 'Curso de Formação Vocacional' THEN 1 ELSE 0 END,
		                                    School_Material_Male = CASE WHEN ben.Gender = 'M' AND rt.ReferenceName = 'Material Escolar' THEN 1 ELSE 0 END,
		                                    School_Material_Female = CASE WHEN ben.Gender = 'F' AND rt.ReferenceName = 'Material Escolar' THEN 1 ELSE 0 END,
		                                    Basic_Basket_Male = CASE WHEN ben.Gender = 'M' AND rt.ReferenceName = 'Cesta Básica' THEN 1 ELSE 0 END,
		                                    Basic_Basket_Female = CASE WHEN ben.Gender = 'F' AND rt.ReferenceName = 'Cesta Básica' THEN 1 ELSE 0 END,
		                                    INAS_Benefit_Male = CASE WHEN ben.Gender = 'M' AND rt.ReferenceName = 'Subsídios Sociais do INAS' THEN 1 ELSE 0 END,
		                                    INAS_Benefit_Female = CASE WHEN ben.Gender = 'F' AND rt.ReferenceName = 'Subsídios Sociais do INAS' THEN 1 ELSE 0 END,
			                                SAAJ_Male = CASE WHEN ben.Gender = 'M' AND rt.ReferenceName = 'SAAJ' THEN 1 ELSE 0 END,
		                                    SAAJ_Female = CASE WHEN ben.Gender = 'F' AND rt.ReferenceName = 'SAAJ' THEN 1 ELSE 0 END,
			                                Desnutrition_Male = CASE WHEN ben.Gender = 'M' AND rt.ReferenceName = 'Desnutrição' THEN 1 ELSE 0 END,
		                                    Desnutrition_Female = CASE WHEN ben.Gender = 'F' AND rt.ReferenceName = 'Desnutrição' THEN 1 ELSE 0 END,
			                                Development_Delay_Male = CASE WHEN ben.Gender = 'M' AND rt.ReferenceName = 'Atraso no Desenvolvimento' THEN 1 ELSE 0 END,
		                                    Development_Delay_Female = CASE WHEN ben.Gender = 'F' AND rt.ReferenceName = 'Atraso no Desenvolvimento' THEN 1 ELSE 0 END,
                                            Others_Male = CASE WHEN ben.Gender = 'M' AND rt.ReferenceName not in ('ATS','PTV','Testado HIV+','Pré-TARV/IO','Abandono TARV','PPE','CCR','Maternidade p/ Parto','CPN'
                                            ,'CPN Familiar','Consulta Pós-Parto','ITS','Circuncisao Masculina','GAVV','Apoio Psico-Social','Posto Policial'
                                            ,'Atestado de Pobreza','Registo de Nascimento/Cédula','Bilhete de Identidade (B.I)','Integração Escolar'
                                            ,'Curso de Formação Vocacional','Material Escolar','Cesta Básica','Subsídios Sociais do INAS') THEN 1 ELSE 0 END,
                                            Others_Female = CASE WHEN ben.Gender = 'F' AND rt.ReferenceName not in ('ATS','PTV','Testado HIV+','Pré-TARV/IO','Abandono TARV','PPE','CCR','Maternidade p/ Parto','CPN'
                                            ,'CPN Familiar','Consulta Pós-Parto','ITS','Circuncisao Masculina','GAVV','Apoio Psico-Social','Posto Policial'
                                            ,'Atestado de Pobreza','Registo de Nascimento/Cédula','Bilhete de Identidade (B.I)','Integração Escolar'
                                            ,'Curso de Formação Vocacional','Material Escolar','Cesta Básica','Subsídios Sociais do INAS') THEN 1 ELSE 0 END
		                                FROM  [Partner] p
		                                inner join [Partner] cp on (p.SuperiorId = cp.PartnerID) 
		                                inner join [HouseHold] hh on (p.PartnerID = hh.PartnerID) 
		                                inner join [Beneficiary] ben ON ben.HouseHoldID = hh.HouseHoldID
		                                inner join [ReferenceService] rs on (ben.BeneficiaryID = rs.BeneficiaryID)
		                                inner join [Reference] r on (r.ReferenceServiceID = rs.ReferenceServiceID)
		                                inner join [ReferenceType] rt on (rt.ReferenceTypeID = r.ReferenceTypeID)
		                                AND rs.ReferenceDate between @initialDate AND @lastDate
		                                AND ben.BeneficiaryID IS NOT NULL
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

        public String query12 = @"SELECT
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
	                                ISNULL(age_obj.SAAJ_Male,0) As SAAJ_Male,
	                                ISNULL(age_obj.SAAJ_Female,0) As SAAJ_Female,
	                                ISNULL(age_obj.Desnutrition_Male,0) As Desnutrition_Male,
	                                ISNULL(age_obj.Desnutrition_Female,0) As Desnutrition_Female,
	                                ISNULL(age_obj.Development_Delay_Male,0) As Development_Delay_Male,
	                                ISNULL(age_obj.Development_Delay_Female,0) As Development_Delay_Female,
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
		                                SUM(ben.SAAJ_Male) As SAAJ_Male,
		                                SUM(ben.SAAJ_Female) As SAAJ_Female,
		                                SUM(ben.Desnutrition_Male) As Desnutrition_Male,
		                                SUM(ben.Desnutrition_Female) As Desnutrition_Female,
		                                SUM(ben.Development_Delay_Male) As Development_Delay_Male,
		                                SUM(ben.Development_Delay_Female) As Development_Delay_Female,
	                                    SUM(ben.Others_Male) As Others_Male,
	                                    SUM(ben.Others_Female) As Others_Female
	                                FROM
	                                (
		                                SELECT
			                                p.[Name] AS Partner,
			                                ATS_Male = CASE WHEN ben.Gender = 'M' AND rt.ReferenceName = 'ATS' THEN 1 ELSE 0 END,
		                                    ATS_Female = CASE WHEN ben.Gender = 'F' AND rt.ReferenceName = 'ATS' THEN 1 ELSE 0 END,
		                                    TARV_Male = CASE WHEN ben.Gender = 'M' AND rt.ReferenceName in ('Pré TARV/IO','Testado HIV+','PPE','PTV','TARV','CD') THEN 1 ELSE 0 END,
		                                    TARV_Female = CASE WHEN ben.Gender = 'F' AND rt.ReferenceName  in ('Pré TARV/IO','Testado HIV+','PPE','PTV','TARV','CD') THEN 1 ELSE 0 END,
		                                    CCR_Male = CASE WHEN ben.Gender = 'M' AND rt.ReferenceName = 'CCR' THEN 1 ELSE 0 END,
		                                    CCR_Female = CASE WHEN ben.Gender = 'F' AND rt.ReferenceName = 'CCR' THEN 1 ELSE 0 END,
		                                    SSR_Male = CASE WHEN ben.Gender = 'M' AND rt.ReferenceName in ('Maternidade p/ parto','CPN','CPN Familiar','Consulta Pós-parto','ITS') THEN 1 ELSE 0 END,
		                                    SSR_Female = CASE WHEN ben.Gender = 'F' AND rt.ReferenceName in ('Maternidade p/ parto','CPN','CPN Familiar','Consulta Pós-parto','ITS') THEN 1 ELSE 0 END,
		                                    VGB_Male = CASE WHEN ben.Gender = 'M' AND rt.ReferenceName in ('GAVV') THEN 1 ELSE 0 END,
		                                    VGB_Female = CASE WHEN ben.Gender = 'F' AND rt.ReferenceName in ('GAVV') THEN 1 ELSE 0 END,
		                                    Poverty_Proof_Male = CASE WHEN ben.Gender = 'M' AND rt.ReferenceName = 'Atestado de Pobreza' THEN 1 ELSE 0 END,
		                                    Poverty_Proof_Female = CASE WHEN ben.Gender = 'F' AND rt.ReferenceName = 'Atestado de Pobreza' THEN 1 ELSE 0 END,
		                                    Birth_Registration_Male = CASE WHEN ben.Gender = 'M' AND rt.ReferenceName = 'Registo de Nascimento/Cédula' THEN 1 ELSE 0 END,
		                                    Birth_Registration_Female = CASE WHEN ben.Gender = 'F' AND rt.ReferenceName = 'Registo de Nascimento/Cédula' THEN 1 ELSE 0 END,
		                                    Identification_Card_Male = CASE WHEN ben.Gender = 'M' AND rt.ReferenceName = 'Bilhete de Identidade (B.I)' THEN 1 ELSE 0 END,
		                                    Identification_Card_Female = CASE WHEN ben.Gender = 'F' AND rt.ReferenceName = 'Bilhete de Identidade (B.I)' THEN 1 ELSE 0 END,
		                                    School_Integration_Male = CASE WHEN ben.Gender = 'M' AND rt.ReferenceName = 'Integração Escolar' THEN 1 ELSE 0 END,
		                                    School_Integration_Female = CASE WHEN ben.Gender = 'F' AND rt.ReferenceName = 'Integração Escolar' THEN 1 ELSE 0 END,
		                                    Vocational_Courses_Male = CASE WHEN ben.Gender = 'M' AND rt.ReferenceName = 'Curso de Formação Vocacional' THEN 1 ELSE 0 END,
		                                    Vocational_Courses_Female = CASE WHEN ben.Gender = 'F' AND rt.ReferenceName = 'Curso de Formação Vocacional' THEN 1 ELSE 0 END,
		                                    School_Material_Male = CASE WHEN ben.Gender = 'M' AND rt.ReferenceName = 'Material Escolar' THEN 1 ELSE 0 END,
		                                    School_Material_Female = CASE WHEN ben.Gender = 'F' AND rt.ReferenceName = 'Material Escolar' THEN 1 ELSE 0 END,
		                                    Basic_Basket_Male = CASE WHEN ben.Gender = 'M' AND rt.ReferenceName = 'Cesta Básica' THEN 1 ELSE 0 END,
		                                    Basic_Basket_Female = CASE WHEN ben.Gender = 'F' AND rt.ReferenceName = 'Cesta Básica' THEN 1 ELSE 0 END,
		                                    INAS_Benefit_Male = CASE WHEN ben.Gender = 'M' AND rt.ReferenceName = 'Subsídios Sociais do INAS' THEN 1 ELSE 0 END,
		                                    INAS_Benefit_Female = CASE WHEN ben.Gender = 'F' AND rt.ReferenceName = 'Subsídios Sociais do INAS' THEN 1 ELSE 0 END,
			                                SAAJ_Male = CASE WHEN ben.Gender = 'M' AND rt.ReferenceName = 'SAAJ' THEN 1 ELSE 0 END,
		                                    SAAJ_Female = CASE WHEN ben.Gender = 'F' AND rt.ReferenceName = 'SAAJ' THEN 1 ELSE 0 END,
			                                Desnutrition_Male = CASE WHEN ben.Gender = 'M' AND rt.ReferenceName = 'Desnutrição' THEN 1 ELSE 0 END,
		                                    Desnutrition_Female = CASE WHEN ben.Gender = 'F' AND rt.ReferenceName = 'Desnutrição' THEN 1 ELSE 0 END,
			                                Development_Delay_Male = CASE WHEN ben.Gender = 'M' AND rt.ReferenceName = 'Atraso no Desenvolvimento' THEN 1 ELSE 0 END,
		                                    Development_Delay_Female = CASE WHEN ben.Gender = 'F' AND rt.ReferenceName = 'Atraso no Desenvolvimento' THEN 1 ELSE 0 END,
		                                    Others_Male = CASE WHEN ben.Gender = 'M' AND rt.ReferenceName not in ('ATS','Pré TARV/IO','Testado HIV+','PPE','PTV','TARV','CD','CCR','Maternidade p/ parto'
	                                        ,'CPN','CPN Familiar','Consulta Pós-parto','ITS','GAVV','Atestado de Pobreza','Registo de Nascimento/Cédula','Bilhete de Identidade (B.I)'
	                                        ,'Integração Escolar','Curso de Formação Vocacional','Material Escolar','Cesta Básica','Subsídios Sociais do INAS') THEN 1 ELSE 0 END,
		                                    Others_Female = CASE WHEN ben.Gender = 'F' AND rt.ReferenceName not in ('ATS','Pré TARV/IO','Testado HIV+','PPE','PTV','TARV','CD','CCR','Maternidade p/ parto'
	                                        ,'CPN','CPN Familiar','Consulta Pós-parto','ITS','GAVV','Atestado de Pobreza','Registo de Nascimento/Cédula','Bilhete de Identidade (B.I)'
	                                        ,'Integração Escolar','Curso de Formação Vocacional','Material Escolar','Cesta Básica','Subsídios Sociais do INAS') THEN 1 ELSE 0 END
		                                FROM  [Partner] p
		                                inner join [Partner] cp on (p.SuperiorId = cp.PartnerID) 
		                                inner join [HouseHold] hh on (p.PartnerID = hh.PartnerID) 
		                                inner join [Beneficiary] ben ON ben.HouseHoldID = hh.HouseHoldID
		                                inner join [CSI_PROD].[dbo].[ReferenceService] rs on (ben.BeneficiaryID = rs.BeneficiaryID)
		                                inner join [CSI_PROD].[dbo].[Reference] r on (r.ReferenceServiceID = rs.ReferenceServiceID)
		                                inner join [CSI_PROD].[dbo].[ReferenceType] rt on (rt.ReferenceTypeID = r.ReferenceTypeID)
		                                AND ben.RegistrationDate between @initialDate AND @lastDate
		                                AND ben.BeneficiaryID IS NOT NULL
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

        public String query13 = @"SELECT
	                                p.Partner,
	                                ISNULL(SUM(LeftWithouGraduating.DeathChild),0) As DeathChild, 
	                                ISNULL(SUM(LeftWithouGraduating.DeathAdult),0) As DeathAdult, 
	                                ISNULL(SUM(LeftWithouGraduating.LostChild),0) As LostChild, 
	                                ISNULL(SUM(LeftWithouGraduating.LostAdult),0) As LostAdult,
	                                ISNULL(SUM(LeftWithouGraduating.GaveUpChild),0) As GaveUpChild, 
	                                ISNULL(SUM(LeftWithouGraduating.GaveUpAdult),0) As GaveUpAdult, 
	                                ISNULL(SUM(LeftWithouGraduating.OthersChild),0) As OthersChild, 
	                                ISNULL(SUM(LeftWithouGraduating.OthersAdult),0) As OthersAdult,
	                                ISNULL(SUM(LeftWithouGraduating.TransfersChild),0) As TransfersChild, 
	                                ISNULL(SUM(LeftWithouGraduating.TransfersAdult),0) As TransfersAdult,
	                                ISNULL(SUM(InactiveBeneficiaries.InactiveChild),0) AS InactiveChild,
	                                ISNULL(SUM(InactiveBeneficiaries.InactiveAdult),0) AS InactiveAdult
                                FROM
                                (
	                                SELECT
		                                Partner
	                                FROM
	                                (
		                                SELECT cp.Name As ChiefPartner, p.Name As Partner
		                                FROM [Partner] p
		                                inner join [Partner] cp on (p.SuperiorId = cp.PartnerID)
		                                Where p.CollaboratorRoleID = 1
	                                ) p
	                                group by p.partner
                                ) p
                                LEFT JOIN
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
		                                OthersChild = CASE WHEN cs.Description = 'Outras Saídas' AND DATEDIFF(year,CAST(Ben.DateOfBirth AS Date), @lastDate) < 18 THEN 1 ELSE 0 END,
		                                OthersAdult = CASE WHEN cs.Description = 'Outras Saídas' AND DATEDIFF(year,CAST(Ben.DateOfBirth AS Date), @lastDate) >= 18 THEN 1 ELSE 0 END,
		                                TransfersChild = CASE WHEN cs.Description in ('Transferido p/ programas NÃO de PEPFAR)', 'Transferido p/ programas de PEPFAR') AND DATEDIFF(year,CAST(Ben.DateOfBirth AS Date), @lastDate) < 18 THEN 1 ELSE 0 END,
		                                TransfersAdult = CASE WHEN cs.Description in ('Transferido p/ programas NÃO de PEPFAR)', 'Transferido p/ programas de PEPFAR') AND DATEDIFF(year,CAST(Ben.DateOfBirth AS Date), @lastDate) >= 18 THEN 1 ELSE 0 END
	                                FROM  [Partner] p
	                                inner join [Partner] cp on (p.SuperiorId = cp.PartnerID) 
	                                inner join [HouseHold] hh on (p.PartnerID = hh.PartnerID)
	                                inner join [Beneficiary] ben ON ben.HouseholdID = hh.HouseHoldID
	                                inner join [ChildStatusHistory] csh ON csh.BeneficiaryID = ben.BeneficiaryID 
	                                AND csh.ChildStatusHistoryID IN -- Último estado no intervalo de datas
	                                (
		                                SELECT
			                                benStatusObj.ChildStatusHistoryID
		                                FROM
		                                (
			                                SELECT 
				                                row_number() OVER (PARTITION BY BeneficiaryID ORDER BY EffectiveDate DESC) AS LastStatusRow
				                                --Obter o número da linha de acordo com BeneficiaryID, e ordenado pela Data de Efectividade de forma DESCENDENTE(Último ao Primeiro)
				                                ,ChildStatusHistoryID
				                                ,BeneficiaryID
			                                FROM [ChildStatusHistory] csh
			                                WHERE csh.EffectiveDate	BETWEEN @initialDate AND @lastDate
		                                )benStatusObj
		                                WHERE benStatusObj.LastStatusRow = 1 
		                                -- Verifica o último estado do Intervalo
	                                )
	                                inner join [ChildStatus] cs ON cs.StatusID = csh.ChildStatusID
	                                WHERE p.CollaboratorRoleID = 1
	                                group by cp.Name, p.Name, ben.BeneficiaryID, ben.FirstName, ben.LastName, cs.Description, Ben.DateOfBirth
                                )LeftWithouGraduating
                                ON p.Partner = LeftWithouGraduating.Partner
                                LEFT JOIN
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
			                                InactiveChild = CASE WHEN DATEDIFF(year,CAST(Ben.DateOfBirth AS Date), @lastDate) < 18 THEN 1 ELSE 0 END,
			                                InactiveAdult = CASE WHEN DATEDIFF(year,CAST(Ben.DateOfBirth AS Date), @lastDate) >= 18 THEN 1 ELSE 0 END
		                                FROM  [Partner] p
		                                inner join [Partner] cp on (p.SuperiorId = cp.PartnerID) 
		                                inner join [HouseHold] hh on (p.PartnerID = hh.PartnerID) 
		                                inner join [Beneficiary] ben ON ben.HouseHoldID = hh.HouseHoldID and ben.[ServicesStatusForReportDate] between @initialDate and @lastDate
		                                inner join [SimpleEntity] se ON ben.[ServicesStatusForReportID] = se.SimpleEntityID and se.Type='ben-services-status' and se.Description = 'Inactivo'
                                )InactiveBeneficiaries
                                ON p.Partner = InactiveBeneficiaries.Partner
                                group by p.Partner";
        
        public List<AgeGroupDTO> getReport_One(DateTime initialDate, DateTime lastDate)
        {
            return UnitOfWork.DbContext.Database.SqlQuery<AgeGroupDTO>(query1,
                                                            new SqlParameter("initialDate", initialDate),
                                                            new SqlParameter("lastDate", lastDate)).ToList();
        }
        
        public List<HIVGroupDTO> getReport_Two(DateTime initialDate, DateTime lastDate)
        {
            return UnitOfWork.DbContext.Database.SqlQuery<HIVGroupDTO>(query2,
                                                            new SqlParameter("initialDate", initialDate),
                                                            new SqlParameter("lastDate", lastDate)).ToList();
        }
        
        
        public List<AgeGroupDTO> getReport_Three(DateTime initialDate, DateTime lastDate)
        {
            return UnitOfWork.DbContext.Database.SqlQuery<AgeGroupDTO>(query3,
                                                            new SqlParameter("initialDate", initialDate),
                                                            new SqlParameter("lastDate", lastDate)).ToList();
        }

        
        public List<HIVGroupDTO> getReport_Four(DateTime initialDate, DateTime lastDate)
        {
            return UnitOfWork.DbContext.Database.SqlQuery<HIVGroupDTO>(query4,
                                                            new SqlParameter("initialDate", initialDate),
                                                            new SqlParameter("lastDate", lastDate)).ToList();
        }

        
        public List<AgeGroupDTO> getReport_Five(DateTime initialDate, DateTime lastDate)
        {
            return UnitOfWork.DbContext.Database.SqlQuery<AgeGroupDTO>(query5,
                                                            new SqlParameter("initialDate", initialDate),
                                                            new SqlParameter("lastDate", lastDate)).ToList();
        }

        
        public List<ReferenceOriginDTO> getReport_Six(DateTime initialDate, DateTime lastDate)
        {
            return UnitOfWork.DbContext.Database.SqlQuery<ReferenceOriginDTO>(query6,
                                                            new SqlParameter("initialDate", initialDate),
                                                            new SqlParameter("lastDate", lastDate)).ToList();
        }

												
        public List<HIVGroupDTO> getReport_Seven(DateTime initialDate, DateTime lastDate)
        {
            return UnitOfWork.DbContext.Database.SqlQuery<HIVGroupDTO>(query7,
                                                            new SqlParameter("initialDate", initialDate),
                                                            new SqlParameter("lastDate", lastDate)).ToList();
        }

												
        public List<RoutineVisitGroupV2DTO> getReport_Eight(DateTime initialDate, DateTime lastDate)
        {
            return UnitOfWork.DbContext.Database.SqlQuery<RoutineVisitGroupV2DTO>(query8,
                                                            new SqlParameter("initialDate", initialDate),
                                                            new SqlParameter("lastDate", lastDate)).ToList();
        }

												
        public List<AgeGroupDTO> getReport_Nine(DateTime initialDate, DateTime lastDate)
        {
            return UnitOfWork.DbContext.Database.SqlQuery<AgeGroupDTO>(query9,
                                                            new SqlParameter("initialDate", initialDate),
                                                            new SqlParameter("lastDate", lastDate)).ToList();
        }

				
        public List<AgeGroupDTO> getReport_Ten(DateTime initialDate, DateTime lastDate)
        {
            return UnitOfWork.DbContext.Database.SqlQuery<AgeGroupDTO>(query10,
                                                            new SqlParameter("initialDate", initialDate),
                                                            new SqlParameter("lastDate", lastDate)).ToList();
        }

        
        public List<ReferenceGroupV2DTO> getReport_Eleven(DateTime initialDate, DateTime lastDate)
        {
            return UnitOfWork.DbContext.Database.SqlQuery<ReferenceGroupV2DTO>(query11,
                                                            new SqlParameter("initialDate", initialDate),
                                                            new SqlParameter("lastDate", lastDate)).ToList();
        }


        public List<ReferenceGroupV2DTO> getReport_Twelve(DateTime initialDate, DateTime lastDate)
        {
            return UnitOfWork.DbContext.Database.SqlQuery<ReferenceGroupV2DTO>(query12,
                                                            new SqlParameter("initialDate", initialDate),
                                                            new SqlParameter("lastDate", lastDate)).ToList();
        }

        public List<NonGraduatedBeneficiaryV2DTO> getReport_Thirteen(DateTime initialDate, DateTime lastDate)
        {
            return UnitOfWork.DbContext.Database.SqlQuery<NonGraduatedBeneficiaryV2DTO>(query13,
                                                            new SqlParameter("initialDate", initialDate),
                                                            new SqlParameter("lastDate", lastDate)).ToList();
        }

        public Dictionary<string, List<string>> FindReportDataByCodeAndInitialDateAndLastDate(string QueryCode, DateTime initialDate, DateTime lastDate)
        {
            // Group and Sum this informtation
            List<ReportData> reportData = UnitOfWork.Repository<ReportData>().GetAll().Where(x => x.QueryCode.Equals(QueryCode) && 
                initialDate <= x.InitialPositionDate && lastDate >= x.FinalPositionDate).ToList();

            List<string> outList;
            Dictionary<string, List<string>> groupedData = new Dictionary<string, List<string>>();

            foreach (var DataRow in reportData)
            {
                if (groupedData.TryGetValue(DataRow.Field1, out outList))
                { groupedData[DataRow.Field1] = groupedData[DataRow.Field1].ToList().Concat(ConvertReportDataToStringList(DataRow, 2)).ToList(); }
                else
                { groupedData.Add(DataRow.Field1, ConvertReportDataToStringList(DataRow, 1)); }
            }

            return groupedData;
        }

        public List<string> ConvertReportDataToStringList(ReportData DataRow, int startFrom)
        {
            List<string> DataRowList = new List<string>();

            for (int i = startFrom; true; i++)
            {
                PropertyInfo prop = DataRow.GetType().GetProperty("Field" + i, BindingFlags.Public | BindingFlags.Instance);
                if (null == prop) { break; }
                else {
                    var value = prop.GetValue(DataRow, null);
                    DataRowList.Add(value == null ? string.Empty : (string) prop.GetValue(DataRow, null));
                }
            }

            return DataRowList;
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
            // ClearMEReportTableBeforeImport(dt2);

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
                    ReportData.Field31 = row["Field31"].ToString().Equals("") ? "0" : row["Field31"].ToString();
                    ReportData.Field32 = row["Field32"].ToString().Equals("") ? "0" : row["Field32"].ToString();
                    ReportData.Field33 = row["Field33"].ToString().Equals("") ? "0" : row["Field33"].ToString();
                    ReportData.Field34 = row["Field34"].ToString().Equals("") ? "0" : row["Field34"].ToString();
                    ReportData.Field35 = row["Field35"].ToString().Equals("") ? "0" : row["Field35"].ToString();
                    ReportData.InitialPositionDate = (row["InitialPositionDate"].ToString()).Length == 0 ? (DateTime?)null : DateTime.Parse(row["InitialPositionDate"].ToString());
                    ReportData.FinalPositionDate = (row["FinalPositionDate"].ToString()).Length == 0 ? (DateTime?)null : DateTime.Parse(row["FinalPositionDate"].ToString());
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
        }

        private void ClearMEReportTableBeforeImport(DataTable dt2)
        {
            DataRow FileFirstRow = dt2.Rows[0];
            string SiteName = FileFirstRow["SiteName"].ToString();
            DateTime initialDate = DateTime.Parse(FileFirstRow["InitialPositionDate"].ToString());
            DateTime lastDate = DateTime.Parse(FileFirstRow["FinalPositionDate"].ToString());
            List<string> QueryCodes = new List<string>
            { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13"};
            QueryCodes.ForEach(queryCode =>
            {
                List<ReportData> existingRecords = UnitOfWork.DbContext.ReportData.Where(data =>
                data.InitialPositionDate == initialDate &&
                data.FinalPositionDate == lastDate &&
                data.QueryCode == queryCode && data.SiteName == SiteName)
                .ToList();

                if (existingRecords.Count() > 0)
                {
                    existingRecords.ForEach(record => UnitOfWork.DbContext.ReportData.Remove(record));
                    UnitOfWork.DbContext.SaveChanges();
                }
            });
        }
    }
}
