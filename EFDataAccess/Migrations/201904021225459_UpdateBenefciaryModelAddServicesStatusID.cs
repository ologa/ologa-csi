namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateBenefciaryModelAddServicesStatusID : DbMigration
    {
        public override void Up()
        {
            RenameIndex(table: "dbo.Beneficiary", name: "IX_ServicesStatus_SimpleEntityID", newName: "IX_ServicesStatusID");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Beneficiary", name: "IX_ServicesStatusID", newName: "IX_ServicesStatus_SimpleEntityID");
        }
    }
}
