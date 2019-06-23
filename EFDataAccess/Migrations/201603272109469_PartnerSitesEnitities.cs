namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PartnerSitesEnitities : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PartnerLocation",
                c => new
                    {
                        PartnerLocationID = c.Int(nullable: false, identity: true),
                        Active = c.Boolean(nullable: false),
                        PartnerSite_guid = c.Guid(nullable: false),
                        LocationID = c.Int(),
                        PartnerID = c.Int(),
                    })
                .PrimaryKey(t => t.PartnerLocationID)
                .ForeignKey("dbo.Location", t => t.LocationID)
                .ForeignKey("dbo.Partner", t => t.PartnerID)
                .Index(t => t.LocationID, name: "IX_Location_LocationID")
                .Index(t => t.PartnerID, name: "IX_Partner_PartnerID");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.PartnerLocation", "PartnerID", "dbo.Partner");
            DropForeignKey("dbo.PartnerLocation", "LocationID", "dbo.Location");
            DropIndex("dbo.PartnerLocation", "IX_Partner_PartnerID");
            DropIndex("dbo.PartnerLocation", "IX_Location_LocationID");
            DropTable("dbo.PartnerLocation");
        }
    }
}
