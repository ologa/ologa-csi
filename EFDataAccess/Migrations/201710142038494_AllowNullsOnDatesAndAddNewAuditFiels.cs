namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AllowNullsOnDatesAndAddNewAuditFiels : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CSIDomainScore", "SyncDate", c => c.DateTime());
            AddColumn("dbo.CSIDomainScore", "CreatedDate", c => c.DateTime());
            AddColumn("dbo.CSIDomainScore", "LastUpdatedDate", c => c.DateTime());
            AddColumn("dbo.CSIDomainScore", "CreatedUserID", c => c.Int());
            AddColumn("dbo.CSIDomainScore", "LastUpdatedUserID", c => c.Int());
            AddColumn("dbo.CSIDomain", "SyncDate", c => c.DateTime());
            AlterColumn("dbo.HouseHold", "CreatedDate", c => c.DateTime());
            AlterColumn("dbo.HouseHold", "LastUpdatedDate", c => c.DateTime());
            AlterColumn("dbo.ReferenceService", "CreatedDate", c => c.DateTime());
            AlterColumn("dbo.ReferenceService", "LastUpdatedDate", c => c.DateTime());
            AlterColumn("dbo.RoutineVisit", "CreatedDate", c => c.DateTime());
            AlterColumn("dbo.RoutineVisit", "LastUpdatedDate", c => c.DateTime());
            CreateIndex("dbo.CSIDomainScore", "CreatedUserID", name: "IX_CreatedUser_UserID");
            CreateIndex("dbo.CSIDomainScore", "LastUpdatedUserID", name: "IX_LastUpdatedUser_UserID");
            AddForeignKey("dbo.CSIDomainScore", "CreatedUserID", "dbo.User", "UserID");
            AddForeignKey("dbo.CSIDomainScore", "LastUpdatedUserID", "dbo.User", "UserID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CSIDomainScore", "LastUpdatedUserID", "dbo.User");
            DropForeignKey("dbo.CSIDomainScore", "CreatedUserID", "dbo.User");
            DropIndex("dbo.CSIDomainScore", "IX_LastUpdatedUser_UserID");
            DropIndex("dbo.CSIDomainScore", "IX_CreatedUser_UserID");
            AlterColumn("dbo.RoutineVisit", "LastUpdatedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.RoutineVisit", "CreatedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.ReferenceService", "LastUpdatedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.ReferenceService", "CreatedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.HouseHold", "LastUpdatedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.HouseHold", "CreatedDate", c => c.DateTime(nullable: false));
            DropColumn("dbo.CSIDomain", "SyncDate");
            DropColumn("dbo.CSIDomainScore", "LastUpdatedUserID");
            DropColumn("dbo.CSIDomainScore", "CreatedUserID");
            DropColumn("dbo.CSIDomainScore", "LastUpdatedDate");
            DropColumn("dbo.CSIDomainScore", "CreatedDate");
            DropColumn("dbo.CSIDomainScore", "SyncDate");
        }
    }
}
