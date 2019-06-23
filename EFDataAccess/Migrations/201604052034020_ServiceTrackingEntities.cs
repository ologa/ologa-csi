namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ServiceTrackingEntities : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ServiceCategory",
                c => new
                    {
                        ServiceCategoryID = c.Int(nullable: false, identity: true),
                        CategoryName = c.String(),
                        CategoryOrder = c.Int(nullable: false),
                        ServiceCategoryGuid = c.Guid(nullable: false, defaultValueSql: "newid()"),
                        ServiceTrackSectionID = c.Int(),
                    })
                .PrimaryKey(t => t.ServiceCategoryID)
                .ForeignKey("dbo.ServiceTrackSection", t => t.ServiceTrackSectionID)
                .Index(t => t.ServiceTrackSectionID, name: "IX_ServiceTrackSection_ServiceTrackSectionID");
            
            CreateTable(
                "dbo.Service",
                c => new
                    {
                        ServiceID = c.Int(nullable: false, identity: true),
                        ServiceName = c.String(),
                        ServiceOrder = c.Int(nullable: false),
                        ServiceFieldType = c.String(),
                        ServiceGuid = c.Guid(nullable: false, defaultValueSql: "newid()"),
                        ServiceCategoryID = c.Int(),
                    })
                .PrimaryKey(t => t.ServiceID)
                .ForeignKey("dbo.ServiceCategory", t => t.ServiceCategoryID)
                .Index(t => t.ServiceCategoryID, name: "IX_ServiceCategory_ServiceCategoryID");
            
            CreateTable(
                "dbo.ServiceInstance",
                c => new
                    {
                        ServiceInstanceID = c.Int(nullable: false, identity: true),
                        Value = c.String(),
                        ServiceInstanceGuid = c.Guid(nullable: false, defaultValueSql: "newid()"),
                        ServiceID = c.Int(),
                        ServiceTrackID = c.Int(),
                    })
                .PrimaryKey(t => t.ServiceInstanceID)
                .ForeignKey("dbo.Service", t => t.ServiceID)
                .ForeignKey("dbo.ServiceTrack", t => t.ServiceTrackID)
                .Index(t => t.ServiceID, name: "IX_Service_ServiceID")
                .Index(t => t.ServiceTrackID, name: "IX_ServiceTrack_ServiceTrackID");
            
            CreateTable(
                "dbo.ServiceTrack",
                c => new
                    {
                        ServiceTrackID = c.Int(nullable: false, identity: true),
                        ServiceTrackGuid = c.Guid(nullable: false, defaultValueSql: "newid()"),
                        ReferenceNumber = c.Int(nullable: false),
                        ReferencedPerson = c.String(),
                        ReferencedPersonAge = c.Int(nullable: false),
                        ReferencedPersonGender = c.String(),
                        NID = c.Int(nullable: false),
                        NIT = c.Int(nullable: false),
                        District = c.String(),
                        Location = c.String(),
                        Neighborhood = c.String(),
                        CommunalUnit = c.String(),
                        BlockNumber = c.Int(nullable: false),
                        NearByLocation = c.String(),
                        Organization = c.String(),
                        Project = c.String(),
                        Program = c.String(),
                    })
                .PrimaryKey(t => t.ServiceTrackID);
            
            CreateTable(
                "dbo.ServiceTrackSection",
                c => new
                    {
                        ServiceTrackSectionID = c.Int(nullable: false, identity: true),
                        SectionName = c.String(),
                        SectionOrder = c.Int(nullable: false),
                        ServiceTrackSectionGuid = c.Guid(nullable: false, defaultValueSql: "newid()"),
                    })
                .PrimaryKey(t => t.ServiceTrackSectionID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ServiceCategory", "ServiceTrackSectionID", "dbo.ServiceTrackSection");
            DropForeignKey("dbo.ServiceInstance", "ServiceTrackID", "dbo.ServiceTrack");
            DropForeignKey("dbo.ServiceInstance", "ServiceID", "dbo.Service");
            DropForeignKey("dbo.Service", "ServiceCategoryID", "dbo.ServiceCategory");
            DropIndex("dbo.ServiceInstance", "IX_ServiceTrack_ServiceTrackID");
            DropIndex("dbo.ServiceInstance", "IX_Service_ServiceID");
            DropIndex("dbo.Service", "IX_ServiceCategory_ServiceCategoryID");
            DropIndex("dbo.ServiceCategory", "IX_ServiceTrackSection_ServiceTrackSectionID");
            DropTable("dbo.ServiceTrackSection");
            DropTable("dbo.ServiceTrack");
            DropTable("dbo.ServiceInstance");
            DropTable("dbo.Service");
            DropTable("dbo.ServiceCategory");
        }
    }
}
