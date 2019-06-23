namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCodeOnHouseholdAndPartner : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Partner", "Code", c => c.String(nullable: false));
            AddColumn("dbo.HouseHold", "Code", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.HouseHold", "Code");
            DropColumn("dbo.Partner", "Code");
        }
    }
}
