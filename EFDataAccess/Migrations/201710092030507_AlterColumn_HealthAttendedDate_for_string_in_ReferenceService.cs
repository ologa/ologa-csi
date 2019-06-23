namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AlterColumn_HealthAttendedDate_for_string_in_ReferenceService : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ReferenceService", "HealthAttendedDate", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ReferenceService", "HealthAttendedDate", c => c.DateTime(nullable: false));
        }
    }
}
