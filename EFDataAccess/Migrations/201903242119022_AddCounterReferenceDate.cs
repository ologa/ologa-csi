namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCounterReferenceDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ReferenceService", "CounterReferenceDate", c => c.DateTime());
            Sql(@"UPDATE rs
                SET rs.CounterReferenceDate = rs.HealthAttendedDate
                FROM ReferenceService rs
                where HealthAttendedDate is not null 
                and SocialAttendedDate is not null");

            Sql(@"UPDATE rs
                SET rs.CounterReferenceDate = rs.HealthAttendedDate
                FROM ReferenceService rs
                where rs.HealthAttendedDate is not null 
                and rs.SocialAttendedDate is null");

            Sql(@"UPDATE rs
                SET rs.CounterReferenceDate = rs.SocialAttendedDate
                FROM ReferenceService rs
                where rs.HealthAttendedDate is null 
                and rs.SocialAttendedDate is not null");
        }
        
        public override void Down()
        {
            DropColumn("dbo.ReferenceService", "CounterReferenceDate");
        }
    }
}
