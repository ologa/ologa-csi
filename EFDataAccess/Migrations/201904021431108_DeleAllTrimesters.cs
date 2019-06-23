namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DeleAllTrimesters : DbMigration
    {
        public override void Up()
        {
            Sql(@"DELETE FROM Trimester");
        }
        
        public override void Down()
        {
        }
    }
}
