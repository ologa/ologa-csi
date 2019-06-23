namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_Child_and_Adult_To_ReferenceServiceTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ReferenceService", "AdultId", c => c.Int());
            AddColumn("dbo.ReferenceService", "ChildID", c => c.Int());
            Sql(@"UPDATE rs SET rs.ChildID = rvm.ChildID 
                    FROM  [ReferenceService] rs 
                    JOIN  RoutineVisitMember rvm ON rvm.routinevisitmemberID = rs.RoutineVisitMemberID", false);
            Sql(@"UPDATE rs
                    SET rs.AdultID = rvm.AdultID
                    FROM  [ReferenceService] rs
                    JOIN  RoutineVisitMember rvm ON rvm.routinevisitmemberID = rs.RoutineVisitMemberID", false);
        }
        
        public override void Down()
        {
            DropColumn("dbo.ReferenceService", "ChildID");
            DropColumn("dbo.ReferenceService", "AdultId");
        }
    }
}
