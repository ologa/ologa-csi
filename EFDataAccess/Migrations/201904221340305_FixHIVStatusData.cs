namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixHIVStatusData : DbMigration
    {
        public override void Up()
        {
            // Fix HIVStatus From Child That Migrated to Adult and Merged the Status
            Sql(@"UPDATE hiv
                set HIV.BeneficiaryID = 0
                FROM Beneficiary b
                JOIN [HIVStatus] hiv ON hiv.BeneficiaryID = b.BeneficiaryID
                JOIN
                (
	                SELECT 
	                a.adultID,
	                a.FirstName, 
	                a.LastName, 
	                hiv.HIVStatusID,
	                hiv.HIV, 
	                hiv.HIVInTreatment, 
	                hiv.HIVUndisclosedReason
	                FROM Adult a
	                JOIN [HIVStatus] hiv ON hiv.AdultID = a.AdultID
	                JOIN
	                (
		                SELECT
			                a.AdultId, 
			                hiv.HIV,
			                hiv.HIVInTreatment,
			                hiv.HIVUndisclosedReason,
			                hiv.InformationDate
		                FROM Child c
		                JOIN Adult a ON a.ChildID = c.ChildID
		                JOIN [HIVStatus] hiv ON hiv.ChildID = c.ChildID
		                AND hiv.BeneficiaryID <> 0
	                ) child 
	                ON child.AdultId = a.AdultId 
	                AND child.InformationDate = hiv.InformationDate 
	                AND child.HIVInTreatment = hiv.HIVInTreatment
	                AND child.HIVUndisclosedReason = hiv.HIVUndisclosedReason
	                AND child.InformationDate = hiv.InformationDate
	                WHERE hiv.BeneficiaryID <> 0
	                AND a.ChildID <> 0
	                AND hiv.AdultId <> 0
                )adult
                ON adult.HIVStatusID = hiv.HIVStatusID
                AND adult.AdultId = hiv.AdultID");
        }
        
        public override void Down()
        {
        }
    }
}
