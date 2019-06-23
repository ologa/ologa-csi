namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateChildStatusHistoryToInheritFromAuditedEntity : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ChildStatusHistory", "LastUpdatedDate", c => c.DateTime());
            AddColumn("dbo.ChildStatusHistory", "LastUpdatedUserID", c => c.Int());
            AlterColumn("dbo.ChildStatusHistory", "CreatedDate", c => c.DateTime());
            CreateIndex("dbo.ChildStatusHistory", "LastUpdatedUserID", name: "IX_LastUpdatedUser_UserID");
            AddForeignKey("dbo.ChildStatusHistory", "LastUpdatedUserID", "dbo.User", "UserID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ChildStatusHistory", "LastUpdatedUserID", "dbo.User");
            DropIndex("dbo.ChildStatusHistory", "IX_LastUpdatedUser_UserID");
            AlterColumn("dbo.ChildStatusHistory", "CreatedDate", c => c.DateTime(nullable: false));
            DropColumn("dbo.ChildStatusHistory", "LastUpdatedUserID");
            DropColumn("dbo.ChildStatusHistory", "LastUpdatedDate");
        }
    }
}
