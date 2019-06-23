namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_Selected_Reference_For_CheckboxList_in_Reference_Service : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ReferenceService", "SelectedReference", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ReferenceService", "SelectedReference");
        }
    }
}
