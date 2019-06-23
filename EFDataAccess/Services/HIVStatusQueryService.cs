using EFDataAccess.DTO;
using EFDataAccess.UOW;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using VPPS.CSI.Domain;
using VPPS.CSI.Domain;

namespace EFDataAccess.Services
{
    public class HIVStatusQueryService : BaseService
    {
        public HIVStatusQueryService(UnitOfWork uow) : base(uow)
        {
            UnitOfWork = uow;
        }

        public List<HIVStatusDTO> findAll()
        {
            String query = @"SELECT
                                hiv.HIVStatusID, hiv.SyncState,
                                hiv.HIVStatus_guid, hiv.HIV, hiv.HIVInTreatment, hiv.HIVUndisclosedReason, 
								hiv.InformationDate, hiv.CreatedAt, hiv.AdultID, hiv.ChildID,
								ISNULL(a.AdultGuid, (SELECT CAST(CAST(0 AS BINARY) AS UNIQUEIDENTIFIER))) As AdultGuid, 
								ISNULL(c.child_guid, (SELECT CAST(CAST(0 AS BINARY) AS UNIQUEIDENTIFIER))) As ChildGuid, 
								ISNULL(u.User_guid,'00000000-0000-0000-0000-000000000000') As CreatedUserGuid,
                                hiv.NID,
                                hiv.BeneficiaryGuid
                            FROM
	                            [HIVStatus] AS hiv
                            LEFT JOIN
	                            [Child] AS c ON c.[ChildID] = hiv.[ChildID]
                            LEFT JOIN
	                            [Adult] AS a ON a.AdultId = hiv.AdultID
                            LEFT JOIN
	                            [User] AS u ON u.UserID = hiv.UserID";

            return UnitOfWork.DbContext.Database.SqlQuery<VPPS.CSI.Domain.HIVStatusDTO>(query).ToList();
        }

        /*
         * ########################################################
         * ###  Relatório do Seroestado Inicial e Actualizado   ###
         * ########################################################
         */

