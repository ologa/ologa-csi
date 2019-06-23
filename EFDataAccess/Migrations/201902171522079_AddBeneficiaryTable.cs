namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class AddBeneficiaryTable : DbMigration
    {
        public override void Up()
        {
            Sql(@"DROP VIEW Beneficiary");

            DropIndex("dbo.CSI", new[] { "ChildID" });
            CreateTable(
                "dbo.Beneficiary",
                c => new
                {
                    BeneficiaryID = c.Int(nullable: false, identity: true),
                    Beneficiary_guid = c.Guid(nullable: false, defaultValueSql: "newid()"),
                    RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    CreatedUserID = c.Int(),
                    LastUpdatedUserID = c.Int(),
                    IDNumber = c.String(),
                    NID = c.String(maxLength: 50),
                    Code = c.String(),
                    FirstName = c.String(nullable: false, maxLength: 30),
                    LastName = c.String(nullable: false, maxLength: 30),
                    Gender = c.String(nullable: false, maxLength: 1),
                    IsHouseHoldChef = c.Boolean(nullable: false),
                    MaritalStatusID = c.Int(),
                    DateOfBirth = c.DateTime(nullable: false),
                    DateOfBirthUnknown = c.Boolean(nullable: false),
                    IsPartSavingGroup = c.Boolean(nullable: false),
                    RegistrationDate = c.DateTime(),
                    RegistrationDateDifferentFromHouseholdDate = c.Boolean(nullable: false),
                    HIVTracked = c.Boolean(nullable: false),
                    OVCTypeID = c.Int(),
                    KinshipToFamilyHeadID = c.Int(nullable: false),
                    OtherKinshipToFamilyHead = c.String(),
                    HIVStatusID = c.Int(nullable: false),
                    HouseholdID = c.Int(),
                    OrgUnitID = c.Int(),
                    ContactNo = c.String(maxLength: 20),
                    Guardian = c.String(maxLength: 50),
                    Notes = c.String(unicode: false, storeType: "text"),
                    Locked = c.Int(),
                    PrincipalChief = c.String(maxLength: 50),
                    VillageChief = c.String(maxLength: 50),
                    HIV = c.String(maxLength: 1),
                    HIVInTreatment = c.Int(),
                    HIVUndisclosedReason = c.Int(),
                    HIVDisclosed = c.String(maxLength: 1),
                    passport = c.Int(),
                    EducationBursary = c.String(maxLength: 1),
                    HighestSchoolLevelID = c.Int(),
                    EligibleFamilyReunification = c.String(maxLength: 1),
                    MaterialStationery = c.String(maxLength: 1),
                    Literate = c.String(maxLength: 1),
                    Numeracy = c.String(maxLength: 1),
                    MarketingStarter = c.String(maxLength: 1),
                    CapacityIGA = c.String(maxLength: 1),
                    EarnIGA = c.String(maxLength: 1),
                    SufficientMeals = c.String(maxLength: 1),
                    GuardianIdNo = c.String(maxLength: 50),
                    SchoolName = c.String(maxLength: 50),
                    SchoolContactNo = c.String(maxLength: 20),
                    DisabilityNotes = c.String(maxLength: 500),
                    PrincipalName = c.String(maxLength: 50),
                    PrincipalContactNo = c.String(maxLength: 20),
                    TeacherName = c.String(maxLength: 50),
                    TeacherContactNo = c.String(maxLength: 20),
                    HeadName = c.String(maxLength: 50),
                    HeadContactNo = c.String(maxLength: 20),
                    Address = c.String(maxLength: 100),
                    Grade = c.String(maxLength: 20),
                    Class = c.String(maxLength: 20),
                    ChildID = c.Int(),
                    AdultID = c.Int(),
                    State = c.Int(nullable: false),
                    CreatedDate = c.DateTime(),
                    LastUpdatedDate = c.DateTime(),
                    SyncState = c.Int(nullable: false),
                    SyncDate = c.DateTime(),
                    SyncGuid = c.Guid(),
                    CommunityCouncilID = c.Int(),
                    DistrictID = c.Int(),
                    GuardianRelationID = c.Int(),
                    LocationID = c.Int(),
                    VillageID = c.Int(),
                    WardID = c.Int(),
                })
                .PrimaryKey(t => t.BeneficiaryID)
                .ForeignKey("dbo.HouseHold", t => t.HouseholdID)
                .ForeignKey("dbo.CommunityCouncil", t => t.CommunityCouncilID)
                .ForeignKey("dbo.User", t => t.CreatedUserID)
                .ForeignKey("dbo.District", t => t.DistrictID)
                .ForeignKey("dbo.GuardianRelation", t => t.GuardianRelationID)
                .ForeignKey("dbo.HIVStatus", t => t.HIVStatusID, cascadeDelete: true)
                .ForeignKey("dbo.SimpleEntity", t => t.KinshipToFamilyHeadID)
                .ForeignKey("dbo.User", t => t.LastUpdatedUserID)
                .ForeignKey("dbo.Location", t => t.LocationID)
                .ForeignKey("dbo.OrgUnit", t => t.OrgUnitID)
                .ForeignKey("dbo.OVCType", t => t.OVCTypeID)
                .ForeignKey("dbo.Village", t => t.VillageID)
                .ForeignKey("dbo.Ward", t => t.WardID)
                .Index(t => t.CreatedUserID)
                .Index(t => t.LastUpdatedUserID)
                .Index(t => t.OVCTypeID)
                .Index(t => t.KinshipToFamilyHeadID)
                .Index(t => t.HIVStatusID)
                .Index(t => t.HouseholdID)
                .Index(t => t.OrgUnitID)
                .Index(t => t.CommunityCouncilID, name: "IX_CommunityCouncil_CommunityCouncilID")
                .Index(t => t.DistrictID, name: "IX_District_DistrictID")
                .Index(t => t.GuardianRelationID, name: "IX_GuardianRelation_RelationID")
                .Index(t => t.LocationID, name: "IX_Location_LocationID")
                .Index(t => t.VillageID, name: "IX_Village_VillageID")
                .Index(t => t.WardID, name: "IX_Ward_WardID");

            AddColumn("dbo.ChildStatusHistory", "BeneficiaryID", c => c.Int());
            AddColumn("dbo.ChildDisability", "BeneficiaryID", c => c.Int());
            AddColumn("dbo.ChildEvent", "BeneficiaryID", c => c.Int());
            AddColumn("dbo.ChildPartner", "BeneficiaryID", c => c.Int());
            AddColumn("dbo.CSI", "BeneficiaryID", c => c.Int());
            AddColumn("dbo.ChildProject", "BeneficiaryID", c => c.Int());
            AddColumn("dbo.ChildRegistration", "BeneficiaryID", c => c.Int());
            AddColumn("dbo.ChildTrainingEvent", "BeneficiaryID", c => c.Int());
            AddColumn("dbo.Encounter", "BeneficiaryID", c => c.Int());
            AddColumn("dbo.HIVStatus", "BeneficiaryID", c => c.Int(nullable: false));
            AddColumn("dbo.RoutineVisitMember", "BeneficiaryID", c => c.Int());
            AddColumn("dbo.ReferenceService", "BeneficiaryID", c => c.Int());
            AlterColumn("dbo.CSI", "ChildID", c => c.Int());
            CreateIndex("dbo.ChildStatusHistory", "BeneficiaryID");
            CreateIndex("dbo.ChildDisability", "BeneficiaryID", name: "IX_Beneficiary_BeneficiaryID");
            CreateIndex("dbo.ChildEvent", "BeneficiaryID", name: "IX_Beneficiary_BeneficiaryID");
            CreateIndex("dbo.ChildPartner", "BeneficiaryID", name: "IX_Beneficiary_BeneficiaryID");
            CreateIndex("dbo.CSI", "ChildID");
            CreateIndex("dbo.CSI", "BeneficiaryID");
            CreateIndex("dbo.ChildProject", "BeneficiaryID", name: "IX_Beneficiary_BeneficiaryID");
            CreateIndex("dbo.ChildRegistration", "BeneficiaryID", name: "IX_Beneficiary_BeneficiaryID");
            CreateIndex("dbo.ChildTrainingEvent", "BeneficiaryID", name: "IX_Beneficiary_BeneficiaryID");
            CreateIndex("dbo.Encounter", "BeneficiaryID", name: "IX_Beneficiary_BeneficiaryID");
            CreateIndex("dbo.RoutineVisitMember", "BeneficiaryID");
            CreateIndex("dbo.ReferenceService", "BeneficiaryID");
            AddForeignKey("dbo.CSI", "BeneficiaryID", "dbo.Beneficiary", "BeneficiaryID");
            AddForeignKey("dbo.RoutineVisitMember", "BeneficiaryID", "dbo.Beneficiary", "BeneficiaryID");
            AddForeignKey("dbo.ChildDisability", "BeneficiaryID", "dbo.Beneficiary", "BeneficiaryID");
            AddForeignKey("dbo.ChildEvent", "BeneficiaryID", "dbo.Beneficiary", "BeneficiaryID");
            AddForeignKey("dbo.ChildPartner", "BeneficiaryID", "dbo.Beneficiary", "BeneficiaryID");
            AddForeignKey("dbo.ChildProject", "BeneficiaryID", "dbo.Beneficiary", "BeneficiaryID");
            AddForeignKey("dbo.ChildRegistration", "BeneficiaryID", "dbo.Beneficiary", "BeneficiaryID");
            AddForeignKey("dbo.ChildStatusHistory", "BeneficiaryID", "dbo.Beneficiary", "BeneficiaryID");
            AddForeignKey("dbo.ChildTrainingEvent", "BeneficiaryID", "dbo.Beneficiary", "BeneficiaryID");
            AddForeignKey("dbo.Encounter", "BeneficiaryID", "dbo.Beneficiary", "BeneficiaryID");
            AddForeignKey("dbo.ReferenceService", "BeneficiaryID", "dbo.Beneficiary", "BeneficiaryID");

            // Copy Child to Beneficiary
            Sql(@"INSERT INTO Beneficiary 
                (ChildID, FirstName, LastName, Gender, DateOfBirth, DateOfBirthUnknown, CreatedDate, 
                 LastUpdatedDate, ContactNo, CreatedUserID, LastUpdatedUserID, OVCTypeID, HouseholdID,
                 OrgUnitID, NID, IsPartSavingGroup, KinshipToFamilyHeadID, OtherKinshipToFamilyHead, HIVStatusID,
                 SyncDate, SyncGuid, [State], SyncState, RegistrationDate, RegistrationDateDifferentFromHouseholdDate,
                 HIVTracked, Code, IsHouseHoldChef)
                SELECT ChildID, FirstName, LastName, Gender, DateOfBirth, DateOfBirthUnknown, CreatedDate, 
                 LastUpdatedDate, ContactNo, CreatedUserID, LastUpdatedUserID, OVCTypeID, HouseholdID,
                 OrgUnitID, NID, IsPartSavingGroup, KinshipToFamilyHeadID, OtherKinshipToFamilyHead, HIVStatusID,
                 SyncDate, SyncGuid, [State], SyncState, RegistrationDate, RegistrationDateDifferentFromHouseholdDate,
                 HIVTracked, Code, 0 As IsHouseHoldChef
                FROM Child
                WHERE ChildID not in (select a.ChildID from Adult a where a.ChildID is not null)");

            // Copy Adult to Beneficiary
            Sql(@"INSERT INTO Beneficiary 
                (AdultID, FirstName, LastName, Gender, DateOfBirth, DateOfBirthUnknown, CreatedDate, 
                 LastUpdatedDate, ContactNo, CreatedUserID, LastUpdatedUserID, HouseholdID,
                 NID, IsPartSavingGroup, KinshipToFamilyHeadID, OtherKinshipToFamilyHead, HIVStatusID,
                 SyncDate, SyncGuid, [State], SyncState, RegistrationDate, RegistrationDateDifferentFromHouseholdDate,
                 HIVTracked, Code, IsHouseHoldChef, IDNumber)
                SELECT AdultId, FirstName, LastName, Gender, DateOfBirth, DateOfBirthUnknown, CreatedDate, 
                 LastUpdatedDate, ContactNo, CreatedUserID, LastUpdatedUserID, HouseholdID,
                 NID, IsPartSavingGroup, KinshipToFamilyHeadID, OtherKinshipToFamilyHead, HIVStatusID,
                 SyncDate, SyncGuid, [State], SyncState, RegistrationDate, RegistrationDateDifferentFromHouseholdDate,
                 HIVTracked, Code, IsHouseHoldChef, IDNumber
                FROM Adult");

            // Set CSI for Beneficiary
            Sql(@"UPDATE csi SET csi.BeneficiaryID = b.BeneficiaryID
                FROM CSI csi INNER JOIN Beneficiary b ON csi.ChildID = b.ChildID");
            // Set ReferenceService for Beneficiary
            Sql(@"UPDATE r SET r.BeneficiaryID = b.BeneficiaryID
                FROM ReferenceService r INNER JOIN Beneficiary b ON r.ChildID = b.ChildID");
            Sql(@"UPDATE r SET r.BeneficiaryID = b.BeneficiaryID
                FROM ReferenceService r INNER JOIN Beneficiary b ON r.AdultID = b.AdultID");
            // Set HIVStatus for Beneficiary
            Sql(@"UPDATE h SET h.BeneficiaryID = b.BeneficiaryID, h.BeneficiaryGuid = b.Beneficiary_guid
                FROM HIVStatus h INNER JOIN Beneficiary b ON h.AdultID = b.AdultID");
            Sql(@"UPDATE h SET h.BeneficiaryID = b.BeneficiaryID, h.BeneficiaryGuid = b.Beneficiary_guid
                FROM HIVStatus h INNER JOIN Beneficiary b ON h.ChildID = b.ChildID");
            // Set ChildStatusHistory for Beneficiary
            Sql(@"UPDATE csh SET csh.BeneficiaryID = b.BeneficiaryID, csh.BeneficiaryGuid = b.Beneficiary_guid
                FROM ChildStatusHistory csh INNER JOIN Beneficiary b ON csh.AdultID = b.AdultID");
            Sql(@"UPDATE csh SET csh.BeneficiaryID = b.BeneficiaryID, csh.BeneficiaryGuid = b.Beneficiary_guid
                FROM ChildStatusHistory csh INNER JOIN Beneficiary b ON csh.ChildID = b.ChildID");
            // Set RoutineVisitMember for Beneficiary
            Sql(@"UPDATE rvm SET rvm.BeneficiaryID = b.BeneficiaryID
                FROM RoutineVisitMember rvm INNER JOIN Beneficiary b ON rvm.AdultID = b.AdultID");
            Sql(@"UPDATE rvm SET rvm.BeneficiaryID = b.BeneficiaryID
                FROM RoutineVisitMember rvm INNER JOIN Beneficiary b ON rvm.ChildID = b.ChildID");

        }

        public override void Down()
        {
            DropForeignKey("dbo.ReferenceService", "BeneficiaryID", "dbo.Beneficiary");
            DropForeignKey("dbo.Beneficiary", "WardID", "dbo.Ward");
            DropForeignKey("dbo.Beneficiary", "VillageID", "dbo.Village");
            DropForeignKey("dbo.Beneficiary", "OVCTypeID", "dbo.OVCType");
            DropForeignKey("dbo.Beneficiary", "OrgUnitID", "dbo.OrgUnit");
            DropForeignKey("dbo.Beneficiary", "LocationID", "dbo.Location");
            DropForeignKey("dbo.Beneficiary", "LastUpdatedUserID", "dbo.User");
            DropForeignKey("dbo.Beneficiary", "KinshipToFamilyHeadID", "dbo.SimpleEntity");
            DropForeignKey("dbo.Beneficiary", "HIVStatusID", "dbo.HIVStatus");
            DropForeignKey("dbo.Beneficiary", "GuardianRelationID", "dbo.GuardianRelation");
            DropForeignKey("dbo.Encounter", "BeneficiaryID", "dbo.Beneficiary");
            DropForeignKey("dbo.Beneficiary", "DistrictID", "dbo.District");
            DropForeignKey("dbo.Beneficiary", "CreatedUserID", "dbo.User");
            DropForeignKey("dbo.Beneficiary", "CommunityCouncilID", "dbo.CommunityCouncil");
            DropForeignKey("dbo.ChildTrainingEvent", "BeneficiaryID", "dbo.Beneficiary");
            DropForeignKey("dbo.ChildStatusHistory", "BeneficiaryID", "dbo.Beneficiary");
            DropForeignKey("dbo.ChildRegistration", "BeneficiaryID", "dbo.Beneficiary");
            DropForeignKey("dbo.ChildProject", "BeneficiaryID", "dbo.Beneficiary");
            DropForeignKey("dbo.ChildPartner", "BeneficiaryID", "dbo.Beneficiary");
            DropForeignKey("dbo.ChildEvent", "BeneficiaryID", "dbo.Beneficiary");
            DropForeignKey("dbo.ChildDisability", "BeneficiaryID", "dbo.Beneficiary");
            DropForeignKey("dbo.RoutineVisitMember", "BeneficiaryID", "dbo.Beneficiary");
            DropForeignKey("dbo.Beneficiary", "HouseholdID", "dbo.HouseHold");
            DropForeignKey("dbo.CSI", "BeneficiaryID", "dbo.Beneficiary");
            DropIndex("dbo.ReferenceService", new[] { "BeneficiaryID" });
            DropIndex("dbo.RoutineVisitMember", new[] { "BeneficiaryID" });
            DropIndex("dbo.Encounter", "IX_Beneficiary_BeneficiaryID");
            DropIndex("dbo.ChildTrainingEvent", "IX_Beneficiary_BeneficiaryID");
            DropIndex("dbo.ChildRegistration", "IX_Beneficiary_BeneficiaryID");
            DropIndex("dbo.ChildProject", "IX_Beneficiary_BeneficiaryID");
            DropIndex("dbo.CSI", new[] { "BeneficiaryID" });
            DropIndex("dbo.CSI", new[] { "ChildID" });
            DropIndex("dbo.ChildPartner", "IX_Beneficiary_BeneficiaryID");
            DropIndex("dbo.ChildEvent", "IX_Beneficiary_BeneficiaryID");
            DropIndex("dbo.ChildDisability", "IX_Beneficiary_BeneficiaryID");
            DropIndex("dbo.Beneficiary", "IX_Ward_WardID");
            DropIndex("dbo.Beneficiary", "IX_Village_VillageID");
            DropIndex("dbo.Beneficiary", "IX_Location_LocationID");
            DropIndex("dbo.Beneficiary", "IX_GuardianRelation_RelationID");
            DropIndex("dbo.Beneficiary", "IX_District_DistrictID");
            DropIndex("dbo.Beneficiary", "IX_CommunityCouncil_CommunityCouncilID");
            DropIndex("dbo.Beneficiary", new[] { "OrgUnitID" });
            DropIndex("dbo.Beneficiary", new[] { "HouseholdID" });
            DropIndex("dbo.Beneficiary", new[] { "HIVStatusID" });
            DropIndex("dbo.Beneficiary", new[] { "KinshipToFamilyHeadID" });
            DropIndex("dbo.Beneficiary", new[] { "OVCTypeID" });
            DropIndex("dbo.Beneficiary", new[] { "LastUpdatedUserID" });
            DropIndex("dbo.Beneficiary", new[] { "CreatedUserID" });
            DropIndex("dbo.ChildStatusHistory", new[] { "BeneficiaryID" });
            //AlterColumn("dbo.CSI", "ChildID", c => c.Int(nullable: false));
            DropColumn("dbo.ReferenceService", "BeneficiaryID");
            DropColumn("dbo.RoutineVisitMember", "BeneficiaryID");
            DropColumn("dbo.HIVStatus", "BeneficiaryID");
            DropColumn("dbo.Encounter", "BeneficiaryID");
            DropColumn("dbo.ChildTrainingEvent", "BeneficiaryID");
            DropColumn("dbo.ChildRegistration", "BeneficiaryID");
            DropColumn("dbo.ChildProject", "BeneficiaryID");
            DropColumn("dbo.CSI", "BeneficiaryID");
            DropColumn("dbo.ChildPartner", "BeneficiaryID");
            DropColumn("dbo.ChildEvent", "BeneficiaryID");
            DropColumn("dbo.ChildDisability", "BeneficiaryID");
            DropColumn("dbo.ChildStatusHistory", "BeneficiaryID");
            DropTable("dbo.Beneficiary");
            //CreateIndex("dbo.CSI", "ChildID");

            Sql(@"CREATE VIEW Beneficiary as 
				select * from 
				( 
				select 'adult' as Type, a.AdultID as ID, a.AdultGuid As Guid, a.FirstName, a.LastName, a.DateOfBirth, a.Gender,
				DATEDIFF(year, CAST(a.DateOfBirth As Date), GETDATE()) As Age, cs.Description As BeneficiaryState, csh.EffectiveDate As BeneficiaryStateEffectiveDate,
				a.HouseholdID, KinshipToFamilyHeadID, a.HIVStatusID, hs.HIVStatusID As LastHIVStatusID, HIVTracked, 0 As OVCTypeID, a.IsPartSavingGroup,
				CASE WHEN a.RegistrationDate IS NULL THEN h.RegistrationDate ELSE a.RegistrationDate END As RegistrationDate
				from Adult a					
				inner join  [ChildStatusHistory] csh on (csh.ChildStatusHistoryID = (SELECT max(csh2.ChildStatusHistoryID) FROM  [ChildStatusHistory] csh2 WHERE csh2.AdultId = a.AdultId))
				inner join  [ChildStatus] cs on (cs.StatusID = csh.ChildStatusID)
				inner join  [Household] h on (a.HouseholdID = h.HouseHoldID)
				inner join  [HIVStatus] hs ON (hs.HIVStatusID = (SELECT max(hs2.HIVStatusID) FROM  [HIVStatus] hs2 WHERE (hs2.AdultID = a.AdultId)))
				--
				union all 
				--
				select 'child' as Type, c.ChildID as ID, c.child_guid As Guid, c.FirstName, c.LastName, c.DateOfBirth, c.Gender, 
				DATEDIFF(year, CAST(c.DateOfBirth As Date), GETDATE()) As Age, cs.Description As BeneficiaryState, csh.EffectiveDate As BeneficiaryStateEffectiveDate,
				c.HouseholdID, c.KinshipToFamilyHeadID, c.HIVStatusID, hs.HIVStatusID As LastHIVStatusID, c. HIVTracked, c.OVCTypeID, 0 As IsPartSavingGroup,
				CASE WHEN c.RegistrationDate IS NULL THEN h.RegistrationDate ELSE c.RegistrationDate END As RegistrationDate
				from Child c
				inner join  [ChildStatusHistory] csh on (csh.ChildStatusHistoryID = (SELECT max(csh2.ChildStatusHistoryID) FROM  [ChildStatusHistory] csh2 WHERE csh2.ChildID = c.ChildID))
				inner join  [ChildStatus] cs on (cs.StatusID = csh.ChildStatusID)
				inner join  [Household] h on (c.HouseholdID = h.HouseHoldID)
				inner join  [HIVStatus] hs ON (hs.HIVStatusID = (SELECT max(hs2.HIVStatusID) FROM  [HIVStatus] hs2 WHERE (hs2.ChildID = c.CHildID)))
				) b");
        }
    }
}
