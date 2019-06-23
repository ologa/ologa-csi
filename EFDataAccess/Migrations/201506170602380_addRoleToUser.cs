namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addRoleToUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.User", "RoleID", c => c.Int());
            CreateIndex("dbo.User", "RoleID", name: "IX_Role_RoleID");
            AddForeignKey("dbo.User", "RoleID", "dbo.Role", "RoleID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.User", "RoleID", "dbo.Role");
            DropIndex("dbo.User", "IX_Role_RoleID");
            DropColumn("dbo.User", "RoleID");
        }
    }
}
