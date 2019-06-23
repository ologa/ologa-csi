namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateAllAdultsInAdultStatusToInitialStatus_part2 : DbMigration
    {
        public override void Up()
        {
            //UPDATE DE ADULTOS EM ADULTO PARA INICIAL
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
