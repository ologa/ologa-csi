namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddNewBeneficiaryStatus1 : DbMigration
    {
        public override void Up()
        {
            Sql(@"INSERT INTO  [ChildStatus] (Description,[CreatedDate], [LastUpdatedDate], [SyncDate], [SyncGuid], [CreatedUserID], [LastUpdatedUserID], state,SyncState) 
            VALUES ('Eliminado - Sem Serviços',GETDATE(), GETDATE(), NULL, NULL, 1, 1, 0, 0)");

            Sql(@"INSERT INTO  [ChildStatus] (Description,[CreatedDate], [LastUpdatedDate], [SyncDate], [SyncGuid], [CreatedUserID], [LastUpdatedUserID], state, SyncState) 
            VALUES ('Eliminado - Transferido', GETDATE(), GETDATE(), NULL, NULL, 1, 1, 0, 0)");

            Sql(@"INSERT INTO  [ChildStatus] (Description,[CreatedDate], [LastUpdatedDate], [SyncDate], [SyncGuid], [CreatedUserID], [LastUpdatedUserID], state,SyncState) 
            VALUES ('Eliminado - Graduação', GETDATE(), GETDATE(), NULL, NULL, 1, 1, 0, 0)");

            Sql(@"INSERT INTO  [ChildStatus] (Description,[CreatedDate], [LastUpdatedDate], [SyncDate], [SyncGuid], [CreatedUserID], [LastUpdatedUserID], state,SyncState) 
            VALUES ('Eliminado - Desistência',GETDATE(), GETDATE(), NULL, NULL, 1, 1, 0, 0)");

            Sql(@"INSERT INTO  [ChildStatus] (Description,[CreatedDate], [LastUpdatedDate], [SyncDate], [SyncGuid], [CreatedUserID], [LastUpdatedUserID], state,SyncState) 
            VALUES ('Eliminado - Perdido', GETDATE(), GETDATE(), NULL, NULL, 1, 1, 0, 0)");

            Sql(@"INSERT INTO  [ChildStatus] (Description,[CreatedDate], [LastUpdatedDate], [SyncDate], [SyncGuid], [CreatedUserID], [LastUpdatedUserID], state, SyncState) 
            VALUES ('Eliminado - Óbito', GETDATE(), GETDATE(), NULL, NULL, 1, 1, 0, 0)");

            Sql(@"INSERT INTO  [ChildStatus] (Description,[CreatedDate], [LastUpdatedDate], [SyncDate], [SyncGuid], [CreatedUserID], [LastUpdatedUserID], state,SyncState) 
            VALUES ('Eliminado - Outras Saídas', GETDATE(), GETDATE(), NULL, NULL, 1, 1, 0, 0)");
        }
        
        public override void Down()
        {
        }
    }
}
