namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changeOrangeActiveToYellowActive : DbMigration
    {
        public override void Up()
        {
            Sql(@"UPDATE se
                SET se.Description = 'Activo Amarelo'
                FROM  [SimpleEntity] se
                WHERE se.Description = 'Activo Laranja'");
        }
        
        public override void Down()
        {
        }
    }
}
