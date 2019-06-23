namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChildExposeHouseholdIDKey : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Child", "HouseholdID", "dbo.HouseHold");
            DropIndex("dbo.Child", "IX_Household_HouseHoldID");
            AlterColumn("dbo.Child", "HouseholdID", c => c.Int(nullable: true));
            CreateIndex("dbo.Child", "HouseholdID");
            AddForeignKey("dbo.Child", "HouseholdID", "dbo.HouseHold", "HouseHoldID", cascadeDelete: false);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Child", "HouseholdID", "dbo.HouseHold");
            DropIndex("dbo.Child", new[] { "HouseholdID" });
            AlterColumn("dbo.Child", "HouseholdID", c => c.Int());
            CreateIndex("dbo.Child", "HouseholdID", name: "IX_Household_HouseHoldID");
            AddForeignKey("dbo.Child", "HouseholdID", "dbo.HouseHold", "HouseHoldID");
        }
    }
}
