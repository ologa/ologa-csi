namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class HIVStatusAnd_BeneficiariesStatusView : DbMigration
    {
        public override void Up()
        {
            // HIV Status View
            Sql(@"CREATE VIEW HIVStatusView AS 
                SELECT
	                cp.Name AS ChiefPartner
	                ,p.[Name] AS Partner
	                ,hh.HouseholdName AS HouseholdName
	                ,ben.RegistrationDate AS RegistrationDate
	                ,ben.BeneficiaryID
	                ,ben.FirstName AS FirstName
	                ,ben.LastName AS LastName
	                ,ben.Gender AS Gender
	                ,ben.DateOfBirth
	                ,hiv.CreatedAt
	                ,hiv.InformationDate
	                ,hiv.HIV
	                ,hiv.HIVInTreatment
	                ,hiv.HIVUndisclosedReason
	                ,ROW_NUMBER() OVER (PARTITION BY hiv.BeneficiaryID ORDER BY hiv.InformationDate DESC) AS LastHIVStatus
	                ,ROW_NUMBER() OVER (PARTITION BY hiv.BeneficiaryID ORDER BY hiv.InformationDate ASC) AS FirstHIVStatus
                FROM [Partner] p
                inner join [Partner] cp on (p.SuperiorId = cp.PartnerID) 
                inner join [HouseHold] hh on (p.PartnerID = hh.PartnerID)
                inner join [Beneficiary] ben ON ben.HouseholdID = hh.HouseHoldID
                inner join [HIVStatus] hiv ON hiv.BeneficiaryID = ben.BeneficiaryID");


            // Beneficiary Status View
            Sql(@"CREATE VIEW BeneficiaryStatusView AS 
                SELECT
	                cp.Name AS ChiefPartner
	                ,p.[Name] AS Partner
	                ,hh.HouseholdName AS HouseholdName
	                ,ben.RegistrationDate AS RegistrationDate
	                ,ben.BeneficiaryID
	                ,ben.FirstName AS FirstName
	                ,ben.LastName AS LastName
	                ,ben.Gender AS Gender
	                ,ben.DateOfBirth
	                ,csh.CreatedDate
	                ,csh.EffectiveDate
	                ,csh.ChildStatusID
	                ,cs.Description
	                ,ROW_NUMBER() OVER (PARTITION BY csh.BeneficiaryID ORDER BY csh.EffectiveDate DESC) AS LastBeneficiaryStatus
	                ,ROW_NUMBER() OVER (PARTITION BY csh.BeneficiaryID ORDER BY csh.EffectiveDate ASC) AS FirstBeneficiaryStatus
                FROM [Partner] p
                inner join [Partner] cp on (p.SuperiorId = cp.PartnerID) 
                inner join [HouseHold] hh on (p.PartnerID = hh.PartnerID)
                inner join [Beneficiary] ben ON ben.HouseholdID = hh.HouseHoldID
                inner join [ChildStatusHistory] csh ON csh.BeneficiaryID = ben.BeneficiaryID
                inner join [ChildStatus] cs ON cs.StatusID = csh.ChildStatusID");
        }
        
        public override void Down()
        {
        }
    }
}
