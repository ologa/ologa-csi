namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Columns_CreatedDate_LastUpdatedDate_CreatedUser_LastUpdatedUser_in_RoutineVisit : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RoutineVisit", "CreatedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.RoutineVisit", "LastUpdatedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.RoutineVisit", "CreatedUserID", c => c.Int());
            AddColumn("dbo.RoutineVisit", "LastUpdatedUserID", c => c.Int());
            CreateIndex("dbo.RoutineVisit", "CreatedUserID", name: "IX_CreatedUser_UserID");
            CreateIndex("dbo.RoutineVisit", "LastUpdatedUserID", name: "IX_LastUpdatedUser_UserID");
            AddForeignKey("dbo.RoutineVisit", "CreatedUserID", "dbo.User", "UserID");
            AddForeignKey("dbo.RoutineVisit", "LastUpdatedUserID", "dbo.User", "UserID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RoutineVisit", "LastUpdatedUserID", "dbo.User");
            DropForeignKey("dbo.RoutineVisit", "CreatedUserID", "dbo.User");
            DropIndex("dbo.RoutineVisit", "IX_LastUpdatedUser_UserID");
            DropIndex("dbo.RoutineVisit", "IX_CreatedUser_UserID");
            DropColumn("dbo.RoutineVisit", "LastUpdatedUserID");
            DropColumn("dbo.RoutineVisit", "CreatedUserID");
            DropColumn("dbo.RoutineVisit", "LastUpdatedDate");
            DropColumn("dbo.RoutineVisit", "CreatedDate");
        }
    }
}
