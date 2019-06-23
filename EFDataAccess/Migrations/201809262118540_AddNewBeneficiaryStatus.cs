namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddNewBeneficiaryStatus : DbMigration
    {
        public override void Up()
        {
            Sql(@"INSERT INTO  [ChildStatus] (Description,state,SyncState) VALUES ('Eliminado',0,0)", false);
        }
        
        public override void Down()
        {
            Sql(@"DELETE FROM [ChildStatus] WHERE Description = 'Eliminado'", false);
        }
    }
}
