namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addRoleEntity : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Role",
                c => new
                    {
                        RoleID = c.Int(nullable: false, identity: true),
                        Description = c.String(maxLength: 200),
                        Role_guid = c.Guid(nullable: false, defaultValueSql: "newid()"),
                    })
                .PrimaryKey(t => t.RoleID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Role");
        }
    }
}
