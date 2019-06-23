namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCSIIDonCarePlan : DbMigration
    {
        public override void Up()
        {
            RenameIndex(table: "dbo.CarePlan", name: "IX_CSI_CSIID", newName: "IX_CSIID");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.CarePlan", name: "IX_CSIID", newName: "IX_CSI_CSIID");
        }
    }
}
