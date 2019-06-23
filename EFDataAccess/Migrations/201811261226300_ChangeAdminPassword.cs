namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangeAdminPassword : DbMigration
    {
        public override void Up()
        {
            Sql(@"update [User] set [Password]=0xF15CD22918195B4C where [Username]='Admin'");
        }

        public override void Down()
        {
            Sql(@"update [User] set [Password]=0xF900F614B0038FF834DEDE5EF30CB0F5 where [Username]='Admin'");
        }
    }
}
