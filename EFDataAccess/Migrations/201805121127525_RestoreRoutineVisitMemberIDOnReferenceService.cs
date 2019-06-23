namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RestoreRoutineVisitMemberIDOnReferenceService : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ReferenceService", "RoutineVisitMemberID", c => c.Int(nullable: true));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ReferenceService", "RoutineVisitMemberID", c => c.Int(nullable: false));
        }
    }
}
