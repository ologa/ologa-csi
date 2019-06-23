namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CreateRoutineVisitSummaryViews : DbMigration
    {
        public override void Up()
        {
            Sql(@"CREATE VIEW Children_Routine_Visit_Summary
						AS
						SELECT
						cp.Name As ChiefPartner,
						p.[Name] As [Partner],
						rvm.ChildID,
						rvs.RoutineVisitSupportID,
						PartnerRole = CASE p.CollaboratorRoleID WHEN 3 THEN 'Supervior' WHEN 2 THEN 'Activista Chefe' ELSE 'Activista' END,
						FinaceAid = CASE WHEN rvs.SupportType = 'Domain' AND rvs.SupportID = 6 THEN rvs.SupportValue ELSE 0 END,
						Health =  CASE WHEN rvs.SupportType = 'Domain' AND rvs.SupportID = 3 THEN rvs.SupportValue ELSE 0 END,
						Food =  CASE WHEN rvs.SupportType = 'Domain' AND rvs.SupportID = 1 THEN rvs.SupportValue ELSE 0 END,
						Education =  CASE WHEN rvs.SupportType = 'Domain' AND rvs.SupportID = 2 THEN rvs.SupportValue ELSE 0 END,
						LegalAdvice =  CASE WHEN rvs.SupportType = 'Domain' AND rvs.SupportID = 4 THEN rvs.SupportValue ELSE 0 END,
						Housing =  CASE WHEN rvs.SupportType = 'Domain' AND rvs.SupportID = 7 THEN rvs.SupportValue ELSE 0 END,
						SocialAid =  CASE WHEN rvs.SupportType = 'Domain' AND rvs.SupportID = 5 THEN rvs.SupportValue ELSE 0 END,
						DPI =  CASE WHEN rvs.SupportType = 'DPI' AND rvs.SupportID = 1 THEN rvs.SupportValue ELSE 0 END,
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
						Where p.CollaboratorRoleID = 1 AND rvm.ChildID IS NOT NULL;", false);


            Sql(@"CREATE VIEW Adults_Routine_Visit_Summary
						AS
						SELECT
						cp.Name As ChiefPartner,
						p.[Name] As [Partner],
						rvm.AdultID,
						rvs.RoutineVisitSupportID,
						PartnerRole = CASE p.CollaboratorRoleID WHEN 3 THEN 'Supervior' WHEN 2 THEN 'Activista Chefe' ELSE 'Activista' END,
						FinaceAid = CASE WHEN rvs.SupportType = 'Domain' AND rvs.SupportID = 6 THEN rvs.SupportValue ELSE 0 END,
						Health =  CASE WHEN rvs.SupportType = 'Domain' AND rvs.SupportID = 3 THEN rvs.SupportValue ELSE 0 END,
						Food =  CASE WHEN rvs.SupportType = 'Domain' AND rvs.SupportID = 1 THEN rvs.SupportValue ELSE 0 END,
						Education =  CASE WHEN rvs.SupportType = 'Domain' AND rvs.SupportID = 2 THEN rvs.SupportValue ELSE 0 END,
						LegalAdvice =  CASE WHEN rvs.SupportType = 'Domain' AND rvs.SupportID = 4 THEN rvs.SupportValue ELSE 0 END,
						Housing =  CASE WHEN rvs.SupportType = 'Domain' AND rvs.SupportID = 7 THEN rvs.SupportValue ELSE 0 END,
						SocialAid =  CASE WHEN rvs.SupportType = 'Domain' AND rvs.SupportID = 5 THEN rvs.SupportValue ELSE 0 END,
						DPI =  CASE WHEN rvs.SupportType = 'DPI' AND rvs.SupportID = 1 THEN rvs.SupportValue ELSE 0 END,
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
						Where p.CollaboratorRoleID = 1 AND rvm.AdultID IS NOT NULL", false);

            Sql(@"CREATE VIEW Routine_Visit_Summary
						AS
						SELECT
						cp.Name As ChiefPartner,
						p.[Name] As [Partner],
						rvm.ChildID,
						rvm.AdultID,
						rvs.RoutineVisitSupportID,
						PartnerRole = CASE p.CollaboratorRoleID WHEN 3 THEN 'Supervior' WHEN 2 THEN 'Activista Chefe' ELSE 'Activista' END,
						FinaceAid = CASE WHEN rvs.SupportType = 'Domain' AND rvs.SupportID = 6 THEN rvs.SupportValue ELSE 0 END,
						Health =  CASE WHEN rvs.SupportType = 'Domain' AND rvs.SupportID = 3 THEN rvs.SupportValue ELSE 0 END,
						Food =  CASE WHEN rvs.SupportType = 'Domain' AND rvs.SupportID = 1 THEN rvs.SupportValue ELSE 0 END,
						Education =  CASE WHEN rvs.SupportType = 'Domain' AND rvs.SupportID = 2 THEN rvs.SupportValue ELSE 0 END,
						LegalAdvice =  CASE WHEN rvs.SupportType = 'Domain' AND rvs.SupportID = 4 THEN rvs.SupportValue ELSE 0 END,
						Housing =  CASE WHEN rvs.SupportType = 'Domain' AND rvs.SupportID = 7 THEN rvs.SupportValue ELSE 0 END,
						SocialAid =  CASE WHEN rvs.SupportType = 'Domain' AND rvs.SupportID = 5 THEN rvs.SupportValue ELSE 0 END,
						DPI =  CASE WHEN rvs.SupportType = 'DPI' AND rvs.SupportID = 1 THEN rvs.SupportValue ELSE 0 END,
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
						Where p.CollaboratorRoleID = 1", false);
        }

        public override void Down()
        {
        }
    }
}
