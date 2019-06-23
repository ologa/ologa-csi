namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateAll_CheckBoxes_For_OldRoutineVisits_With_FE_and_HAB_Checked : DbMigration
    {
        public override void Up()
        {
            //UPDATE PARA TODOS CHECKS PARA FORTALECIMENTO ECONÓMICO
            Sql(@"UPDATE rvs
                SET rvs.SupportValue = '1'
                FROM [RoutineVisitSupport] rvs
                JOIN [RoutineVisitMember] rvm ON rvs.[RoutineVisitMemberID] = rvm.[RoutineVisitMemberID]
                JOIN [RoutineVisit] rv ON rv.[RoutineVisitID] = rvm.[RoutineVisitID]
                JOIN [Beneficiary] ben ON ben.beneficiaryID = rvm.BeneficiaryID
                JOIN [Household] hh ON rv.HouseholdID = hh.HouseholdID
                JOIN [ChildStatusHistory] csh ON (csh.ChildStatusHistoryID = 
                (SELECT max(csh2.ChildStatusHistoryID) FROM  [ChildStatusHistory] csh2 WHERE csh2.BeneficiaryID = ben.BeneficiaryID
			                AND (csh2.EffectiveDate < rv.RoutineVisitDate)))
                JOIN [ChildStatus] cs ON (csh.childStatusID = cs.StatusID AND cs.Description = 'Inicial')
                WHERE rvs.[SupportType] in ('Domain','DomainEntity') AND rvs.SupportID = '6'
                AND rv.RoutineVisitID IN
                (
	                SELECT 
		                obj.RoutineVisitID
	                FROM
	                (
		                SELECT 
			                rvs.[RoutineVisitSupportID]
			                ,rvs.[SupportType]
			                ,rvs.[SupportID]
			                ,rvs.[SupportValue]
			                ,rvs.[RoutineVisitMemberID]
			                ,rv.RoutineVisitID
			                ,rv.VisitRecordCode
			                ,hh.HouseHoldID
			                ,hh.HouseholdName
			                ,ben.BeneficiaryID
			                ,ben.FirstName
			                ,ben.LastName
		                FROM [RoutineVisitSupport] rvs
		                JOIN [RoutineVisitMember] rvm ON rvs.[RoutineVisitMemberID] = rvm.[RoutineVisitMemberID]
		                JOIN [RoutineVisit] rv ON rv.[RoutineVisitID] = rvm.[RoutineVisitID]
		                JOIN [Beneficiary] ben ON ben.beneficiaryID = rvm.BeneficiaryID
		                JOIN [Household] hh ON rv.HouseholdID = hh.HouseholdID
		                JOIN [ChildStatusHistory] csh ON (csh.ChildStatusHistoryID = 
		                (SELECT max(csh2.ChildStatusHistoryID) FROM  [ChildStatusHistory] csh2 WHERE csh2.BeneficiaryID = ben.BeneficiaryID
					                AND (csh2.EffectiveDate < rv.RoutineVisitDate)))
		                JOIN [ChildStatus] cs ON (csh.childStatusID = cs.StatusID AND cs.Description = 'Inicial')
		                WHERE rvs.[SupportType] in ('Domain','DomainEntity')
		                AND rvs.SupportID = '6' AND rvs.SupportValue = '1'
		                AND rv.RoutineVisitDate > '2018-09-21'
	                )obj
                )");

            //UPDATE PARA TODOS CHECKS PARA HABITAÇÃO
            Sql(@"UPDATE rvs
                SET rvs.SupportValue = '1' 
                FROM [RoutineVisitSupport] rvs 
                JOIN [RoutineVisitMember] rvm ON rvs.[RoutineVisitMemberID] = rvm.[RoutineVisitMemberID]
                JOIN [RoutineVisit] rv ON rv.[RoutineVisitID] = rvm.[RoutineVisitID]
                JOIN [Beneficiary] ben ON ben.beneficiaryID = rvm.BeneficiaryID
                JOIN [Household] hh ON rv.HouseholdID = hh.HouseholdID
                JOIN [ChildStatusHistory] csh ON (csh.ChildStatusHistoryID = 
                (SELECT max(csh2.ChildStatusHistoryID) FROM  [ChildStatusHistory] csh2 WHERE csh2.BeneficiaryID = ben.BeneficiaryID
			                AND (csh2.EffectiveDate < rv.RoutineVisitDate)))
                JOIN [ChildStatus] cs ON (csh.childStatusID = cs.StatusID AND cs.Description = 'Inicial')
                WHERE rvs.[SupportType] in ('Domain','DomainEntity') AND rvs.SupportID = '7'
                AND rv.RoutineVisitID IN
                (
	                SELECT 
		                obj.RoutineVisitID
	                FROM
	                (
		                SELECT 
			                rvs.[RoutineVisitSupportID]
			                ,rvs.[SupportType]
			                ,rvs.[SupportID]
			                ,rvs.[SupportValue]
			                ,rvs.[RoutineVisitMemberID]
			                ,rv.RoutineVisitID
			                ,rv.VisitRecordCode
			                ,hh.HouseHoldID
			                ,hh.HouseholdName
			                ,ben.BeneficiaryID
			                ,ben.FirstName
			                ,ben.LastName
		                FROM [RoutineVisitSupport] rvs 
		                JOIN [RoutineVisitMember] rvm ON rvs.[RoutineVisitMemberID] = rvm.[RoutineVisitMemberID]
		                JOIN [RoutineVisit] rv ON rv.[RoutineVisitID] = rvm.[RoutineVisitID]
		                JOIN [Beneficiary] ben ON ben.beneficiaryID = rvm.BeneficiaryID
		                JOIN [Household] hh ON rv.HouseholdID = hh.HouseholdID
		                JOIN [ChildStatusHistory] csh ON (csh.ChildStatusHistoryID = 
		                (SELECT max(csh2.ChildStatusHistoryID) FROM  [ChildStatusHistory] csh2 WHERE csh2.BeneficiaryID = ben.BeneficiaryID
					                AND (csh2.EffectiveDate < rv.RoutineVisitDate)))
		                JOIN [ChildStatus] cs ON (csh.childStatusID = cs.StatusID AND cs.Description = 'Inicial')
		                WHERE rvs.[SupportType] in ('Domain','DomainEntity')
		                AND rvs.SupportID = '7' AND rvs.SupportValue = '1'
		                AND rv.RoutineVisitDate > '2018-09-21'
	                )obj
                )");

        }
        
        public override void Down()
        {
        }
    }
}
