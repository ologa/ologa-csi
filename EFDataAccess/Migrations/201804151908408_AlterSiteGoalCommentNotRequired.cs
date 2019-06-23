namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AlterSiteGoalCommentNotRequired : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.SiteGoal", "SitePerformanceComment", c => c.String(maxLength: 250));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.SiteGoal", "SitePerformanceComment", c => c.String(nullable: false, maxLength: 250));
        }
    }
}
