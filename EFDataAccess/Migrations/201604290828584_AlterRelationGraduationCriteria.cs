namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AlterRelationGraduationCriteria : DbMigration
    {
        public override void Up()
        {
            //RenameTable(name: "dbo.GraduationCriteriaQuestions", newName: "QuestionGraduationCriterias");
            RenameIndex(table: "dbo.QuestionGraduationCriterias", name: "IX_GraduationCriteria_GraduationCriteriaID", newName: "IX_GraduationCriteriaID");
            RenameIndex(table: "dbo.QuestionGraduationCriterias", name: "IX_Question_QuestionID", newName: "IX_QuestionID");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.QuestionGraduationCriterias", name: "IX_QuestionID", newName: "IX_Question_QuestionID");
            RenameIndex(table: "dbo.QuestionGraduationCriterias", name: "IX_GraduationCriteriaID", newName: "IX_GraduationCriteria_GraduationCriteriaID");
            RenameTable(name: "dbo.QuestionGraduationCriterias", newName: "GraduationCriteriaQuestions");
        }
    }
}
