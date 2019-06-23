namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSupportServiceFields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SupportServiceType", "TypeCode", c => c.String());
            AddColumn("dbo.SupportServiceType", "Tool", c => c.String());
            AddColumn("dbo.RoutineVisitSupport", "Checked", c => c.Boolean(nullable: false));
            AlterColumn("dbo.SupportServiceType", "Description", c => c.String(nullable: false, maxLength: 250, unicode: false));
            AlterColumn("dbo.RoutineVisitSupport", "SupportID", c => c.Int());
            CreateIndex("dbo.RoutineVisitSupport", "SupportID");
            AddForeignKey("dbo.RoutineVisitSupport", "SupportID", "dbo.SupportServiceType", "SupportServiceTypeID");
        }

        public override void Down()
        {
            DropForeignKey("dbo.RoutineVisitSupport", "SupportID", "dbo.SupportServiceType");
            DropIndex("dbo.RoutineVisitSupport", new[] { "SupportID" });
            AlterColumn("dbo.RoutineVisitSupport", "SupportID", c => c.Int(nullable: false));
            AlterColumn("dbo.SupportServiceType", "Description", c => c.String(nullable: false, maxLength: 50, unicode: false));
            DropColumn("dbo.RoutineVisitSupport", "Checked");
            DropColumn("dbo.SupportServiceType", "Tool");
            DropColumn("dbo.SupportServiceType", "TypeCode");
        }
    }
}
