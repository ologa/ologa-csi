namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddKinshipToFamilyHeadOnChildAndAdult : DbMigration
    {
        public override void Up()
        {
            RenameIndex(table: "dbo.Adult", name: "IX_KinshipToFamilyHead_SimpleEntityID", newName: "IX_KinshipToFamilyHeadID");
            RenameIndex(table: "dbo.Child", name: "IX_KinshipToFamilyHead_SimpleEntityID", newName: "IX_KinshipToFamilyHeadID");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Child", name: "IX_KinshipToFamilyHeadID", newName: "IX_KinshipToFamilyHead_SimpleEntityID");
            RenameIndex(table: "dbo.Adult", name: "IX_KinshipToFamilyHeadID", newName: "IX_KinshipToFamilyHead_SimpleEntityID");
        }
    }
}
