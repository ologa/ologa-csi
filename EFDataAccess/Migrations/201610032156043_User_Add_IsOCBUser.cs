namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class User_Add_IsOCBUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.User", "IsOCBUser", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.User", "IsOCBUser");
        }
    }
}
