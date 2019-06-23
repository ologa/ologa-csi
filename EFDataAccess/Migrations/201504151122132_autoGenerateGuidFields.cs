namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class autoGenerateGuidFields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ScoreType", "Score_guid", c => c.Guid(nullable: false, defaultValueSql: "newid()"));
            AlterColumn("dbo.Answer", "AnswerGUID", c => c.Guid(nullable: false, defaultValueSql: "newid()"));
            AlterColumn("dbo.Question", "question_guid", c => c.Guid(nullable: false, defaultValueSql: "newid()"));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Question", "question_guid", c => c.Guid(nullable: false));
            AlterColumn("dbo.Answer", "AnswerGUID", c => c.Guid(nullable: false));
            DropColumn("dbo.ScoreType", "Score_guid");
        }
    }
}
