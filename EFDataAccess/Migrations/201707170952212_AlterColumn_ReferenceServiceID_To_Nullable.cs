namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AlterColumn_ReferenceServiceID_To_Nullable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Reference", "ReferenceServiceID", "dbo.ReferenceService");
            DropIndex("dbo.Reference", new[] { "ReferenceServiceID" });
            AlterColumn("dbo.Reference", "ReferenceServiceID", c => c.Int());
            CreateIndex("dbo.Reference", "ReferenceServiceID");
            AddForeignKey("dbo.Reference", "ReferenceServiceID", "dbo.ReferenceService", "ReferenceServiceID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Reference", "ReferenceServiceID", "dbo.ReferenceService");
            DropIndex("dbo.Reference", new[] { "ReferenceServiceID" });
            AlterColumn("dbo.Reference", "ReferenceServiceID", c => c.Int(nullable: false));
            CreateIndex("dbo.Reference", "ReferenceServiceID");
            AddForeignKey("dbo.Reference", "ReferenceServiceID", "dbo.ReferenceService", "ReferenceServiceID", cascadeDelete: true);
        }
    }
}
