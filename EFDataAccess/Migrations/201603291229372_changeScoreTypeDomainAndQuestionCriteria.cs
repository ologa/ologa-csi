namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changeScoreTypeDomainAndQuestionCriteria : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.DomainCriteria", "Score", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.QuestionCriteria", "Score", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.QuestionCriteria", "Score", c => c.Int(nullable: false));
            AlterColumn("dbo.DomainCriteria", "Score", c => c.Int(nullable: false));
        }
    }
}
