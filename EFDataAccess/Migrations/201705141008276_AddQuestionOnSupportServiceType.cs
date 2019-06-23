namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddQuestionOnSupportServiceType : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SupportServiceType", "QuestionID", c => c.Int());
            CreateIndex("dbo.SupportServiceType", "QuestionID", name: "IX_Question_QuestionID");
            AddForeignKey("dbo.SupportServiceType", "QuestionID", "dbo.Question", "QuestionID");

            Sql(@"UPDATE [ChildStatus] SET Description = 'Inicial' WHERE Description = 'Intensivo'", false);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SupportServiceType", "QuestionID", "dbo.Question");
            DropIndex("dbo.SupportServiceType", "IX_Question_QuestionID");
            DropColumn("dbo.SupportServiceType", "QuestionID");
        }
    }
}
