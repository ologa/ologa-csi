namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RecreateSystemViewsForNewAproach : DbMigration
    {
        public override void Up()
        {
            Sql("DROP VIEW Routine_Visit_Summary", false);
            Sql("DROP VIEW vw_beneficiary_details", false);

            Sql(@"CREATE VIEW Routine_Visit_Summary AS SELECT  cp.Name AS ChiefPartner, p.Name AS Partner, 
                CASE WHEN   DATEDIFF(year,CAST(Ben.DateOfBirth AS Date), rv.RoutineVisitDate) BETWEEN 0 AND   17 THEN 'child' ELSE 'adult'  end AS BeneficiaryType, 
                Ben.FirstName AS BeneficiaryFirstName, Ben.LastName AS BeneficiaryLastName, Ben.Beneficiary_guid AS Guid, Ben.BeneficiaryID,
                DATEDIFF(year,CAST(Ben.DateOfBirth AS Date), GETDATE()) AS Age, rvs.RoutineVisitSupportID, Ben.DateOfBirth, Ben.HIVTracked AS HousehouldHIVTracked, 
                CASE WHEN Ben.RegistrationDate IS NULL THEN hh.RegistrationDate ELSE Ben.RegistrationDate END AS RegistrationDate, 
                CASE p.CollaboratorRoleID WHEN 3 THEN 'Supervior' WHEN 2 THEN 'Activista Chefe' ELSE 'Activista' END AS PartnerRole, 
                CASE WHEN rvs.SupportType IN ('Domain', 'DomainEntity') AND rvs.SupportID = 6 THEN rvs.SupportValue ELSE 0 END AS FinaceAid,
                CASE WHEN rvs.SupportType IN ('Domain', 'DomainEntity') AND rvs.SupportID = 3 THEN rvs.SupportValue ELSE 0 END AS Health, 
                CASE WHEN rvs.SupportType IN ('Domain', 'DomainEntity') AND rvs.SupportID = 1 THEN rvs.SupportValue ELSE 0 END AS Food, 
                CASE WHEN rvs.SupportType IN ('Domain', 'DomainEntity') AND rvs.SupportID = 2 THEN rvs.SupportValue ELSE 0 END AS Education, 
                CASE WHEN rvs.SupportType IN ('Domain', 'DomainEntity') AND rvs.SupportID = 4 THEN rvs.SupportValue ELSE 0 END AS LegalAdvice, 
                CASE WHEN rvs.SupportType IN ('Domain', 'DomainEntity') AND rvs.SupportID = 7 THEN rvs.SupportValue ELSE 0 END AS Housing, 
                CASE WHEN rvs.SupportType IN ('Domain', 'DomainEntity') AND rvs.SupportID = 5 THEN rvs.SupportValue ELSE 0 END AS SocialAid, 
                CASE WHEN rvs.SupportType = 'DPI' AND rvs.SupportID = 1 THEN  rvs.SupportValue ELSE 0 END AS DPI, 
                CASE WHEN rvs.SupportType = 'HIVRISK' AND rvs.SupportID = 1 AND rvs.SupportValue = 'True' THEN 1 ELSE 0 END AS RoutineVisitHIVRiskTracked, 
                CASE WHEN rvs.SupportType = 'MUAC' AND rvs.SupportID = 1 THEN rvs.SupportValue ELSE 0 END AS MUACGreen, 
                CASE WHEN rvs.SupportType = 'MUAC' AND rvs.SupportID = 2 THEN rvs.SupportValue ELSE 0 END AS MUACYellow, 
                CASE WHEN rvs.SupportType = 'MUAC' AND rvs.SupportID = 3 THEN rvs.SupportValue ELSE 0 END AS MUACRed, 
                CASE WHEN rv.FamilyKitReceived = 'True' THEN 1 ELSE 0 END AS FamilyKitReceived,
                rv.RoutineVisitDate, rvm.BeneficiaryHasServices
                FROM dbo.Partner AS p INNER JOIN
                dbo.Partner AS cp ON p.SuperiorId = cp.PartnerID INNER JOIN
                dbo.HouseHold AS hh ON p.PartnerID = hh.PartnerID INNER JOIN
                dbo.RoutineVisit AS rv ON hh.HouseHoldID = rv.HouseholdID INNER JOIN
                dbo.RoutineVisitMember AS rvm ON rvm.RoutineVisitID = rv.RoutineVisitID INNER JOIN
                dbo.RoutineVisitSupport AS rvs ON rvs.RoutineVisitMemberID = rvm.RoutineVisitMemberID INNER JOIN
                dbo.Beneficiary AS Ben ON (Ben.BeneficiaryID = rvm.BeneficiaryID)
                ");

            Sql(@"CREATE VIEW vw_beneficiary_details AS SELECT case when (DATEDIFF(year, CAST(a.DateOfBirth AS Date), GETDATE()) > 17) then 'adult' else 'child' end AS Type, 
                a.BeneficiaryID AS ID, a.Beneficiary_guid AS Guid, a.FirstName, a.LastName, a.DateOfBirth, a.Gender, DATEDIFF(year, CAST(a.DateOfBirth AS Date), GETDATE()) AS Age, 
                cs.Description AS BeneficiaryState, csh.EffectiveDate AS BeneficiaryStateEffectiveDate, a.HouseholdID, a.KinshipToFamilyHeadID, 
                a.HIVStatusID, hs.HIVStatusID AS LastHIVStatusID, a.HIVTracked, 0 AS OVCTypeID,
                CASE WHEN a.RegistrationDate IS NULL THEN h.RegistrationDate ELSE a.RegistrationDate END AS RegistrationDate
                FROM dbo.Beneficiary AS a 
                INNER JOIN dbo.ChildStatusHistory AS csh ON csh.ChildStatusHistoryID =
                        (SELECT MAX(ChildStatusHistoryID) AS Expr1
                        FROM      dbo.ChildStatusHistory AS csh2
                        WHERE   (BeneficiaryID = a.BeneficiaryID)) 
                INNER JOIN dbo.ChildStatus AS cs ON cs.StatusID = csh.ChildStatusID 
                INNER JOIN dbo.HouseHold AS h ON a.HouseholdID = h.HouseHoldID 
                INNER JOIN dbo.HIVStatus AS hs ON hs.HIVStatusID =
                        (SELECT MAX(HIVStatusID) AS Expr1
                        FROM      dbo.HIVStatus AS hs2
                        WHERE   (BeneficiaryID = a.BeneficiaryID))");
        }
        
        public override void Down()
        {
        }
    }
}
