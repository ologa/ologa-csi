namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangesToTrimesterModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Trimester",
                c => new
                    {
                        TrimesterID = c.Int(nullable: false, identity: true),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        TrimesterDefinitionID = c.Int(),
                        State = c.Int(nullable: false),
                        CreatedDate = c.DateTime(),
                        LastUpdatedDate = c.DateTime(),
                        SyncState = c.Int(nullable: false),
                        SyncDate = c.DateTime(),
                        SyncGuid = c.Guid(),
                        CreatedUserID = c.Int(),
                        LastUpdatedUserID = c.Int(),
                    })
                .PrimaryKey(t => t.TrimesterID)
                .ForeignKey("dbo.User", t => t.CreatedUserID)
                .ForeignKey("dbo.User", t => t.LastUpdatedUserID)
                .ForeignKey("dbo.TrimesterDefinition", t => t.TrimesterDefinitionID)
                .Index(t => t.TrimesterDefinitionID)
                .Index(t => t.CreatedUserID, name: "IX_CreatedUser_UserID")
                .Index(t => t.LastUpdatedUserID, name: "IX_LastUpdatedUser_UserID");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Trimester", "TrimesterDefinitionID", "dbo.TrimesterDefinition");
            DropForeignKey("dbo.Trimester", "LastUpdatedUserID", "dbo.User");
            DropForeignKey("dbo.Trimester", "CreatedUserID", "dbo.User");
            DropIndex("dbo.Trimester", "IX_LastUpdatedUser_UserID");
            DropIndex("dbo.Trimester", "IX_CreatedUser_UserID");
            DropIndex("dbo.Trimester", new[] { "TrimesterDefinitionID" });
            DropTable("dbo.Trimester");
        }
    }
}