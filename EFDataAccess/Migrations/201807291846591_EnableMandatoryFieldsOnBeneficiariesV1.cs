namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class EnableMandatoryFieldsOnBeneficiariesV1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Adult", "HIVStatusID", "dbo.HIVStatus");
            DropForeignKey("dbo.Child", "HIVStatusID", "dbo.HIVStatus");
            DropIndex("dbo.Adult", new[] { "HIVStatusID" });
            DropIndex("dbo.Child", new[] { "HIVStatusID" });
            AlterColumn("dbo.Adult", "FirstName", c => c.String(nullable: false));
            AlterColumn("dbo.Adult", "LastName", c => c.String(nullable: false));
            AlterColumn("dbo.Adult", "DateOfBirth", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Adult", "HIVStatusID", c => c.Int(nullable: false));
            AlterColumn("dbo.Child", "HIVStatusID", c => c.Int(nullable: false));
            CreateIndex("dbo.Adult", "HIVStatusID");
            CreateIndex("dbo.Child", "HIVStatusID");
            AddForeignKey("dbo.Adult", "HIVStatusID", "dbo.HIVStatus", "HIVStatusID", cascadeDelete: true);
            AddForeignKey("dbo.Child", "HIVStatusID", "dbo.HIVStatus", "HIVStatusID", cascadeDelete: true);
        }

        public override void Down()
        {
            DropForeignKey("dbo.Child", "HIVStatusID", "dbo.HIVStatus");
            DropForeignKey("dbo.Adult", "HIVStatusID", "dbo.HIVStatus");
            DropIndex("dbo.Child", new[] { "HIVStatusID" });
            DropIndex("dbo.Adult", new[] { "HIVStatusID" });
            AlterColumn("dbo.Child", "HIVStatusID", c => c.Int());
            AlterColumn("dbo.Adult", "HIVStatusID", c => c.Int());
            AlterColumn("dbo.Adult", "DateOfBirth", c => c.DateTime());
            AlterColumn("dbo.Adult", "LastName", c => c.String());
            AlterColumn("dbo.Adult", "FirstName", c => c.String());
            CreateIndex("dbo.Child", "HIVStatusID");
            CreateIndex("dbo.Adult", "HIVStatusID");
            AddForeignKey("dbo.Child", "HIVStatusID", "dbo.HIVStatus", "HIVStatusID");
            AddForeignKey("dbo.Adult", "HIVStatusID", "dbo.HIVStatus", "HIVStatusID");
        }
    }
}
