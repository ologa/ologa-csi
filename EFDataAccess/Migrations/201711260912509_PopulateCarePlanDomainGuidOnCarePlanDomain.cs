namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PopulateCarePlanDomainGuidOnCarePlanDomain : DbMigration
    {
        public override void Up()
        {
            Sql(@"update dbo.CarePlanDomain set careplandomain_guid = newid()");
        }

        public override void Down()
        {
        }
    }
}
