using EFDataAccess.UOW;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using EFDataAccess.DTO.AgreggatedDTO;
using EFDataAccess.DTO;

namespace EFDataAccess.Services
{
    public class AgreggatedReportService
    {
        public AgreggatedReportService()
        { }

        public AgreggatedReportService(UnitOfWork uow)
        {
            UnitOfWork = uow;
        }

        public UnitOfWork UnitOfWork { set; private get; }


        public List<AgreggatedGlobalReportDTO> getAgreggatedGlobalReport(DateTime initialDate, DateTime lastDate)
        {
            String query = @"SELECT
		                        dados.SiteName
		                        ,dados.personType
		                        ,dados.FirstName
		                        ,dados.LastName --NEW APELIDO
		                        ,dados.HouseholdName
		                        ,dados.ChiefPartner
		                        ,dados.Partner
		                        ,dados.Age
		                        ,dados.DateOFBirth
		                        ,dados.DateOfBirthUnknown
		                        ,dados.Gender
		                        ,dados.District
		                        ,dados.AdministrativePost
		                        ,dados.NeighborhoodName

		                        ,dados.Block --NEW QUARTEIRÃO
		                        ,dados.ClosePlaceToHome --NEW LUGAR PROXIMO DE CASA

		                        ,dados.PrincipalChiefName AS PrincipalChiefName

		                        ,dados.FamilyHeadDescription --NEW RESPONSAVEL PELA FAMILIA

		                        ,dados.RegistrationDate
		                        ,dados.InstitutionalAid
		                        ,dados.InstitutionalAidDetail --NEW DETALHES APOIO INSTITUCIONAL
		                        ,dados.communityAid
		                        ,dados.communityAidDetail --NEW DETALHES APOIO COMUNIDADE
		                        ,dados.individualAid --NEW APOIO INDIVIDUAL
		                        ,dados.FamilyPhoneNumber
		                        ,dados.AnyoneBedridden
		                        ,dados.FamilyOriginReference
		                        ,dados.OtherFamilyOriginRef --NEW NOME DE ORIGEM DA FAMILIA
		                        ,dados.ovcDescription
		                        ,dados.degreeOfKingshipDescription
		                        ,dados.IsPartSavingGroup
		                        ,dados.HIVStatus
		                        ,dados.HIVStatusDetails --NEW DETALHES HIV
		                        ,dados.NID

		                        ,dados.P1
		                        ,dados.P2
		                        ,dados.P3
		                        ,dados.P4
		                        ,dados.P5
		                        ,dados.P6
		                        ,dados.P7
		                        ,dados.P8
		                        ,dados.P9
		                        ,dados.P10
		                        ,dados.P11
		                        ,dados.P12
		                        ,dados.P13
		                        ,dados.P14
		                        ,dados.P15
		                        ,dados.P16
		                        ,dados.P17
		                        ,dados.P18
		                        ,dados.P19
		                        ,dados.P20
		                        ,dados.P21
		                        ,dados.P22
		                        ,dados.P23
		                        ,dados.P24
		                        ,dados.P25
		                        ,dados.P26
		                        ,dados.P27
		                        ,dados.P28
		                        ,dados.P29
		                        ,dados.P30
		                        ,dados.P31
		                        ,dados.P32
		                        ,dados.P33
		                        ,dados.A1
		                        ,dados.A2
		                        ,dados.A3
		                        ,dados.A4
		                        ,dados.A5
		                        ,dados.A6
		                        ,dados.A7
		                        ,dados.A8
		                        ,dados.A9
		                        ,dados.A10
		                        ,dados.A11
		                        ,dados.A12
		                        ,dados.A13
		                        ,dados.A14
		                        ,dados.A15
		                        ,dados.A16
		                        ,dados.A17
		                        ,dados.A18
		                        ,dados.A19
		                        ,dados.A20
		                        ,dados.A21
		                        ,dados.A22
		                        ,dados.A23
		                        ,dados.A24
		                        ,dados.A25
		                        ,dados.A26
		                        ,dados.A27
		                        ,dados.A28
		                        ,dados.A29
		                        ,dados.A30
		                        ,dados.A31
		                        ,dados.A32
		                        ,dados.A33
           
		                        ,dados.ChildStatusDescription
		                        ,dados.FirstTimeSavingGroup
		                        ,dados.FE
		                        ,dados.AN
		                        ,dados.HAB
		                        ,dados.ED
		                        ,dados.SD
		                        ,dados.APS
		                        ,dados.PL
		                        ,dados.DPI
		                        ,dados.MUACGREEN
		                        ,dados.MUACYELLOW
		                        ,dados.MUACRED
		                        ,dados.RoutineVisitDate

		                        ,dados.ATS
		                        ,dados.TARV
		                        ,dados.CCR
		                        ,dados.SSR
		                        ,dados.VGB
		                        ,dados.Others


		                        ,dados.ReferenceDate

		                        ,dados.RC_ATS
		                        ,dados.RC_TARV
		                        ,dados.RC_CCR
		                        ,dados.RC_SSR
		                        ,dados.RC_VGB

		                        ,dados.HealthAttendedDate

		                        ,dados.SocialAttendedDate

                        FROM
                        (
		                        SELECT
				                        agregado_obj.SiteName
				                        ,'CRIANÇA' as personType
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
				                        ,ISNULL(CONVERT(varchar,fichaseguimento_obj.RoutineVisitDate,103),'') AS RoutineVisitDate

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
								                        --'CRIANÇA' as personType
								                        --,(c.FirstName) + ' ' + (c.LastName) As FullName
								                        siteocb.SiteName
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
								                         [Site] AS siteocb ON siteocb.SiteID = cp.siteID
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
		                        ----------------------------------------------------------------------------------------------------------------------------------------
					                        LEFT JOIN 
							                         [RoutineVisit] rv on (hh.HouseHoldID = rv.HouseholdID)
						                        LEFT JOIN 
							                         [RoutineVisitMember] rvm on (rvm.RoutineVisitID = rv.RoutineVisitID)
					                        AND (c.ChildID = rvm.ChildID)
						                        --LEFT JOIN 
						                           --  [ReferenceService] rs on (rs.RoutineVisitMemberID = rvm.RoutineVisitMemberID)
						                        LEFT JOIN 
							                         [RoutineVisitSupport] rvs on (rvs.RoutineVisitMemberID = rvm.RoutineVisitMemberID)
						                        LEFT JOIN
							                         [Domain] dom on dom.[DomainID] = rvs.SupportID
					                        WHERE
							                        cp.CollaboratorRoleID = 2 AND c.HouseholdID IS NOT NULL
							                        --AND rv.RoutineVisitDate >= @initialDate AND rv.RoutineVisitDate <= @lastDate
						                            --AND hh.RegistrationDate >= @initialDate AND hh.RegistrationDate <= @lastDate
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
							                        ,siteocb.SiteName
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
					                        --AND csi.IndexDate >= @initialDate AND csi.IndexDate <= @lastDate
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
						                        -- AND cplan.CarePlanDate >= @initialDate AND cplan.CarePlanDate <= @lastDate
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
							                        ,rv.RoutineVisitDate
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
									                         Child c on (c.ChildID = rvm.ChildID) AND (hh.HouseHoldID=c.HouseholdID)
								                        --LEFT JOIN 
									                        --  [ReferenceService] rs on (rs.RoutineVisitMemberID = rvm.RoutineVisitMemberID)
								                        LEFT JOIN 
									                         [RoutineVisitSupport] rvs on (rvs.RoutineVisitMemberID = rvm.RoutineVisitMemberID)
								                        LEFT JOIN
									                         [Domain] dom on dom.[DomainID] = rvs.SupportID
								                        --LEFT JOIN 
									                        -- [Reference] r on (r.ReferenceServiceID = rs.ReferenceServiceID)
								                        --LEFT JOIN 
									                        -- [ReferenceType] rt on (rt.ReferenceTypeID = r.ReferenceTypeID)
							                        WHERE 
								                        cp.CollaboratorRoleID = 2  AND c.HouseholdID IS NOT NULL
								                        --AND rt.FieldType = 'CheckBox' AND rvm.ChildID IS NOT NULL
								                        --AND rv.RoutineVisitDate >= @initialDate AND rv.RoutineVisitDate <= @lastDate
								                        --AND rs.ReferenceDate >= @initialDate AND rs.ReferenceDate  <= @lastDate
								                        --AND rs.HealthAttendedDate >= @initialDate AND rs.HealthAttendedDate  <= @lastDate
							                        group by
								                        c.FirstName, c.LastName, rv.RoutineVisitDate	
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
						                        ,ISNULL(MAX(CASE WHEN rt.ReferenceName = 'ATS' AND rt.ReferenceCategory = 'Activist' AND r.Value <>'0'  AND r.Value <> '' THEN 'SIM' ELSE 'NÃO' END), '')  As ATS
						                        ,ISNULL(MAX(CASE WHEN rt.ReferenceName in ('PTV','Testado HIV+','Pré-TARV/IO','Abandono TARV','PPE') AND rt.ReferenceCategory = 'Activist' AND r.Value <>'0'  AND r.Value <> '' THEN 'SIM' ELSE 'NÃO' END), '')  As TARV
						                        ,ISNULL(MAX(CASE WHEN rt.ReferenceName = 'CCR' AND rt.ReferenceCategory = 'Activist' AND r.Value <>'0'  AND r.Value <> '' THEN 'SIM' ELSE 'NÃO' END), '')  As CCR
						                        ,ISNULL(MAX(CASE WHEN rt.ReferenceName in ('Maternidade p/ Parto','CPN','CPN Familiar','Consulta Pós-Parto','ITS','Circuncisao Masculina') AND rt.ReferenceCategory = 'Activist' AND r.Value <>'0'  AND r.Value <> '' THEN 'SIM' ELSE 'NÃO' END), '')  As SSR
						                        ,ISNULL(MAX(CASE WHEN rt.ReferenceName in ('GAVV','Apoio Psico-Social','Posto Policial') AND rt.ReferenceCategory = 'Activist' AND r.Value <>'0'  AND r.Value <> '' THEN 'SIM' ELSE 'NÃO' END), '') As VGB
						                        ,ISNULL(MAX(CASE WHEN rt.ReferenceName not in ('ATS','PTV','Testado HIV+','Pré-TARV/IO'
															                        ,'Abandono TARV','PPE','CCR','Maternidade p/ Parto','CPN'
															                        ,'CPN Familiar','Consulta Pós-Parto','ITS','Circuncisao Masculina'
															                        ,'GAVV','Apoio Psico-Social','Posto Policial')  
						                        AND rt.ReferenceCategory = 'Activist' AND r.Value <>'0'  AND r.Value <> '' THEN 'SIM' ELSE 'NÃO' END), '') As Others
						                        ,rs.ReferenceDate
						                        --,(CASE WHEN rs.ReferenceDate >= '2016/09/01' THEN rs.ReferenceDate ELSE NULL END) AS ReferenceDate

						                        ,ISNULL(MAX(CASE WHEN rt.ReferenceName = 'ATS' AND rt.ReferenceCategory in ('Health','Social') AND r.Value <>'0'  AND r.Value <> '' THEN 'SIM' ELSE 'NÃO' END), '') As RC_ATS
						                        ,ISNULL(MAX(CASE WHEN rt.ReferenceName in ('Pré TARV/IO','Testado HIV+','PPE','PTV','TARV','CD')  AND rt.ReferenceCategory in ('Health','Social') AND r.Value <>'0'  AND r.Value <> '' THEN 'SIM' ELSE 'NÃO' END), '') As RC_TARV
						                        ,ISNULL(MAX(CASE WHEN rt.ReferenceName = 'CCR' AND rt.ReferenceCategory in ('Health','Social') AND r.Value <>'0'  AND r.Value <> '' THEN 'SIM' ELSE 'NÃO' END), '') As RC_CCR
						                        ,ISNULL(MAX(CASE WHEN rt.ReferenceName in ('Maternidade p/ parto','CPN','CPN Familiar','Consulta Pós-parto','ITS') AND rt.ReferenceCategory in ('Health','Social') AND r.Value <>'0'  AND r.Value <> ''  THEN 'SIM' ELSE 'NÃO' END), '') As RC_SSR
						                        ,ISNULL(MAX(CASE WHEN rt.ReferenceName = 'GAVV' AND rt.ReferenceCategory in ('Health','Social')  AND r.Value <>'0'  AND r.Value <> '' THEN 'SIM' ELSE 'NÃO' END), '') As RC_VGB
						                        ,ISNULL(MAX(CASE WHEN rt.ReferenceName not in ('ATS','Pré TARV/IO','Testado HIV+','PPE','PTV','TARV','CD'
						                        ,'CCR','Maternidade p/ parto','CPN','CPN Familiar','Consulta Pós-parto','ITS','GAVV') AND rt.ReferenceCategory in ('Health','Social')  AND r.Value <>'0'  AND r.Value <> '' THEN 'SIM' ELSE 'NÃO' END), '') As RC_Others
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
								                         Child c on (c.ChildID = rvm.ChildID) AND (hh.HouseHoldID=c.HouseholdID)
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
							                        --AND rt.FieldType = 'CheckBox' --AND rvm.ChildID IS NOT NULL
							                        --AND rv.RoutineVisitDate >= @initialDate AND rv.RoutineVisitDate <= @lastDate
							                        --AND rs.ReferenceDate >= @initialDate AND rs.ReferenceDate  <= @lastDate
							                        --AND rs.HealthAttendedDate >= @initialDate AND rs.HealthAttendedDate  <= @lastDate
							                        AND rvm.childID IS NOT NULL
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
						                        group by
							                        c.FirstName, c.LastName, rs.ReferenceDate, rs.HealthAttendedDate, rs.SocialAttendedDate
				                        )ref_obj
				                        ON
				                        (
					                        fichaseguimento_obj.FirstName = ref_obj.FirstName
					                        AND
					                        fichaseguimento_obj.LastName = ref_obj.LastName
				                        )
				                        --ORDER BY
					                        --agregado_obj.personType DESC

		                        UNION ALL

			                        SELECT
				                        agregado_obj.SiteName
				                        ,'ADULTO' as personType
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

				                        ,'-1' AS P1
				                        ,'-1' AS P2
				                        ,'-1' AS P3
				                        ,'-1' AS P4
				                        ,'-1' AS P5
				                        ,'-1' AS P6
				                        ,'-1' AS P7
				                        ,'-1' AS P8
				                        ,'-1' AS P9
				                        ,'-1' AS P10
				                        ,'-1' AS P11
				                        ,'-1' AS P12
				                        ,'-1' AS P13
				                        ,'-1' AS P14
				                        ,'-1' AS P15
				                        ,'-1' AS P16
				                        ,'-1' AS P17
				                        ,'-1' AS P18
				                        ,'-1' AS P19
				                        ,'-1' AS P20
				                        ,'-1' AS P21
				                        ,'-1' AS P22
				                        ,'-1' AS P23
				                        ,'-1' AS P24
				                        ,'-1' AS P25
				                        ,'-1' AS P26
				                        ,'-1' AS P27
				                        ,'-1' AS P28
				                        ,'-1' AS P29
				                        ,'-1' AS P30
				                        ,'-1' AS P31
				                        ,'-1' AS P32
				                        ,'-1' AS P33


				                        ,'' AS A1
				                        ,'' AS A2
				                        ,'' AS A3
				                        ,'' AS A4
				                        ,'' AS A5
				                        ,'' AS A6
				                        ,'' AS A7
				                        ,'' AS A8
				                        ,'' AS A9
				                        ,'' AS A10
				                        ,'' AS A11
				                        ,'' AS A12
				                        ,'' AS A13
				                        ,'' AS A14
				                        ,'' AS A15
				                        ,'' AS A16
				                        ,'' AS A17
				                        ,'' AS A18
				                        ,'' AS A19
				                        ,'' AS A20
				                        ,'' AS A21
				                        ,'' AS A22
				                        ,'' AS A23
				                        ,'' AS A24
				                        ,'' AS A25
				                        ,'' AS A26
				                        ,'' AS A27
				                        ,'' AS A28
				                        ,'' AS A29
				                        ,'' AS A30
				                        ,'' AS A31
				                        ,'' AS A32
				                        ,'' AS A33

           
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
				                        ,ISNULL(CONVERT(varchar,fichaseguimento_obj.RoutineVisitDate,103),'') AS RoutineVisitDate

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
			                                                            --'ADULTO' as personType
			                                                            --,(c.FirstName) + ' ' + (c.LastName) As FullName
												                        siteocb.SiteName
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
												                         [Site] AS siteocb ON siteocb.SiteID = cp.siteID
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
                        ----------------------------------------------------------------------------------------------------------------------------------------
										                        LEFT JOIN 
		                                                             [RoutineVisit] rv on (hh.HouseHoldID = rv.HouseholdID)
	                                                            LEFT JOIN 
		                                                             [RoutineVisitMember] rvm on (rvm.RoutineVisitID = rv.RoutineVisitID)
										                        AND (a.AdultId = rvm.AdultID)
	                                                            --LEFT JOIN 
		                                                           --  [ReferenceService] rs on (rs.RoutineVisitMemberID = rvm.RoutineVisitMemberID)
	                                                            LEFT JOIN 
		                                                             [RoutineVisitSupport] rvs on (rvs.RoutineVisitMemberID = rvm.RoutineVisitMemberID)
	                                                            LEFT JOIN
		                                                             [Domain] dom on dom.[DomainID] = rvs.SupportID
                                                            WHERE
		                                                            cp.CollaboratorRoleID = 2 AND a.HouseholdID IS NOT NULL
											                        --AND rv.RoutineVisitDate >= @initialDate AND rv.RoutineVisitDate <= @lastDate
		                                                            --AND hh.RegistrationDate >= @initialDate AND hh.RegistrationDate <= @lastDate
		                                                            --AND hh.RegistrationDate >= '2017/08/01' AND hh.RegistrationDate <= '2017/09/28'
	                                                            GROUP BY 
		                                                            cp.Name
		                                                            ,hh.HouseholdName
		                                                            ,p.Name
		                                                            ,a.FirstName
		                                                            ,a.LastName
		                                                            ,a.DateOfBirth
		                                                            ,a.gender
											                        ,siteocb.SiteName
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
				                        )agregado_obj
				                        LEFT JOIN
				                        (
					                        SELECT
						                        --,(c.FirstName) + ' ' + (c.LastName) As FullName
						                        a.FirstName
						                        ,a.LastName
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
						                        ,rv.RoutineVisitDate
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
								                         Adult a on (a.AdultID = rvm.AdultID) AND (hh.HouseHoldID=a.HouseholdID)
							                        --LEFT JOIN 
								                        --  [ReferenceService] rs on (rs.RoutineVisitMemberID = rvm.RoutineVisitMemberID)
							                        LEFT JOIN 
								                         [RoutineVisitSupport] rvs on (rvs.RoutineVisitMemberID = rvm.RoutineVisitMemberID)
							                        LEFT JOIN
								                         [Domain] dom on dom.[DomainID] = rvs.SupportID
							                        --LEFT JOIN 
								                        -- [Reference] r on (r.ReferenceServiceID = rs.ReferenceServiceID)
							                        --LEFT JOIN 
								                        -- [ReferenceType] rt on (rt.ReferenceTypeID = r.ReferenceTypeID)
						                        WHERE 
							                        cp.CollaboratorRoleID = 2  AND a.HouseholdID IS NOT NULL
							                        --AND rt.FieldType = 'CheckBox' AND rvm.ChildID IS NOT NULL
							                        --AND rv.RoutineVisitDate >= @initialDate AND rv.RoutineVisitDate <= @lastDate
							                        --AND rs.ReferenceDate >= @initialDate AND rs.ReferenceDate  <= @lastDate
							                        --AND rs.HealthAttendedDate >= @initialDate AND rs.HealthAttendedDate  <= @lastDate
						                        group by
							                        a.FirstName, a.LastName, rv.RoutineVisitDate
				                        )fichaseguimento_obj
				                        ON
				                        (
					                        agregado_obj.FirstName = fichaseguimento_obj.FirstName
					                        AND
					                        agregado_obj.LastName = fichaseguimento_obj.LastName
				                        )

