namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameIndexesOnChildAndAdult : DbMigration
    {
        public override void Up()
        {
            RenameIndex(table: "dbo.Adult", name: "IX_Household_HouseHoldID", newName: "IX_HouseholdID");
            RenameIndex(table: "dbo.Adult", name: "IX_HIVStatus_HIVStatusID", newName: "IX_HIVStatusID");
            RenameIndex(table: "dbo.Child", name: "IX_HIVStatus_HIVStatusID", newName: "IX_HIVStatusID");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Child", name: "IX_HIVStatusID", newName: "IX_HIVStatus_HIVStatusID");
            RenameIndex(table: "dbo.Adult", name: "IX_HIVStatusID", newName: "IX_HIVStatus_HIVStatusID");
            RenameIndex(table: "dbo.Adult", name: "IX_HouseholdID", newName: "IX_Household_HouseHoldID");
        }
    }
}
