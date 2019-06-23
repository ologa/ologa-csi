namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReCreateSystemViews : DbMigration
    {
        public override void Up()
        {
            Sql("DROP VIEW Children_Routine_Visit_Summary", false);
            Sql("DROP VIEW Adults_Routine_Visit_Summary", false);
            Sql("DROP VIEW Routine_Visit_Summary", false);
            Sql("DROP VIEW Beneficiary", false);

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
						FinaceAid = CASE WHEN rvs.SupportType = 'Domain' AND rvs.SupportID = 6 THEN rvs.SupportValue ELSE 0 END,
						Health =  CASE WHEN rvs.SupportType = 'Domain' AND rvs.SupportID = 3 THEN rvs.SupportValue ELSE 0 END,
						Food =  CASE WHEN rvs.SupportType = 'Domain' AND rvs.SupportID = 1 THEN rvs.SupportValue ELSE 0 END,
						Education =  CASE WHEN rvs.SupportType = 'Domain' AND rvs.SupportID = 2 THEN rvs.SupportValue ELSE 0 END,
						LegalAdvice =  CASE WHEN rvs.SupportType = 'Domain' AND rvs.SupportID = 4 THEN rvs.SupportValue ELSE 0 END,
						Housing =  CASE WHEN rvs.SupportType = 'Domain' AND rvs.SupportID = 7 THEN rvs.SupportValue ELSE 0 END,
						SocialAid =  CASE WHEN rvs.SupportType = 'Domain' AND rvs.SupportID = 5 THEN rvs.SupportValue ELSE 0 END,
						DPI =  CASE WHEN rvs.SupportType = 'DPI' AND rvs.SupportID = 1 THEN rvs.SupportValue ELSE 0 END,
						HIVRisk =  CASE WHEN rvs.SupportType = 'HIVRISK' AND rvs.SupportID = 1 AND rvs.SupportValue = 'True' THEN 1 ELSE 0 END,
						MUACGreen =  CASE WHEN rvs.SupportType = 'MUAC' AND rvs.SupportID = 1 THEN rvs.SupportValue ELSE 0 END,
						MUACYellow =  CASE WHEN rvs.SupportType = 'MUAC' AND rvs.SupportID = 2 THEN rvs.SupportValue ELSE 0 END,
						MUACRed =  CASE WHEN rvs.SupportType = 'MUAC' AND rvs.SupportID = 3 THEN rvs.SupportValue ELSE 0 END,
						rv.FamilyKitReceived,
						rv.RoutineVisitDate
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
						FinaceAid = CASE WHEN rvs.SupportType = 'Domain' AND rvs.SupportID = 6 THEN rvs.SupportValue ELSE 0 END,
						Health =  CASE WHEN rvs.SupportType = 'Domain' AND rvs.SupportID = 3 THEN rvs.SupportValue ELSE 0 END,
						Food =  CASE WHEN rvs.SupportType = 'Domain' AND rvs.SupportID = 1 THEN rvs.SupportValue ELSE 0 END,
						Education =  CASE WHEN rvs.SupportType = 'Domain' AND rvs.SupportID = 2 THEN rvs.SupportValue ELSE 0 END,
						LegalAdvice =  CASE WHEN rvs.SupportType = 'Domain' AND rvs.SupportID = 4 THEN rvs.SupportValue ELSE 0 END,
						Housing =  CASE WHEN rvs.SupportType = 'Domain' AND rvs.SupportID = 7 THEN rvs.SupportValue ELSE 0 END,
						SocialAid =  CASE WHEN rvs.SupportType = 'Domain' AND rvs.SupportID = 5 THEN rvs.SupportValue ELSE 0 END,
						DPI =  CASE WHEN rvs.SupportType = 'DPI' AND rvs.SupportID = 1 THEN rvs.SupportValue ELSE 0 END,
						HIVRisk =  CASE WHEN rvs.SupportType = 'HIVRISK' AND rvs.SupportID = 1 AND rvs.SupportValue = 'True' THEN 1 ELSE 0 END,
						MUACGreen =  CASE WHEN rvs.SupportType = 'MUAC' AND rvs.SupportID = 1 THEN rvs.SupportValue ELSE 0 END,
						MUACYellow =  CASE WHEN rvs.SupportType = 'MUAC' AND rvs.SupportID = 2 THEN rvs.SupportValue ELSE 0 END,
						MUACRed =  CASE WHEN rvs.SupportType = 'MUAC' AND rvs.SupportID = 3 THEN rvs.SupportValue ELSE 0 END,
						rv.FamilyKitReceived,
						rv.RoutineVisitDate
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
						) rvs", false);

            Sql(@"CREATE VIEW Children_Routine_Visit_Summary
						AS
						SELECT
						cp.Name As ChiefPartner,
						p.[Name] As [Partner],
						'child' As BeneficiaryType,
						child.FirstName As BeneficiaryFirstName,
						child.LastName As BeneficiaryLastName,
						child.child_guid As [Guid],
						rvm.ChildID,
						rvm.AdultID,
						DATEDIFF(year, CAST(child.DateOfBirth As Date), GETDATE()) As Age,
						rvs.RoutineVisitSupportID,
						cs.Description As BeneficiaryState,
						csh.EffectiveDate As BeneficiaryStateEffectiveDate,
						child.DateOfBirth,
						child.HIVTracked,
						CASE WHEN child.RegistrationDate IS NULL THEN hh.RegistrationDate ELSE 0 END As RegistrationDate,
						PartnerRole = CASE p.CollaboratorRoleID WHEN 3 THEN 'Supervior' WHEN 2 THEN 'Activista Chefe' ELSE 'Activista' END,
						FinaceAid = CASE WHEN rvs.SupportType = 'Domain' AND rvs.SupportID = 6 THEN rvs.SupportValue ELSE 0 END,
						Health =  CASE WHEN rvs.SupportType = 'Domain' AND rvs.SupportID = 3 THEN rvs.SupportValue ELSE 0 END,
						Food =  CASE WHEN rvs.SupportType = 'Domain' AND rvs.SupportID = 1 THEN rvs.SupportValue ELSE 0 END,
						Education =  CASE WHEN rvs.SupportType = 'Domain' AND rvs.SupportID = 2 THEN rvs.SupportValue ELSE 0 END,
						LegalAdvice =  CASE WHEN rvs.SupportType = 'Domain' AND rvs.SupportID = 4 THEN rvs.SupportValue ELSE 0 END,
						Housing =  CASE WHEN rvs.SupportType = 'Domain' AND rvs.SupportID = 7 THEN rvs.SupportValue ELSE 0 END,
						SocialAid =  CASE WHEN rvs.SupportType = 'Domain' AND rvs.SupportID = 5 THEN rvs.SupportValue ELSE 0 END,
						DPI =  CASE WHEN rvs.SupportType = 'DPI' AND rvs.SupportID = 1 THEN rvs.SupportValue ELSE 0 END,
						HIVRisk =  CASE WHEN rvs.SupportType = 'HIVRISK' AND rvs.SupportID = 1 AND rvs.SupportValue = 'True' THEN 1 ELSE 0 END,
						MUACGreen =  CASE WHEN rvs.SupportType = 'MUAC' AND rvs.SupportID = 1 THEN rvs.SupportValue ELSE 0 END,
						MUACYellow =  CASE WHEN rvs.SupportType = 'MUAC' AND rvs.SupportID = 2 THEN rvs.SupportValue ELSE 0 END,
						MUACRed =  CASE WHEN rvs.SupportType = 'MUAC' AND rvs.SupportID = 3 THEN rvs.SupportValue ELSE 0 END,
						FamilyKitReceived = CASE WHEN rv.FamilyKitReceived = 'True' THEN 1 ELSE 0 END,
						rv.RoutineVisitDate
						FROM  [Partner] p
						inner join  [Partner] cp on (p.SuperiorId = cp.PartnerID)
						inner join  [HouseHold] hh on (p.PartnerID = hh.PartnerID)
						inner join  [RoutineVisit] rv on (hh.HouseHoldID = rv.HouseholdID)
						inner join  [RoutineVisitMember] rvm on (rvm.RoutineVisitID = rv.RoutineVisitID)
						inner join  [RoutineVisitSupport] rvs on (rvs.RoutineVisitMemberID = rvm.RoutineVisitMemberID)
						inner join  [Child] child ON (child.ChildID = rvm.ChildID)
						inner join  [ChildStatusHistory] csh on (csh.ChildStatusHistoryID = (SELECT max(csh2.ChildStatusHistoryID) FROM  [ChildStatusHistory] csh2 WHERE csh2.ChildID = child.ChildID))
						inner join  [ChildStatus] cs on (cs.StatusID = csh.ChildStatusID)
						Where p.CollaboratorRoleID = 1", false);

            Sql(@"CREATE VIEW Adults_Routine_Visit_Summary
						AS
						SELECT
						cp.Name As ChiefPartner,
						p.[Name] As [Partner],
						'adult' As BeneficiaryType,
						adult.FirstName As BeneficiaryFirstName,
						adult.LastName As BeneficiaryLastName,
						adult.AdultGuid As [Guid],
						rvm.ChildID,
						rvm.AdultID,
						DATEDIFF(year, CAST(adult.DateOfBirth As Date), GETDATE()) As Age,
						rvs.RoutineVisitSupportID,
						cs.Description As BeneficiaryState,
						csh.EffectiveDate As BeneficiaryStateEffectiveDate,
						adult.DateOfBirth,
						adult.HIVTracked,
						CASE WHEN adult.RegistrationDate IS NULL THEN hh.RegistrationDate ELSE 0 END As RegistrationDate,
						PartnerRole = CASE p.CollaboratorRoleID WHEN 3 THEN 'Supervior' WHEN 2 THEN 'Activista Chefe' ELSE 'Activista' END,
						FinaceAid = CASE WHEN rvs.SupportType = 'Domain' AND rvs.SupportID = 6 THEN rvs.SupportValue ELSE 0 END,
						Health =  CASE WHEN rvs.SupportType = 'Domain' AND rvs.SupportID = 3 THEN rvs.SupportValue ELSE 0 END,
						Food =  CASE WHEN rvs.SupportType = 'Domain' AND rvs.SupportID = 1 THEN rvs.SupportValue ELSE 0 END,
						Education =  CASE WHEN rvs.SupportType = 'Domain' AND rvs.SupportID = 2 THEN rvs.SupportValue ELSE 0 END,
						LegalAdvice =  CASE WHEN rvs.SupportType = 'Domain' AND rvs.SupportID = 4 THEN rvs.SupportValue ELSE 0 END,
						Housing =  CASE WHEN rvs.SupportType = 'Domain' AND rvs.SupportID = 7 THEN rvs.SupportValue ELSE 0 END,
						SocialAid =  CASE WHEN rvs.SupportType = 'Domain' AND rvs.SupportID = 5 THEN rvs.SupportValue ELSE 0 END,
						DPI =  CASE WHEN rvs.SupportType = 'DPI' AND rvs.SupportID = 1 THEN rvs.SupportValue ELSE 0 END,
						HIVRisk =  CASE WHEN rvs.SupportType = 'HIVRISK' AND rvs.SupportID = 1 AND rvs.SupportValue = 'True' THEN 1 ELSE 0 END,
						MUACGreen =  CASE WHEN rvs.SupportType = 'MUAC' AND rvs.SupportID = 1 THEN rvs.SupportValue ELSE 0 END,
						MUACYellow =  CASE WHEN rvs.SupportType = 'MUAC' AND rvs.SupportID = 2 THEN rvs.SupportValue ELSE 0 END,
						MUACRed =  CASE WHEN rvs.SupportType = 'MUAC' AND rvs.SupportID = 3 THEN rvs.SupportValue ELSE 0 END,
						FamilyKitReceived =  CASE WHEN rv.FamilyKitReceived = 'True' THEN 1 ELSE 0 END,
						rv.RoutineVisitDate
						FROM  [Partner] p
						inner join  [Partner] cp on (p.SuperiorId = cp.PartnerID)
						inner join  [HouseHold] hh on (p.PartnerID = hh.PartnerID)
						inner join  [RoutineVisit] rv on (hh.HouseHoldID = rv.HouseholdID)
						inner join  [RoutineVisitMember] rvm on (rvm.RoutineVisitID = rv.RoutineVisitID)
						inner join  [RoutineVisitSupport] rvs on (rvs.RoutineVisitMemberID = rvm.RoutineVisitMemberID)
						inner join  [Adult] adult ON (adult.AdultId = rvm.AdultID)
						left join  [ChildStatusHistory] csh on (csh.ChildStatusHistoryID = (SELECT max(csh2.ChildStatusHistoryID) FROM  [ChildStatusHistory] csh2 WHERE csh2.AdultId = adult.AdultId))
						left join  [ChildStatus] cs on (cs.StatusID = csh.ChildStatusID)
						Where p.CollaboratorRoleID = 1", false);

            Sql(@"CREATE VIEW Beneficiary as 
						select * from 
						( 
						select 'adult' as Type, a.AdultID as ID, a.AdultGuid As Guid, a.FirstName, a.LastName, a.DateOfBirth, a.Gender,
						DATEDIFF(year, CAST(a.DateOfBirth As Date), GETDATE()) As Age, cs.Description As BeneficiaryState, csh.EffectiveDate As BeneficiaryStateEffectiveDate,
						a.HouseholdID, KinshipToFamilyHeadID, a.HIVStatusID, hs.HIVStatusID As LastHIVStatusID, HIVTracked, 0 As OVCTypeID,
						CASE WHEN a.RegistrationDate IS NULL THEN h.RegistrationDate ELSE a.RegistrationDate END As RegistrationDate
						from Adult a					
						inner join  [ChildStatusHistory] csh on (csh.ChildStatusHistoryID = (SELECT max(csh2.ChildStatusHistoryID) FROM  [ChildStatusHistory] csh2 WHERE csh2.AdultId = a.AdultId))
						inner join  [ChildStatus] cs on (cs.StatusID = csh.ChildStatusID)
						inner join  [Household] h on (a.HouseholdID = h.HouseHoldID)
						inner join  [HIVStatus] hs ON (hs.HIVStatusID = (SELECT max(hs2.HIVStatusID) FROM  [HIVStatus] hs2 WHERE (hs2.AdultID = a.AdultId)))
						--
						union all 
						--
						select 'child' as Type, c.ChildID as ID, c.child_guid As Guid, c.FirstName, c.LastName, c.DateOfBirth, c.Gender, 
						DATEDIFF(year, CAST(c.DateOfBirth As Date), GETDATE()) As Age, cs.Description As BeneficiaryState, csh.EffectiveDate As BeneficiaryStateEffectiveDate,
						c.HouseholdID, c.KinshipToFamilyHeadID, c.HIVStatusID, hs.HIVStatusID As LastHIVStatusID, c. HIVTracked, c.OVCTypeID,
						CASE WHEN c.RegistrationDate IS NULL THEN h.RegistrationDate ELSE c.RegistrationDate END As RegistrationDate
						from Child c
						inner join  [ChildStatusHistory] csh on (csh.ChildStatusHistoryID = (SELECT max(csh2.ChildStatusHistoryID) FROM  [ChildStatusHistory] csh2 WHERE csh2.ChildID = c.ChildID))
						inner join  [ChildStatus] cs on (cs.StatusID = csh.ChildStatusID)
						inner join  [Household] h on (c.HouseholdID = h.HouseHoldID)
						inner join  [HIVStatus] hs ON (hs.HIVStatusID = (SELECT max(hs2.HIVStatusID) FROM  [HIVStatus] hs2 WHERE (hs2.ChildID = c.CHildID)))
						) b", false);
        }
        
        public override void Down()
        {
        }
    }
}
