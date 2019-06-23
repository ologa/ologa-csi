namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DatabaseGeneratedGuidOnChild : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Child", "child_guid", c => c.Guid(nullable: false, defaultValueSql: "newid()"));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Child", "child_guid", c => c.Guid(nullable: false));
        }
    }
}
