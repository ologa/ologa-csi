namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCSISyncGUIDOnCSI : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CSI", "csi_sync_guid", c => c.Guid(nullable: true));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CSI", "csi_sync_guid");
        }
    }
}
