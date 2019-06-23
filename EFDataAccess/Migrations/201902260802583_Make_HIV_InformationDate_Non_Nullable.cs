namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Make_HIV_InformationDate_Non_Nullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.HIVStatus", "InformationDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.HIVStatus", "InformationDate", c => c.DateTime(nullable: false));
        }
    }
}
