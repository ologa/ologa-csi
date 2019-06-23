namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReferenceServicesEntities : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Reference",
                c => new
                    {
                        ReferenceID = c.Int(nullable: false, identity: true),
                        Value = c.String(),
                        ReferenceTypeID = c.Int(nullable: false),
                        ReferenceServiceID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ReferenceID)
                .ForeignKey("dbo.ReferenceService", t => t.ReferenceServiceID, cascadeDelete: false)
                .ForeignKey("dbo.ReferenceType", t => t.ReferenceTypeID, cascadeDelete: false)
                .Index(t => t.ReferenceTypeID)
                .Index(t => t.ReferenceServiceID);
            
            CreateTable(
                "dbo.ReferenceService",
                c => new
                    {
                        ReferenceServiceID = c.Int(nullable: false, identity: true),
                        ReferenceServiceGuid = c.Guid(nullable: false),
                        ReferenceNumber = c.Int(nullable: false),
                        ReferencedBy = c.String(),
                        ReferenceDate = c.DateTime(nullable: false),
                        HealthWorker = c.String(),
                        HealthUnitName = c.String(),
                        HealthAttendedDate = c.String(),
                        SocialWorker = c.String(),
                        SocialAttendedDate = c.String(),
                        RoutineVisitMemberID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ReferenceServiceID)
                .ForeignKey("dbo.RoutineVisitMember", t => t.RoutineVisitMemberID, cascadeDelete: false)
                .Index(t => t.RoutineVisitMemberID);
            
            CreateTable(
                "dbo.ReferenceType",
                c => new
                    {
                        ReferenceTypeID = c.Int(nullable: false, identity: true),
                        ReferenceName = c.String(),
                        ReferenceCategory = c.String(),
                        ReferenceOrder = c.Int(nullable: false),
                        FieldType = c.String(),
                        OriginReferenceID = c.Int(),
                    })
                .PrimaryKey(t => t.ReferenceTypeID)
                .ForeignKey("dbo.ReferenceType", t => t.OriginReferenceID)
                .Index(t => t.OriginReferenceID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Reference", "ReferenceTypeID", "dbo.ReferenceType");
            DropForeignKey("dbo.ReferenceType", "OriginReferenceID", "dbo.ReferenceType");
            DropForeignKey("dbo.ReferenceService", "RoutineVisitMemberID", "dbo.RoutineVisitMember");
            DropForeignKey("dbo.Reference", "ReferenceServiceID", "dbo.ReferenceService");
            DropIndex("dbo.ReferenceType", new[] { "OriginReferenceID" });
            DropIndex("dbo.ReferenceService", new[] { "RoutineVisitMemberID" });
            DropIndex("dbo.Reference", new[] { "ReferenceServiceID" });
            DropIndex("dbo.Reference", new[] { "ReferenceTypeID" });
            DropTable("dbo.ReferenceType");
            DropTable("dbo.ReferenceService");
            DropTable("dbo.Reference");
        }
    }
}
