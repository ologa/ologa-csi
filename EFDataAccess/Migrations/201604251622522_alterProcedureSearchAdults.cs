namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class alterProcedureSearchAdults : DbMigration
    {
        public override void Up()
        {
            AlterStoredProcedure("dbo.SearchAdults", p => new { firstName = p.String(maxLength: 30), lastName = p.String(maxLength: 30), onlyChief = p.String(maxLength: 30), }, @"
                DECLARE @strWhere VARCHAR(8000)
                DECLARE @strSelect VARCHAR(8000)

                /******************************************************************************
                **	File: 
                **	Name: dbo.SearchAdults
                **	Desc: 
                **
                **	This template can be customized:
                **
                **	Return values:
                **
                **	Called by:   
                **
                **	Parameters:
                **	Input							Output
                **	----------						-----------
                **
                **	Auth: VPPS 1
                **	Date: 2010/02/01 11:56:11 AM
                **	Generator Version: 1.0
                *******************************************************************************
                **	Change History
                *******************************************************************************
                **	Date:		Author:		Description:
                **	--------	--------	---------------------------------------
                **
                *******************************************************************************/

                BEGIN
	                SET @strWhere = ''
	                IF LEN(@firstName) > 0
	                BEGIN
		                SET @strWhere = @strWhere + ' WHERE [FirstName] Like ''%' + @firstName + '%'''
	                END
	                IF LEN(@lastName) > 0
	                BEGIN
		                IF LEN(@strWhere) > 0
		                BEGIN
			                SET @strWhere = @strWhere + ' AND [LastName] Like ''%' + @lastName + '%'''
		                END
		                ELSE
		                BEGIN
			                SET @strWhere = @strWhere + ' WHERE [LastName] Like ''%' + @lastName + '%'''
		                END
	                END
                    IF LEN(@onlyChief) > 0
	                BEGIN
		                IF LEN(@strWhere) > 0
		                BEGIN
			                SET @strWhere = @strWhere + ' AND [isHouseHoldChef] = ''' + @onlyChief + ''''
		                END
		                ELSE
		                BEGIN
			                SET @strWhere = @strWhere + ' WHERE [isHouseHoldChef] = '''+ @onlyChief + ''''
		                END 
	                END                  

	                SET @strSelect = 'SELECT DISTINCT [FirstName], [LastName], [Adult].[AdultID], IDNumber, convert(varchar(10), DateOfBirth, 103) AS DateOfBirth FROM Adult ' + @strWhere + ' ORDER BY [LastName], [FirstName]'
	
	                EXEC(@strSelect)
                END
                ");
        }
        
        public override void Down()
        {
        }
    }
}
