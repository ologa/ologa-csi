namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Add_PartnerID_relation_with_ReferenceService : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ReferenceService", "PartnerID", c => c.Int(nullable: true));
            CreateIndex("dbo.ReferenceService", "PartnerID");
            AddForeignKey("dbo.ReferenceService", "PartnerID", "dbo.Partner", "PartnerID", cascadeDelete: true);

            Sql(@" UPDATE rs
                    SET rs.PartnerID = simplePartner.PartnerID
                    FROM  [ReferenceService] rs
                    JOIN  [Partner] simplePartner ON rs.ReferencedBy = simplePartner.Name", false);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ReferenceService", "PartnerID", "dbo.Partner");
            DropIndex("dbo.ReferenceService", new[] { "PartnerID" });
            DropColumn("dbo.ReferenceService", "PartnerID");
        }
    }
}
