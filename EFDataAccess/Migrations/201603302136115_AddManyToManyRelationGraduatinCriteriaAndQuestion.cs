namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddManyToManyRelationGraduatinCriteriaAndQuestion : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Question", "GraduationCriteriaID", "dbo.GraduationCriteria");
            DropIndex("dbo.Question", "IX_GraduationCriteria_GraduationCriteriaID");
            CreateTable(
                "dbo.GraduationCriteriaQuestions",
                c => new
                    {
                        GraduationCriteriaID = c.Int(nullable: false),
                        QuestionID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.GraduationCriteriaID, t.QuestionID })
                .ForeignKey("dbo.GraduationCriteria", t => t.GraduationCriteriaID, cascadeDelete: true)
                .ForeignKey("dbo.Question", t => t.QuestionID, cascadeDelete: true)
                .Index(t => t.GraduationCriteriaID, name: "IX_GraduationCriteria_GraduationCriteriaID")
                .Index(t => t.QuestionID, name: "IX_Question_QuestionID");
            
            DropColumn("dbo.Question", "GraduationCriteriaID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Question", "GraduationCriteriaID", c => c.Int());
            DropForeignKey("dbo.GraduationCriteriaQuestions", "QuestionID", "dbo.Question");
            DropForeignKey("dbo.GraduationCriteriaQuestions", "GraduationCriteriaID", "dbo.GraduationCriteria");
            DropIndex("dbo.GraduationCriteriaQuestions", "IX_Question_QuestionID");
            DropIndex("dbo.GraduationCriteriaQuestions", "IX_GraduationCriteria_GraduationCriteriaID");
            DropTable("dbo.GraduationCriteriaQuestions");
            CreateIndex("dbo.Question", "GraduationCriteriaID", name: "IX_GraduationCriteria_GraduationCriteriaID");
            AddForeignKey("dbo.Question", "GraduationCriteriaID", "dbo.GraduationCriteria", "GraduationCriteriaID");
        }
    }
}
