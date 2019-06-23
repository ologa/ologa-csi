namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeTrimesterTableColumnOrderToTrimesterSequence : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Trimester", "TrimesterSequence", c => c.Int(nullable: false));
            DropColumn("dbo.Trimester", "Order");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Trimester", "Order", c => c.Int(nullable: false));
            DropColumn("dbo.Trimester", "TrimesterSequence");
        }
    }
}
