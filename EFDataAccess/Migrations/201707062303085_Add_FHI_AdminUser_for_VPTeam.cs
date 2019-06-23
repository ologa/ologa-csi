namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_FHI_AdminUser_for_VPTeam : DbMigration
    {
        public override void Up()
        {
            Sql(@"INSERT INTO[dbo].[User] ([Username],[Password],[FirstName],[LastName],[Admin],[DefSite],[LoggedON],[IsOCBUser]) 
                VALUES ('fhiadmin',0x2D5C47A772DBA64C8107E12C8D32FB02,'FHI','Admin' ,1 ,1 ,0 ,0)", false); 
            // Admin User para questoes emergencia VP
            // O código da pass é: fhi1234
        }
        
        public override void Down()
        {
            Sql(@"DELETE FROM [dbo].[User] WHERE Username = 'fhiadmin'", false);
        }
    }
}
