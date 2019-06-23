namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameAidIndexOnHouseholdTable : DbMigration
    {
        public override void Up()
        {
            RenameIndex(table: "dbo.HouseHold", name: "IX_Aid_AidID", newName: "IX_AidID");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.HouseHold", name: "IX_AidID", newName: "IX_Aid_AidID");
        }
    }
}
