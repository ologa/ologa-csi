namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddRoutineVisitDateAndBeneficiaryHasServicesOnRoutineVisitMember : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RoutineVisitMember", "RoutineVisitDate", c => c.DateTime());
            AddColumn("dbo.RoutineVisitMember", "BeneficiaryHasServices", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RoutineVisitMember", "BeneficiaryHasServices");
            DropColumn("dbo.RoutineVisitMember", "RoutineVisitDate");
        }
    }
}
