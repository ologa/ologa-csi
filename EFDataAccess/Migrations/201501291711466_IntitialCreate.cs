namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IntitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Action",
                c => new
                    {
                        ActionID = c.Int(nullable: false, identity: true),
                        Code = c.String(nullable: false, maxLength: 10),
                        Name = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.ActionID);
            
            CreateTable(
                "dbo.Answer",
                c => new
                    {
                        AnswerID = c.Int(nullable: false, identity: true),
                        Description = c.String(maxLength: 30),
                        QuestionID = c.Int(),
                        ScoreID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.AnswerID)
                .ForeignKey("dbo.Question", t => t.QuestionID)
                .ForeignKey("dbo.ScoreType", t => t.ScoreID)
                .Index(t => t.QuestionID, name: "IX_Question_QuestionID")
                .Index(t => t.ScoreID, name: "IX_Score_ScoreTypeID");
            
            CreateTable(
                "dbo.Question",
                c => new
                    {
                        QuestionID = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false, maxLength: 100, unicode: false),
                        Code = c.String(nullable: false, maxLength: 20),
                        Goal = c.String(nullable: false, maxLength: 100),
                        question_guid = c.Guid(nullable: false, defaultValueSql: "newid()"),
                        DomainID = c.Int(),
                    })
                .PrimaryKey(t => t.QuestionID)
                .ForeignKey("dbo.Domain", t => t.DomainID)
                .Index(t => t.DomainID, name: "IX_Domain_DomainID");
            
            CreateTable(
                "dbo.Domain",
                c => new
                    {
                        DomainID = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false, maxLength: 30, unicode: false),
                        DomainCode = c.String(maxLength: 5),
                        DomainColor = c.String(maxLength: 30),
                        Guidelines = c.String(unicode: false),
                        domain_guid = c.Guid(nullable: false, defaultValueSql: "newid()"),
                    })
                .PrimaryKey(t => t.DomainID);
            
            CreateTable(
                "dbo.ScoreType",
                c => new
                    {
                        ScoreTypeID = c.Int(nullable: false, identity: true),
                        Score = c.Int(nullable: false),
                        Rating = c.String(maxLength: 30),
                        Color = c.String(maxLength: 30),
                    })
                .PrimaryKey(t => t.ScoreTypeID);
            
            CreateTable(
                "dbo.CareGiverRelation",
                c => new
                    {
                        RelationID = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false, maxLength: 30, unicode: false),
                        caregiverrelation_guid = c.Guid(nullable: false, defaultValueSql: "newid()"),
                    })
                .PrimaryKey(t => t.RelationID);
            
            CreateTable(
                "dbo.CarePlanDomain",
                c => new
                    {
                        CarePlanDomainID = c.Int(nullable: false, identity: true),
                        Actions = c.String(unicode: false),
                        careplandomain_guid = c.Guid(nullable: false, defaultValueSql: "newid()"),
                        FamilyReunificationAction = c.String(maxLength: 1, fixedLength: true, unicode: false),
                        ActionCompleted = c.String(maxLength: 1, fixedLength: true, unicode: false),
                        ActionCompletedDate = c.DateTime(storeType: "date"),
                        ActionCompletedComments = c.String(unicode: false),
                        ResponsibleUser = c.String(maxLength: 100, unicode: false),
                        CarePlanID = c.Int(),
                        DomainID = c.Int(),
                    })
                .PrimaryKey(t => t.CarePlanDomainID)
                .ForeignKey("dbo.CarePlan", t => t.CarePlanID)
                .ForeignKey("dbo.Domain", t => t.DomainID)
                .Index(t => t.CarePlanID, name: "IX_CarePlan_CarePlanID")
                .Index(t => t.DomainID, name: "IX_Domain_DomainID");
            
            CreateTable(
                "dbo.CarePlan",
                c => new
                    {
                        CarePlanID = c.Int(nullable: false, identity: true),
                        CarePlanDate = c.DateTime(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        LastUpdatedDate = c.DateTime(nullable: false),
                        careplan_guid = c.Guid(nullable: false, defaultValueSql: "newid()"),
                        CreatedUserID = c.Int(),
                        CSIID = c.Int(nullable: false),
                        LastUpdatedUserID = c.Int(),
                    })
                .PrimaryKey(t => t.CarePlanID)
                .ForeignKey("dbo.User", t => t.CreatedUserID)
                .ForeignKey("dbo.CSI", t => t.CSIID)
                .ForeignKey("dbo.User", t => t.LastUpdatedUserID)
                .Index(t => t.CreatedUserID, name: "IX_CreatedUser_UserID")
                .Index(t => t.CSIID, name: "IX_CSI_CSIID")
                .Index(t => t.LastUpdatedUserID, name: "IX_LastUpdatedUser_UserID");
            
            CreateTable(
                "dbo.User",
                c => new
                    {
                        UserID = c.Int(nullable: false, identity: true),
                        Username = c.String(nullable: false, maxLength: 10),
                        Password = c.Binary(nullable: false),
                        FirstName = c.String(nullable: false, maxLength: 20),
                        LastName = c.String(nullable: false, maxLength: 30),
                        Admin = c.Boolean(nullable: false),
                        DefSite = c.Int(nullable: false),
                        LoggedOn = c.Boolean(nullable: false),
                        DefaultLanguageID = c.Int(),
                        PartnerID = c.Int(),
                    })
                .PrimaryKey(t => t.UserID)
                .ForeignKey("dbo.Language", t => t.DefaultLanguageID)
                .ForeignKey("dbo.Partner", t => t.PartnerID)
                .Index(t => t.DefaultLanguageID, name: "IX_DefaultLanguage_LanguageID")
                .Index(t => t.PartnerID, name: "IX_Partner_PartnerID");
            
            CreateTable(
                "dbo.Language",
                c => new
                    {
                        LanguageID = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false, maxLength: 30, unicode: false),
                        language_guid = c.Guid(nullable: false, defaultValueSql: "newid()"),
                    })
                .PrimaryKey(t => t.LanguageID);
            
            CreateTable(
                "dbo.Partner",
                c => new
                    {
                        PartnerID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50, unicode: false),
                        Address = c.String(unicode: false),
                        ContactNo = c.String(maxLength: 30, unicode: false),
                        FaxNo = c.String(maxLength: 30, unicode: false),
                        ContactName = c.String(maxLength: 60, unicode: false),
                        partner_guid = c.Guid(nullable: false, defaultValueSql: "newid()"),
                    })
                .PrimaryKey(t => t.PartnerID);
            
            CreateTable(
                "dbo.CSI",
                c => new
                    {
                        CSIID = c.Int(nullable: false, identity: true),
                        IndexDate = c.DateTime(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        LastUpdatedDate = c.DateTime(nullable: false),
                        StatusID = c.Int(nullable: false),
                        Photo = c.Binary(storeType: "image"),
                        Height = c.Decimal(precision: 10, scale: 2),
                        Weight = c.Decimal(precision: 10, scale: 2),
                        BMI = c.Decimal(precision: 10, scale: 2),
                        TakingMedication = c.Boolean(nullable: false),
                        MedicationDescription = c.String(unicode: false),
                        Suggestions = c.String(unicode: false),
                        Caregiver = c.String(maxLength: 50, unicode: false),
                        csi_guid = c.Guid(nullable: false, defaultValueSql: "newid()"),
                        OutreachBenefit = c.String(maxLength: 1, fixedLength: true, unicode: false),
                        SocialWorkerName = c.String(maxLength: 100, unicode: false),
                        SocialWorkerContactNo = c.String(maxLength: 20, unicode: false),
                        DoctorName = c.String(maxLength: 100, unicode: false),
                        DoctorContactNo = c.String(maxLength: 20, unicode: false),
                        AllergyNotes = c.String(maxLength: 500, unicode: false),
                        CareGiverRelationID = c.Int(),
                        ChildID = c.Int(nullable: false),
                        CreatedUserID = c.Int(),
                        DistrictID = c.Int(),
                        LastUpdatedUserID = c.Int(),
                    })
                .PrimaryKey(t => t.CSIID)
                .ForeignKey("dbo.CareGiverRelation", t => t.CareGiverRelationID)
                .ForeignKey("dbo.Child", t => t.ChildID)
                .ForeignKey("dbo.User", t => t.CreatedUserID)
                .ForeignKey("dbo.District", t => t.DistrictID)
                .ForeignKey("dbo.User", t => t.LastUpdatedUserID)
                .Index(t => t.CareGiverRelationID, name: "IX_CareGiverRelation_RelationID")
                .Index(t => t.ChildID, name: "IX_Child_ChildID")
                .Index(t => t.CreatedUserID, name: "IX_CreatedUser_UserID")
                .Index(t => t.DistrictID, name: "IX_District_DistrictID")
                .Index(t => t.LastUpdatedUserID, name: "IX_LastUpdatedUser_UserID");
            
            CreateTable(
                "dbo.Child",
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
            
            CreateTable(
                "dbo.ChildDisability",
                c => new
                    {
                        ChildDisabilityID = c.Int(nullable: false, identity: true),
                        childdisability_guid = c.Guid(nullable: false, defaultValueSql: "newid()"),
                        DisabilityID = c.Int(),
                        ChildID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ChildDisabilityID)
                .ForeignKey("dbo.Disability", t => t.DisabilityID)
                .ForeignKey("dbo.Child", t => t.ChildID)
                .Index(t => t.DisabilityID, name: "IX_Disability_DisabilityID")
                .Index(t => t.ChildID, name: "IX_Child_ChildID");
            
            CreateTable(
                "dbo.Disability",
                c => new
                    {
                        DisabilityID = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false, maxLength: 30, unicode: false),
                        disability_guid = c.Guid(nullable: false, defaultValueSql: "newid()"),
                    })
                .PrimaryKey(t => t.DisabilityID);
            
            CreateTable(
                "dbo.ChildEvent",
                c => new
                    {
                        ChildEventID = c.Int(nullable: false, identity: true),
                        Comments = c.String(maxLength: 1, fixedLength: true, unicode: false),
                        EffectiveDate = c.DateTime(storeType: "date"),
                        childevent_guid = c.Guid(nullable: false, defaultValueSql: "newid()"),
                        EventID = c.Int(),
                        ChildID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ChildEventID)
                .ForeignKey("dbo.Event", t => t.EventID)
                .ForeignKey("dbo.Child", t => t.ChildID)
                .Index(t => t.EventID, name: "IX_Event_EventID")
                .Index(t => t.ChildID, name: "IX_Child_ChildID");
            
            CreateTable(
                "dbo.Event",
                c => new
                    {
                        EventID = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false, maxLength: 50, unicode: false),
                        event_guid = c.Guid(nullable: false, defaultValueSql: "newid()"),
                    })
                .PrimaryKey(t => t.EventID);
            
            CreateTable(
                "dbo.ChildPartner",
                c => new
                    {
                        ChildPartnerID = c.Int(nullable: false, identity: true),
                        EffectiveDate = c.DateTime(nullable: false),
                        Active = c.Boolean(nullable: false),
                        childpartner_guid = c.Guid(nullable: false, defaultValueSql: "newid()"),
                        PartnerID = c.Int(),
                        ChildID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ChildPartnerID)
                .ForeignKey("dbo.Partner", t => t.PartnerID)
                .ForeignKey("dbo.Child", t => t.ChildID)
                .Index(t => t.PartnerID, name: "IX_Partner_PartnerID")
                .Index(t => t.ChildID, name: "IX_Child_ChildID");
            
            CreateTable(
                "dbo.ChildProject",
                c => new
                    {
                        ChildProjectID = c.Int(nullable: false, identity: true),
                        RegisteredDate = c.DateTime(nullable: false, storeType: "date"),
                        childproject_guid = c.Guid(nullable: false, defaultValueSql: "newid()"),
                        ProjectID = c.Int(),
                        ChildID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ChildProjectID)
                .ForeignKey("dbo.Project", t => t.ProjectID)
                .ForeignKey("dbo.Child", t => t.ChildID)
                .Index(t => t.ProjectID, name: "IX_Project_ProjectID")
                .Index(t => t.ChildID, name: "IX_Child_ChildID");
            
            CreateTable(
                "dbo.Project",
                c => new
                    {
                        ProjectID = c.Int(nullable: false, identity: true),
                        ProjectName = c.String(nullable: false, maxLength: 30, unicode: false),
                        project_guid = c.Guid(nullable: false, defaultValueSql: "newid()"),
                    })
                .PrimaryKey(t => t.ProjectID);
            
            CreateTable(
                "dbo.ChildRegistration",
                c => new
                    {
                        ChildRegistrationID = c.Int(nullable: false, identity: true),
                        Detail = c.String(maxLength: 50, unicode: false),
                        RegisteredDate = c.DateTime(storeType: "date"),
                        childregistration_guid = c.Guid(nullable: false, defaultValueSql: "newid()"),
                        RegistrationTypeID = c.Int(),
                        ChildID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ChildRegistrationID)
                .ForeignKey("dbo.RegistrationType", t => t.RegistrationTypeID)
                .ForeignKey("dbo.Child", t => t.ChildID)
                .Index(t => t.RegistrationTypeID, name: "IX_RegistrationType_RegistrationTypeID")
                .Index(t => t.ChildID, name: "IX_Child_ChildID");
            
            CreateTable(
                "dbo.RegistrationType",
                c => new
                    {
                        RegistrationTypeID = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false, maxLength: 50, unicode: false),
                        registrationtype_guid = c.Guid(nullable: false, defaultValueSql: "newid()"),
                    })
                .PrimaryKey(t => t.RegistrationTypeID);
            
            CreateTable(
                "dbo.ChildStatusHistory",
                c => new
                    {
                        ChildStatusHistoryID = c.Int(nullable: false, identity: true),
                        EffectiveDate = c.DateTime(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        childstatushistory_guid = c.Guid(nullable: false, defaultValueSql: "newid()"),
                        ChildStatusID = c.Int(),
                        CreatedUserID = c.Int(),
                        ChildID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ChildStatusHistoryID)
                .ForeignKey("dbo.ChildStatus", t => t.ChildStatusID)
                .ForeignKey("dbo.User", t => t.CreatedUserID)
                .ForeignKey("dbo.Child", t => t.ChildID)
                .Index(t => t.ChildStatusID, name: "IX_ChildStatus_StatusID")
                .Index(t => t.CreatedUserID, name: "IX_CreatedUser_UserID")
                .Index(t => t.ChildID, name: "IX_Child_ChildID");
            
            CreateTable(
                "dbo.ChildStatus",
                c => new
                    {
                        StatusID = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false, maxLength: 30, unicode: false),
                        childstatus_guid = c.Guid(nullable: false, defaultValueSql: "newid()"),
                    })
                .PrimaryKey(t => t.StatusID);
            
            CreateTable(
                "dbo.ChildTrainingEvent",
                c => new
                    {
                        ChildTrainingEventID = c.Int(nullable: false, identity: true),
                        Registered = c.String(nullable: false, maxLength: 1, fixedLength: true, unicode: false),
                        Participated = c.String(maxLength: 1, fixedLength: true, unicode: false),
                        Completed = c.String(maxLength: 1, fixedLength: true, unicode: false),
                        childtrainingevent_guid = c.Guid(nullable: false, defaultValueSql: "newid()"),
                        TrainingEventID = c.Int(),
                        ChildID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ChildTrainingEventID)
                .ForeignKey("dbo.TrainingEvent", t => t.TrainingEventID)
                .ForeignKey("dbo.Child", t => t.ChildID)
                .Index(t => t.TrainingEventID, name: "IX_TrainingEvent_TrainingEventID")
                .Index(t => t.ChildID, name: "IX_Child_ChildID");
            
            CreateTable(
                "dbo.ChildTrainingEventItem",
                c => new
                    {
                        ChildTrainingEventItemID = c.Int(nullable: false, identity: true),
                        Proficient = c.String(nullable: false, maxLength: 1, fixedLength: true, unicode: false),
                        childtrainingeventitem_guid = c.Guid(nullable: false, defaultValueSql: "newid()"),
                        TrainingItemID = c.Int(),
                        ChildTrainingEventID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ChildTrainingEventItemID)
                .ForeignKey("dbo.TrainingItem", t => t.TrainingItemID)
                .ForeignKey("dbo.ChildTrainingEvent", t => t.ChildTrainingEventID)
                .Index(t => t.TrainingItemID, name: "IX_TrainingItem_TrainingItemID")
                .Index(t => t.ChildTrainingEventID, name: "IX_ChildTrainingEvent_ChildTrainingEventID");
            
            CreateTable(
                "dbo.TrainingItem",
                c => new
                    {
                        TrainingItemID = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false, maxLength: 50, unicode: false),
                        trainingitem_guid = c.Guid(nullable: false, defaultValueSql: "newid()"),
                        TrainingID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.TrainingItemID)
                .ForeignKey("dbo.Training", t => t.TrainingID)
                .Index(t => t.TrainingID, name: "IX_Training_TrainingID");
            
            CreateTable(
                "dbo.Training",
                c => new
                    {
                        TrainingID = c.Int(nullable: false, identity: true),
                        ProjectID = c.Int(nullable: false),
                        Description = c.String(nullable: false, maxLength: 50, unicode: false),
                        training_guid = c.Guid(nullable: false, defaultValueSql: "newid()"),
                    })
                .PrimaryKey(t => t.TrainingID)
                .ForeignKey("dbo.Project", t => t.ProjectID, cascadeDelete: true)
                .Index(t => t.ProjectID);
            
            CreateTable(
                "dbo.TrainingEvent",
                c => new
                    {
                        TrainingEventID = c.Int(nullable: false, identity: true),
                        TrainingDate = c.DateTime(nullable: false, storeType: "date"),
                        trainingevent_guid = c.Guid(nullable: false, defaultValueSql: "newid()"),
                        PartnerProjectID = c.Int(nullable: false),
                        TrainingID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.TrainingEventID)
                .ForeignKey("dbo.PartnerProject", t => t.PartnerProjectID)
                .ForeignKey("dbo.Training", t => t.TrainingID)
                .Index(t => t.PartnerProjectID, name: "IX_PartnerProject_PartnerProjectID")
                .Index(t => t.TrainingID, name: "IX_Training_TrainingID");
            
            CreateTable(
                "dbo.PartnerProject",
                c => new
                    {
                        PartnerProjectID = c.Int(nullable: false, identity: true),
                        PartnerID = c.Int(),
                        ProjectID = c.Int(),
                    })
                .PrimaryKey(t => t.PartnerProjectID)
                .ForeignKey("dbo.Partner", t => t.PartnerID)
                .ForeignKey("dbo.Project", t => t.ProjectID)
                .Index(t => t.PartnerID, name: "IX_Partner_PartnerID")
                .Index(t => t.ProjectID, name: "IX_Project_ProjectID");
            
            CreateTable(
                "dbo.PartnerProjectSnapshot",
                c => new
                    {
                        PartnerProjectSnapshotID = c.Int(nullable: false, identity: true),
                        CreatedDate = c.DateTime(nullable: false, storeType: "date"),
                        PartnerProjectID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.PartnerProjectSnapshotID)
                .ForeignKey("dbo.PartnerProject", t => t.PartnerProjectID)
                .Index(t => t.PartnerProjectID, name: "IX_PartnerProject_PartnerProjectID");
            
            CreateTable(
                "dbo.Snapshot",
                c => new
                    {
                        SnapshotID = c.Int(nullable: false, identity: true),
                        Value = c.String(nullable: false, maxLength: 20, unicode: false),
                        TrackerItemID = c.Int(nullable: false),
                        PartnerProjectSnapshotID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.SnapshotID)
                .ForeignKey("dbo.TrackerItem", t => t.TrackerItemID)
                .ForeignKey("dbo.PartnerProjectSnapshot", t => t.PartnerProjectSnapshotID)
                .Index(t => t.TrackerItemID, name: "IX_TrackerItem_TrackerItemID")
                .Index(t => t.PartnerProjectSnapshotID, name: "IX_PartnerProjectSnapshot_PartnerProjectSnapshotID");
            
            CreateTable(
                "dbo.TrackerItem",
                c => new
                    {
                        TrackerItemID = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false, maxLength: 100, unicode: false),
                    })
                .PrimaryKey(t => t.TrackerItemID);
            
            CreateTable(
                "dbo.ProjectTrackerItem",
                c => new
                    {
                        ProjectTrackerItemID = c.Int(nullable: false, identity: true),
                        Numerator = c.String(nullable: false, unicode: false),
                        Denominator = c.String(nullable: false, unicode: false),
                        List = c.String(nullable: false, unicode: false),
                        TimeBased = c.Boolean(nullable: false),
                        ProjectID = c.Int(),
                        TrackerItemID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ProjectTrackerItemID)
                .ForeignKey("dbo.Project", t => t.ProjectID)
                .ForeignKey("dbo.TrackerItem", t => t.TrackerItemID)
                .Index(t => t.ProjectID, name: "IX_Project_ProjectID")
                .Index(t => t.TrackerItemID, name: "IX_TrackerItem_TrackerItemID");
            
            CreateTable(
                "dbo.ProjectTrackerItemValue",
                c => new
                    {
                        ProjectTrackerItemValueID = c.Int(nullable: false, identity: true),
                        RepCurrent = c.Boolean(nullable: false),
                        RepYear = c.Int(nullable: false),
                        RepMonth = c.Int(nullable: false),
                        Value = c.Decimal(nullable: false, precision: 6, scale: 2),
                        Denominator = c.Decimal(precision: 6, scale: 2),
                        Numerator = c.Decimal(precision: 6, scale: 2),
                        ProjectTrackerItemID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ProjectTrackerItemValueID)
                .ForeignKey("dbo.ProjectTrackerItem", t => t.ProjectTrackerItemID)
                .Index(t => t.ProjectTrackerItemID, name: "IX_ProjectTrackerItem_ProjectTrackerItemID");
            
            CreateTable(
                "dbo.SiteProgress",
                c => new
                    {
                        SiteProgressID = c.Int(nullable: false, identity: true),
                        FormDate = c.DateTime(nullable: false, storeType: "date"),
                        PreparedBy = c.String(maxLength: 50, unicode: false),
                        ProjectSchedule = c.String(unicode: false),
                        Resources = c.String(unicode: false),
                        Comments = c.String(unicode: false),
                        siteprogress_guid = c.Guid(nullable: false, defaultValueSql: "newid()"),
                        PartnerProjectID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.SiteProgressID)
                .ForeignKey("dbo.PartnerProject", t => t.PartnerProjectID)
                .Index(t => t.PartnerProjectID, name: "IX_PartnerProject_PartnerProjectID");
            
            CreateTable(
                "dbo.SiteProgressItem",
                c => new
                    {
                        SiteProgressItemID = c.Int(nullable: false, identity: true),
                        Detail = c.String(maxLength: 1000, unicode: false),
                        siteprogressitem_guid = c.Guid(nullable: false, defaultValueSql: "newid()"),
                        SiteProgressItemTypeID = c.Int(),
                        SiteProgressID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.SiteProgressItemID)
                .ForeignKey("dbo.SiteProgressItemType", t => t.SiteProgressItemTypeID)
                .ForeignKey("dbo.SiteProgress", t => t.SiteProgressID)
                .Index(t => t.SiteProgressItemTypeID, name: "IX_SiteProgressItemType_SiteProgressItemTypeID")
                .Index(t => t.SiteProgressID, name: "IX_SiteProgress_SiteProgressID");
            
            CreateTable(
                "dbo.SiteProgressItemType",
                c => new
                    {
                        SiteProgressItemTypeID = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false, maxLength: 50, unicode: false),
                        siteprogressitemtype_guid = c.Guid(nullable: false, defaultValueSql: "newid()"),
                    })
                .PrimaryKey(t => t.SiteProgressItemTypeID);
            
            CreateTable(
                "dbo.CommunityCouncil",
                c => new
                    {
                        CommunityCouncilID = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false, maxLength: 30, unicode: false),
                        communitycouncil_guid = c.Guid(nullable: false, defaultValueSql: "newid()"),
                    })
                .PrimaryKey(t => t.CommunityCouncilID);
            
            CreateTable(
                "dbo.District",
                c => new
                    {
                        DistrictID = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false, maxLength: 50, unicode: false),
                        district_guid = c.Guid(nullable: false, defaultValueSql: "newid()"),
                    })
                .PrimaryKey(t => t.DistrictID);
            
            CreateTable(
                "dbo.Village",
                c => new
                    {
                        VillageID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50, unicode: false),
                        village_guid = c.Guid(nullable: false, defaultValueSql: "newid()"),
                        DistrictID = c.Int(),
                    })
                .PrimaryKey(t => t.VillageID)
                .ForeignKey("dbo.District", t => t.DistrictID)
                .Index(t => t.DistrictID, name: "IX_District_DistrictID");
            
            CreateTable(
                "dbo.Encounter",
                c => new
                    {
                        EncounterID = c.Int(nullable: false, identity: true),
                        EncounterDate = c.DateTime(nullable: false, storeType: "date"),
                        Height = c.Decimal(precision: 10, scale: 2),
                        Weight = c.Decimal(precision: 10, scale: 2),
                        BMI = c.Decimal(precision: 10, scale: 2),
                        CD4Known = c.String(maxLength: 1, fixedLength: true, unicode: false),
                        CD4 = c.Int(),
                        CD4Date = c.DateTime(storeType: "date"),
                        CD4Requested = c.String(maxLength: 1, fixedLength: true, unicode: false),
                        CD4OrderNo = c.String(maxLength: 20, unicode: false),
                        AttendCaregiverDay = c.String(maxLength: 1, fixedLength: true, unicode: false),
                        CaregiverDayDate = c.DateTime(storeType: "date"),
                        FeelFamilySupported = c.String(maxLength: 1, fixedLength: true, unicode: false),
                        ExtremeWeatherGearUse = c.String(maxLength: 1, fixedLength: true, unicode: false),
                        Hypothermia = c.String(maxLength: 1, fixedLength: true, unicode: false),
                        Frostbite = c.String(maxLength: 1, fixedLength: true, unicode: false),
                        encounter_guid = c.Guid(nullable: false, defaultValueSql: "newid()"),
                        AllAdherenceRate = c.Decimal(precision: 10, scale: 2),
                        ARVAdherenceRate = c.Decimal(precision: 10, scale: 2),
                        EncounterTypeID = c.Int(),
                        ChildID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.EncounterID)
                .ForeignKey("dbo.EncounterType", t => t.EncounterTypeID)
                .ForeignKey("dbo.Child", t => t.ChildID)
                .Index(t => t.EncounterTypeID, name: "IX_EncounterType_EncounterTypeID")
                .Index(t => t.ChildID, name: "IX_Child_ChildID");
            
            CreateTable(
                "dbo.EncounterType",
                c => new
                    {
                        EncounterTypeID = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false, maxLength: 50, unicode: false),
                        encountertype_guid = c.Guid(nullable: false, defaultValueSql: "newid()"),
                        ProjectID = c.Int(),
                    })
                .PrimaryKey(t => t.EncounterTypeID)
                .ForeignKey("dbo.Project", t => t.ProjectID)
                .Index(t => t.ProjectID, name: "IX_Project_ProjectID");
            
            CreateTable(
                "dbo.GuardianRelation",
                c => new
                    {
                        RelationID = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false, maxLength: 30, unicode: false),
                        GuardianRelation_guid = c.Guid(nullable: false, defaultValueSql: "newid()"),
                    })
                .PrimaryKey(t => t.RelationID);
            
            CreateTable(
                "dbo.OVCType",
                c => new
                    {
                        OVCTypeID = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false, maxLength: 50, unicode: false),
                        ovctype_guid = c.Guid(nullable: false, defaultValueSql: "newid()"),
                    })
                .PrimaryKey(t => t.OVCTypeID);
            
            CreateTable(
                "dbo.Ward",
                c => new
                    {
                        WardID = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false, maxLength: 30, unicode: false),
                        ward_guid = c.Guid(nullable: false, defaultValueSql: "newid()"),
                    })
                .PrimaryKey(t => t.WardID);
            
            CreateTable(
                "dbo.CarePlanDomainSupportService",
                c => new
                    {
                        CarePlanDomainSupportServiceID = c.Int(nullable: false, identity: true),
                        careplandomainsupportservice_guid = c.Guid(nullable: false, defaultValueSql: "newid()"),
                        SupportServiceTypeID = c.Int(),
                        CarePlanDomainID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CarePlanDomainSupportServiceID)
                .ForeignKey("dbo.SupportServiceType", t => t.SupportServiceTypeID)
                .ForeignKey("dbo.CarePlanDomain", t => t.CarePlanDomainID)
                .Index(t => t.SupportServiceTypeID, name: "IX_SupportServiceType_SupportServiceTypeID")
                .Index(t => t.CarePlanDomainID, name: "IX_CarePlanDomain_CarePlanDomainID");
            
            CreateTable(
                "dbo.SupportServiceType",
                c => new
                    {
                        SupportServiceTypeID = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false, maxLength: 50, unicode: false),
                        Default = c.Boolean(),
                        supportservicetype_guid = c.Guid(nullable: false, defaultValueSql: "newid()"),
                        DomainID = c.Int(),
                    })
                .PrimaryKey(t => t.SupportServiceTypeID)
                .ForeignKey("dbo.Domain", t => t.DomainID)
                .Index(t => t.DomainID, name: "IX_Domain_DomainID");
            
            CreateTable(
                "dbo.CSIDomain",
                c => new
                    {
                        CSIDomainID = c.Int(nullable: false, identity: true),
                        csidomain_guid = c.Guid(nullable: false, defaultValueSql: "newid()"),
                        CSIID = c.Int(),
                        DomainID = c.Int(),
                    })
                .PrimaryKey(t => t.CSIDomainID)
                .ForeignKey("dbo.CSI", t => t.CSIID)
                .ForeignKey("dbo.Domain", t => t.DomainID)
                .Index(t => t.CSIID, name: "IX_CSI_CSIID")
                .Index(t => t.DomainID, name: "IX_Domain_DomainID");
            
            CreateTable(
                "dbo.CSIDomainScore",
                c => new
                    {
                        CSIDomainScoreID = c.Int(nullable: false, identity: true),
                        Score = c.Int(nullable: false),
                        Comments = c.String(unicode: false),
                        csidomainscore_guid = c.Guid(nullable: false, defaultValueSql: "newid()"),
                        CSIDomainID = c.Int(),
                        QuestionID = c.Int(),
                    })
                .PrimaryKey(t => t.CSIDomainScoreID)
                .ForeignKey("dbo.CSIDomain", t => t.CSIDomainID)
                .ForeignKey("dbo.Question", t => t.QuestionID)
                .Index(t => t.CSIDomainID, name: "IX_CSIDomain_CSIDomainID")
                .Index(t => t.QuestionID, name: "IX_Question_QuestionID");
            
            CreateTable(
                "dbo.CSIDomainSource",
                c => new
                    {
                        CSIDomainSourceID = c.Int(nullable: false, identity: true),
                        csidomainsource_guid = c.Guid(nullable: false, defaultValueSql: "newid()"),
                        CSIDomainID = c.Int(),
                        SourceID = c.Int(),
                    })
                .PrimaryKey(t => t.CSIDomainSourceID)
                .ForeignKey("dbo.CSIDomain", t => t.CSIDomainID)
                .ForeignKey("dbo.Source", t => t.SourceID)
                .Index(t => t.CSIDomainID, name: "IX_CSIDomain_CSIDomainID")
                .Index(t => t.SourceID, name: "IX_Source_SourceID");
            
            CreateTable(
                "dbo.Source",
                c => new
                    {
                        SourceID = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false, maxLength: 30, unicode: false),
                        source_guid = c.Guid(nullable: false, defaultValueSql: "newid()"),
                    })
                .PrimaryKey(t => t.SourceID);
            
            CreateTable(
                "dbo.CSIDomainSupportService",
                c => new
                    {
                        CSIDomainSupportServiceID = c.Int(nullable: false, identity: true),
                        OtherService = c.String(maxLength: 50, unicode: false),
                        OtherServiceProvider = c.String(maxLength: 50, unicode: false),
                        csidomainsupportservice_guid = c.Guid(nullable: false, defaultValueSql: "newid()"),
                        CSIDomainID = c.Int(),
                        ServiceProviderID = c.Int(),
                        SupportServiceTypeID = c.Int(),
                    })
                .PrimaryKey(t => t.CSIDomainSupportServiceID)
                .ForeignKey("dbo.CSIDomain", t => t.CSIDomainID)
                .ForeignKey("dbo.ServiceProvider", t => t.ServiceProviderID)
                .ForeignKey("dbo.SupportServiceType", t => t.SupportServiceTypeID)
                .Index(t => t.CSIDomainID, name: "IX_CSIDomain_CSIDomainID")
                .Index(t => t.ServiceProviderID, name: "IX_ServiceProvider_ServiceProviderID")
                .Index(t => t.SupportServiceTypeID, name: "IX_SupportServiceType_SupportServiceTypeID");
            
            CreateTable(
                "dbo.ServiceProvider",
                c => new
                    {
                        ServiceProviderID = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false, maxLength: 50, unicode: false),
                        InformationURL = c.String(maxLength: 200, unicode: false),
                        serviceprovider_guid = c.Guid(nullable: false, defaultValueSql: "newid()"),
                        ServiceProviderTypeID = c.Int(),
                    })
                .PrimaryKey(t => t.ServiceProviderID)
                .ForeignKey("dbo.ServiceProviderType", t => t.ServiceProviderTypeID)
                .Index(t => t.ServiceProviderTypeID, name: "IX_ServiceProviderType_ServiceProviderTypeID");
            
            CreateTable(
                "dbo.ServiceProviderType",
                c => new
                    {
                        ServiceProviderTypeID = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false, maxLength: 30, unicode: false),
                        serviceprovidertype_guid = c.Guid(nullable: false, defaultValueSql: "newid()"),
                    })
                .PrimaryKey(t => t.ServiceProviderTypeID);
            
            CreateTable(
                "dbo.CSIEvent",
                c => new
                    {
                        CSIEventID = c.Int(nullable: false, identity: true),
                        Comments = c.String(unicode: false),
                        EffectiveDate = c.DateTime(nullable: false),
                        csievent_guid = c.Guid(nullable: false, defaultValueSql: "newid()"),
                        CSIID = c.Int(),
                        EventID = c.Int(),
                    })
                .PrimaryKey(t => t.CSIEventID)
                .ForeignKey("dbo.CSI", t => t.CSIID)
                .ForeignKey("dbo.Event", t => t.EventID)
                .Index(t => t.CSIID, name: "IX_CSI_CSIID")
                .Index(t => t.EventID, name: "IX_Event_EventID");
            
            CreateTable(
                "dbo.DrugDose",
                c => new
                    {
                        DrugDoseID = c.Int(nullable: false, identity: true),
                        WeightFrom = c.Decimal(nullable: false, precision: 3, scale: 1),
                        WeightTo = c.Decimal(nullable: false, precision: 3, scale: 1),
                        Dose = c.Decimal(nullable: false, precision: 2, scale: 1),
                        DrugID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.DrugDoseID)
                .ForeignKey("dbo.Drug", t => t.DrugID)
                .Index(t => t.DrugID, name: "IX_Drug_DrugID");
            
            CreateTable(
                "dbo.Drug",
                c => new
                    {
                        DrugID = c.Int(nullable: false, identity: true),
                        DrugName = c.String(nullable: false, maxLength: 50, unicode: false),
                        Acronym = c.String(maxLength: 20, unicode: false),
                        ARVDrug = c.Boolean(nullable: false),
                        Active = c.Boolean(),
                        drug_guid = c.Guid(nullable: false, defaultValueSql: "newid()"),
                    })
                .PrimaryKey(t => t.DrugID);
            
            CreateTable(
                "dbo.EncounterDrug",
                c => new
                    {
                        EncounterDrugID = c.Int(nullable: false, identity: true),
                        PillsDispensed = c.Int(nullable: false),
                        PillsReturned = c.Int(nullable: false),
                        PillsExpected = c.Int(nullable: false),
                        AdherenceRate = c.Decimal(nullable: false, precision: 10, scale: 2),
                        encounterdrug_guid = c.Guid(nullable: false, defaultValueSql: "newid()"),
                        DrugID = c.Int(),
                        EncounterID = c.Int(),
                    })
                .PrimaryKey(t => t.EncounterDrugID)
                .ForeignKey("dbo.Drug", t => t.DrugID)
                .ForeignKey("dbo.Encounter", t => t.EncounterID)
                .Index(t => t.DrugID, name: "IX_Drug_DrugID")
                .Index(t => t.EncounterID, name: "IX_Encounter_EncounterID");
            
            CreateTable(
                "dbo.Field",
                c => new
                    {
                        FieldID = c.Int(nullable: false, identity: true),
                        FieldCode = c.String(nullable: false, maxLength: 10, fixedLength: true, unicode: false),
                        Description = c.String(nullable: false, maxLength: 100, unicode: false),
                    })
                .PrimaryKey(t => t.FieldID);
            
            CreateTable(
                "dbo.FollowUp",
                c => new
                    {
                        FollowUpID = c.Int(nullable: false, identity: true),
                        DateOfFollowUp = c.DateTime(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                        ChildID = c.Int(),
                        CreatedUserID = c.Int(),
                        OutcomeID = c.Int(),
                        RecipientTypeID = c.Int(),
                    })
                .PrimaryKey(t => t.FollowUpID)
                .ForeignKey("dbo.Child", t => t.ChildID)
                .ForeignKey("dbo.User", t => t.CreatedUserID)
                .ForeignKey("dbo.Outcome", t => t.OutcomeID)
                .ForeignKey("dbo.RecipientType", t => t.RecipientTypeID)
                .Index(t => t.ChildID, name: "IX_Child_ChildID")
                .Index(t => t.CreatedUserID, name: "IX_CreatedUser_UserID")
                .Index(t => t.OutcomeID, name: "IX_Outcome_OutcomeID")
                .Index(t => t.RecipientTypeID, name: "IX_RecipientType_RecipientTypeID");
            
            CreateTable(
                "dbo.Outcome",
                c => new
                    {
                        OutcomeID = c.Int(nullable: false),
                        Code = c.String(nullable: false, maxLength: 4, unicode: false),
                        Description = c.String(nullable: false, maxLength: 50, unicode: false),
                        outcome_guid = c.Guid(nullable: false, defaultValueSql: "newid()"),
                    })
                .PrimaryKey(t => t.OutcomeID);
            
            CreateTable(
                "dbo.RecipientType",
                c => new
                    {
                        RecipientTypeID = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false, maxLength: 50, unicode: false),
                        recipienttype_guid = c.Guid(nullable: false, defaultValueSql: "newid()"),
                    })
                .PrimaryKey(t => t.RecipientTypeID);
            
            CreateTable(
                "dbo.FormField",
                c => new
                    {
                        FormFieldID = c.Int(nullable: false, identity: true),
                        Value = c.String(nullable: false, maxLength: 100, unicode: false),
                        FieldID = c.Int(),
                        FormID = c.Int(nullable: false),
                        LanguageID = c.Int(),
                    })
                .PrimaryKey(t => t.FormFieldID)
                .ForeignKey("dbo.Field", t => t.FieldID)
                .ForeignKey("dbo.Form", t => t.FormID)
                .ForeignKey("dbo.Language", t => t.LanguageID)
                .Index(t => t.FieldID, name: "IX_Field_FieldID")
                .Index(t => t.FormID, name: "IX_Form_FormID")
                .Index(t => t.LanguageID, name: "IX_Language_LanguageID");
            
            CreateTable(
                "dbo.Form",
                c => new
                    {
                        FormID = c.Int(nullable: false, identity: true),
                        FormName = c.String(nullable: false, maxLength: 30, unicode: false),
                    })
                .PrimaryKey(t => t.FormID);
            
            CreateTable(
                "dbo.GeneralInfo",
                c => new
                    {
                        AdminMode = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.AdminMode);
            
            CreateTable(
                "dbo.Guideline",
                c => new
                    {
                        GuidelineID = c.Int(nullable: false, identity: true),
                        DescriptionEnglish = c.String(nullable: false, unicode: false),
                        DescriptionSesotho = c.String(unicode: false),
                        guideline_guid = c.Guid(nullable: false, defaultValueSql: "newid()"),
                    })
                .PrimaryKey(t => t.GuidelineID);
            
            CreateTable(
                "dbo.SchoolLevel",
                c => new
                    {
                        SchoolLevelID = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false, maxLength: 50, unicode: false),
                        schoollevel_guid = c.Guid(nullable: false, defaultValueSql: "newid()"),
                    })
                .PrimaryKey(t => t.SchoolLevelID);
            
            CreateTable(
                "dbo.Site",
                c => new
                    {
                        SiteID = c.Int(nullable: false, identity: true),
                        SiteName = c.String(nullable: false, maxLength: 20),
                        LastReplicatedDate = c.DateTime(storeType: "date"),
                        SiteType = c.String(maxLength: 10, unicode: false),
                    })
                .PrimaryKey(t => t.SiteID);
            
            CreateTable(
                "dbo.UserAction",
                c => new
                    {
                        UserActionID = c.Int(nullable: false, identity: true),
                        ActionID = c.Int(),
                        SiteID = c.Int(),
                        UserID = c.Int(),
                    })
                .PrimaryKey(t => t.UserActionID)
                .ForeignKey("dbo.Action", t => t.ActionID)
                .ForeignKey("dbo.Site", t => t.SiteID)
                .ForeignKey("dbo.User", t => t.UserID)
                .Index(t => t.ActionID, name: "IX_Action_ActionID")
                .Index(t => t.SiteID, name: "IX_Site_SiteID")
                .Index(t => t.UserID, name: "IX_User_UserID");
            
            CreateTable(
                "dbo.WellbeingXref",
                c => new
                    {
                        WellbeingXrefID = c.Int(nullable: false, identity: true),
                        CarePlanID = c.Int(),
                        CSIID = c.Int(),
                    })
                .PrimaryKey(t => t.WellbeingXrefID)
                .ForeignKey("dbo.CarePlan", t => t.CarePlanID)
                .ForeignKey("dbo.CSI", t => t.CSIID)
                .Index(t => t.CarePlanID, name: "IX_CarePlan_CarePlanID")
                .Index(t => t.CSIID, name: "IX_CSI_CSIID");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.WellbeingXref", "CSIID", "dbo.CSI");
            DropForeignKey("dbo.WellbeingXref", "CarePlanID", "dbo.CarePlan");
            DropForeignKey("dbo.UserAction", "UserID", "dbo.User");
            DropForeignKey("dbo.UserAction", "SiteID", "dbo.Site");
            DropForeignKey("dbo.UserAction", "ActionID", "dbo.Action");
            DropForeignKey("dbo.FormField", "LanguageID", "dbo.Language");
            DropForeignKey("dbo.FormField", "FormID", "dbo.Form");
            DropForeignKey("dbo.FormField", "FieldID", "dbo.Field");
            DropForeignKey("dbo.FollowUp", "RecipientTypeID", "dbo.RecipientType");
            DropForeignKey("dbo.FollowUp", "OutcomeID", "dbo.Outcome");
            DropForeignKey("dbo.FollowUp", "CreatedUserID", "dbo.User");
            DropForeignKey("dbo.FollowUp", "ChildID", "dbo.Child");
            DropForeignKey("dbo.EncounterDrug", "EncounterID", "dbo.Encounter");
            DropForeignKey("dbo.EncounterDrug", "DrugID", "dbo.Drug");
            DropForeignKey("dbo.DrugDose", "DrugID", "dbo.Drug");
            DropForeignKey("dbo.CSIEvent", "EventID", "dbo.Event");
            DropForeignKey("dbo.CSIEvent", "CSIID", "dbo.CSI");
            DropForeignKey("dbo.CSIDomainSupportService", "SupportServiceTypeID", "dbo.SupportServiceType");
            DropForeignKey("dbo.CSIDomainSupportService", "ServiceProviderID", "dbo.ServiceProvider");
            DropForeignKey("dbo.ServiceProvider", "ServiceProviderTypeID", "dbo.ServiceProviderType");
            DropForeignKey("dbo.CSIDomainSupportService", "CSIDomainID", "dbo.CSIDomain");
            DropForeignKey("dbo.CSIDomainSource", "SourceID", "dbo.Source");
            DropForeignKey("dbo.CSIDomainSource", "CSIDomainID", "dbo.CSIDomain");
            DropForeignKey("dbo.CSIDomainScore", "QuestionID", "dbo.Question");
            DropForeignKey("dbo.CSIDomainScore", "CSIDomainID", "dbo.CSIDomain");
            DropForeignKey("dbo.CSIDomain", "DomainID", "dbo.Domain");
            DropForeignKey("dbo.CSIDomain", "CSIID", "dbo.CSI");
            DropForeignKey("dbo.CarePlanDomain", "DomainID", "dbo.Domain");
            DropForeignKey("dbo.CarePlanDomainSupportService", "CarePlanDomainID", "dbo.CarePlanDomain");
            DropForeignKey("dbo.CarePlanDomainSupportService", "SupportServiceTypeID", "dbo.SupportServiceType");
            DropForeignKey("dbo.SupportServiceType", "DomainID", "dbo.Domain");
            DropForeignKey("dbo.CarePlanDomain", "CarePlanID", "dbo.CarePlan");
            DropForeignKey("dbo.CarePlan", "LastUpdatedUserID", "dbo.User");
            DropForeignKey("dbo.CSI", "LastUpdatedUserID", "dbo.User");
            DropForeignKey("dbo.CSI", "DistrictID", "dbo.District");
            DropForeignKey("dbo.CSI", "CreatedUserID", "dbo.User");
            DropForeignKey("dbo.Child", "WardID", "dbo.Ward");
            DropForeignKey("dbo.Child", "VillageID", "dbo.Village");
            DropForeignKey("dbo.Child", "OVCTypeID", "dbo.OVCType");
            DropForeignKey("dbo.Child", "LastUpdatedUserID", "dbo.User");
            DropForeignKey("dbo.Child", "GuardianRelationID", "dbo.GuardianRelation");
            DropForeignKey("dbo.Encounter", "ChildID", "dbo.Child");
            DropForeignKey("dbo.Encounter", "EncounterTypeID", "dbo.EncounterType");
            DropForeignKey("dbo.EncounterType", "ProjectID", "dbo.Project");
            DropForeignKey("dbo.Child", "DistrictID", "dbo.District");
            DropForeignKey("dbo.Village", "DistrictID", "dbo.District");
            DropForeignKey("dbo.CSI", "ChildID", "dbo.Child");
            DropForeignKey("dbo.Child", "CreatedUserID", "dbo.User");
            DropForeignKey("dbo.Child", "CommunityCouncilID", "dbo.CommunityCouncil");
            DropForeignKey("dbo.ChildTrainingEvent", "ChildID", "dbo.Child");
            DropForeignKey("dbo.ChildTrainingEvent", "TrainingEventID", "dbo.TrainingEvent");
            DropForeignKey("dbo.ChildTrainingEventItem", "ChildTrainingEventID", "dbo.ChildTrainingEvent");
            DropForeignKey("dbo.ChildTrainingEventItem", "TrainingItemID", "dbo.TrainingItem");
            DropForeignKey("dbo.TrainingItem", "TrainingID", "dbo.Training");
            DropForeignKey("dbo.TrainingEvent", "TrainingID", "dbo.Training");
            DropForeignKey("dbo.TrainingEvent", "PartnerProjectID", "dbo.PartnerProject");
            DropForeignKey("dbo.SiteProgress", "PartnerProjectID", "dbo.PartnerProject");
            DropForeignKey("dbo.SiteProgressItem", "SiteProgressID", "dbo.SiteProgress");
            DropForeignKey("dbo.SiteProgressItem", "SiteProgressItemTypeID", "dbo.SiteProgressItemType");
            DropForeignKey("dbo.PartnerProject", "ProjectID", "dbo.Project");
            DropForeignKey("dbo.PartnerProjectSnapshot", "PartnerProjectID", "dbo.PartnerProject");
            DropForeignKey("dbo.Snapshot", "PartnerProjectSnapshotID", "dbo.PartnerProjectSnapshot");
            DropForeignKey("dbo.Snapshot", "TrackerItemID", "dbo.TrackerItem");
            DropForeignKey("dbo.ProjectTrackerItem", "TrackerItemID", "dbo.TrackerItem");
            DropForeignKey("dbo.ProjectTrackerItemValue", "ProjectTrackerItemID", "dbo.ProjectTrackerItem");
            DropForeignKey("dbo.ProjectTrackerItem", "ProjectID", "dbo.Project");
            DropForeignKey("dbo.PartnerProject", "PartnerID", "dbo.Partner");
            DropForeignKey("dbo.Training", "ProjectID", "dbo.Project");
            DropForeignKey("dbo.ChildStatusHistory", "ChildID", "dbo.Child");
            DropForeignKey("dbo.ChildStatusHistory", "CreatedUserID", "dbo.User");
            DropForeignKey("dbo.ChildStatusHistory", "ChildStatusID", "dbo.ChildStatus");
            DropForeignKey("dbo.ChildRegistration", "ChildID", "dbo.Child");
            DropForeignKey("dbo.ChildRegistration", "RegistrationTypeID", "dbo.RegistrationType");
            DropForeignKey("dbo.ChildProject", "ChildID", "dbo.Child");
            DropForeignKey("dbo.ChildProject", "ProjectID", "dbo.Project");
            DropForeignKey("dbo.ChildPartner", "ChildID", "dbo.Child");
            DropForeignKey("dbo.ChildPartner", "PartnerID", "dbo.Partner");
            DropForeignKey("dbo.ChildEvent", "ChildID", "dbo.Child");
            DropForeignKey("dbo.ChildEvent", "EventID", "dbo.Event");
            DropForeignKey("dbo.ChildDisability", "ChildID", "dbo.Child");
            DropForeignKey("dbo.ChildDisability", "DisabilityID", "dbo.Disability");
            DropForeignKey("dbo.CarePlan", "CSIID", "dbo.CSI");
            DropForeignKey("dbo.CSI", "CareGiverRelationID", "dbo.CareGiverRelation");
            DropForeignKey("dbo.CarePlan", "CreatedUserID", "dbo.User");
            DropForeignKey("dbo.User", "PartnerID", "dbo.Partner");
            DropForeignKey("dbo.User", "DefaultLanguageID", "dbo.Language");
            DropForeignKey("dbo.Answer", "ScoreID", "dbo.ScoreType");
            DropForeignKey("dbo.Question", "DomainID", "dbo.Domain");
            DropForeignKey("dbo.Answer", "QuestionID", "dbo.Question");
            DropIndex("dbo.WellbeingXref", "IX_CSI_CSIID");
            DropIndex("dbo.WellbeingXref", "IX_CarePlan_CarePlanID");
            DropIndex("dbo.UserAction", "IX_User_UserID");
            DropIndex("dbo.UserAction", "IX_Site_SiteID");
            DropIndex("dbo.UserAction", "IX_Action_ActionID");
            DropIndex("dbo.FormField", "IX_Language_LanguageID");
            DropIndex("dbo.FormField", "IX_Form_FormID");
            DropIndex("dbo.FormField", "IX_Field_FieldID");
            DropIndex("dbo.FollowUp", "IX_RecipientType_RecipientTypeID");
            DropIndex("dbo.FollowUp", "IX_Outcome_OutcomeID");
            DropIndex("dbo.FollowUp", "IX_CreatedUser_UserID");
            DropIndex("dbo.FollowUp", "IX_Child_ChildID");
            DropIndex("dbo.EncounterDrug", "IX_Encounter_EncounterID");
            DropIndex("dbo.EncounterDrug", "IX_Drug_DrugID");
            DropIndex("dbo.DrugDose", "IX_Drug_DrugID");
            DropIndex("dbo.CSIEvent", "IX_Event_EventID");
            DropIndex("dbo.CSIEvent", "IX_CSI_CSIID");
            DropIndex("dbo.ServiceProvider", "IX_ServiceProviderType_ServiceProviderTypeID");
            DropIndex("dbo.CSIDomainSupportService", "IX_SupportServiceType_SupportServiceTypeID");
            DropIndex("dbo.CSIDomainSupportService", "IX_ServiceProvider_ServiceProviderID");
            DropIndex("dbo.CSIDomainSupportService", "IX_CSIDomain_CSIDomainID");
            DropIndex("dbo.CSIDomainSource", "IX_Source_SourceID");
            DropIndex("dbo.CSIDomainSource", "IX_CSIDomain_CSIDomainID");
            DropIndex("dbo.CSIDomainScore", "IX_Question_QuestionID");
            DropIndex("dbo.CSIDomainScore", "IX_CSIDomain_CSIDomainID");
            DropIndex("dbo.CSIDomain", "IX_Domain_DomainID");
            DropIndex("dbo.CSIDomain", "IX_CSI_CSIID");
            DropIndex("dbo.SupportServiceType", "IX_Domain_DomainID");
            DropIndex("dbo.CarePlanDomainSupportService", "IX_CarePlanDomain_CarePlanDomainID");
            DropIndex("dbo.CarePlanDomainSupportService", "IX_SupportServiceType_SupportServiceTypeID");
            DropIndex("dbo.EncounterType", "IX_Project_ProjectID");
            DropIndex("dbo.Encounter", "IX_Child_ChildID");
            DropIndex("dbo.Encounter", "IX_EncounterType_EncounterTypeID");
            DropIndex("dbo.Village", "IX_District_DistrictID");
            DropIndex("dbo.SiteProgressItem", "IX_SiteProgress_SiteProgressID");
            DropIndex("dbo.SiteProgressItem", "IX_SiteProgressItemType_SiteProgressItemTypeID");
            DropIndex("dbo.SiteProgress", "IX_PartnerProject_PartnerProjectID");
            DropIndex("dbo.ProjectTrackerItemValue", "IX_ProjectTrackerItem_ProjectTrackerItemID");
            DropIndex("dbo.ProjectTrackerItem", "IX_TrackerItem_TrackerItemID");
            DropIndex("dbo.ProjectTrackerItem", "IX_Project_ProjectID");
            DropIndex("dbo.Snapshot", "IX_PartnerProjectSnapshot_PartnerProjectSnapshotID");
            DropIndex("dbo.Snapshot", "IX_TrackerItem_TrackerItemID");
            DropIndex("dbo.PartnerProjectSnapshot", "IX_PartnerProject_PartnerProjectID");
            DropIndex("dbo.PartnerProject", "IX_Project_ProjectID");
            DropIndex("dbo.PartnerProject", "IX_Partner_PartnerID");
            DropIndex("dbo.TrainingEvent", "IX_Training_TrainingID");
            DropIndex("dbo.TrainingEvent", "IX_PartnerProject_PartnerProjectID");
            DropIndex("dbo.Training", new[] { "ProjectID" });
            DropIndex("dbo.TrainingItem", "IX_Training_TrainingID");
            DropIndex("dbo.ChildTrainingEventItem", "IX_ChildTrainingEvent_ChildTrainingEventID");
            DropIndex("dbo.ChildTrainingEventItem", "IX_TrainingItem_TrainingItemID");
            DropIndex("dbo.ChildTrainingEvent", "IX_Child_ChildID");
            DropIndex("dbo.ChildTrainingEvent", "IX_TrainingEvent_TrainingEventID");
            DropIndex("dbo.ChildStatusHistory", "IX_Child_ChildID");
            DropIndex("dbo.ChildStatusHistory", "IX_CreatedUser_UserID");
            DropIndex("dbo.ChildStatusHistory", "IX_ChildStatus_StatusID");
            DropIndex("dbo.ChildRegistration", "IX_Child_ChildID");
            DropIndex("dbo.ChildRegistration", "IX_RegistrationType_RegistrationTypeID");
            DropIndex("dbo.ChildProject", "IX_Child_ChildID");
            DropIndex("dbo.ChildProject", "IX_Project_ProjectID");
            DropIndex("dbo.ChildPartner", "IX_Child_ChildID");
            DropIndex("dbo.ChildPartner", "IX_Partner_PartnerID");
            DropIndex("dbo.ChildEvent", "IX_Child_ChildID");
            DropIndex("dbo.ChildEvent", "IX_Event_EventID");
            DropIndex("dbo.ChildDisability", "IX_Child_ChildID");
            DropIndex("dbo.ChildDisability", "IX_Disability_DisabilityID");
            DropIndex("dbo.Child", "IX_Ward_WardID");
            DropIndex("dbo.Child", "IX_Village_VillageID");
            DropIndex("dbo.Child", "IX_OVCType_OVCTypeID");
            DropIndex("dbo.Child", "IX_LastUpdatedUser_UserID");
            DropIndex("dbo.Child", "IX_GuardianRelation_RelationID");
            DropIndex("dbo.Child", "IX_District_DistrictID");
            DropIndex("dbo.Child", "IX_CreatedUser_UserID");
            DropIndex("dbo.Child", "IX_CommunityCouncil_CommunityCouncilID");
            DropIndex("dbo.CSI", "IX_LastUpdatedUser_UserID");
            DropIndex("dbo.CSI", "IX_District_DistrictID");
            DropIndex("dbo.CSI", "IX_CreatedUser_UserID");
            DropIndex("dbo.CSI", "IX_Child_ChildID");
            DropIndex("dbo.CSI", "IX_CareGiverRelation_RelationID");
            DropIndex("dbo.User", "IX_Partner_PartnerID");
            DropIndex("dbo.User", "IX_DefaultLanguage_LanguageID");
            DropIndex("dbo.CarePlan", "IX_LastUpdatedUser_UserID");
            DropIndex("dbo.CarePlan", "IX_CSI_CSIID");
            DropIndex("dbo.CarePlan", "IX_CreatedUser_UserID");
            DropIndex("dbo.CarePlanDomain", "IX_Domain_DomainID");
            DropIndex("dbo.CarePlanDomain", "IX_CarePlan_CarePlanID");
            DropIndex("dbo.Question", "IX_Domain_DomainID");
            DropIndex("dbo.Answer", "IX_Score_ScoreTypeID");
            DropIndex("dbo.Answer", "IX_Question_QuestionID");
            DropTable("dbo.WellbeingXref");
            DropTable("dbo.UserAction");
            DropTable("dbo.Site");
            DropTable("dbo.SchoolLevel");
            DropTable("dbo.Guideline");
            DropTable("dbo.GeneralInfo");
            DropTable("dbo.Form");
            DropTable("dbo.FormField");
            DropTable("dbo.RecipientType");
            DropTable("dbo.Outcome");
            DropTable("dbo.FollowUp");
            DropTable("dbo.Field");
            DropTable("dbo.EncounterDrug");
            DropTable("dbo.Drug");
            DropTable("dbo.DrugDose");
            DropTable("dbo.CSIEvent");
            DropTable("dbo.ServiceProviderType");
            DropTable("dbo.ServiceProvider");
            DropTable("dbo.CSIDomainSupportService");
            DropTable("dbo.Source");
            DropTable("dbo.CSIDomainSource");
            DropTable("dbo.CSIDomainScore");
            DropTable("dbo.CSIDomain");
            DropTable("dbo.SupportServiceType");
            DropTable("dbo.CarePlanDomainSupportService");
            DropTable("dbo.Ward");
            DropTable("dbo.OVCType");
            DropTable("dbo.GuardianRelation");
            DropTable("dbo.EncounterType");
            DropTable("dbo.Encounter");
            DropTable("dbo.Village");
            DropTable("dbo.District");
            DropTable("dbo.CommunityCouncil");
            DropTable("dbo.SiteProgressItemType");
            DropTable("dbo.SiteProgressItem");
            DropTable("dbo.SiteProgress");
            DropTable("dbo.ProjectTrackerItemValue");
            DropTable("dbo.ProjectTrackerItem");
            DropTable("dbo.TrackerItem");
            DropTable("dbo.Snapshot");
            DropTable("dbo.PartnerProjectSnapshot");
            DropTable("dbo.PartnerProject");
            DropTable("dbo.TrainingEvent");
            DropTable("dbo.Training");
            DropTable("dbo.TrainingItem");
            DropTable("dbo.ChildTrainingEventItem");
            DropTable("dbo.ChildTrainingEvent");
            DropTable("dbo.ChildStatus");
            DropTable("dbo.ChildStatusHistory");
            DropTable("dbo.RegistrationType");
            DropTable("dbo.ChildRegistration");
            DropTable("dbo.Project");
            DropTable("dbo.ChildProject");
            DropTable("dbo.ChildPartner");
            DropTable("dbo.Event");
            DropTable("dbo.ChildEvent");
            DropTable("dbo.Disability");
            DropTable("dbo.ChildDisability");
            DropTable("dbo.Child");
            DropTable("dbo.CSI");
            DropTable("dbo.Partner");
            DropTable("dbo.Language");
            DropTable("dbo.User");
            DropTable("dbo.CarePlan");
            DropTable("dbo.CarePlanDomain");
            DropTable("dbo.CareGiverRelation");
            DropTable("dbo.ScoreType");
            DropTable("dbo.Domain");
            DropTable("dbo.Question");
            DropTable("dbo.Answer");
            DropTable("dbo.Action");
        }
    }
}
