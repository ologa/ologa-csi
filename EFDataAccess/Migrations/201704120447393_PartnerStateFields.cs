namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PartnerStateFields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Partner", "ActivationDate", c => c.DateTime());
            AddColumn("dbo.Partner", "InactivationDate", c => c.DateTime());
            AddColumn("dbo.Partner", "Active", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Partner", "Active");
            DropColumn("dbo.Partner", "InactivationDate");
            DropColumn("dbo.Partner", "ActivationDate");
        }
    }
}
