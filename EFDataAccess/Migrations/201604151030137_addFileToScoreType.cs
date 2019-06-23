namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addFileToScoreType : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ScoreType", "FileID", c => c.Int());
            CreateIndex("dbo.ScoreType", "FileID", name: "IX_File_FileId");
            AddForeignKey("dbo.ScoreType", "FileID", "dbo.File", "FileId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ScoreType", "FileID", "dbo.File");
            DropIndex("dbo.ScoreType", "IX_File_FileId");
            DropColumn("dbo.ScoreType", "FileID");
        }
    }
}
