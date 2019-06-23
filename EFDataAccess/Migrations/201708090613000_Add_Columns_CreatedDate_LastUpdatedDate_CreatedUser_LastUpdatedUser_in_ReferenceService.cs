namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Columns_CreatedDate_LastUpdatedDate_CreatedUser_LastUpdatedUser_in_ReferenceService : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ReferenceService", "CreatedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.ReferenceService", "LastUpdatedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.ReferenceService", "CreatedUserID", c => c.Int());
            AddColumn("dbo.ReferenceService", "LastUpdatedUserID", c => c.Int());
            CreateIndex("dbo.ReferenceService", "CreatedUserID", name: "IX_CreatedUser_UserID");
            CreateIndex("dbo.ReferenceService", "LastUpdatedUserID", name: "IX_LastUpdatedUser_UserID");
            AddForeignKey("dbo.ReferenceService", "CreatedUserID", "dbo.User", "UserID");
            AddForeignKey("dbo.ReferenceService", "LastUpdatedUserID", "dbo.User", "UserID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ReferenceService", "LastUpdatedUserID", "dbo.User");
            DropForeignKey("dbo.ReferenceService", "CreatedUserID", "dbo.User");
            DropIndex("dbo.ReferenceService", "IX_LastUpdatedUser_UserID");
            DropIndex("dbo.ReferenceService", "IX_CreatedUser_UserID");
            DropColumn("dbo.ReferenceService", "LastUpdatedUserID");
            DropColumn("dbo.ReferenceService", "CreatedUserID");
            DropColumn("dbo.ReferenceService", "LastUpdatedDate");
            DropColumn("dbo.ReferenceService", "CreatedDate");
        }
    }
}
