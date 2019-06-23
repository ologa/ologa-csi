namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddColumnOrderToQuestionEntity : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Question", "QuestionOrder", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Question", "QuestionOrder");
        }
    }
}
