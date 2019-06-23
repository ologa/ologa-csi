namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RoutineVisitSyncChanges : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.RoutineVisitMember", "RoutineVisitID", "dbo.RoutineVisit");
            DropForeignKey("dbo.RoutineVisitSupport", "RoutineVisitMemberID", "dbo.RoutineVisitMember");
            DropIndex("dbo.RoutineVisitMember", "IX_RoutineVisit_RoutineVisitID");
            DropIndex("dbo.RoutineVisitSupport", "IX_RoutineVisitMember_RoutineVisitMemberID");
            AddColumn("dbo.RoutineVisitSupport", "CreatedDate", c => c.DateTime());
            AddColumn("dbo.RoutineVisitSupport", "LastUpdatedDate", c => c.DateTime());
            AddColumn("dbo.RoutineVisitSupport", "SyncDate", c => c.DateTime());
            AddColumn("dbo.RoutineVisitSupport", "SyncGuid", c => c.Guid());
            AddColumn("dbo.RoutineVisitSupport", "CreatedUserID", c => c.Int());
            AddColumn("dbo.RoutineVisitSupport", "LastUpdatedUserID", c => c.Int());
            AlterColumn("dbo.RoutineVisitMember", "RoutineVisitID", c => c.Int(nullable: false));
            AlterColumn("dbo.RoutineVisitSupport", "RoutineVisitMemberID", c => c.Int(nullable: false));
            CreateIndex("dbo.RoutineVisitMember", "RoutineVisitID");
            CreateIndex("dbo.RoutineVisitSupport", "RoutineVisitMemberID");
            CreateIndex("dbo.RoutineVisitSupport", "CreatedUserID", name: "IX_CreatedUser_UserID");
            CreateIndex("dbo.RoutineVisitSupport", "LastUpdatedUserID", name: "IX_LastUpdatedUser_UserID");
            AddForeignKey("dbo.RoutineVisitSupport", "CreatedUserID", "dbo.User", "UserID");
            AddForeignKey("dbo.RoutineVisitSupport", "LastUpdatedUserID", "dbo.User", "UserID");
            AddForeignKey("dbo.RoutineVisitMember", "RoutineVisitID", "dbo.RoutineVisit", "RoutineVisitID", cascadeDelete: true);
            AddForeignKey("dbo.RoutineVisitSupport", "RoutineVisitMemberID", "dbo.RoutineVisitMember", "RoutineVisitMemberID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RoutineVisitSupport", "RoutineVisitMemberID", "dbo.RoutineVisitMember");
            DropForeignKey("dbo.RoutineVisitMember", "RoutineVisitID", "dbo.RoutineVisit");
            DropForeignKey("dbo.RoutineVisitSupport", "LastUpdatedUserID", "dbo.User");
            DropForeignKey("dbo.RoutineVisitSupport", "CreatedUserID", "dbo.User");
            DropIndex("dbo.RoutineVisitSupport", "IX_LastUpdatedUser_UserID");
            DropIndex("dbo.RoutineVisitSupport", "IX_CreatedUser_UserID");
            DropIndex("dbo.RoutineVisitSupport", new[] { "RoutineVisitMemberID" });
            DropIndex("dbo.RoutineVisitMember", new[] { "RoutineVisitID" });
            AlterColumn("dbo.RoutineVisitSupport", "RoutineVisitMemberID", c => c.Int());
            AlterColumn("dbo.RoutineVisitMember", "RoutineVisitID", c => c.Int());
            DropColumn("dbo.RoutineVisitSupport", "LastUpdatedUserID");
            DropColumn("dbo.RoutineVisitSupport", "CreatedUserID");
            DropColumn("dbo.RoutineVisitSupport", "SyncGuid");
            DropColumn("dbo.RoutineVisitSupport", "SyncDate");
            DropColumn("dbo.RoutineVisitSupport", "LastUpdatedDate");
            DropColumn("dbo.RoutineVisitSupport", "CreatedDate");
            CreateIndex("dbo.RoutineVisitSupport", "RoutineVisitMemberID", name: "IX_RoutineVisitMember_RoutineVisitMemberID");
            CreateIndex("dbo.RoutineVisitMember", "RoutineVisitID", name: "IX_RoutineVisit_RoutineVisitID");
            AddForeignKey("dbo.RoutineVisitSupport", "RoutineVisitMemberID", "dbo.RoutineVisitMember", "RoutineVisitMemberID");
            AddForeignKey("dbo.RoutineVisitMember", "RoutineVisitID", "dbo.RoutineVisit", "RoutineVisitID");
        }
    }
}
