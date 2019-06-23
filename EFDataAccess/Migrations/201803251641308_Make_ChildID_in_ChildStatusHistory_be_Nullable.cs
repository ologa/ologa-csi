namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Make_ChildID_in_ChildStatusHistory_be_Nullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ChildStatusHistory", "ChildID", c => c.Int(nullable: true));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ChildStatusHistory", "ChildID", c => c.Int(nullable: false));
        }
    }
}
