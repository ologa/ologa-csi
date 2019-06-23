namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddHIVTrackedOnAdultsAndChildren : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Adult", "HIVTracked", c => c.Boolean(nullable: false));
            AddColumn("dbo.Child", "HIVTracked", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Child", "HIVTracked");
            DropColumn("dbo.Adult", "HIVTracked");
        }
    }
}
