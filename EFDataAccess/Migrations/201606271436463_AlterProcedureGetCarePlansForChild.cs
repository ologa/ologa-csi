namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AlterProcedureGetCarePlansForChild : DbMigration
    {
        public override void Up()
        {
            AlterStoredProcedure("dbo.GetCarePlansForChild", p => new { SearchFrom = p.DateTime(), SearchTo = p.DateTime(), ChildID = p.Int(), Gender = p.String(), PartnerID = p.Int(), }, @"
                    DECLARE @strWhere VARCHAR(8000)
                    DECLARE @strSelect VARCHAR(8000)

                    /******************************************************************************
                    **	File: 
                    **	Name: dbo.GetCarePlansForChild
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
	                    IF (@ChildID > 0) 
	                    BEGIN
		                    SET @strWhere = 'WHERE CSI.ChildID = ' + CAST(@ChildID AS CHAR) + ' '
	                    END
	                    ELSE
	                    BEGIN
			                    SET @strWhere = 'WHERE CarePlan.CarePlanDate BETWEEN ''' + CAST(@SearchFrom AS VARCHAR) + ''' AND ''' + CAST(@SearchTo AS VARCHAR) + ''' '
	                    END

                        IF (@Gender = 'M')
                        BEGIN
                            SET @strWhere = @strWhere +  ' AND Child.Gender = ''' + @Gender +''' '
                        END

                        IF (@Gender = 'F')
                        BEGIN
                            SET @strWhere = @strWhere +  ' AND Child.Gender = ''' + @Gender +''' '
                        END

	                    SET @strSelect = 'SELECT CarePlan.CarePlanID, Child.ChildID, Child.FirstName + '' '' + Child.LastName As FullName, CarePlanDate 
										                    FROM [CarePlan] INNER JOIN [CSI] ON [CarePlan].[CSIID] = [CSI].[CSIID] 
										                    INNER JOIN [Child] ON [CSI].[ChildID] = [Child].[ChildID] '
                        IF (@PartnerID > 0) 
	                    BEGIN
		                    SET @strWhere = @strWhere + ' AND Partner.PartnerID = ' + CAST(@PartnerID AS CHAR) + ' '

                            SET @strSelect = @strSelect + ' INNER JOIN [ChildPartner] ON [ChildPartner].[ChildID] = [Child].[ChildID]
                                                          INNER JOIN [Partner] ON [Partner].[PartnerID] = [ChildPartner].[PartnerID] '
	                    END
                        
                        SET @strSelect = @strSelect + @strWhere + ' ORDER BY CarePlanDate ASC'
	                    EXEC(@strSelect)
                    END
                    ");
        }
        
        public override void Down()
        {
        }
    }
}
