namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Task : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Tasks",
                c => new
                    {
                        TaskID = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                        CompleteDate = c.DateTime(nullable: false),
                        Completed = c.Boolean(nullable: false),
                        Comments = c.String(),
                        TaskGuid = c.Guid(nullable: false),
                        CarePlanDomainID = c.Int(),
                        ResourceID = c.Int(),
                    })
                .PrimaryKey(t => t.TaskID)
                .ForeignKey("dbo.CarePlanDomain", t => t.CarePlanDomainID)
                .ForeignKey("dbo.Resources", t => t.ResourceID)
                .Index(t => t.CarePlanDomainID, name: "IX_CarePlanDomain_CarePlanDomainID")
                .Index(t => t.ResourceID, name: "IX_Resource_ResourceID");
            
            CreateTable(
                "dbo.Resources",
                c => new
                    {
                        ResourceID = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                        ResourceGuid = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => t.ResourceID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tasks", "ResourceID", "dbo.Resources");
            DropForeignKey("dbo.Tasks", "CarePlanDomainID", "dbo.CarePlanDomain");
            DropIndex("dbo.Tasks", "IX_Resource_ResourceID");
            DropIndex("dbo.Tasks", "IX_CarePlanDomain_CarePlanDomainID");
            DropTable("dbo.Resources");
            DropTable("dbo.Tasks");
        }
    }
}
