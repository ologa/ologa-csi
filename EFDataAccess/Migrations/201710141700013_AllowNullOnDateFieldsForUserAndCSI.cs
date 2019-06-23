namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AllowNullOnDateFieldsForUserAndCSI : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.User", "CreatedDate", c => c.DateTime());
            AlterColumn("dbo.User", "LastUpdatedDate", c => c.DateTime());
            AlterColumn("dbo.User", "SyncDate", c => c.DateTime());
            AlterColumn("dbo.CSI", "CreatedDate", c => c.DateTime());
            AlterColumn("dbo.CSI", "LastUpdatedDate", c => c.DateTime());
            AlterColumn("dbo.CSI", "SyncDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.CSI", "SyncDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.CSI", "LastUpdatedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.CSI", "CreatedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.User", "SyncDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.User", "LastUpdatedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.User", "CreatedDate", c => c.DateTime(nullable: false));
        }
    }
}
