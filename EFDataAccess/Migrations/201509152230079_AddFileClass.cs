namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddFileClass : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.File",
                c => new
                    {
                        FileId = c.Int(nullable: false, identity: true),
                        FileName = c.String(),
                        Content = c.Binary(),
                    })
                .PrimaryKey(t => t.FileId);
            
            AddColumn("dbo.Question", "FileID", c => c.Int());
            CreateIndex("dbo.Question", "FileID", name: "IX_File_FileId");
            AddForeignKey("dbo.Question", "FileID", "dbo.File", "FileId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Question", "FileID", "dbo.File");
            DropIndex("dbo.Question", "IX_File_FileId");
            DropColumn("dbo.Question", "FileID");
            DropTable("dbo.File");
        }
    }
}
