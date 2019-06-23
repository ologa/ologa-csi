namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MAC_Reorder : DbMigration
    {
        public override void Up()
        {
            Sql(@"Update D SET D.[Order] = 1 FROM [Domain] D WHERE D.DomainID = 3", false); // Saúde
            Sql(@"Update D SET D.[Order] = 2 FROM [Domain] D WHERE D.DomainID = 1", false); // Alimentacão/Nutrição
            Sql(@"Update D SET D.[Order] = 3 FROM [Domain] D WHERE D.DomainID = 2", false); // Educação
            Sql(@"Update D SET D.[Order] = 4 FROM [Domain] D WHERE D.DomainID = 4", false); // Protecção e Apoio Legal
            Sql(@"Update D SET D.[Order] = 5 FROM [Domain] D WHERE D.DomainID = 7", false); // Habitação
            Sql(@"Update D SET D.[Order] = 6 FROM [Domain] D WHERE D.DomainID = 5", false); // Apoio Psico social
            Sql(@"Update D SET D.[Order] = 7 FROM [Domain] D WHERE D.DomainID = 6", false); // Fortalecimento Económico
        }
        
        public override void Down()
        {
        }
    }
}
