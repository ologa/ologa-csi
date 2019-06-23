namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddServiceProviderToCarePlan : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CarePlanDomainSupportService", "ServiceProviderID", c => c.Int());
            CreateIndex("dbo.CarePlanDomainSupportService", "ServiceProviderID", name: "IX_ServiceProvider_ServiceProviderID");
            AddForeignKey("dbo.CarePlanDomainSupportService", "ServiceProviderID", "dbo.ServiceProvider", "ServiceProviderID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CarePlanDomainSupportService", "ServiceProviderID", "dbo.ServiceProvider");
            DropIndex("dbo.CarePlanDomainSupportService", "IX_ServiceProvider_ServiceProviderID");
            DropColumn("dbo.CarePlanDomainSupportService", "ServiceProviderID");
        }
    }
}
