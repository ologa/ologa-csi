namespace EFDataAccess.Migrations
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateAll_CheckBoxes_For_NewRoutineVisits_With_Todos_in_Description : DbMigration
    {
        public override void Up()
        {
            List<int> SupportServiceOrderInDomain = new List<int>(new int[] { 89, 90, 92, 103, 113, 114, 115, 121, 122, 123, 141, 142 });

            foreach (int sso in SupportServiceOrderInDomain)
            {
                Sql(@"UPDATE rvs
                    SET rvs.Checked = 1
                    FROM [RoutineVisitSupport] rvs 
                    JOIN [RoutineVisitMember] rvm ON rvs.[RoutineVisitMemberID] = rvm.[RoutineVisitMemberID]
                    JOIN [RoutineVisit] rv ON rv.[RoutineVisitID] = rvm.[RoutineVisitID]
                    JOIN [SupportServiceType] sst ON sst.[SupportServiceTypeID] = rvs.SupportID AND sst.Tool='routine-visit'
                    JOIN [Beneficiary] ben ON ben.beneficiaryID = rvm.BeneficiaryID
                    JOIN [ChildStatusHistory] csh ON (csh.ChildStatusHistoryID = 
                    (SELECT max(csh2.ChildStatusHistoryID) FROM  [ChildStatusHistory] csh2 WHERE csh2.BeneficiaryID = ben.BeneficiaryID
			                    AND (csh2.EffectiveDate < rv.RoutineVisitDate)))
                    JOIN [ChildStatus] cs ON (csh.childStatusID = cs.StatusID AND cs.Description = 'Inicial')
                    WHERE sst.Description LIKE '%(todos)%'
                    AND sst.SupportServiceOrderInDomain =  " + sso + @"
                    AND rv.[RoutineVisitID] IN
                    (
	                    SELECT 
		                    obj.[RoutineVisitID]
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
			                    ,rvs.Checked
			                    ,sst.SupportServiceOrderInDomain
		                    FROM [RoutineVisitSupport] rvs 
		                    JOIN [RoutineVisitMember] rvm ON rvs.[RoutineVisitMemberID] = rvm.[RoutineVisitMemberID]
		                    JOIN [RoutineVisit] rv ON rv.[RoutineVisitID] = rvm.[RoutineVisitID]
		                    JOIN [SupportServiceType] sst ON sst.[SupportServiceTypeID] = rvs.SupportID AND sst.Tool='routine-visit'
                            JOIN [Beneficiary] ben ON ben.beneficiaryID = rvm.BeneficiaryID
                            JOIN [ChildStatusHistory] csh ON (csh.ChildStatusHistoryID = 
                            (SELECT max(csh2.ChildStatusHistoryID) FROM  [ChildStatusHistory] csh2 WHERE csh2.BeneficiaryID = ben.BeneficiaryID
			                            AND (csh2.EffectiveDate < rv.RoutineVisitDate)))
                            JOIN [ChildStatus] cs ON (csh.childStatusID = cs.StatusID AND cs.Description = 'Inicial')
		                    WHERE sst.Description LIKE '%(todos)%'
		                    AND sst.SupportServiceOrderInDomain = " + sso + @"
		                    AND rvs.Checked = 1
		                    AND rv.RoutineVisitDate > '2018-09-21'
	                    )obj
                    )");
            }
        }
        
        public override void Down()
        {
        }
    }
}
