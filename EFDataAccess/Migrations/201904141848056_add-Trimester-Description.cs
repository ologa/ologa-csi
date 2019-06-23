namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addTrimesterDescription : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Trimester", "TrimesterDescription", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Trimester", "TrimesterDescription");
        }
    }
}
