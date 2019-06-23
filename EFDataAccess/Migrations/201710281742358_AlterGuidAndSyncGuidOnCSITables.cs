namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AlterGuidAndSyncGuidOnCSITables : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CSI", "SyncGuid", c => c.Guid());
            AddColumn("dbo.CSIDomainScore", "SyncGuid", c => c.Guid());
            AddColumn("dbo.CSIDomain", "SyncGuid", c => c.Guid());
            AlterColumn("dbo.CSI", "csi_guid", c => c.Guid(nullable: false, defaultValueSql: "newid()"));
            AlterColumn("dbo.CSIDomainScore", "csidomainscore_guid", c => c.Guid(nullable: false, defaultValueSql: "newid()"));
            AlterColumn("dbo.CSIDomain", "csidomain_guid", c => c.Guid(nullable: false, defaultValueSql: "newid()"));
            DropColumn("dbo.CSI", "csi_sync_guid");
            Sql(@"update dbo.CSIDomain set csidomain_guid = newid()");
            Sql(@"update dbo.CSIDomainScore set csidomainscore_guid = newid()");
        }
        
        public override void Down()
        {
            AddColumn("dbo.CSI", "csi_sync_guid", c => c.Guid());
            AlterColumn("dbo.CSIDomain", "csidomain_guid", c => c.Guid(nullable: false));
            AlterColumn("dbo.CSIDomainScore", "csidomainscore_guid", c => c.Guid(nullable: false));
            AlterColumn("dbo.CSI", "csi_guid", c => c.Guid(nullable: false));
            DropColumn("dbo.CSIDomain", "SyncGuid");
            DropColumn("dbo.CSIDomainScore", "SyncGuid");
            DropColumn("dbo.CSI", "SyncGuid");
        }
    }
}
