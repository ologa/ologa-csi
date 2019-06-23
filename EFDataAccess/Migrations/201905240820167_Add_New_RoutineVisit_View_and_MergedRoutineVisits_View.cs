namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_New_RoutineVisit_View_and_MergedRoutineVisits_View : DbMigration
    {
        public override void Up()
        {
            // VIEW TO CREATE NEW ROUTINEVISIT VIEW
            Sql(@"CREATE VIEW New_Routine_Visit_Summary AS SELECT  
                cp.Name AS ChiefPartner,
                p.Name AS Partner,
                CASE WHEN DATEDIFF(year, CAST(Ben.DateOfBirth AS Date),
                rv.RoutineVisitDate) BETWEEN 0 AND 17 THEN 'child' ELSE 'adult' END AS BeneficiaryType,
                Ben.FirstName AS BeneficiaryFirstName,
                Ben.LastName AS BeneficiaryLastName,
                Ben.Beneficiary_guid AS Guid,
                Ben.BeneficiaryID,
                DATEDIFF(year, CAST(Ben.DateOfBirth AS Date), GETDATE()) AS Age,
                rvs.RoutineVisitSupportID,
                Ben.DateOfBirth,
                Ben.HIVTracked AS HousehouldHIVTracked,
                CASE WHEN Ben.RegistrationDate IS NULL THEN hh.RegistrationDate ELSE Ben.RegistrationDate END AS RegistrationDate,
                CASE p.CollaboratorRoleID WHEN 3 THEN 'Supervior' WHEN 2 THEN 'Activista Chefe' ELSE 'Activista' END AS PartnerRole,
                CASE WHEN sst.[SupportServiceOrderInDomainTypeCode] = 'FE01' AND rvs.Checked = 1 THEN 1 ELSE 0 END AS FE01,
                CASE WHEN sst.[SupportServiceOrderInDomainTypeCode] = 'FE02' AND rvs.Checked = 1 THEN 1 ELSE 0 END AS FE02,
                CASE WHEN sst.[SupportServiceOrderInDomainTypeCode] = 'FE03' AND rvs.Checked = 1 THEN 1 ELSE 0 END AS FE03,
                CASE WHEN sst.[SupportServiceOrderInDomainTypeCode] = 'FE04' AND rvs.Checked = 1 THEN 1 ELSE 0 END AS FE04,
                CASE WHEN sst.[SupportServiceOrderInDomainTypeCode] = 'SD01' AND rvs.Checked = 1 THEN 1 ELSE 0 END AS SD01,
                CASE WHEN sst.[SupportServiceOrderInDomainTypeCode] = 'SD02' AND rvs.Checked = 1 THEN 1 ELSE 0 END AS SD02,
                CASE WHEN sst.[SupportServiceOrderInDomainTypeCode] = 'SD03' AND rvs.Checked = 1 THEN 1 ELSE 0 END AS SD03,
                CASE WHEN sst.[SupportServiceOrderInDomainTypeCode] = 'SD04' AND rvs.Checked = 1 THEN 1 ELSE 0 END AS SD04,
                CASE WHEN sst.[SupportServiceOrderInDomainTypeCode] = 'SD05' AND rvs.Checked = 1 THEN 1 ELSE 0 END AS SD05,
                CASE WHEN sst.[SupportServiceOrderInDomainTypeCode] = 'SD06' AND rvs.Checked = 1 THEN 1 ELSE 0 END AS SD06,
                CASE WHEN sst.[SupportServiceOrderInDomainTypeCode] = 'SD07' AND rvs.Checked = 1 THEN 1 ELSE 0 END AS SD07,
                CASE WHEN sst.[SupportServiceOrderInDomainTypeCode] = 'SD08' AND rvs.Checked = 1 THEN 1 ELSE 0 END AS SD08,
                CASE WHEN sst.[SupportServiceOrderInDomainTypeCode] = 'SD09' AND rvs.Checked = 1 THEN 1 ELSE 0 END AS SD09,
                CASE WHEN sst.[SupportServiceOrderInDomainTypeCode] = 'SD10' AND rvs.Checked = 1 THEN 1 ELSE 0 END AS SD10,
                CASE WHEN sst.[SupportServiceOrderInDomainTypeCode] = 'SD11' AND rvs.Checked = 1 THEN 1 ELSE 0 END AS SD11,
                CASE WHEN sst.[SupportServiceOrderInDomainTypeCode] = 'SD12' AND rvs.Checked = 1 THEN 1 ELSE 0 END AS SD12,
                cASE WHEN sst.[SupportServiceOrderInDomainTypeCode] = 'SD13' AND rvs.Checked = 1 THEN 1 ELSE 0 END AS SD13,
                CASE WHEN sst.[SupportServiceOrderInDomainTypeCode] = 'HIV01' AND rvs.Checked = 1 THEN 1 ELSE 0 END AS HIV01,
                CASE WHEN sst.[SupportServiceOrderInDomainTypeCode] = 'HIV02' AND rvs.Checked = 1 THEN 1 ELSE 0 END AS HIV02,
                CASE WHEN sst.[SupportServiceOrderInDomainTypeCode] = 'HIV03' AND rvs.Checked = 1 THEN 1 ELSE 0 END AS HIV03,
                CASE WHEN sst.[SupportServiceOrderInDomainTypeCode] = 'HIV04' AND rvs.Checked = 1 THEN 1 ELSE 0 END AS HIV04,
                CASE WHEN sst.[SupportServiceOrderInDomainTypeCode] = 'HIV05' AND rvs.Checked = 1 THEN 1 ELSE 0 END AS HIV05,
                CASE WHEN sst.[SupportServiceOrderInDomainTypeCode] = 'HIV06' AND rvs.Checked = 1 THEN 1 ELSE 0 END AS HIV06,
                CASE WHEN sst.[SupportServiceOrderInDomainTypeCode] = 'HIV07' AND rvs.Checked = 1 THEN 1 ELSE 0 END AS HIV07,
                CASE WHEN sst.[SupportServiceOrderInDomainTypeCode] = 'HAB01' AND rvs.Checked = 1 THEN 1 ELSE 0 END AS HAB01,
                CASE WHEN sst.[SupportServiceOrderInDomainTypeCode] = 'HAB02' AND rvs.Checked = 1 THEN 1 ELSE 0 END AS HAB02,
                CASE WHEN sst.[SupportServiceOrderInDomainTypeCode] = 'HAB03' AND rvs.Checked = 1 THEN 1 ELSE 0 END AS HAB03,
                CASE WHEN sst.[SupportServiceOrderInDomainTypeCode] = 'MUAC01' AND rvs.Checked = 1 THEN 1 ELSE 0 END AS MUAC01,
                CASE WHEN sst.[SupportServiceOrderInDomainTypeCode] = 'MUAC02' AND rvs.Checked = 1 THEN 1 ELSE 0 END AS MUAC02,
                CASE WHEN sst.[SupportServiceOrderInDomainTypeCode] = 'MUAC03' AND rvs.Checked = 1 THEN 1 ELSE 0 END AS MUAC03,
                CASE WHEN sst.[SupportServiceOrderInDomainTypeCode] = 'MUAC04' AND rvs.Checked = 1 THEN 1 ELSE 0 END AS MUAC04,
                CASE WHEN sst.[SupportServiceOrderInDomainTypeCode] = 'MUAC05' AND rvs.Checked = 1 THEN 1 ELSE 0 END AS MUAC05,
                CASE WHEN sst.[SupportServiceOrderInDomainTypeCode] = 'AN01' AND rvs.Checked = 1 THEN 1 ELSE 0 END AS AN01,
                CASE WHEN sst.[SupportServiceOrderInDomainTypeCode] = 'AN02' AND rvs.Checked = 1 THEN 1 ELSE 0 END AS AN02,
                CASE WHEN sst.[SupportServiceOrderInDomainTypeCode] = 'AN03' AND rvs.Checked = 1 THEN 1 ELSE 0 END AS AN03,
                CASE WHEN sst.[SupportServiceOrderInDomainTypeCode] = 'ED01' AND rvs.Checked = 1 THEN 1 ELSE 0 END AS ED01,
                CASE WHEN sst.[SupportServiceOrderInDomainTypeCode] = 'ED02' AND rvs.Checked = 1 THEN 1 ELSE 0 END AS ED02,
                CASE WHEN sst.[SupportServiceOrderInDomainTypeCode] = 'ED03' AND rvs.Checked = 1 THEN 1 ELSE 0 END AS ED03,
                CASE WHEN sst.[SupportServiceOrderInDomainTypeCode] = 'ED04' AND rvs.Checked = 1 THEN 1 ELSE 0 END AS ED04,
                CASE WHEN sst.[SupportServiceOrderInDomainTypeCode] = 'ED05' AND rvs.Checked = 1 THEN 1 ELSE 0 END AS ED05,
                CASE WHEN sst.[SupportServiceOrderInDomainTypeCode] = 'ED06' AND rvs.Checked = 1 THEN 1 ELSE 0 END AS ED06,
                CASE WHEN sst.[SupportServiceOrderInDomainTypeCode] = 'ED07' AND rvs.Checked = 1 THEN 1 ELSE 0 END AS ED07,
                CASE WHEN sst.[SupportServiceOrderInDomainTypeCode] = 'ED08' AND rvs.Checked = 1 THEN 1 ELSE 0 END AS ED08,
                CASE WHEN sst.[SupportServiceOrderInDomainTypeCode] = 'PAL01' AND rvs.Checked = 1 THEN 1 ELSE 0 END AS PAL01,
                CASE WHEN sst.[SupportServiceOrderInDomainTypeCode] = 'PAL02' AND rvs.Checked = 1 THEN 1 ELSE 0 END AS PAL02,
                CASE WHEN sst.[SupportServiceOrderInDomainTypeCode] = 'PAL03' AND rvs.Checked = 1 THEN 1 ELSE 0 END AS PAL03,
                cASE WHEN sst.[SupportServiceOrderInDomainTypeCode] = 'PAL04' AND rvs.Checked = 1 THEN 1 ELSE 0 END AS PAL04,
                CASE WHEN sst.[SupportServiceOrderInDomainTypeCode] = 'PAL05' AND rvs.Checked = 1 THEN 1 ELSE 0 END AS PAL05,
                CASE WHEN sst.[SupportServiceOrderInDomainTypeCode] = 'PAL06' AND rvs.Checked = 1 THEN 1 ELSE 0 END AS PAL06,
                CASE WHEN sst.[SupportServiceOrderInDomainTypeCode] = 'PAL07' AND rvs.Checked = 1 THEN 1 ELSE 0 END AS PAL07,
                CASE WHEN sst.[SupportServiceOrderInDomainTypeCode] = 'PAL08' AND rvs.Checked = 1 THEN 1 ELSE 0 END AS PAL08,
                CASE WHEN sst.[SupportServiceOrderInDomainTypeCode] = 'PAL09' AND rvs.Checked = 1 THEN 1 ELSE 0 END AS PAL09,
                CASE WHEN sst.[SupportServiceOrderInDomainTypeCode] = 'PAL10' AND rvs.Checked = 1 THEN 1 ELSE 0 END AS PAL10,
                CASE WHEN sst.[SupportServiceOrderInDomainTypeCode] = 'APS01' AND rvs.Checked = 1 THEN 1 ELSE 0 END AS APS01,
                CASE WHEN sst.[SupportServiceOrderInDomainTypeCode] = 'APS02' AND rvs.Checked = 1 THEN 1 ELSE 0 END AS APS02,
                CASE WHEN sst.[SupportServiceOrderInDomainTypeCode] = 'APS03' AND rvs.Checked = 1 THEN 1 ELSE 0 END AS APS03,
                CASE WHEN sst.[SupportServiceOrderInDomainTypeCode] = 'DPI01' AND rvs.Checked = 1 THEN 1 ELSE 0 END AS DPI01,
                CASE WHEN sst.[SupportServiceOrderInDomainTypeCode] = 'DPI02' AND rvs.Checked = 1 THEN 1 ELSE 0 END AS DPI02,
                CASE WHEN sst.[SupportServiceOrderInDomainTypeCode] = 'DPI03' AND rvs.Checked = 1 THEN 1 ELSE 0 END AS DPI03,
                CASE WHEN sst.[SupportServiceOrderInDomainTypeCode] = 'DPI04' AND rvs.Checked = 1 THEN 1 ELSE 0 END AS DPI04,
                CASE WHEN sst.[SupportServiceOrderInDomainTypeCode] = 'OTR01' AND rvs.Checked = 1 THEN 1 ELSE 0 END AS OTR01,
                CASE WHEN sst.[SupportServiceOrderInDomainTypeCode] = 'OTR02' AND rvs.Checked = 1 THEN 1 ELSE 0 END AS OTR02,
                CASE WHEN sst.[SupportServiceOrderInDomainTypeCode] = 'OTR03' AND rvs.Checked = 1 THEN 1 ELSE 0 END AS OTR03,
                rv.RoutineVisitDate,
                rvm.BeneficiaryHasServices
            FROM Partner AS p
            INNER JOIN Partner AS cp ON p.SuperiorId = cp.PartnerID
            INNER JOIN HouseHold hh ON hh.PartnerID = p.PartnerID
            INNER JOIN Beneficiary ben on ben.HouseholdID = hh.HouseHoldID
            INNER JOIN RoutineVisitMember rvm on rvm.BeneficiaryID = ben.BeneficiaryID
            INNER JOIN RoutineVisit rv on rv.RoutineVisitID = rvm.RoutineVisitID AND rv.Version = 'v2'
            INNER JOIN RoutineVisitSupport rvs on rvs.RoutineVisitMemberID = rvm.RoutineVisitMemberID
            INNER JOIN SupportServiceType sst on sst.SupportServiceTypeID = rvs.SupportID AND sst.Tool = 'routine-visit'");


            // VIEW TO CREATE MERGED ROUTINEVISITS (ONLY MAIN DOMAINS FROM OLD AND NEW ROUTINE VISIT)
            Sql(@"CREATE VIEW Merged_Routine_Visit_Summary AS SELECT  
                    cp.Name AS ChiefPartner,
                    p.Name AS Partner,
                    CASE WHEN   DATEDIFF(year, CAST(Ben.DateOfBirth AS Date), rv.RoutineVisitDate) BETWEEN 0 AND   17 THEN 'child' ELSE 'adult'  end AS BeneficiaryType,
                    Ben.FirstName AS BeneficiaryFirstName,
                    Ben.LastName AS BeneficiaryLastName,
                    Ben.Beneficiary_guid AS Guid,
                    Ben.BeneficiaryID,
                    DATEDIFF(year, CAST(Ben.DateOfBirth AS Date),
                    GETDATE()) AS Age,
                    rvs.RoutineVisitSupportID,
                    Ben.DateOfBirth,
                    Ben.HIVTracked AS HousehouldHIVTracked,
                    CASE WHEN Ben.RegistrationDate IS NULL THEN hh.RegistrationDate ELSE Ben.RegistrationDate END AS RegistrationDate,
                    CASE p.CollaboratorRoleID WHEN 3 THEN 'Supervior' WHEN 2 THEN 'Activista Chefe' ELSE 'Activista' END AS PartnerRole,
                    rv.Version,
                    CASE WHEN rvs.SupportType IN('Domain', 'DomainEntity') AND rvs.SupportID = 6 THEN rvs.SupportValue ELSE 0 END AS FinaceAid,
                    CASE WHEN rvs.SupportType IN('Domain', 'DomainEntity') AND rvs.SupportID = 3 THEN rvs.SupportValue ELSE 0 END AS Health,
                    CASE WHEN rvs.SupportType IN('Domain', 'DomainEntity') AND rvs.SupportID = 7 THEN rvs.SupportValue ELSE 0 END AS Housing,
                    CASE WHEN rvs.SupportType = 'MUAC' AND rvs.SupportID = 1 THEN rvs.SupportValue ELSE 0 END AS MUACGreen,
                    CASE WHEN rvs.SupportType = 'MUAC' AND rvs.SupportID = 2 THEN rvs.SupportValue ELSE 0 END AS MUACYellow,
                    CASE WHEN rvs.SupportType = 'MUAC' AND rvs.SupportID = 3 THEN rvs.SupportValue ELSE 0 END AS MUACRed,
                    CASE WHEN rvs.SupportType IN('Domain', 'DomainEntity') AND rvs.SupportID = 1 THEN rvs.SupportValue ELSE 0 END AS Food,
                    CASE WHEN rvs.SupportType IN('Domain', 'DomainEntity') AND rvs.SupportID = 2 THEN rvs.SupportValue ELSE 0 END AS Education,
                    CASE WHEN rvs.SupportType IN('Domain', 'DomainEntity') AND rvs.SupportID = 4 THEN rvs.SupportValue ELSE 0 END AS LegalAdvice,
                    CASE WHEN rvs.SupportType IN('Domain', 'DomainEntity') AND rvs.SupportID = 5 THEN rvs.SupportValue ELSE 0 END AS SocialAid,
                    CASE WHEN rvs.SupportType = 'DPI' AND rvs.SupportID = 1 THEN  rvs.SupportValue ELSE 0 END AS DPI,
                    CASE WHEN rvs.SupportType = 'HIVRISK' AND rvs.SupportID = 1 AND rvs.SupportValue = 'True' THEN 1 ELSE 0 END AS RoutineVisitHIVRiskTracked,
                    rv.RoutineVisitDate,
                    rvm.BeneficiaryHasServices
                FROM dbo.Partner AS p INNER JOIN
                dbo.Partner AS cp ON p.SuperiorId = cp.PartnerID
                INNER JOIN dbo.HouseHold AS hh ON p.PartnerID = hh.PartnerID
                INNER JOIN dbo.RoutineVisit AS rv ON hh.HouseHoldID = rv.HouseholdID  AND rv.Version = 'v1'
                INNER JOIN dbo.RoutineVisitMember AS rvm ON rvm.RoutineVisitID = rv.RoutineVisitID
                INNER JOIN dbo.RoutineVisitSupport AS rvs ON rvs.RoutineVisitMemberID = rvm.RoutineVisitMemberID
                INNER JOIN dbo.Beneficiary AS Ben ON(Ben.BeneficiaryID = rvm.BeneficiaryID)

                UNION ALL

                SELECT
                    cp.Name AS ChiefPartner,
                    p.Name AS Partner,
                    CASE WHEN DATEDIFF(year, CAST(Ben.DateOfBirth AS Date),
                    rv.RoutineVisitDate) BETWEEN 0 AND 17 THEN 'child' ELSE 'adult' END AS BeneficiaryType,
                    Ben.FirstName AS BeneficiaryFirstName,
                    Ben.LastName AS BeneficiaryLastName,
                    Ben.Beneficiary_guid AS Guid,
                    Ben.BeneficiaryID,
                    DATEDIFF(year, CAST(Ben.DateOfBirth AS Date), GETDATE()) AS Age,
                    rvs.RoutineVisitSupportID,
                    Ben.DateOfBirth,
                    Ben.HIVTracked AS HousehouldHIVTracked,
                    CASE WHEN Ben.RegistrationDate IS NULL THEN hh.RegistrationDate ELSE Ben.RegistrationDate END AS RegistrationDate,
                    CASE p.CollaboratorRoleID WHEN 3 THEN 'Supervior' WHEN 2 THEN 'Activista Chefe' ELSE 'Activista' END AS PartnerRole,
                    rv.Version,
                    CASE WHEN sst.TypeCode = 'FE' AND rvs.Checked = 1 THEN 1 ELSE 0 END AS FinaceAid,
                    CASE WHEN sst.TypeCode = 'SD' AND rvs.Checked = 1 THEN 1 ELSE 0 END AS Health,
                    CASE WHEN sst.TypeCode = 'HAB' AND rvs.Checked = 1 THEN 1 ELSE 0 END AS Housing,
                    CASE WHEN sst.TypeCode = 'MUAC' AND sst.[SupportServiceOrderInDomainTypeCode] = 'MUAC01' AND rvs.Checked = 1 THEN 1 ELSE 0 END AS MUACGreen,
                    CASE WHEN sst.TypeCode = 'MUAC' AND sst.[SupportServiceOrderInDomainTypeCode] = 'MUAC02' AND rvs.Checked = 1 THEN 1 ELSE 0 END AS MUACYellow,
                    CASE WHEN sst.TypeCode = 'MUAC' AND sst.[SupportServiceOrderInDomainTypeCode] = 'MUAC03' AND rvs.Checked = 1 THEN 1 ELSE 0 END AS MUACRed,
                    CASE WHEN sst.TypeCode = 'AN' AND rvs.Checked = 1 THEN 1 ELSE 0 END AS Food,
                    CASE WHEN sst.TypeCode = 'ED' AND rvs.Checked = 1 THEN 1 ELSE 0 END AS Education,
                    CASE WHEN sst.TypeCode = 'PAL' AND rvs.Checked = 1 THEN 1 ELSE 0 END AS LegalAdvice,
                    CASE WHEN sst.TypeCode = 'APS' AND rvs.Checked = 1 THEN 1 ELSE 0 END AS SocialAid,
                    CASE WHEN sst.TypeCode = 'DPI' AND rvs.Checked = 1 THEN 1 ELSE 0 END AS DPI,
                    CASE WHEN sst.TypeCode = 'HIV' AND sst.[SupportServiceOrderInDomainTypeCode] IN('HIV01', 'HIV02') AND rvs.Checked = 1 THEN 1 ELSE 0 END AS RoutineVisitHIVRiskTracked,
                    rv.RoutineVisitDate,
                    rvm.BeneficiaryHasServices
                FROM Partner AS p
                INNER JOIN Partner AS cp ON p.SuperiorId = cp.PartnerID
                INNER JOIN HouseHold hh ON hh.PartnerID = p.PartnerID
                INNER JOIN Beneficiary ben on ben.HouseholdID = hh.HouseHoldID
                INNER JOIN RoutineVisitMember rvm on rvm.BeneficiaryID = ben.BeneficiaryID
                INNER JOIN RoutineVisit rv on rv.RoutineVisitID = rvm.RoutineVisitID AND rv.Version = 'v2'
                INNER JOIN RoutineVisitSupport rvs on rvs.RoutineVisitMemberID = rvm.RoutineVisitMemberID
                INNER JOIN SupportServiceType sst on sst.SupportServiceTypeID = rvs.SupportID AND sst.Tool = 'routine-visit'");

        }
        
        public override void Down()
        {
        }
    }
}
