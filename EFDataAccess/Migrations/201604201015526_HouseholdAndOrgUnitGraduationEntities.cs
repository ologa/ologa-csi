namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class HouseholdAndOrgUnitGraduationEntities : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.GraduationCriteriaQuestions", newName: "QuestionGraduationCriterias");
            RenameColumn(table: "dbo.DomainCriteria", name: "GraduationCriteriaID", newName: "GradutionCriteriaID");
            RenameColumn(table: "dbo.QuestionCriteria", name: "GraduationCriteriaID", newName: "GradutionCriteriaID");
            RenameIndex(table: "dbo.DomainCriteria", name: "IX_GraduationCriteria_GraduationCriteriaID", newName: "IX_GradutionCriteria_GraduationCriteriaID");
            RenameIndex(table: "dbo.QuestionCriteria", name: "IX_GraduationCriteria_GraduationCriteriaID", newName: "IX_GradutionCriteria_GraduationCriteriaID");
            DropPrimaryKey("dbo.QuestionGraduationCriterias");
            CreateTable(
                "dbo.Adult",
                c => new
                    {
                        AdultId = c.Int(nullable: false, identity: true),
                        FirstName = c.String(),
                        LastName = c.String(),
                        isHouseHoldChef = c.Boolean(nullable: false),
                        Gender = c.String(nullable: false, maxLength: 1),
                        IDNumber = c.String(),
                        passport = c.Int(nullable: false),
                        HouseholdID = c.Int(),
                    })
                .PrimaryKey(t => t.AdultId)
                .ForeignKey("dbo.HouseHold", t => t.HouseholdID)
                .Index(t => t.HouseholdID, name: "IX_Household_HouseHoldID");
            
            CreateTable(
                "dbo.HouseHold",
                c => new
                    {
                        HouseHoldID = c.Int(nullable: false, identity: true),
                        OfficialHouseholdIdentifierNumber = c.Int(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        RegistrationDate = c.DateTime(nullable: false),
                        HouseholdUniqueIdentifier = c.Guid(nullable: false),
                        OrgUnitID = c.Int(),
                    })
                .PrimaryKey(t => t.HouseHoldID)
                .ForeignKey("dbo.OrgUnit", t => t.OrgUnitID)
                .Index(t => t.OrgUnitID, name: "IX_OrgUnit_OrgUnitID");
            
            CreateTable(
                "dbo.OrgUnit",
                c => new
                    {
                        OrgUnitID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                        ChiefName = c.String(maxLength: 50),
                        HouseCount = c.Int(nullable: false),
                        MemberCount = c.Int(nullable: false),
                        MaleCount = c.Int(nullable: false),
                        FemaleCount = c.Int(nullable: false),
                        AdultCount = c.Int(nullable: false),
                        ChildCount = c.Int(nullable: false),
                        Quota = c.Int(nullable: false),
                        QuotaSat = c.Int(nullable: false),
                        ParentOrgUnitId = c.Int(),
                        OrgUnitTypeID = c.Int(),
                    })
                .PrimaryKey(t => t.OrgUnitID)
                .ForeignKey("dbo.OrgUnit", t => t.ParentOrgUnitId)
                .ForeignKey("dbo.OrgUnitType", t => t.OrgUnitTypeID)
                .Index(t => t.ParentOrgUnitId)
                .Index(t => t.OrgUnitTypeID, name: "IX_OrgUnitType_OrgUnitTypeID");
            
            CreateTable(
                "dbo.OrgUnitType",
                c => new
                    {
                        OrgUnitTypeID = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false, maxLength: 50),
                        NounDescription = c.String(nullable: false, maxLength: 50),
                        ParentID = c.Int(),
                    })
                .PrimaryKey(t => t.OrgUnitTypeID)
                .ForeignKey("dbo.OrgUnitType", t => t.ParentID)
                .Index(t => t.ParentID, name: "IX_Parent_OrgUnitTypeID");
            
            AddColumn("dbo.Child", "HouseholdID", c => c.Int());
            AddPrimaryKey("dbo.QuestionGraduationCriterias", new[] { "QuestionID", "GraduationCriteriaID" });
            CreateIndex("dbo.Child", "HouseholdID", name: "IX_Household_HouseHoldID");
            AddForeignKey("dbo.Child", "HouseholdID", "dbo.HouseHold", "HouseHoldID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.OrgUnit", "OrgUnitTypeID", "dbo.OrgUnitType");
            DropForeignKey("dbo.OrgUnitType", "ParentID", "dbo.OrgUnitType");
            DropForeignKey("dbo.HouseHold", "OrgUnitID", "dbo.OrgUnit");
            DropForeignKey("dbo.OrgUnit", "ParentOrgUnitId", "dbo.OrgUnit");
            DropForeignKey("dbo.Child", "HouseholdID", "dbo.HouseHold");
            DropForeignKey("dbo.Adult", "HouseholdID", "dbo.HouseHold");
            DropIndex("dbo.OrgUnitType", "IX_Parent_OrgUnitTypeID");
            DropIndex("dbo.OrgUnit", "IX_OrgUnitType_OrgUnitTypeID");
            DropIndex("dbo.OrgUnit", new[] { "ParentOrgUnitId" });
            DropIndex("dbo.Child", "IX_Household_HouseHoldID");
            DropIndex("dbo.HouseHold", "IX_OrgUnit_OrgUnitID");
            DropIndex("dbo.Adult", "IX_Household_HouseHoldID");
            DropPrimaryKey("dbo.QuestionGraduationCriterias");
            DropColumn("dbo.Child", "HouseholdID");
            DropTable("dbo.OrgUnitType");
            DropTable("dbo.OrgUnit");
            DropTable("dbo.HouseHold");
            DropTable("dbo.Adult");
            AddPrimaryKey("dbo.QuestionGraduationCriterias", new[] { "GraduationCriteriaID", "QuestionID" });
            RenameIndex(table: "dbo.QuestionCriteria", name: "IX_GradutionCriteria_GraduationCriteriaID", newName: "IX_GraduationCriteria_GraduationCriteriaID");
            RenameIndex(table: "dbo.DomainCriteria", name: "IX_GradutionCriteria_GraduationCriteriaID", newName: "IX_GraduationCriteria_GraduationCriteriaID");
            RenameColumn(table: "dbo.QuestionCriteria", name: "GradutionCriteriaID", newName: "GraduationCriteriaID");
            RenameColumn(table: "dbo.DomainCriteria", name: "GradutionCriteriaID", newName: "GraduationCriteriaID");
            RenameTable(name: "dbo.QuestionGraduationCriterias", newName: "GraduationCriteriaQuestions");
        }
    }
}
