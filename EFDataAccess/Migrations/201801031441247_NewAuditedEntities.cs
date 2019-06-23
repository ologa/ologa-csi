namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NewAuditedEntities : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ChildStatus", "CreatedDate", c => c.DateTime());
            AddColumn("dbo.ChildStatus", "LastUpdatedDate", c => c.DateTime());
            AddColumn("dbo.ChildStatus", "SyncDate", c => c.DateTime());
            AddColumn("dbo.ChildStatus", "SyncGuid", c => c.Guid());
            AddColumn("dbo.ChildStatus", "CreatedUserID", c => c.Int());
            AddColumn("dbo.ChildStatus", "LastUpdatedUserID", c => c.Int());
            AddColumn("dbo.OrgUnit", "CreatedDate", c => c.DateTime());
            AddColumn("dbo.OrgUnit", "LastUpdatedDate", c => c.DateTime());
            AddColumn("dbo.OrgUnit", "SyncDate", c => c.DateTime());
            AddColumn("dbo.OrgUnit", "SyncGuid", c => c.Guid());
            AddColumn("dbo.OrgUnit", "CreatedUserID", c => c.Int());
            AddColumn("dbo.OrgUnit", "LastUpdatedUserID", c => c.Int());
            AddColumn("dbo.OrgUnitType", "CreatedDate", c => c.DateTime());
            AddColumn("dbo.OrgUnitType", "LastUpdatedDate", c => c.DateTime());
            AddColumn("dbo.OrgUnitType", "SyncDate", c => c.DateTime());
            AddColumn("dbo.OrgUnitType", "SyncGuid", c => c.Guid());
            AddColumn("dbo.OrgUnitType", "CreatedUserID", c => c.Int());
            AddColumn("dbo.OrgUnitType", "LastUpdatedUserID", c => c.Int());
            AddColumn("dbo.SimpleEntity", "CreatedDate", c => c.DateTime());
            AddColumn("dbo.SimpleEntity", "LastUpdatedDate", c => c.DateTime());
            AddColumn("dbo.SimpleEntity", "SyncDate", c => c.DateTime());
            AddColumn("dbo.SimpleEntity", "SyncGuid", c => c.Guid());
            AddColumn("dbo.SimpleEntity", "CreatedUserID", c => c.Int());
            AddColumn("dbo.SimpleEntity", "LastUpdatedUserID", c => c.Int());
            CreateIndex("dbo.ChildStatus", "CreatedUserID", name: "IX_CreatedUser_UserID");
            CreateIndex("dbo.ChildStatus", "LastUpdatedUserID", name: "IX_LastUpdatedUser_UserID");
            CreateIndex("dbo.OrgUnit", "CreatedUserID", name: "IX_CreatedUser_UserID");
            CreateIndex("dbo.OrgUnit", "LastUpdatedUserID", name: "IX_LastUpdatedUser_UserID");
            CreateIndex("dbo.OrgUnitType", "CreatedUserID", name: "IX_CreatedUser_UserID");
            CreateIndex("dbo.OrgUnitType", "LastUpdatedUserID", name: "IX_LastUpdatedUser_UserID");
            CreateIndex("dbo.SimpleEntity", "CreatedUserID", name: "IX_CreatedUser_UserID");
            CreateIndex("dbo.SimpleEntity", "LastUpdatedUserID", name: "IX_LastUpdatedUser_UserID");
            AddForeignKey("dbo.ChildStatus", "CreatedUserID", "dbo.User", "UserID");
            AddForeignKey("dbo.ChildStatus", "LastUpdatedUserID", "dbo.User", "UserID");
            AddForeignKey("dbo.OrgUnit", "CreatedUserID", "dbo.User", "UserID");
            AddForeignKey("dbo.OrgUnit", "LastUpdatedUserID", "dbo.User", "UserID");
            AddForeignKey("dbo.OrgUnitType", "CreatedUserID", "dbo.User", "UserID");
            AddForeignKey("dbo.OrgUnitType", "LastUpdatedUserID", "dbo.User", "UserID");
            AddForeignKey("dbo.SimpleEntity", "CreatedUserID", "dbo.User", "UserID");
            AddForeignKey("dbo.SimpleEntity", "LastUpdatedUserID", "dbo.User", "UserID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SimpleEntity", "LastUpdatedUserID", "dbo.User");
            DropForeignKey("dbo.SimpleEntity", "CreatedUserID", "dbo.User");
            DropForeignKey("dbo.OrgUnitType", "LastUpdatedUserID", "dbo.User");
            DropForeignKey("dbo.OrgUnitType", "CreatedUserID", "dbo.User");
            DropForeignKey("dbo.OrgUnit", "LastUpdatedUserID", "dbo.User");
            DropForeignKey("dbo.OrgUnit", "CreatedUserID", "dbo.User");
            DropForeignKey("dbo.ChildStatus", "LastUpdatedUserID", "dbo.User");
            DropForeignKey("dbo.ChildStatus", "CreatedUserID", "dbo.User");
            DropIndex("dbo.SimpleEntity", "IX_LastUpdatedUser_UserID");
            DropIndex("dbo.SimpleEntity", "IX_CreatedUser_UserID");
            DropIndex("dbo.OrgUnitType", "IX_LastUpdatedUser_UserID");
            DropIndex("dbo.OrgUnitType", "IX_CreatedUser_UserID");
            DropIndex("dbo.OrgUnit", "IX_LastUpdatedUser_UserID");
            DropIndex("dbo.OrgUnit", "IX_CreatedUser_UserID");
            DropIndex("dbo.ChildStatus", "IX_LastUpdatedUser_UserID");
            DropIndex("dbo.ChildStatus", "IX_CreatedUser_UserID");
            DropColumn("dbo.SimpleEntity", "LastUpdatedUserID");
            DropColumn("dbo.SimpleEntity", "CreatedUserID");
            DropColumn("dbo.SimpleEntity", "SyncGuid");
            DropColumn("dbo.SimpleEntity", "SyncDate");
            DropColumn("dbo.SimpleEntity", "LastUpdatedDate");
            DropColumn("dbo.SimpleEntity", "CreatedDate");
            DropColumn("dbo.OrgUnitType", "LastUpdatedUserID");
            DropColumn("dbo.OrgUnitType", "CreatedUserID");
            DropColumn("dbo.OrgUnitType", "SyncGuid");
            DropColumn("dbo.OrgUnitType", "SyncDate");
            DropColumn("dbo.OrgUnitType", "LastUpdatedDate");
            DropColumn("dbo.OrgUnitType", "CreatedDate");
            DropColumn("dbo.OrgUnit", "LastUpdatedUserID");
            DropColumn("dbo.OrgUnit", "CreatedUserID");
            DropColumn("dbo.OrgUnit", "SyncGuid");
            DropColumn("dbo.OrgUnit", "SyncDate");
            DropColumn("dbo.OrgUnit", "LastUpdatedDate");
            DropColumn("dbo.OrgUnit", "CreatedDate");
            DropColumn("dbo.ChildStatus", "LastUpdatedUserID");
            DropColumn("dbo.ChildStatus", "CreatedUserID");
            DropColumn("dbo.ChildStatus", "SyncGuid");
            DropColumn("dbo.ChildStatus", "SyncDate");
            DropColumn("dbo.ChildStatus", "LastUpdatedDate");
            DropColumn("dbo.ChildStatus", "CreatedDate");
        }
    }
}
