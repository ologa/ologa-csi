namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AlterAdultNotAllowNullForIsheadColumn : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Adult", "IsHouseHoldChef", c => c.Boolean(nullable: false, defaultValue: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Adult", "IsHouseHoldChef", c => c.Boolean());
        }
    }
}
