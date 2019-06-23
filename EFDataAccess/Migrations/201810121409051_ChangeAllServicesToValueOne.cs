namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeAllServicesToValueOne : DbMigration
    {
        public override void Up()
        {
            Sql(@"UPDATE rvs
                SET rvs.SupportValue = '1'
                FROM [CSI_PROD].[dbo].[RoutineVisitSupport] rvs
                WHERE rvs.SupportType in ('Domain','DPI','MUAC')
                AND rvs.SupportValue not in ('0','1')", false);
        }
        
        public override void Down()
        {
        }
    }
}
