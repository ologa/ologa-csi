namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddNewFieldsInReportData : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ReportData", "Field36", c => c.String());
            AddColumn("dbo.ReportData", "Field37", c => c.String());
            AddColumn("dbo.ReportData", "Field38", c => c.String());
            AddColumn("dbo.ReportData", "Field39", c => c.String());
            AddColumn("dbo.ReportData", "Field40", c => c.String());
            AddColumn("dbo.ReportData", "Field41", c => c.String());
            AddColumn("dbo.ReportData", "Field42", c => c.String());
            AddColumn("dbo.ReportData", "Field43", c => c.String());
            AddColumn("dbo.ReportData", "Field44", c => c.String());
            AddColumn("dbo.ReportData", "Field45", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ReportData", "Field45");
            DropColumn("dbo.ReportData", "Field44");
            DropColumn("dbo.ReportData", "Field43");
            DropColumn("dbo.ReportData", "Field42");
            DropColumn("dbo.ReportData", "Field41");
            DropColumn("dbo.ReportData", "Field40");
            DropColumn("dbo.ReportData", "Field39");
            DropColumn("dbo.ReportData", "Field38");
            DropColumn("dbo.ReportData", "Field37");
            DropColumn("dbo.ReportData", "Field36");
        }
    }
}
