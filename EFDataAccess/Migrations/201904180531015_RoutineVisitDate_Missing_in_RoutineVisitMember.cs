namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RoutineVisitDate_Missing_in_RoutineVisitMember : DbMigration
    {
        public override void Up()
        {
            Sql(@"UPDATE rvm
                SET rvm.RoutineVisitDate = rv.RoutineVisitDate
                FROM RoutineVisit rv
                JOIN RoutineVisitMember rvm ON rvm.RoutineVisitID = rv.RoutineVisitID
                JOIN RoutineVisitSupport rvs ON rvs.RoutineVisitMemberID = rvm.RoutineVisitMemberID 
                AND rvm.RoutineVisitDate IS NULL");
        }
        
        public override void Down()
        {
        }
    }
}
