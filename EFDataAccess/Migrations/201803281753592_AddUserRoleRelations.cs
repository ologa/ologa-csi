namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserRoleRelations : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserRole",
                c => new
                    {
                        UserRoleID = c.Int(nullable: false, identity: true),
                        UserRole_guid = c.Guid(nullable: false, defaultValueSql: "newid()"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        UserID = c.Int(nullable: false),
                        RoleID = c.Int(nullable: false),
                        CreatedDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.UserRoleID)
                .ForeignKey("dbo.Role", t => t.RoleID, cascadeDelete: true)
                .ForeignKey("dbo.User", t => t.UserID, cascadeDelete: true)
                .Index(t => t.UserID)
                .Index(t => t.RoleID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserRole", "UserID", "dbo.User");
            DropForeignKey("dbo.UserRole", "RoleID", "dbo.Role");
            DropIndex("dbo.UserRole", new[] { "RoleID" });
            DropIndex("dbo.UserRole", new[] { "UserID" });
            DropTable("dbo.UserRole");
        }
    }
}
