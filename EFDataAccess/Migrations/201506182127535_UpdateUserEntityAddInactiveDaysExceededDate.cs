namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateUserEntityAddInactiveDaysExceededDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.User", "InactiveDaysExceededDate", c => c.DateTime(precision: 7, storeType: "datetime2"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.User", "InactiveDaysExceededDate");
        }
    }
}
