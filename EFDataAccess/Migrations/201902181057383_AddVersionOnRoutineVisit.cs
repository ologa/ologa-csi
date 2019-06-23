namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddVersionOnRoutineVisit : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RoutineVisit", "Version", c => c.String());

            Sql(@"Update RoutineVisit Set [Version] = 'v1'");
        }
        
        public override void Down()
        {
            DropColumn("dbo.RoutineVisit", "Version");
        }
    }
}
