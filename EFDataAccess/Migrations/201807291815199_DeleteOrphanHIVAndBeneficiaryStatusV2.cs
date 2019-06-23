namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeleteOrphanHIVAndBeneficiaryStatusV2 : DbMigration
    {
        public override void Up()
        {
            //if (System.Configuration.ConfigurationManager.AppSettings["AppVersion"].Contains("desktop"))
            //{
                Sql(@"Delete From HIVStatus where HIVStatusID  in
                (Select h.HIVStatusID from HIVStatus h where
                h.ChildID not in (Select ChildID from Child) 
				and h.AdultID not in (Select AdultID from Adult)
				and h.HIVStatusID not in (Select HIVStatusID from Child)
				and h.HIVStatusID not in (Select HIVStatusID from Adult))");

                Sql(@"Delete From ChildStatusHistory where ChildStatusHistoryID in
                (Select csh.ChildStatusHistoryID from ChildStatusHistory csh where
                (ChildID is NULL and AdultID is NULL) or
                (ChildID not in (Select ChildID from Child) or AdultID not in (Select AdultID from Adult)))");
            //}
        }
        
        public override void Down()
        {
        }
    }
}
