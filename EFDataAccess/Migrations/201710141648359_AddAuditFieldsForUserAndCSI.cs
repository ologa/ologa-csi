namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAuditFieldsForUserAndCSI : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.User", "CreatedDate", c => c.DateTime(nullable: true));
            AddColumn("dbo.User", "LastUpdatedDate", c => c.DateTime(nullable: true));
            AddColumn("dbo.User", "SyncDate", c => c.DateTime(nullable: true));
            AddColumn("dbo.CSI", "SyncDate", c => c.DateTime(nullable: true));
        }
        
        public override void Down()
        {
            DropColumn("dbo.CSI", "SyncDate");
            DropColumn("dbo.User", "SyncDate");
            DropColumn("dbo.User", "LastUpdatedDate");
            DropColumn("dbo.User", "CreatedDate");
        }
    }
}
