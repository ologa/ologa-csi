namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeGetStatusHistoryCompleteLegacyProcedure : DbMigration
    {
        public override void Up()
        {            
            AlterStoredProcedure("dbo.GetStatusHistoryComplete", p => new { childID = p.Int(), }, @"
                DECLARE @strSelect VARCHAR(8000)

                /******************************************************************************
                **	File: 
                **	Name: dbo.GetStatusHistoryComplete
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
	                SET @strSelect = 'SELECT [ChildStatusHistoryID], [ChildID], [ChildStatus].[Description] AS ChildStatus, [CreatedDate], 
									                  RTRIM([User].[FirstName]) + '' '' + LTRIM([User].[LastName]) AS UserFullName,  CONVERT(VARCHAR, [EffectiveDate], 120) AS EffectiveDate 
									                  FROM ([ChildStatusHistory] INNER JOIN ChildStatus ON ChildStatusHistory.ChildStatusID = ChildStatus.StatusID) 
									                  INNER JOIN [User] ON [ChildStatusHistory].[CreatedUserID] = [User].[UserID] 
									                  WHERE [ChildID] = ' + CAST(@ChildID AS CHAR) + ' ORDER BY [EffectiveDate] DESC, [ChildStatusHistoryID] DESC '
	                EXEC(@strSelect)
                END
           ");
        }
        
        public override void Down()
        {
        }
    }
}
