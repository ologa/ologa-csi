namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_ParceirosdePopulacoesChaveAs_Family_Origin : DbMigration
    {
        public override void Up()
        {
            Sql(@"UPDATE se
                SET se.Code = '05'
                FROM[dbo].[SimpleEntity] se
                WHERE se.Type = 'fam-origin-ref-type' AND se.[Description] = 'Outra'");

            Sql(@"INSERT INTO [SimpleEntity] 
            (Type,Code,Description,CreatedDate,LastUpdatedDate,CreatedUserID,LastUpdatedUserID,State,SyncState)
            VALUES ('fam-origin-ref-type','04','Parceiros de Populacoes-Chave',GETDATE(),GETDATE(),1,1,0,0)");
        }
        
        public override void Down()
        {
        }
    }
}
