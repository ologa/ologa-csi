namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_FiscalYearVersion_To_TrimesterDefinition : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TrimesterDefinition", "FiscalYearVersion", c => c.Int(nullable: false));

            //DELETE TABLE Trimester
            Sql("DELETE FROM Trimester", false);

            // UPDATE ALL EXISTING ROWS TO VERSION 1 TRIMESTER DEFINITIONS
            Sql(@"UPDATE [TrimesterDefinition] 
	            SET FiscalYearVersion = 1, FirstDay = 21, LastDay = 20");

            // INSERT VERSION 2 TRIMESTER DEFINITIONS
            Sql(@"INSERT INTO [TrimesterDefinition] 
	            (CreatedDate, LastUpdatedDate, SyncState, CreatedUserID, LastUpdatedUserID, FirstDay, LastDay, FirstMonth, LastMonth, TrimesterSequence, State, FiscalYearVersion) 
	            VALUES (SYSDATETIME(), SYSDATETIME(), 0, 1, 1, 21, 15, 9, 12, 1, 0, 2)");

            Sql(@"INSERT INTO [TrimesterDefinition] 
	            (CreatedDate, LastUpdatedDate, SyncState, CreatedUserID, LastUpdatedUserID, FirstDay, LastDay, FirstMonth, LastMonth, TrimesterSequence, State, FiscalYearVersion) 
	            VALUES (SYSDATETIME(), SYSDATETIME(), 0, 1, 1, 16, 15, 12, 3, 2, 0, 2)");

            Sql(@"INSERT INTO [TrimesterDefinition] 
	            (CreatedDate, LastUpdatedDate, SyncState, CreatedUserID, LastUpdatedUserID, FirstDay, LastDay, FirstMonth, LastMonth, TrimesterSequence, State, FiscalYearVersion) 
	            VALUES (SYSDATETIME(), SYSDATETIME(), 0, 1, 1, 16, 15, 3, 6, 3, 0, 2)");

            Sql(@"INSERT INTO [TrimesterDefinition] 
	            (CreatedDate, LastUpdatedDate, SyncState, CreatedUserID, LastUpdatedUserID, FirstDay, LastDay, FirstMonth, LastMonth, TrimesterSequence, State, FiscalYearVersion) 
	            VALUES (SYSDATETIME(), SYSDATETIME(), 0, 1, 1, 16, 15, 6, 9, 4, 0, 2)");

            // INSERT VERSION 3 TRIMESTER DEFINITIONS
            Sql(@"INSERT INTO [TrimesterDefinition] 
	            (CreatedDate, LastUpdatedDate, SyncState, CreatedUserID, LastUpdatedUserID, FirstDay, LastDay, FirstMonth, LastMonth, TrimesterSequence, State, FiscalYearVersion) 
	            VALUES (SYSDATETIME(), SYSDATETIME(), 0, 1, 1, 16, 15, 9, 12, 1, 0, 3)");

            Sql(@"INSERT INTO [TrimesterDefinition] 
	            (CreatedDate, LastUpdatedDate, SyncState, CreatedUserID, LastUpdatedUserID, FirstDay, LastDay, FirstMonth, LastMonth, TrimesterSequence, State, FiscalYearVersion) 
	            VALUES (SYSDATETIME(), SYSDATETIME(), 0, 1, 1, 16, 15, 12, 3, 2, 0, 3)");

            Sql(@"INSERT INTO [TrimesterDefinition] 
	            (CreatedDate, LastUpdatedDate, SyncState, CreatedUserID, LastUpdatedUserID, FirstDay, LastDay, FirstMonth, LastMonth, TrimesterSequence, State, FiscalYearVersion) 
	            VALUES (SYSDATETIME(), SYSDATETIME(), 0, 1, 1, 16, 15, 3, 6, 3, 0, 3)");

            Sql(@"INSERT INTO [TrimesterDefinition] 
	            (CreatedDate, LastUpdatedDate, SyncState, CreatedUserID, LastUpdatedUserID, FirstDay, LastDay, FirstMonth, LastMonth, TrimesterSequence, State, FiscalYearVersion) 
	            VALUES (SYSDATETIME(), SYSDATETIME(), 0, 1, 1, 16, 15, 6, 9, 4, 0, 3)");
        }
        
        public override void Down()
        {
            DropColumn("dbo.TrimesterDefinition", "FiscalYearVersion");
        }
    }
}
