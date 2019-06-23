namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update_Existing_Beneficiary_in_StatusHistory_without_Correct_BeneficiaryGuid : DbMigration
    {
        public override void Up()
        {
            Sql(@"UPDATE csh
                SET csh.BeneficiaryGuid = c.child_guid
                FROM  [ChildStatusHistory] csh
                JOIN  [Child] c ON c.ChildID = csh.ChildID
                where csh.BeneficiaryGuid = '00000000-0000-0000-0000-000000000000'", false);


            Sql(@"UPDATE csh
                SET csh.BeneficiaryGuid = a.AdultGuid
                FROM  [ChildStatusHistory] csh
                JOIN  [Adult] a ON a.AdultID = csh.AdultID
                where csh.BeneficiaryGuid = '00000000-0000-0000-0000-000000000000'", false);

        }
        
        public override void Down()
        {
        }
    }
}
