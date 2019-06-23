namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeTrimesterToTrimesterDefinition : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Trimester", "CreatedUserID", "dbo.User");
            DropForeignKey("dbo.Trimester", "LastUpdatedUserID", "dbo.User");
            DropIndex("dbo.Trimester", "IX_CreatedUser_UserID");
            DropIndex("dbo.Trimester", "IX_LastUpdatedUser_UserID");
            CreateTable(
                "dbo.TrimesterDefinition",
                c => new
                    {
                        TrimesterDefinitionID = c.Int(nullable: false, identity: true),
                        FirstDay = c.Int(nullable: false),
                        LastDay = c.Int(nullable: false),
                        FirstMonth = c.Int(nullable: false),
                        LastMonth = c.Int(nullable: false),
                        TrimesterSequence = c.Int(nullable: false),
                        State = c.Int(nullable: false),
                        CreatedDate = c.DateTime(),
                        LastUpdatedDate = c.DateTime(),
                        SyncState = c.Int(nullable: false),
                        SyncDate = c.DateTime(),
                        SyncGuid = c.Guid(),
                        CreatedUserID = c.Int(),
                        LastUpdatedUserID = c.Int(),
                    })
                .PrimaryKey(t => t.TrimesterDefinitionID)
                .ForeignKey("dbo.User", t => t.CreatedUserID)
                .ForeignKey("dbo.User", t => t.LastUpdatedUserID)
                .Index(t => t.CreatedUserID, name: "IX_CreatedUser_UserID")
                .Index(t => t.LastUpdatedUserID, name: "IX_LastUpdatedUser_UserID");
            
            DropTable("dbo.Trimester");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Trimester",
                c => new
                    {
                        TrimesterID = c.Int(nullable: false, identity: true),
                        FirstDay = c.Int(nullable: false),
                        LastDay = c.Int(nullable: false),
                        FirstMonth = c.Int(nullable: false),
                        LastMonth = c.Int(nullable: false),
                        TrimesterSequence = c.Int(nullable: false),
                        State = c.Int(nullable: false),
                        CreatedDate = c.DateTime(),
                        LastUpdatedDate = c.DateTime(),
                        SyncState = c.Int(nullable: false),
                        SyncDate = c.DateTime(),
                        SyncGuid = c.Guid(),
                        CreatedUserID = c.Int(),
                        LastUpdatedUserID = c.Int(),
                    })
                .PrimaryKey(t => t.TrimesterID);
            
            DropForeignKey("dbo.TrimesterDefinition", "LastUpdatedUserID", "dbo.User");
            DropForeignKey("dbo.TrimesterDefinition", "CreatedUserID", "dbo.User");
            DropIndex("dbo.TrimesterDefinition", "IX_LastUpdatedUser_UserID");
            DropIndex("dbo.TrimesterDefinition", "IX_CreatedUser_UserID");
            DropTable("dbo.TrimesterDefinition");
            CreateIndex("dbo.Trimester", "LastUpdatedUserID", name: "IX_LastUpdatedUser_UserID");
            CreateIndex("dbo.Trimester", "CreatedUserID", name: "IX_CreatedUser_UserID");
            AddForeignKey("dbo.Trimester", "LastUpdatedUserID", "dbo.User", "UserID");
            AddForeignKey("dbo.Trimester", "CreatedUserID", "dbo.User", "UserID");
        }
    }
}
