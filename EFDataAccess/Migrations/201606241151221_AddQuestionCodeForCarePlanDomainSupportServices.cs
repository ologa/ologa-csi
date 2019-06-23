namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddQuestionCodeForCarePlanDomainSupportServices : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CarePlanDomainSupportService", "QuestionCode", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.CarePlanDomainSupportService", "QuestionCode");
        }
    }
}
