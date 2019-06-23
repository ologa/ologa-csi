namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddBeneficiaryGuidOndHIVStatusAndChildStatusHistory : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ChildStatusHistory", "BeneficiaryGuid", c => c.Guid(nullable: false));
            AddColumn("dbo.HIVStatus", "BeneficiaryGuid", c => c.Guid(nullable: false));

            Sql(@"UPDATE HIVStatus SET HIVStatus.BeneficiaryGuid = Child.child_guid from HIVStatus As HIVStatus inner join Child As Child on HIVStatus.ChildID = Child.ChildID
				UPDATE HIVStatus SET HIVStatus.BeneficiaryGuid = Adult.AdultGuid from HIVStatus As HIVStatus inner join Adult As Adult on HIVStatus.AdultID = Adult.AdultID
				UPDATE ChildStatusHistory SET ChildStatusHistory.BeneficiaryGuid = Child.child_guid from ChildStatusHistory As ChildStatusHistory inner join Child As Child on ChildStatusHistory.ChildID = Child.ChildID
				UPDATE ChildStatusHistory SET ChildStatusHistory.BeneficiaryGuid = Adult.AdultGuid from ChildStatusHistory As ChildStatusHistory inner join Adult As Adult on ChildStatusHistory.AdultID = Adult.AdultID");
        }
        
        public override void Down()
        {
            DropColumn("dbo.HIVStatus", "BeneficiaryGuid");
            DropColumn("dbo.ChildStatusHistory", "BeneficiaryGuid");
        }
    }
}
