namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_Husband_or_Wife_into_KingshipToFamilyHead : DbMigration
    {
        public override void Up()
        {
            Sql(@"INSERT INTO[dbo].[SimpleEntity] ([Type],[Code],[Description]) VALUES('degree-of-kinship',6,'Esposo(a)')", false); // Adicionar Esposo(a) como relação com chefe da família

            Sql(@"UPDATE[dbo].[SimpleEntity] SET [Code] = '0' + [Code] WHERE LEN([Code]) = 1", false); // Adicionar 0 à esquerda de números com 1 digito. Ex: 3 = 03, 1 = 01, 9 = 09

        }
        
        public override void Down()
        {
            Sql(@"DELETE FROM[dbo].[SimpleEntity] WHERE Description = 'Esposo(a)'", false); // Adicionar 0 à esquerda de números com 1 digito. Ex: 3 = 03, 1 = 01, 9 = 09
        }
    }
}
