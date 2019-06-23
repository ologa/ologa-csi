namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIsPartSavingGroupOnBeneficiaryView : DbMigration
    {
        public override void Up()
        {
            Sql("DROP VIEW Beneficiary");

            Sql(@"CREATE VIEW Beneficiary as 
						select * from 
						( 
						select 'adult' as Type, a.AdultID as ID, a.AdultGuid As Guid, a.FirstName, a.LastName, a.DateOfBirth, a.Gender,
						DATEDIFF(year, CAST(a.DateOfBirth As Date), GETDATE()) As Age, cs.Description As BeneficiaryState, csh.EffectiveDate As BeneficiaryStateEffectiveDate,
						a.HouseholdID, KinshipToFamilyHeadID, a.HIVStatusID, hs.HIVStatusID As LastHIVStatusID, HIVTracked, 0 As OVCTypeID, a.IsPartSavingGroup,
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
						c.HouseholdID, c.KinshipToFamilyHeadID, c.HIVStatusID, hs.HIVStatusID As LastHIVStatusID, c. HIVTracked, c.OVCTypeID, 0 As IsPartSavingGroup,
						CASE WHEN c.RegistrationDate IS NULL THEN h.RegistrationDate ELSE c.RegistrationDate END As RegistrationDate
						from Child c
						inner join  [ChildStatusHistory] csh on (csh.ChildStatusHistoryID = (SELECT max(csh2.ChildStatusHistoryID) FROM  [ChildStatusHistory] csh2 WHERE csh2.ChildID = c.ChildID))
						inner join  [ChildStatus] cs on (cs.StatusID = csh.ChildStatusID)
						inner join  [Household] h on (c.HouseholdID = h.HouseHoldID)
						inner join  [HIVStatus] hs ON (hs.HIVStatusID = (SELECT max(hs2.HIVStatusID) FROM  [HIVStatus] hs2 WHERE (hs2.ChildID = c.CHildID)))
						) b");

        }
        
        public override void Down()
        {
        }
    }
}
