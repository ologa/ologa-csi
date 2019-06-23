namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Change_SocialAttendedDate_To_DateFormat_and_change_HealthAttendedDate_VBDate_to_NULL : DbMigration
    {
        public override void Up()
        {
            //INSERT NULL IN BLACK ROWS
            Sql(@"UPDATE rs
	        SET rs.SocialAttendedDate = NULL
	        FROM  [ReferenceService] rs
	        WHERE [SocialAttendedDate] = ''", false);

            //UPDATE DIFFERENT VARCHAR DATES TO DATEFORMAT
            Sql(@"UPDATE rs
                SET rs.SocialAttendedDate = CONVERT(datetime, rs.SocialAttendedDate,111)
                FROM  [ReferenceService] rs
                WHERE rs.[SocialAttendedDate] IS NOT NULL
                AND SUBSTRING(rs.SocialAttendedDate, 5, 1) = '/'

                UPDATE rs
                SET rs.SocialAttendedDate = CONVERT(datetime, rs.SocialAttendedDate,120)
                FROM  [ReferenceService] rs
                WHERE rs.[SocialAttendedDate] IS NOT NULL
                AND SUBSTRING(rs.SocialAttendedDate, 5, 1) = '-'

                UPDATE rs
                SET rs.SocialAttendedDate = CONVERT(datetime, rs.SocialAttendedDate,105)
                FROM  [ReferenceService] rs
                WHERE rs.[SocialAttendedDate] IS NOT NULL
                AND SUBSTRING(rs.SocialAttendedDate, 6, 1) = '-'

                UPDATE rs
                SET rs.SocialAttendedDate = CONVERT(datetime, rs.SocialAttendedDate,103)
                FROM  [ReferenceService] rs
                WHERE rs.[SocialAttendedDate] IS NOT NULL
                AND SUBSTRING(rs.SocialAttendedDate, 6, 1) = '/'", false);

            //AlTER STRING COLUMN TO DATETIME
            AlterColumn("dbo.ReferenceService", "SocialAttendedDate", c => c.DateTime());

            //UPDATE (dd mon yyyy hh:mm:ss:mmm) DATEFORMAT TO (yyyy-mm-dd hh:mm:ss) DATEFORMAT
            Sql(@"UPDATE rs
	        SET rs.SocialAttendedDate = FORMAT(cast(rs.SocialAttendedDate as datetime),'yyyy-MM-dd hh:mm:ss')
	        FROM  [ReferenceService] rs
	        WHERE rs.[SocialAttendedDate] IS NOT NULL", false);

            //UPDATE HEALTHATTENDEDDATE MIN VB.DATE(1753-01-01 00:00:00.000) TO NULL
            Sql(@"UPDATE rs
		    SET rs.HealthAttendedDate = NULL
		    FROM  [ReferenceService] rs
		    WHERE rs.HealthAttendedDate='1753-01-01 00:00:00.000'", false);
        }

        public override void Down()
        {
            AlterColumn("dbo.ReferenceService", "SocialAttendedDate", c => c.String());
        }
    }
}
