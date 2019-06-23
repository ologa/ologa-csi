namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChildSyncChanges : DbMigration
    {
        public override void Up()
        {
            RenameIndex(table: "dbo.Child", name: "IX_OVCType_OVCTypeID", newName: "IX_OVCTypeID");
            RenameIndex(table: "dbo.Child", name: "IX_OrgUnit_OrgUnitID", newName: "IX_OrgUnitID");
            RenameIndex(table: "dbo.ChildStatusHistory", name: "IX_Child_ChildID", newName: "IX_ChildID");
            RenameIndex(table: "dbo.ChildStatusHistory", name: "IX_ChildStatus_StatusID", newName: "IX_ChildStatusID");
            RenameIndex(table: "dbo.ChildStatusHistory", name: "IX_CreatedUser_UserID", newName: "IX_CreatedUserID");
            AddColumn("dbo.Child", "SyncDate", c => c.DateTime());
            AddColumn("dbo.Child", "SyncGuid", c => c.Guid());
            AddColumn("dbo.ChildStatusHistory", "SyncDate", c => c.DateTime());
            AddColumn("dbo.ChildStatusHistory", "SyncGuid", c => c.Guid());
            AddColumn("dbo.HIVStatus", "SyncDate", c => c.DateTime());
            AddColumn("dbo.HIVStatus", "SyncGuid", c => c.Guid());
            AlterColumn("dbo.Child", "CreatedDate", c => c.DateTime());
            AlterColumn("dbo.Child", "LastUpdatedDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Child", "LastUpdatedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Child", "CreatedDate", c => c.DateTime(nullable: false));
            DropColumn("dbo.HIVStatus", "SyncGuid");
            DropColumn("dbo.HIVStatus", "SyncDate");
            DropColumn("dbo.ChildStatusHistory", "SyncGuid");
            DropColumn("dbo.ChildStatusHistory", "SyncDate");
            DropColumn("dbo.Child", "SyncGuid");
            DropColumn("dbo.Child", "SyncDate");
            RenameIndex(table: "dbo.ChildStatusHistory", name: "IX_CreatedUserID", newName: "IX_CreatedUser_UserID");
            RenameIndex(table: "dbo.ChildStatusHistory", name: "IX_ChildStatusID", newName: "IX_ChildStatus_StatusID");
            RenameIndex(table: "dbo.ChildStatusHistory", name: "IX_ChildID", newName: "IX_Child_ChildID");
            RenameIndex(table: "dbo.Child", name: "IX_OrgUnitID", newName: "IX_OrgUnit_OrgUnitID");
            RenameIndex(table: "dbo.Child", name: "IX_OVCTypeID", newName: "IX_OVCType_OVCTypeID");
        }
    }
}
