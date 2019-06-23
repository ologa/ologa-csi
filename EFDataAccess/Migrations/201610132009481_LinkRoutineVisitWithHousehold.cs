namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LinkRoutineVisitWithHousehold : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RoutineVisit", "HouseholdID", c => c.Int());
            CreateIndex("dbo.RoutineVisit", "HouseholdID", name: "IX_Household_HouseHoldID");
            AddForeignKey("dbo.RoutineVisit", "HouseholdID", "dbo.HouseHold", "HouseHoldID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RoutineVisit", "HouseholdID", "dbo.HouseHold");
            DropIndex("dbo.RoutineVisit", "IX_Household_HouseHoldID");
            DropColumn("dbo.RoutineVisit", "HouseholdID");
        }
    }
}
