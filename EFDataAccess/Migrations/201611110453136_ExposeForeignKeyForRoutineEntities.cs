namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ExposeForeignKeyForRoutineEntities : DbMigration
    {
        public override void Up()
        {
            RenameIndex(table: "dbo.RoutineVisitMember", name: "IX_Adult_AdultId", newName: "IX_AdultId");
            RenameIndex(table: "dbo.RoutineVisitMember", name: "IX_Child_ChildID", newName: "IX_ChildID");
            RenameIndex(table: "dbo.RoutineVisit", name: "IX_Household_HouseHoldID", newName: "IX_HouseHoldID");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.RoutineVisit", name: "IX_HouseHoldID", newName: "IX_Household_HouseHoldID");
            RenameIndex(table: "dbo.RoutineVisitMember", name: "IX_ChildID", newName: "IX_Child_ChildID");
            RenameIndex(table: "dbo.RoutineVisitMember", name: "IX_AdultId", newName: "IX_Adult_AdultId");
        }
    }
}
