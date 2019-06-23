namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Status_for_Old_Adults : DbMigration
    {
        public override void Up()
        {
            Sql(@"INSERT INTO  [ChildStatusHistory] 
                    (
                        [EffectiveDate] ,[CreatedDate] ,[ChildStatusID] ,[CreatedUserID] ,[ChildID] , [AdultID]
                    )
                    SELECT hh.RegistrationDate, adult.CreatedDate, 6, adult.CreatedUserID, NULL, adult.AdultId
                    FROM [Adult] adult
                    JOIN household hh ON adult.HouseholdId = hh.householdId", false);
        }
        
        public override void Down()
        {
            Sql(@"DELETE FROM  [ChildStatusHistory]
                    WHERE AdultID > 0", false);

        }
    }
}
