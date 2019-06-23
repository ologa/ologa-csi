namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Remove_MaitenanceState : DbMigration
    {
        public override void Up()
        {
            //UPDATE DE CRIANÇAS EM MANUTENÇÃO PARA INICIAL
            Sql(@"UPDATE csh
                SET csh.[ChildStatusID] = 
                (
	                SELECT cs.[StatusID]
	                FROM  [ChildStatus] cs
	                WHERE cs.Description = 'Inicial'
                )
                FROM  [ChildStatusHistory] csh
                JOIN  [ChildStatus] cs ON csh.ChildStatusID = cs.StatusID AND cs.Description = 'Manutenção' AND csh.childID IS NOT NULL", false);

            //UPDATE DE ADULTOS EM MANUTENÇÃO PARA ADULTO
            Sql(@"UPDATE csh
                SET csh.[ChildStatusID] = 
                (
	                SELECT cs.[StatusID]
	                FROM  [ChildStatus] cs
	                WHERE cs.Description = 'Adulto'
                )
                FROM  [ChildStatusHistory] csh
                JOIN  [ChildStatus] cs ON csh.ChildStatusID = cs.StatusID AND cs.Description = 'Manutenção' AND csh.AdultID IS NOT NULL", false);

            //DELETE MANUTENÇÃO
            Sql(@"DELETE FROM [ChildStatus] WHERE Description = 'Manutenção'", false);

        }
        
        public override void Down()
        {
            //INSERT MANUTENÇÃO
            Sql(@"INSERT INTO  [ChildStatus] (Description,state,SyncState) VALUES ('Manutenção',0,0)", false);

        }
    }
}
