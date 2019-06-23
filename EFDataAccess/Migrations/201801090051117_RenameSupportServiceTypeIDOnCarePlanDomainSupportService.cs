namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameSupportServiceTypeIDOnCarePlanDomainSupportService : DbMigration
    {
        public override void Up()
        {
            RenameIndex(table: "dbo.CarePlanDomainSupportService", name: "IX_SupportServiceType_SupportServiceTypeID", newName: "IX_SupportServiceTypeID");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.CarePlanDomainSupportService", name: "IX_SupportServiceTypeID", newName: "IX_SupportServiceType_SupportServiceTypeID");
        }
    }
}
