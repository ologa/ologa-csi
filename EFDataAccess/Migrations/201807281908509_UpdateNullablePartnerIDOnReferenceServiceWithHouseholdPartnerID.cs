namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateNullablePartnerIDOnReferenceServiceWithHouseholdPartnerID : DbMigration
    {
        public override void Up()
        {
            //if (System.Configuration.ConfigurationManager.AppSettings["AppVersion"].Contains("desktop"))
            //{
                Sql(@"UPDATE rs
                SET rs.PartnerID = hh.PartnerID
                FROM ReferenceService rs
                JOIN Child c ON c.ChildID = rs.ChildID
                JOIN Household hh ON hh.HouseholdID = c.HouseholdID
                WHERE rs.partnerID IS NULL
                AND rs.AdultID IS NULL", false);

                Sql(@"UPDATE rs
                SET rs.PartnerID = hh.PartnerID
                FROM ReferenceService rs
                JOIN Adult a ON a.adultID = rs.adultID
                JOIN Household hh ON hh.HouseholdID = a.HouseholdID
                WHERE rs.partnerID IS NULL
                AND rs.ChildID IS NULL", false);
            //}
        }
        
        public override void Down()
        {
        }
    }
}
