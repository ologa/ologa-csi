namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixRoutineVisitFalseValuesInDomainEtities : DbMigration
    {
        public override void Up()
        {
            Sql(@"UPDATE rvs
                SET rvs.SupportValue = '1'
                FROM [RoutineVisitSupport] rvs
                WHERE rvs.[SupportType] IN('Domain', 'DomainEntity')
                AND SupportValue NOT IN('0','1')
                AND SupportValue = 'True'");

            Sql(@"UPDATE rvs
                SET rvs.SupportValue = '0'
                FROM [RoutineVisitSupport] rvs
                WHERE rvs.[SupportType] IN('Domain', 'DomainEntity')
                AND SupportValue NOT IN('0','1')
                AND SupportValue = 'False'");
        }
        
        public override void Down()
        {
        }
    }
}
