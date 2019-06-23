namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddColumn_QuestionVersion_on_QuestionTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Question", "QuestionVersion", c => c.Int(nullable: false));
            Sql(@"UPDATE [Question] SET QuestionVersion = 1", false);
        }

        public override void Down()
        {
            DropColumn("dbo.Question", "QuestionVersion");
        }
    }
}
