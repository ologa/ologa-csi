namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Adding_Order_To_Domain_And_SupportService : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SupportServiceType", "DomainOrder", c => c.Int());
            AddColumn("dbo.SupportServiceType", "SupportServiceOrderInDomain", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.SupportServiceType", "SupportServiceOrderInDomain");
            DropColumn("dbo.SupportServiceType", "DomainOrder");
        }
    }
}
