namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddNewHouseholdStatus_And_InsertAllHouseholdsWithInitialState : DbMigration
    {
        public override void Up()
        {

            Sql(@"INSERT INTO[dbo].[HouseholdStatus]
            ([Description], [HouseholdStatus_guid], [CreatedUserID], [LastUpdatedUserID],
            [State], [CreatedDate], [LastUpdatedDate], [SyncState], [SyncDate], [SyncGuid])
            VALUES('Inicial', NEWID(), 1, null, 0, GETDATE(), null, 0, null, null)");

            Sql(@"INSERT INTO[dbo].[HouseholdStatus]
            ([Description], [HouseholdStatus_guid], [CreatedUserID], [LastUpdatedUserID],
            [State], [CreatedDate], [LastUpdatedDate], [SyncState], [SyncDate], [SyncGuid])
            VALUES('Graduação', NEWID(), 1, null, 0, GETDATE(), null, 0, null, null)");

            Sql(@"INSERT INTO[dbo].[HouseholdStatus]
            ([Description], [HouseholdStatus_guid], [CreatedUserID], [LastUpdatedUserID],
            [State], [CreatedDate], [LastUpdatedDate], [SyncState], [SyncDate], [SyncGuid])
            VALUES('Transferência', NEWID(), 1, null, 0, GETDATE(), null, 0, null, null)");

            Sql(@"INSERT INTO[dbo].[HouseholdStatus]
            ([Description], [HouseholdStatus_guid], [CreatedUserID], [LastUpdatedUserID],
            [State], [CreatedDate], [LastUpdatedDate], [SyncState], [SyncDate], [SyncGuid])
            VALUES('Inativo - Desistência', NEWID(), 1, null, 0, GETDATE(), null, 0, null, null)");

            Sql(@"INSERT INTO[dbo].[HouseholdStatus]
            ([Description], [HouseholdStatus_guid], [CreatedUserID], [LastUpdatedUserID],
            [State], [CreatedDate], [LastUpdatedDate], [SyncState], [SyncDate], [SyncGuid])
            VALUES('Inativo - Perdido', NEWID(), 1, null, 0, GETDATE(), null, 0, null, null)");

            Sql(@"INSERT INTO[dbo].[HouseholdStatus]
            ([Description], [HouseholdStatus_guid], [CreatedUserID], [LastUpdatedUserID],
            [State], [CreatedDate], [LastUpdatedDate], [SyncState], [SyncDate], [SyncGuid])
            VALUES('Óbito', NEWID(), 1, null, 0, GETDATE(), null, 0, null, null)");

            Sql(@"INSERT INTO [dbo].[HouseholdStatusHistory] 
            ([RegistrationDate] ,[HouseholdID] ,[HouseholdStatusID] 
            ,[HouseholdStatusHistory_Guid] ,[CreatedUserID] ,[LastUpdatedUserID] 
            ,[State] ,[CreatedDate] ,[LastUpdatedDate] ,[SyncState] ,[SyncDate] ,[SyncGuid])
            SELECT hh.RegistrationDate, hh.HouseholdID, (SELECT hh.HouseholdStatusID 
            FROM [dbo].[HouseholdStatus] hh WHERE hh.Description = 'Inicial'), NEWID(), 1, NULL, 0, GETDATE(), null, 0, null, null FROM [dbo].[Household] hh");

        }

        public override void Down()
        {
        }
    }
}
