namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddBeneficiaryCodeOnBeneficiaries : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Adult", "Code", c => c.String(nullable: false));
            AddColumn("dbo.Child", "Code", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Child", "Code");
            DropColumn("dbo.Adult", "Code");
        }
    }
}
