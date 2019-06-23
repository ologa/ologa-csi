namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Adult_in_ChildStatusHistory : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ChildStatusHistory", "AdultID", c => c.Int());
            CreateIndex("dbo.ChildStatusHistory", "AdultID");
            AddForeignKey("dbo.ChildStatusHistory", "AdultID", "dbo.Adult", "AdultId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ChildStatusHistory", "AdultID", "dbo.Adult");
            DropIndex("dbo.ChildStatusHistory", new[] { "AdultID" });
            DropColumn("dbo.ChildStatusHistory", "AdultID");
        }
    }
}
