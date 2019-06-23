namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class freeTextSupportServices : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CarePlanDomainSupportService", "Description", c => c.String());
            AddColumn("dbo.CarePlanDomainSupportService", "order", c => c.Int(nullable: false));
            AddColumn("dbo.SupportServiceType", "Generic", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.SupportServiceType", "Generic");
            DropColumn("dbo.CarePlanDomainSupportService", "order");
            DropColumn("dbo.CarePlanDomainSupportService", "Description");
        }
    }
}
