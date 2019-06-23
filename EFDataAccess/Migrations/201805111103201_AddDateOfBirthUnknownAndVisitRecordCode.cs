namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDateOfBirthUnknownAndVisitRecordCode : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Adult", "DateOfBirthUnknown", c => c.Boolean(nullable: false));
            AddColumn("dbo.RoutineVisit", "VisitRecordCode", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.RoutineVisit", "VisitRecordCode");
            DropColumn("dbo.Adult", "DateOfBirthUnknown");
        }
    }
}
