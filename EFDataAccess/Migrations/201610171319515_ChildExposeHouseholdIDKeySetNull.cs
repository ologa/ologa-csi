namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChildExposeHouseholdIDKeySetNull : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Child", "HouseholdID", "dbo.HouseHold");
            DropIndex("dbo.Child", new[] { "HouseholdID" });
            AlterColumn("dbo.Child", "HouseholdID", c => c.Int());
            CreateIndex("dbo.Child", "HouseholdID");
            AddForeignKey("dbo.Child", "HouseholdID", "dbo.HouseHold", "HouseHoldID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Child", "HouseholdID", "dbo.HouseHold");
            DropIndex("dbo.Child", new[] { "HouseholdID" });
            AlterColumn("dbo.Child", "HouseholdID", c => c.Int(nullable: false));
            CreateIndex("dbo.Child", "HouseholdID");
            AddForeignKey("dbo.Child", "HouseholdID", "dbo.HouseHold", "HouseHoldID", cascadeDelete: true);
        }
    }
}
