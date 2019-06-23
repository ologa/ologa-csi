namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Fields_to_Report_Data_Model : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ReportData", "Field21", c => c.String());
            AddColumn("dbo.ReportData", "Field22", c => c.String());
            AddColumn("dbo.ReportData", "Field23", c => c.String());
            AddColumn("dbo.ReportData", "Field24", c => c.String());
            AddColumn("dbo.ReportData", "Field25", c => c.String());
            AddColumn("dbo.ReportData", "Field26", c => c.String());
            AddColumn("dbo.ReportData", "Field27", c => c.String());
            AddColumn("dbo.ReportData", "Field28", c => c.String());
            AddColumn("dbo.ReportData", "Field29", c => c.String());
            AddColumn("dbo.ReportData", "Field30", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ReportData", "Field30");
            DropColumn("dbo.ReportData", "Field29");
            DropColumn("dbo.ReportData", "Field28");
            DropColumn("dbo.ReportData", "Field27");
            DropColumn("dbo.ReportData", "Field26");
            DropColumn("dbo.ReportData", "Field25");
            DropColumn("dbo.ReportData", "Field24");
            DropColumn("dbo.ReportData", "Field23");
            DropColumn("dbo.ReportData", "Field22");
            DropColumn("dbo.ReportData", "Field21");
        }
    }
}
