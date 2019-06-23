namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class HIV_SavingGroup_Fields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Adult", "IsPartSavingGroup", c => c.Boolean(nullable: false));
            AddColumn("dbo.Adult", "HIVInTreatment", c => c.Int(nullable: false));
            AddColumn("dbo.Adult", "HIVUndisclosedReason", c => c.Int(nullable: false));
            AddColumn("dbo.Child", "IsPartSavingGroup", c => c.Boolean(nullable: false));
            AddColumn("dbo.Child", "HIVInTreatment", c => c.Int(nullable: false));
            AddColumn("dbo.Child", "HIVUndisclosedReason", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Child", "HIVUndisclosedReason");
            DropColumn("dbo.Child", "HIVInTreatment");
            DropColumn("dbo.Child", "IsPartSavingGroup");
            DropColumn("dbo.Adult", "HIVUndisclosedReason");
            DropColumn("dbo.Adult", "HIVInTreatment");
            DropColumn("dbo.Adult", "IsPartSavingGroup");
        }
    }
}
