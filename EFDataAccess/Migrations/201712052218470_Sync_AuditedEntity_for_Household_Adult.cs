namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Sync_AuditedEntity_for_Household_Adult : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Adult", "SyncDate", c => c.DateTime());
            AddColumn("dbo.Adult", "SyncGuid", c => c.Guid());
            AddColumn("dbo.HouseHold", "SyncDate", c => c.DateTime());
            AddColumn("dbo.HouseHold", "SyncGuid", c => c.Guid());
            AlterColumn("dbo.Adult", "CreatedDate", c => c.DateTime());
            AlterColumn("dbo.Adult", "LastUpdatedDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Adult", "LastUpdatedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Adult", "CreatedDate", c => c.DateTime(nullable: false));
            DropColumn("dbo.HouseHold", "SyncGuid");
            DropColumn("dbo.HouseHold", "SyncDate");
            DropColumn("dbo.Adult", "SyncGuid");
            DropColumn("dbo.Adult", "SyncDate");
        }
    }
}
