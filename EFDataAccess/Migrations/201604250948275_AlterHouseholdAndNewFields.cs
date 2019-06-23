namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AlterHouseholdAndNewFields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.HouseHold", "Address", c => c.String());
            AddColumn("dbo.HouseHold", "LastUpdatedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.HouseHold", "CreatedUserID", c => c.Int());
            AddColumn("dbo.HouseHold", "LastUpdatedUserID", c => c.Int());
            CreateIndex("dbo.HouseHold", "CreatedUserID", name: "IX_CreatedUser_UserID");
            CreateIndex("dbo.HouseHold", "LastUpdatedUserID", name: "IX_LastUpdatedUser_UserID");
            AddForeignKey("dbo.HouseHold", "CreatedUserID", "dbo.User", "UserID");
            AddForeignKey("dbo.HouseHold", "LastUpdatedUserID", "dbo.User", "UserID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.HouseHold", "LastUpdatedUserID", "dbo.User");
            DropForeignKey("dbo.HouseHold", "CreatedUserID", "dbo.User");
            DropIndex("dbo.HouseHold", "IX_LastUpdatedUser_UserID");
            DropIndex("dbo.HouseHold", "IX_CreatedUser_UserID");
            DropColumn("dbo.HouseHold", "LastUpdatedUserID");
            DropColumn("dbo.HouseHold", "CreatedUserID");
            DropColumn("dbo.HouseHold", "LastUpdatedDate");
            DropColumn("dbo.HouseHold", "Address");
        }
    }
}
