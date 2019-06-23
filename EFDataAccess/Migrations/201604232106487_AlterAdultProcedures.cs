namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AlterAdultProcedures : DbMigration
    {
        public override void Up()
        {

            AlterStoredProcedure("dbo.InsertAdult", p => new { FirstName = p.String(maxLength: 30), LastName = p.String(maxLength: 30), Gender = p.String(maxLength: 1), IDNumber = p.String(maxLength: 20), Passport = p.Int(), DateOfBirth = p.DateTime(), MaritalStatusID = p.String(maxLength: 1), HIV = p.String(maxLength: 1), ContactNo = p.String(maxLength: 20), CreatedDate = p.DateTime(), CreatedUser = p.Int(), LastUpdatedDate = p.DateTime(), LastUpdatedUser = p.Int(), }, @"

                /******************************************************************************
                **	File: 
                **	Name: dbo.InsertAdult
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
	                INSERT INTO Adult (FirstName, LastName, Gender, IDNumber, Passport, MaritalStatusID, HIV, DateOfBirth, ContactNo, CreatedDate, CreatedUserID, LastUpdatedDate, LastUpdatedUserID)
	                VALUES (@FirstName, @LastName, @Gender, @IDNumber, @Passport, @MaritalStatusID, @HIV, @DateOfBirth, @ContactNo, @CreatedDate, @CreatedUser, @LastUpdatedDate, @LastUpdatedUser)

	                SELECT @@Identity
                END
                ");

            AlterStoredProcedure("dbo.UpdateAdult", p => new { AdultID = p.Int(), FirstName = p.String(maxLength: 30), LastName = p.String(maxLength: 30), Gender = p.String(maxLength: 1), IDNumber = p.String(maxLength: 20), Passport = p.Int(), DateOfBirth = p.DateTime(), MaritalStatusID = p.String(maxLength: 1), HIV = p.String(maxLength: 1), ContactNo = p.String(maxLength: 20), CreatedDate = p.DateTime(), CreatedUser = p.Int(), LastUpdatedDate = p.DateTime(), LastUpdatedUser = p.Int(), }, @"

                /******************************************************************************
                **	File: 
                **	Name: dbo.UpdateAdult
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
	                UPDATE Adult
	                SET	FirstName = @FirstName,
	                LastName = @LastName, 
	                Gender = @Gender, 
	                IDNumber = @IDNumber,
	                Passport = @Passport,
	                DateOfBirth = @DateOfBirth, 
	                MaritalStatusID = @MaritalStatusID,
	                HIV = @HIV,
	                ContactNo = @ContactNo,
	                CreatedDate = @CreatedDate, 
	                CreatedUserID = @CreatedUser, 
	                LastUpdatedDate = @LastUpdatedDate, 
	                LastUpdatedUserID = @LastUpdatedUser
	                WHERE AdultID = @AdultID
                END
                ");
        }

        public override void Down()
        {
        }
    }
}