        public List<InitialAndFinalHIVStatusDTO> getInitialAndFinalHIVStatus(DateTime initialDate, DateTime finalDate, int partnerID)
        {
            String query = @"SELECT
								hiv_obj1.ChiefPartner,
								SUM(hiv_obj1.HIV_N) + SUM(hiv_obj1.HIV_P_IN_TARV) + SUM(hiv_obj1.HIV_P_NOT_TARV) + SUM(hiv_obj1.HIV_KNOWN_NREVEAL) + SUM(hiv_obj1.HIV_UNKNOWN) AS BENEFICIARIES,
								SUM(hiv_obj1.HIV_N) As HIV_N1,
								SUM(hiv_obj1.HIV_P_IN_TARV) As HIV_P_IN_TARV1,
								SUM(hiv_obj1.HIV_P_NOT_TARV) As HIV_P_NOT_TARV1,
								SUM(hiv_obj1.HIV_KNOWN_NREVEAL) As HIV_KNOWN_NREVEAL1,
								SUM(hiv_obj1.HIV_UNKNOWN) As HIV_UNKNOWN1,
								SUM(hiv_obj2.HIV_N) As HIV_N2,
								SUM(hiv_obj2.HIV_P_IN_TARV) As HIV_P_IN_TARV2,
								SUM(hiv_obj2.HIV_P_NOT_TARV) As HIV_P_NOT_TARV2,
								SUM(hiv_obj2.HIV_KNOWN_NREVEAL) As HIV_KNOWN_NREVEAL2,
								SUM(hiv_obj2.HIV_UNKNOWN) As HIV_UNKNOWN2
								FROM
								(
									SELECT
									hiv_obj1.ChiefPartner As ChiefPartner,
									SUM(hiv_obj1.HIV_N) As HIV_N,
									SUM(hiv_obj1.HIV_P_IN_TARV) As HIV_P_IN_TARV,
									SUM(hiv_obj1.HIV_P_NOT_TARV) As HIV_P_NOT_TARV,
									SUM(hiv_obj1.HIV_KNOWN_NREVEAL) As HIV_KNOWN_NREVEAL,
									SUM(hiv_obj1.HIV_UNKNOWN) As HIV_UNKNOWN
									FROM
									(
										SELECT
										cp.Name As ChiefPartner, 
										ChiefPartnerRole = CASE cp.CollaboratorRoleID WHEN 3 THEN 'Supervior' WHEN 2 THEN 'Activista Chefe' ELSE 'Activista' END,
										p.Name As Partner, 
										PartnerRole = CASE p.CollaboratorRoleID WHEN 3 THEN 'Supervior' WHEN 2 THEN 'Activista Chefe' ELSE 'Activista' END,
										c.FirstName As FirstName,
										c.LastName As LastName,
										hs.HIV,
										HIV_N = CASE WHEN hs.HIV = 'N' THEN 1 ELSE 0 END,
										HIV_P_IN_TARV = CASE WHEN hs.HIV = 'P' AND hs.HIVInTreatment = 0 THEN 1 ELSE 0 END,
										HIV_P_NOT_TARV = CASE WHEN hs.HIV = 'P' AND hs.HIVInTreatment = 1 THEN 1 ELSE 0 END,
										HIV_KNOWN_NREVEAL = CASE WHEN hs.HIV = 'U' AND hs.HIVUndisclosedReason = 0 THEN 1 ELSE 0 END,
										HIV_UNKNOWN = CASE WHEN hs.HIV = 'U' AND hs.HIVUndisclosedReason = 1 THEN 1 ELSE 0 END
										FROM  [Partner] p
										inner join  [Partner] cp on (p.SuperiorId = cp.PartnerID) 
										inner join  [HouseHold] hh on (p.PartnerID = hh.PartnerID)
										inner join  [Child] c on (hh.HouseHoldID = c.HouseholdID)
										inner join  [HIVStatus] hs on (hs.HIVStatusID = c.HIVStatusID)
										Where p.CollaboratorRoleID = 1 and (cp.PartnerID = @partnerID or @partnerID = 0)
                                        and hs.InformationDate Between @initialDate and @finalDate

										union all

										SELECT
										cp.Name As ChiefPartner, 
										ChiefPartnerRole = CASE cp.CollaboratorRoleID WHEN 3 THEN 'Supervior' WHEN 2 THEN 'Activista Chefe' ELSE 'Activista' END,
										p.Name As Partner, 
										PartnerRole = CASE p.CollaboratorRoleID WHEN 3 THEN 'Supervior' WHEN 2 THEN 'Activista Chefe' ELSE 'Activista' END,
										a.FirstName As FirstName,
										a.LastName As LastName,
										hs.HIV,
										HIV_N = CASE WHEN hs.HIV = 'N' THEN 1 ELSE 0 END,
										HIV_P_IN_TARV = CASE WHEN hs.HIV = 'P' AND hs.HIVInTreatment = 0 THEN 1 ELSE 0 END,
										HIV_P_NOT_TARV = CASE WHEN hs.HIV = 'P' AND hs.HIVInTreatment = 1 THEN 1 ELSE 0 END,
										HIV_KNOWN_NREVEAL = CASE WHEN hs.HIV = 'U' AND hs.HIVUndisclosedReason = 0 THEN 1 ELSE 0 END,
										HIV_UNKNOWN = CASE WHEN hs.HIV = 'U' AND hs.HIVUndisclosedReason = 1 THEN 1 ELSE 0 END
										FROM  [Partner] p
										inner join  [Partner] cp on (p.SuperiorId = cp.PartnerID) 
										inner join  [HouseHold] hh on (p.PartnerID = hh.PartnerID)
										inner join  [Adult] a on (hh.HouseHoldID = a.HouseholdID)
										inner join  [HIVStatus] hs on (hs.HIVStatusID = a.HIVStatusID)
										Where p.CollaboratorRoleID = 1 and (cp.PartnerID = @partnerID or @partnerID = 0)
                                        and hs.InformationDate Between @initialDate and @finalDate
									) hiv_obj1 
									group by hiv_obj1.ChiefPartner
								) hiv_obj1 inner join
								(
									SELECT
									hiv_obj2.ChiefPartner As ChiefPartner,
									SUM(hiv_obj2.HIV_N) As HIV_N,
									SUM(hiv_obj2.HIV_P_IN_TARV) As HIV_P_IN_TARV,
									SUM(hiv_obj2.HIV_P_NOT_TARV) As HIV_P_NOT_TARV,
									SUM(hiv_obj2.HIV_KNOWN_NREVEAL) As HIV_KNOWN_NREVEAL,
									SUM(hiv_obj2.HIV_UNKNOWN) As HIV_UNKNOWN
									FROM
									(
										SELECT
										cp.Name As ChiefPartner, 
										ChiefPartnerRole = CASE cp.CollaboratorRoleID WHEN 3 THEN 'Supervior' WHEN 2 THEN 'Activista Chefe' ELSE 'Activista' END,
										p.Name As Partner, 
										PartnerRole = CASE p.CollaboratorRoleID WHEN 3 THEN 'Supervior' WHEN 2 THEN 'Activista Chefe' ELSE 'Activista' END,
										c.FirstName As FirstName,
										c.LastName As LastName,
										hs.HIV,
										HIV_N = CASE WHEN hs.HIV = 'N' THEN 1 ELSE 0 END,
										HIV_P_IN_TARV = CASE WHEN hs.HIV = 'P' AND hs.HIVInTreatment = 0 THEN 1 ELSE 0 END,
										HIV_P_NOT_TARV = CASE WHEN hs.HIV = 'P' AND hs.HIVInTreatment = 1 THEN 1 ELSE 0 END,
										HIV_KNOWN_NREVEAL = CASE WHEN hs.HIV = 'U' AND hs.HIVUndisclosedReason = 0 THEN 1 ELSE 0 END,
										HIV_UNKNOWN = CASE WHEN hs.HIV = 'U' AND hs.HIVUndisclosedReason = 1 THEN 1 ELSE 0 END
										FROM  [Partner] p
										inner join  [Partner] cp on (p.SuperiorId = cp.PartnerID) 
										inner join  [HouseHold] hh on (p.PartnerID = hh.PartnerID)
										inner join  [Child] c on (hh.HouseHoldID = c.HouseholdID)
										inner join  [HIVStatus] hs on (hs.HIVStatusID = 
										(SELECT max(hs1.HIVStatusID) FROM  [HIVStatus] hs1 WHERE hs1.ChildID = c.ChildID))
										Where p.CollaboratorRoleID = 1 and (cp.PartnerID = @partnerID or @partnerID = 0)
                                        and hs.InformationDate Between @initialDate and @finalDate

										union all

										SELECT
										cp.Name As ChiefPartner, 
										ChiefPartnerRole = CASE cp.CollaboratorRoleID WHEN 3 THEN 'Supervior' WHEN 2 THEN 'Activista Chefe' ELSE 'Activista' END,
										p.Name As Partner, 
										PartnerRole = CASE p.CollaboratorRoleID WHEN 3 THEN 'Supervior' WHEN 2 THEN 'Activista Chefe' ELSE 'Activista' END,
										a.FirstName As FirstName,
										a.LastName As LastName,
										hs.HIV,
										HIV_N = CASE WHEN hs.HIV = 'N' THEN 1 ELSE 0 END,
										HIV_P_IN_TARV = CASE WHEN hs.HIV = 'P' AND hs.HIVInTreatment = 0 THEN 1 ELSE 0 END,
										HIV_P_NOT_TARV = CASE WHEN hs.HIV = 'P' AND hs.HIVInTreatment = 1 THEN 1 ELSE 0 END,
										HIV_KNOWN_NREVEAL = CASE WHEN hs.HIV = 'U' AND hs.HIVUndisclosedReason = 0 THEN 1 ELSE 0 END,
										HIV_UNKNOWN = CASE WHEN hs.HIV = 'U' AND hs.HIVUndisclosedReason = 1 THEN 1 ELSE 0 END
										FROM  [Partner] p
										inner join  [Partner] cp on (p.SuperiorId = cp.PartnerID) 
										inner join  [HouseHold] hh on (p.PartnerID = hh.PartnerID)
										inner join  [Adult] a on (hh.HouseHoldID = a.HouseholdID)
										inner join  [HIVStatus] hs on (hs.HIVStatusID = 
										(SELECT max(hs1.HIVStatusID) FROM  [HIVStatus] hs1 WHERE hs1.AdultID = a.AdultId))
										Where p.CollaboratorRoleID = 1 and (cp.PartnerID = @partnerID or @partnerID = 0)
                                        and hs.InformationDate Between @initialDate and @finalDate
									) hiv_obj2
									group by hiv_obj2.ChiefPartner
								) hiv_obj2 on (hiv_obj1.ChiefPartner = hiv_obj2.ChiefPartner)
								group by hiv_obj1.ChiefPartner";
            return UnitOfWork.DbContext.Database.SqlQuery<InitialAndFinalHIVStatusDTO>(query,
                                                            new SqlParameter("initialDate", initialDate),
                                                            new SqlParameter("finalDate", finalDate),
                                                            new SqlParameter("partnerID", partnerID)).ToList();
        }

