namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddInitialAndFinalPositionDateForAutomaticGeneratedReports : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ReportData", "InitialPositionDate", c => c.DateTime());
            AddColumn("dbo.ReportData", "FinalPositionDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ReportData", "FinalPositionDate");
            DropColumn("dbo.ReportData", "InitialPositionDate");
        }
    }
}
