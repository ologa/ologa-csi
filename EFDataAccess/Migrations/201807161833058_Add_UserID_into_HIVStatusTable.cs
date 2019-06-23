namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_UserID_into_HIVStatusTable : DbMigration
    {
        public override void Up()
        {
            RenameIndex(table: "dbo.HIVStatus", name: "IX_User_UserID", newName: "IX_UserID");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.HIVStatus", name: "IX_UserID", newName: "IX_User_UserID");
        }
    }
}
