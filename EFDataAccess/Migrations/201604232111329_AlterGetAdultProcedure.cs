namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AlterGetAdultProcedure : DbMigration
    {
        public override void Up()
        {
            AlterStoredProcedure("dbo.GetAdult", p => new { AdultID = p.Int(), }, @"

            /******************************************************************************
            **	File: 
            **	Name: dbo.GetAdult
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
	            SELECT FirstName, LastName, Gender, IDNumber, Passport, MaritalStatusID, HIV, ContactNo, DateOfBirth, CreatedDate, CreatedUserID, LastUpdatedDate, LastUpdatedUserID
	            FROM Adult
	            WHERE AdultID = @AdultID
            END
            ");
        }
        
        public override void Down()
        {
        }
    }
}
