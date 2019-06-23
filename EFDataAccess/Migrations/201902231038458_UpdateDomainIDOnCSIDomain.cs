namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateDomainIDOnCSIDomain : DbMigration
    {
        public override void Up()
        {
            RenameIndex(table: "dbo.CSIDomain", name: "IX_Domain_DomainID", newName: "IX_DomainID");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.CSIDomain", name: "IX_DomainID", newName: "IX_Domain_DomainID");
        }
    }
}
