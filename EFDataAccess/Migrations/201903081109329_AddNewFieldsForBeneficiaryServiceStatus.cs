namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddNewFieldsForBeneficiaryServiceStatus : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Beneficiary", "ServicesStatusDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Beneficiary", "ServicesStatusID", c => c.Int());
            CreateIndex("dbo.Beneficiary", "ServicesStatusID", name: "IX_ServicesStatus_SimpleEntityID");
            AddForeignKey("dbo.Beneficiary", "ServicesStatusID", "dbo.SimpleEntity", "SimpleEntityID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Beneficiary", "ServicesStatusID", "dbo.SimpleEntity");
            DropIndex("dbo.Beneficiary", "IX_ServicesStatus_SimpleEntityID");
            DropColumn("dbo.Beneficiary", "ServicesStatusID");
            DropColumn("dbo.Beneficiary", "ServicesStatusDate");
        }
    }
}
