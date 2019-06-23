namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeAnswerIndexOnCSIDomainScore : DbMigration
    {
        public override void Up()
        {
            RenameIndex(table: "dbo.CSIDomainScore", name: "IX_Answer_AnswerID", newName: "IX_AnswerID");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.CSIDomainScore", name: "IX_AnswerID", newName: "IX_Answer_AnswerID");
        }
    }
}
