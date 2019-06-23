namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update_And_Order_BeneficiaryStatus_To_OriginalID : DbMigration
    {
        public override void Up()
        {
            //Update Duplicated Status to Original
            
            Sql(@"UPDATE csh
                SET csh.ChildStatusID = csFirst.StatusID 
                FROM ChildStatusHistory csh
                JOIN ChildStatus cs ON cs.StatusID = csh.ChildStatusID
                JOIN ChildStatus csFirst ON csFirst.Description = cs.Description
                WHERE csFirst.StatusID IN 
                (
	                SELECT
		                obj.StatusID
	                FROM
	                (
		                SELECT 
			                row_number() OVER (PARTITION BY description ORDER BY StatusID ASC) AS numeroLinha
			                ,StatusID
			                ,description
		                FROM 
			                 ChildStatus
	                )
	                obj
	                WHERE 
		                obj.numeroLinha=1
                )");

                //Delete Duplicated Status
                Sql(@"DELETE csFirst
                FROM ChildStatus csFirst
                WHERE csFirst.StatusID IN 
                (
	                SELECT
		                obj.StatusID
	                FROM
	                (
		                SELECT 
			                row_number() OVER (PARTITION BY description ORDER BY StatusID ASC) AS numeroLinha
			                ,StatusID
			                ,description
		                FROM 
			                 ChildStatus
	                )
	                obj
	                WHERE 
		                obj.numeroLinha>1
                )");



                //Retirar a permissao de ordenação de IDs
                Sql(@"set identity_insert [ChildStatus] on");

                //Reordenar IDs
                Sql(@"UPDATE cs
                SET cs.Description = 'Inicial' 
                ,cs.childstatus_guid = newID()
                ,cs.CreatedDate = getDate()
                ,cs.LastUpdatedDate = getDate()
                ,cs.CreatedUserID = 1
                ,cs.LastUpdatedUserID = 1
                FROM ChildStatus cs
                WHERE cs.Description = 'Inicial'



                DECLARE @Counter INT
                DECLARE @MaxChildStatus INT

                SET @Counter = 2
                SET @MaxChildStatus = 9--(SELECT MAX(StatusID) FROM ChildStatus)


                WHILE @Counter <= @MaxChildStatus
                BEGIN
	                insert into ChildStatus (StatusID, Description, childstatus_guid, CreatedDate, LastUpdatedDate, CreatedUserID, LastUpdatedUserID)
	                values 
	                --(1,'Inicial',GetDate(),GetDate(),1,1),
	                (@Counter,
	                CASE WHEN @Counter = 1 THEN 'Inicial'
	                WHEN @Counter = 2 THEN 'Transferência'
	                WHEN @Counter = 3 THEN 'Desistência'
	                WHEN @Counter = 4 THEN 'Perdido'
	                WHEN @Counter = 5 THEN 'Adulto'
	                WHEN @Counter = 6 THEN 'Óbito'
	                WHEN @Counter = 7 THEN 'Outras Saídas'
	                WHEN @Counter = 8 THEN 'Graduação' 
	                WHEN @Counter = 9 THEN 'Eliminado' END
	                ,NEWID(),GetDate(),GetDate(),1,1)

	                --Update Duplicated Status to Original
	                UPDATE csh
	                SET csh.ChildStatusID = csFirst.StatusID 
	                --SELECT csh.ChildStatusHistoryID, cs.StatusID, cs.Description
	                FROM ChildStatusHistory csh
	                JOIN ChildStatus cs ON cs.StatusID = csh.ChildStatusID
	                JOIN ChildStatus csFirst ON csFirst.Description = cs.Description
	                WHERE csFirst.StatusID IN 
	                (
		                SELECT
			                obj.StatusID
		                FROM
		                (
			                SELECT 
				                row_number() OVER (PARTITION BY description ORDER BY StatusID ASC) AS numeroLinha
				                ,StatusID
				                ,description
			                FROM 
				                 ChildStatus
		                )
		                obj
		                WHERE 
			                obj.numeroLinha=1
	                )

	                --Delete Duplicated Status
	                DELETE csFirst
	                FROM ChildStatus csFirst
	                WHERE csFirst.StatusID IN 
	                (
		                SELECT
			                obj.StatusID
		                FROM
		                (
			                SELECT 
				                row_number() OVER (PARTITION BY description ORDER BY StatusID ASC) AS numeroLinha
				                ,StatusID
				                ,description
			                FROM 
				                 ChildStatus
		                )
		                obj
		                WHERE 
			                obj.numeroLinha>1
	                )
	                SET @Counter += 1
                END");

            //Repor a permissao de ordenação de IDs
            Sql(@"set identity_insert [ChildStatus] off");
        }
        
        public override void Down()
        {
        }
    }
}
