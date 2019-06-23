namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameIndexesOnChildTable : DbMigration
    {
        public override void Up()
        {
            RenameIndex(table: "dbo.CSI", name: "IX_Child_ChildID", newName: "IX_ChildID");
            RenameIndex(table: "dbo.CSIDomainScore", name: "IX_CSIDomain_CSIDomainID", newName: "IX_CSIDomainID");
            RenameIndex(table: "dbo.CSIDomain", name: "IX_CSI_CSIID", newName: "IX_CSIID");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.CSIDomain", name: "IX_CSIID", newName: "IX_CSI_CSIID");
            RenameIndex(table: "dbo.CSIDomainScore", name: "IX_CSIDomainID", newName: "IX_CSIDomain_CSIDomainID");
            RenameIndex(table: "dbo.CSI", name: "IX_ChildID", newName: "IX_Child_ChildID");
        }
    }
}
