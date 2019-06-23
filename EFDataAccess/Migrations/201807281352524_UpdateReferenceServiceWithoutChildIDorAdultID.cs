namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateReferenceServiceWithoutChildIDorAdultID : DbMigration
    {
        public override void Up()
        {
            Sql(@"UPDATE rs SET rs.ChildID = rvm.ChildID 
                FROM ReferenceService rs 
                JOIN RoutineVisitMember rvm ON rvm.routinevisitmemberID = rs.RoutineVisitMemberID
                where rs.childID is null and rs.adultid is null", false);


            Sql(@"UPDATE rs
                SET rs.AdultID = rvm.AdultID
                FROM ReferenceService rs
                JOIN RoutineVisitMember rvm ON rvm.routinevisitmemberID = rs.RoutineVisitMemberID
                where rs.childID is null and rs.adultid is null", false);
        }
        
        public override void Down()
        {
        }
    }
}
