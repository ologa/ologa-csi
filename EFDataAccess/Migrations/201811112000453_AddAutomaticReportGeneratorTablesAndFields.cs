namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAutomaticReportGeneratorTablesAndFields : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Query",
                c => new
                    {
                        QueryID = c.Int(nullable: false, identity: true),
                        Query_guid = c.Guid(nullable: false, defaultValueSql: "newid()"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        CreatedUserID = c.Int(),
                        LastUpdatedUserID = c.Int(),
                        Name = c.String(),
                        Code = c.String(),
                        Sql = c.String(),
                        State = c.Int(nullable: false),
                        CreatedDate = c.DateTime(),
                        LastUpdatedDate = c.DateTime(),
                        SyncState = c.Int(nullable: false),
                        SyncDate = c.DateTime(),
                        SyncGuid = c.Guid(),
                    })
                .PrimaryKey(t => t.QueryID)
                .ForeignKey("dbo.User", t => t.CreatedUserID)
                .ForeignKey("dbo.User", t => t.LastUpdatedUserID)
                .Index(t => t.CreatedUserID)
                .Index(t => t.LastUpdatedUserID);
            
            CreateTable(
                "dbo.ReportData",
                c => new
                    {
                        ReportDataID = c.Int(nullable: false, identity: true),
                        ReportData_guid = c.Guid(nullable: false, defaultValueSql: "newid()"),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        CreatedUserID = c.Int(),
                        LastUpdatedUserID = c.Int(),
                        ExecutionNumber = c.Int(nullable: false),
                        QueryCode = c.String(),
                        SiteName = c.String(),
                        Province = c.String(),
                        District = c.String(),
                        Field1 = c.String(),
                        Field2 = c.String(),
                        Field3 = c.String(),
                        Field4 = c.String(),
                        Field5 = c.String(),
                        Field6 = c.String(),
                        Field7 = c.String(),
                        Field8 = c.String(),
                        Field9 = c.String(),
                        Field10 = c.String(),
                        Field11 = c.String(),
                        Field12 = c.String(),
                        Field13 = c.String(),
                        Field14 = c.String(),
                        Field15 = c.String(),
                        Field16 = c.String(),
                        Field17 = c.String(),
                        Field18 = c.String(),
                        Field19 = c.String(),
                        Field20 = c.String(),
                        State = c.Int(nullable: false),
                        CreatedDate = c.DateTime(),
                        LastUpdatedDate = c.DateTime(),
                        SyncState = c.Int(nullable: false),
                        SyncDate = c.DateTime(),
                        SyncGuid = c.Guid(),
                    })
                .PrimaryKey(t => t.ReportDataID)
                .ForeignKey("dbo.User", t => t.CreatedUserID)
                .ForeignKey("dbo.User", t => t.LastUpdatedUserID)
                .Index(t => t.CreatedUserID)
                .Index(t => t.LastUpdatedUserID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ReportData", "LastUpdatedUserID", "dbo.User");
            DropForeignKey("dbo.ReportData", "CreatedUserID", "dbo.User");
            DropForeignKey("dbo.Query", "LastUpdatedUserID", "dbo.User");
            DropForeignKey("dbo.Query", "CreatedUserID", "dbo.User");
            DropIndex("dbo.ReportData", new[] { "LastUpdatedUserID" });
            DropIndex("dbo.ReportData", new[] { "CreatedUserID" });
            DropIndex("dbo.Query", new[] { "LastUpdatedUserID" });
            DropIndex("dbo.Query", new[] { "CreatedUserID" });
            DropTable("dbo.ReportData");
            DropTable("dbo.Query");
        }
    }
}
