namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixMUAC_AgeInterval_Description : DbMigration
    {
        public override void Up()
        {
            Sql(@"UPDATE rv
                SET rv.[TypeDescription] = 'MUAC (6-59 meses)'
                FROM [SupportServiceType] rv
                WHERE rv.[TypeDescription] = 'MUAC (6-69 meses)'");
        }
        
        public override void Down()
        {
        }
    }
}
