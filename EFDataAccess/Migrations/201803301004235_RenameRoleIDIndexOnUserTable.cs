namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameRoleIDIndexOnUserTable : DbMigration
    {
        public override void Up()
        {
            RenameIndex(table: "dbo.User", name: "IX_Role_RoleID", newName: "IX_RoleID");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.User", name: "IX_RoleID", newName: "IX_Role_RoleID");
        }
    }
}
