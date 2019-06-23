namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AlterProcedureGetChildCountForDashboard : DbMigration
    {
        public override void Up()
        {
            AlterStoredProcedure("dbo.GetChildCountForDashboard", p => new { partnerID = p.Int(), }, @"
                DECLARE @strWhere VARCHAR(100)
                DECLARE @strSelect VARCHAR(8000)

                /******************************************************************************
                **	File: 
                **	Name: dbo.GetChildCountForDashboard
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
	                IF (@partnerID > 0) 
	                BEGIN
		                SET @strWhere = ' WHERE [Partner].[PartnerID] = ' + CAST(@partnerID AS CHAR)
	                END
	                ELSE
	                BEGIN
		                SET @strWhere = ''
	                END
	
	                SET @strSelect = 'SELECT [Partner].[Name], (SELECT COUNT([Child].[ChildID]) 
																							                FROM [Child] LEFT JOIN [ChildPartner] ON [Child].[ChildID] = [ChildPartner].[ChildID] 
																							                WHERE [ChildPartner].[PartnerID] = [Partner].[PartnerID] 
																							                AND (SELECT TOP 1 [ChildStatusHistory].[ChildStatusID] 
																									                 FROM [ChildStatusHistory] 
																									                 WHERE ChildID = Child.ChildID  
																									                 AND CAST([EffectiveDate] as DATE) <= CAST(getdate() as DATE) 
																									                 ORDER BY [EffectiveDate] DESC, [ChildStatusHistoryID] DESC) = 1) AS Active, 
																						                 (SELECT COUNT([Child].[ChildID]) 
																							                FROM [Child] LEFT JOIN [ChildPartner] ON [Child].[ChildID] = [ChildPartner].[ChildID] 
																							                WHERE [ChildPartner].[PartnerID] = [Partner].[PartnerID] 
																							                AND (SELECT TOP 1 [ChildStatusHistory].[ChildStatusID] 
																									                 FROM [ChildStatusHistory] WHERE ChildID = Child.ChildID  AND CAST([EffectiveDate] as DATE) <= CAST(getdate() as DATE) 
																									                 ORDER BY [EffectiveDate] DESC, [ChildStatusHistoryID] DESC) <> 1) AS InActive 
										                FROM [Partner] ' + @strWhere + ' ORDER BY [Partner].[Name] ASC'
	                EXEC(@strSelect)
	
                END
                ");
        }
        
        public override void Down()
        {
        }
    }
}
