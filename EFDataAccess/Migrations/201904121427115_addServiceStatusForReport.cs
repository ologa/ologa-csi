namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addServiceStatusForReport : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Beneficiary", "ServicesStatusForReportID", c => c.Int());
            AddColumn("dbo.Beneficiary", "ServicesStatusForReportDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Beneficiary", "ServicesStatusForReportDate");
            DropColumn("dbo.Beneficiary", "ServicesStatusForReportID");
        }
    }
}
