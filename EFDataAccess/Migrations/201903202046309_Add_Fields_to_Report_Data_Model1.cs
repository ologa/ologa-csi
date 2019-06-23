namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Fields_to_Report_Data_Model1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ReportData", "Field31", c => c.String());
            AddColumn("dbo.ReportData", "Field32", c => c.String());
            AddColumn("dbo.ReportData", "Field33", c => c.String());
            AddColumn("dbo.ReportData", "Field34", c => c.String());
            AddColumn("dbo.ReportData", "Field35", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ReportData", "Field35");
            DropColumn("dbo.ReportData", "Field34");
            DropColumn("dbo.ReportData", "Field33");
            DropColumn("dbo.ReportData", "Field32");
            DropColumn("dbo.ReportData", "Field31");
        }
    }
}
