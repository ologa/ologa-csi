namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRoutineVisitLog : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RoutineVisitLog",
                c => new
                    {
                        RoutineVisitLogID = c.Int(nullable: false, identity: true),
                        VisitDate = c.DateTime(nullable: false),
                        ChildID = c.Int(nullable: false),
                        DomainID = c.Int(nullable: false),
                        ReturnVisitID = c.Int(nullable: false),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.RoutineVisitLogID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.RoutineVisitLog");
        }
    }
}
