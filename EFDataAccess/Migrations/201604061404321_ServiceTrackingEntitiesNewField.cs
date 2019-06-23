namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ServiceTrackingEntitiesNewField : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ServiceTrack", "CareplanID", c => c.Int());
            CreateIndex("dbo.ServiceTrack", "CareplanID", name: "IX_Careplan_CarePlanID");
            AddForeignKey("dbo.ServiceTrack", "CareplanID", "dbo.CarePlan", "CarePlanID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ServiceTrack", "CareplanID", "dbo.CarePlan");
            DropIndex("dbo.ServiceTrack", "IX_Careplan_CarePlanID");
            DropColumn("dbo.ServiceTrack", "CareplanID");
        }
    }
}
