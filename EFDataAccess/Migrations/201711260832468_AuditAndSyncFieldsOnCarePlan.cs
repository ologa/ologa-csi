namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AuditAndSyncFieldsOnCarePlan : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.User", "SyncGuid", c => c.Guid());
            AddColumn("dbo.Partner", "SyncDate", c => c.DateTime());
            AddColumn("dbo.Partner", "SyncGuid", c => c.Guid());
            AddColumn("dbo.Partner", "CreatedDate", c => c.DateTime());
            AddColumn("dbo.Partner", "LastUpdatedDate", c => c.DateTime());
            AddColumn("dbo.Partner", "CreatedUserID", c => c.Int());
            AddColumn("dbo.Partner", "LastUpdatedUserID", c => c.Int());
            AddColumn("dbo.CarePlan", "SyncDate", c => c.DateTime());
            AddColumn("dbo.CarePlan", "SyncGuid", c => c.Guid());
            AddColumn("dbo.CarePlanDomain", "SyncDate", c => c.DateTime());
            AddColumn("dbo.CarePlanDomain", "SyncGuid", c => c.Guid());
            AddColumn("dbo.CarePlanDomain", "CreatedDate", c => c.DateTime());
            AddColumn("dbo.CarePlanDomain", "LastUpdatedDate", c => c.DateTime());
            AddColumn("dbo.CarePlanDomain", "CreatedUserID", c => c.Int());
            AddColumn("dbo.CarePlanDomain", "LastUpdatedUserID", c => c.Int());
            AddColumn("dbo.CarePlanDomainSupportService", "SyncDate", c => c.DateTime());
            AddColumn("dbo.CarePlanDomainSupportService", "SyncGuid", c => c.Guid());
            AddColumn("dbo.CarePlanDomainSupportService", "CreatedDate", c => c.DateTime());
            AddColumn("dbo.CarePlanDomainSupportService", "LastUpdatedDate", c => c.DateTime());
            AddColumn("dbo.CarePlanDomainSupportService", "CreatedUserID", c => c.Int());
            AddColumn("dbo.CarePlanDomainSupportService", "LastUpdatedUserID", c => c.Int());
            AddColumn("dbo.CSIDomain", "CreatedDate", c => c.DateTime());
            AddColumn("dbo.CSIDomain", "LastUpdatedDate", c => c.DateTime());
            AddColumn("dbo.CSIDomain", "CreatedUserID", c => c.Int());
            AddColumn("dbo.CSIDomain", "LastUpdatedUserID", c => c.Int());
            AddColumn("dbo.Tasks", "SyncDate", c => c.DateTime());
            AddColumn("dbo.Tasks", "SyncGuid", c => c.Guid());
            AddColumn("dbo.Tasks", "CreatedDate", c => c.DateTime());
            AddColumn("dbo.Tasks", "LastUpdatedDate", c => c.DateTime());
            AddColumn("dbo.Tasks", "CreatedUserID", c => c.Int());
            AddColumn("dbo.Tasks", "LastUpdatedUserID", c => c.Int());
            AlterColumn("dbo.CarePlan", "CreatedDate", c => c.DateTime());
            AlterColumn("dbo.CarePlan", "LastUpdatedDate", c => c.DateTime());
            CreateIndex("dbo.Partner", "CreatedUserID", name: "IX_CreatedUser_UserID");
            CreateIndex("dbo.Partner", "LastUpdatedUserID", name: "IX_LastUpdatedUser_UserID");
            CreateIndex("dbo.CarePlanDomain", "CreatedUserID", name: "IX_CreatedUser_UserID");
            CreateIndex("dbo.CarePlanDomain", "LastUpdatedUserID", name: "IX_LastUpdatedUser_UserID");
            CreateIndex("dbo.CarePlanDomainSupportService", "CreatedUserID", name: "IX_CreatedUser_UserID");
            CreateIndex("dbo.CarePlanDomainSupportService", "LastUpdatedUserID", name: "IX_LastUpdatedUser_UserID");
            CreateIndex("dbo.CSIDomain", "CreatedUserID", name: "IX_CreatedUser_UserID");
            CreateIndex("dbo.CSIDomain", "LastUpdatedUserID", name: "IX_LastUpdatedUser_UserID");
            CreateIndex("dbo.Tasks", "CreatedUserID", name: "IX_CreatedUser_UserID");
            CreateIndex("dbo.Tasks", "LastUpdatedUserID", name: "IX_LastUpdatedUser_UserID");
            AddForeignKey("dbo.CSIDomain", "CreatedUserID", "dbo.User", "UserID");
            AddForeignKey("dbo.CSIDomain", "LastUpdatedUserID", "dbo.User", "UserID");
            AddForeignKey("dbo.CarePlanDomainSupportService", "CreatedUserID", "dbo.User", "UserID");
            AddForeignKey("dbo.CarePlanDomainSupportService", "LastUpdatedUserID", "dbo.User", "UserID");
            AddForeignKey("dbo.Tasks", "CreatedUserID", "dbo.User", "UserID");
            AddForeignKey("dbo.Tasks", "LastUpdatedUserID", "dbo.User", "UserID");
            AddForeignKey("dbo.CarePlanDomain", "CreatedUserID", "dbo.User", "UserID");
            AddForeignKey("dbo.CarePlanDomain", "LastUpdatedUserID", "dbo.User", "UserID");
            AddForeignKey("dbo.Partner", "CreatedUserID", "dbo.User", "UserID");
            AddForeignKey("dbo.Partner", "LastUpdatedUserID", "dbo.User", "UserID");
            DropColumn("dbo.User", "User_sync_guid");
        }
        
        public override void Down()
        {
            AddColumn("dbo.User", "User_sync_guid", c => c.Guid());
            DropForeignKey("dbo.Partner", "LastUpdatedUserID", "dbo.User");
            DropForeignKey("dbo.Partner", "CreatedUserID", "dbo.User");
            DropForeignKey("dbo.CarePlanDomain", "LastUpdatedUserID", "dbo.User");
            DropForeignKey("dbo.CarePlanDomain", "CreatedUserID", "dbo.User");
            DropForeignKey("dbo.Tasks", "LastUpdatedUserID", "dbo.User");
            DropForeignKey("dbo.Tasks", "CreatedUserID", "dbo.User");
            DropForeignKey("dbo.CarePlanDomainSupportService", "LastUpdatedUserID", "dbo.User");
            DropForeignKey("dbo.CarePlanDomainSupportService", "CreatedUserID", "dbo.User");
            DropForeignKey("dbo.CSIDomain", "LastUpdatedUserID", "dbo.User");
            DropForeignKey("dbo.CSIDomain", "CreatedUserID", "dbo.User");
            DropIndex("dbo.Tasks", "IX_LastUpdatedUser_UserID");
            DropIndex("dbo.Tasks", "IX_CreatedUser_UserID");
            DropIndex("dbo.CSIDomain", "IX_LastUpdatedUser_UserID");
            DropIndex("dbo.CSIDomain", "IX_CreatedUser_UserID");
            DropIndex("dbo.CarePlanDomainSupportService", "IX_LastUpdatedUser_UserID");
            DropIndex("dbo.CarePlanDomainSupportService", "IX_CreatedUser_UserID");
            DropIndex("dbo.CarePlanDomain", "IX_LastUpdatedUser_UserID");
            DropIndex("dbo.CarePlanDomain", "IX_CreatedUser_UserID");
            DropIndex("dbo.Partner", "IX_LastUpdatedUser_UserID");
            DropIndex("dbo.Partner", "IX_CreatedUser_UserID");
            AlterColumn("dbo.CarePlan", "LastUpdatedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.CarePlan", "CreatedDate", c => c.DateTime(nullable: false));
            DropColumn("dbo.Tasks", "LastUpdatedUserID");
            DropColumn("dbo.Tasks", "CreatedUserID");
            DropColumn("dbo.Tasks", "LastUpdatedDate");
            DropColumn("dbo.Tasks", "CreatedDate");
            DropColumn("dbo.Tasks", "SyncGuid");
            DropColumn("dbo.Tasks", "SyncDate");
            DropColumn("dbo.CSIDomain", "LastUpdatedUserID");
            DropColumn("dbo.CSIDomain", "CreatedUserID");
            DropColumn("dbo.CSIDomain", "LastUpdatedDate");
            DropColumn("dbo.CSIDomain", "CreatedDate");
            DropColumn("dbo.CarePlanDomainSupportService", "LastUpdatedUserID");
            DropColumn("dbo.CarePlanDomainSupportService", "CreatedUserID");
            DropColumn("dbo.CarePlanDomainSupportService", "LastUpdatedDate");
            DropColumn("dbo.CarePlanDomainSupportService", "CreatedDate");
            DropColumn("dbo.CarePlanDomainSupportService", "SyncGuid");
            DropColumn("dbo.CarePlanDomainSupportService", "SyncDate");
            DropColumn("dbo.CarePlanDomain", "LastUpdatedUserID");
            DropColumn("dbo.CarePlanDomain", "CreatedUserID");
            DropColumn("dbo.CarePlanDomain", "LastUpdatedDate");
            DropColumn("dbo.CarePlanDomain", "CreatedDate");
            DropColumn("dbo.CarePlanDomain", "SyncGuid");
            DropColumn("dbo.CarePlanDomain", "SyncDate");
            DropColumn("dbo.CarePlan", "SyncGuid");
            DropColumn("dbo.CarePlan", "SyncDate");
            DropColumn("dbo.Partner", "LastUpdatedUserID");
            DropColumn("dbo.Partner", "CreatedUserID");
            DropColumn("dbo.Partner", "LastUpdatedDate");
            DropColumn("dbo.Partner", "CreatedDate");
            DropColumn("dbo.Partner", "SyncGuid");
            DropColumn("dbo.Partner", "SyncDate");
            DropColumn("dbo.User", "SyncGuid");
        }
    }
}
