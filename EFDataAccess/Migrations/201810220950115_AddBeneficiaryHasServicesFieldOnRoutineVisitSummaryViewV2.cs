namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddBeneficiaryHasServicesFieldOnRoutineVisitSummaryViewV2 : DbMigration
    {
        public override void Up()
        {
            Sql(@"DROP VIEW Routine_Visit_Summary");

            Sql(@"CREATE VIEW Routine_Visit_Summary AS
						SELECT * FROM 
						(
						SELECT
						cp.Name As ChiefPartner,
						p.[Name] As [Partner],
						'child' As BeneficiaryType,
						child.FirstName As BeneficiaryFirstName,
						child.LastName As BeneficiaryLastName,
						child.child_guid As [Guid],
						rvm.ChildID,
						rvm.AdultID,
                        rvm.RoutineVisitMemberID,
						DATEDIFF(year, CAST(child.DateOfBirth As Date), GETDATE()) As Age,
						rvs.RoutineVisitSupportID,
						cs.Description As BeneficiaryState,
						csh.EffectiveDate As BeneficiaryStateEffectiveDate,
						child.DateOfBirth,
						child.HIVTracked,
						CASE WHEN child.RegistrationDate IS NULL THEN hh.RegistrationDate ELSE 0 END As RegistrationDate,
						PartnerRole = CASE p.CollaboratorRoleID WHEN 3 THEN 'Supervior' WHEN 2 THEN 'Activista Chefe' ELSE 'Activista' END,
						FinaceAid = CASE WHEN rvs.SupportType in ('Domain','DomainEntity') AND rvs.SupportID = 6 THEN rvs.SupportValue ELSE 0 END,
						Health =  CASE WHEN rvs.SupportType in ('Domain','DomainEntity') AND rvs.SupportID = 3 THEN rvs.SupportValue ELSE 0 END,
						Food =  CASE WHEN rvs.SupportType in ('Domain','DomainEntity') AND rvs.SupportID = 1 THEN rvs.SupportValue ELSE 0 END,
						Education =  CASE WHEN rvs.SupportType in ('Domain','DomainEntity') AND rvs.SupportID = 2 THEN rvs.SupportValue ELSE 0 END,
						LegalAdvice =  CASE WHEN rvs.SupportType in ('Domain','DomainEntity') AND rvs.SupportID = 4 THEN rvs.SupportValue ELSE 0 END,
						Housing =  CASE WHEN rvs.SupportType in ('Domain','DomainEntity') AND rvs.SupportID = 7 THEN rvs.SupportValue ELSE 0 END,
						SocialAid =  CASE WHEN rvs.SupportType in ('Domain','DomainEntity') AND rvs.SupportID = 5 THEN rvs.SupportValue ELSE 0 END,
						DPI =  CASE WHEN rvs.SupportType = 'DPI' AND rvs.SupportID = 1 THEN rvs.SupportValue ELSE 0 END,
						HIVRisk =  CASE WHEN rvs.SupportType = 'HIVRISK' AND rvs.SupportID = 1 AND rvs.SupportValue = 'True' THEN 1 ELSE 0 END,
						MUACGreen =  CASE WHEN rvs.SupportType = 'MUAC' AND rvs.SupportID = 1 THEN rvs.SupportValue ELSE 0 END,
						MUACYellow =  CASE WHEN rvs.SupportType = 'MUAC' AND rvs.SupportID = 2 THEN rvs.SupportValue ELSE 0 END,
						MUACRed =  CASE WHEN rvs.SupportType = 'MUAC' AND rvs.SupportID = 3 THEN rvs.SupportValue ELSE 0 END,
						rv.FamilyKitReceived,
						rv.RoutineVisitDate,
						rvm.BeneficiaryHasServices
						FROM  [Partner] p
						inner join  [Partner] cp on (p.SuperiorId = cp.PartnerID)
						inner join  [HouseHold] hh on (p.PartnerID = hh.PartnerID)
						inner join  [RoutineVisit] rv on (hh.HouseHoldID = rv.HouseholdID)
						inner join  [RoutineVisitMember] rvm on (rvm.RoutineVisitID = rv.RoutineVisitID)
						inner join  [RoutineVisitSupport] rvs on (rvs.RoutineVisitMemberID = rvm.RoutineVisitMemberID)
						inner join  [Child] child ON (child.ChildID = rvm.ChildID)
						inner join  [ChildStatusHistory] csh on (csh.ChildStatusHistoryID = (SELECT max(csh2.ChildStatusHistoryID) FROM  [ChildStatusHistory] csh2 WHERE csh2.ChildID = child.ChildID))
						inner join  [ChildStatus] cs on (cs.StatusID = csh.ChildStatusID)
						Where p.CollaboratorRoleID = 1
						--
						UNION ALL
						--
						SELECT
						cp.Name As ChiefPartner,
						p.[Name] As [Partner],
						'adult' As BeneficiaryType,
						adult.FirstName As BeneficiaryFirstName,
						adult.LastName As BeneficiaryLastName,
						adult.AdultGuid As [Guid],
						rvm.ChildID,
						rvm.AdultID,
                        rvm.RoutineVisitMemberID,
						DATEDIFF(year, CAST(adult.DateOfBirth As Date), GETDATE()) As Age,
						rvs.RoutineVisitSupportID,
						cs.Description As BeneficiaryState,
						csh.EffectiveDate As BeneficiaryStateEffectiveDate,
						adult.DateOfBirth,
						adult.HIVTracked,
						CASE WHEN adult.RegistrationDate IS NULL THEN hh.RegistrationDate ELSE 0 END As RegistrationDate,
						PartnerRole = CASE p.CollaboratorRoleID WHEN 3 THEN 'Supervior' WHEN 2 THEN 'Activista Chefe' ELSE 'Activista' END,
						FinaceAid = CASE WHEN rvs.SupportType in ('Domain','DomainEntity') AND rvs.SupportID = 6 THEN rvs.SupportValue ELSE 0 END,
						Health =  CASE WHEN rvs.SupportType in ('Domain','DomainEntity') AND rvs.SupportID = 3 THEN rvs.SupportValue ELSE 0 END,
						Food =  CASE WHEN rvs.SupportType in ('Domain','DomainEntity') AND rvs.SupportID = 1 THEN rvs.SupportValue ELSE 0 END,
						Education =  CASE WHEN rvs.SupportType in ('Domain','DomainEntity') AND rvs.SupportID = 2 THEN rvs.SupportValue ELSE 0 END,
						LegalAdvice =  CASE WHEN rvs.SupportType in ('Domain','DomainEntity') AND rvs.SupportID = 4 THEN rvs.SupportValue ELSE 0 END,
						Housing =  CASE WHEN rvs.SupportType in ('Domain','DomainEntity') AND rvs.SupportID = 7 THEN rvs.SupportValue ELSE 0 END,
						SocialAid =  CASE WHEN rvs.SupportType in ('Domain','DomainEntity') AND rvs.SupportID = 5 THEN rvs.SupportValue ELSE 0 END,
						DPI =  CASE WHEN rvs.SupportType = 'DPI' AND rvs.SupportID = 1 THEN rvs.SupportValue ELSE 0 END,
						HIVRisk =  CASE WHEN rvs.SupportType = 'HIVRISK' AND rvs.SupportID = 1 AND rvs.SupportValue = 'True' THEN 1 ELSE 0 END,
						MUACGreen =  CASE WHEN rvs.SupportType = 'MUAC' AND rvs.SupportID = 1 THEN rvs.SupportValue ELSE 0 END,
						MUACYellow =  CASE WHEN rvs.SupportType = 'MUAC' AND rvs.SupportID = 2 THEN rvs.SupportValue ELSE 0 END,
						MUACRed =  CASE WHEN rvs.SupportType = 'MUAC' AND rvs.SupportID = 3 THEN rvs.SupportValue ELSE 0 END,
						rv.FamilyKitReceived,
						rv.RoutineVisitDate,
						rvm.BeneficiaryHasServices
						FROM  [Partner] p
						inner join  [Partner] cp on (p.SuperiorId = cp.PartnerID)
						inner join  [HouseHold] hh on (p.PartnerID = hh.PartnerID)
						inner join  [RoutineVisit] rv on (hh.HouseHoldID = rv.HouseholdID)
						inner join  [RoutineVisitMember] rvm on (rvm.RoutineVisitID = rv.RoutineVisitID)
						inner join  [RoutineVisitSupport] rvs on (rvs.RoutineVisitMemberID = rvm.RoutineVisitMemberID)
						inner join  [Adult] adult ON (adult.AdultId = rvm.AdultID)
						left join  [ChildStatusHistory] csh on (csh.ChildStatusHistoryID = (SELECT max(csh2.ChildStatusHistoryID) FROM  [ChildStatusHistory] csh2 WHERE csh2.AdultId = adult.AdultId))
						left join  [ChildStatus] cs on (cs.StatusID = csh.ChildStatusID)
						Where p.CollaboratorRoleID = 1
						) rvs");
        }
        
        public override void Down()
        {
        }
    }
}
