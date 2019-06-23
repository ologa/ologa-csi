namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCutPointEntity : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CutPoint",
                c => new
                    {
                        CutPointID = c.Int(nullable: false, identity: true),
                        CutPointGuid = c.Guid(nullable: false),
                        Description = c.String(),
                        InitialValue = c.Double(nullable: false),
                        FinalValue = c.Double(nullable: false),
                        Color = c.String(),
                    })
                .PrimaryKey(t => t.CutPointID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.CutPoint");
        }
    }
}