        /*
         * ########################################################
         * #######  Relatório de Mudanças do Seroestado   #########
         * ########################################################
         */

        public List<HIVStatusChangesDTO> getHIVStatusChanges(DateTime initialDate, DateTime finalDate, int partnerID, String initialHIVState, String finalHIVState)
        {
            String query = @"SELECT
								hiv_obj1.Partner,
								COUNT(hiv_obj1.FirstName) As Beneficiaries,
								hiv_obj1.Initial_HIV_State,
								hiv_obj2.Final_HIV_State
								FROM
								(
									SELECT
									hiv_obj1.Partner,
									hiv_obj1.FirstName,
									hiv_obj1.LastName,
									hiv_obj1.Initial_HIV_State
									FROM
									(
										SELECT
										p.Name As Partner,
										c.FirstName As FirstName,
										c.LastName As LastName,
										CASE 
											WHEN hs.HIV = 'N' THEN 'HIV Negativo'
											WHEN hs.HIV = 'P' AND hs.HIVInTreatment = 0 THEN 'HIV Positivo em Tratamento'
											WHEN hs.HIV = 'P' AND hs.HIVInTreatment = 1 THEN 'HIV Positivo sem Tratamento'
											WHEN hs.HIV = 'U' AND hs.HIVUndisclosedReason = 0 THEN 'Estado HIV Não revelado'
											WHEN hs.HIV = 'U' AND hs.HIVUndisclosedReason = 1 THEN 'Estado HIV Desconhecido'
										END As Initial_HIV_State
										FROM  [Partner] p 
										inner join  [HouseHold] hh on (p.PartnerID = hh.PartnerID)
										inner join  [Child] c on (hh.HouseHoldID = c.HouseholdID)
										inner join  [HIVStatus] hs on (hs.HIVStatusID = c.HIVStatusID)
										Where p.CollaboratorRoleID = 1 and (p.PartnerID = @partnerID or @partnerID = 0)
                                        and hs.InformationDate Between @initialDate and @finalDate

										union all

										SELECT
										p.Name As Partner,
										a.FirstName As FirstName,
										a.LastName As LastName,
										CASE 
											WHEN hs.HIV = 'N' THEN 'HIV Negativo'
											WHEN hs.HIV = 'P' AND hs.HIVInTreatment = 0 THEN 'HIV Positivo em Tratamento'
											WHEN hs.HIV = 'P' AND hs.HIVInTreatment = 1 THEN 'HIV Positivo sem Tratamento'
											WHEN hs.HIV = 'U' AND hs.HIVUndisclosedReason = 0 THEN 'Estado HIV Não revelado'
											WHEN hs.HIV = 'U' AND hs.HIVUndisclosedReason = 1 THEN 'Estado HIV Desconhecido'
										END As Initial_HIV_State
										FROM  [Partner] p 
										inner join  [HouseHold] hh on (p.PartnerID = hh.PartnerID)
										inner join  [Adult] a on (hh.HouseHoldID = a.HouseholdID)
										inner join  [HIVStatus] hs on (hs.HIVStatusID = a.HIVStatusID)
										Where p.CollaboratorRoleID = 1 and (p.PartnerID = @partnerID or @partnerID = 0)
                                        and hs.InformationDate Between @initialDate and @finalDate

									) hiv_obj1 
									group by hiv_obj1.Partner, hiv_obj1.FirstName, hiv_obj1.LastName, hiv_obj1.Initial_HIV_State
								) hiv_obj1 inner join
								(
									SELECT
									hiv_obj2.Partner As Partner,
									hiv_obj2.FirstName As FirstName,
									hiv_obj2.LastName As LastName,
									hiv_obj2.Final_HIV_State
									FROM
									(
										SELECT
										p.Name As Partner,
										c.FirstName As FirstName,
										c.LastName As LastName,
										hs.HIVStatusID,
										CASE 
											WHEN hs.HIV = 'N' THEN 'HIV Negativo'
											WHEN hs.HIV = 'P' AND hs.HIVInTreatment = 0 THEN 'HIV Positivo em Tratamento'
											WHEN hs.HIV = 'P' AND hs.HIVInTreatment = 1 THEN 'HIV Positivo sem Tratamento'
											WHEN hs.HIV = 'U' AND hs.HIVUndisclosedReason = 0 THEN 'Estado HIV Não revelado'
											WHEN hs.HIV = 'U' AND hs.HIVUndisclosedReason = 1 THEN 'Estado HIV Desconhecido'
										END As Final_HIV_State
										FROM  [Partner] p
										inner join  [HouseHold] hh on (p.PartnerID = hh.PartnerID)
										inner join  [Child] c on (hh.HouseHoldID = c.HouseholdID)
										inner join  [HIVStatus] hs on (hs.HIVStatusID = 
										(SELECT hs1.HIVStatusID FROM  [HIVStatus] hs1 WHERE hs1.ChildID = c.ChildID and c.HIVStatusID != hs1.HIVStatusID))
										Where p.CollaboratorRoleID = 1 and (p.PartnerID = @partnerID or @partnerID = 0)
                                        and hs.InformationDate Between @initialDate and @finalDate

										union all

										SELECT
										p.Name As Partner,
										a.FirstName As FirstName,
										a.LastName As LastName,
										hs.HIVStatusID,
										CASE 
											WHEN hs.HIV = 'N' THEN 'HIV Negativo'
											WHEN hs.HIV = 'P' AND hs.HIVInTreatment = 0 THEN 'HIV Positivo em Tratamento'
											WHEN hs.HIV = 'P' AND hs.HIVInTreatment = 1 THEN 'HIV Positivo sem Tratamento'
											WHEN hs.HIV = 'U' AND hs.HIVUndisclosedReason = 0 THEN 'Estado HIV Não revelado'
											WHEN hs.HIV = 'U' AND hs.HIVUndisclosedReason = 1 THEN 'Estado HIV Desconhecido'
										END As Final_HIV_State
										FROM  [Partner] p
										inner join  [HouseHold] hh on (p.PartnerID = hh.PartnerID)
										inner join  [Adult] a on (hh.HouseHoldID = a.HouseholdID)
										inner join  [HIVStatus] hs on (hs.HIVStatusID = 
										(SELECT hs1.HIVStatusID FROM  [HIVStatus] hs1 WHERE hs1.ChildID = a.AdultID and a.HIVStatusID != hs1.HIVStatusID))
										Where p.CollaboratorRoleID = 1 and (p.PartnerID = @partnerID or @partnerID = 0)
                                        and hs.InformationDate Between @initialDate and @finalDate
									) hiv_obj2
									group by hiv_obj2.Partner, hiv_obj2.FirstName, hiv_obj2.LastName, hiv_obj2.Final_HIV_State
								) hiv_obj2 on (hiv_obj1.Partner = hiv_obj2.Partner and hiv_obj1.FirstName = hiv_obj2.FirstName and hiv_obj1.LastName = hiv_obj2.LastName)
								WHERE hiv_obj1.Initial_HIV_State != hiv_obj2.Final_HIV_State
                                and hiv_obj1.Initial_HIV_State = @initialHIVState and hiv_obj2.Final_HIV_State = @finalHIVState
								group by 
								hiv_obj1.Partner,
								hiv_obj1.Initial_HIV_State, 
								hiv_obj2.Final_HIV_State";
            return UnitOfWork.DbContext.Database.SqlQuery<HIVStatusChangesDTO>(query,
                                                            new SqlParameter("initialDate", initialDate),
                                                            new SqlParameter("finalDate", finalDate),
                                                            new SqlParameter("partnerID", partnerID),
                                                            new SqlParameter("initialHIVState", initialHIVState),
                                                            new SqlParameter("finalHIVState", finalHIVState)
                                                            ).ToList();
        }

