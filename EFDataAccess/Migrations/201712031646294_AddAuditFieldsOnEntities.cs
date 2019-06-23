namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAuditFieldsOnEntities : DbMigration
    {
        public override void Up()
        {
            RenameIndex(table: "dbo.Partner", name: "IX_site_SiteID", newName: "IX_siteID");
            AddColumn("dbo.Site", "SyncGuid", c => c.Guid(nullable: false));
            AddColumn("dbo.Site", "SyncDate", c => c.DateTime());
            AddColumn("dbo.Reference", "SyncDate", c => c.DateTime());
            AddColumn("dbo.Reference", "SyncGuid", c => c.Guid());
            AddColumn("dbo.Reference", "CreatedDate", c => c.DateTime());
            AddColumn("dbo.Reference", "LastUpdatedDate", c => c.DateTime());
            AddColumn("dbo.Reference", "CreatedUserID", c => c.Int());
            AddColumn("dbo.Reference", "LastUpdatedUserID", c => c.Int());
            AddColumn("dbo.ReferenceService", "SyncDate", c => c.DateTime());
            AddColumn("dbo.ReferenceService", "SyncGuid", c => c.Guid());
            AddColumn("dbo.RoutineVisitMember", "SyncDate", c => c.DateTime());
            AddColumn("dbo.RoutineVisitMember", "SyncGuid", c => c.Guid());
            AddColumn("dbo.RoutineVisitMember", "CreatedDate", c => c.DateTime());
            AddColumn("dbo.RoutineVisitMember", "LastUpdatedDate", c => c.DateTime());
            AddColumn("dbo.RoutineVisitMember", "CreatedUserID", c => c.Int());
            AddColumn("dbo.RoutineVisitMember", "LastUpdatedUserID", c => c.Int());
            AddColumn("dbo.RoutineVisit", "SyncDate", c => c.DateTime());
            AddColumn("dbo.RoutineVisit", "SyncGuid", c => c.Guid());
            AlterColumn("dbo.Partner", "partner_guid", c => c.Guid(nullable: false, defaultValueSql: "newid()"));
            CreateIndex("dbo.Reference", "CreatedUserID", name: "IX_CreatedUser_UserID");
            CreateIndex("dbo.Reference", "LastUpdatedUserID", name: "IX_LastUpdatedUser_UserID");
            CreateIndex("dbo.RoutineVisitMember", "CreatedUserID", name: "IX_CreatedUser_UserID");
            CreateIndex("dbo.RoutineVisitMember", "LastUpdatedUserID", name: "IX_LastUpdatedUser_UserID");
            AddForeignKey("dbo.Reference", "CreatedUserID", "dbo.User", "UserID");
            AddForeignKey("dbo.Reference", "LastUpdatedUserID", "dbo.User", "UserID");
            AddForeignKey("dbo.RoutineVisitMember", "CreatedUserID", "dbo.User", "UserID");
            AddForeignKey("dbo.RoutineVisitMember", "LastUpdatedUserID", "dbo.User", "UserID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RoutineVisitMember", "LastUpdatedUserID", "dbo.User");
            DropForeignKey("dbo.RoutineVisitMember", "CreatedUserID", "dbo.User");
            DropForeignKey("dbo.Reference", "LastUpdatedUserID", "dbo.User");
            DropForeignKey("dbo.Reference", "CreatedUserID", "dbo.User");
            DropIndex("dbo.RoutineVisitMember", "IX_LastUpdatedUser_UserID");
            DropIndex("dbo.RoutineVisitMember", "IX_CreatedUser_UserID");
            DropIndex("dbo.Reference", "IX_LastUpdatedUser_UserID");
            DropIndex("dbo.Reference", "IX_CreatedUser_UserID");
            AlterColumn("dbo.Partner", "partner_guid", c => c.Guid(nullable: false));
            DropColumn("dbo.RoutineVisit", "SyncGuid");
            DropColumn("dbo.RoutineVisit", "SyncDate");
            DropColumn("dbo.RoutineVisitMember", "LastUpdatedUserID");
            DropColumn("dbo.RoutineVisitMember", "CreatedUserID");
            DropColumn("dbo.RoutineVisitMember", "LastUpdatedDate");
            DropColumn("dbo.RoutineVisitMember", "CreatedDate");
            DropColumn("dbo.RoutineVisitMember", "SyncGuid");
            DropColumn("dbo.RoutineVisitMember", "SyncDate");
            DropColumn("dbo.ReferenceService", "SyncGuid");
            DropColumn("dbo.ReferenceService", "SyncDate");
            DropColumn("dbo.Reference", "LastUpdatedUserID");
            DropColumn("dbo.Reference", "CreatedUserID");
            DropColumn("dbo.Reference", "LastUpdatedDate");
            DropColumn("dbo.Reference", "CreatedDate");
            DropColumn("dbo.Reference", "SyncGuid");
            DropColumn("dbo.Reference", "SyncDate");
            DropColumn("dbo.Site", "SyncDate");
            DropColumn("dbo.Site", "SyncGuid");
            RenameIndex(table: "dbo.Partner", name: "IX_siteID", newName: "IX_site_SiteID");
        }
    }
}
