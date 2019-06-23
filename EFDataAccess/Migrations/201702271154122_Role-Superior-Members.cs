namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RoleSuperiorMembers : DbMigration
    {

        public override void Up()
        {
            CreateTable(
                "dbo.CollaboratorRole",
                c => new
                    {
                        CollaboratorRoleID = c.Int(nullable: false, identity: true),
                        Code = c.String(maxLength: 20),
                        Description = c.String(maxLength: 200),
                        CollaboratorRole_guid = c.Guid(nullable: false, defaultValueSql: "newid()"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.CollaboratorRoleID);
            
            AddColumn("dbo.Partner", "SuperiorId", c => c.Int());
            AddColumn("dbo.Partner", "CollaboratorRoleID", c => c.Int());
            CreateIndex("dbo.Partner", "SuperiorId");
            CreateIndex("dbo.Partner", "CollaboratorRoleID", name: "IX_CollaboratorRole_CollaboratorRoleID");
            AddForeignKey("dbo.Partner", "CollaboratorRoleID", "dbo.CollaboratorRole", "CollaboratorRoleID");
            AddForeignKey("dbo.Partner", "SuperiorId", "dbo.Partner", "PartnerID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Partner", "SuperiorId", "dbo.Partner");
            DropForeignKey("dbo.Partner", "CollaboratorRoleID", "dbo.CollaboratorRole");
            DropIndex("dbo.Partner", "IX_CollaboratorRole_CollaboratorRoleID");
            DropIndex("dbo.Partner", new[] { "SuperiorId" });
            DropColumn("dbo.Partner", "CollaboratorRoleID");
            DropColumn("dbo.Partner", "SuperiorId");
            DropTable("dbo.CollaboratorRole");
        }
    }
}
