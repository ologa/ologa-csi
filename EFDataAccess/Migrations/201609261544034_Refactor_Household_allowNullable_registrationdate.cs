namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Refactor_Household_allowNullable_registrationdate : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.HouseHold", "RegistrationDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.HouseHold", "RegistrationDate", c => c.DateTime(nullable: false));
        }
    }
}
