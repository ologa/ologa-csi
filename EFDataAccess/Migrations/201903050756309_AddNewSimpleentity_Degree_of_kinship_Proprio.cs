namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddNewSimpleentity_Degree_of_kinship_Proprio : DbMigration
    {
        public override void Up()
        {
            Sql(@"insert into [simpleentity] (Type,Code,Description,CreatedDate,LastUpdatedDate,CreatedUserID,LastUpdatedUserID,State,SyncState)
                 values                      ('degree-of-kinship','07','Próprio',SYSDATETIME(),SYSDATETIME(),1,1,0,0)");
        }
        
        public override void Down()
        {
            Sql("delete[simpleentity] where Description = 'Próprio'");
        }
    }
}
