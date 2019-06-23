namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LocationConfigurations : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Location",
                c => new
                    {
                        LocationID = c.Int(nullable: false, identity: true),
                        LocationName = c.String(nullable: false, maxLength: 50),
                        Address = c.String(),
                        ContactNo = c.String(maxLength: 30),
                        FaxNo = c.String(maxLength: 30),
                        SupervisorName = c.String(maxLength: 60),
                        Location_guid = c.Guid(nullable: false, defaultValueSql: "newid()"),
                        SiteID = c.Int(),
                    })
                .PrimaryKey(t => t.LocationID)
                .ForeignKey("dbo.Site", t => t.SiteID)
                .Index(t => t.SiteID, name: "IX_Site_SiteID");
            
            AddColumn("dbo.Partner", "LocationID", c => c.Int());
            AddColumn("dbo.Child", "LocationID", c => c.Int());
            CreateIndex("dbo.Partner", "LocationID", name: "IX_Location_LocationID");
            CreateIndex("dbo.Child", "LocationID", name: "IX_Location_LocationID");
            AddForeignKey("dbo.Child", "LocationID", "dbo.Location", "LocationID");
            AddForeignKey("dbo.Partner", "LocationID", "dbo.Location", "LocationID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Location", "SiteID", "dbo.Site");
            DropForeignKey("dbo.Partner", "LocationID", "dbo.Location");
            DropForeignKey("dbo.Child", "LocationID", "dbo.Location");
            DropIndex("dbo.Location", "IX_Site_SiteID");
            DropIndex("dbo.Child", "IX_Location_LocationID");
            DropIndex("dbo.Partner", "IX_Location_LocationID");
            DropColumn("dbo.Child", "LocationID");
            DropColumn("dbo.Partner", "LocationID");
            DropTable("dbo.Location");
        }
    }
}
