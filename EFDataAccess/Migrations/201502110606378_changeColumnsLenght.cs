namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changeColumnsLenght : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Answer", "Description", c => c.String(maxLength: 200));
            AlterColumn("dbo.Question", "Description", c => c.String(nullable: false, maxLength: 200, unicode: false));
            AlterColumn("dbo.Question", "Goal", c => c.String(nullable: false, maxLength: 200));
            AlterColumn("dbo.Domain", "Description", c => c.String(nullable: false, maxLength: 100, unicode: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Domain", "Description", c => c.String(nullable: false, maxLength: 30, unicode: false));
            AlterColumn("dbo.Question", "Goal", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.Question", "Description", c => c.String(nullable: false, maxLength: 100, unicode: false));
            AlterColumn("dbo.Answer", "Description", c => c.String(maxLength: 30));
        }
    }
}
