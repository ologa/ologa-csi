namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LinkServiceToDomain : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Service", "DomainID", c => c.Int());
            CreateIndex("dbo.Service", "DomainID", name: "IX_Domain_DomainID");
            AddForeignKey("dbo.Service", "DomainID", "dbo.Domain", "DomainID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Service", "DomainID", "dbo.Domain");
            DropIndex("dbo.Service", "IX_Domain_DomainID");
            DropColumn("dbo.Service", "DomainID");
        }
    }
}
