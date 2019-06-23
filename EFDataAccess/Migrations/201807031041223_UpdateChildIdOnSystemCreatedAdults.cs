namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateChildIdOnSystemCreatedAdults : DbMigration
    {
        public override void Up()
        {
            Sql(@"Update Adult Set 
                ChildID = obj.CChildID-- Ligação entre a criança que se tornou adulto e o adulto automaticamente criado pelo sistema
                From
                (
                select
                c.ChildID As CChildID, c.FirstName As CFirstName, c.LastName As CLastName, c.DateOfBirth As CDateOfBirth,
                a.AdultId As AAdultId, a.FirstName As AFirstName, a.LastName As ALastName, a.DateOfBirth As ADateOfBirth
                From Adult a inner join Child c on(c.FirstName = a.FirstName and c.LastName = a.LastName and c.DateOfBirth = a.DateOfBirth)
                ) obj
                where AdultId = obj.AAdultID And ChildID is null");
        }
        
        public override void Down()
        {
        }
    }
}
