namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddChildIDOnAdult : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Adult", "ChildID", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Adult", "ChildID");
        }
    }
}