				                        LEFT JOIN
				                        (
					                        SELECT
						                        a.FirstName
						                        ,a.LastName
						                        ,ISNULL(MAX(CASE WHEN rt.ReferenceName = 'ATS' AND rt.ReferenceCategory = 'Activist' AND r.Value <>'0'  AND r.Value <> '' THEN 'SIM' ELSE 'NÃO' END), '')  As ATS
						                        ,ISNULL(MAX(CASE WHEN rt.ReferenceName in ('PTV','Testado HIV+','Pré-TARV/IO','Abandono TARV','PPE') AND rt.ReferenceCategory = 'Activist' AND r.Value <>'0'  AND r.Value <> '' THEN 'SIM' ELSE 'NÃO' END), '')  As TARV
						                        ,ISNULL(MAX(CASE WHEN rt.ReferenceName = 'CCR' AND rt.ReferenceCategory = 'Activist' AND r.Value <>'0'  AND r.Value <> '' THEN 'SIM' ELSE 'NÃO' END), '')  As CCR
						                        ,ISNULL(MAX(CASE WHEN rt.ReferenceName in ('Maternidade p/ Parto','CPN','CPN Familiar','Consulta Pós-Parto','ITS','Circuncisao Masculina') AND rt.ReferenceCategory = 'Activist' AND r.Value <>'0'  AND r.Value <> '' THEN 'SIM' ELSE 'NÃO' END), '')  As SSR
						                        ,ISNULL(MAX(CASE WHEN rt.ReferenceName in ('GAVV','Apoio Psico-Social','Posto Policial') AND rt.ReferenceCategory = 'Activist' AND r.Value <>'0'  AND r.Value <> '' THEN 'SIM' ELSE 'NÃO' END), '') As VGB
						                        ,ISNULL(MAX(CASE WHEN rt.ReferenceName not in ('ATS','PTV','Testado HIV+','Pré-TARV/IO'
															                        ,'Abandono TARV','PPE','CCR','Maternidade p/ Parto','CPN'
															                        ,'CPN Familiar','Consulta Pós-Parto','ITS','Circuncisao Masculina'
															                        ,'GAVV','Apoio Psico-Social','Posto Policial')  
						                        AND rt.ReferenceCategory = 'Activist' AND r.Value <>'0'  AND r.Value <> '' THEN 'SIM' ELSE 'NÃO' END), '') As Others
						                        ,rs.ReferenceDate
						                        --,(CASE WHEN rs.ReferenceDate >= '2016/09/01' THEN rs.ReferenceDate ELSE NULL END) AS ReferenceDate

						                        ,ISNULL(MAX(CASE WHEN rt.ReferenceName = 'ATS' AND rt.ReferenceCategory in ('Health','Social') AND r.Value <>'0'  AND r.Value <> '' THEN 'SIM' ELSE 'NÃO' END), '') As RC_ATS
						                        ,ISNULL(MAX(CASE WHEN rt.ReferenceName in ('Pré TARV/IO','Testado HIV+','PPE','PTV','TARV','CD')  AND rt.ReferenceCategory in ('Health','Social') AND r.Value <>'0'  AND r.Value <> '' THEN 'SIM' ELSE 'NÃO' END), '') As RC_TARV
						                        ,ISNULL(MAX(CASE WHEN rt.ReferenceName = 'CCR' AND rt.ReferenceCategory in ('Health','Social') AND r.Value <>'0'  AND r.Value <> '' THEN 'SIM' ELSE 'NÃO' END), '') As RC_CCR
						                        ,ISNULL(MAX(CASE WHEN rt.ReferenceName in ('Maternidade p/ parto','CPN','CPN Familiar','Consulta Pós-parto','ITS') AND rt.ReferenceCategory in ('Health','Social') AND r.Value <>'0'  AND r.Value <> ''  THEN 'SIM' ELSE 'NÃO' END), '') As RC_SSR
						                        ,ISNULL(MAX(CASE WHEN rt.ReferenceName = 'GAVV' AND rt.ReferenceCategory in ('Health','Social')  AND r.Value <>'0'  AND r.Value <> '' THEN 'SIM' ELSE 'NÃO' END), '') As RC_VGB
						                        ,ISNULL(MAX(CASE WHEN rt.ReferenceName not in ('ATS','Pré TARV/IO','Testado HIV+','PPE','PTV','TARV','CD'
						                        ,'CCR','Maternidade p/ parto','CPN','CPN Familiar','Consulta Pós-parto','ITS','GAVV') AND rt.ReferenceCategory in ('Health','Social')  AND r.Value <>'0'  AND r.Value <> '' THEN 'SIM' ELSE 'NÃO' END), '') As RC_Others
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
								                         Adult a on (a.AdultId = rvm.AdultId) AND (hh.HouseHoldID=a.HouseholdID)
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
							                        cp.CollaboratorRoleID = 2  AND a.HouseholdID IS NOT NULL
							                        --AND rt.FieldType = 'CheckBox' --AND rvm.ChildID IS NOT NULL
							                        --AND rv.RoutineVisitDate >= @initialDate AND rv.RoutineVisitDate <= @lastDate
							                        --AND rs.ReferenceDate >= @initialDate AND rs.ReferenceDate  <= @lastDate
							                        --AND rs.HealthAttendedDate >= @initialDate AND rs.HealthAttendedDate  <= @lastDate
							                        AND rvm.childID IS NOT NULL
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
						                        group by
							                        a.FirstName, a.LastName, rs.ReferenceDate, rs.HealthAttendedDate, rs.SocialAttendedDate
				                        )ref_obj
				                        ON
				                        (
					                        fichaseguimento_obj.FirstName = ref_obj.FirstName
					                        AND
					                        fichaseguimento_obj.LastName = ref_obj.LastName
				                        )
                        )dados
                        ORDER BY
	                        dados.siteName DESC, dados.personType DESC";

