namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_RegistrationDate_for_Beneficiaries : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Adult", "RegistrationDate", c => c.DateTime());
            AddColumn("dbo.Adult", "RegistrationDateDifferentFromHouseholdDate", c => c.Boolean(nullable: false));
            AddColumn("dbo.Child", "RegistrationDate", c => c.DateTime());
            AddColumn("dbo.Child", "RegistrationDateDifferentFromHouseholdDate", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Child", "RegistrationDateDifferentFromHouseholdDate");
            DropColumn("dbo.Child", "RegistrationDate");
            DropColumn("dbo.Adult", "RegistrationDateDifferentFromHouseholdDate");
            DropColumn("dbo.Adult", "RegistrationDate");
        }
    }
}
