namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_NIDNumber_ToAdult : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Adult", "NID", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Adult", "NID");
        }
    }
}
