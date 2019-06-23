namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAudtiFieldsOnSiteGoals : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SiteGoal", "CreatedDate", c => c.DateTime());
            AddColumn("dbo.SiteGoal", "LastUpdatedDate", c => c.DateTime());
            AddColumn("dbo.SiteGoal", "SyncDate", c => c.DateTime());
            AddColumn("dbo.SiteGoal", "SyncGuid", c => c.Guid());
            AddColumn("dbo.SiteGoal", "CreatedUserID", c => c.Int());
            AddColumn("dbo.SiteGoal", "LastUpdatedUserID", c => c.Int());
            AlterColumn("dbo.Site", "SyncGuid", c => c.Guid());
            CreateIndex("dbo.SiteGoal", "CreatedUserID", name: "IX_CreatedUser_UserID");
            CreateIndex("dbo.SiteGoal", "LastUpdatedUserID", name: "IX_LastUpdatedUser_UserID");
            AddForeignKey("dbo.SiteGoal", "CreatedUserID", "dbo.User", "UserID");
            AddForeignKey("dbo.SiteGoal", "LastUpdatedUserID", "dbo.User", "UserID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SiteGoal", "LastUpdatedUserID", "dbo.User");
            DropForeignKey("dbo.SiteGoal", "CreatedUserID", "dbo.User");
            DropIndex("dbo.SiteGoal", "IX_LastUpdatedUser_UserID");
            DropIndex("dbo.SiteGoal", "IX_CreatedUser_UserID");
            AlterColumn("dbo.Site", "SyncGuid", c => c.Guid(nullable: false));
            DropColumn("dbo.SiteGoal", "LastUpdatedUserID");
            DropColumn("dbo.SiteGoal", "CreatedUserID");
            DropColumn("dbo.SiteGoal", "SyncGuid");
            DropColumn("dbo.SiteGoal", "SyncDate");
            DropColumn("dbo.SiteGoal", "LastUpdatedDate");
            DropColumn("dbo.SiteGoal", "CreatedDate");
        }
    }
}
