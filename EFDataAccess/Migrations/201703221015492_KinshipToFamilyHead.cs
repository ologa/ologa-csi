namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class KinshipToFamilyHead : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Adult", "OtherKinshipToFamilyHead", c => c.String());
            AddColumn("dbo.Adult", "KinshipToFamilyHeadID", c => c.Int());
            AddColumn("dbo.Child", "OtherKinshipToFamilyHead", c => c.String());
            AddColumn("dbo.Child", "KinshipToFamilyHeadID", c => c.Int());
            CreateIndex("dbo.Adult", "KinshipToFamilyHeadID", name: "IX_KinshipToFamilyHead_SimpleEntityID");
            CreateIndex("dbo.Child", "KinshipToFamilyHeadID", name: "IX_KinshipToFamilyHead_SimpleEntityID");
            AddForeignKey("dbo.Child", "KinshipToFamilyHeadID", "dbo.SimpleEntity", "SimpleEntityID");
            AddForeignKey("dbo.Adult", "KinshipToFamilyHeadID", "dbo.SimpleEntity", "SimpleEntityID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Adult", "KinshipToFamilyHeadID", "dbo.SimpleEntity");
            DropForeignKey("dbo.Child", "KinshipToFamilyHeadID", "dbo.SimpleEntity");
            DropIndex("dbo.Child", "IX_KinshipToFamilyHead_SimpleEntityID");
            DropIndex("dbo.Adult", "IX_KinshipToFamilyHead_SimpleEntityID");
            DropColumn("dbo.Child", "KinshipToFamilyHeadID");
            DropColumn("dbo.Child", "OtherKinshipToFamilyHead");
            DropColumn("dbo.Adult", "KinshipToFamilyHeadID");
            DropColumn("dbo.Adult", "OtherKinshipToFamilyHead");
        }
    }
}
