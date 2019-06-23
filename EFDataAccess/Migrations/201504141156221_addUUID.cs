namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addUUID : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Answer", "AnswerGUID", c => c.Guid(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Answer", "AnswerGUID");
        }
    }
}
