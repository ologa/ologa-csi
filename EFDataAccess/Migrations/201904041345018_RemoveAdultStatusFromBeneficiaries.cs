namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveAdultStatusFromBeneficiaries : DbMigration
    {
        public override void Up()
        {
            Sql(@"UPDATE csh
                SET csh.BeneficiaryID = NULL
                FROM [ChildStatusHistory] csh
                WHERE csh.childStatusID = 5
                AND BeneficiaryID is not null");
        }
        
        public override void Down()
        {
        }
    }
}
