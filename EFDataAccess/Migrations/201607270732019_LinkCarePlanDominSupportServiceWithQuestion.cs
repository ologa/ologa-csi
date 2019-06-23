namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LinkCarePlanDominSupportServiceWithQuestion : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CarePlanDomainSupportService", "AnswerID", c => c.Int());
            AddColumn("dbo.CarePlanDomainSupportService", "QuestionID", c => c.Int());
            CreateIndex("dbo.CarePlanDomainSupportService", "AnswerID", name: "IX_Answer_AnswerID");
            CreateIndex("dbo.CarePlanDomainSupportService", "QuestionID", name: "IX_Question_QuestionID");
            AddForeignKey("dbo.CarePlanDomainSupportService", "AnswerID", "dbo.Answer", "AnswerID");
            AddForeignKey("dbo.CarePlanDomainSupportService", "QuestionID", "dbo.Question", "QuestionID");
            DropColumn("dbo.CarePlanDomainSupportService", "QuestionCode");
        }
        
        public override void Down()
        {
            AddColumn("dbo.CarePlanDomainSupportService", "QuestionCode", c => c.String());
            DropForeignKey("dbo.CarePlanDomainSupportService", "QuestionID", "dbo.Question");
            DropForeignKey("dbo.CarePlanDomainSupportService", "AnswerID", "dbo.Answer");
            DropIndex("dbo.CarePlanDomainSupportService", "IX_Question_QuestionID");
            DropIndex("dbo.CarePlanDomainSupportService", "IX_Answer_AnswerID");
            DropColumn("dbo.CarePlanDomainSupportService", "QuestionID");
            DropColumn("dbo.CarePlanDomainSupportService", "AnswerID");
        }
    }
}
