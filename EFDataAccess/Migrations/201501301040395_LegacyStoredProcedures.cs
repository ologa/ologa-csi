namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LegacyStoredProcedures : DbMigration
    {
        public override void Up()
        {
            CreateStoredProcedure("dbo.IsSupportServiceTypeInUse", p => new { SupportServiceTypeID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.IsSupportServiceTypeInUse
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
	SELECT COUNT(*) AS CountSupportServiceType
	FROM CSIDomainSupportService
	WHERE SupportServiceTypeID = @SupportServiceTypeID
END
");

            CreateStoredProcedure("dbo.GetHouseholds", @"

/******************************************************************************
**	File: 
**	Name: dbo.GetHouseholds
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
	SELECT HouseholdID, Address, VillageID, DistrictID, Country, ParticipantRegistrationID, CreatedDate, CreatedUser, LastUpdatedDate, LastUpdatedUser
	FROM Household
END
");

            CreateStoredProcedure("dbo.IsSupportServiceTypeUnique", p => new { SupportServiceTypeID = p.Int(), Description = p.String(maxLength: 50), }, @"
DECLARE @sql AS VARCHAR(1000)
/******************************************************************************
**	File: 
**	Name: dbo.IsSupportServiceTypeUnique
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
	SET @sql = '
	SELECT COUNT(*) AS CountSupportServiceType
	FROM SupportServiceType
	WHERE SupportServiceTypeID <> ' + CAST(@SupportServiceTypeID AS CHAR) + '
	AND Description = ''' + REPLACE(RTRIM(LTRIM(@Description)), '''', '''''') + '''' 
	EXEC(@sql)
END
");

            CreateStoredProcedure("dbo.GetIndexesForChild", p => new { ChildID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.GetIndexesForChild
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
**	@CSIID						CSIID
**									ChildID
**									IndexDate
**									CreatedUserID
**									CreatedDate
**									LastUpdatedUserID
**									LastUpdatedDate
**									StatusID
**									Photo
**									Height
**									Weight
**									BMI
**									TakingMedication
**									MedicationDescription
**									Suggestions
**									DistrictID
**									Caregiver
**									CaregiverRelationID
**									SocialWorkername
**									SocialWorkerContactNo
**									DoctorName
**									DoctorContactNo
**									AllergyNotes
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
	SELECT CSIID, IndexDate, CreatedUserID, CreatedDate, LastUpdatedUserID, LastUpdatedDate, StatusID, Photo, Height, Weight, BMI, TakingMedication, MedicationDescription, Suggestions, DistrictID, Caregiver, CaregiverRelationID, SocialWorkerName, socialWorkerContactNo, DoctorName, DoctorContactNo, AllergyNotes
	FROM CSI
	WHERE ChildID = @ChildID
END
");

            CreateStoredProcedure("dbo.IsUserFullNameUnique", p => new { UserID = p.Int(), FirstName = p.String(maxLength: 50), LastName = p.String(maxLength: 50), }, @"
DECLARE @sql AS VARCHAR(1000)
/******************************************************************************
**	File: 
**	Name: dbo.IsUserFullNameUnique
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
	SET @sql = '
	SELECT COUNT(*) AS CountUser
	FROM [User]
	WHERE UserID <> ' + CAST(@UserID AS CHAR) + '
	AND FirstName = ''' + REPLACE(RTRIM(LTRIM(@FirstName)), '''', '''''') + '''
	AND LastName = ''' + REPLACE(RTRIM(LTRIM(@LastName)), '''', '''''') + ''''
	EXEC(@sql)
END
");

            CreateStoredProcedure("dbo.GetLastChild", @"

/******************************************************************************
**	File: 
**	Name: dbo.GetLastChild
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
**									ChildID
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
	SELECT MAX(ChildID) AS ChildID
	FROM Child
	WHERE Locked IS NULL 
	OR Locked = 0
END
");

            CreateStoredProcedure("dbo.IsUserInUseCarePlan", p => new { UserID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.IsUserInUseCarePlan
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
	SELECT COUNT(*) AS CountUser
	FROM CarePlan
	WHERE CreatedUserID = @UserID
	OR LastUpdateduserID = @UserID
END
");

            CreateStoredProcedure("dbo.GetLoggedOnUsers", @"

/******************************************************************************
**	File: 
**	Name: dbo.GetLoggedOnUsers
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
**	@UserID						UserID
**									Username
**									Password
**									FirstName
**									LastName
**									Admin
**									DefSite
**									LoggedOn
**									PartnerID
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
	SELECT UserID, Username, Password, FirstName, LastName, Admin, DefSite, LoggedOn, PartnerID
	FROM [User]
	WHERE LoggedOn = 'True'
END
");

            CreateStoredProcedure("dbo.IsUserInUseChild", p => new { UserID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.IsUserInUseChild
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
	SELECT COUNT(*) AS CountUser
	FROM Child
	WHERE CreatedUserID = @UserID
	OR LastUpdateduserID = @UserID
END
");

            CreateStoredProcedure("dbo.GetMembershipList", p => new { GroupType = p.String(maxLength: 10), MembershipID = p.Int(), LastName = p.String(maxLength: 100), }, @"
DECLARE @strWhere VARCHAR(8000)
DECLARE @strSelect VARCHAR(8000)
DECLARE @strInner varchar(1000)

/******************************************************************************
**	File: 
**	Name: dbo.GetMembershipList
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
	IF (@MembershipID <> '') 
	BEGIN
		IF (@LastName <> '')
		BEGIN
			SET @strWhere = 'WHERE m.MembershipID = ' + @MembershipID
			SET @strWhere = @strWhere + 'AND (c.[LastName] Like ''%' + @LastName + '%'') '
			SET @strWhere = @strWhere + 'OR a.[LastName] Like ''%' + @LastName + '%'') '
		END
		ELSE
		BEGIN
			SET @strWhere = 'WHERE m.MembershipID = ' + @MembershipID
			SET @strWhere = @strWhere + 'OR a.[LastName] Like ''%' + @LastName + '%'''
		END
	END
	ELSE
	BEGIN
		IF (@LastName <> '')
		BEGIN
			SET @strWhere = 'WHERE c.[LastName] Like ''%' + @LastName + '%'''
			SET @strWhere = @strWhere + 'OR a.[LastName] Like ''%' + @LastName + '%'''
		END
	END
	
	IF @GroupType = 'Household'
	BEGIN
		SET @strInner = 'INNER JOIN HouseholdMembership hm ON m.MembershipID = hm.MembershipID'
	END
	ELSE IF @GroupType = 'SILC'
	BEGIN
		SET @strInner = 'INNER JOIN SILCMembership silc ON m.MembershipID = silc.MembershipID'
	END
	ELSE IF @GroupType = 'ECCD'
	BEGIN
		SET @strInner = 'INNER JOIN ECCDMembership eccd ON m.MembershipID = eccd.MembershipID'
	END
	ELSE IF @GroupType = 'Care Group'
	BEGIN
		SET @strInner = 'INNER JOIN CareGroupMembership cgm ON m.MembershipID = cgm.MembershipID'
	END
	ELSE IF @GroupType = 'CFM'
	BEGIN
		SET @strInner = 'INNER JOIN CFMMembership cfm ON m.MembershipID = cfm.MembershipID'
	END
	ELSE IF @GroupType = 'VGA'
	BEGIN
		SET @strInner = 'INNER JOIN VGAMembership vga ON m.MembershipID = vga.MembershipID'
	END

	SET @strSelect = 'SELECT distinct m.MembershipID, m.MembershipName
						FROM Membership m ' + @strInner + '
						left outer join ChildMembership cm on cm.MembershipID = m.MembershipID
						left outer join Child c on c.ChildID = cm.ChildID
						left outer join AdultMembership am on am.MembershipID = m.membershipID
						left outer join Adult a on a.adultid = am.AdultID ' + @strWhere + ' ORDER BY h.HouseholdID ASC '
	EXEC(@strSelect)
END
");

            CreateStoredProcedure("dbo.ChildHasCSI", p => new { ChildID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.ChildHasCSI
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
**	@ChildID						ChildID
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
	SELECT COUNT(*) 
	FROM CSI
	WHERE ChildID = @ChildID
END
");

            CreateStoredProcedure("dbo.IsUserInUseChildStatusHistory", p => new { UserID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.IsUserInUseChildStatusHistory
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
	SELECT COUNT(*) AS CountUser
	FROM ChildStatusHistory
	WHERE CreatedUserID = @UserID
END
");

            CreateStoredProcedure("dbo.GetOutcome", p => new { OutcomeID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.GetOutcome
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
**	@OutcomeID						OutcomeID
**									Code
**									Name
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
	SELECT Code, Description
	FROM Outcome
	WHERE OutcomeID = @OutcomeID
END
");

            CreateStoredProcedure("dbo.ClearBookingLocks", p => new { UserID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.ClearBookingLocks
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
	UPDATE Booking
	SET Locked = NULL
	WHERE Locked = @UserID
END
");

            CreateStoredProcedure("dbo.IsUserInUseCSI", p => new { UserID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.IsUserInUseCSI
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
	SELECT COUNT(*) AS CountUser
	FROM CSI
	WHERE CreatedUserID = @UserID
	OR LastUpdatedUserID = @UserID
END
");

            CreateStoredProcedure("dbo.GetOVCType", p => new { OVCTypeID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.GetOVCType
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
**	@ServiceProviderTypeID						ServiceProviderTypeID
**									Description
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
	SELECT Description
	FROM OVCType
	WHERE OVCTypeID = @OVCTypeID
END
");

            CreateStoredProcedure("dbo.ClearContactLocks", p => new { UserID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.ClearContactLocks
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
	UPDATE Contact
	SET Locked = NULL
	WHERE Locked = @UserID
END
");

            CreateStoredProcedure("dbo.IsUserInUseFollowUp", p => new { UserID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.IsUserInUseFollowUp
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
	SELECT COUNT(*) AS CountUser
	FROM FollowUp
	WHERE CreatedUserID = @UserID
END
");

            CreateStoredProcedure("dbo.GetOVCTypes", @"

/******************************************************************************
**	File: 
**	Name: dbo.GetOVCTypes
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
	SELECT OVCTypeID, Description
	FROM OVCType
END
");

            CreateStoredProcedure("dbo.ClearNoticeLocks", p => new { UserID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.ClearNoticeLocks
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
	UPDATE Notice
	SET Locked = NULL
	WHERE Locked = @UserID
END
");

            CreateStoredProcedure("dbo.IsUserInUseUserAction", p => new { UserID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.IsUserInUseUserAction
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
	SELECT COUNT(*) AS CountUser
	FROM UserAction
	WHERE UserID = @UserID
END
");

            CreateStoredProcedure("dbo.GetParticipantRegistration", p => new { ParticipantRegistrationID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.GetParticipantRegistration
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
	SELECT Enumerator, DateOfReg, ProjectAreaID, ActivePersonID, AnyPregnant, CropLand, DistrictID, VillageID
	FROM ParticipantRegistration
	WHERE ParticipantRegistrationID = @ParticipantRegistrationID
END
");

            CreateStoredProcedure("dbo.DeleteAction", p => new { ActionID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.DeleteAction
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
**	@ActionID
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
	DELETE FROM Action
	WHERE ActionID = @ActionID
END
");

            CreateStoredProcedure("dbo.IsUserNameUnique", p => new { UserID = p.Int(), UserName = p.String(maxLength: 50), }, @"
DECLARE @sql AS VARCHAR(1000)
/******************************************************************************
**	File: 
**	Name: dbo.IsUserNameUnique
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
	SET @sql = '
	SELECT COUNT(*) AS CountUser
	FROM [User]
	WHERE UserID <> ' + CAST(@UserID AS CHAR) + '
	AND [UserName] = ''' + REPLACE(RTRIM(LTRIM(@UserName)), '''', '''''') + '''' 
	EXEC(@sql)
END
");

            CreateStoredProcedure("dbo.GetParticipantRegistrations", @"

/******************************************************************************
**	File: 
**	Name: dbo.GetParticipantRegistrations
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
	SELECT Enumerator, DateOfReg, ProjectAreaID, ActivePersonID, AnyPregnant, CropLand, DistrictID, VillageID
	FROM ParticipantRegistration
END
");

            CreateStoredProcedure("dbo.DeleteAdult", p => new { AdultID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.DeleteAdult
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
**	@ActionID
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
	DELETE FROM AdultService
	WHERE AdultID = @AdultID
	
	DELETE FROM AdultHousehold
	WHERE AdultID = @AdultID
	
	DELETE FROM Adult
	WHERE AdultID = @AdultID
END
");

            CreateStoredProcedure("dbo.IsVillageInUse", p => new { VillageID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.IsVillageInUse
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
	SELECT COUNT(*) AS CountVillage
	FROM Child
	WHERE VillageID = @VillageID
END
");

            CreateStoredProcedure("dbo.GetPartner", p => new { PartnerID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.GetPartner
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
**	@PartnerID						PartnerID
**									Name
**									Address
**									ContactNo
**									FaxNo
**									ContactName
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
	SELECT Name, Address, ContactNo, FaxNo, ContactName
	FROM Partner
	WHERE PartnerID = @PartnerID
END
");

            CreateStoredProcedure("dbo.DeleteAdultHousehold", p => new { AdultHouseholdID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.DeleteAdultHousehold
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
**	@ChildID
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
	DELETE FROM AdultHousehold
	WHERE AdultHouseholdID = @AdultHouseholdID
END
");

            CreateStoredProcedure("dbo.IsVillageUnique", p => new { VillageID = p.Int(), Name = p.String(maxLength: 50), }, @"
DECLARE @sql AS VARCHAR(1000)
/******************************************************************************
**	File: 
**	Name: dbo.IsVillageUnique
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
	SET @sql = '
	SELECT COUNT(*) AS CountVillage
	FROM Village
	WHERE VillageID <> ' + CAST(@VillageID AS CHAR) + '
	AND [Name] = ''' + REPLACE(RTRIM(LTRIM(@Name)), '''', '''''') + '''' 
	EXEC(@sql)
END
");

            CreateStoredProcedure("dbo.GetPartners", @"

/******************************************************************************
**	File: 
**	Name: dbo.GetPartners
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
	SELECT PartnerID, [Name], Address, ContactNo, FaxNo, ContactName
	FROM Partner
END
");

            CreateStoredProcedure("dbo.DeleteAdultService", p => new { AdultServiceID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.DeleteAdultService
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
**	@AdultID
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
	DELETE FROM AdultService
	WHERE AdultServiceID = @AdultServiceID
END
");

            CreateStoredProcedure("dbo.IsWardInUse", p => new { WardID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.IsWardInUse
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
	SELECT COUNT(*) AS CountWard
	FROM Child
	WHERE WardID = @WardID
END
");

            CreateStoredProcedure("dbo.GetPartnersForChild", p => new { ChildID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.GetPartnersForChild
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
**	@ChildPartnerID						ChildPartnerID
**									ChildID
**									PartnerID
**									EffectiveDate
**									Active
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
	SELECT ChildPartnerID, ChildID, PartnerID, EffectiveDate, Active
	FROM ChildPartner
	WHERE ChildID = @ChildID
END
");

            CreateStoredProcedure("dbo.DeleteCareGiverRelation", p => new { RelationID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.DeleteCareGiverRelation
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
**	@RelationID
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
	DELETE FROM CareGiverRelation
	WHERE RelationID = @RelationID
END
");

            CreateStoredProcedure("dbo.IsWardUnique", p => new { WardID = p.Int(), Description = p.String(maxLength: 50), }, @"
DECLARE @sql AS VARCHAR(1000)
/******************************************************************************
**	File: 
**	Name: dbo.IsWardUnique
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
	SET @sql = '
	SELECT COUNT(*) AS CountWard
	FROM Ward
	WHERE WardID <> ' + CAST(@WardID AS CHAR) + '
	AND Description = ''' + REPLACE(RTRIM(LTRIM(@Description)), '''', '''''') + '''' 
	EXEC(@sql)
END
");

            CreateStoredProcedure("dbo.GetProjectList", p => new { SearchFrom = p.DateTime(), SearchTo = p.DateTime(), HouseholdID = p.Int(), }, @"
DECLARE @strWhere VARCHAR(8000)
DECLARE @strSelect VARCHAR(8000)

/******************************************************************************
**	File: 
**	Name: dbo.GetProjectList
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
	IF (@HouseholdID > 0) 
	BEGIN
		SET @strWhere = 'WHERE h.[HouseholdID] = ' + CAST(@HouseholdID AS CHAR) + ' '
	END
	ELSE
	BEGIN
		SET @strWhere = 'WHERE hp.CreatedDate BETWEEN ''' + CAST(@searchFrom AS VARCHAR) + ''' AND ''' + CAST(@searchTo AS VARCHAR) + ''' '
	END

	SET @strSelect = 'SELECT h.HouseholdID, hp.HouseholdProjectID, hp.ParticipantRegistrationID, hp.FieldAgentMonitoringID, hp.TrainingRegistryID, 
											CONVERT(Varchar(10), hp.CreatedDate, 103) AS CreatedDate
										FROM HouseholdProject hp
										INNER JOIN Household h ON h.HouseholdID = hp.HouseholdID ' + @strWhere + ' ORDER BY hp.CreatedDate ASC '
	EXEC(@strSelect)
END
");

            CreateStoredProcedure("dbo.DeleteCarePlan", p => new { CarePlanID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.DeleteCarePlan
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
**	@CarePlanID
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
	DECLARE @CarePlanDomainSupportServices TABLE (
	CarePlanDomainSupportServiceID int)

	INSERT INTO @CarePlanDomainSupportServices (CarePlanDomainSupportServiceID)
	SELECT 	CarePlanDomainSupportServiceID
	FROM 	CarePlanDomainSupportService
	WHERE 	CarePlanDomainID IN (SELECT CarePlanDomainID FROM CarePlanDomain WHERE CarePlanID = @CarePlanID)

	DELETE FROM CarePlanDomainSupportService
	WHERE CarePlanDomainSupportServiceID IN (SELECT CarePlanDomainSupportServiceID FROM @CarePlanDomainSupportServices)
	
	DELETE FROM CarePlanDomain
	WHERE CarePlanID = @CarePlanID
	
	DELETE FROM CarePlan
	WHERE CarePlanID = @CarePlanID
END
");

            CreateStoredProcedure("dbo.LockChild", p => new { ChildID = p.Int(), UserID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.LockChild
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
	UPDATE Child
	SET Locked = @UserID
	WHERE ChildID = @ChildID
END
");

            CreateStoredProcedure("dbo.GetQuestion", p => new { QuestionID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.GetQuestion
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
**	@QuestionID						QuestionID
**									DomainID
**									Description
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
	SELECT DomainID, Description
	FROM Question
	WHERE QuestionID = @QuestionID
END
");

            CreateStoredProcedure("dbo.DeleteCarePlanDomain", p => new { CarePlanDomainID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.DeleteCarePlanDomain
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
**	@CarePlanDomainID
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
	DELETE FROM CarePlanDomain
	WHERE CarePlanDomainID = @CarePlanDomainID
END
");

            CreateStoredProcedure("dbo.LockUser", p => new { UserID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.LockUser
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
	UPDATE [User]
	SET LoggedOn = 1
	WHERE UserID = @UserID
END
");

            CreateStoredProcedure("dbo.GetQuestionReport", p => new { questionID = p.Int(), score = p.Int(), lastSixMonths = p.Boolean(), }, @"
DECLARE @strWhere VARCHAR(8000)
DECLARE @strSelect VARCHAR(8000)

/******************************************************************************
**	File: 
**	Name: dbo.GetQuestionReport
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
	SET @strWhere = 'WHERE Question.QuestionID = ' + CAST(@questionID AS CHAR) + ' 
									 AND CSIDomainScore.Score = ' + CAST(@score AS CHAR) + ' '
	IF @lastSixMonths = 1 
	BEGIN
		SET @strWhere = @strWhere + 'AND CSI.IndexDate > DATEADD(month, -6, GETDATE()) '
	END

	SET @strSelect = 
		'SELECT LTRIM(RTRIM(FirstName)) + '' '' + LTRIM(RTRIM(LastName)) As FullName, 
			Child.ChildID AS ChildID, 
			DATEDIFF(YEAR, DateOFBirth, GETDATE()) AS Age, 
			DATENAME(d, CSI.IndexDate) + '' '' + DATENAME(m, CSI.IndexDate) + '' '' + DATENAME(YEAR, CSI.IndexDate) AS CSIDate,
			CASE CSIDomain.DomainID WHEN 1 THEN ''Food & Nutrition''
				WHEN 2 THEN ''Shelter & Care'' 
				WHEN 3 THEN ''Protection''
				WHEN 4 THEN ''Health''
				WHEN 5 THEN ''Psychosocial''
				WHEN 6 THEN ''Education & Skills'' END AS Domain,
			CSIDomainScore.Comments AS Comments 
		FROM Child 
		INNER JOIN CSI ON Child.ChildID = CSI.ChildID 
		INNER JOIN CSIDomain ON CSI.CSIID = CSIDomain.CSIID
		INNER JOIN CSIDomainScore ON CSIDomain.CSIDomainID = CSIDomainScore.CSIDomainID
		INNER JOIN Question ON CSIDomainScore.QuestionID = Question.QuestionID '
		+ @strWhere + ' ORDER BY LTRIM(RTRIM([LastName])), LTRIM(RTRIM([FirstName])) '
	EXEC(@strSelect)
END
");

            CreateStoredProcedure("dbo.DeleteCarePlanDomainSupportService", p => new { CarePlanDomainSupportServiceID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.DeleteCarePlanDomainSupportService
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
**	@CarePlanDomainSupportServiceID
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
	DELETE FROM CarePlanDomainSupportService
	WHERE CarePlanDomainSupportServiceID = @CarePlanDomainSupportServiceID
END
");

            CreateStoredProcedure("dbo.SearchAdults", p => new { firstName = p.String(maxLength: 30), lastName = p.String(maxLength: 30), }, @"
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

	SET @strSelect = 'SELECT DISTINCT [FirstName], [LastName], [Adult].[AdultID], IDNumber, convert(varchar(10), DateOfBirth, 103) AS DateOfBirth FROM Adult ' + @strWhere + ' ORDER BY [LastName], [FirstName]'
	
	EXEC(@strSelect)
END
");

            CreateStoredProcedure("dbo.GetQuestions", @"

/******************************************************************************
**	File: 
**	Name: dbo.GetQuestions
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
**									QuestionID
**									DomainID
**									Description
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
	SELECT QuestionID, DomainID, Description
	FROM Question
END
");

            CreateStoredProcedure("dbo.DeleteChild", p => new { ChildID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.DeleteChild
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
**	@ChildID
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

	DELETE FROM ChildHousehold
	WHERE ChildID = @ChildID
	
	DELETE FROM ChildStatusHistory
	WHERE ChildID = @ChildID
	
	DELETE FROM ChildPartner
	WHERE ChildID = @ChildID
	
	DELETE FROM Child
	WHERE ChildID = @ChildID
END
");

            CreateStoredProcedure("dbo.SearchAdultsByLetter", p => new { letter = p.String(maxLength: 4), }, @"
DECLARE @strWhere VARCHAR(8000)
DECLARE @strSelect VARCHAR(8000)

/******************************************************************************
**	File: 
**	Name: dbo.SearchAdultsByLetter
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
	IF @letter <> 'All'
	BEGIN
		SET @strWhere = 'WHERE LTRIM(RTRIM([LastName])) LIKE ''' + @letter + '%'''
	END

	SET @strSelect = 'SELECT DISTINCT [FirstName], [LastName], [Adult].[AdultID]
		FROM Adult ' + @strWhere + ' ORDER BY [LastName], [FirstName]'
	EXEC(@strSelect)
END
");

            CreateStoredProcedure("dbo.GetRecipientType", p => new { RecipientTypeID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.GetRecipientType
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
**	@RecipientTypeID						RecipientTypeID
**									Description
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
	SELECT Description
	FROM RecipientType
	WHERE RecipientTypeID = @RecipientTypeID
END
");

            CreateStoredProcedure("dbo.DeleteChildDisability", p => new { ChildDisabilityID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.DeleteChildDisability
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
**	@ChildDisabilityID
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
	DELETE FROM ChildDisability
	WHERE ChildDisabilityID = @ChildDisabilityID
END
");

            CreateStoredProcedure("dbo.SearchChildren", p => new { firstName = p.String(maxLength: 30), lastName = p.String(maxLength: 30), partnerID = p.Int(), }, @"
DECLARE @strWhere VARCHAR(8000)
DECLARE @strSelect VARCHAR(8000)

/******************************************************************************
**	File: 
**	Name: dbo.SearchChildren
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
	IF (@partnerID > 0) 
	BEGIN
		SET @strWhere = 'WHERE ChildPartner.PartnerID = ' + CAST(@partnerID AS CHAR) + ' '
		IF LEN(@firstName) > 0
		BEGIN
			SET @strWhere = @strWhere + ' AND [FirstName] Like ''%' + @firstName + '%'''
		END
		IF LEN(@lastName) > 0
		BEGIN
			SET @strWhere = @strWhere + ' AND [LastName] Like ''%' + @lastName + '%'''
		END
	END
	ELSE
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
	END

	SET @strSelect = 'SELECT DISTINCT [FirstName], [LastName], [Child].[ChildID], [Ward].[Description] AS Ward, [Village].[Name] AS Village, [District].[Description] AS District 
					  FROM Child LEFT JOIN ChildPartner ON Child.ChildID = ChildPartner.ChildID 
					  LEFT JOIN Ward ON Child.WardID = Ward.WardID 
					  LEFT JOIN Village ON Child.VillageID = Village.VillageID 
					  LEFT JOIN District ON Child.DistrictID = District.DistrictID ' + @strWhere + ' ORDER BY [LastName], [FirstName]'
	EXEC(@strSelect)
END
");

            CreateStoredProcedure("dbo.GetScoreAverageForDashboard", p => new { partnerID = p.Int(), }, @"
DECLARE @strWhere VARCHAR(8000)
DECLARE @strSelect VARCHAR(MAX)

/******************************************************************************
**	File: 
**	Name: dbo.GetScoreAverageForDashboard
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
		SET @strWhere = ' AND [Partner].[PartnerID] = ' + CAST(@partnerID AS CHAR)
	END
	ELSE
	BEGIN
		SET @strWhere = ''
	END

	SET @strSelect = 'SELECT DomainID, Score, Count(*) AS Total 
					  From [CSIDomainScore] 
					  INNER JOIN [CSIDomain] ON [CSIDomainScore].[CSIDomainID] = [CSIDomain].[CSIDomainID] 
					  INNER JOIN [CSI] ON [CSIDomain].[CSIID] = [CSI].[CSIID] 
					  INNER JOIN [Child] ON [CSI].[ChildID] = [Child].[ChildID] 
					  LEFT JOIN [ChildPartner] ON [Child].[ChildID] = [ChildPartner].[ChildID] 
					  LEFT JOIN [Partner] ON [ChildPartner].[PartnerID] = [Partner].[PartnerID] 
					  WHERE IndexDate = (SELECT MAX(IndexDate) 
										 FROM CSI WHERE ChildID = Child.ChildID) AND [Score] > 0 AND 
											(SELECT TOP 1 [ChildStatusHistory].[ChildStatusID] 
											 FROM [ChildStatusHistory] WHERE ChildID = Child.ChildID  AND [EffectiveDate] <= getdate() ORDER BY [EffectiveDate] DESC, [ChildStatusHistoryID]) = 1 '
					  + @strWhere + ' GROUP BY [DomainID], Score ORDER BY [DomainID], Score'
	EXEC(@strSelect)
END
");

            CreateStoredProcedure("dbo.DeleteChildHousehold", p => new { ChildHouseholdID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.DeleteChildHousehold
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
**	@ChildID
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
	DELETE FROM ChildHousehold
	WHERE ChildHouseholdID = @ChildHouseholdID
END
");

            CreateStoredProcedure("dbo.SearchChildrenByLetter", p => new { letter = p.String(maxLength: 4), partnerID = p.Int(), }, @"
DECLARE @strWhere VARCHAR(8000)
DECLARE @strSelect VARCHAR(8000)

/******************************************************************************
**	File: 
**	Name: dbo.SearchChildrenByLetter
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
		SET @strWhere = 'WHERE ChildPartner.PartnerID = ' + CAST(@partnerID AS CHAR) + ' '
		IF @letter <> 'All'
		BEGIN
			SET @strWhere = @strWhere + ' AND LTRIM(RTRIM([LastName])) LIKE ''' + @letter + '%'''
		END
	END
	ELSE
	BEGIN
		SET @strWhere = ''
		IF @letter <> 'All'
		BEGIN
			SET @strWhere = ' WHERE LTRIM(RTRIM([LastName])) LIKE ''' + @letter + '%'''
		END
	END

	SET @strSelect = 'SELECT DISTINCT [FirstName], [LastName], [Child].[ChildID], [Ward].[Description] AS Ward, [Village].[Name] AS Village, [District].[Description] AS District 
					  FROM Child LEFT JOIN ChildPartner ON Child.ChildID = ChildPartner.ChildID 
					  LEFT JOIN Ward ON Child.WardID = Ward.WardID 
					  LEFT JOIN Village ON Child.VillageID = Village.VillageID 
					  LEFT JOIN District ON Child.DistrictID = District.DistrictID ' + @strWhere + ' ORDER BY [LastName], [FirstName]'
	EXEC(@strSelect)
END
");

            CreateStoredProcedure("dbo.GetScoresForCSIDomain", p => new { CSIDomainID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.GetScoresForCSIDomain
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
**	@CSIDomainScoreID						CSIDomainScoreID
**									CSIDomainID
**									QuestionID
**									Score
**									Comments
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
	SELECT CSIDomainScoreID, QuestionID, Score, Comments
	FROM CSIDomainScore
	WHERE CSIDomainID = @CSIDomainID
END
");

            CreateStoredProcedure("dbo.DeleteChildPartner", p => new { ChildPartnerID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.DeleteChildPartner
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
**	@ChildPartnerID
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
	DELETE FROM ChildPartner
	WHERE ChildPartnerID = @ChildPartnerID
END
");

            CreateStoredProcedure("dbo.SetAdminMode", p => new { Mode = p.Boolean(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.SetAdminMode
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
	UPDATE GeneralInfo
	SET AdminMode = @Mode
END
");

            CreateStoredProcedure("dbo.GetScoresForWBTDomain", p => new { WBTDomainID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.GetScoresForWBTDomain
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
**	@WBTDomainScoreID						WBTDomainScoreID
**									WBTDomainID
**									QuestionID
**									Score
**									Comments
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
	SELECT WBTDomainScoreID, QuestionID, Score, Comments
	FROM WBTDomainScore
	WHERE WBTDomainID = @WBTDomainID
END
");

            CreateStoredProcedure("dbo.DeleteChildService", p => new { ChildServiceID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.DeleteChildService
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
**	@ChildID
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
	DELETE FROM ChildService
	WHERE ChildServiceID = @ChildServiceID
END
");

            CreateStoredProcedure("dbo.UnlockChild", p => new { ChildID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.UnlockChild
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
	UPDATE Child
	SET Locked = NULL
	WHERE ChildID = @ChildID
END
");

            CreateStoredProcedure("dbo.GetService", p => new { ServiceID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.GetService
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
**									Description
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
	SELECT Description
	FROM Service
	WHERE ServiceID = @ServiceID
END
");

            CreateStoredProcedure("dbo.DeleteChildStatus", p => new { StatusID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.DeleteChildStatus
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
**	@StatusID
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
	DELETE FROM ChildStatus
	WHERE StatusID = @StatusID
END
");

            CreateStoredProcedure("dbo.UnLockUser", p => new { UserID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.UnLockUser
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
	UPDATE [User]
	SET LoggedOn = 0
	WHERE UserID = @UserID
END
");

            CreateStoredProcedure("dbo.GetServiceProvider", p => new { ServiceProviderID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.GetServiceProvider
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
**	@ServiceProviderID						ServiceProviderID
**									ServiceProviderTypeID
**									Description
**									InformationURL
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
	SELECT ServiceProviderTypeID, Description, InformationURL
	FROM ServiceProvider
	WHERE ServiceProviderID = @ServiceProviderID
END
");

            CreateStoredProcedure("dbo.DeleteChildStatusHistory", p => new { ChildStatusHistoryID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.DeleteChildStatusHistory
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
**	@ChildStatusHistoryID
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
	DELETE FROM ChildStatusHistory
	WHERE ChildStatusHistoryID = @ChildStatusHistoryID
END
");

            CreateStoredProcedure("dbo.UpdateAction", p => new { ActionID = p.Int(), Code = p.String(maxLength: 10), Name = p.String(maxLength: 50), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.UpdateAction
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
**	@ActionID
**	@Code
**	@Name
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
	UPDATE Action
	SET	Code = @Code, 
		Name = @Name
	WHERE ActionID = @ActionID
END
");

            CreateStoredProcedure("dbo.GetServiceProviderReport", p => new { serviceProviderID = p.Int(), searchFrom = p.DateTime(), searchTo = p.DateTime(), }, @"
DECLARE @strWhere VARCHAR(8000)
DECLARE @strSelect VARCHAR(8000)

/******************************************************************************
**	File: 
**	Name: dbo.GetServiceProviderReport
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
	IF (@serviceProviderID > 0) 
	BEGIN
		SET @strWhere = ''
		SET @strWhere = 'WHERE ServiceProvider.ServiceProviderID = ' + CAST(@serviceProviderID AS CHAR) + ' ' +
										'AND IndexDate BETWEEN ''' + CAST(@searchFrom AS VARCHAR) + ''' AND ''' + CAST(@searchTo AS VARCHAR) + ''' '
	END
	ELSE
	BEGIN
		SET @strWhere = ''
		SET @strWhere = 'WHERE IndexDate BETWEEN ''' + CAST(@searchFrom AS VARCHAR) + ''' AND ''' + CAST(@searchTo AS VARCHAR) + ''' '
	END

	SET @strSelect = 
		'SELECT LTRIM(RTRIM(FirstName)) + '' '' + LTRIM(RTRIM(LastName)) As FullName, 
			Child.ChildID AS ChildID, 
			DATEDIFF(YEAR, DateOFBirth, GETDATE()) AS Age, 
			DATENAME(d, CSI.IndexDate) + '' '' + DATENAME(m, CSI.IndexDate) + '' '' + DATENAME(YEAR, CSI.IndexDate) AS CSIDate,
			CASE CSIDomain.DomainID WHEN 1 THEN ''Food & Nutrition''
				WHEN 2 THEN ''Shelter & Care'' 
				WHEN 3 THEN ''Protection''
				WHEN 4 THEN ''Health''
				WHEN 5 THEN ''Psychosocial''
				WHEN 6 THEN ''Education & Skills'' END AS Domain 
		FROM Child 
		INNER JOIN CSI ON Child.ChildID = CSI.ChildID 
		INNER JOIN CSIDomain ON CSI.CSIID = CSIDomain.CSIID
		INNER JOIN CSIDomainSupportService ON CSIDomain.CSIDomainID = CSIDomainSupportService.CSIDomainID
		INNER JOIN ServiceProvider ON CSIDomainSupportService.ServiceProviderID = ServiceProvider.ServiceProviderID '
		+ @strWhere + ' ORDER BY LTRIM(RTRIM([LastName])), LTRIM(RTRIM([FirstName])) '
	EXEC(@strSelect)
END
");

            CreateStoredProcedure("dbo.DeleteCommunityCouncil", p => new { CommunityCouncilID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.DeleteCommunityCouncil
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
**	@CommunityCouncilID
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
	DELETE FROM CommunityCouncil
	WHERE CommunityCouncilID = @CommunityCouncilID
END
");

            CreateStoredProcedure("dbo.UpdateAdult", p => new { AdultID = p.Int(), FirstName = p.String(maxLength: 30), LastName = p.String(maxLength: 30), Gender = p.String(maxLength: 1), IDNumber = p.String(maxLength: 20), Passport = p.Int(), DateOfBirth = p.DateTime(), MaritalStatusID = p.String(maxLength: 1), HIV = p.String(maxLength: 1), ContactNo = p.String(maxLength: 20), CreatedDate = p.DateTime(), CreatedUser = p.Int(), LastUpdatedDate = p.DateTime(), LastUpdatedUser = p.Int(), }, @"

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
	CreatedUser = @CreatedUser, 
	LastUpdatedDate = @LastUpdatedDate, 
	@LastUpdatedUser = LastUpdatedUser
	WHERE AdultID = @AdultID
END
");

            CreateStoredProcedure("dbo.GetServiceProviders", @"

/******************************************************************************
**	File: 
**	Name: dbo.GetServiceProviders
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
	SELECT ServiceProviderID, ServiceProviderTypeID, Description, InformationURL
	FROM ServiceProvider
END
");

            CreateStoredProcedure("dbo.DeleteCSI", p => new { CSIID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.DeleteCSI
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
**	@CSIID
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
	DECLARE @CarePlanDomains TABLE (
	CarePlanDomainID int)

	INSERT INTO @CarePlanDomains (CarePlanDomainID)
	SELECT 	CarePlanDomainID
	FROM 	CarePlanDomain
	WHERE 	CarePlanID IN (SELECT CarePlanID FROM CarePlan WHERE CSIID = @CSIID)
	
	DELETE FROM CarePlanDomainSupportService
	WHERE CarePlanDomainID IN (SELECT CarePlanDomainID FROM @CarePlanDomains)

	DELETE FROM CarePlanDomain
	WHERE CarePlanDomainID IN (SELECT CarePlanDomainID FROM @CarePlanDomains)

	DELETE FROM CarePlan
	WHERE CSIID = @CSIID
	
	DECLARE @CSIDomainScores TABLE (
	CSIDomainScoreID int)

	INSERT INTO @CSIDomainScores (CSIDomainScoreID)
	SELECT 	CSIDomainScoreID
	FROM 	CSIDomainScore
	WHERE 	CSIDomainID IN (SELECT CSIDomainID FROM CSIDomain WHERE CSIID = @CSIID)

	DELETE FROM CSIDomainScore
	WHERE CSIDomainScoreID IN (SELECT CSIDomainScoreID FROM @CSIDomainScores)
	
	DECLARE @CSIDomainSources TABLE (
	CSIDomainSourceID int)

	INSERT INTO @CSIDomainSources (CSIDomainSourceID)
	SELECT 	CSIDomainSourceID
	FROM 	CSIDomainSource
	WHERE 	CSIDomainID IN (SELECT CSIDomainID FROM CSIDomain WHERE CSIID = @CSIID)

	DELETE FROM CSIDomainSource
	WHERE CSIDomainSourceID IN (SELECT CSIDomainSourceID FROM @CSIDomainSources)
	
	DELETE FROM CSIDomain
	WHERE CSIID = @CSIID
	
	DELETE FROM CSIEvent
	WHERE CSIID = @CSIID
	
	DELETE FROM CSI
	WHERE CSIID = @CSIID
END
");

            CreateStoredProcedure("dbo.UpdateAdultHousehold", p => new { AdultHouseholdID = p.Int(), AdultID = p.Int(), HouseholdID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.UpdateAdultHousehold
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
**	@CSIDomainID
**	@CSIID
**	@DomainID
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
	UPDATE AdultHousehold
	SET	AdultID = @AdultID, 
		HouseholdID = @HouseholdID
	WHERE AdultHouseholdID = @AdultHouseholdID
END
");

            CreateStoredProcedure("dbo.GetServiceProviderType", p => new { ServiceProviderTypeID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.GetServiceProviderType
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
**	@ServiceProviderTypeID						ServiceProviderTypeID
**									Description
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
	SELECT Description
	FROM ServiceProviderType
	WHERE ServiceProviderTypeID = @ServiceProviderTypeID
END
");

            CreateStoredProcedure("dbo.DeleteCSIContact", p => new { CSIContactID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.DeleteCSIContact
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
**	@CSIContactID
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
	DELETE FROM CSIContact
	WHERE CSIContactID = @CSIContactID
END
");

            CreateStoredProcedure("dbo.UpdateAdultService", p => new { AdultServiceID = p.Int(), AdultID = p.Int(), ServiceID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.UpdateAdultService
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
**	@CSIDomainID
**	@CSIID
**	@DomainID
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
	UPDATE AdultService
	SET	AdultID = @AdultID, 
		ServiceID = @ServiceID
	WHERE AdultServiceID = @AdultServiceID
END
");

            CreateStoredProcedure("dbo.GetServiceProviderTypes", @"

/******************************************************************************
**	File: 
**	Name: dbo.GetServiceProviderTypes
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
	SELECT ServiceProviderTypeID, Description
	FROM ServiceProviderType
END
");

            CreateStoredProcedure("dbo.DeleteCSIDomain", p => new { CSIDomainID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.DeleteCSIDomain
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
**	@CSIDomainID
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
	DELETE FROM CSIDomain
	WHERE CSIDomainID = @CSIDomainID
END
");

            CreateStoredProcedure("dbo.UpdateCareGiverRelation", p => new { RelationID = p.Int(), Description = p.String(maxLength: 30), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.UpdateCareGiverRelation
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
**	@RelationID
**	@Description
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
	UPDATE CareGiverRelation
	SET	Description = @Description
	WHERE RelationID = @RelationID
END
");

            CreateStoredProcedure("dbo.GetServices", @"

/******************************************************************************
**	File: 
**	Name: dbo.GetServices
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
	SELECT ServiceID, Description
	FROM Service
END
");

            CreateStoredProcedure("dbo.DeleteCSIDomainScore", p => new { CSIDomainScoreID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.DeleteCSIDomainScore
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
**	@CSIDomainScoreID
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
	DELETE FROM CSIDomainScore
	WHERE CSIDomainScoreID = @CSIDomainScoreID
END
");

            CreateStoredProcedure("dbo.UpdateCarePlan", p => new { CarePlanID = p.Int(), CSIID = p.Int(), CarePlanDate = p.DateTime(), CreatedUser = p.Int(), CreatedDate = p.DateTime(), LastUpdatedUser = p.Int(), LastUpdatedDate = p.DateTime(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.UpdateCarePlan
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
**	@CarePlanID
**	@CSIID
**	@CarePlanDate
**	@CreatedUser
**	@CreatedDate
**	@LastUpdatedUser
**	@LastUpdatedDate
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
	UPDATE CarePlan
	SET	CSIID = @CSIID, 
		CarePlanDate = @CarePlanDate, 
		CreatedUserID = @CreatedUser, 
		CreatedDate = @CreatedDate, 
		LastUpdatedUserID = @LastUpdatedUser, 
		LastUpdatedDate = @LastUpdatedDate
	WHERE CarePlanID = @CarePlanID
END
");

            CreateStoredProcedure("dbo.GetServicesByAdultID", p => new { AdultID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.GetServicesByAdultID
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
	SELECT AdultServiceID, AdultID, ServiceID
	FROM AdultService
	WHERE AdultID = @AdultID
END
");

            CreateStoredProcedure("dbo.DeleteCSIDomainSource", p => new { CSIDomainSourceID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.DeleteCSIDomainSource
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
**	@CSIDomainSourceID
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
	DELETE FROM CSIDomainSource
	WHERE CSIDomainSourceID = @CSIDomainSourceID
END
");

            CreateStoredProcedure("dbo.UpdateCarePlanDomain", p => new { CarePlanDomainID = p.Int(), CarePlanID = p.Int(), DomainID = p.Int(), Actions = p.String(storeType: "varchar(max)"), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.UpdateCarePlanDomain
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
**	@CarePlanDomainID
**	@CarePlanID
**	@DomainID
**	@Actions
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
	UPDATE CarePlanDomain
	SET	CarePlanID = @CarePlanID, 
		DomainID = @DomainID, 
		Actions = @Actions
	WHERE CarePlanDomainID = @CarePlanDomainID
END
");

            CreateStoredProcedure("dbo.GetServicesByChildID", p => new { ChildID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.GetServicesByChildID
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
	SELECT ChildServiceID, ChildID, ServiceID
	FROM ChildService
	WHERE ChildID = @ChildID
END
");

            CreateStoredProcedure("dbo.DeleteCSIDomainSupportService", p => new { CSIDomainSupportServiceID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.DeleteCSIDomainSupportService
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
**	@CSIDomainSupportServiceID
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
	DELETE FROM CSIDomainSupportService
	WHERE CSIDomainSupportServiceID = @CSIDomainSupportServiceID
END
");

            CreateStoredProcedure("dbo.UpdateCarePlanDomainSupportService", p => new { CarePlanDomainSupportServiceID = p.Int(), CarePlanDomainID = p.Int(), SupportServiceTypeID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.UpdateCarePlanDomainSupportService
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
**	@CarePlanDomainSupportServiceID
**	@CarePlanDomainID
**	@SupportServiceTypeID
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
	UPDATE CarePlanDomainSupportService
	SET	CarePlanDomainID = @CarePlanDomainID, 
		SupportServiceTypeID = @SupportServiceTypeID
	WHERE CarePlanDomainSupportServiceID = @CarePlanDomainSupportServiceID
END
");

            CreateStoredProcedure("dbo.GetSite", p => new { SiteID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.GetSite
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
**	@SiteID						SiteID
**									SiteName
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
	SELECT SiteName
	FROM Site
	WHERE SiteID = @SiteID
END
");

            CreateStoredProcedure("dbo.DeleteCSIEvent", p => new { CSIEventID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.DeleteCSIEvent
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
**	@CSIEventID
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
	DELETE FROM CSIEvent
	WHERE CSIEventID = @CSIEventID
END
");

            CreateStoredProcedure("dbo.UpdateChild", p => new { ChildID = p.Int(), FirstName = p.String(maxLength: 30), LastName = p.String(maxLength: 30), Gender = p.String(maxLength: 1), DateOfBirth = p.DateTime(), DateOfBirthUnknown = p.String(maxLength: 5), WardID = p.Int(), VillageID = p.Int(), DistrictID = p.Int(), Guardian = p.String(maxLength: 50), GuardianRelationID = p.Int(), GuardianIdNo = p.String(maxLength: 15), ContactNo = p.String(maxLength: 15), Notes = p.String(maxLength: 1000), CreatedDate = p.DateTime(), CreatedUser = p.Int(), LastUpdatedDate = p.DateTime(), LastUpdatedUser = p.Int(), Locked = p.Int(), PrincipalChief = p.String(maxLength: 50), VillageChief = p.String(maxLength: 50), CommunityCouncilID = p.Int(), HIV = p.String(maxLength: 1), SchoolName = p.String(maxLength: 50), SchoolContactNo = p.String(maxLength: 20), DisabilityNotes = p.String(), PrincipalName = p.String(maxLength: 100), PrincipalContactNo = p.String(maxLength: 20), TeacherName = p.String(maxLength: 100), TeacherContactNo = p.String(maxLength: 20), OVCTypeID = p.Int(), HeadName = p.String(maxLength: 100), HeadContactNo = p.String(maxLength: 20), Address = p.String(maxLength: 200), Grade = p.String(maxLength: 2), Class = p.String(maxLength: 1), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.UpdateChild
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
**	@ChildID
**	@FirstName
**	@LastName
**	@Gender
**	@DateOfBirth
**	@DateOfBirthUnknown
**	@WardID
**	@VillageID
**	@DistrictID
**	@Guardian
**	@GuardianRelationID
**	@Notes
**	@CreatedDate
**	@CreatedUser
**	@LastUpdatedDate
**	@LastUpdatedUser
**	@Locked
**	@PrincipalChief
**	@VillageChief
**	@CommunityCouncilID
**	@HIV
**	@SchoolName
**	@SchoolContactNo
**	@DisabilityNotes
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
	UPDATE Child
	SET	FirstName = @FirstName, 
		LastName = @LastName, 
		Gender = @Gender, 
		DateOfBirth = @DateOfBirth, 
		DateOfBirthUnknown = @DateOfBirthUnknown, 
		WardID = @WardID, 
		VillageID = @VillageID, 
		DistrictID = @DistrictID, 
		Guardian = @Guardian, 
		GuardianRelationID = @GuardianRelationID, 
		GuardianIdNo = @GuardianIdNo,
		ContactNo = @ContactNo,
		Notes = @Notes, 
		CreatedDate = @CreatedDate, 
		CreatedUserID = @CreatedUser, 
		LastUpdatedDate = @LastUpdatedDate, 
		LastUpdatedUserID = @LastUpdatedUser, 
		Locked = @Locked, 
		PrincipalChief = @PrincipalChief, 
		VillageChief = @VillageChief, 
		CommunityCouncilID = @CommunityCouncilID, 
		HIV = @HIV,
		SchoolName = @SchoolName,
		SchoolContactNo = @SchoolContactNo,
		DisabilityNotes = @DisabilityNotes,
		PrincipalName = @PrincipalName,
		PrincipalContactNo = @PrincipalContactNo,
		TeacherName = @TeacherName,
		TeacherContactNo = @TeacherContactNo,
		OVCTypeID = @OVCTypeID,
		HeadName = @HeadName,
		HeadContactNo = @HeadContactNo,
		Address = @Address,
		Class = @Class
	WHERE ChildID = @ChildID
END
");

            CreateStoredProcedure("dbo.GetSites", @"

/******************************************************************************
**	File: 
**	Name: dbo.GetSites
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
	SELECT SiteID, SiteName
	FROM Site
END
");

            CreateStoredProcedure("dbo.DeleteDisability", p => new { DisabilityID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.DeleteDisability
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
**	@DisabilityID
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
	DELETE FROM Disability
	WHERE DisabilityID = @DisabilityID
END
");

            CreateStoredProcedure("dbo.UpdateChildDisability", p => new { ChildDisabilityID = p.Int(), ChildID = p.Int(), DisabilityID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.UpdateChildDisability
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
**	@ChildDisabilityID
**	@ChildID
**	@DisabilityID
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
	UPDATE ChildDisability
	SET	ChildID = @ChildID, 
		DisabilityID = @DisabilityID
	WHERE ChildDisabilityID = @ChildDisabilityID
END
");

            CreateStoredProcedure("dbo.GetSource", p => new { SourceID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.GetSource
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
**	@SourceID						SourceID
**									Description
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
	SELECT Description
	FROM Source
	WHERE SourceID = @SourceID
END
");

            CreateStoredProcedure("dbo.DeleteDistrict", p => new { DistrictID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.DeleteDistrict
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
**	@DistrictID
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
	DELETE FROM District
	WHERE DistrictID = @DistrictID
END
");

            CreateStoredProcedure("dbo.UpdateChildHousehold", p => new { ChildHouseholdID = p.Int(), ChildID = p.Int(), HouseholdID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.UpdateChildHousehold
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
**	@CSIDomainID
**	@CSIID
**	@DomainID
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
	UPDATE ChildHousehold
	SET	ChildID = @ChildID, 
		HouseholdID = @HouseholdID
	WHERE ChildHouseholdID = @ChildHouseholdID
END
");

            CreateStoredProcedure("dbo.GetSources", @"

/******************************************************************************
**	File: 
**	Name: dbo.GetSources
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
	SELECT SourceID, Description
	FROM Source
END
");

            CreateStoredProcedure("dbo.DeleteDomain", p => new { DomainID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.DeleteDomain
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
**	@DomainID
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
	DELETE FROM Domain
	WHERE DomainID = @DomainID
END
");

            CreateStoredProcedure("dbo.UpdateChildPartner", p => new { ChildPartnerID = p.Int(), ChildID = p.Int(), PartnerID = p.Int(), EffectiveDate = p.DateTime(), Active = p.String(maxLength: 5), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.UpdateChildPartner
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
**	@ChildPartnerID
**	@ChildID
**	@PartnerID
**	@EffectiveDate
**	@Active
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
	UPDATE ChildPartner
	SET	ChildID = @ChildID, 
		PartnerID = @PartnerID, 
		EffectiveDate = @EffectiveDate, 
		Active = @Active
	WHERE ChildPartnerID = @ChildPartnerID
END
");

            CreateStoredProcedure("dbo.GetSourcesForCSIDomain", p => new { CSIDomainID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.GetSourcesForCSIDomain
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
**	@CSIDomainSourceID						CSIDomainSourceID
**									CSIDomainID
**									SourceID
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
	SELECT CSIDomainSourceID, CSIDomainID, SourceID
	FROM CSIDomainSource
	WHERE CSIDomainID = @CSIDomainID
END
");

            CreateStoredProcedure("dbo.DeleteEvent", p => new { EventID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.DeleteEvent
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
**	@EventID
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
	DELETE FROM Event
	WHERE EventID = @EventID
END
");

            CreateStoredProcedure("dbo.UpdateChildService", p => new { ChildServiceID = p.Int(), ChildID = p.Int(), ServiceID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.UpdateChildService
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
	UPDATE ChildService
	SET	ChildID = @ChildID, 
		ServiceID = @ServiceID
	WHERE ChildServiceID = @ChildServiceID
END
");

            CreateStoredProcedure("dbo.GetStatusHistoryComplete", p => new { childID = p.Int(), }, @"
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
									  FROM ([ChildStatusHistory] INNER JOIN ChildStatus ON ChildStatusHistory.StatusID = ChildStatus.StatusID) 
									  INNER JOIN [User] ON [ChildStatusHistory].[CreatedUser] = [User].[UserID] 
									  WHERE [ChildID] = ' + CAST(@ChildID AS CHAR) + ' ORDER BY [EffectiveDate] DESC, [ChildStatusHistoryID] DESC '
	EXEC(@strSelect)
END
");

            CreateStoredProcedure("dbo.DeleteFollowUp", p => new { FollowUpID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.DeleteFollowUp
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
**	@FollowUpID
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
	DELETE FROM FollowUp
	WHERE FollowUpID = @FollowUpID
END
");

            CreateStoredProcedure("dbo.UpdateChildStatus", p => new { StatusID = p.Int(), Description = p.String(maxLength: 30), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.UpdateChildStatus
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
**	@StatusID
**	@Description
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
	UPDATE ChildStatus
	SET	Description = @Description
	WHERE StatusID = @StatusID
END
");

            CreateStoredProcedure("dbo.GetSupportServicesForCarePlanDomain", p => new { CarePlanDomainID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.GetSupportServicesForCarePlanDomain
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
**	@CarePlanDomainSupportServiceID						CarePlanDomainSupportServiceID
**									CarePlanDomainID
**									SupportServiceTypeID
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
	SELECT CarePlanDomainSupportServiceID, CarePlanDomainID, SupportServiceTypeID
	FROM CarePlanDomainSupportService
	WHERE CarePlanDomainID = @CarePlanDomainID
END
");

            CreateStoredProcedure("dbo.DeleteGuardianRelation", p => new { RelationID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.DeleteGuardianRelation
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
**	@RelationID
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
	DELETE FROM GuardianRelation
	WHERE RelationID = @RelationID
END
");

            CreateStoredProcedure("dbo.UpdateChildStatusHistory", p => new { ChildStatusHistoryID = p.Int(), ChildID = p.Int(), StatusID = p.Int(), EffectiveDate = p.DateTime(), CreatedDate = p.DateTime(), CreatedUser = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.UpdateChildStatusHistory
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
**	@ChildStatusHistoryID
**	@ChildID
**	@StatusID
**	@EffectiveDate
**	@CreatedDate
**	@CreatedUser
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
	UPDATE ChildStatusHistory
	SET	ChildID = @ChildID, 
		ChildStatusID = @StatusID, 
		EffectiveDate = @EffectiveDate, 
		CreatedDate = @CreatedDate, 
		CreatedUserID = @CreatedUser
	WHERE ChildStatusHistoryID = @ChildStatusHistoryID
END
");

            CreateStoredProcedure("dbo.GetSupportServicesForCSIDomain", p => new { CSIDomainID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.GetSupportServicesForCSIDomain
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
**	@CarePlanDomainSupportServiceID						CarePlanDomainSupportServiceID
**									CarePlanDomainID
**									SupportServiceTypeID
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
	SELECT CSIDomainSupportServiceID, CSIDomainID, SupportServiceTypeID, OtherService, ServiceProviderID, OtherServiceProvider
	FROM CSIDomainSupportService
	WHERE CSIDomainID = @CSIDomainID
END
");

            CreateStoredProcedure("dbo.DeleteGuideline", p => new { GuidelineID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.DeleteGuideline
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
**	@GuidelineID
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
	DELETE FROM Guideline
	WHERE GuidelineID = @GuidelineID
END
");

            CreateStoredProcedure("dbo.UpdateCommunityCouncil", p => new { CommunityCouncilID = p.Int(), Description = p.String(maxLength: 30), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.UpdateCommunityCouncil
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
**	@CommunityCouncilID
**	@Description
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
	UPDATE CommunityCouncil
	SET	Description = @Description
	WHERE CommunityCouncilID = @CommunityCouncilID
END
");

            CreateStoredProcedure("dbo.GetSupportServiceType", p => new { SupportServiceTypeID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.GetSupportServiceType
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
**	@SupportServiceTypeID						SupportServiceTypeID
**									DomainID
**									Description
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
	SELECT DomainID, Description, [Default]
	FROM SupportServiceType
	WHERE SupportServiceTypeID = @SupportServiceTypeID
END
");

            CreateStoredProcedure("dbo.DeleteHomeEnvironment", p => new { HomeEnvironmentID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.DeleteHomeEnvironment
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
**	@HomeEnvironmentID
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
	DELETE FROM HomeEnvironment
	WHERE HomeEnvironmentID = @HomeEnvironmentID
END
");

            CreateStoredProcedure("dbo.UpdateCSI", p => new { CSIID = p.Int(), ChildID = p.Int(), IndexDate = p.DateTime(), CreatedUser = p.Int(), CreatedDate = p.DateTime(), LastUpdatedUser = p.Int(), LastUpdatedDate = p.DateTime(), StatusID = p.Int(), Photo = p.Binary(storeType: "image"), Height = p.Decimal(precision: 9, scale: 0), Weight = p.Decimal(precision: 9, scale: 0), BMI = p.Decimal(precision: 9, scale: 0), TakingMedication = p.String(maxLength: 5), MedicationDescription = p.String(storeType: "varchar(max)"), Suggestions = p.String(storeType: "varchar(max)"), DistrictID = p.Int(), Caregiver = p.String(maxLength: 50), CaregiverRelationID = p.Int(), SocialWorkerName = p.String(maxLength: 100), SocialWorkerContactNo = p.String(maxLength: 20), DoctorName = p.String(maxLength: 100), DoctorContactNo = p.String(maxLength: 20), AllergyNotes = p.String(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.UpdateCSI
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
**	@CSIID
**	@ChildID
**	@IndexDate
**	@CreatedUser
**	@CreatedDate
**	@LastUpdatedUser
**	@LastUpdatedDate
**	@StatusID
**	@Photo
**	@Height
**	@Weight
**	@BMI
**	@TakingMedication
**	@MedicationDescription
**	@Suggestions
**	@DistrictID
**	@Caregiver
**	@CaregiverRelationID
**	@SocialWorkerName
**	@SocialWorkerContactNo
**	@DoctorName
**	@DoctorContactNo
**	@AllergyNotes
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
	UPDATE CSI
	SET	ChildID = @ChildID, 
		IndexDate = @IndexDate, 
		CreatedUserID = @CreatedUser, 
		CreatedDate = @CreatedDate, 
		LastUpdatedUserID = @LastUpdatedUser, 
		LastUpdatedDate = @LastUpdatedDate, 
		StatusID = @StatusID, 
		Photo = @Photo, 
		Height = @Height, 
		Weight = @Weight, 
		BMI = @BMI, 
		TakingMedication = @TakingMedication, 
		MedicationDescription = @MedicationDescription, 
		Suggestions = @Suggestions, 
		DistrictID = @DistrictID, 
		Caregiver = @Caregiver, 
		CaregiverRelationID = @CaregiverRelationID,
		SocialWorkerName = @SocialWorkerName,
		SocialWorkerContactNo = @SocialWorkerContactNo,
		DoctorName = @DoctorName,
		DoctorContactNo = @DoctorContactNo,
		AllergyNotes = @AllergyNotes
	WHERE CSIID = @CSIID
END
");

            CreateStoredProcedure("dbo.GetSupportServiceTypes", @"

/******************************************************************************
**	File: 
**	Name: dbo.GetSupportServiceTypes
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
	SELECT SupportServiceTypeID, DomainID, [Description], [Default]
	FROM SupportServiceType
END
");

            CreateStoredProcedure("dbo.DeleteHousehold", p => new { HouseholdID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.DeleteHousehold
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
**	@ActionID
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
	DELETE FROM ChildHousehold
	WHERE HouseholdID = @HouseholdID
	
	DELETE FROM AdultHousehold
	WHERE HouseholdID = @HouseholdID
	
	DELETE FROM Household
	WHERE HouseholdID = @HouseholdID
END
");

            CreateStoredProcedure("dbo.UpdateCSIContact", p => new { CSIContactID = p.Int(), CSIID = p.Int(), ContactPerson = p.String(maxLength: 100), Telephone = p.String(maxLength: 30), ContactType = p.String(maxLength: 50), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.UpdateCSIContact
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
**	@CSIEventID
**	@CSIID
**	@ContactPerson
**	@telephone
**	@ContactType
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
	UPDATE CSIContact
	SET	CSIID = @CSIID, 
		ContactPerson = @ContactPerson, 
		Telephone = @Telephone, 
		ContactType = @ContactType
	WHERE CSIContactID = @CSIContactID
END
");

            CreateStoredProcedure("dbo.GetUser", p => new { UserID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.GetUser
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
**	@UserID						UserID
**									Username
**									Password
**									FirstName
**									LastName
**									Admin
**									DefSite
**									LoggedOn
**									PartnerID
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
	SELECT Username, Password, FirstName, LastName, Admin, DefSite, LoggedOn, PartnerID
	FROM [User]
	WHERE UserID = @UserID
END
");

            CreateStoredProcedure("dbo.DeleteHouseholdProject", p => new { HouseholdProjectID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.DeleteHouseholdProject
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
**	@UserActionID
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
	DELETE FROM HouseholdProject
	WHERE HouseholdProjectID = @HouseholdProjectID
END
");

            CreateStoredProcedure("dbo.UpdateCSIDomain", p => new { CSIDomainID = p.Int(), CSIID = p.Int(), DomainID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.UpdateCSIDomain
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
**	@CSIDomainID
**	@CSIID
**	@DomainID
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
	UPDATE CSIDomain
	SET	CSIID = @CSIID, 
		DomainID = @DomainID
	WHERE CSIDomainID = @CSIDomainID
END
");

            CreateStoredProcedure("dbo.GetUserAction", p => new { UserActionID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.GetUserAction
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
**	@UserActionID						UserActionID
**									UserID
**									SiteID
**									ActionID
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
	SELECT UserID, SiteID, ActionID
	FROM UserAction
	WHERE UserActionID = @UserActionID
END
");

            CreateStoredProcedure("dbo.DeleteOutcome", p => new { OutcomeID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.DeleteOutcome
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
**	@OutcomeID
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
	DELETE FROM Outcome
	WHERE OutcomeID = @OutcomeID
END
");

            CreateStoredProcedure("dbo.UpdateCSIDomainScore", p => new { CSIDomainScoreID = p.Int(), CSIDomainID = p.Int(), QuestionID = p.Int(), Score = p.Int(), Comments = p.String(storeType: "varchar(max)"), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.UpdateCSIDomainScore
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
**	@CSIDomainScoreID
**	@CSIDomainID
**	@QuestionID
**	@Score
**	@Comments
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
	UPDATE CSIDomainScore
	SET	CSIDomainID = @CSIDomainID, 
		QuestionID = @QuestionID, 
		Score = @Score, 
		Comments = @Comments
	WHERE CSIDomainScoreID = @CSIDomainScoreID
END
");

            CreateStoredProcedure("dbo.GetUserByUsername", p => new { UserName = p.String(maxLength: 50), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.GetUserByUsername
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
**	@UserID						UserID
**									Username
**									Password
**									FirstName
**									LastName
**									Admin
**									DefSite
**									LoggedOn
**									PartnerID
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
	SELECT UserID, Username, Password, FirstName, LastName, Admin, DefSite, LoggedOn, PartnerID
	FROM [User]
	WHERE UserName = @Username
END
");

            CreateStoredProcedure("dbo.DeleteOVCType", p => new { OVCTypeID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.DeleteOVCType
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
**	@ServiceProviderTypeID
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
	DELETE FROM OVCType
	WHERE OVCTypeID = @OVCTypeID
END
");

            CreateStoredProcedure("dbo.UpdateCSIDomainSource", p => new { CSIDomainSourceID = p.Int(), CSIDomainID = p.Int(), SourceID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.UpdateCSIDomainSource
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
**	@CSIDomainSourceID
**	@CSIDomainID
**	@SourceID
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
	UPDATE CSIDomainSource
	SET	CSIDomainID = @CSIDomainID, 
		SourceID = @SourceID
	WHERE CSIDomainSourceID = @CSIDomainSourceID
END
");

            CreateStoredProcedure("dbo.GetUsers", @"

/******************************************************************************
**	File: 
**	Name: dbo.GetUsers
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
**	@UserID						UserID
**									Username
**									Password
**									FirstName
**									LastName
**									Admin
**									DefSite
**									LoggedOn
**									PartnerID
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
	SELECT UserID, Username, Password, FirstName, LastName, Admin, DefSite, LoggedOn, PartnerID
	FROM [User]
END
");

            CreateStoredProcedure("dbo.DeleteParticipantRegistration", p => new { ParticipantRegistrationID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.DeleteParticipantRegistration
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
**	@ActionID
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
	DELETE FROM ParticipantRegistration
	WHERE ParticipantRegistrationID = @ParticipantRegistrationID
END
");

            CreateStoredProcedure("dbo.UpdateCSIDomainSupportService", p => new { CSIDomainSupportServiceID = p.Int(), CSIDomainID = p.Int(), SupportServiceTypeID = p.Int(), OtherService = p.String(maxLength: 50), ServiceProviderID = p.Int(), OtherServiceProvider = p.String(maxLength: 50), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.UpdateCSIDomainSupportService
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
**	@CSIDomainSupportServiceID
**	@CSIDomainID
**	@SupportServiceTypeID
**	@OtherService
**	@ServiceProviderID
**	@OtherServiceProvider
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
	UPDATE CSIDomainSupportService
	SET	CSIDomainID = @CSIDomainID, 
		SupportServiceTypeID = @SupportServiceTypeID, 
		OtherService = @OtherService, 
		ServiceProviderID = @ServiceProviderID, 
		OtherServiceProvider = @OtherServiceProvider
	WHERE CSIDomainSupportServiceID = @CSIDomainSupportServiceID
END
");

            CreateStoredProcedure("dbo.GetVillage", p => new { VillageID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.GetVillage
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
**	@VillageID						VillageID
**									Name
**									DistrictID
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
	SELECT Name, DistrictID
	FROM Village
	WHERE VillageID = @VillageID
END
");

            CreateStoredProcedure("dbo.DeletePartner", p => new { PartnerID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.DeletePartner
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
**	@PartnerID
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
	DELETE FROM Partner
	WHERE PartnerID = @PartnerID
END
");

            CreateStoredProcedure("dbo.UpdateCSIEvent", p => new { CSIEventID = p.Int(), CSIID = p.Int(), EventID = p.Int(), Comments = p.String(storeType: "varchar(max)"), EffectiveDate = p.DateTime(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.UpdateCSIEvent
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
**	@CSIEventID
**	@CSIID
**	@EventID
**	@Comments
**	@EffectiveDate
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
	UPDATE CSIEvent
	SET	CSIID = @CSIID, 
		EventID = @EventID, 
		Comments = @Comments, 
		EffectiveDate = @EffectiveDate
	WHERE CSIEventID = @CSIEventID
END
");

            CreateStoredProcedure("dbo.GetVillages", @"

/******************************************************************************
**	File: 
**	Name: dbo.GetVillages
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
	SELECT VillageID, [Name], DistrictID
	FROM Village
END
");

            CreateStoredProcedure("dbo.DeleteQuestion", p => new { QuestionID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.DeleteQuestion
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
**	@QuestionID
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
	DELETE FROM Question
	WHERE QuestionID = @QuestionID
END
");

            CreateStoredProcedure("dbo.UpdateDisability", p => new { DisabilityID = p.Int(), Description = p.String(maxLength: 30), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.UpdateDisability
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
**	@DisabilityID
**	@Description
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
	UPDATE Disability
	SET	Description = @Description
	WHERE DisabilityID = @DisabilityID
END
");

            CreateStoredProcedure("dbo.GetVillagesFiltered", p => new { FilterText = p.String(maxLength: 100), DistrictID = p.Int(), }, @"
DECLARE @sqlSelect varchar(1000)
/******************************************************************************
**	File: 
**	Name: dbo.GetVillagesFiltered
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
	SET @sqlSelect = 
						 'SELECT VillageID, [Name] ' +
						 'FROM Village ' +
						 'WHERE DistrictID = ' + cast(@DistrictID as char) + 
						 'AND [Name] LIKE ''' + @FilterText + '%'' ' +
						 'ORDER BY [Name] ASC '
	
	exec (@sqlSelect)
END
");

            CreateStoredProcedure("dbo.DeleteRecipientType", p => new { RecipientTypeID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.DeleteRecipientType
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
**	@RecipientTypeID
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
	DELETE FROM RecipientType
	WHERE RecipientTypeID = @RecipientTypeID
END
");

            CreateStoredProcedure("dbo.UpdateDistrict", p => new { DistrictID = p.Int(), Description = p.String(maxLength: 50), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.UpdateDistrict
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
**	@DistrictID
**	@Description
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
	UPDATE District
	SET	Description = @Description
	WHERE DistrictID = @DistrictID
END
");

            CreateStoredProcedure("dbo.GetVillagesForDistrict", p => new { DistrictID = p.Int(), }, @"
DECLARE @strWhere VARCHAR(8000)
DECLARE @strSelect VARCHAR(8000)

/******************************************************************************
**	File: 
**	Name: dbo.GetVillagesForDistrict
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
	SET @strSelect = 'SELECT [VillageID], [Name], [DistrictID] FROM [Village] WHERE [DistrictID] = ' + CAST(@DistrictID AS CHAR)
	EXEC(@strSelect)
END
");

            CreateStoredProcedure("dbo.DeleteService", p => new { ServiceID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.DeleteService
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
**	@ServiceProviderTypeID
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
	DELETE FROM Service
	WHERE ServiceID = @ServiceID
END
");

            CreateStoredProcedure("dbo.UpdateDomain", p => new { DomainID = p.Int(), Description = p.String(maxLength: 30), DomainCode = p.String(maxLength: 5), DomainColor = p.String(maxLength: 30), Guidelines = p.String(storeType: "varchar(max)"), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.UpdateDomain
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
**	@DomainID
**	@Description
**	@DomainCode
**  @DomainColor
**	@Guidelines
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
	UPDATE Domain
	SET	Description = @Description,
		DomainColor = @DomainColor,
		DomainCode = @DomainCode,
		Guidelines = @Guidelines
	WHERE DomainID = @DomainID
END
");

            CreateStoredProcedure("dbo.GetWard", p => new { WardID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.GetWard
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
**	@WardID						WardID
**									Description
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
	SELECT Description
	FROM Ward
	WHERE WardID = @WardID
END
");

            CreateStoredProcedure("dbo.DeleteServiceProvider", p => new { ServiceProviderID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.DeleteServiceProvider
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
**	@ServiceProviderID
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
	DELETE FROM ServiceProvider
	WHERE ServiceProviderID = @ServiceProviderID
END
");

            CreateStoredProcedure("dbo.UpdateEvent", p => new { EventID = p.Int(), Description = p.String(maxLength: 50), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.UpdateEvent
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
**	@EventID
**	@Description
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
	UPDATE Event
	SET	Description = @Description
	WHERE EventID = @EventID
END
");

            CreateStoredProcedure("dbo.GetWards", @"

/******************************************************************************
**	File: 
**	Name: dbo.GetWards
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
	SELECT WardID, Description
	FROM Ward
END
");

            CreateStoredProcedure("dbo.DeleteServiceProviderType", p => new { ServiceProviderTypeID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.DeleteServiceProviderType
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
**	@ServiceProviderTypeID
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
	DELETE FROM ServiceProviderType
	WHERE ServiceProviderTypeID = @ServiceProviderTypeID
END
");

            CreateStoredProcedure("dbo.UpdateFollowUp", p => new { FollowUpID = p.Int(), ChildID = p.Int(), DateOfFollowUp = p.DateTime(), CreatedUser = p.Int(), CreatedDate = p.DateTime(), RecipientTypeID = p.Int(), OutcomeID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.UpdateFollowUp
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
**	@FollowUpID
**	@ChildID
**	@DateOfFollowUp
**	@CreatedUser
**	@CreatedDate
**	@RecipientTypeID
**	@OutcomeID
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
	UPDATE FollowUp
	SET	ChildID = @ChildID, 
		DateOfFollowUp = @DateOfFollowUp, 
		CreatedUserID = @CreatedUser, 
		CreatedDate = @CreatedDate, 
		RecipientTypeID = @RecipientTypeID, 
		OutcomeID = @OutcomeID
	WHERE FollowUpID = @FollowUpID
END
");

            CreateStoredProcedure("dbo.GetWBT", p => new { WBTID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.GetWBT
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
**	@CSIID						CSIID
**									ChildID
**									WBTDate
**									CreatedUser
**									CreatedDate
**									LastUpdatedUser
**									LastUpdatedDate
**									StatusID
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
	SELECT ChildID, WBTDate, CreatedUser, CreatedDate, LastUpdatedUser, LastUpdatedDate, StatusID, ChildIdNo, TestAdmin
	FROM WBT
	WHERE WBTID = @WBTID
END
");

            CreateStoredProcedure("dbo.DeleteSite", p => new { SiteID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.DeleteSite
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
**	@SiteID
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
	DELETE FROM Site
	WHERE SiteID = @SiteID
END
");

            CreateStoredProcedure("dbo.UpdateGuardianRelation", p => new { RelationID = p.Int(), Description = p.String(maxLength: 30), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.UpdateGuardianRelation
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
**	@RelationID
**	@Description
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
	UPDATE GuardianRelation
	SET	Description = @Description
	WHERE RelationID = @RelationID
END
");

            CreateStoredProcedure("dbo.GetWBTDomain", p => new { WBTDomainID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.GetWBTDomain
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
**	@WBTDomainID						WBTDomainID
**									WBTID
**									DomainID
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
	SELECT WBTID, DomainID
	FROM WBTDomain
	WHERE WBTDomainID = @WBTDomainID
END
");

            CreateStoredProcedure("dbo.DeleteSource", p => new { SourceID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.DeleteSource
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
**	@SourceID
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
	DELETE FROM Source
	WHERE SourceID = @SourceID
END
");

            CreateStoredProcedure("dbo.UpdateGuideline", p => new { GuidelineID = p.Int(), DescriptionEnglish = p.String(storeType: "varchar(max)"), DescriptionSesotho = p.String(storeType: "varchar(max)"), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.UpdateGuideline
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
**	@GuidelineID
**	@DescriptionEnglish
**	@DescriptionSesotho
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
	UPDATE Guideline
	SET	DescriptionEnglish = @DescriptionEnglish, 
		DescriptionSesotho = @DescriptionSesotho
	WHERE GuidelineID = @GuidelineID
END
");

            CreateStoredProcedure("dbo.GetWBTDomainScore", p => new { WBTDomainScoreID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.GetWBTDomainScore
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
**	@WBTDomainScoreID						WBTDomainScoreID
**									WBTDomainID
**									QuestionID
**									Score
**									Comments
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
	SELECT WBTDomainID, QuestionID, Score, Comments
	FROM WBTDomainScore
	WHERE WBTDomainScoreID = @WBTDomainScoreID
END
");

            CreateStoredProcedure("dbo.DeleteSupportServiceType", p => new { SupportServiceTypeID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.DeleteSupportServiceType
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
**	@SupportServiceTypeID
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
	DELETE FROM SupportServiceType
	WHERE SupportServiceTypeID = @SupportServiceTypeID
END
");

            CreateStoredProcedure("dbo.UpdateHomeEnvironment", p => new { HomeEnvironmentID = p.Int(), HouseholdID = p.Int(), HouseMembersCount = p.Int(), HouseOVCsMale = p.Int(), HouseOVCsFemale = p.Int(), HouseMinorsMale = p.Int(), HouseMinorsFemale = p.Int(), HouseAdequate = p.String(maxLength: 1), HouseAdequateSpecify = p.String(maxLength: 2000), FoodSourceFarming = p.Boolean(), FoodSourceRelative = p.Boolean(), FoodSourceDonations = p.Boolean(), FoodSourceNoReliable = p.Boolean(), AllHHMembersWell = p.String(maxLength: 1), HHMembersOccassionallyIll = p.String(maxLength: 1), HHMembersFrequentlySick = p.String(maxLength: 1), OneHHMemberIll = p.String(maxLength: 1), GoToHealthCentre = p.Boolean(), CommunityHealthWorker = p.Boolean(), TraditionalHealer = p.Boolean(), NoHealthCare = p.Boolean(), LatrineInGoodRepair = p.String(maxLength: 1), LatrineInBadRepair = p.String(maxLength: 1), SharingLatrine = p.String(maxLength: 1), NoLatrine = p.String(maxLength: 1), WarmClothing = p.Boolean(), Blanket = p.Boolean(), Shoes = p.Boolean(), SchoolUniform = p.Boolean(), ClothingSpecify = p.String(maxLength: 2000), NeedsFood = p.Boolean(), NeedsSchool = p.Boolean(), NeedsPsychosocial = p.Boolean(), NeedsGuardianship = p.Boolean(), NeedsToiletries = p.Boolean(), NeedsSchoolSupplies = p.Boolean(), NeedsShelter = p.Boolean(), NeedsHealth = p.Boolean(), NoSpecialNeed = p.Boolean(), CaregiverWithDisabilities = p.Boolean(), SpecialNeedsSpecify = p.String(maxLength: 2000), SizeOfUniform = p.Int(), SizeOfShoes = p.Int(), Remarks = p.String(maxLength: 2000), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.UpdateHomeEnvironment
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
	UPDATE HomeEnvironment
	SET	HouseholdID = @HouseholdID, 
			HouseMembersCount = @HouseMembersCount, 
			HouseOVCsMale = @HouseOVCsMale, 
			HouseOVCsFemale = @HouseOVCsFemale, 
			HouseMinorsMale = @HouseMinorsMale, 
			HouseMinorsFemale = @HouseMinorsFemale, 
			HouseAdequate = @HouseAdequate, 
			HouseAdequateSpecify = @HouseAdequateSpecify, 
			FoodSourceFarming = @FoodSourceFarming, 
			FoodSourceRelative = @FoodSourceRelative, 
			FoodSourceDonations = @FoodSourceDonations, 
			FoodSourceNoReliable = @FoodSourceNoReliable, 
			AllHHMembersWell = @AllHHMembersWell, 
			HHMembersOccassionallyIll = @HHMembersOccassionallyIll, 
			HHMembersFrequentlySick = @HHMembersFrequentlySick, 
			OneHHMemberIll = @OneHHMemberIll, 
			GoToHealthCentre = @GoToHealthCentre, 
			CommunityHealthWorker = @CommunityHealthWorker, 
			TraditionalHealer = @TraditionalHealer, 
			NoHealthCare = @NoHealthCare, 
			LatrineInGoodRepair = @LatrineInGoodRepair, 
			LatrineInBadRepair = @LatrineInBadRepair, 
			SharingLatrine = @SharingLatrine, 
			NoLatrine = @NoLatrine, 
			WarmClothing = @WarmClothing, 
			Blanket = @Blanket, 
			Shoes = @Shoes, 
			SchoolUniform = @SchoolUniform, 
			ClothingSpecify = @ClothingSpecify, 
			NeedsFood = @NeedsFood, 
			NeedsSchool = @NeedsSchool, 
			NeedsPsychosocial = @NeedsPsychosocial, 	
			NeedsGuardianship = @NeedsGuardianship, 
			NeedsToiletries = @NeedsToiletries,
			NeedsSchoolSupplies = @NeedsSchoolSupplies, 
			NeedsShelter = @NeedsShelter, 
			NeedsHealth = @NeedsHealth, 
			NoSpecialNeed = @NoSpecialNeed, 
			CaregiverWithDisabilities = @CaregiverWithDisabilities, 
			SpecialNeedsSpecify = @SpecialNeedsSpecify, 
			SizeOfUniform = @SizeOfUniform, 
			SizeOfShoes = @SizeOfShoes, 
			Remarks = @Remarks
	WHERE HomeEnvironmentID = @HomeEnvironmentID
END
");

            CreateStoredProcedure("dbo.DeleteUser", p => new { UserID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.DeleteUser
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
**	@UserID
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
	DELETE FROM [User]
	WHERE UserID = @UserID
END
");

            CreateStoredProcedure("dbo.UpdateHousehold", p => new { HouseholdID = p.Int(), Address = p.String(maxLength: 200), VillageID = p.Int(), DistrictID = p.Int(), Country = p.String(maxLength: 50), ParticipantRegistrationID = p.Int(), CreatedDate = p.DateTime(), CreatedUser = p.Int(), LastUpdatedDate = p.DateTime(), LastUpdatedUser = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.UpdateHousehold
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
	UPDATE Household
	SET	Address = @Address, 
	VillageID = @VillageID, 
	DistrictID = @DistrictID, 
	Country = @Country, 
	ParticipantRegistrationID = @ParticipantRegistrationID,
	CreatedDate = @CreatedDate, 
	CreatedUser = @CreatedUser, 
	LastUpdatedDate = @LastUpdatedDate, 
	LastUpdatedUser = @LastUpdatedUser
	WHERE HouseholdID = @HouseholdID
END
");

            CreateStoredProcedure("dbo.InsertAction", p => new { Code = p.String(maxLength: 10), Name = p.String(maxLength: 50), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.InsertAction
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
**	@Code						@@Identity
**	@Name
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
	INSERT INTO Action (Code, [Name])
	VALUES (@Code, @Name)

	SELECT @@Identity
END
");

            CreateStoredProcedure("dbo.DeleteUserAction", p => new { UserActionID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.DeleteUserAction
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
**	@UserActionID
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
	DELETE FROM UserAction
	WHERE UserActionID = @UserActionID
END
");

            CreateStoredProcedure("dbo.UpdateHouseholdProject", p => new { HouseholdProjectID = p.Int(), HouseholdID = p.Int(), ParticipantRegistrationID = p.Int(), FieldAgentMonitoringID = p.Int(), TrainingRegistryID = p.Int(), CreatedDate = p.DateTime(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.UpdateHouseholdProject
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
**	@UserActionID
**	@UserID
**	@SiteID
**	@ActionID
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
	UPDATE HouseholdProject
	SET	HouseholdID = @HouseholdID, 
		ParticipantRegistrationID = @ParticipantRegistrationID, 
		FieldAgentMonitoringID = @FieldAgentMonitoringID,
		TrainingRegistryID = @TrainingRegistryID,
		CreatedDate = @CreatedDate
	WHERE HouseholdProjectID = @HouseholdProjectID
END
");

            CreateStoredProcedure("dbo.InsertAdult", p => new { FirstName = p.String(maxLength: 30), LastName = p.String(maxLength: 30), Gender = p.String(maxLength: 1), IDNumber = p.String(maxLength: 20), Passport = p.Int(), DateOfBirth = p.DateTime(), MaritalStatusID = p.String(maxLength: 1), HIV = p.String(maxLength: 1), ContactNo = p.String(maxLength: 20), CreatedDate = p.DateTime(), CreatedUser = p.Int(), LastUpdatedDate = p.DateTime(), LastUpdatedUser = p.Int(), }, @"

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
	INSERT INTO Adult (FirstName, LastName, Gender, IDNumber, Passport, MaritalStatusID, HIV, DateOfBirth, ContactNo, CreatedDate, CreatedUser, LastUpdatedDate, LastUpdatedUser)
	VALUES (@FirstName, @LastName, @Gender, @IDNumber, @Passport, @MaritalStatusID, @HIV, @DateOfBirth, @ContactNo, @CreatedDate, @CreatedUser, @LastUpdatedDate, @LastUpdatedUser)

	SELECT @@Identity
END
");

            CreateStoredProcedure("dbo.DeleteVillage", p => new { VillageID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.DeleteVillage
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
**	@VillageID
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
	DELETE FROM Village
	WHERE VillageID = @VillageID
END
");

            CreateStoredProcedure("dbo.UpdateOutcome", p => new { OutcomeID = p.Int(), Code = p.String(maxLength: 10), Description = p.String(maxLength: 50), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.UpdateOutcome
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
**	@OutcomeID
**	@Code
**	@Name
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
	UPDATE Outcome
	SET	Code = @Code, 
		Description = @Description
	WHERE OutcomeID = @OutcomeID
END
");

            CreateStoredProcedure("dbo.InsertAdultHousehold", p => new { AdultID = p.Int(), HouseholdID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.InsertAdultHousehold
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
**	@CSIID						@@Identity
**	@DomainID
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
	INSERT INTO AdultHousehold (AdultID, HouseholdID)
	VALUES (@AdultID, @HouseholdID)

	SELECT @@Identity
END
");

            CreateStoredProcedure("dbo.DeleteWard", p => new { WardID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.DeleteWard
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
**	@WardID
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
	DELETE FROM Ward
	WHERE WardID = @WardID
END
");

            CreateStoredProcedure("dbo.UpdateOVCType", p => new { OVCTypeID = p.Int(), Description = p.String(maxLength: 100), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.UpdateOVCType
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
**	@ServiceProviderTypeID
**	@Description
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
	UPDATE OVCType
	SET	Description = @Description
	WHERE OVCTypeID = @OVCTypeID
END
");

            CreateStoredProcedure("dbo.InsertAdultService", p => new { AdultID = p.Int(), ServiceID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.InsertAdultService
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
**	@CSIID						@@Identity
**	@DomainID
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
	INSERT INTO AdultService (AdultID, ServiceID)
	VALUES (@AdultID, @ServiceID)

	SELECT @@Identity
END
");

            CreateStoredProcedure("dbo.DeleteWBT", p => new { WBTID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.DeleteWBT
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
**	@CSIID
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
	DELETE FROM WBT
	WHERE WBTID = @WBTID
END
");

            CreateStoredProcedure("dbo.UpdateParticipantRegistration", p => new { ParticipantRegistrationID = p.Int(), Enumerator = p.String(maxLength: 100), DateOfReg = p.DateTime(), ProjectAreaID = p.Int(), ActivePersonID = p.Int(), AnyPregnant = p.String(maxLength: 1), CropLand = p.String(maxLength: 1), DistrictID = p.Int(), VillageID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.UpdateParticipantRegistration
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
	UPDATE participantRegistration
	SET	Enumerator = @Enumerator,
	DateOfReg = @DateOfReg,
	ProjectAreaID = @ProjectAreaID,
	ActivePersonID = @ActivePersonID,
	AnyPregnant = @AnyPregnant,
	CropLand = @CropLand,
	DistrictID = @DistrictID,
	VillageID = @VillageID
	WHERE ParticipantRegistrationID = @ParticipantRegistrationID
END
");

            CreateStoredProcedure("dbo.InsertCareGiverRelation", p => new { Description = p.String(maxLength: 30), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.InsertCareGiverRelation
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
**	@Description						@@Identity
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
	INSERT INTO CareGiverRelation (Description)
	VALUES (@Description)

	SELECT @@Identity
END
");

            CreateStoredProcedure("dbo.DeleteWBTDomain", p => new { WBTDomainID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.DeleteWBTDomain
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
**	@WBTDomainID
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
	DELETE FROM WBTDomain
	WHERE WBTDomainID = @WBTDomainID
END
");

            CreateStoredProcedure("dbo.UpdatePartner", p => new { PartnerID = p.Int(), Name = p.String(maxLength: 50), Address = p.String(storeType: "varchar(max)"), ContactNo = p.String(maxLength: 30), FaxNo = p.String(maxLength: 30), ContactName = p.String(maxLength: 60), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.UpdatePartner
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
**	@PartnerID
**	@Name
**	@Address
**	@ContactNo
**	@FaxNo
**	@ContactName
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
	UPDATE Partner
	SET	Name = @Name, 
		Address = @Address, 
		ContactNo = @ContactNo, 
		FaxNo = @FaxNo, 
		ContactName = @ContactName
	WHERE PartnerID = @PartnerID
END
");

            CreateStoredProcedure("dbo.InsertCarePlan", p => new { CSIID = p.Int(), CarePlanDate = p.DateTime(), CreatedUser = p.Int(), CreatedDate = p.DateTime(), LastUpdatedUser = p.Int(), LastUpdatedDate = p.DateTime(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.InsertCarePlan
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
**	@CSIID						@@Identity
**	@CarePlanDate
**	@CreatedUser
**	@CreatedDate
**	@LastUpdatedUser
**	@LastUpdatedDate
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
	INSERT INTO CarePlan (CSIID, CarePlanDate, CreatedUserID, CreatedDate, LastUpdatedUserID, LastUpdatedDate)
	VALUES (@CSIID, @CarePlanDate, @CreatedUser, @CreatedDate, @LastUpdatedUser, @LastUpdatedDate)

	SELECT @@Identity
END
");

            CreateStoredProcedure("dbo.DeleteWBTDomainScore", p => new { WBTDomainScoreID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.DeleteWBTDomainScore
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
**	@WBTDomainScoreID
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
	DELETE FROM WBTDomainScore
	WHERE WBTDomainScoreID = @WBTDomainScoreID
END
");

            CreateStoredProcedure("dbo.UpdateQuestion", p => new { QuestionID = p.Int(), DomainID = p.Int(), Description = p.String(maxLength: 20), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.UpdateQuestion
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
**	@QuestionID
**	@DomainID
**	@Description
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
	UPDATE Question
	SET	DomainID = @DomainID, 
		Description = @Description
	WHERE QuestionID = @QuestionID
END
");

            CreateStoredProcedure("dbo.InsertCarePlanDomain", p => new { CarePlanID = p.Int(), DomainID = p.Int(), Actions = p.String(storeType: "varchar(max)"), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.InsertCarePlanDomain
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
**	@CarePlanID						@@Identity
**	@DomainID
**	@Actions
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
	INSERT INTO CarePlanDomain (CarePlanID, DomainID, Actions)
	VALUES (@CarePlanID, @DomainID, @Actions)

	SELECT @@Identity
END
");

            CreateStoredProcedure("dbo.GetAction", p => new { ActionID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.GetAction
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
**	@ActionID						ActionID
**									Code
**									Name
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
	SELECT ActionID, Code, [Name]
	FROM Action
	WHERE ActionID = @ActionID
END
");

            CreateStoredProcedure("dbo.UpdateRecipientType", p => new { RecipientTypeID = p.Int(), Description = p.String(maxLength: 50), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.UpdateRecipientType
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
**	@RecipientTypeID
**	@Description
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
	UPDATE RecipientType
	SET	Description = @Description
	WHERE RecipientTypeID = @RecipientTypeID
END
");

            CreateStoredProcedure("dbo.InsertCarePlanDomainSupportService", p => new { CarePlanDomainID = p.Int(), SupportServiceTypeID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.InsertCarePlanDomainSupportService
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
**	@CarePlanDomainID						@@Identity
**	@SupportServiceTypeID
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
	INSERT INTO CarePlanDomainSupportService (CarePlanDomainID, SupportServiceTypeID)
	VALUES (@CarePlanDomainID, @SupportServiceTypeID)

	SELECT @@Identity
END
");

            CreateStoredProcedure("dbo.GetActions", @"

/******************************************************************************
**	File: 
**	Name: dbo.GetActions
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
**	@ActionID						ActionID
**									Code
**									Name
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
	SELECT ActionID, Code, [Name]
	FROM Action
END
");

            CreateStoredProcedure("dbo.UpdateService", p => new { ServiceID = p.Int(), Description = p.String(maxLength: 100), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.UpdateService
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
**	@Description
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
	UPDATE Service
	SET	Description = @Description
	WHERE ServiceID = @ServiceID
END
");

            CreateStoredProcedure("dbo.InsertChild", p => new { FirstName = p.String(maxLength: 30), LastName = p.String(maxLength: 30), Gender = p.String(maxLength: 1), DateOfBirth = p.DateTime(), DateOfBirthUnknown = p.String(maxLength: 5), WardID = p.Int(), VillageID = p.Int(), DistrictID = p.Int(), Guardian = p.String(maxLength: 50), GuardianRelationID = p.Int(), GuardianIdNo = p.String(maxLength: 15), ContactNo = p.String(maxLength: 15), Notes = p.String(maxLength: 1000), CreatedDate = p.DateTime(), CreatedUser = p.Int(), LastUpdatedDate = p.DateTime(), LastUpdatedUser = p.Int(), Locked = p.Int(), PrincipalChief = p.String(maxLength: 50), VillageChief = p.String(maxLength: 50), CommunityCouncilID = p.Int(), HIV = p.String(maxLength: 1), SchoolName = p.String(maxLength: 50), SchoolContactNo = p.String(maxLength: 20), DisabilityNotes = p.String(), PrincipalName = p.String(maxLength: 100), PrincipalContactNo = p.String(maxLength: 20), TeacherName = p.String(maxLength: 100), TeacherContactNo = p.String(maxLength: 20), OVCTypeID = p.Int(), HeadName = p.String(maxLength: 100), HeadContactNo = p.String(maxLength: 20), Address = p.String(maxLength: 200), Grade = p.String(maxLength: 2), Class = p.String(maxLength: 1), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.InsertChild
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
**	@FirstName						@@Identity
**	@LastName
**	@Gender
**	@DateOfBirth
**	@DateOfBirthUnknown
**	@WardID
**	@VillageID
**	@DistrictID
**	@Guardian
**	@GuardianRelationID
**  @GuardianIdNo
**  @ContactNo
**	@Notes
**	@CreatedDate
**	@CreatedUser
**	@LastUpdatedDate
**	@LastUpdatedUser
**	@Locked
**	@PrincipalChief
**	@VillageChief
**	@CommunityCouncilID
**	@HIV
**	@SchoolName
**	@SchoolContactNo
**	@DisabilityNotes
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
	INSERT INTO Child (FirstName, LastName, Gender, DateOfBirth, DateOfBirthUnknown, WardID, VillageID, DistrictID, Guardian, GuardianRelationID, GuardianIdNo, ContactNo, Notes, CreatedDate, CreatedUserID, LastUpdatedDate, LastUpdatedUserID, Locked, PrincipalChief, VillageChief, CommunityCouncilID, HIV, SchoolName, SchoolContactNo, DisabilityNotes, PrincipalName, PrincipalContactNo, TeacherName, TeacherContactNo, OVCTypeID, HeadName, HeadContactNo, Address, Grade, Class)
	VALUES (@FirstName, @LastName, @Gender, @DateOfBirth, @DateOfBirthUnknown, @WardID, @VillageID, @DistrictID, @Guardian, @GuardianRelationID, @GuardianIdNo, @ContactNo, @Notes, @CreatedDate, @CreatedUser, @LastUpdatedDate, @LastUpdatedUser, @Locked, @PrincipalChief, @VillageChief, @CommunityCouncilID, @HIV, @SchoolName, @SchoolContactNo, @DisabilityNotes, @PrincipalName, @PrincipalContactNo, @TeacherName, @TeacherContactNo, @OVCTypeID, @HeadName, @HeadContactNo, @Address, @Grade, @Class)

	SELECT @@Identity
END
");

            CreateStoredProcedure("dbo.GetActiveUsers", p => new { UserID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.GetActiveUsers
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
**	@UserID						UserID
**									
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
	SELECT COUNT(*) AS ActiveUsers
	FROM [User]
	WHERE LoggedOn <> 0
	AND UserID <> @UserID
END
");

            CreateStoredProcedure("dbo.UpdateServiceProvider", p => new { ServiceProviderID = p.Int(), ServiceProviderTypeID = p.Int(), Description = p.String(maxLength: 50), InformationURL = p.String(maxLength: 200), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.UpdateServiceProvider
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
**	@ServiceProviderID
**	@ServiceProviderTypeID
**	@Description
**	@InformationURL
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
	UPDATE ServiceProvider
	SET	ServiceProviderTypeID = @ServiceProviderTypeID, 
		Description = @Description, 
		InformationURL = @InformationURL
	WHERE ServiceProviderID = @ServiceProviderID
END
");

            CreateStoredProcedure("dbo.InsertChildDisability", p => new { ChildID = p.Int(), DisabilityID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.InsertChildDisability
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
**	@ChildID						@@Identity
**	@DisabilityID
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
	INSERT INTO ChildDisability (ChildID, DisabilityID)
	VALUES (@ChildID, @DisabilityID)

	SELECT @@Identity
END
");

            CreateStoredProcedure("dbo.GetAdminMode", @"

/******************************************************************************
**	File: 
**	Name: dbo.GetAdminMode
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
	SELECT AdminMode
	FROM GeneralInfo
END
");

            CreateStoredProcedure("dbo.UpdateServiceProviderType", p => new { ServiceProviderTypeID = p.Int(), Description = p.String(maxLength: 30), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.UpdateServiceProviderType
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
**	@ServiceProviderTypeID
**	@Description
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
	UPDATE ServiceProviderType
	SET	Description = @Description
	WHERE ServiceProviderTypeID = @ServiceProviderTypeID
END
");

            CreateStoredProcedure("dbo.InsertChildHousehold", p => new { ChildID = p.Int(), HouseholdID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.InsertChildHousehold
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
**	@CSIID						@@Identity
**	@DomainID
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
	INSERT INTO ChildHousehold (ChildID, HouseholdID)
	VALUES (@ChildID, @HouseholdID)

	SELECT @@Identity
END
");

            CreateStoredProcedure("dbo.GetAdult", p => new { AdultID = p.Int(), }, @"

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
	SELECT FirstName, LastName, Gender, IDNumber, Passport, MaritalStatusID, HIV, ContactNo, DateOfBirth, CreatedDate, CreatedUser, LastUpdatedDate, LastUpdatedUser
	FROM Adult
	WHERE AdultID = @AdultID
END
");

            CreateStoredProcedure("dbo.UpdateSite", p => new { SiteID = p.Int(), SiteName = p.String(maxLength: 20), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.UpdateSite
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
**	@SiteID
**	@SiteName
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
	UPDATE Site
	SET	SiteName = @SiteName
	WHERE SiteID = @SiteID
END
");

            CreateStoredProcedure("dbo.InsertChildPartner", p => new { ChildID = p.Int(), PartnerID = p.Int(), EffectiveDate = p.DateTime(), Active = p.String(maxLength: 5), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.InsertChildPartner
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
**	@ChildID						@@Identity
**	@PartnerID
**	@EffectiveDate
**	@Active
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
	INSERT INTO ChildPartner (ChildID, PartnerID, EffectiveDate, Active)
	VALUES (@ChildID, @PartnerID, @EffectiveDate, @Active)

	SELECT @@Identity
END
");

            CreateStoredProcedure("dbo.GetAdultHousehold", p => new { AdultHouseholdID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.GetAdultHousehold
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
**	@CSIDomainID						CSIDomainID
**									CSIID
**									DomainID
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
	SELECT AdultID, HouseholdID
	FROM AdultHousehold
	WHERE AdultHouseholdID = @AdultHouseholdID
END
");

            CreateStoredProcedure("dbo.UpdateSource", p => new { SourceID = p.Int(), Description = p.String(maxLength: 30), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.UpdateSource
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
**	@SourceID
**	@Description
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
	UPDATE Source
	SET	Description = @Description
	WHERE SourceID = @SourceID
END
");

            CreateStoredProcedure("dbo.InsertChildService", p => new { ChildID = p.Int(), ServiceID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.InsertChildService
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
	INSERT INTO ChildService (ChildID, ServiceID)
	VALUES (@ChildID, @ServiceID)

	SELECT @@Identity
END
");

            CreateStoredProcedure("dbo.GetAdultHouseholdByAdultIDHouseholdID", p => new { AdultID = p.Int(), HouseholdID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.GetAdultHouseholdByAdultIDHouseholdID
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
	SELECT AdultID, HouseholdID, AdultHouseholdID
	FROM AdultHousehold
	WHERE AdultID = @AdultID
	AND HouseholdID = @HouseholdID
END
");

            CreateStoredProcedure("dbo.UpdateSupportServiceType", p => new { SupportServiceTypeID = p.Int(), DomainID = p.Int(), Description = p.String(maxLength: 50), Default = p.String(maxLength: 5), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.UpdateSupportServiceType
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
**	@SupportServiceTypeID
**	@DomainID
**	@Description
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
	UPDATE SupportServiceType
	SET	DomainID = @DomainID, 
		Description = @Description,
		[Default] = @Default
	WHERE SupportServiceTypeID = @SupportServiceTypeID
END
");

            CreateStoredProcedure("dbo.InsertChildStatus", p => new { Description = p.String(maxLength: 30), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.InsertChildStatus
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
**	@Description						@@Identity
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
	INSERT INTO ChildStatus (Description)
	VALUES (@Description)

	SELECT @@Identity
END
");

            CreateStoredProcedure("dbo.GetAdults", @"

/******************************************************************************
**	File: 
**	Name: dbo.GetAdults
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
	SELECT AdultID, FirstName, LastName, Gender, IDNumber, Passport, MaritalStatusID, HIV, ContactNo, DateOfBirth, CreatedDate, CreatedUser, LastUpdatedDate, LastUpdatedUser
	FROM Adult
END
");

            CreateStoredProcedure("dbo.UpdateUser", p => new { UserID = p.Int(), Username = p.String(maxLength: 10), Password = p.Binary(), FirstName = p.String(maxLength: 20), LastName = p.String(maxLength: 30), Admin = p.Boolean(), DefSite = p.Int(), LoggedOn = p.Boolean(), PartnerID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.UpdateUser
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
**	@UserID
**	@Username
**	@Password
**	@FirstName
**	@LastName
**	@Admin
**	@DefSite
**	@LoggedOn
**	@PartnerID
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
	UPDATE [User]
	SET	Username = @Username, 
		Password = @Password, 
		FirstName = @FirstName, 
		LastName = @LastName, 
		Admin = @Admin, 
		DefSite = @DefSite, 
		LoggedOn = @LoggedOn, 
		PartnerID = @PartnerID
	WHERE UserID = @UserID
END
");

            CreateStoredProcedure("dbo.InsertChildStatusHistory", p => new { ChildID = p.Int(), StatusID = p.Int(), EffectiveDate = p.DateTime(), CreatedDate = p.DateTime(), CreatedUser = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.InsertChildStatusHistory
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
**	@ChildID						@@Identity
**	@StatusID
**	@EffectiveDate
**	@CreatedDate
**	@CreatedUser
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
	INSERT INTO ChildStatusHistory (ChildID, ChildStatusID, EffectiveDate, CreatedDate, CreatedUserID)
	VALUES (@ChildID, @StatusID, @EffectiveDate, @CreatedDate, @CreatedUser)

	SELECT @@Identity
END
");

            CreateStoredProcedure("dbo.GetAdultsByHouseholdID", p => new { HouseholdID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.GetAdultsByHouseholdID
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
	SELECT Adult.AdultID, FirstName, LastName, Gender, IDNumber, Passport, MaritalStatusID, HIV, ContactNo, DateOfBirth, CreatedDate, CreatedUser, LastUpdatedDate, LastUpdatedUser
	FROM Adult
	INNER JOIN AdultHousehold ON Adult.AdultID = AdultHousehold.AdultID
	WHERE HouseholdID = @HouseholdID
END
");

            CreateStoredProcedure("dbo.UpdateUserAction", p => new { UserActionID = p.Int(), UserID = p.Int(), SiteID = p.Int(), ActionID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.UpdateUserAction
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
**	@UserActionID
**	@UserID
**	@SiteID
**	@ActionID
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
	UPDATE UserAction
	SET	UserID = @UserID, 
		SiteID = @SiteID, 
		ActionID = @ActionID
	WHERE UserActionID = @UserActionID
END
");

            CreateStoredProcedure("dbo.InsertCommunityCouncil", p => new { Description = p.String(maxLength: 30), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.InsertCommunityCouncil
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
**	@Description						@@Identity
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
	INSERT INTO CommunityCouncil (Description)
	VALUES (@Description)

	SELECT @@Identity
END
");

            CreateStoredProcedure("dbo.GetAdultsByLetter", p => new { letter = p.String(maxLength: 10), }, @"
DECLARE @sql varchar(1000)
/******************************************************************************
**	File: 
**	Name: dbo.GetAdultsByLetter
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
	SET @sql = 'SELECT [FirstName], [LastName], [Adult].[AdultID] ' +
						 'FROM Adult ' +
						 'WHERE [LastName] LIKE ''%' + @letter + '%'' ' +
						 'ORDER BY [LastName], [FirstName] '
	EXEC (@sql)
END
");

            CreateStoredProcedure("dbo.UpdateVillage", p => new { VillageID = p.Int(), Name = p.String(maxLength: 50), DistrictID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.UpdateVillage
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
**	@VillageID
**	@Name
**	@DistrictID
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
	UPDATE Village
	SET	Name = @Name, 
		DistrictID = @DistrictID
	WHERE VillageID = @VillageID
END
");

            CreateStoredProcedure("dbo.InsertCSI", p => new { ChildID = p.Int(), IndexDate = p.DateTime(), CreatedUser = p.Int(), CreatedDate = p.DateTime(), LastUpdatedUser = p.Int(), LastUpdatedDate = p.DateTime(), StatusID = p.Int(), Photo = p.Binary(storeType: "image"), Height = p.Decimal(precision: 9, scale: 0), Weight = p.Decimal(precision: 9, scale: 0), BMI = p.Decimal(precision: 9, scale: 0), TakingMedication = p.String(maxLength: 5), MedicationDescription = p.String(storeType: "varchar(max)"), Suggestions = p.String(storeType: "varchar(max)"), DistrictID = p.Int(), Caregiver = p.String(maxLength: 50), CaregiverRelationID = p.Int(), SocialWorkerName = p.String(maxLength: 100), SocialWorkerContactNo = p.String(maxLength: 20), DoctorName = p.String(maxLength: 100), DoctorContactNo = p.String(maxLength: 20), AllergyNotes = p.String(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.InsertCSI
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
**	@ChildID						@@Identity
**	@IndexDate
**	@CreatedUser
**	@CreatedDate
**	@LastUpdatedUser
**	@LastUpdatedDate
**	@StatusID
**	@Photo
**	@Height
**	@Weight
**	@BMI
**	@TakingMedication
**	@MedicationDescription
**	@Suggestions
**	@DistrictID
**	@Caregiver
**	@CaregiverRelationID
**	@SocialWorkerName
**	@SocialWorkerContactNo
**	@DoctorName
**	@DoctorContactNo
**	@AllergyNotes
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
	INSERT INTO CSI (ChildID, IndexDate, CreatedUserID, CreatedDate, LastUpdatedUserID, LastUpdatedDate, StatusID, Photo, Height, Weight, BMI, TakingMedication, MedicationDescription, Suggestions, DistrictID, Caregiver, CaregiverRelationID, SocialWorkerName, SocialWorkerContactNo, DoctorName, DoctorContactNo, AllergyNotes)
	VALUES (@ChildID, @IndexDate, @CreatedUser, @CreatedDate, @LastUpdatedUser, @LastUpdatedDate, @StatusID, @Photo, @Height, @Weight, @BMI, @TakingMedication, @MedicationDescription, @Suggestions, @DistrictID, @Caregiver, @CaregiverRelationID, @SocialWorkerName, @SocialWorkerContactNo, @DoctorName, @DoctorContactNo, @AllergyNotes)

	SELECT @@Identity
END
");

            CreateStoredProcedure("dbo.GetAdultService", p => new { AdultServiceID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.GetAdultService
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
**	@CSIDomainID						CSIDomainID
**									CSIID
**									DomainID
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
	SELECT AdultID, ServiceID
	FROM AdultService
	WHERE AdultServiceID = @AdultServiceID
END
");

            CreateStoredProcedure("dbo.UpdateWard", p => new { WardID = p.Int(), Description = p.String(maxLength: 30), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.UpdateWard
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
**	@WardID
**	@Description
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
	UPDATE Ward
	SET	Description = @Description
	WHERE WardID = @WardID
END
");

            CreateStoredProcedure("dbo.InsertCSIContact", p => new { CSIID = p.Int(), ContactPerson = p.String(maxLength: 100), Telephone = p.String(maxLength: 30), contactType = p.String(maxLength: 50), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.InsertCSIContact
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
**	@CSIID						@@Identity
**	@ContactPerson
**	@Telephone
**	@contactType
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
	INSERT INTO CSIContact (CSIID, ContactPerson, Telephone, ContactType)
	VALUES (@CSIID, @ContactPerson, @Telephone, @ContactType)

	SELECT @@Identity
END
");

            CreateStoredProcedure("dbo.GetAdultServiceByAdultIDServiceID", p => new { AdultID = p.Int(), ServiceID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.GetAdultServiceByAdultIDServiceID
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
	SELECT AdultID, ServiceID, AdultServiceID
	FROM AdultService
	WHERE AdultID = @AdultID
	AND ServiceID = @ServiceID
END
");

            CreateStoredProcedure("dbo.UpdateWBT", p => new { WBTID = p.Int(), ChildID = p.Int(), WBTDate = p.DateTime(), CreatedUser = p.Int(), CreatedDate = p.DateTime(), LastUpdatedUser = p.Int(), LastUpdatedDate = p.DateTime(), StatusID = p.Int(), ChildIdNo = p.String(maxLength: 20), TestAdmin = p.String(maxLength: 1), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.UpdateWBT
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
**	@CSIID
**	@ChildID
**	@BWTDate
**	@CreatedUser
**	@CreatedDate
**	@LastUpdatedUser
**	@LastUpdatedDate
**	@StatusID
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
	UPDATE WBT
	SET	ChildID = @ChildID, 
		WBTDate = @WBTDate, 
		CreatedUser = @CreatedUser, 
		CreatedDate = @CreatedDate, 
		LastUpdatedUser = @LastUpdatedUser, 
		LastUpdatedDate = @LastUpdatedDate, 
		StatusID = @StatusID,
		ChildIdNo = @ChildIdNo,
		TestAdmin = @TestAdmin
	WHERE WBTID = @WBTID
END
");

            CreateStoredProcedure("dbo.InsertCSIDomain", p => new { CSIID = p.Int(), DomainID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.InsertCSIDomain
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
**	@CSIID						@@Identity
**	@DomainID
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
	INSERT INTO CSIDomain (CSIID, DomainID)
	VALUES (@CSIID, @DomainID)

	SELECT @@Identity
END
");

            CreateStoredProcedure("dbo.GetCareGiverRelation", p => new { RelationID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.GetCareGiverRelation
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
**	@RelationID						RelationID
**									Description
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
	SELECT Description
	FROM CareGiverRelation
	WHERE RelationID = @RelationID
END
");

            CreateStoredProcedure("dbo.UpdateWBTDomain", p => new { WBTDomainID = p.Int(), WBTID = p.Int(), DomainID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.UpdateWBTDomain
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
**	@WBTDomainID
**	@WBTID
**	@DomainID
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
	UPDATE WBTDomain
	SET	WBTID = @WBTID, 
		DomainID = @DomainID
	WHERE WBTDomainID = @WBTDomainID
END
");

            CreateStoredProcedure("dbo.InsertCSIDomainScore", p => new { CSIDomainID = p.Int(), QuestionID = p.Int(), Score = p.Int(), Comments = p.String(storeType: "varchar(max)"), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.InsertCSIDomainScore
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
**	@CSIDomainID						@@Identity
**	@QuestionID
**	@Score
**	@Comments
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
	INSERT INTO CSIDomainScore (CSIDomainID, QuestionID, Score, Comments)
	VALUES (@CSIDomainID, @QuestionID, @Score, @Comments)

	SELECT @@Identity
END
");

            CreateStoredProcedure("dbo.GetCareGiverRelations", @"

/******************************************************************************
**	File: 
**	Name: dbo.GetCareGiverRelations
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
	SELECT RelationID, Description
	FROM CareGiverRelation
END
");

            CreateStoredProcedure("dbo.UpdateWBTDomainScore", p => new { WBTDomainScoreID = p.Int(), WBTDomainID = p.Int(), QuestionID = p.Int(), Score = p.Int(), Comments = p.String(storeType: "varchar(max)"), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.UpdateWBTDomainScore
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
**	@WBTDomainScoreID
**	@WBTDomainID
**	@QuestionID
**	@Score
**	@Comments
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
	UPDATE WBTDomainScore
	SET	WBTDomainID = @WBTDomainID, 
		QuestionID = @QuestionID, 
		Score = @Score, 
		Comments = @Comments
	WHERE WBTDomainScoreID = @WBTDomainScoreID
END
");

            CreateStoredProcedure("dbo.InsertCSIDomainSource", p => new { CSIDomainID = p.Int(), SourceID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.InsertCSIDomainSource
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
**	@CSIDomainID						@@Identity
**	@SourceID
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
	INSERT INTO CSIDomainSource (CSIDomainID, SourceID)
	VALUES (@CSIDomainID, @SourceID)

	SELECT @@Identity
END
");

            CreateStoredProcedure("dbo.GetCarePlan", p => new { CarePlanID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.GetCarePlan
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
**	@CarePlanID						CarePlanID
**									CSIID
**									CarePlanDate
**									CreatedUser
**									CreatedDate
**									LastUpdatedUser
**									LastUpdatedDate
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
	SELECT CSIID, CarePlanDate, CreatedUserID, CreatedDate, LastUpdatedUserID, LastUpdatedDate
	FROM CarePlan
	WHERE CarePlanID = @CarePlanID
END
");

            CreateStoredProcedure("dbo.InsertCSIDomainSupportService", p => new { CSIDomainID = p.Int(), SupportServiceTypeID = p.Int(), OtherService = p.String(maxLength: 50), ServiceProviderID = p.Int(), OtherServiceProvider = p.String(maxLength: 50), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.InsertCSIDomainSupportService
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
**	@CSIDomainID						@@Identity
**	@SupportServiceTypeID
**	@OtherService
**	@ServiceProviderID
**	@OtherServiceProvider
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
	INSERT INTO CSIDomainSupportService (CSIDomainID, SupportServiceTypeID, OtherService, ServiceProviderID, OtherServiceProvider)
	VALUES (@CSIDomainID, @SupportServiceTypeID, @OtherService, @ServiceProviderID, @OtherServiceProvider)

	SELECT @@Identity
END
");

            CreateStoredProcedure("dbo.GetCarePlanDomain", p => new { CarePlanDomainID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.GetCarePlanDomain
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
**	@CarePlanDomainID						CarePlanDomainID
**									CarePlanID
**									DomainID
**									Actions
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
	SELECT CarePlanID, DomainID, Actions
	FROM CarePlanDomain
	WHERE CarePlanDomainID = @CarePlanDomainID
END
");

            CreateStoredProcedure("dbo.InsertCSIEvent", p => new { CSIID = p.Int(), EventID = p.Int(), Comments = p.String(storeType: "varchar(max)"), EffectiveDate = p.DateTime(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.InsertCSIEvent
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
**	@CSIID						@@Identity
**	@EventID
**	@Comments
**	@EffectiveDate
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
	INSERT INTO CSIEvent (CSIID, EventID, Comments, EffectiveDate)
	VALUES (@CSIID, @EventID, @Comments, @EffectiveDate)

	SELECT @@Identity
END
");

            CreateStoredProcedure("dbo.GetCarePlanDomainsForCarePlan", p => new { CarePlanID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.GetCarePlanDomainsForCarePlan
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
**	@CarePlanID						CarePlanID
**									CSIID
**									CarePlanDate
**									CreatedUser
**									CreatedDate
**									LastUpdatedUser
**									LastUpdatedDate
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
	SELECT CarePlanDomainID, CarePlanID, DomainID, Actions
	FROM CarePlanDomain
	WHERE CarePlanID = @CarePlanID
END
");

            CreateStoredProcedure("dbo.InsertDisability", p => new { Description = p.String(maxLength: 30), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.InsertDisability
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
**	@Description						@@Identity
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
	INSERT INTO Disability (Description)
	VALUES (@Description)

	SELECT @@Identity
END
");

            CreateStoredProcedure("dbo.GetCarePlanDomainSupportService", p => new { CarePlanDomainSupportServiceID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.GetCarePlanDomainSupportService
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
**	@CarePlanDomainSupportServiceID						CarePlanDomainSupportServiceID
**									CarePlanDomainID
**									SupportServiceTypeID
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
	SELECT CarePlanDomainID, SupportServiceTypeID
	FROM CarePlanDomainSupportService
	WHERE CarePlanDomainSupportServiceID = @CarePlanDomainSupportServiceID
END
");

            CreateStoredProcedure("dbo.InsertDistrict", p => new { Description = p.String(maxLength: 50), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.InsertDistrict
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
**	@Description						@@Identity
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
	INSERT INTO District (Description)
	VALUES (@Description)

	SELECT @@Identity
END
");

            CreateStoredProcedure("dbo.GetCarePlanList", p => new { searchFrom = p.DateTime(), searchTo = p.DateTime(), childID = p.Int(), }, @"
DECLARE @strWhere VARCHAR(8000)
DECLARE @strSelect VARCHAR(8000)

/******************************************************************************
**	File: 
**	Name: dbo.GetCarePlanList
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
	IF (@childID > 0) 
	BEGIN
		SET @strWhere = 'WHERE [CSI].[ChildID] = ' + CAST(@childID AS CHAR) + ' '
	END
	ELSE
	BEGIN
		SET @strWhere = 'WHERE [CarePlan].[CarePlanDate] BETWEEN ''' + CAST(@searchFrom AS VARCHAR) + ''' AND ''' + CAST(@searchTo AS VARCHAR) + ''' ' 
	END
	
	SET @strSelect = 'SELECT CarePlan.CarePlanID, Child.ChildID, Child.FirstName + '' '' + Child.LastName As FullName, CarePlanDate 
										FROM [CarePlan] INNER JOIN [CSI] ON [CarePlan].[CSIID] = [CSI].[CSIID] 
										INNER JOIN [Child] ON [CSI].[ChildID] = [Child].[ChildID] ' + @strWhere + ' ORDER BY CarePlanDate ASC'
	EXEC(@strSelect)
END
");

            CreateStoredProcedure("dbo.InsertDomain", p => new { Description = p.String(maxLength: 30), DomainCode = p.String(maxLength: 5), DomainColor = p.String(maxLength: 30), Guidelines = p.String(storeType: "varchar(max)"), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.InsertDomain
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
**	@Description						@@Identity
**	@DomainCode
**  @DomainColor
**	@Guidelines
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
	INSERT INTO Domain (Description,DomainCode,DomainColor,Guidelines)
	VALUES (@Description,@DomainCode,@DomainColor,@Guidelines)

	SELECT @@Identity
END
");

            CreateStoredProcedure("dbo.GetCarePlansForChild", p => new { SearchFrom = p.DateTime(), SearchTo = p.DateTime(), ChildID = p.Int(), }, @"
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

	SET @strSelect = 'SELECT CarePlan.CarePlanID, Child.ChildID, Child.FirstName + '' '' + Child.LastName As FullName, CarePlanDate 
										FROM [CarePlan] INNER JOIN [CSI] ON [CarePlan].[CSIID] = [CSI].[CSIID] 
										INNER JOIN [Child] ON [CSI].[ChildID] = [Child].[ChildID] ' + @strWhere + ' ORDER BY CarePlanDate ASC'
	EXEC(@strSelect)
END
");

            CreateStoredProcedure("dbo.InsertEvent", p => new { Description = p.String(maxLength: 50), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.InsertEvent
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
**	@Description						@@Identity
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
	INSERT INTO Event (Description)
	VALUES (@Description)

	SELECT @@Identity
END
");

            CreateStoredProcedure("dbo.GetChild", p => new { ChildID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.GetChild
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
**	@ChildID						ChildID
**									FirstName
**									LastName
**									Gender
**									DateOfBirth
**									DateOfBirthUnknown
**									WardID
**									VillageID
**									DistrictID
**									Guardian
**									GuardianRelationID
**									GuardianIdNo
**                  ContactNo
**									Notes
**									CreatedDate
**									CreatedUser
**									LastUpdatedDate
**									LastUpdatedUser
**									Locked
**									PrincipalChief
**									VillageChief
**									CommunityCouncilID
**									HIV
**									SchoolName
**									SchoolContactNo
**									DisabilityNotes
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
	SELECT FirstName, LastName, Gender, DateOfBirth, DateOfBirthUnknown, WardID, VillageID, DistrictID, Guardian, GuardianRelationID, GuardianIdNo, ContactNo, Notes, CreatedDate, CreatedUserID, LastUpdatedDate, LastUpdatedUserID, Locked, PrincipalChief, VillageChief, CommunityCouncilID, HIV, SchoolName, SchoolContactNo, DisabilityNotes, PrincipalName, PrincipalContactNo, TeacherName, TeacherContactNo, OVCTypeID, HeadName, HeadContactNo, Address, Grade, Class
	FROM Child
	WHERE ChildID = @ChildID
END
");

            CreateStoredProcedure("dbo.InsertFollowUp", p => new { ChildID = p.Int(), DateOfFollowUp = p.DateTime(), CreatedUser = p.Int(), CreatedDate = p.DateTime(), RecipientTypeID = p.Int(), OutcomeID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.InsertFollowUp
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
**	@ChildID						@@Identity
**	@DateOfFollowUp
**	@CreatedUser
**	@CreatedDate
**	@RecipientTypeID
**	@OutcomeID
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
	INSERT INTO FollowUp (ChildID, DateOfFollowUp, CreatedUserID, CreatedDate, RecipientTypeID, OutcomeID)
	VALUES (@ChildID, @DateOfFollowUp, @CreatedUser, @CreatedDate, @RecipientTypeID, @OutcomeID)

	SELECT @@Identity
END
");

            CreateStoredProcedure("dbo.GetChildCountForDashboard", p => new { partnerID = p.Int(), }, @"
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
																									 AND [EffectiveDate] <= getdate() 
																									 ORDER BY [EffectiveDate] DESC, [ChildStatusHistoryID]) = 1) AS Active, 
																						 (SELECT COUNT([Child].[ChildID]) 
																							FROM [Child] LEFT JOIN [ChildPartner] ON [Child].[ChildID] = [ChildPartner].[ChildID] 
																							WHERE [ChildPartner].[PartnerID] = [Partner].[PartnerID] 
																							AND (SELECT TOP 1 [ChildStatusHistory].[ChildStatusID] 
																									 FROM [ChildStatusHistory] WHERE ChildID = Child.ChildID  AND [EffectiveDate] <= getdate() 
																									 ORDER BY [EffectiveDate] DESC, [ChildStatusHistoryID]) <> 1) AS InActive 
										FROM [Partner] ' + @strWhere + ' ORDER BY [Partner].[Name] ASC'
	EXEC(@strSelect)
	
END
");

            CreateStoredProcedure("dbo.InsertGuardianRelation", p => new { Description = p.String(maxLength: 30), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.InsertGuardianRelation
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
**	@Description						@@Identity
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
	INSERT INTO GuardianRelation (Description)
	VALUES (@Description)

	SELECT @@Identity
END
");

            CreateStoredProcedure("dbo.GetChildDisability", p => new { ChildDisabilityID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.GetChildDisability
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
**	@ChildDisabilityID						ChildDisabilityID
**									ChildID
**									DisabilityID
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
	SELECT ChildID, DisabilityID
	FROM ChildDisability
	WHERE ChildDisabilityID = @ChildDisabilityID
END
");

            CreateStoredProcedure("dbo.InsertGuideline", p => new { DescriptionEnglish = p.String(storeType: "varchar(max)"), DescriptionSesotho = p.String(storeType: "varchar(max)"), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.InsertGuideline
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
**	@DescriptionEnglish						@@Identity
**	@DescriptionSesotho
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
	INSERT INTO Guideline (DescriptionEnglish, DescriptionSesotho)
	VALUES (@DescriptionEnglish, @DescriptionSesotho)

	SELECT @@Identity
END
");

            CreateStoredProcedure("dbo.GetChildDisabilityIDByDescription", p => new { Description = p.String(maxLength: 50), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.GetChildDisabilityIDByDescription
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
**	@ChildDisabilityID						ChildDisabilityID
**									ChildID
**									DisabilityID
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
	SELECT ChildDisabilityID
	FROM ChildDisability
	INNER JOIN Disability ON ChildDisability.DisabilityID = ChildDisability.DisabilityID
	WHERE disability.Description = @Description
END
");

            CreateStoredProcedure("dbo.InsertHomeEnvironment", p => new { HouseholdID = p.Int(), HouseMembersCount = p.Int(), HouseOVCsMale = p.Int(), HouseOVCsFemale = p.Int(), HouseMinorsMale = p.Int(), HouseMinorsFemale = p.Int(), HouseAdequate = p.String(maxLength: 1), HouseAdequateSpecify = p.String(maxLength: 2000), FoodSourceFarming = p.Boolean(), FoodSourceRelative = p.Boolean(), FoodSourceDonations = p.Boolean(), FoodSourceNoReliable = p.Boolean(), AllHHMembersWell = p.String(maxLength: 1), HHMembersOccassionallyIll = p.String(maxLength: 1), HHMembersFrequentlySick = p.String(maxLength: 1), OneHHMemberIll = p.String(maxLength: 1), GoToHealthCentre = p.Boolean(), CommunityHealthWorker = p.Boolean(), TraditionalHealer = p.Boolean(), NoHealthCare = p.Boolean(), LatrineInGoodRepair = p.String(maxLength: 1), LatrineInBadRepair = p.String(maxLength: 1), SharingLatrine = p.String(maxLength: 1), NoLatrine = p.String(maxLength: 1), WarmClothing = p.Boolean(), Blanket = p.Boolean(), Shoes = p.Boolean(), SchoolUniform = p.Boolean(), ClothingSpecify = p.String(maxLength: 2000), NeedsFood = p.Boolean(), NeedsSchool = p.Boolean(), NeedsPsychosocial = p.Boolean(), NeedsGuardianship = p.Boolean(), NeedsToiletries = p.Boolean(), NeedsSchoolSupplies = p.Boolean(), NeedsShelter = p.Boolean(), NeedsHealth = p.Boolean(), NoSpecialNeed = p.Boolean(), CaregiverWithDisabilities = p.Boolean(), SpecialNeedsSpecify = p.String(maxLength: 2000), SizeOfUniform = p.Int(), SizeOfShoes = p.Int(), Remarks = p.String(maxLength: 2000), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.InsertHomeEnvironment
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
	INSERT INTO HomeEnvironment (HouseholdID, HouseMembersCount, HouseOVCsMale, HouseOVCsFemale, HouseMinorsMale, HouseMinorsFemale, HouseAdequate, HouseAdequateSpecify, FoodSourceFarming, FoodSourceRelative, FoodSourceDonations, FoodSourceNoReliable, AllHHMembersWell, HHMembersOccassionallyIll, HHMembersFrequentlySick, OneHHMemberIll, GoToHealthCentre, CommunityHealthWorker, TraditionalHealer, NoHealthCare, LatrineInGoodRepair, LatrineInBadRepair, SharingLatrine, NoLatrine, WarmClothing, Blanket, Shoes, SchoolUniform, ClothingSpecify, NeedsFood, NeedsSchool, NeedsPsychosocial, NeedsGuardianship, NeedsToiletries, NeedsSchoolSupplies, NeedsShelter, NeedsHealth, NoSpecialNeed, CaregiverWithDisabilities, SpecialNeedsSpecify, SizeOfUniform, SizeOfShoes, Remarks)
	VALUES (@HouseholdID, @HouseMembersCount, @HouseOVCsMale, @HouseOVCsFemale, @HouseMinorsMale, @HouseMinorsFemale, @HouseAdequate, @HouseAdequateSpecify, @FoodSourceFarming, @FoodSourceRelative, @FoodSourceDonations, @FoodSourceNoReliable, @AllHHMembersWell, @HHMembersOccassionallyIll, @HHMembersFrequentlySick, @OneHHMemberIll, @GoToHealthCentre, @CommunityHealthWorker, @TraditionalHealer, @NoHealthCare, @LatrineInGoodRepair, @LatrineInBadRepair, @SharingLatrine, @NoLatrine, @WarmClothing, @Blanket, @Shoes, @SchoolUniform, @ClothingSpecify, @NeedsFood, @NeedsSchool, @NeedsPsychosocial, @NeedsGuardianship, @NeedsToiletries, @NeedsSchoolSupplies, @NeedsShelter, @NeedsHealth, @NoSpecialNeed, @CaregiverWithDisabilities, @SpecialNeedsSpecify, @SizeOfUniform, @SizeOfShoes, @Remarks)

	SELECT @@Identity
END
");

            CreateStoredProcedure("dbo.GetChildDomainReport", p => new { searchFrom = p.DateTime(), searchTo = p.DateTime(), activeOption = p.Int(), byGender = p.Boolean(), byAgeGroup = p.Boolean(), domainID = p.Int(), }, @"
DECLARE @strWhere VARCHAR(1000)
DECLARE @strSelect VARCHAR(8000)

/******************************************************************************
**	File: 
**	Name: dbo.GetChildDomainReport
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
	
	SET @strSelect = 'SELECT P.[Name], [DomainID], [QuestionID], [Score] '
	If @byGender = 1 
	BEGIN
		SET @strSelect = @strSelect + ', [Gender] '
	END
	If @byAgeGroup = 1
	BEGIN
		SET @strSelect = @strSelect + ', CASE WHEN (DATEDIFF(dd, [DateOfBirth], getdate())) < 1825 THEN ''<5'' WHEN (DATEDIFF(dd, [DateOfBirth], getdate())) BETWEEN 1825 AND 4379 THEN ''5-13'' WHEN (DATEDIFF(dd, [DateOfBirth], getdate())) > 4380 THEN ''>13'' END AS AgeGroup'
	End 
	SET @strSelect = @strSelect + ', COUNT(*) AS NrChild '
  
	SET @strSelect = @strSelect + 'FROM [CSIDomainScore] S INNER JOIN [CSIDomain] CD ON S.[CSIDomainID] = CD.[CSIDomainID] INNER JOIN [CSI] C ON CD.[CSIID] = C.[CSIID] INNER JOIN [Child] CH ON C.[ChildID] = CH.[ChildID] LEFT JOIN [ChildPartner] CP ON CH.[ChildID] = CP.[ChildID] LEFT JOIN [Partner] P ON CP.[PartnerID] = P.[PartnerID] '
	SET @strSelect = @strSelect + 'WHERE [Score] > 0 AND [DomainID] = ' + CAST(@domainID AS CHAR) + ' AND C.[CSIID] = (SELECT MAX(CSIID) FROM [CSI] WHERE [ChildID] = C.[ChildID] AND [IndexDate] BETWEEN ''' + CAST(@searchFrom AS CHAR) + ''' AND ''' + CAST(@searchTo AS CHAR) + ''') '
  
	If @activeOption = 1  -- Active only
	BEGIN
		SET @strSelect = @strSelect + 'AND (SELECT TOP 1 StatusID FROM [ChildStatusHistory] WHERE ChildID = C.[ChildID] AND [EffectiveDate] <= getdate() ORDER BY [EffectiveDate] DESC, [ChildStatusHistoryID]) = 1 '
	END
	ELSE If @activeOption = 2 -- Inactive
	BEGIN
		SET @strSelect = @strSelect + 'AND (SELECT TOP 1 StatusID FROM [ChildStatusHistory] WHERE ChildID = C.[ChildID] AND [EffectiveDate] <= getdate() ORDER BY [EffectiveDate] DESC, [ChildStatusHistoryID]) > 1 '
	END
  
	SET @strSelect = @strSelect + 'GROUP BY P.[Name], [DomainID], [QuestionID], [Score] '
	If @byGender = 1 
	BEGIN
		SET @strSelect = @strSelect + ', [Gender] '
	End 
	If @byAgeGroup = 1
	BEGIN
		SET @strSelect = @strSelect + ', CASE WHEN (DATEDIFF(dd, [DateOfBirth], getdate())) < 1825 THEN ''<5'' WHEN (DATEDIFF(dd, [DateOfBirth], getdate())) BETWEEN 1825 AND 4379 THEN ''5-13'' WHEN (DATEDIFF(dd, [DateOfBirth], getdate())) > 4380 THEN ''>13'' END '
	End 
  
	SET @strSelect = @strSelect + ' ORDER BY P.[Name], [Score], [DomainID], [QuestionID] '
	EXEC(@strSelect)
END
");

            CreateStoredProcedure("dbo.InsertHousehold", p => new { Address = p.String(maxLength: 200), VillageID = p.Int(), DistrictID = p.Int(), Country = p.String(maxLength: 50), ParticipantRegistrationID = p.Int(), CreatedDate = p.DateTime(), CreatedUser = p.Int(), LastUpdatedDate = p.DateTime(), LastUpdatedUser = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.InsertHousehold
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
	INSERT INTO Household (Address, VillageID, DistrictID, Country, ParticipantRegistrationID, CreatedDate, CreatedUser, LastUpdatedDate, LastUpdatedUser)
	VALUES (@Address, @VillageID, @DistrictID, @Country, @ParticipantRegistrationID, @CreatedDate, @CreatedUser, @LastUpdatedDate, @LastUpdatedUser)

	SELECT @@Identity
END
");

            CreateStoredProcedure("dbo.GetChildHousehold", p => new { ChildHouseholdID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.GetChildHousehold
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
**	@CSIDomainID						CSIDomainID
**									CSIID
**									DomainID
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
	SELECT ChildID, HouseholdID
	FROM ChildHousehold
	WHERE ChildHouseholdID = @ChildHouseholdID
END
");

            CreateStoredProcedure("dbo.InsertHouseholdProject", p => new { HouseholdID = p.Int(), ParticipantRegistrationID = p.Int(), FieldAgentMonitoringID = p.Int(), TrainingRegistryID = p.Int(), CreatedDate = p.DateTime(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.InsertHouseholdProject
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
**	@UserID						@@Identity
**	@SiteID
**	@ActionID
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
	INSERT INTO HouseholdProject (HouseholdID, ParticipantRegistrationID, FieldAgentMonitoringID, TrainingRegistryID, CreatedDate)
	VALUES (@HouseholdID, @ParticipantRegistrationID, @FieldAgentMonitoringID, @TrainingRegistryID, @CreatedDate)

	SELECT @@Identity
END
");

            CreateStoredProcedure("dbo.GetChildHouseholdByChildIDHouseholdID", p => new { ChildID = p.Int(), HouseholdID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.GetChildHouseholdByChildIDHouseholdID
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
	SELECT ChildID, HouseholdID, ChildHouseholdID
	FROM ChildHousehold
	WHERE ChildID = @ChildID
	AND HouseholdID = @HouseholdID
END
");

            CreateStoredProcedure("dbo.InsertOutcome", p => new { Code = p.String(maxLength: 10), Description = p.String(maxLength: 50), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.InsertOutcome
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
**	@Code						@@Identity
**	@Name
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
	INSERT INTO Outcome (Code, Description)
	VALUES (@Code, @Description)

	SELECT @@Identity
END
");

            CreateStoredProcedure("dbo.GetChildListForDashboard", p => new { partnerID = p.Int(), }, @"
DECLARE @strWhere VARCHAR(100)
DECLARE @strSelect VARCHAR(8000)

/******************************************************************************
**	File: 
**	Name: dbo.GetChildListForDashboard
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
		SET @strWhere = ' AND [ChildPartner].[PartnerID] = ' + CAST(@partnerID AS CHAR)
	END
	ELSE
	BEGIN
		SET @strWhere = ''
	END

	SET @strSelect = 'SELECT CSI.CSIID AS CSIID, Child.ChildID AS ChildID, QuestionID, Score, LTRIM(RTRIM(Child.FirstName)) + '' '' + LTRIM(RTRIM(Child.LastName)) AS FullName, IndexDate, Gender, DateOfBirth, 
										FLOOR(DATEDIFF(dd, DateOfBirth, getdate()) / 365.25) AS Age, [Partner].Name AS [Partner] 
										FROM [CSIDomainScore] INNER JOIN [CSIDomain] ON [CSIDomainScore].[CSIDomainID] = [CSIDomain].[CSIDomainID] 
										INNER JOIN [CSI] ON [CSIDomain].[CSIID] = [CSI].[CSIID] 
										INNER JOIN [Child] ON [CSI].[ChildID] = [Child].[ChildID] 
										LEFT JOIN [ChildPartner] ON [Child].[ChildID] = [ChildPartner].[ChildID] 
										LEFT JOIN [Partner] ON [ChildPartner].[PartnerID] = [Partner].[PartnerID] 
										WHERE IndexDate = (SELECT MAX(IndexDate) FROM CSI WHERE ChildID = Child.ChildID) ' + @strWhere + ' ORDER BY Child.LastName ASC, IndexDate ASC'
	EXEC(@strSelect)
END
");

            CreateStoredProcedure("dbo.InsertOVCType", p => new { Description = p.String(maxLength: 100), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.InsertOVCType
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
**	@Description						@@Identity
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
	INSERT INTO OVCType (Description)
	VALUES (@Description)

	SELECT @@Identity
END
");

            CreateStoredProcedure("dbo.GetChildPartner", p => new { ChildPartnerID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.GetChildPartner
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
**	@ChildPartnerID						ChildPartnerID
**									ChildID
**									PartnerID
**									EffectiveDate
**									Active
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
	SELECT ChildID, PartnerID, EffectiveDate, Active
	FROM ChildPartner
	WHERE ChildPartnerID = @ChildPartnerID
END
");

            CreateStoredProcedure("dbo.InsertParticipantRegistration", p => new { Enumerator = p.String(maxLength: 100), DateOfReg = p.DateTime(), ProjectAreaID = p.Int(), ActivePersonID = p.Int(), AnyPregnant = p.String(maxLength: 1), CropLand = p.String(maxLength: 1), DistrictID = p.Int(), VillageID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.InsertParticipantRegistration
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
	INSERT INTO ParticipantRegistration (Enumerator, DateOfReg, ProjectAreaID, ActivePersonID, AnyPregnant, CropLand, DistrictID, VillageID)
	VALUES (@Enumerator, @DateOfReg, @ProjectAreaID, @ActivePersonID, @AnyPregnant, @CropLand, @DistrictID, @VillageID)

	SELECT @@Identity
END
");

            CreateStoredProcedure("dbo.GetChildren", @"

/******************************************************************************
**	File: 
**	Name: dbo.GetChildren
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
**									ChildID
**									FirstName
**									LastName
**									Gender
**									DateOfBirth
**									DateOfBirthUnknown
**									WardID
**									VillageID
**									DistrictID
**									Guardian
**									GuardianRelationID
**									GuardianIdNo
**									ContactNo
**									Notes
**									CreatedDate
**									CreatedUserID
**									LastUpdatedDate
**									LastUpdatedUserID
**									Locked
**									PrincipalChief
**									VillageChief
**									CommunityCouncilID
**									HIV
**									SchoolName
**									SchoolContactNo
**									DisabilityNotes
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
	SELECT ChildID, FirstName, LastName, Gender, DateOfBirth, DateOfBirthUnknown, WardID, VillageID, DistrictID, Guardian, GuardianRelationID, GuardianIdNo, ContactNo, Notes, CreatedDate, CreatedUserID, LastUpdatedDate, LastUpdatedUserID, Locked, PrincipalChief, VillageChief, CommunityCouncilID, HIV, SchoolName, SchoolContactNo, DisabilityNotes, PrincipalName, PrincipalContactNo, TeacherName, TeacherContactNo, OVCTypeID, HeadName, HeadContactNo, Address, Grade, Class
	FROM Child
END
");

            CreateStoredProcedure("dbo.InsertPartner", p => new { Name = p.String(maxLength: 50), Address = p.String(storeType: "varchar(max)"), ContactNo = p.String(maxLength: 30), FaxNo = p.String(maxLength: 30), ContactName = p.String(maxLength: 60), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.InsertPartner
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
**	@Name						@@Identity
**	@Address
**	@ContactNo
**	@FaxNo
**	@ContactName
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
	INSERT INTO Partner (Name, Address, ContactNo, FaxNo, ContactName)
	VALUES (@Name, @Address, @ContactNo, @FaxNo, @ContactName)

	SELECT @@Identity
END
");

            CreateStoredProcedure("dbo.GetChildrenByHouseholdID", p => new { HouseholdID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.GetChildrenByHouseholdID
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
	SELECT Child.ChildID, FirstName, LastName, Gender, DateOfBirth, DateOfBirthUnknown, WardID, VillageID, DistrictID, Guardian, GuardianRelationID, GuardianIdNo, ContactNo, Notes, CreatedDate, CreatedUser, LastUpdatedDate, LastUpdatedUser, Locked, PrincipalChief, VillageChief, CommunityCouncilID, HIV, SchoolName, SchoolContactNo, DisabilityNotes, PrincipalName, PrincipalContactNo, TeacherName, TeacherContactNo, OVCTypeID, HeadName, HeadContactNo, Address, Grade, Class
	FROM Child
	INNER JOIN ChildHousehold ON Child.ChildID = ChildHousehold.ChildID
	WHERE HouseholdID = @HouseholdID
END
");

            CreateStoredProcedure("dbo.InsertQuestion", p => new { DomainID = p.Int(), Description = p.String(maxLength: 20), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.InsertQuestion
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
**	@DomainID						@@Identity
**	@Description
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
	INSERT INTO Question (DomainID, Description)
	VALUES (@DomainID, @Description)

	SELECT @@Identity
END
");

            CreateStoredProcedure("dbo.GetChildrenByLetter", p => new { letter = p.String(maxLength: 10), }, @"
DECLARE @sql varchar(1000)
/******************************************************************************
**	File: 
**	Name: dbo.GetChildrenByLetter
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
	SET @sql = 'SELECT [FirstName], [LastName], [Child].[ChildID] ' +
						 'FROM Child ' +
						 'WHERE [LastName] LIKE ''%' + @letter + '%'' ' +
						 'ORDER BY [LastName], [FirstName] '
	EXEC (@sql)
END
");

            CreateStoredProcedure("dbo.InsertRecipientType", p => new { Description = p.String(maxLength: 50), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.InsertRecipientType
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
**	@Description						@@Identity
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
	INSERT INTO RecipientType (Description)
	VALUES (@Description)

	SELECT @@Identity
END
");

            CreateStoredProcedure("dbo.GetChildService", p => new { ChildServiceID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.GetChildService
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
	SELECT ChildID, ServiceID
	FROM ChildService
	WHERE ChildServiceID = @ChildServiceID
END
");

            CreateStoredProcedure("dbo.InsertService", p => new { Description = p.String(maxLength: 100), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.InsertService
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
**	@Description						@@Identity
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
	INSERT INTO Service (Description)
	VALUES (@Description)

	SELECT @@Identity
END
");

            CreateStoredProcedure("dbo.GetChildServiceByChildIDServiceID", p => new { ChildID = p.Int(), ServiceID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.GetChildServiceByChildIDServiceID
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
	SELECT ChildID, ServiceID, ChildServiceID
	FROM ChildService
	WHERE ChildID = @ChildID
	AND ServiceID = @ServiceID
END
");

            CreateStoredProcedure("dbo.InsertServiceProvider", p => new { ServiceProviderTypeID = p.Int(), Description = p.String(maxLength: 50), InformationURL = p.String(maxLength: 200), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.InsertServiceProvider
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
**	@ServiceProviderTypeID						@@Identity
**	@Description
**	@InformationURL
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
	INSERT INTO ServiceProvider (ServiceProviderTypeID, Description, InformationURL)
	VALUES (@ServiceProviderTypeID, @Description, @InformationURL)

	SELECT @@Identity
END
");

            CreateStoredProcedure("dbo.GetChildStatus", p => new { StatusID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.GetChildStatus
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
**	@StatusID						StatusID
**									Description
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
	SELECT Description
	FROM ChildStatus
	WHERE StatusID = @StatusID
END
");

            CreateStoredProcedure("dbo.InsertServiceProviderType", p => new { Description = p.String(maxLength: 30), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.InsertServiceProviderType
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
**	@Description						@@Identity
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
	INSERT INTO ServiceProviderType (Description)
	VALUES (@Description)

	SELECT @@Identity
END
");

            CreateStoredProcedure("dbo.GetChildStatusHistory", p => new { ChildStatusHistoryID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.GetChildStatusHistory
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
**	@ChildStatusHistoryID						ChildStatusHistoryID
**									ChildID
**									StatusID
**									EffectiveDate
**									CreatedDate
**									CreatedUserID
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
	SELECT ChildID, ChildStatusID, EffectiveDate, CreatedDate, CreatedUserID
	FROM ChildStatusHistory
	WHERE ChildStatusHistoryID = @ChildStatusHistoryID
END
");

            CreateStoredProcedure("dbo.InsertSite", p => new { SiteName = p.String(maxLength: 20), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.InsertSite
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
**	@SiteName						@@Identity
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
	INSERT INTO Site (SiteName)
	VALUES (@SiteName)

	SELECT @@Identity
END
");

            CreateStoredProcedure("dbo.GetChildStatuss", @"

/******************************************************************************
**	File: 
**	Name: dbo.GetChildStatuss
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
	SELECT StatusID, Description
	FROM ChildStatus
END
");

            CreateStoredProcedure("dbo.InsertSource", p => new { Description = p.String(maxLength: 30), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.InsertSource
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
**	@Description						@@Identity
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
	INSERT INTO Source (Description)
	VALUES (@Description)

	SELECT @@Identity
END
");

            CreateStoredProcedure("dbo.GetCommunityCouncil", p => new { CommunityCouncilID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.GetCommunityCouncil
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
**	@CommunityCouncilID						CommunityCouncilID
**									Description
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
	SELECT Description
	FROM CommunityCouncil
	WHERE CommunityCouncilID = @CommunityCouncilID
END
");

            CreateStoredProcedure("dbo.InsertSupportServiceType", p => new { DomainID = p.Int(), Description = p.String(maxLength: 50), Default = p.String(maxLength: 5), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.InsertSupportServiceType
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
**	@DomainID						@@Identity
**	@Description
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
	INSERT INTO SupportServiceType (DomainID, Description, [Default])
	VALUES (@DomainID, @Description, @Default)

	SELECT @@Identity
END
");

            CreateStoredProcedure("dbo.GetCommunityCouncils", @"

/******************************************************************************
**	File: 
**	Name: dbo.GetCommunityCouncils
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
	SELECT CommunityCouncilID, Description
	FROM CommunityCouncil
END
");

            CreateStoredProcedure("dbo.InsertUser", p => new { Username = p.String(maxLength: 10), Password = p.Binary(), FirstName = p.String(maxLength: 20), LastName = p.String(maxLength: 30), Admin = p.String(maxLength: 1), DefSite = p.Int(), LoggedOn = p.String(maxLength: 1), PartnerID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.InsertUser
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
**	@Username						@@Identity
**	@Password
**	@FirstName
**	@LastName
**	@Admin
**	@DefSite
**	@LoggedOn
**	@PartnerID
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
	INSERT INTO [User] (Username, Password, FirstName, LastName, Admin, DefSite, LoggedOn, PartnerID)
	VALUES (@Username, @Password, @FirstName, @LastName, @Admin, @DefSite, @LoggedOn, @PartnerID)

	SELECT @@Identity
END
");

            CreateStoredProcedure("dbo.GetContactsForCSI", p => new { CSIID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.GetContactsForCSI
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
**	@CSIID					CSIContactID
**									ContactPerson
**									Telephone
**									ContactType
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
	SELECT ContactPerson, Telephone, ContactType, CSIContactID
	FROM CSIContact
	WHERE CSIID = @CSIID
END
");

            CreateStoredProcedure("dbo.InsertUserAction", p => new { UserID = p.Int(), SiteID = p.Int(), ActionID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.InsertUserAction
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
**	@UserID						@@Identity
**	@SiteID
**	@ActionID
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
	INSERT INTO UserAction (UserID, SiteID, ActionID)
	VALUES (@UserID, @SiteID, @ActionID)

	SELECT @@Identity
END
");

            CreateStoredProcedure("dbo.GetCSI", p => new { CSIID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.GetCSI
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
**	@CSIID						CSIID
**									ChildID
**									IndexDate
**									CreatedUserID
**									CreatedDate
**									LastUpdatedUserID
**									LastUpdatedDate
**									StatusID
**									Photo
**									Height
**									Weight
**									BMI
**									TakingMedication
**									MedicationDescription
**									Suggestions
**									DistrictID
**									Caregiver
**									CaregiverRelationID
**									SocialWorkername
**									SocialWorkerContactNo
**									DoctorName
**									DoctorContactNo
**									AllergyNotes
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
	SELECT ChildID, IndexDate, CreatedUserID, CreatedDate, LastUpdatedUserID, LastUpdatedDate, StatusID, Photo, Height, Weight, BMI, TakingMedication, MedicationDescription, Suggestions, DistrictID, Caregiver, CaregiverRelationID, SocialWorkerName, socialWorkerContactNo, DoctorName, DoctorContactNo, AllergyNotes
	FROM CSI
	WHERE CSIID = @CSIID
END
");

            CreateStoredProcedure("dbo.InsertVillage", p => new { Name = p.String(maxLength: 50), DistrictID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.InsertVillage
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
**	@Name						@@Identity
**	@DistrictID
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
	INSERT INTO Village ([Name], DistrictID)
	VALUES (@Name, @DistrictID)

	SELECT @@Identity
END
");

            CreateStoredProcedure("dbo.GetCSIContact", p => new { CSIContactID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.GetCSIContact
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
**	@CSIEventID						CSIContactID
**									CSIID
**									ContactPerson
**									Telephone
**									ContactType
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
	SELECT CSIContactID, CSIID, Contactperson, Telephone, ContactType
	FROM CSIContact
	WHERE CSIContactID = @CSIContactID
END
");

            CreateStoredProcedure("dbo.InsertWard", p => new { Description = p.String(maxLength: 30), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.InsertWard
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
**	@Description						@@Identity
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
	INSERT INTO Ward (Description)
	VALUES (@Description)

	SELECT @@Identity
END
");

            CreateStoredProcedure("dbo.GetCSIContacts", @"

/******************************************************************************
**	File: 
**	Name: dbo.GetCSIContacts
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
**									CSIContactID
**									CSIID
**									ContactPerson
**									Telephone
**									ContactType
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
	SELECT CSIContactID, CSIID, ContactPerson, Telephone, ContactType
	FROM CSIContact
END
");

            CreateStoredProcedure("dbo.InsertWBT", p => new { ChildID = p.Int(), WBTDate = p.DateTime(), CreatedUser = p.Int(), CreatedDate = p.DateTime(), LastUpdatedUser = p.Int(), LastUpdatedDate = p.DateTime(), StatusID = p.Int(), ChildIdNo = p.String(maxLength: 20), TestAdmin = p.String(maxLength: 1), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.InsertWBT
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
**	@ChildID						@@Identity
**	@WBTDate
**	@CreatedUser
**	@CreatedDate
**	@LastUpdatedUser
**	@LastUpdatedDate
**	@StatusID
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
	INSERT INTO WBT (ChildID, WBTDate, CreatedUser, CreatedDate, LastUpdatedUser, LastUpdatedDate, StatusID, ChildIdNo, TestAdmin)
	VALUES (@ChildID, @WBTDate, @CreatedUser, @CreatedDate, @LastUpdatedUser, @LastUpdatedDate, @StatusID, @ChildIdNo, @TestAdmin)

	SELECT @@Identity
END
");

            CreateStoredProcedure("dbo.GetCSIDomain", p => new { CSIDomainID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.GetCSIDomain
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
**	@CSIDomainID						CSIDomainID
**									CSIID
**									DomainID
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
	SELECT CSIID, DomainID
	FROM CSIDomain
	WHERE CSIDomainID = @CSIDomainID
END
");

            CreateStoredProcedure("dbo.InsertWBTDomain", p => new { WBTID = p.Int(), DomainID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.InsertWBTDomain
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
**	@WBTID						@@Identity
**	@DomainID
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
	INSERT INTO WBTDomain (WBTID, DomainID)
	VALUES (@WBTID, @DomainID)

	SELECT @@Identity
END
");

            CreateStoredProcedure("dbo.GetCSIDomainScore", p => new { CSIDomainScoreID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.GetCSIDomainScore
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
**	@CSIDomainScoreID						CSIDomainScoreID
**									CSIDomainID
**									QuestionID
**									Score
**									Comments
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
	SELECT CSIDomainID, QuestionID, Score, Comments
	FROM CSIDomainScore
	WHERE CSIDomainScoreID = @CSIDomainScoreID
END
");

            CreateStoredProcedure("dbo.InsertWBTDomainScore", p => new { WBTDomainID = p.Int(), QuestionID = p.Int(), Score = p.Int(), Comments = p.String(storeType: "varchar(max)"), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.InsertWBTDomainScore
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
**	@WBTDomainID						@@Identity
**	@QuestionID
**	@Score
**	@Comments
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
	INSERT INTO WBTDomainScore (WBTDomainID, QuestionID, Score, Comments)
	VALUES (@WBTDomainID, @QuestionID, @Score, @Comments)

	SELECT @@Identity
END
");

            CreateStoredProcedure("dbo.GetCSIDomainSource", p => new { CSIDomainSourceID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.GetCSIDomainSource
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
**	@CSIDomainSourceID						CSIDomainSourceID
**									CSIDomainID
**									SourceID
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
	SELECT CSIDomainID, SourceID
	FROM CSIDomainSource
	WHERE CSIDomainSourceID = @CSIDomainSourceID
END
");

            CreateStoredProcedure("dbo.IsAdultUnique", p => new { AdultID = p.Int(), FirstName = p.String(maxLength: 30), LastName = p.String(maxLength: 30), }, @"
DECLARE @sql as varchar(1000)
/******************************************************************************
**	File: 
**	Name: dbo.IsAdultUnique
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
**	@ChildID						ChildID
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
	SET @sql = '
	SELECT COUNT(*) AS CountAdult
	FROM Adult
	WHERE AdultID <> ' + CAST(@AdultID AS CHAR) + '
	AND FirstName = ''' + REPLACE(RTRIM(LTRIM(@FirstName)), '''', '''''') + ''' 
	AND LastName = ''' + REPLACE(RTRIM(LTRIM(@LastName)), '''', '''''') + ''''
	EXEC(@sql)
END
");

            CreateStoredProcedure("dbo.GetCSIDomainSupportService", p => new { CSIDomainSupportServiceID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.GetCSIDomainSupportService
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
**	@CSIDomainSupportServiceID						CSIDomainSupportServiceID
**									CSIDomainID
**									SupportServiceTypeID
**									OtherService
**									ServiceProviderID
**									OtherServiceProvider
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
	SELECT CSIDomainID, SupportServiceTypeID, OtherService, ServiceProviderID, OtherServiceProvider
	FROM CSIDomainSupportService
	WHERE CSIDomainSupportServiceID = @CSIDomainSupportServiceID
END
");

            CreateStoredProcedure("dbo.IsCareGiverRelationInUse", p => new { CareGiverRelationID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.IsCareGiverRelationInUse
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
	SELECT COUNT(*) AS CountCareGiverRelation
	FROM CSI
	WHERE CareGiverRelationID = @CareGiverRelationID
END
");

            CreateStoredProcedure("dbo.GetCSIEvent", p => new { CSIEventID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.GetCSIEvent
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
**	@CSIEventID						CSIEventID
**									CSIID
**									EventID
**									Comments
**									EffectiveDate
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
	SELECT CSIID, EventID, Comments, EffectiveDate
	FROM CSIEvent
	WHERE CSIEventID = @CSIEventID
END
");

            CreateStoredProcedure("dbo.IsCareGiverRelationUnique", p => new { CareGiverRelationID = p.Int(), Description = p.String(maxLength: 50), }, @"
DECLARE @sql AS VARCHAR(1000)
/******************************************************************************
**	File: 
**	Name: dbo.IsCareGiverRelationUnique
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
	SET @sql = '
	SELECT COUNT(*) AS CountCareGiverRelation
	FROM CareGiverRelation
	WHERE RelationID <> ' + CAST(@CareGiverRelationID AS CHAR) + '
	AND Description = ''' + REPLACE(RTRIM(LTRIM(@Description)), '''', '''''') + '''' 
	EXEC(@sql)
END
");

            CreateStoredProcedure("dbo.GetCSIList", p => new { searchFrom = p.DateTime(), searchTo = p.DateTime(), childID = p.Int(), }, @"
DECLARE @strWhere VARCHAR(8000)
DECLARE @strSelect VARCHAR(8000)

/******************************************************************************
**	File: 
**	Name: dbo.GetCSIList
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
	IF (@childID > 0) 
	BEGIN
		SET @strWhere = 'WHERE [CSI].[ChildID] = ' + CAST(@childID AS CHAR) + ' '
	END
	ELSE
	BEGIN
		SET @strWhere = 'WHERE [CSI].[IndexDate] BETWEEN ''' + CAST(@searchFrom AS VARCHAR) + ''' AND ''' + CAST(@searchTo AS VARCHAR) + ''' '
	END

	SET @strSelect = 'SELECT CSI.CSIID, QuestionID, Score, Child.FirstName + '' '' + Child.LastName AS FullName, IndexDate  
										FROM [CSIDomainScore] INNER JOIN [CSIDomain] ON [CSIDomainScore].[CSIDomainID] = [CSIDomain].[CSIDomainID] 
										INNER JOIN [CSI] ON [CSIDomain].[CSIID] = [CSI].[CSIID] 
										INNER JOIN [Child] ON [CSI].[ChildID] = [Child].[ChildID] ' + @strWhere + ' ORDER BY Child.LastName ASC, IndexDate ASC '
	EXEC(@strSelect)
END
");

            CreateStoredProcedure("dbo.IsChildLocked", p => new { ChildID = p.Int(), }, @"
DECLARE @sql as varchar(1000)
/******************************************************************************
**	File: 
**	Name: dbo.IsChildLocked
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
**	@ChildID						ChildID
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
	SELECT (CASE WHEN (([Locked] <> 0) AND ([Locked] <> NULL))
		THEN Locked
	ELSE
		0 
	END) AS Locked
	FROM Child
	WHERE ChildID = @ChildID
END
");

            CreateStoredProcedure("dbo.GetCurrentChildStatus", p => new { ChildID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.GetCurrentChildStatus
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
**	@StatusID						StatusID
**									Description
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
	SELECT TOP 1 ChildStatusHistoryID, ChildID, ChildStatusID, CreatedUserID, CreatedDate, EffectiveDate
	FROM ChildStatusHistory
	WHERE ChildID = @ChildID
	AND EffectiveDate <= GetDate()
	ORDER BY EffectiveDate DESC, ChildStatusHistoryID DESC
END
");

            CreateStoredProcedure("dbo.IsChildUnique", p => new { ChildID = p.Int(), FirstName = p.String(maxLength: 30), LastName = p.String(maxLength: 30), }, @"
DECLARE @sql as varchar(1000)
/******************************************************************************
**	File: 
**	Name: dbo.IsChildUnique
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
**	@ChildID						ChildID
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
	SET @sql = '
	SELECT COUNT(*) AS CountChild
	FROM Child
	WHERE ChildID <> ' + CAST(@ChildID AS CHAR) + '
	AND FirstName = ''' + REPLACE(RTRIM(LTRIM(@FirstName)), '''', '''''') + ''' 
	AND LastName = ''' + REPLACE(RTRIM(LTRIM(@LastName)), '''', '''''') + ''''
	EXEC(@sql)
END
");

            CreateStoredProcedure("dbo.GetDisabilities", @"

/******************************************************************************
**	File: 
**	Name: dbo.GetDisabilities
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
	SELECT DisabilityID, Description
	FROM Disability
END
");

            CreateStoredProcedure("dbo.IsCommunityCouncilInUse", p => new { CommunityCouncilID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.IsCommunityCouncilInUse
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
	SELECT COUNT(*) AS CountCommunityCouncil
	FROM Child
	WHERE CommunityCouncilID = @CommunityCouncilID
END
");

            CreateStoredProcedure("dbo.GetDisabilitiesForChild", p => new { ChildID = p.Int(), }, @"
DECLARE @strWhere VARCHAR(8000)
DECLARE @strSelect VARCHAR(8000)

/******************************************************************************
**	File: 
**	Name: dbo.GetDisabilitiesForChild
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
	SET @strSelect = 'SELECT [ChildDisability].[DisabilityID], ChildDisability.ChildDisabilityID, [Description] 
										FROM [ChildDisability] INNER JOIN [Disability] ON [ChildDisability].[DisabilityID] = [Disability].[DisabilityID] 
										WHERE [ChildDisability].[ChildID] = ' + CAST(@ChildID AS CHAR)
	EXEC(@strSelect)
END
");

            CreateStoredProcedure("dbo.IsCommunityCouncilUnique", p => new { CommunityCouncilID = p.Int(), Description = p.String(maxLength: 50), }, @"
DECLARE @sql AS VARCHAR(1000)
/******************************************************************************
**	File: 
**	Name: dbo.IsCommunityCouncilUnique
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
	SET @sql = '
	SELECT COUNT(*) AS CountCommunityCouncil
	FROM CommunityCouncil
	WHERE CommunityCouncilID <> ' + CAST(@CommunityCouncilID AS CHAR) + '
	AND Description = ''' + REPLACE(RTRIM(LTRIM(@Description)), '''', '''''') + '''' 
	EXEC(@sql)
END
");

            CreateStoredProcedure("dbo.GetDisability", p => new { DisabilityID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.GetDisability
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
**	@DisabilityID						DisabilityID
**									Description
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
	SELECT Description
	FROM Disability
	WHERE DisabilityID = @DisabilityID
END
");

            CreateStoredProcedure("dbo.IsDisabilityInUse", p => new { DisabilityID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.IsDisabilityInUse
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
	SELECT COUNT(*) AS CountDisability
	FROM ChildDisability
	WHERE DisabilityID = @DisabilityID
END
");

            CreateStoredProcedure("dbo.GetDistrict", p => new { DistrictID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.GetDistrict
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
**	@DistrictID						DistrictID
**									Description
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
	SELECT Description
	FROM District
	WHERE DistrictID = @DistrictID
END
");

            CreateStoredProcedure("dbo.IsDisabilityUnique", p => new { DisabilityID = p.Int(), Description = p.String(maxLength: 50), }, @"
DECLARE @sql AS VARCHAR(1000)
/******************************************************************************
**	File: 
**	Name: dbo.IsDisabilityUnique
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
	SET @sql = '
	SELECT COUNT(*) AS CountDisability
	FROM Disability
	WHERE DisabilityID <> ' + CAST(@DisabilityID AS CHAR) + '
	AND Description = ''' + REPLACE(RTRIM(LTRIM(@Description)), '''', '''''') + '''' 
	EXEC(@sql)
END
");

            CreateStoredProcedure("dbo.GetDistricts", @"

/******************************************************************************
**	File: 
**	Name: dbo.GetDistricts
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
	SELECT DistrictID, Description
	FROM District
END
");

            CreateStoredProcedure("dbo.IsDistrictInUse", p => new { DistrictID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.IsDistrictInUse
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
	SELECT COUNT(*) AS CountDistrict
	FROM Child
	WHERE DistrictID = @DistrictID
END
");

            CreateStoredProcedure("dbo.GetDomain", p => new { DomainID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.GetDomain
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
**	@DomainID						DomainID
**									Description
**									DomainCode
**									DomainColor
**									Guidelines
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
	SELECT Description,DomainCode, DomainColor, Guidelines
	FROM Domain
	WHERE DomainID = @DomainID
END
");

            CreateStoredProcedure("dbo.IsDistrictUnique", p => new { DistrictID = p.Int(), Description = p.String(maxLength: 50), }, @"
DECLARE @sql AS VARCHAR(1000)
/******************************************************************************
**	File: 
**	Name: dbo.IsDistrictUnique
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
	SET @sql = '
	SELECT COUNT(*) AS CountDistrict
	FROM District
	WHERE DistrictID <> ' + CAST(@DistrictID AS CHAR) + '
	AND Description = ''' + REPLACE(RTRIM(LTRIM(@Description)), '''', '''''') + '''' 
	EXEC(@sql)
END
");

            CreateStoredProcedure("dbo.GetDomains", @"

/******************************************************************************
**	File: 
**	Name: dbo.GetDomains
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
	SELECT DomainID,Description,DomainCode, DomainColor, Guidelines
	FROM Domain
END
");

            CreateStoredProcedure("dbo.IsDomainInUse", p => new { DomainID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.IsDomainInUse
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
	SELECT COUNT(*) AS CountDomain
	FROM Question
	WHERE DomainID = @DomainID
END
");

            CreateStoredProcedure("dbo.GetDomainsForCSI", p => new { CSIID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.GetDomainsForCSI
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
**	@DomainID						DomainID
**									Description
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
	SELECT CSIDomainID, CSIID, DomainID 
	FROM CSIDomain 
	WHERE CSIDomain.CSIID = @CSIID
END
");

            CreateStoredProcedure("dbo.IsEventInUse", p => new { EventID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.IsEventInUse
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
	SELECT COUNT(*) AS CountEvent
	FROM CSIEvent
	WHERE EventID = @EventID
END
");

            CreateStoredProcedure("dbo.GetDomainsForWBT", p => new { WBTID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.GetDomainsForWBT
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
**	@DomainID						DomainID
**									Description
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
	SELECT WBTDomainID, WBTID, DomainID 
	FROM WBTDomain 
	WHERE WBTDomain.WBTID = @WBTID
END
");

            CreateStoredProcedure("dbo.IsEventUnique", p => new { EventID = p.Int(), Description = p.String(maxLength: 50), }, @"
DECLARE @sql AS VARCHAR(1000)
/******************************************************************************
**	File: 
**	Name: dbo.IsEventUnique
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
	SET @sql = '
	SELECT COUNT(*) AS CountEvent
	FROM Event
	WHERE EventID <> ' + CAST(@EventID AS CHAR) + '
	AND Description = ''' + REPLACE(RTRIM(LTRIM(@Description)), '''', '''''') + '''' 
	EXEC(@sql)
END
");

            CreateStoredProcedure("dbo.GetEvent", p => new { EventID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.GetEvent
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
**	@EventID						EventID
**									Description
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
	SELECT Description
	FROM Event
	WHERE EventID = @EventID
END
");

            CreateStoredProcedure("dbo.IsGuardianRelationInUse", p => new { GuardianRelationID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.IsGuardianRelationInUse
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
	SELECT COUNT(*) AS CountGuardianRelation
	FROM Child
	WHERE GuardianRelationID = @GuardianRelationID
END
");

            CreateStoredProcedure("dbo.GetEvents", @"

/******************************************************************************
**	File: 
**	Name: dbo.GetEvents
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
	SELECT EventID, Description
	FROM Event
END
");

            CreateStoredProcedure("dbo.IsGuardianRelationUnique", p => new { GuardianRelationID = p.Int(), Description = p.String(maxLength: 50), }, @"
DECLARE @sql AS VARCHAR(1000)
/******************************************************************************
**	File: 
**	Name: dbo.IsGuardianRelationUnique
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
	SET @sql = '
	SELECT COUNT(*) AS CountGuardianRelation
	FROM GuardianRelation
	WHERE RelationID <> ' + CAST(@GuardianRelationID AS CHAR) + '
	AND Description = ''' + REPLACE(RTRIM(LTRIM(@Description)), '''', '''''') + '''' 
	EXEC(@sql)
END
");

            CreateStoredProcedure("dbo.GetEventsForCSI", p => new { CSIID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.GetEventsForCSI
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
	SELECT [CSIEventID], [CSIID], [EventID], [Comments], [EffectiveDate] 
	FROM [CSIEvent] WHERE [CSIEvent].[CSIID] = @CSIID
END
");

            CreateStoredProcedure("dbo.IsOVCTypeInUse", p => new { OVCTypeID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.IsOVCTypeInUse
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
	SELECT COUNT(*) AS CountOVCType
	FROM Child
	WHERE OVCTypeID = @OVCTypeID
END
");

            CreateStoredProcedure("dbo.GetFollowUp", p => new { FollowUpID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.GetFollowUp
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
**	@FollowUpID						FollowUpID
**									ChildID
**									DateOfFollowUp
**									CreatedUserID
**									CreatedDate
**									RecipientTypeID
**									OutcomeID
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
	SELECT ChildID, DateOfFollowUp, CreatedUserID, CreatedDate, RecipientTypeID, OutcomeID
	FROM FollowUp
	WHERE FollowUpID = @FollowUpID
END
");

            CreateStoredProcedure("dbo.IsOVCTypeUnique", p => new { OVCTypeID = p.Int(), Description = p.String(maxLength: 100), }, @"
DECLARE @sql AS VARCHAR(1000)
/******************************************************************************
**	File: 
**	Name: dbo.IsOVCTypeUnique
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
	SET @sql = '
	SELECT COUNT(*) AS CountOVCType
	FROM OVCType
	WHERE OVCTypeID <> ' + CAST(@OVCTypeID AS CHAR) + '
	AND Description = ''' + REPLACE(RTRIM(LTRIM(@Description)), '''', '''''') + '''' 
	EXEC(@sql)
END
");

            CreateStoredProcedure("dbo.GetGuardianRelation", p => new { RelationID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.GetGuardianRelation
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
**	@RelationID						RelationID
**									Description
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
	SELECT Description
	FROM GuardianRelation
	WHERE RelationID = @RelationID
END
");

            CreateStoredProcedure("dbo.IsPartnerInUse", p => new { PartnerID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.IsPartnerInUse
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
**	@ChildID						ChildID
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
	SELECT COUNT(*) AS CountPartner
	FROM ChildPartner
	WHERE PartnerID = @PartnerID
END
");

            CreateStoredProcedure("dbo.GetGuardianRelations", @"

/******************************************************************************
**	File: 
**	Name: dbo.GetGuardianRelations
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
	SELECT RelationID, Description
	FROM GuardianRelation
END
");

            CreateStoredProcedure("dbo.IsPartnerUnique", p => new { PartnerID = p.Int(), Name = p.String(maxLength: 50), }, @"
DECLARE @sql AS VARCHAR(1000)
/******************************************************************************
**	File: 
**	Name: dbo.IsPartnerUnique
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
**	@ChildID						ChildID
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
	SET @sql = '
	SELECT COUNT(*) AS CountPartner
	FROM Partner
	WHERE PartnerID <> ' + CAST(@PartnerID AS CHAR) + '
	AND Name = ''' + REPLACE(RTRIM(LTRIM(@Name)), '''', '''''') + '''' 
	EXEC(@sql)
END
");

            CreateStoredProcedure("dbo.GetGuideline", p => new { GuidelineID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.GetGuideline
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
**	@GuidelineID						GuidelineID
**									DescriptionEnglish
**									DescriptionSesotho
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
	SELECT DescriptionEnglish, DescriptionSesotho
	FROM Guideline
	WHERE GuidelineID = @GuidelineID
END
");

            CreateStoredProcedure("dbo.IsServiceInUse", p => new { ServiceID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.IsServiceInUse
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
	SELECT COUNT(*) AS CountService
	FROM Service
	WHERE ServiceID = @ServiceID
END
");

            CreateStoredProcedure("dbo.GetHomeEnvironment", p => new { HomeEnvironmentID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.GetHomeEnvironment
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
	SELECT HouseholdID, HouseMembersCount, HouseOVCsMale, HouseOVCsFemale, HouseMinorsMale, HouseMinorsFemale, HouseAdequate, HouseAdequateSpecify, FoodSourceFarming, FoodSourceRelative, FoodSourceDonations, FoodSourceNoReliable, AllHHMembersWell, HHMembersOccassionallyIll, HHMembersFrequentlySick, OneHHMemberIll, GoToHealthCentre, CommunityHealthWorker, TraditionalHealer, NoHealthCare, LatrineInGoodRepair, LatrineInBadRepair, SharingLatrine, NoLatrine, WarmClothing, Blanket, Shoes, SchoolUniform, ClothingSpecify, NeedsFood, NeedsSchool, NeedsPsychosocial, NeedsGuardianship, NeedsToiletries, NeedsSchoolSupplies, NeedsShelter, NeedsHealth, NoSpecialNeed, CaregiverWithDisabilities, SpecialNeedsSpecify, SizeOfUniform, SizeOfShoes, Remarks
	FROM HomeEnvironment
	WHERE HomeEnvironmentID = @HomeEnvironmentID
END
");

            CreateStoredProcedure("dbo.IsServiceProviderInUse", p => new { ServiceProviderID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.IsServiceProviderInUse
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
	SELECT COUNT(*) AS CountServiceProvider
	FROM CSIDomainSupportService
	WHERE ServiceProviderID = @ServiceProviderID
END
");

            CreateStoredProcedure("dbo.GetHomeEnvironmentByChildID", p => new { ChildID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.GetHomeEnvironmentByChildID
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
	SELECT HomeEnvironmentID, ChildID, HouseMembersCount, HouseOVCsMale, HouseOVCsFemale, HouseMinorsMale, HouseMinorsFemale, HouseAdequate, HouseAdequateSpecify, FoodSourceFarming, FoodSourceRelative, FoodSourceDonations, FoodSourceNoReliable, AllHHMembersWell, HHMembersOccassionallyIll, HHMembersFrequentlySick, OneHHMemberIll, GoToHealthCentre, CommunityHealthWorker, TraditionalHealer, NoHealthCare, LatrineInGoodRepair, LatrineInBadRepair, SharingLatrine, NoLatrine, WarmClothing, Blanket, Shoes, SchoolUniform, ClothingSpecify, NeedsFood, NeedsSchool, NeedsPsychosocial, NeedsGuardianship, NeedsToiletries, NeedsSchoolSupplies, NeedsShelter, NeedsHealth, NoSpecialNeed, CaregiverWithDisabilities, SpecialNeedsSpecify, SizeOfUniform, SizeOfShoes, Remarks
	FROM HomeEnvironment
	WHERE ChildID = @ChildID
END
");

            CreateStoredProcedure("dbo.IsServiceProviderTypeInUse", p => new { ServiceProviderTypeID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.IsServiceProviderTypeInUse
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
	SELECT COUNT(*) AS CountServiceProviderType
	FROM ServiceProvider
	WHERE ServiceProviderTypeID = @ServiceProviderTypeID
END
");

            CreateStoredProcedure("dbo.GetHomeEnvironmentByHouseholdID", p => new { HouseholdID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.GetHomeEnvironmentByHouseholdID
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
	SELECT HomeEnvironmentID, HouseMembersCount, HouseOVCsMale, HouseOVCsFemale, HouseMinorsMale, HouseMinorsFemale, HouseAdequate, HouseAdequateSpecify, FoodSourceFarming, FoodSourceRelative, FoodSourceDonations, FoodSourceNoReliable, AllHHMembersWell, HHMembersOccassionallyIll, HHMembersFrequentlySick, OneHHMemberIll, GoToHealthCentre, CommunityHealthWorker, TraditionalHealer, NoHealthCare, LatrineInGoodRepair, LatrineInBadRepair, SharingLatrine, NoLatrine, WarmClothing, Blanket, Shoes, SchoolUniform, ClothingSpecify, NeedsFood, NeedsSchool, NeedsPsychosocial, NeedsGuardianship, NeedsToiletries, NeedsSchoolSupplies, NeedsShelter, NeedsHealth, NoSpecialNeed, CaregiverWithDisabilities, SpecialNeedsSpecify, SizeOfUniform, SizeOfShoes, Remarks
	FROM HomeEnvironment
	WHERE HouseholdID = @HouseholdID
END
");

            CreateStoredProcedure("dbo.IsServiceProviderTypeUnique", p => new { ServiceProviderTypeID = p.Int(), Description = p.String(maxLength: 50), }, @"
DECLARE @sql AS VARCHAR(1000)
/******************************************************************************
**	File: 
**	Name: dbo.IsServiceProviderTypeUnique
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
	SET @sql = '
	SELECT COUNT(*) AS CountServiceProviderType
	FROM ServiceProviderType
	WHERE ServiceProviderTypeID <> ' + CAST(@ServiceProviderTypeID AS CHAR) + '
	AND Description = ''' + REPLACE(RTRIM(LTRIM(@Description)), '''', '''''') + '''' 
	EXEC(@sql)
END
");

            CreateStoredProcedure("dbo.GetHousehold", p => new { HouseholdID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.GetHousehold
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
	SELECT Address, VillageID, DistrictID, Country, ParticipantRegistrationID, CreatedDate, CreatedUser, LastUpdatedDate, LastUpdatedUser
	FROM Household
	WHERE HouseholdID = @HouseholdID
END
");

            CreateStoredProcedure("dbo.IsServiceProviderUnique", p => new { ServiceProviderID = p.Int(), Description = p.String(maxLength: 50), }, @"
DECLARE @sql AS VARCHAR(1000)
/******************************************************************************
**	File: 
**	Name: dbo.IsServiceProviderUnique
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
	SET @sql = '
	SELECT COUNT(*) AS CountServiceProvider
	FROM ServiceProvider
	WHERE ServiceProviderID <> ' + CAST(@ServiceProviderID AS CHAR) + '
	AND Description = ''' + REPLACE(RTRIM(LTRIM(@Description)), '''', '''''') + '''' 
	EXEC(@sql)
END
");

            CreateStoredProcedure("dbo.GetHouseholdByAdultID", p => new { AdultID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.GetHouseholdByAdultID
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
	SELECT Household.HouseholdID, Address, VillageID, DistrictID, Country, ParticipantRegistrationID, CreatedDate, CreatedUser, LastUpdatedDate, LastUpdatedUser
	FROM Household
	INNER JOIN AdultHousehold ON AdultHousehold.HouseholdID = Household.HouseholdID
	WHERE AdultID = @AdultID
END
");

            CreateStoredProcedure("dbo.IsServiceUnique", p => new { ServiceID = p.Int(), Description = p.String(maxLength: 100), }, @"
DECLARE @sql AS VARCHAR(1000)
/******************************************************************************
**	File: 
**	Name: dbo.IsServiceUnique
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
	SET @sql = '
	SELECT COUNT(*) AS CountService
	FROM Service
	WHERE ServiceID <> ' + CAST(@ServiceID AS CHAR) + '
	AND Description = ''' + REPLACE(RTRIM(LTRIM(@Description)), '''', '''''') + '''' 
	EXEC(@sql)
END
");

            CreateStoredProcedure("dbo.GetHouseholdByChildID", p => new { ChildID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.GetHouseholdByChildID
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
	SELECT Household.HouseholdID, Address, VillageID, DistrictID, Country, ParticipantRegistrationID, CreatedDate, CreatedUser, LastUpdatedDate, LastUpdatedUser
	FROM Household
	INNER JOIN ChildHousehold ON ChildHousehold.HouseholdID = Household.HouseholdID
	WHERE ChildID = @ChildID
END
");

            CreateStoredProcedure("dbo.IsSourceInUse", p => new { SourceID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.IsSourceInUse
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
	SELECT COUNT(*) AS CountSource
	FROM CSIDomainSource
	WHERE SourceID = @sourceID
END
");

            CreateStoredProcedure("dbo.GetHouseholdList", p => new { FirstName = p.String(maxLength: 100), Lastname = p.String(maxLength: 100), }, @"
DECLARE @strWhere VARCHAR(8000)
DECLARE @strSelect VARCHAR(8000)

/******************************************************************************
**	File: 
**	Name: dbo.GetHouseholdList
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
	IF (@FirstName <> '') 
	BEGIN
		IF (@LastName <> '')
		BEGIN
			SET @strWhere = 'WHERE (c.[FirstName] Like ''%' + @FirstName + '%'''
			SET @strWhere = @strWhere + 'AND c.[LastName] Like ''%' + @FirstName + '%'') '
			SET @strWhere = @strWhere + 'OR (a.[FirstName] Like ''%' + @FirstName + '%'''
			SET @strWhere = @strWhere + 'AND a.[LastName] Like ''%' + @FirstName + '%'') '
		END
		ELSE
		BEGIN
			SET @strWhere = 'WHERE c.[FirstName] Like ''%' + @FirstName + '%'''
			SET @strWhere = @strWhere + 'OR a.[FirstName] Like ''%' + @FirstName + '%'''
		END
	END
	ELSE
	BEGIN
		IF (@LastName <> '')
		BEGIN
			SET @strWhere = 'WHERE c.[LastName] Like ''%' + @LastName + '%'''
			SET @strWhere = @strWhere + 'OR a.[LastName] Like ''%' + @LastName + '%'''
		END
	END

	SET @strSelect = 'SELECT distinct h.HouseholdID, h.[Address]
										FROM Household h
										left outer join ChildHousehold ch on ch.HouseholdID = h.HouseholdID
										left outer join Child c on c.ChildID = ch.ChildID
										left outer join AdultHousehold ah on ah.HouseholdID = h.HouseholdID
										left outer join Adult a on a.adultid = ah.AdultID ' + @strWhere + ' ORDER BY h.HouseholdID ASC '
	EXEC(@strSelect)
END
");

            CreateStoredProcedure("dbo.IsSourceUnique", p => new { SourceID = p.Int(), Description = p.String(maxLength: 50), }, @"
DECLARE @sql AS VARCHAR(1000)
/******************************************************************************
**	File: 
**	Name: dbo.IsSourceUnique
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
	SET @sql = '
	SELECT COUNT(*) AS CountSource
	FROM Source
	WHERE SourceID <> ' + CAST(@SourceID AS CHAR) + '
	AND Description = ''' + REPLACE(RTRIM(LTRIM(@Description)), '''', '''''') + '''' 
	EXEC(@sql)
END
");

            CreateStoredProcedure("dbo.GetHouseholdProject", p => new { HouseholdProjectID = p.Int(), }, @"

/******************************************************************************
**	File: 
**	Name: dbo.GetHouseholdProject
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
**	@UserActionID						UserActionID
**									UserID
**									SiteID
**									ActionID
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
	SELECT HouseholdID, HouseholdID, ParticipantRegistrationID, FieldAgentMonitoringID, TrainingRegistryID, CreatedDate
	FROM HouseholdProject
	WHERE HouseholdProjectID = @HouseholdProjectID
END
");
        }   
        
        public override void Down()
        {
        }
    }
}
