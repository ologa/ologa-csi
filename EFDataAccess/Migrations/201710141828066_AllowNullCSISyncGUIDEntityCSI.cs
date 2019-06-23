namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AllowNullCSISyncGUIDEntityCSI : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.CSI", "csi_sync_guid", c => c.Guid());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.CSI", "csi_sync_guid", c => c.Guid(nullable: false));
        }
    }
}
