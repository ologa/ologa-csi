namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixChildStatusHistoryData : DbMigration
    {
        public override void Up()
        {
            // Fix ChildStatusHistory With Effective Date: 20-09-2019 to 2018 This was due to Inserted Eliminated Status
            Sql(@"UPDATE csh
                SET csh.EffectiveDate = '2018-09-20'
                FROM[ChildStatusHistory] csh
                where csh.EffectiveDate = '2019-09-20'");

            // Fix ChildStatusHistory From Child That Migrated to Adult and Merged the Status
            Sql(@"UPDATE csh
                set csh.BeneficiaryID = null
                FROM Beneficiary b
                JOIN ChildStatusHistory csh ON csh.BeneficiaryID = b.BeneficiaryID
                JOIN
                (
	                SELECT 
	                a.adultID,
	                a.FirstName, 
	                a.LastName, 
	                csh.ChildStatusHistoryID,
	                csh.ChildStatusID, 
	                csh.EffectiveDate 
	                FROM Adult a
	                JOIN ChildStatusHistory csh ON csh.AdultID = a.AdultID
	                JOIN
	                (
		                SELECT
			                a.AdultId, 
			                csh.ChildStatusHistoryID,
			                csh.ChildStatusID, 
			                csh.EffectiveDate 
		                FROM Child c
		                JOIN Adult a ON a.ChildID = c.ChildID
		                JOIN ChildStatusHistory csh ON csh.ChildID = c.ChildID
		                AND csh.BeneficiaryID is not null
	                ) child 
	                ON child.AdultId = a.AdultId 
	                AND child.EffectiveDate = csh.EffectiveDate 
	                AND child.ChildStatusID = csh.ChildStatusID
	                WHERE  csh.BeneficiaryID is not null
	                AND a.ChildID is not null
	                AND csh.AdultId is not null
                )adult
                ON adult.ChildStatusHistoryID = csh.ChildStatusHistoryID
                AND adult.AdultId = csh.AdultID");
 
        }
        
        public override void Down()
        {
        }
    }
}
