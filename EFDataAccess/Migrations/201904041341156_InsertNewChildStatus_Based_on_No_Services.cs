namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InsertNewChildStatus_Based_on_No_Services : DbMigration
    {
        public override void Up()
        {
            Sql(@"INSERT INTO [dbo].[ChildStatusHistory]
                ([EffectiveDate] ,[CreatedDate], [ChildStatusID], [CreatedUserID], [ChildID], [SyncDate], [SyncGuid], [AdultID], 
                [LastUpdatedDate], [LastUpdatedUserID], [State], [SyncState], [BeneficiaryGuid], [BeneficiaryID])
                SELECT
	                '2019-09-20', GETDATE(), (SELECT StatusID FROM [ChildStatus] Where Description = 'Eliminado - Sem Serviços')
	                , 1 , NULL , NULL , NULL , NULL , GETDATE() , 1 , 0 , 0 , ben.Beneficiary_guid, ben.BeneficiaryID
                FROM [Beneficiary] ben
				JOIN [vw_beneficiary_details] benView ON ben.beneficiaryID = benView.ID AND benView.RegistrationDate < '2018-09-20'
				JOIN [ChildStatusHistory] csh ON csh.BeneficiaryID = ben.BeneficiaryID AND csh.EffectiveDate < '2019-03-15'
				--Beneficiarios em que o último estado era inicial
				AND csh.ChildStatusHistoryID IN
				(
					SELECT
							csh.ChildStatusHistoryID
						FROM [Beneficiary] ben
						JOIN [vw_beneficiary_details] benView ON ben.beneficiaryID = benView.ID AND benView.RegistrationDate < '2018-09-20'
						JOIN [ChildStatusHistory] csh ON csh.BeneficiaryID = ben.BeneficiaryID AND csh.EffectiveDate < '2019-03-15'
						AND csh.ChildStatusHistoryID IN
						(--Brings all Beneficiaries LastStatus
							SELECT 
								csh.ChildStatusHistoryID
							FROM  [Beneficiary] ben 
							JOIN [Household] hh ON hh.HouseHoldID = ben.HouseholdID
							JOIN [vw_beneficiary_details] benView ON ben.beneficiaryID = benView.ID AND benView.RegistrationDate < '2018-09-20'
							JOIN [ChildStatusHistory] csh ON (csh.ChildStatusHistoryID = 
							(SELECT max(csh2.ChildStatusHistoryID) FROM  [ChildStatusHistory] csh2 WHERE csh2.BeneficiaryID = ben.BeneficiaryID))
							JOIN [ChildStatus] cs ON (csh.childStatusID = cs.StatusID 
							AND cs.Description in ('Inicial'))
						)
				)
				--Beneficiarios que não tiveram Servicos Ficha Antiga
				WHERE ben.BeneficiaryID NOT IN
				(
					SELECT
						ben.BeneficiaryID
					FROM [RoutineVisitSupport] rvs
					JOIN [RoutineVisitMember] rvm ON rvs.[RoutineVisitMemberID] = rvm.[RoutineVisitMemberID]
					JOIN [RoutineVisit] rv ON rv.[RoutineVisitID] = rvm.[RoutineVisitID] 
					JOIN [Beneficiary] ben ON ben.beneficiaryID = rvm.BeneficiaryID
					JOIN [vw_beneficiary_details] benView ON ben.beneficiaryID = benView.ID AND benView.RegistrationDate < '2018-09-20'
					WHERE (rv.RoutineVisitDate BETWEEN '2018-09-21' AND '2019-03-15' AND rvs.SupportType in ('Domain','DomainEntity','MUAC',
				'DPI','HIVRISK') AND rvs.SupportValue = '1')
					OR (rv.RoutineVisitDate BETWEEN '2018-09-21' AND '2019-03-15' AND rvs.SupportType IS NULL AND rvs.Checked = '1')
				)
				--Beneficiarios que não tiveram Servicos Ficha Nova
				AND ben.BeneficiaryID NOT IN
				(
					SELECT
						ben.BeneficiaryID
					FROM [RoutineVisitSupport] rvs
					JOIN [RoutineVisitMember] rvm ON rvs.[RoutineVisitMemberID] = rvm.[RoutineVisitMemberID]
					JOIN [RoutineVisit] rv ON rv.[RoutineVisitID] = rvm.[RoutineVisitID] 
					JOIN [SupportServiceType] sst ON sst.[SupportServiceTypeID] = rvs.SupportID AND sst.Tool='routine-visit'
					JOIN [Beneficiary] ben ON ben.beneficiaryID = rvm.BeneficiaryID
					JOIN [vw_beneficiary_details] benView ON ben.beneficiaryID = benView.ID AND benView.RegistrationDate < '2018-09-20'
					WHERE (rv.RoutineVisitDate BETWEEN '2018-09-21' AND '2019-03-15' AND rvs.SupportType IS NULL AND rvs.Checked = '1')
				)
				--Beneficiarios que não tiveram Referencias
				AND ben.BeneficiaryID NOT IN
				(
					SELECT
						ben.BeneficiaryID
					FROM [Beneficiary] ben
					JOIN [vw_beneficiary_details] benView ON ben.beneficiaryID = benView.ID AND benView.RegistrationDate < '2018-09-20'
					JOIN [ReferenceService] rs ON rs.BeneficiaryID = ben.BeneficiaryID
					JOIN [Reference] r on (r.ReferenceServiceID = rs.ReferenceServiceID)
					JOIN [ReferenceType] rt on (rt.ReferenceTypeID = r.ReferenceTypeID)
					WHERE (rs.ReferenceDate BETWEEN '2018-09-21' AND '2019-03-15' AND rt.ReferenceCategory in ('Activist')
					 AND r.Value <> '0' AND r.Value <> '' AND r.Value IS NOT NULL)
				)");
        }
        
        public override void Down()
        {
        }
    }
}
