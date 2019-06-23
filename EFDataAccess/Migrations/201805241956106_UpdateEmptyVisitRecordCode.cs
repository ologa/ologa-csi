namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateEmptyVisitRecordCode : DbMigration
    {
        public override void Up()
        {
            Sql(@"UPDATE RoutineVisit SET VisitRecordCode = CONCAT(MONTH(RoutineVisitDate),'/', YEAR(RoutineVisitDate))");
        }

        public override void Down()
        {
        }
    }
}
