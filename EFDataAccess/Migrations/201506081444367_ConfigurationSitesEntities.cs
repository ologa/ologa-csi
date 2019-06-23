namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ConfigurationSitesEntities : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Config",
                c => new
                    {
                        ConfigID = c.Int(nullable: false, identity: true),
                        Config_Guid = c.Guid(nullable: false, defaultValueSql: "newid()"),
                        Detail = c.String(),
                        ConfigTypeID = c.Int(),
                    })
                .PrimaryKey(t => t.ConfigID)
                .ForeignKey("dbo.ConfigType", t => t.ConfigTypeID)
                .Index(t => t.ConfigTypeID, name: "IX_ConfigType_ConfigTypeID");
            
            CreateTable(
                "dbo.ConfigType",
                c => new
                    {
                        ConfigTypeID = c.Int(nullable: false, identity: true),
                        ConfigType_Guid = c.Guid(nullable: false, defaultValueSql: "newid()"),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.ConfigTypeID);
            
            CreateTable(
                "dbo.SiteConfig",
                c => new
                    {
                        SiteConfigID = c.Int(nullable: false, identity: true),
                        SiteConfig_Guid = c.Guid(nullable: false, defaultValueSql: "newid()"),
                        ConfigID = c.Int(),
                        SiteID = c.Int(),
                    })
                .PrimaryKey(t => t.SiteConfigID)
                .ForeignKey("dbo.Config", t => t.ConfigID)
                .ForeignKey("dbo.Site", t => t.SiteID)
                .Index(t => t.ConfigID, name: "IX_Config_ConfigID")
                .Index(t => t.SiteID, name: "IX_Site_SiteID");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SiteConfig", "SiteID", "dbo.Site");
            DropForeignKey("dbo.SiteConfig", "ConfigID", "dbo.Config");
            DropForeignKey("dbo.Config", "ConfigTypeID", "dbo.ConfigType");
            DropIndex("dbo.SiteConfig", "IX_Site_SiteID");
            DropIndex("dbo.SiteConfig", "IX_Config_ConfigID");
            DropIndex("dbo.Config", "IX_ConfigType_ConfigTypeID");
            DropTable("dbo.SiteConfig");
            DropTable("dbo.ConfigType");
            DropTable("dbo.Config");
        }
    }
}
