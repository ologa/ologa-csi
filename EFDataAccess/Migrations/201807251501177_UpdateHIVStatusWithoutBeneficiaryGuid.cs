namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateHIVStatusWithoutBeneficiaryGuid : DbMigration
    {
        public override void Up()
        {
            Sql(@"UPDATE h
                SET h.BeneficiaryGuid = c.child_guid
                FROM  [HIVStatus] h
                JOIN  [Child] c ON c.ChildID = h.ChildID
                where h.BeneficiaryGuid = '00000000-0000-0000-0000-000000000000'", false);


            Sql(@"UPDATE h
                SET h.BeneficiaryGuid = a.AdultGuid
                FROM  [HIVStatus] h
                JOIN  [Adult] a ON a.AdultID = h.AdultID
                where h.BeneficiaryGuid = '00000000-0000-0000-0000-000000000000'", false);
        }
        
        public override void Down()
        {
        }
    }
}
