namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FixHIVStatusWithWrongUndisclosedReasonsValues : DbMigration
    {
        public override void Up()
        {
            Sql(@"UPDATE   [HIVStatus] set HIVUndisclosedReason = -1 WHERE HIV <> 'U'", false);
        }
        
        public override void Down()
        {
        }
    }
}
