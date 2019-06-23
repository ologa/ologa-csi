namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Sync_AuditedEntityForAid : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Aid", "SyncDate", c => c.DateTime());
            AddColumn("dbo.Aid", "SyncGuid", c => c.Guid());
            AddColumn("dbo.Aid", "CreatedDate", c => c.DateTime());
            AddColumn("dbo.Aid", "LastUpdatedDate", c => c.DateTime());
            AddColumn("dbo.Aid", "CreatedUserID", c => c.Int());
            AddColumn("dbo.Aid", "LastUpdatedUserID", c => c.Int());
            CreateIndex("dbo.Aid", "CreatedUserID", name: "IX_CreatedUser_UserID");
            CreateIndex("dbo.Aid", "LastUpdatedUserID", name: "IX_LastUpdatedUser_UserID");
            AddForeignKey("dbo.Aid", "CreatedUserID", "dbo.User", "UserID");
            AddForeignKey("dbo.Aid", "LastUpdatedUserID", "dbo.User", "UserID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Aid", "LastUpdatedUserID", "dbo.User");
            DropForeignKey("dbo.Aid", "CreatedUserID", "dbo.User");
            DropIndex("dbo.Aid", "IX_LastUpdatedUser_UserID");
            DropIndex("dbo.Aid", "IX_CreatedUser_UserID");
            DropColumn("dbo.Aid", "LastUpdatedUserID");
            DropColumn("dbo.Aid", "CreatedUserID");
            DropColumn("dbo.Aid", "LastUpdatedDate");
            DropColumn("dbo.Aid", "CreatedDate");
            DropColumn("dbo.Aid", "SyncGuid");
            DropColumn("dbo.Aid", "SyncDate");
        }
    }
}
