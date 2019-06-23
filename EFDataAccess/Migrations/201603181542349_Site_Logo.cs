namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Site_Logo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Site", "Logo", c => c.Binary());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Site", "Logo");
        }
    }
}
