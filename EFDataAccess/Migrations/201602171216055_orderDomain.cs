namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class orderDomain : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Domain", "Order", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Domain", "Order");
        }
    }
}
