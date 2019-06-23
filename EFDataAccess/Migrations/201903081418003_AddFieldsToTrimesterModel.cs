namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFieldsToTrimesterModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Trimester", "FirstDay", c => c.Int(nullable: false));
            AddColumn("dbo.Trimester", "LastDay", c => c.Int(nullable: false));
            AddColumn("dbo.Trimester", "FirstMonth", c => c.Int(nullable: false));
            AddColumn("dbo.Trimester", "LastMonth", c => c.Int(nullable: false));
            AddColumn("dbo.Trimester", "Order", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Trimester", "Order");
            DropColumn("dbo.Trimester", "LastMonth");
            DropColumn("dbo.Trimester", "FirstMonth");
            DropColumn("dbo.Trimester", "LastDay");
            DropColumn("dbo.Trimester", "FirstDay");
        }
    }
}
