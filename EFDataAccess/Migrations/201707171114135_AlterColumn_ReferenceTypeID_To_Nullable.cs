namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AlterColumn_ReferenceTypeID_To_Nullable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Reference", "ReferenceTypeID", "dbo.ReferenceType");
            DropIndex("dbo.Reference", new[] { "ReferenceTypeID" });
            AlterColumn("dbo.Reference", "ReferenceTypeID", c => c.Int());
            CreateIndex("dbo.Reference", "ReferenceTypeID");
            AddForeignKey("dbo.Reference", "ReferenceTypeID", "dbo.ReferenceType", "ReferenceTypeID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Reference", "ReferenceTypeID", "dbo.ReferenceType");
            DropIndex("dbo.Reference", new[] { "ReferenceTypeID" });
            AlterColumn("dbo.Reference", "ReferenceTypeID", c => c.Int(nullable: false));
            CreateIndex("dbo.Reference", "ReferenceTypeID");
            AddForeignKey("dbo.Reference", "ReferenceTypeID", "dbo.ReferenceType", "ReferenceTypeID", cascadeDelete: true);
        }
    }
}
