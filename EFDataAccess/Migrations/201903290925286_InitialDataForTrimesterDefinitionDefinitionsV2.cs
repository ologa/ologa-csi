namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialDataForTrimesterDefinitionDefinitionsV2 : DbMigration
    {
        public override void Up()
        {
            Sql(@"DELETE FROM [Trimester]");
            Sql(@"DELETE FROM [TrimesterDefinition]");

            Sql(@"INSERT INTO [TrimesterDefinition] 
                    (CreatedDate, LastUpdatedDate, SyncState, CreatedUserID, LastUpdatedUserID, FirstDay, LastDay, FirstMonth, LastMonth, TrimesterSequence, State) 
                    VALUES (SYSDATETIME(), SYSDATETIME(), 0, 1, 1, 16, 15, 9, 12, 1, 0)");
            Sql(@"INSERT INTO [TrimesterDefinition] 
                    (CreatedDate, LastUpdatedDate, SyncState, CreatedUserID, LastUpdatedUserID, FirstDay, LastDay, FirstMonth, LastMonth, TrimesterSequence, State) 
                    VALUES (SYSDATETIME(), SYSDATETIME(), 0, 1, 1, 16, 15, 12, 3, 2, 0)");
            Sql(@"INSERT INTO [TrimesterDefinition] 
                    (CreatedDate, LastUpdatedDate, SyncState, CreatedUserID, LastUpdatedUserID, FirstDay, LastDay, FirstMonth, LastMonth, TrimesterSequence, State) 
                    VALUES (SYSDATETIME(), SYSDATETIME(), 0, 1, 1, 16, 15, 3, 6, 3, 0)");
            Sql(@"INSERT INTO [TrimesterDefinition] 
                    (CreatedDate, LastUpdatedDate, SyncState, CreatedUserID, LastUpdatedUserID, FirstDay, LastDay, FirstMonth, LastMonth, TrimesterSequence, State) 
                    VALUES (SYSDATETIME(), SYSDATETIME(), 0, 1, 1, 16, 15, 6, 9, 4, 0)");
        }
        
        public override void Down()
        {
        }
    }
}
