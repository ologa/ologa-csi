namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DatabaseGeneratedGuidOnCarePlan : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.CarePlan", "careplan_guid", c => c.Guid(nullable: false, defaultValueSql: "newid()"));
            AlterColumn("dbo.CarePlanDomain", "careplandomain_guid", c => c.Guid(nullable: false, defaultValueSql: "newid()"));
            AlterColumn("dbo.CarePlanDomainSupportService", "careplandomainsupportservice_guid", c => c.Guid(nullable: false, defaultValueSql: "newid()"));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.CarePlanDomainSupportService", "careplandomainsupportservice_guid", c => c.Guid(nullable: false));
            AlterColumn("dbo.CarePlanDomain", "careplandomain_guid", c => c.Guid(nullable: false));
            AlterColumn("dbo.CarePlan", "careplan_guid", c => c.Guid(nullable: false));
        }
    }
}
