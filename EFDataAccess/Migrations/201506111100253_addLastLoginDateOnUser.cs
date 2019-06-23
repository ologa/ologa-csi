namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addLastLoginDateOnUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.User", "LastLoginDate", c => c.DateTime(precision: 7, storeType: "datetime2"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.User", "LastLoginDate");
        }
    }
}
