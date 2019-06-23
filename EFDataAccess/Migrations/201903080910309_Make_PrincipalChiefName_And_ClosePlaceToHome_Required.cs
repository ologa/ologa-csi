namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Make_PrincipalChiefName_And_ClosePlaceToHome_Required : DbMigration
    {
        public override void Up()
        {
            Sql(@"UPDATE [Household] SET [PrincipalChiefName] = '' WHERE [PrincipalChiefName] IS NULL");
            Sql(@"UPDATE [Household] SET [ClosePlaceToHome] = '' WHERE [ClosePlaceToHome] IS NULL");
            AlterColumn("dbo.HouseHold", "PrincipalChiefName", c => c.String(nullable: false));
            AlterColumn("dbo.HouseHold", "ClosePlaceToHome", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.HouseHold", "ClosePlaceToHome", c => c.String());
            AlterColumn("dbo.HouseHold", "PrincipalChiefName", c => c.String());
        }
    }
}
