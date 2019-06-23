namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IntitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Beneficiary",
                c => new
                    {
                        ChildID = c.Int(nullable: false, identity: true),
                        FirstName = c.String(nullable: false, maxLength: 30, unicode: false),
                        LastName = c.String(nullable: false, maxLength: 30, unicode: false),
                        Gender = c.String(nullable: false, maxLength: 1, fixedLength: true, unicode: false),
                        DateOfBirth = c.DateTime(nullable: false),
                        DateOfBirthUnknown = c.Boolean(nullable: false),
                        Guardian = c.String(maxLength: 50, unicode: false),
                        Notes = c.String(unicode: false, storeType: "text"),
                        CreatedDate = c.DateTime(nullable: false),
                        LastUpdatedDate = c.DateTime(nullable: false),
                        Locked = c.Int(),
                        PrincipalChief = c.String(maxLength: 50, unicode: false),
                        VillageChief = c.String(maxLength: 50, unicode: false),
                        HIV = c.String(maxLength: 1, fixedLength: true, unicode: false),
                        child_guid = c.Guid(nullable: false, defaultValueSql: "newid()"),
                        HIVDisclosed = c.String(maxLength: 1, fixedLength: true, unicode: false),
                        EducationBursary = c.String(maxLength: 1, fixedLength: true, unicode: false),
                        HighestSchoolLevelID = c.Int(),
                        EligibleFamilyReunification = c.String(maxLength: 1, fixedLength: true, unicode: false),
                        MaterialStationery = c.String(maxLength: 1, fixedLength: true, unicode: false),
                        Literate = c.String(maxLength: 1, fixedLength: true, unicode: false),
                        Numeracy = c.String(maxLength: 1, fixedLength: true, unicode: false),
                        MarketingStarter = c.String(maxLength: 1, fixedLength: true, unicode: false),
                        CapacityIGA = c.String(maxLength: 1, fixedLength: true, unicode: false),
                        EarnIGA = c.String(maxLength: 1, fixedLength: true, unicode: false),
                        SufficientMeals = c.String(maxLength: 1, fixedLength: true, unicode: false),
                        GuardianIdNo = c.String(maxLength: 50, unicode: false),
                        ContactNo = c.String(maxLength: 20, unicode: false),
                        SchoolName = c.String(maxLength: 50, unicode: false),
                        SchoolContactNo = c.String(maxLength: 20, unicode: false),
                        DisabilityNotes = c.String(maxLength: 500, unicode: false),
                        PrincipalName = c.String(maxLength: 50, unicode: false),
                        PrincipalContactNo = c.String(maxLength: 20, unicode: false),
                        TeacherName = c.String(maxLength: 50, unicode: false),
                        TeacherContactNo = c.String(maxLength: 20, unicode: false),
                        HeadName = c.String(maxLength: 50, unicode: false),
                        HeadContactNo = c.String(maxLength: 20, unicode: false),
                        Address = c.String(maxLength: 100, unicode: false),
                        Grade = c.String(maxLength: 20, unicode: false),
                        Class = c.String(maxLength: 20, unicode: false),
                        CommunityCouncilID = c.Int(),
                        CreatedUserID = c.Int(),
                        DistrictID = c.Int(),
                        GuardianRelationID = c.Int(),
                        LastUpdatedUserID = c.Int(),
                        OVCTypeID = c.Int(),
                        VillageID = c.Int(),
                        WardID = c.Int(),
                    })
                .PrimaryKey(t => t.ChildID)
                .ForeignKey("dbo.CommunityCouncil", t => t.CommunityCouncilID)
                .ForeignKey("dbo.User", t => t.CreatedUserID)
                .ForeignKey("dbo.District", t => t.DistrictID)
                .ForeignKey("dbo.GuardianRelation", t => t.GuardianRelationID)
                .ForeignKey("dbo.User", t => t.LastUpdatedUserID)
                .ForeignKey("dbo.OVCType", t => t.OVCTypeID)
                .ForeignKey("dbo.Village", t => t.VillageID)
                .ForeignKey("dbo.Ward", t => t.WardID)
                .Index(t => t.CommunityCouncilID, name: "IX_CommunityCouncil_CommunityCouncilID")
                .Index(t => t.CreatedUserID, name: "IX_CreatedUser_UserID")
                .Index(t => t.DistrictID, name: "IX_District_DistrictID")
                .Index(t => t.GuardianRelationID, name: "IX_GuardianRelation_RelationID")
                .Index(t => t.LastUpdatedUserID, name: "IX_LastUpdatedUser_UserID")
                .Index(t => t.OVCTypeID, name: "IX_OVCType_OVCTypeID")
                .Index(t => t.VillageID, name: "IX_Village_VillageID")
                .Index(t => t.WardID, name: "IX_Ward_WardID");           
        }
        
        public override void Down()
        {
        }
    }
}
