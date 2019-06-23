namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Remove_Codes : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Adult", "Code", c => c.String());
            AlterColumn("dbo.Child", "Code", c => c.String());
            AlterColumn("dbo.Partner", "Code", c => c.String());
            AlterColumn("dbo.HouseHold", "Code", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.HouseHold", "Code", c => c.String(nullable: false));
            AlterColumn("dbo.Partner", "Code", c => c.String(nullable: false));
            AlterColumn("dbo.Child", "Code", c => c.String(nullable: false));
            AlterColumn("dbo.Adult", "Code", c => c.String(nullable: false));
        }
    }
}
