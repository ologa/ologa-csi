namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddHIVStatus : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.HIVStatus",
                c => new
                {
                    HIVStatusID = c.Int(nullable: false, identity: true),
                    RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    HIVStatus_guid = c.Guid(nullable: false, defaultValueSql: "newid()"),
                    HIV = c.String(maxLength: 1),
                    HIVInTreatment = c.Int(nullable: false),
                    HIVUndisclosedReason = c.Int(nullable: false),
                    InformationDate = c.DateTime(nullable: false),
                    CreatedAt = c.DateTime(nullable: false),
                    AdultID = c.Int(nullable: false),
                    ChildID = c.Int(nullable: false),
                    UserID = c.Int(),
                })
                .PrimaryKey(t => t.HIVStatusID)
                .ForeignKey("dbo.User", t => t.UserID)
                .Index(t => t.UserID, name: "IX_User_UserID");

            AddColumn("dbo.Adult", "HIVStatusID", c => c.Int());
            AddColumn("dbo.Child", "HIVStatusID", c => c.Int());
            AlterColumn("dbo.Child", "HIV", c => c.String(maxLength: 1));
            AlterColumn("dbo.Child", "HIVDisclosed", c => c.String(maxLength: 1));
            CreateIndex("dbo.Adult", "HIVStatusID", name: "IX_HIVStatus_HIVStatusID");
            CreateIndex("dbo.Child", "HIVStatusID", name: "IX_HIVStatus_HIVStatusID");
            AddForeignKey("dbo.Child", "HIVStatusID", "dbo.HIVStatus", "HIVStatusID");
            AddForeignKey("dbo.Adult", "HIVStatusID", "dbo.HIVStatus", "HIVStatusID");

            AlterStoredProcedure("dbo.GetChild", p => new { ChildID = p.Int(), }, @"
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
                **                  				ContactNo
                **									Notes
                **									CreatedDate
                **									CreatedUser
                **									LastUpdatedDate
                **									LastUpdatedUser
                **									Locked
                **									PrincipalChief
                **									VillageChief
                **									CommunityCouncilID
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
    	            SELECT FirstName, LastName, Gender, DateOfBirth, DateOfBirthUnknown, WardID, VillageID, DistrictID, Guardian, GuardianRelationID, GuardianIdNo, ContactNo, Notes, CreatedDate, CreatedUserID, LastUpdatedDate, LastUpdatedUserID, Locked, PrincipalChief, VillageChief, CommunityCouncilID, SchoolName, SchoolContactNo, DisabilityNotes, PrincipalName, PrincipalContactNo, TeacherName, TeacherContactNo, OVCTypeID, HeadName, HeadContactNo, Address, Grade, Class
    	            FROM Child
    	            WHERE ChildID = @ChildID
                END
                 ");



            AlterStoredProcedure("dbo.GetChildrenByHouseholdID", p => new { HouseholdID = p.Int(), }, @"
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
    	            SELECT Child.ChildID, FirstName, LastName, Gender, DateOfBirth, DateOfBirthUnknown, WardID, VillageID, DistrictID, Guardian, GuardianRelationID, GuardianIdNo, ContactNo, Notes, CreatedDate, CreatedUserID, LastUpdatedDate, LastUpdatedUserID, Locked, PrincipalChief, VillageChief, CommunityCouncilID, SchoolName, SchoolContactNo, DisabilityNotes, PrincipalName, PrincipalContactNo, TeacherName, TeacherContactNo, OVCTypeID, HeadName, HeadContactNo, Address, Grade, Class
    	            FROM Child
    	            INNER JOIN ChildHousehold ON Child.ChildID = ChildHousehold.ChildID
    	            WHERE HouseholdID = @HouseholdID
                END
                 ");

            AlterStoredProcedure("dbo.GetChildren", @"
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
    	            SELECT ChildID, FirstName, LastName, Gender, DateOfBirth, DateOfBirthUnknown, WardID, VillageID, DistrictID, Guardian, GuardianRelationID, GuardianIdNo, ContactNo, Notes, CreatedDate, CreatedUserID, LastUpdatedDate, LastUpdatedUserID, Locked, PrincipalChief, VillageChief, CommunityCouncilID, SchoolName, SchoolContactNo, DisabilityNotes, PrincipalName, PrincipalContactNo, TeacherName, TeacherContactNo, OVCTypeID, HeadName, HeadContactNo, Address, Grade, Class
    	            FROM Child
                END
                 ");

            AlterStoredProcedure("dbo.GetChildren", @"
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
    	            SELECT ChildID, FirstName, LastName, Gender, DateOfBirth, DateOfBirthUnknown, WardID, VillageID, DistrictID, Guardian, GuardianRelationID, GuardianIdNo, ContactNo, Notes, CreatedDate, CreatedUserID, LastUpdatedDate, LastUpdatedUserID, Locked, PrincipalChief, VillageChief, CommunityCouncilID, SchoolName, SchoolContactNo, DisabilityNotes, PrincipalName, PrincipalContactNo, TeacherName, TeacherContactNo, OVCTypeID, HeadName, HeadContactNo, Address, Grade, Class
    	            FROM Child
                END
                 ");

            AlterStoredProcedure("dbo.InsertChild", p => new { FirstName = p.String(maxLength: 30), LastName = p.String(maxLength: 30), Gender = p.String(maxLength: 1), DateOfBirth = p.DateTime(), DateOfBirthUnknown = p.String(maxLength: 5), WardID = p.Int(), VillageID = p.Int(), DistrictID = p.Int(), Guardian = p.String(maxLength: 50), GuardianRelationID = p.Int(), GuardianIdNo = p.String(maxLength: 15), ContactNo = p.String(maxLength: 15), Notes = p.String(maxLength: 1000), CreatedDate = p.DateTime(), CreatedUser = p.Int(), LastUpdatedDate = p.DateTime(), LastUpdatedUser = p.Int(), Locked = p.Int(), PrincipalChief = p.String(maxLength: 50), VillageChief = p.String(maxLength: 50), CommunityCouncilID = p.Int(), SchoolName = p.String(maxLength: 50), SchoolContactNo = p.String(maxLength: 20), DisabilityNotes = p.String(), PrincipalName = p.String(maxLength: 100), PrincipalContactNo = p.String(maxLength: 20), TeacherName = p.String(maxLength: 100), TeacherContactNo = p.String(maxLength: 20), OVCTypeID = p.Int(), HeadName = p.String(maxLength: 100), HeadContactNo = p.String(maxLength: 20), Address = p.String(maxLength: 200), Grade = p.String(maxLength: 2), Class = p.String(maxLength: 1), }, @"
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
    	            INSERT INTO Child (FirstName, LastName, Gender, DateOfBirth, DateOfBirthUnknown, WardID, VillageID, DistrictID, Guardian, GuardianRelationID, GuardianIdNo, ContactNo, Notes, CreatedDate, CreatedUserID, LastUpdatedDate, LastUpdatedUserID, Locked, PrincipalChief, VillageChief, CommunityCouncilID, SchoolName, SchoolContactNo, DisabilityNotes, PrincipalName, PrincipalContactNo, TeacherName, TeacherContactNo, OVCTypeID, HeadName, HeadContactNo, Address, Grade, Class)
    	            VALUES (@FirstName, @LastName, @Gender, @DateOfBirth, @DateOfBirthUnknown, @WardID, @VillageID, @DistrictID, @Guardian, @GuardianRelationID, @GuardianIdNo, @ContactNo, @Notes, @CreatedDate, @CreatedUser, @LastUpdatedDate, @LastUpdatedUser, @Locked, @PrincipalChief, @VillageChief, @CommunityCouncilID, @SchoolName, @SchoolContactNo, @DisabilityNotes, @PrincipalName, @PrincipalContactNo, @TeacherName, @TeacherContactNo, @OVCTypeID, @HeadName, @HeadContactNo, @Address, @Grade, @Class)
    
    	            SELECT @@Identity
                END
            ");

            AlterStoredProcedure("dbo.UpdateChild", p => new { ChildID = p.Int(), FirstName = p.String(maxLength: 30), LastName = p.String(maxLength: 30), Gender = p.String(maxLength: 1), DateOfBirth = p.DateTime(), DateOfBirthUnknown = p.String(maxLength: 5), WardID = p.Int(), VillageID = p.Int(), DistrictID = p.Int(), Guardian = p.String(maxLength: 50), GuardianRelationID = p.Int(), GuardianIdNo = p.String(maxLength: 15), ContactNo = p.String(maxLength: 15), Notes = p.String(maxLength: 1000), CreatedDate = p.DateTime(), CreatedUser = p.Int(), LastUpdatedDate = p.DateTime(), LastUpdatedUser = p.Int(), Locked = p.Int(), PrincipalChief = p.String(maxLength: 50), VillageChief = p.String(maxLength: 50), CommunityCouncilID = p.Int(), SchoolName = p.String(maxLength: 50), SchoolContactNo = p.String(maxLength: 20), DisabilityNotes = p.String(), PrincipalName = p.String(maxLength: 100), PrincipalContactNo = p.String(maxLength: 20), TeacherName = p.String(maxLength: 100), TeacherContactNo = p.String(maxLength: 20), OVCTypeID = p.Int(), HeadName = p.String(maxLength: 100), HeadContactNo = p.String(maxLength: 20), Address = p.String(maxLength: 200), Grade = p.String(maxLength: 2), Class = p.String(maxLength: 1), }, @"
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
                END");

            // insertChildHIVStatus
            Sql(@"INSERT INTO  [HIVStatus] ([ChildID],[AdultID],[HIV],[HIVInTreatment],[HIVUndisclosedReason],[InformationDate],[CreatedAt],[UserID])
                SELECT [ChildID],0,[HIV],[HIVInTreatment],[HIVUndisclosedReason],[LastUpdatedDate],[CreatedDate],[CreatedUserID] FROM  [Child]", false);

            // updateChildHIVStatus
            Sql(@"UPDATE C SET C.HIVStatusID = H.HIVStatusID FROM [HIVStatus] AS H INNER JOIN [Child] AS C ON C.ChildID = H.ChildID", false);

            // insertAdultHIVStatus
            Sql(@"INSERT INTO  [HIVStatus] ([AdultID],[ChildID],[HIV],[HIVInTreatment],[HIVUndisclosedReason],[InformationDate],[CreatedAt],[UserID])
                SELECT [AdultID],0,[HIV],[HIVInTreatment],[HIVUndisclosedReason],[LastUpdatedDate],[CreatedDate],[CreatedUserID] FROM  [Adult]", false);

            // updateAdultHIVStatus
            Sql(@"UPDATE A SET A.HIVStatusID = H.HIVStatusID FROM  [HIVStatus] AS H INNER JOIN  [Adult] AS A ON A.AdultID = H.AdultID", false);
        }

        public override void Down()
        {
            DropForeignKey("dbo.Adult", "HIVStatusID", "dbo.HIVStatus");
            DropForeignKey("dbo.Child", "HIVStatusID", "dbo.HIVStatus");
            DropForeignKey("dbo.HIVStatus", "UserID", "dbo.User");
            DropIndex("dbo.HIVStatus", "IX_User_UserID");
            DropIndex("dbo.Child", "IX_HIVStatus_HIVStatusID");
            DropIndex("dbo.Adult", "IX_HIVStatus_HIVStatusID");
            AlterColumn("dbo.Child", "HIVDisclosed", c => c.String(maxLength: 1, fixedLength: true, unicode: false));
            AlterColumn("dbo.Child", "HIV", c => c.String(maxLength: 1, fixedLength: true, unicode: false));
            DropColumn("dbo.Child", "HIVStatusID");
            DropColumn("dbo.Adult", "HIVStatusID");
            DropTable("dbo.HIVStatus");
        }
    }
}
