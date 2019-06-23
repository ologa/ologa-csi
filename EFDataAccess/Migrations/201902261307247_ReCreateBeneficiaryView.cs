namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReCreateBeneficiaryView : DbMigration
    {
        public override void Up()
        {
            Sql(@"CREATE VIEW vw_beneficiary_details AS
                   select * from 
						( 
						select 'adult' as Type, a.AdultID as ID, a.Beneficiary_guid As Guid, a.FirstName, a.LastName, a.DateOfBirth, a.Gender,
						DATEDIFF(year, CAST(a.DateOfBirth As Date), GETDATE()) As Age, cs.Description As BeneficiaryState, csh.EffectiveDate As BeneficiaryStateEffectiveDate,
						a.HouseholdID, KinshipToFamilyHeadID, a.HIVStatusID, hs.HIVStatusID As LastHIVStatusID, HIVTracked, 0 As OVCTypeID,
						CASE WHEN a.RegistrationDate IS NULL THEN h.RegistrationDate ELSE a.RegistrationDate END As RegistrationDate
						from Beneficiary a					
						left join  [ChildStatusHistory] csh on (csh.ChildStatusHistoryID = (SELECT max(csh2.ChildStatusHistoryID) FROM  [ChildStatusHistory] csh2 WHERE csh2.AdultId = a.AdultId))
						left join  [ChildStatus] cs on (cs.StatusID = csh.ChildStatusID)
						left join  [Household] h on (a.HouseholdID = h.HouseHoldID)
						left join  [HIVStatus] hs ON (hs.HIVStatusID = (SELECT max(hs2.HIVStatusID) FROM  [HIVStatus] hs2 WHERE (hs2.AdultID = a.AdultId)))
						where DATEDIFF(year, CAST(a.DateOfBirth As Date), GETDATE()) > 17
						--
						union all 
						--
						select 'child' as Type, c.ChildID as ID, c.Beneficiary_guid As Guid, c.FirstName, c.LastName, c.DateOfBirth, c.Gender, 
						DATEDIFF(year, CAST(c.DateOfBirth As Date), GETDATE()) As Age, cs.Description As BeneficiaryState, csh.EffectiveDate As BeneficiaryStateEffectiveDate,
						c.HouseholdID, c.KinshipToFamilyHeadID, c.HIVStatusID, hs.HIVStatusID As LastHIVStatusID, c. HIVTracked, c.OVCTypeID,
						CASE WHEN c.RegistrationDate IS NULL THEN h.RegistrationDate ELSE c.RegistrationDate END As RegistrationDate
						from Beneficiary c
						left join  [ChildStatusHistory] csh on (csh.ChildStatusHistoryID = (SELECT max(csh2.ChildStatusHistoryID) FROM  [ChildStatusHistory] csh2 WHERE csh2.ChildID = c.ChildID))
						left join  [ChildStatus] cs on (cs.StatusID = csh.ChildStatusID)
						left join  [Household] h on (c.HouseholdID = h.HouseHoldID)
						left join  [HIVStatus] hs ON (hs.HIVStatusID = (SELECT max(hs2.HIVStatusID) FROM  [HIVStatus] hs2 WHERE (hs2.ChildID = c.CHildID)))
						where DATEDIFF(year, CAST(c.DateOfBirth As Date), GETDATE()) <= 17
						) b", false);
        }
        
        public override void Down()
        {
            Sql("DROP VIEW vw_beneficiary_details", false);
        }
    }
}
