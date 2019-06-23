namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenameQuestionIndexOnCSIDomainScore : DbMigration
    {
        public override void Up()
        {
            RenameIndex(table: "dbo.CSIDomainScore", name: "IX_Question_QuestionID", newName: "IX_QuestionID");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.CSIDomainScore", name: "IX_QuestionID", newName: "IX_Question_QuestionID");
        }
    }
}
