namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Task_Allow_nullable_CompleteDate : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Tasks", "CompleteDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Tasks", "CompleteDate", c => c.DateTime(nullable: false));
        }
    }
}
