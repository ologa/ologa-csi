namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class change_Chass_to_Clinic_Partner : DbMigration
    {
        public override void Up()
        {
            Sql(@"UPDATE  [SimpleEntity] SET Description = 'Parceiro Clínico' WHERE Description = 'CHASS'", false);
        }
        
        public override void Down()
        {
            Sql(@"UPDATE  [SimpleEntity] SET Description = 'CHASS' WHERE Description = 'Parceiro Clínico'", false);
        }
    }
}
