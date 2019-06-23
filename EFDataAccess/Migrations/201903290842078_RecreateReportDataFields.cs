namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RecreateReportDataFields : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.ReportData", "Field1");
            DropColumn("dbo.ReportData", "Field2");
            DropColumn("dbo.ReportData", "Field3");
            DropColumn("dbo.ReportData", "Field4");
            DropColumn("dbo.ReportData", "Field5");
            DropColumn("dbo.ReportData", "Field6");
            DropColumn("dbo.ReportData", "Field7");
            DropColumn("dbo.ReportData", "Field8");
            DropColumn("dbo.ReportData", "Field9");
            DropColumn("dbo.ReportData", "Field10");
            DropColumn("dbo.ReportData", "Field11");
            DropColumn("dbo.ReportData", "Field12");
            DropColumn("dbo.ReportData", "Field13");
            DropColumn("dbo.ReportData", "Field14");
            DropColumn("dbo.ReportData", "Field15");
            DropColumn("dbo.ReportData", "Field16");
            DropColumn("dbo.ReportData", "Field17");
            DropColumn("dbo.ReportData", "Field18");
            DropColumn("dbo.ReportData", "Field19");
            DropColumn("dbo.ReportData", "Field20");
            DropColumn("dbo.ReportData", "Field21");
            DropColumn("dbo.ReportData", "Field22");
            DropColumn("dbo.ReportData", "Field23");
            DropColumn("dbo.ReportData", "Field24");
            DropColumn("dbo.ReportData", "Field25");
            DropColumn("dbo.ReportData", "Field26");
            DropColumn("dbo.ReportData", "Field27");
            DropColumn("dbo.ReportData", "Field28");
            DropColumn("dbo.ReportData", "Field29");
            DropColumn("dbo.ReportData", "Field30");
            DropColumn("dbo.ReportData", "Field31");
            DropColumn("dbo.ReportData", "Field32");
            DropColumn("dbo.ReportData", "Field33");
            DropColumn("dbo.ReportData", "Field34");
            DropColumn("dbo.ReportData", "Field35");
            DropColumn("dbo.ReportData", "Field36");
            DropColumn("dbo.ReportData", "Field37");
            DropColumn("dbo.ReportData", "Field38");
            DropColumn("dbo.ReportData", "Field39");
            DropColumn("dbo.ReportData", "Field40");
            DropColumn("dbo.ReportData", "Field41");
            DropColumn("dbo.ReportData", "Field42");
            DropColumn("dbo.ReportData", "Field43");
            DropColumn("dbo.ReportData", "Field44");
            DropColumn("dbo.ReportData", "Field45");

            AddColumn("dbo.ReportData", "Field1", c => c.String());
            AddColumn("dbo.ReportData", "Field2", c => c.String());
            AddColumn("dbo.ReportData", "Field3", c => c.String());
            AddColumn("dbo.ReportData", "Field4", c => c.String());
            AddColumn("dbo.ReportData", "Field5", c => c.String());
            AddColumn("dbo.ReportData", "Field6", c => c.String());
            AddColumn("dbo.ReportData", "Field7", c => c.String());
            AddColumn("dbo.ReportData", "Field8", c => c.String());
            AddColumn("dbo.ReportData", "Field9", c => c.String());
            AddColumn("dbo.ReportData", "Field10", c => c.String());
            AddColumn("dbo.ReportData", "Field11", c => c.String());
            AddColumn("dbo.ReportData", "Field12", c => c.String());
            AddColumn("dbo.ReportData", "Field13", c => c.String());
            AddColumn("dbo.ReportData", "Field14", c => c.String());
            AddColumn("dbo.ReportData", "Field15", c => c.String());
            AddColumn("dbo.ReportData", "Field16", c => c.String());
            AddColumn("dbo.ReportData", "Field17", c => c.String());
            AddColumn("dbo.ReportData", "Field18", c => c.String());
            AddColumn("dbo.ReportData", "Field19", c => c.String());
            AddColumn("dbo.ReportData", "Field20", c => c.String());
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
            AddColumn("dbo.ReportData", "Field31", c => c.String());
            AddColumn("dbo.ReportData", "Field32", c => c.String());
            AddColumn("dbo.ReportData", "Field33", c => c.String());
            AddColumn("dbo.ReportData", "Field34", c => c.String());
            AddColumn("dbo.ReportData", "Field35", c => c.String());
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
        }
    }
}
