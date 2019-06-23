namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateAllAdultsInAdultStatusToInitialStatus : DbMigration
    {
        public override void Up()
        {
            //UPDATE DE ADULTOS EM ADULTO PARA INICIA
            Sql(@"UPDATE csh
                SET csh.[ChildStatusID] = 
                (
	                SELECT cs.[StatusID]
	                FROM  [ChildStatus] cs
	                WHERE cs.Description = 'Inicial'
                )
                FROM  [ChildStatusHistory] csh
                JOIN  [ChildStatus] cs ON csh.ChildStatusID = cs.StatusID AND cs.Description = 'Adulto' AND csh.AdultID IS NOT NULL", false);
        }

        public override void Down()
        {
        }
    }
}
