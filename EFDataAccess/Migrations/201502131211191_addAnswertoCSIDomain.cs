namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addAnswertoCSIDomain : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CSIDomainScore", "AnswerID", c => c.Int());
            CreateIndex("dbo.CSIDomainScore", "AnswerID", name: "IX_Answer_AnswerID");
            AddForeignKey("dbo.CSIDomainScore", "AnswerID", "dbo.Answer", "AnswerID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CSIDomainScore", "AnswerID", "dbo.Answer");
            DropIndex("dbo.CSIDomainScore", "IX_Answer_AnswerID");
            DropColumn("dbo.CSIDomainScore", "AnswerID");
        }
    }
}
