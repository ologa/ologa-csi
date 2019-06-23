namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update_Missing_BeneficiaryHasService : DbMigration
    {
        public override void Up()
        {
            // Update BeneficiaryHasServices With 1, on RoutineVisitMembers That had Services with BeneficiaryHasServices = 0
            Sql(@"UPDATE rvm
            SET rvm.BeneficiaryHasServices = 1
            FROM Beneficiary ben
            INNER JOIN RoutineVisitMember rvm ON ben.BeneficiaryID = rvm.BeneficiaryID
            WHERE rvm.BeneficiaryHasServices = 0
            AND rvm.RoutineVisitMemberID IN
            (	
	            --Routine Visit Members de Beneficiários que tiveram os Serviços CHECKED = 1
	            SELECT rvm.RoutineVisitMemberID
	            FROM HouseHold hh
	            INNER JOIN Beneficiary ben on ben.HouseholdID = hh.HouseHoldID
	            INNER JOIN RoutineVisitMember rvm on rvm.BeneficiaryID = ben.BeneficiaryID 
	            INNER JOIN RoutineVisit rv on rv.RoutineVisitID = rvm.RoutineVisitID AND rv.Version = 'v2'
	            INNER JOIN RoutineVisitSupport rvs on rvs.RoutineVisitMemberID = rvm.RoutineVisitMemberID AND rvs.Checked = '1'
	            INNER JOIN SupportServiceType sst on sst.SupportServiceTypeID = rvs.SupportID AND sst.Tool='routine-visit'
            )");
        }
        
        public override void Down()
        {
        }
    }
}
