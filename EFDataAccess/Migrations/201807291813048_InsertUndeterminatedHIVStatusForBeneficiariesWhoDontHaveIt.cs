namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InsertUndeterminatedHIVStatusForBeneficiariesWhoDontHaveIt : DbMigration
    {
        public override void Up()
        {
            //if (System.Configuration.ConfigurationManager.AppSettings["AppVersion"].Contains("desktop"))
            //{
                //Adult Queries
                //Insert HIVStatus with Indeterminate Status for Adults with HIVStatusID Column Null
                Sql(@"INSERT INTO HIVStatus(InformationDate, CreatedAt, HIV, HIVInTreatment, HIVUndisclosedReason, UserID, AdultID, ChildID, BeneficiaryGuid, NID)
                SELECT (CASE WHEN a.RegistrationDate IS NULL THEN hh.RegistrationDate ELSE a.RegistrationDate END), a.CreatedDate,'Z',-1,-1,a.CreatedUserID, a.adultID, 0, a.adultGuid, a.NID
                FROM Adult a INNER JOIN Household hh ON hh.HouseholdID = a.HouseholdID WHERE a.HIVStatusID is NULL");

                //Insert HIVStatus with Indeterminate Status for Adults that have HIVStatus with AdultID Column that doesnt exist in Adult Table
                Sql(@"INSERT INTO HIVStatus(InformationDate, CreatedAt, HIV, HIVInTreatment, HIVUndisclosedReason, UserID, AdultID, ChildID, BeneficiaryGuid, NID)
                SELECT (CASE WHEN a.RegistrationDate IS NULL THEN hh.RegistrationDate ELSE a.RegistrationDate END), a.CreatedDate,'Z',-1,-1,a.CreatedUserID, a.adultID, 0, a.adultGuid, a.NID
                FROM Adult a INNER JOIN Household hh ON hh.HouseholdID = a.HouseholdID INNER JOIN Hivstatus hiv ON a.HIVStatusID = hiv.HIVStatusID
                WHERE hiv.adultID not in (SELECT adultID from Adult)");

                //Update HIVStatusID Column in Adult with HIVStatusID from Indeterminate Status
                Sql(@"Update a Set a.HIVStatusID = h.HIVStatusID from Adult a inner join HIVStatus h on (a.AdultId = h.AdultID) where h.HIV = 'Z'");


                //Child Queries
                //Insert HIVStatus with Indeterminate Status for Childs with HIVStatusID Column Null
                Sql(@"INSERT INTO HIVStatus(InformationDate, CreatedAt, HIV, HIVInTreatment, HIVUndisclosedReason, UserID, AdultID, ChildID, BeneficiaryGuid, NID)
                SELECT (CASE WHEN c.RegistrationDate IS NULL THEN hh.RegistrationDate ELSE c.RegistrationDate END), c.CreatedDate,'Z',-1,-1,c.CreatedUserID, 0, c.ChildID, c.child_Guid, c.NID
                FROM Child c INNER JOIN Household hh ON hh.HouseholdID = c.HouseholdID WHERE c.HIVStatusID is NULL");

                //Insert HIVStatus with Indeterminate Status for Childs that have HIVStatus with ChildID Column that doesnt exist in Child Table
                Sql(@"INSERT INTO HIVStatus(InformationDate, CreatedAt, HIV, HIVInTreatment, HIVUndisclosedReason, UserID, AdultID, ChildID, BeneficiaryGuid, NID)
                SELECT (CASE WHEN c.RegistrationDate IS NULL THEN hh.RegistrationDate ELSE c.RegistrationDate END), c.CreatedDate,'Z',-1,-1,c.CreatedUserID, 0, c.ChildID, c.child_Guid, c.NID
                FROM Child c INNER JOIN Household hh ON hh.HouseholdID = c.HouseholdID INNER JOIN Hivstatus hiv ON c.HIVStatusID = hiv.HIVStatusID
                WHERE hiv.childID not in (SELECT childID from Child)");

                //Update HIVStatusID Column in Child with HIVStatusID from Indeterminate Status
                Sql(@"Update c Set c.HIVStatusID = h.HIVStatusID from Child c inner join HIVStatus h on (c.ChildID = h.ChildID) where h.HIV = 'Z'");
            //}
        }
        
        public override void Down()
        {
        }
    }
}
