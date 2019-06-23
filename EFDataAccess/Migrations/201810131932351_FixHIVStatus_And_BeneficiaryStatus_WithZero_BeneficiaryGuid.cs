namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixHIVStatus_And_BeneficiaryStatus_WithZero_BeneficiaryGuid : DbMigration
    {
        public override void Up()
        {
            //Update HIVStatus of Children with the correct BeneficiaryGuid
            Sql(@"UPDATE h
            SET h.BeneficiaryGuid = c.child_guid
            FROM  [HIVStatus] h
            JOIN  [Child] c ON c.ChildID = h.ChildID
            where h.BeneficiaryGuid = '00000000-0000-0000-0000-000000000000'", false);

            //Update HIVStatus of Adults with the correct BeneficiaryGuid
            Sql(@"UPDATE h
            SET h.BeneficiaryGuid = a.AdultGuid
            FROM  [HIVStatus] h
            JOIN  [Adult] a ON a.AdultID = h.AdultID
            where h.BeneficiaryGuid = '00000000-0000-0000-0000-000000000000'", false);

            //Update Status of Children with the correct BeneficiaryGuid
            Sql(@"UPDATE csh
            SET csh.BeneficiaryGuid = c.child_guid
            FROM  [ChildStatusHistory] csh
            JOIN  [Child] c ON c.ChildID = csh.ChildID
            where csh.BeneficiaryGuid = '00000000-0000-0000-0000-000000000000'", false);

            //Update Status of Adults with the correct BeneficiaryGuid
            Sql(@"UPDATE csh
            SET csh.BeneficiaryGuid = a.AdultGuid
            FROM  [ChildStatusHistory] csh
            JOIN  [Adult] a ON a.AdultID = csh.AdultID
            where csh.BeneficiaryGuid = '00000000-0000-0000-0000-000000000000'", false);

            //Delete HIVStatus without Beneficiaries
            Sql(@"Delete From HIVStatus where HIVStatusID  in
            (Select h.HIVStatusID from HIVStatus h where
            h.ChildID not in (Select ChildID from Child) 
            and h.AdultID not in (Select AdultID from Adult)
            and h.HIVStatusID not in (Select HIVStatusID from Child)
            and h.HIVStatusID not in (Select HIVStatusID from Adult))", false);

            //Delete Status without Beneficiaries
            Sql(@"Delete From ChildStatusHistory where ChildStatusHistoryID in
            (Select csh.ChildStatusHistoryID from ChildStatusHistory csh where
            (ChildID is NULL and AdultID is NULL) or
            (ChildID not in (Select ChildID from Child) or AdultID not in (Select AdultID from Adult)))", false);
        }
        
        public override void Down()
        {
        }
    }
}
