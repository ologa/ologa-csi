namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSiteGoalsTableAndFamilyKitReceivedOnTableRoutineVisit : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SiteGoal",
                c => new
                    {
                        SiteGoalID = c.Int(nullable: false, identity: true),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                        SiteGoal_guid = c.Guid(nullable: false, defaultValueSql: "newid()"),
                        Indicator = c.String(nullable: false, maxLength: 250),
                        SitePerformanceComment = c.String(nullable: false, maxLength: 250),
                        GoalDate = c.DateTime(nullable: false),
                        GoalNumber = c.Int(nullable: false),
                        SiteID = c.Int(),
                    })
                .PrimaryKey(t => t.SiteGoalID)
                .ForeignKey("dbo.Site", t => t.SiteID)
                .Index(t => t.SiteID, name: "IX_Site_SiteID");
            
            AddColumn("dbo.RoutineVisit", "FamilyKitReceived", c => c.Boolean(nullable: false));

            Sql(@"INSERT INTO[dbo].[SiteGoal] ([Indicator],[SitePerformanceComment],[GoalDate],[GoalNumber])
            VALUES('Número de beneficiários servido por programas de PEPFAR para OVC e famílias afetadas pelo HIV/AIDS','', GETDATE(),1)",false);
            Sql(@"INSERT INTO [SiteGoal] ([Indicator],[SitePerformanceComment],[GoalDate],[GoalNumber])
            VALUES('Número de referencias de saúde e outros serviços sociais','', GETDATE(),1)", false);
            Sql(@"INSERT INTO [SiteGoal] ([Indicator],[SitePerformanceComment],[GoalDate],[GoalNumber])
            VALUES('Número de referencias de saúde e outros serviços sociais designadas por completas','', GETDATE(),1)", false);
            Sql(@"INSERT INTO [SiteGoal] ([Indicator],[SitePerformanceComment],[GoalDate],[GoalNumber])
            VALUES('Número de agregados familiares recebendo Kit Familiar','', GETDATE(),1)", false);
            Sql(@"INSERT INTO [SiteGoal] ([Indicator],[SitePerformanceComment],[GoalDate],[GoalNumber])
            VALUES('Número de crianças dos 6 - 59 meses rastreados para malnutrição aguda ao nível comunitário (MUAC)','', GETDATE(),1)", false);
            Sql(@"INSERT INTO [SiteGoal] ([Indicator],[SitePerformanceComment],[GoalDate],[GoalNumber])
            VALUES('Número de crianças  6 - 59 meses com malnutrição aguda, detetados ao nível  da comunidade (Muac)','', GETDATE(),1)", false);
            Sql(@"INSERT INTO [SiteGoal] ([Indicator],[SitePerformanceComment],[GoalDate],[GoalNumber])
            VALUES('Percentagem de OVC com seroestado reportado ao parceiro de implementação (<18 anos)','', GETDATE(),1)", false);
            Sql(@"INSERT INTO [SiteGoal] ([Indicator],[SitePerformanceComment],[GoalDate],[GoalNumber])
            VALUES('Percentagem de OVC com seroestado reportado ao parceiro de implementação (>18 anos)','', GETDATE(),1)", false);
            Sql(@"INSERT INTO [SiteGoal] ([Indicator],[SitePerformanceComment],[GoalDate],[GoalNumber])
            VALUES('HIV-','', GETDATE(),1)", false);
            Sql(@"INSERT INTO [SiteGoal] ([Indicator],[SitePerformanceComment],[GoalDate],[GoalNumber])
            VALUES('HIV+ em TARV','', GETDATE(),1)", false);
            Sql(@"INSERT INTO [SiteGoal] ([Indicator],[SitePerformanceComment],[GoalDate],[GoalNumber])
            VALUES('HIV+  Não em  TARV','', GETDATE(),1)", false);
            Sql(@"INSERT INTO [SiteGoal] ([Indicator],[SitePerformanceComment],[GoalDate],[GoalNumber])
            VALUES('Seroestado conhecido não revelado','', GETDATE(),1)", false);
            Sql(@"INSERT INTO [SiteGoal] ([Indicator],[SitePerformanceComment],[GoalDate],[GoalNumber])
            VALUES('Desconhecido','', GETDATE(),1)", false);
        }

    public override void Down()
        {
            DropForeignKey("dbo.SiteGoal", "SiteID", "dbo.Site");
            DropIndex("dbo.SiteGoal", "IX_Site_SiteID");
            DropColumn("dbo.RoutineVisit", "FamilyKitReceived");
            DropTable("dbo.SiteGoal");
        }
    }
}