        /*
         * ########################################################
         * #####  Relatório de Beneficiários e Seroestado   #######
         * ########################################################
         */

        public List<BeneficiariesAndHIVStatusDTO> getBeneficiariesAndHIVStatus(String hivState, int partnerID)
        {
            String query = @"SELECT  obj.Partner, obj.FirstName, obj.LastName, obj.Gender, obj.Age, obj.HIV_State
                            FROM
                            (
                            SELECT
							c.FirstName As FirstName,
							c.LastName As LastName,
							p.Name As Partner,
							c.Gender,
							DATEDIFF(year, CAST(c.DateOfBirth As Date), GETDATE()) As Age,
							CASE 
								WHEN hs.HIV = 'N' THEN 'HIV Negativo'
								WHEN hs.HIV = 'P' AND hs.HIVInTreatment = 0 THEN 'HIV Positivo em Tratamento'
								WHEN hs.HIV = 'P' AND hs.HIVInTreatment = 1 THEN 'HIV Positivo sem Tratamento'
								WHEN hs.HIV = 'U' AND hs.HIVUndisclosedReason = 0 THEN 'Estado HIV Não revelado'
								WHEN hs.HIV = 'U' AND hs.HIVUndisclosedReason = 1 THEN 'Estado HIV Desconhecido'
							END As HIV_State
							FROM  [Partner] p
							inner join  [Partner] cp on (p.SuperiorId = cp.PartnerID) 
							inner join  [HouseHold] hh on (p.PartnerID = hh.PartnerID)
							inner join  [Child] c on (hh.HouseHoldID = c.HouseholdID)
							inner join  [HIVStatus] hs on (hs.HIVStatusID = c.HIVStatusID)
							Where p.CollaboratorRoleID = 1
                            and (p.PartnerID = @PartnerID or @PartnerID = 0)

							union all

							SELECT
							a.FirstName As FirstName,
							a.LastName As LastName,
							p.Name As ChiefPartner,
							a.Gender,
							DATEDIFF(year, CAST(a.DateOfBirth As Date), GETDATE()) As Age,
							CASE 
								WHEN hs.HIV = 'N' THEN 'HIV Negativo'
								WHEN hs.HIV = 'P' AND hs.HIVInTreatment = 0 THEN 'HIV Positivo em Tratamento'
								WHEN hs.HIV = 'P' AND hs.HIVInTreatment = 1 THEN 'HIV Positivo sem Tratamento'
								WHEN hs.HIV = 'U' AND hs.HIVUndisclosedReason = 0 THEN 'Estado HIV Não revelado'
								WHEN hs.HIV = 'U' AND hs.HIVUndisclosedReason = 1 THEN 'Estado HIV Desconhecido'
							END As HIV_State
							FROM  [Partner] p
							inner join  [Partner] cp on (p.SuperiorId = cp.PartnerID) 
							inner join  [HouseHold] hh on (p.PartnerID = hh.PartnerID)
							inner join  [Adult] a on (hh.HouseHoldID = a.HouseholdID)
							inner join  [HIVStatus] hs on (hs.HIVStatusID = a.HIVStatusID)
							Where p.CollaboratorRoleID = 1
                            and (p.PartnerID = @PartnerID or @PartnerID = 0)
                            ) obj";
            //obj WHERE @hivState IS NULL or obj.HIV_State = @hivState";
            return UnitOfWork.DbContext.Database.SqlQuery<BeneficiariesAndHIVStatusDTO>(query, 
                                                            new SqlParameter("partnerID", partnerID)
                                                            //new SqlParameter("hivState", hivState)
                                                            ).ToList();
        }

        public List<UniqueEntity> FindAllHIVStatusUniqueEntity() => UnitOfWork.DbContext.Database.SqlQuery<UniqueEntity>("Select ISNULL(SyncGuid, cast(cast(0 as binary) as uniqueidentifier)) As SyncGuid, HIVStatusID As ID from HIVStatus").ToList();
    }
}
