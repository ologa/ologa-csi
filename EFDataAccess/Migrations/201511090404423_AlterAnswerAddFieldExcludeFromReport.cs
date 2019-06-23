namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AlterAnswerAddFieldExcludeFromReport : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Answer", "ExcludeFromReport", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Answer", "ExcludeFromReport");
        }
    }
}
