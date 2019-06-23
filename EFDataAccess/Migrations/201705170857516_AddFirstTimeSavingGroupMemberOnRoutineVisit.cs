namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFirstTimeSavingGroupMemberOnRoutineVisit : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RoutineVisit", "FirstTimeSavingGroupMember", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.RoutineVisit", "FirstTimeSavingGroupMember");
        }
    }
}
