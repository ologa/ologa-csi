namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateSyncState_To_NeverSynced_And_LastUpdatedDate_on_HouseholdStatusHistory : DbMigration
    {
        public override void Up()
        {
            Sql(@"UPDATE hsh
                SET hsh.SyncState = -1, hsh.LastUpdatedDate = hsh.CreatedDate
                FROM[CSI_PROD].[dbo].[HouseholdStatusHistory] hsh");
        }
        
        public override void Down()
        {
        }
    }
}
