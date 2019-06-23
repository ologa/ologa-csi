namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EnableMandatoryFieldsForHIVStatus : DbMigration
    {
        public override void Up()
        {
            //=====Update in HIV Column in HIVStatus with Null or Blank Value=====/
            //Update with 'N'(Negative) 
            Sql(@"UPDATE hiv SET hiv.HIV = 'N' 
                FROM [HIVStatus] hiv 
                WHERE (hiv.HIV IS NULL OR hiv.HIV = '')
                AND hiv.HIVinTreatment = -1 
                AND hiv.HIVUndisclosedReason = -1");


            //Update with 'P'(Positive) IN TARV
            Sql(@"UPDATE hiv
                SET hiv.HIV = 'P'
                FROM[HIVStatus] hiv
                WHERE (hiv.HIV IS NULL OR hiv.HIV = '')
                AND hiv.HIVinTreatment = 0
                AND hiv.HIVUndisclosedReason = -1");


            //Update with 'P'(Positive) NOT IN TARV
            Sql(@"UPDATE hiv
                SET hiv.HIV = 'P'
                FROM[HIVStatus] hiv
                WHERE (hiv.HIV IS NULL OR hiv.HIV = '')
                AND hiv.HIVinTreatment = 1
                AND hiv.HIVUndisclosedReason = -1");


            //Update with 'U'(Unknown) NOT REVEALED
            Sql(@"UPDATE hiv
                SET hiv.HIV = 'U'
                FROM [HIVStatus] hiv
                WHERE (hiv.HIV IS NULL OR hiv.HIV = '')
                AND hiv.HIVinTreatment = -1
                AND hiv.HIVUndisclosedReason = 0");


            //Update with 'U'(Unknown) NOT KNOWN
            Sql(@"UPDATE hiv
                SET hiv.HIV = 'U'
                FROM [HIVStatus] hiv
                WHERE (hiv.HIV IS NULL OR hiv.HIV = '')
                AND hiv.HIVinTreatment = -1
                AND hiv.HIVUndisclosedReason = 1");

            //Update with 'U'(Unknown) NOT RECOMMENDED
            Sql(@"UPDATE hiv
                SET hiv.HIV = 'U'
                FROM [HIVStatus] hiv
                WHERE (hiv.HIV IS NULL OR hiv.HIV = '')
                AND hiv.HIVinTreatment = -1
                AND hiv.HIVUndisclosedReason = 2");

            //Update HIVStatusID from Adult with Nullable HIV Column to Original (First) HIVStatus
            Sql(@"UPDATE a
                SET a.HIVStatusID = hiv2.HIVStatusID
                FROM [HIVStatus] hiv
                JOIN [Adult] a ON a.HIVStatusID = hiv.HIVStatusID
                JOIN [HIVStatus] hiv2 ON a.adultID = hiv2.adultID AND hiv2.HIVStatusID IN
                ( SELECT  obj.HIVStatusID FROM 
                    ( SELECT row_number() OVER (PARTITION BY adultID ORDER BY HIVStatusID ASC) AS numeroLinha ,HIVStatusID ,[AdultID]
		                FROM [HIVStatus]
		                WHERE adultID IN
		                ( SELECT a.AdultID FROM [HIVStatus] hiv JOIN [Adult] a ON a.HIVStatusID = hiv.HIVStatusID 
                            WHERE hiv.HIV is null
		                )
	                ) obj WHERE obj.numeroLinha=1
                ) WHERE hiv.HIV is null");

            //Update HIVStatusID from Child with Nullable HIV Column to Original (First) HIVStatus
            Sql(@"UPDATE c
                SET c.HIVStatusID = hiv2.HIVStatusID
                FROM [HIVStatus] hiv
                JOIN [Child] c ON c.HIVStatusID = hiv.HIVStatusID
                JOIN [HIVStatus] hiv2 ON c.childID = hiv2.childID AND hiv2.HIVStatusID IN
                ( SELECT obj.HIVStatusID FROM
	                ( SELECT  row_number() OVER (PARTITION BY childID ORDER BY HIVStatusID ASC) AS numeroLinha ,HIVStatusID ,childID
		                FROM  [HIVStatus]
		                WHERE childID IN
		                (  SELECT c.childID FROM [HIVStatus] hiv JOIN Child c ON c.HIVStatusID = hiv.HIVStatusID
			               WHERE hiv.HIV is null
		                )
	                ) obj WHERE obj.numeroLinha=1
                )WHERE hiv.HIV is null");

            //Update HIVStatus with Indeterminate Status for Adults that have HIVStatus with Nullable HIV Column
            Sql(@"UPDATE hiv
                SET 
                hiv.InformationDate = (CASE WHEN a.RegistrationDate IS NULL THEN hh.RegistrationDate ELSE a.RegistrationDate END)
                ,hiv.CreatedAt = a.CreatedDate
                ,hiv.HIV = 'Z'
                ,hiv.HIVInTreatment = -1
                ,hiv.HIVUndisclosedReason = -1
                ,hiv.UserID = a.CreatedUserID
                ,hiv.AdultID = a.AdultID
                ,hiv.ChildID = 0
                ,hiv.BeneficiaryGuid = a.adultGuid
                ,hiv.NID = a.NID
                FROM Adult a 
                INNER JOIN Household hh ON hh.HouseholdID = a.HouseholdID 
                INNER JOIN Hivstatus hiv ON a.HIVStatusID = hiv.HIVStatusID
                WHERE hiv.HIV IS NULL");

            //Update HIVStatus with Indeterminate Status for Childs that have HIVStatus with Nullable HIV Column
            Sql(@"UPDATE hiv
                SET 
                hiv.InformationDate = (CASE WHEN c.RegistrationDate IS NULL THEN hh.RegistrationDate ELSE c.RegistrationDate END)
                ,hiv.CreatedAt = c.CreatedDate
                ,hiv.HIV = 'Z'
                ,hiv.HIVInTreatment = -1
                ,hiv.HIVUndisclosedReason = -1
                ,hiv.UserID = c.CreatedUserID
                ,hiv.AdultID = 0
                ,hiv.ChildID = c.ChildID
                ,hiv.BeneficiaryGuid = c.child_Guid
                ,hiv.NID = c.NID
                FROM Child c 
                INNER JOIN Household hh ON hh.HouseholdID = c.HouseholdID 
                INNER JOIN Hivstatus hiv ON c.HIVStatusID = hiv.HIVStatusID
                WHERE hiv.HIV IS NULL");

            //Delete HIVstatus with Nullable HIV Column
            Sql(@"DELETE FROM [HIVStatus] WHERE hiv IS NULL OR hiv =''");

            AlterColumn("dbo.HIVStatus", "HIV", c => c.String(nullable: false, maxLength: 1));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.HIVStatus", "HIV", c => c.String(maxLength: 1));
        }
    }
}
