namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIDsAndIndexesOnEntities : DbMigration
    {
        public override void Up()
        {
            RenameIndex(table: "dbo.CarePlanDomain", name: "IX_CarePlan_CarePlanID", newName: "IX_CarePlanID");
            RenameIndex(table: "dbo.CarePlanDomain", name: "IX_Domain_DomainID", newName: "IX_DomainID");
            RenameIndex(table: "dbo.CarePlanDomainSupportService", name: "IX_Question_QuestionID", newName: "IX_QuestionID");
            RenameIndex(table: "dbo.CarePlanDomainSupportService", name: "IX_Answer_AnswerID", newName: "IX_AnswerID");
            RenameIndex(table: "dbo.CarePlanDomainSupportService", name: "IX_CarePlanDomain_CarePlanDomainID", newName: "IX_CarePlanDomainID");
            RenameIndex(table: "dbo.Tasks", name: "IX_Resource_ResourceID", newName: "IX_ResourceID");
            RenameIndex(table: "dbo.Tasks", name: "IX_CarePlanDomainSupportService_CarePlanDomainSupportServiceID", newName: "IX_CarePlanDomainSupportServiceID");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Tasks", name: "IX_CarePlanDomainSupportServiceID", newName: "IX_CarePlanDomainSupportService_CarePlanDomainSupportServiceID");
            RenameIndex(table: "dbo.Tasks", name: "IX_ResourceID", newName: "IX_Resource_ResourceID");
            RenameIndex(table: "dbo.CarePlanDomainSupportService", name: "IX_CarePlanDomainID", newName: "IX_CarePlanDomain_CarePlanDomainID");
            RenameIndex(table: "dbo.CarePlanDomainSupportService", name: "IX_AnswerID", newName: "IX_Answer_AnswerID");
            RenameIndex(table: "dbo.CarePlanDomainSupportService", name: "IX_QuestionID", newName: "IX_Question_QuestionID");
            RenameIndex(table: "dbo.CarePlanDomain", name: "IX_DomainID", newName: "IX_Domain_DomainID");
            RenameIndex(table: "dbo.CarePlanDomain", name: "IX_CarePlanID", newName: "IX_CarePlan_CarePlanID");
        }
    }
}
