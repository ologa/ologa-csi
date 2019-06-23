namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddGraduationCriteria : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GraduationCriteria",
                c => new
                    {
                        GraduationCriteriaID = c.Int(nullable: false),
                        MinScoreQuestion = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.GraduationCriteriaID)
                .ForeignKey("dbo.Site", t => t.GraduationCriteriaID)
                .Index(t => t.GraduationCriteriaID);
            
            CreateTable(
                "dbo.DomainCriteria",
                c => new
                    {
                        DomainCriteriaID = c.Int(nullable: false, identity: true),
                        Score = c.Int(nullable: false),
                        DomainID = c.Int(),
                        GraduationCriteriaID = c.Int(),
                    })
                .PrimaryKey(t => t.DomainCriteriaID)
                .ForeignKey("dbo.Domain", t => t.DomainID)
                .ForeignKey("dbo.GraduationCriteria", t => t.GraduationCriteriaID)
                .Index(t => t.DomainID, name: "IX_Domain_DomainID")
                .Index(t => t.GraduationCriteriaID, name: "IX_GraduationCriteria_GraduationCriteriaID");
            
            CreateTable(
                "dbo.QuestionCriteria",
                c => new
                    {
                        QuestionCriteriaID = c.Int(nullable: false, identity: true),
                        Score = c.Int(nullable: false),
                        QuestionID = c.Int(),
                        GraduationCriteriaID = c.Int(),
                    })
                .PrimaryKey(t => t.QuestionCriteriaID)
                .ForeignKey("dbo.Question", t => t.QuestionID)
                .ForeignKey("dbo.GraduationCriteria", t => t.GraduationCriteriaID)
                .Index(t => t.QuestionID, name: "IX_Question_QuestionID")
                .Index(t => t.GraduationCriteriaID, name: "IX_GraduationCriteria_GraduationCriteriaID");
            
            AddColumn("dbo.Question", "GraduationCriteriaID", c => c.Int());
            CreateIndex("dbo.Question", "GraduationCriteriaID", name: "IX_GraduationCriteria_GraduationCriteriaID");
            AddForeignKey("dbo.Question", "GraduationCriteriaID", "dbo.GraduationCriteria", "GraduationCriteriaID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.GraduationCriteria", "GraduationCriteriaID", "dbo.Site");
            DropForeignKey("dbo.QuestionCriteria", "GraduationCriteriaID", "dbo.GraduationCriteria");
            DropForeignKey("dbo.QuestionCriteria", "QuestionID", "dbo.Question");
            DropForeignKey("dbo.Question", "GraduationCriteriaID", "dbo.GraduationCriteria");
            DropForeignKey("dbo.DomainCriteria", "GraduationCriteriaID", "dbo.GraduationCriteria");
            DropForeignKey("dbo.DomainCriteria", "DomainID", "dbo.Domain");
            DropIndex("dbo.QuestionCriteria", "IX_GraduationCriteria_GraduationCriteriaID");
            DropIndex("dbo.QuestionCriteria", "IX_Question_QuestionID");
            DropIndex("dbo.DomainCriteria", "IX_GraduationCriteria_GraduationCriteriaID");
            DropIndex("dbo.DomainCriteria", "IX_Domain_DomainID");
            DropIndex("dbo.GraduationCriteria", new[] { "GraduationCriteriaID" });
            DropIndex("dbo.Question", "IX_GraduationCriteria_GraduationCriteriaID");
            DropColumn("dbo.Question", "GraduationCriteriaID");
            DropTable("dbo.QuestionCriteria");
            DropTable("dbo.DomainCriteria");
            DropTable("dbo.GraduationCriteria");
        }
    }
}