            return UnitOfWork.DbContext.Database.SqlQuery<AgreggatedGlobalReportDTO>(query,
                                                new SqlParameter("initialDate", initialDate),
                                                new SqlParameter("lastDate", lastDate)).ToList();
        }

        /*
         * #################################################################################
         * ########## Get Data Agreggated By Prov And Dist And Site And Partner ############
         * #################################################################################
         */

        public List<AgreggatedBaseDataDTO> getDataAgreggatedByProvAndDistAndSiteAndPartner(int ProvID, int DistID, int SiteID, List<IPartnerNameReportDTO> list)
        {
            String query = @"SELECT 
	                            prov.Name As Province, dist.Name As District, s.SiteName As SiteName, part.Name As Partner, part.PartnerID
                             FROM
	                             [Partner] part
	                            inner join  [Site] s on (part.siteID = s.SiteID)
	                            inner join  [OrgUnit] dist on (dist.OrgUnitID = s.OrgUnitID)
	                            inner join  [OrgUnit] prov on (dist.ParentOrgUnitId = prov.OrgUnitID)
                            WHERE  
	                            ((@siteID = 0 AND @DistID = 0 AND @ProvID > 0 AND prov.OrgUnitID = @ProvID) OR 
	                            (@siteID = 0 AND @DistID > 0 AND @ProvID > 0 AND dist.OrgUnitID = @DistID) OR
	                            (@siteID = 0 AND @DistID > 0 AND dist.OrgUnitID = @DistID) OR 
	                            (@siteID > 0 AND s.SiteID = @siteID))
                            group by
	                            prov.Name, dist.Name, s.SiteName, part.Name, part.PartnerID
                            order by
                                part.Name";

            List<AgreggatedBaseDataDTO> data = UnitOfWork.DbContext.Database.SqlQuery<AgreggatedBaseDataDTO>(query,
                                                new SqlParameter("ProvID", ProvID),
                                                new SqlParameter("DistID", DistID),
                                                new SqlParameter("SiteID", SiteID)).ToList();

            foreach (AgreggatedBaseDataDTO agreggatedBaseDataDTO in data)
            {
                agreggatedBaseDataDTO.summaryReportDTO = list.Where(x => x.Partner == agreggatedBaseDataDTO.Partner).FirstOrDefault();
            }

            return data;
        }

        public List<AgreggatedMEDataDTO> getDataAgreggatedByProvAndDistAndSite(List<ISiteNameReportDTO> list)
        {
            String query = @"SELECT 
	                            prov.Name As Province, dist.Name As District, s.SiteName As SiteName
                            FROM [Site] s
                            inner join  [OrgUnit] dist on (dist.OrgUnitID = s.OrgUnitID)
                            inner join  [OrgUnit] prov on (dist.ParentOrgUnitId = prov.OrgUnitID)
                            WHERE s.SiteName not in ('NONE')
                            group by
	                            prov.Name, dist.Name, s.SiteName
                            order by
                                prov.Name, dist.Name, s.SiteName";

            List<AgreggatedMEDataDTO> data = UnitOfWork.DbContext.Database.SqlQuery<AgreggatedMEDataDTO>(query).ToList();

            foreach (AgreggatedMEDataDTO AgreggatedMEDataDTO in data)
            {
                AgreggatedMEDataDTO.siteNameReportDTO = list.Where(x => x.SiteName == AgreggatedMEDataDTO.SiteName).FirstOrDefault();
            }

            return data;
        }


        public List<FieldsDTO> getAgreggatedForAgeMEReport(DateTime initialDate, DateTime lastDate, string queryCode)
        {
            String query = @"SELECT
	                            Province
	                            ,District
	                            ,SiteName
	                            ,SUM(CAST(Field1 as int)) as Field1 
	                            ,SUM(CAST(Field2 as int)) as Field2 
	                            ,SUM(CAST(Field3 as int)) as Field3 
	                            ,SUM(CAST(Field4 as int)) as Field4 
	                            ,SUM(CAST(Field5 as int)) as Field5 
	                            ,SUM(CAST(Field6 as int)) as Field6 
	                            ,SUM(CAST(Field7 as int)) as Field7 
	                            ,SUM(CAST(Field8 as int)) as Field8 
	                            ,SUM(CAST(Field9 as int)) as Field9 
	                            ,SUM(CAST(Field10 as int)) as Field10
	                            ,SUM(CAST(Field11 as int)) as Field11
	                            ,SUM(CAST(Field12 as int)) as Field12
	                            ,SUM(CAST(Field13 as int)) as Field13
	                            ,SUM(CAST(Field14 as int)) as Field14
	                            ,SUM(CAST(Field15 as int)) as Field15
	                            ,SUM(CAST(Field16 as int)) as Field16
	                            ,SUM(CAST(Field17 as int)) as Field17
	                            ,SUM(CAST(Field18 as int)) as Field18
	                            ,SUM(CAST(Field19 as int)) as Field19
	                            ,SUM(CAST(Field20 as int)) as Field20
                                ,SUM(CAST(Field21 as int)) as Field21
                                ,SUM(CAST(Field22 as int)) as Field22
                                ,SUM(CAST(Field23 as int)) as Field23
                                ,SUM(CAST(Field24 as int)) as Field24
                                ,SUM(CAST(Field25 as int)) as Field25
                                ,SUM(CAST(Field26 as int)) as Field26
                                ,SUM(CAST(Field27 as int)) as Field27
                                ,SUM(CAST(Field28 as int)) as Field28
                                ,SUM(CAST(Field29 as int)) as Field29
                                ,SUM(CAST(Field30 as int)) as Field30
                                ,SUM(CAST(Field31 as int)) as Field31
                                ,SUM(CAST(Field32 as int)) as Field32
                                ,SUM(CAST(Field33 as int)) as Field33
                                ,SUM(CAST(Field34 as int)) as Field34
                                ,SUM(CAST(Field35 as int)) as Field35
                            FROM [CSI_PROD].[dbo].[ReportData]
                            WHERE QueryCode = @queryCode
                            AND InitialPositionDate >= @initialDate 
                            AND FinalPositionDate <= @lastDate
                            GROUP BY Province, District, SiteName, QueryCode";

            return UnitOfWork.DbContext.Database.SqlQuery<FieldsDTO>(query,
                                                            new SqlParameter("initialDate", initialDate),
                                                            new SqlParameter("lastDate", lastDate),
                                                            new SqlParameter("queryCode", queryCode)).ToList();
        }

        public List<FieldsDTO> GetAgreggatedDataForMEReport(DateTime initialDate, DateTime lastDate)
        {
            String query = @"SELECT
	                            Province
	                            ,District
	                            ,SiteName
                                ,QueryCode
	                            ,SUM(CAST(Field1 as int)) as Field1 
	                            ,SUM(CAST(Field2 as int)) as Field2 
	                            ,SUM(CAST(Field3 as int)) as Field3 
	                            ,SUM(CAST(Field4 as int)) as Field4 
	                            ,SUM(CAST(Field5 as int)) as Field5 
	                            ,SUM(CAST(Field6 as int)) as Field6 
	                            ,SUM(CAST(Field7 as int)) as Field7 
	                            ,SUM(CAST(Field8 as int)) as Field8 
	                            ,SUM(CAST(Field9 as int)) as Field9 
	                            ,SUM(CAST(Field10 as int)) as Field10
	                            ,SUM(CAST(Field11 as int)) as Field11
	                            ,SUM(CAST(Field12 as int)) as Field12
	                            ,SUM(CAST(Field13 as int)) as Field13
	                            ,SUM(CAST(Field14 as int)) as Field14
	                            ,SUM(CAST(Field15 as int)) as Field15
	                            ,SUM(CAST(Field16 as int)) as Field16
	                            ,SUM(CAST(Field17 as int)) as Field17
	                            ,SUM(CAST(Field18 as int)) as Field18
	                            ,SUM(CAST(Field19 as int)) as Field19
	                            ,SUM(CAST(Field20 as int)) as Field20
                                ,SUM(CAST(Field21 as int)) as Field21
                                ,SUM(CAST(Field22 as int)) as Field22
                                ,SUM(CAST(Field23 as int)) as Field23
                                ,SUM(CAST(Field24 as int)) as Field24
                                ,SUM(CAST(Field25 as int)) as Field25
                                ,SUM(CAST(Field26 as int)) as Field26
                                ,SUM(CAST(Field27 as int)) as Field27
                                ,SUM(CAST(Field28 as int)) as Field28
                                ,SUM(CAST(Field29 as int)) as Field29
                                ,SUM(CAST(Field30 as int)) as Field30
                                ,SUM(CAST(Field31 as int)) as Field31
                                ,SUM(CAST(Field32 as int)) as Field32
                                ,SUM(CAST(Field33 as int)) as Field33
                                ,SUM(CAST(Field34 as int)) as Field34
                                ,SUM(CAST(Field35 as int)) as Field35
                            FROM [CSI_PROD].[dbo].[ReportData]
                            WHERE InitialPositionDate >= @initialDate 
                            AND FinalPositionDate <= @lastDate
                            GROUP BY Province, District, SiteName, QueryCode";

            return UnitOfWork.DbContext.Database.SqlQuery<FieldsDTO>(query,
                                                            new SqlParameter("initialDate", initialDate),
                                                            new SqlParameter("lastDate", lastDate)).ToList();
        }

        public List<FieldsDTO> GetMEReportUniqueSiteNamesWithLocation()
        {
            String query = "select Distinct(SiteName), Province, District from [CSI_PROD].[dbo].[ReportData]";
            return UnitOfWork.DbContext.Database.SqlQuery<FieldsDTO>(query).ToList();
        }
    }
}
