namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Refactor_Household_changeType_Block : DbMigration
    {
        public override void Up()
        {
            var _sql = @" DECLARE @con nvarchar(128)
                         SELECT @con = name
                         FROM sys.default_constraints
                         WHERE parent_object_id = object_id('dbo.HouseHold')
                         AND col_name(parent_object_id, parent_column_id) = 'Block';
                         IF @con IS NOT NULL
                             EXECUTE('ALTER TABLE [dbo].[HouseHold] DROP CONSTRAINT ' + @con)";
            Sql(_sql);
            AlterColumn("dbo.HouseHold", "Block", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.HouseHold", "Block", c => c.Int(nullable: false));
        }
    }
}
