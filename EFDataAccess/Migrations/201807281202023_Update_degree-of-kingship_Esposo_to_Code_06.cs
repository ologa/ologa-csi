namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update_degreeofkingship_Esposo_to_Code_06 : DbMigration
    {
        public override void Up()
        {
            Sql(@"UPDATE s
                SET s.Code = '06'
                FROM SimpleEntity s
                WHERE s.description ='Esposo(a)'
                AND s.type = 'degree-of-kinship'", false);
        }

        public override void Down()
        {
        }
    }
}
