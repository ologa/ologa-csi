namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LinkTaskToService : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tasks", "CarePlanDomainSupportServiceID", c => c.Int());
            AddColumn("dbo.Tasks", "ServiceProviderID", c => c.Int());
            AddColumn("dbo.Resources", "IsOrganization", c => c.Boolean());
            CreateIndex("dbo.Tasks", "CarePlanDomainSupportServiceID", name: "IX_CarePlanDomainSupportService_CarePlanDomainSupportServiceID");
            CreateIndex("dbo.Tasks", "ServiceProviderID", name: "IX_ServiceProvider_ServiceProviderID");
            AddForeignKey("dbo.Tasks", "CarePlanDomainSupportServiceID", "dbo.CarePlanDomainSupportService", "CarePlanDomainSupportServiceID");
            AddForeignKey("dbo.Tasks", "ServiceProviderID", "dbo.ServiceProvider", "ServiceProviderID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tasks", "ServiceProviderID", "dbo.ServiceProvider");
            DropForeignKey("dbo.Tasks", "CarePlanDomainSupportServiceID", "dbo.CarePlanDomainSupportService");
            DropIndex("dbo.Tasks", "IX_ServiceProvider_ServiceProviderID");
            DropIndex("dbo.Tasks", "IX_CarePlanDomainSupportService_CarePlanDomainSupportServiceID");
            DropColumn("dbo.Resources", "IsOrganization");
            DropColumn("dbo.Tasks", "ServiceProviderID");
            DropColumn("dbo.Tasks", "CarePlanDomainSupportServiceID");
        }
    }
}
