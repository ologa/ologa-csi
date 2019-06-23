namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserSyncGUIDOnUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.User", "User_sync_guid", c => c.Guid(nullable: true));
        }
        
        public override void Down()
        {
            DropColumn("dbo.User", "User_sync_guid");
        }
    }
}
