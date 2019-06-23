namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Column_NID_Child : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Child", "NID", c => c.String(maxLength: 50));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Child", "NID");
        }
    }
}
