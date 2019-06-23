namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AlterServicesStatusDateToNullableOnBeneficiary : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Beneficiary", "ServicesStatusDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Beneficiary", "ServicesStatusDate", c => c.DateTime(nullable: false));
        }
    }
}
