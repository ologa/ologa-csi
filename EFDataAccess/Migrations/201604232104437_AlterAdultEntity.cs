namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AlterAdultEntity : DbMigration
    {
        public override void Up()
        {
            
            AddColumn("dbo.Adult", "MaritalStatusID", c => c.Int());
            AddColumn("dbo.Adult", "DateOfBirth", c => c.DateTime());
            AddColumn("dbo.Adult", "ContactNo", c => c.String());
            AddColumn("dbo.Adult", "HIV", c => c.String());
            AddColumn("dbo.Adult", "AdultGuid", c => c.Guid(nullable: false));
            AddColumn("dbo.Adult", "CreatedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Adult", "LastUpdatedDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Adult", "CreatedUserID", c => c.Int());
            AddColumn("dbo.Adult", "LastUpdatedUserID", c => c.Int());
            AlterColumn("dbo.Adult", "IsHouseHoldChef", c => c.Boolean());
            AlterColumn("dbo.Adult", "passport", c => c.Int());
            CreateIndex("dbo.Adult", "CreatedUserID", name: "IX_CreatedUser_UserID");
            CreateIndex("dbo.Adult", "LastUpdatedUserID", name: "IX_LastUpdatedUser_UserID");
            AddForeignKey("dbo.Adult", "CreatedUserID", "dbo.User", "UserID");
            AddForeignKey("dbo.Adult", "LastUpdatedUserID", "dbo.User", "UserID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Adult", "LastUpdatedUserID", "dbo.User");
            DropForeignKey("dbo.Adult", "CreatedUserID", "dbo.User");
            DropIndex("dbo.Adult", "IX_LastUpdatedUser_UserID");
            DropIndex("dbo.Adult", "IX_CreatedUser_UserID");
            AlterColumn("dbo.Adult", "passport", c => c.Int(nullable: false));
            AlterColumn("dbo.Adult", "IsHouseHoldChef", c => c.Boolean(nullable: false));
            DropColumn("dbo.Adult", "LastUpdatedUserID");
            DropColumn("dbo.Adult", "CreatedUserID");
            DropColumn("dbo.Adult", "LastUpdatedDate");
            DropColumn("dbo.Adult", "CreatedDate");
            DropColumn("dbo.Adult", "AdultGuid");
            DropColumn("dbo.Adult", "HIV");
            DropColumn("dbo.Adult", "ContactNo");
            DropColumn("dbo.Adult", "DateOfBirth");
            DropColumn("dbo.Adult", "MaritalStatusID");
        }
    }
}
