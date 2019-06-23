namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeHealthAttendedDateType : DbMigration
    {
        public override void Up()
        {
            Sql(@"ALTER TABLE  [ReferenceService] ADD HAD datetime NULL", false);
            Sql(@"UPDATE  [ReferenceService] 
                SET HAD = PARSE(SUBSTRING(HealthAttendedDate,4,2)+'/'+SUBSTRING(HealthAttendedDate,1,2)+'/'+SUBSTRING(HealthAttendedDate,7,4) AS datetime USING 'en-US') 
                WHERE ISDATE(SUBSTRING(HealthAttendedDate,4,2)+'/'+SUBSTRING(HealthAttendedDate,1,2)+'/'+SUBSTRING(HealthAttendedDate,7,4)) = 1", false);
            Sql(@"ALTER TABLE  [ReferenceService] DROP COLUMN HealthAttendedDate", false);
            Sql(@"ALTER TABLE  [ReferenceService] ADD HealthAttendedDate datetime NULL", false);
            Sql(@"UPDATE  [ReferenceService] SET HealthAttendedDate = HAD", false);
            Sql(@"ALTER TABLE  [ReferenceService] DROP COLUMN HAD", false);
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ReferenceService", "HealthAttendedDate", c => c.String());
        }
    }
}
