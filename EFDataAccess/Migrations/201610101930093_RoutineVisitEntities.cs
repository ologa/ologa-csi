namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RoutineVisitEntities : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.RoutineVisitMember",
                c => new
                    {
                        RoutineVisitMemberID = c.Int(nullable: false, identity: true),
                        AdultID = c.Int(),
                        ChildID = c.Int(),
                        RoutineVisitID = c.Int(),
                    })
                .PrimaryKey(t => t.RoutineVisitMemberID)
                .ForeignKey("dbo.Adult", t => t.AdultID)
                .ForeignKey("dbo.Child", t => t.ChildID)
                .ForeignKey("dbo.RoutineVisit", t => t.RoutineVisitID)
                .Index(t => t.AdultID, name: "IX_Adult_AdultId")
                .Index(t => t.ChildID, name: "IX_Child_ChildID")
                .Index(t => t.RoutineVisitID, name: "IX_RoutineVisit_RoutineVisitID");
            
            CreateTable(
                "dbo.RoutineVisit",
                c => new
                    {
                        RoutineVisitID = c.Int(nullable: false, identity: true),
                        RoutineVisitDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.RoutineVisitID);
            
            CreateTable(
                "dbo.RoutineVisitSupport",
                c => new
                    {
                        RoutineVisitSupportID = c.Int(nullable: false, identity: true),
                        SupportType = c.String(),
                        SupportID = c.Int(nullable: false),
                        SupportValue = c.String(),
                        RoutineVisitMemberID = c.Int(),
                    })
                .PrimaryKey(t => t.RoutineVisitSupportID)
                .ForeignKey("dbo.RoutineVisitMember", t => t.RoutineVisitMemberID)
                .Index(t => t.RoutineVisitMemberID, name: "IX_RoutineVisitMember_RoutineVisitMemberID");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.RoutineVisitSupport", "RoutineVisitMemberID", "dbo.RoutineVisitMember");
            DropForeignKey("dbo.RoutineVisitMember", "RoutineVisitID", "dbo.RoutineVisit");
            DropForeignKey("dbo.RoutineVisitMember", "ChildID", "dbo.Child");
            DropForeignKey("dbo.RoutineVisitMember", "AdultID", "dbo.Adult");
            DropIndex("dbo.RoutineVisitSupport", "IX_RoutineVisitMember_RoutineVisitMemberID");
            DropIndex("dbo.RoutineVisitMember", "IX_RoutineVisit_RoutineVisitID");
            DropIndex("dbo.RoutineVisitMember", "IX_Child_ChildID");
            DropIndex("dbo.RoutineVisitMember", "IX_Adult_AdultId");
            DropTable("dbo.RoutineVisitSupport");
            DropTable("dbo.RoutineVisit");
            DropTable("dbo.RoutineVisitMember");
        }
    }
}
