namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AlterHealthAttendedDateType : DbMigration
    {
        public override void Up()
        {
            Sql(@"ALTER TABLE  [ReferenceService] DROP COLUMN HealthAttendedDate", false);
            Sql(@"ALTER TABLE  [ReferenceService] ADD HealthAttendedDate datetime NULL", false);
        }
        
        public override void Down()
        {
            // AlterColumn("dbo.ReferenceService", "HealthAttendedDate", c => c.String());
        }
    }
}
