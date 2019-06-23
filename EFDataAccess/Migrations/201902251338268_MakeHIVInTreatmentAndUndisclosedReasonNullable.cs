namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MakeHIVInTreatmentAndUndisclosedReasonNullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.HIVStatus", "HIVInTreatment", c => c.Int());
            AlterColumn("dbo.HIVStatus", "HIVUndisclosedReason", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.HIVStatus", "HIVUndisclosedReason", c => c.Int(nullable: false));
            AlterColumn("dbo.HIVStatus", "HIVInTreatment", c => c.Int(nullable: false));
        }
    }
}
