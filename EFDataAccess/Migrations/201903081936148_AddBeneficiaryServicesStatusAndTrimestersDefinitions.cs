namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddBeneficiaryServicesStatusAndTrimestersDefinitions : DbMigration
    {
        public override void Up()
        {
            // ADD BENEFICIARY 
            Sql(@"INSERT INTO [simpleentity] 
                    (Type,Code,Description,CreatedDate,LastUpdatedDate,CreatedUserID,LastUpdatedUserID,State,SyncState)
                    VALUES ('ben-services-status','01','Inactivo',SYSDATETIME(),SYSDATETIME(),1,1,0,0)");

            Sql(@"INSERT INTO [simpleentity] 
                    (Type,Code,Description,CreatedDate,LastUpdatedDate,CreatedUserID,LastUpdatedUserID,State,SyncState)
                    VALUES ('ben-services-status','02','Activo Laranja',SYSDATETIME(),SYSDATETIME(),1,1,0,0)");

            Sql(@"INSERT INTO [simpleentity] 
                    (Type,Code,Description,CreatedDate,LastUpdatedDate,CreatedUserID,LastUpdatedUserID,State,SyncState)
                    VALUES ('ben-services-status','03','Activo Verde',SYSDATETIME(),SYSDATETIME(),1,1,0,0)");

            // ADD TRIMESTERS
            Sql(@"INSERT INTO [Trimester] 
                    (CreatedDate, LastUpdatedDate, SyncState, CreatedUserID, LastUpdatedUserID, FirstDay, LastDay, FirstMonth, LastMonth, TrimesterSequence, State) 
                    VALUES (SYSDATETIME(), SYSDATETIME(), 0, 1, 1, 16, 15, 9, 12, 1, 0)");
            Sql(@"INSERT INTO [Trimester] 
                    (CreatedDate, LastUpdatedDate, SyncState, CreatedUserID, LastUpdatedUserID, FirstDay, LastDay, FirstMonth, LastMonth, TrimesterSequence, State) 
                    VALUES (SYSDATETIME(), SYSDATETIME(), 0, 1, 1, 16, 15, 12, 3, 2, 0)");
            Sql(@"INSERT INTO [Trimester] 
                    (CreatedDate, LastUpdatedDate, SyncState, CreatedUserID, LastUpdatedUserID, FirstDay, LastDay, FirstMonth, LastMonth, TrimesterSequence, State) 
                    VALUES (SYSDATETIME(), SYSDATETIME(), 0, 1, 1, 16, 15, 3, 6, 3, 0)");
            Sql(@"INSERT INTO [Trimester] 
                    (CreatedDate, LastUpdatedDate, SyncState, CreatedUserID, LastUpdatedUserID, FirstDay, LastDay, FirstMonth, LastMonth, TrimesterSequence, State) 
                    VALUES (SYSDATETIME(), SYSDATETIME(), 0, 1, 1, 16, 15, 6, 9, 4, 0)");
        }
        
        public override void Down()
        {
        }
    }
}
