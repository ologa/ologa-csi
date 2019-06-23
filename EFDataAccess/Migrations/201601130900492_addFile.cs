namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addFile : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Answer", "FileID", c => c.Int());
            AddColumn("dbo.Domain", "FileID", c => c.Int());
            CreateIndex("dbo.Answer", "FileID", name: "IX_File_FileId");
            CreateIndex("dbo.Domain", "FileID", name: "IX_File_FileId");
            AddForeignKey("dbo.Domain", "FileID", "dbo.File", "FileId");
            AddForeignKey("dbo.Answer", "FileID", "dbo.File", "FileId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Answer", "FileID", "dbo.File");
            DropForeignKey("dbo.Domain", "FileID", "dbo.File");
            DropIndex("dbo.Domain", "IX_File_FileId");
            DropIndex("dbo.Answer", "IX_File_FileId");
            DropColumn("dbo.Domain", "FileID");
            DropColumn("dbo.Answer", "FileID");
        }
    }
}
