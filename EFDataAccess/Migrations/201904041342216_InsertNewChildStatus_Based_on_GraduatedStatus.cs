namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InsertNewChildStatus_Based_on_GraduatedStatus : DbMigration
    {
        public override void Up()
        {
            //Eliminado - Transferido
            Sql(@"INSERT INTO [dbo].[ChildStatusHistory]
            ([EffectiveDate] ,[CreatedDate], [ChildStatusID], [CreatedUserID], [ChildID], [SyncDate], [SyncGuid], [AdultID], 
            [LastUpdatedDate], [LastUpdatedUserID], [State], [SyncState], [BeneficiaryGuid], [BeneficiaryID])
            SELECT
	            '2019-09-20', GETDATE(), (SELECT StatusID FROM [ChildStatus] Where Description = 'Eliminado - Transferido')
	            , 1 , NULL , NULL , NULL , NULL , GETDATE() , 1 , 0 , 0 , ben.Beneficiary_guid, ben.BeneficiaryID
            FROM  [Beneficiary] ben 
            JOIN [Household] hh ON hh.HouseHoldID = ben.HouseholdID
            JOIN [vw_beneficiary_details] benView ON ben.beneficiaryID = benView.ID AND benView.RegistrationDate < '2018-09-20'
            JOIN [ChildStatusHistory] csh ON csh.BeneficiaryID = ben.BeneficiaryID AND csh.EffectiveDate < '2018-09-20'
            JOIN [ChildStatus] cs ON csh.childStatusID = cs.StatusID
            WHERE csh.ChildStatusHistoryID IN
            (--Brings all Beneficiaries LastStatus
	            SELECT 
		            csh.ChildStatusHistoryID
	            FROM  [Beneficiary] ben 
	            JOIN [Household] hh ON hh.HouseHoldID = ben.HouseholdID
	            JOIN [vw_beneficiary_details] benView ON ben.beneficiaryID = benView.ID AND benView.RegistrationDate < '2018-09-20'
	            JOIN [ChildStatusHistory] csh ON (csh.ChildStatusHistoryID = 
	            (SELECT max(csh2.ChildStatusHistoryID) FROM  [ChildStatusHistory] csh2 WHERE csh2.BeneficiaryID = ben.BeneficiaryID))
	            JOIN [ChildStatus] cs ON (csh.childStatusID = cs.StatusID 
	            AND cs.Description in ('Transferido p/ programas NĂO de PEPFAR)'))
            )");

            //Eliminado - Graduaçăo
            Sql(@"INSERT INTO [dbo].[ChildStatusHistory]
            ([EffectiveDate] ,[CreatedDate], [ChildStatusID], [CreatedUserID], [ChildID], [SyncDate], [SyncGuid], [AdultID], 
            [LastUpdatedDate], [LastUpdatedUserID], [State], [SyncState], [BeneficiaryGuid], [BeneficiaryID])
            SELECT
	            '2019-09-20', GETDATE(), (SELECT StatusID FROM [ChildStatus] Where Description = 'Eliminado - Graduaçăo')
	            , 1 , NULL , NULL , NULL , NULL , GETDATE() , 1 , 0 , 0 , ben.Beneficiary_guid, ben.BeneficiaryID
            FROM  [Beneficiary] ben 
            JOIN [Household] hh ON hh.HouseHoldID = ben.HouseholdID
            JOIN [vw_beneficiary_details] benView ON ben.beneficiaryID = benView.ID AND benView.RegistrationDate < '2018-09-20'
            JOIN [ChildStatusHistory] csh ON csh.BeneficiaryID = ben.BeneficiaryID AND csh.EffectiveDate < '2018-09-20'
            JOIN [ChildStatus] cs ON csh.childStatusID = cs.StatusID
            WHERE csh.ChildStatusHistoryID IN
            (--Brings all Beneficiaries LastStatus
	            SELECT 
		            csh.ChildStatusHistoryID
	            FROM  [Beneficiary] ben 
	            JOIN [Household] hh ON hh.HouseHoldID = ben.HouseholdID
	            JOIN [vw_beneficiary_details] benView ON ben.beneficiaryID = benView.ID AND benView.RegistrationDate < '2018-09-20'
	            JOIN [ChildStatusHistory] csh ON (csh.ChildStatusHistoryID = 
	            (SELECT max(csh2.ChildStatusHistoryID) FROM  [ChildStatusHistory] csh2 WHERE csh2.BeneficiaryID = ben.BeneficiaryID))
	            JOIN [ChildStatus] cs ON (csh.childStatusID = cs.StatusID 
	            AND cs.Description in ('Graduaçăo'))
            )");

            //Eliminado - Desistência
            Sql(@"INSERT INTO [dbo].[ChildStatusHistory]
            ([EffectiveDate] ,[CreatedDate], [ChildStatusID], [CreatedUserID], [ChildID], [SyncDate], [SyncGuid], [AdultID], 
            [LastUpdatedDate], [LastUpdatedUserID], [State], [SyncState], [BeneficiaryGuid], [BeneficiaryID])
            SELECT
	            '2019-09-20', GETDATE(), (SELECT StatusID FROM [ChildStatus] Where Description = 'Eliminado - Desistência')
	            , 1 , NULL , NULL , NULL , NULL , GETDATE() , 1 , 0 , 0 , ben.Beneficiary_guid, ben.BeneficiaryID
            FROM  [Beneficiary] ben 
            JOIN [Household] hh ON hh.HouseHoldID = ben.HouseholdID
            JOIN [vw_beneficiary_details] benView ON ben.beneficiaryID = benView.ID AND benView.RegistrationDate < '2018-09-20'
            JOIN [ChildStatusHistory] csh ON csh.BeneficiaryID = ben.BeneficiaryID AND csh.EffectiveDate < '2018-09-20'
            JOIN [ChildStatus] cs ON csh.childStatusID = cs.StatusID
            WHERE csh.ChildStatusHistoryID IN
            (--Brings all Beneficiaries LastStatus
	            SELECT 
		            csh.ChildStatusHistoryID
	            FROM  [Beneficiary] ben 
	            JOIN [Household] hh ON hh.HouseHoldID = ben.HouseholdID
	            JOIN [vw_beneficiary_details] benView ON ben.beneficiaryID = benView.ID AND benView.RegistrationDate < '2018-09-20'
	            JOIN [ChildStatusHistory] csh ON (csh.ChildStatusHistoryID = 
	            (SELECT max(csh2.ChildStatusHistoryID) FROM  [ChildStatusHistory] csh2 WHERE csh2.BeneficiaryID = ben.BeneficiaryID))
	            JOIN [ChildStatus] cs ON (csh.childStatusID = cs.StatusID 
	            AND cs.Description in ('Desistência'))
            )");


            //Eliminado - Perdido
            Sql(@"INSERT INTO [dbo].[ChildStatusHistory]
            ([EffectiveDate] ,[CreatedDate], [ChildStatusID], [CreatedUserID], [ChildID], [SyncDate], [SyncGuid], [AdultID], 
            [LastUpdatedDate], [LastUpdatedUserID], [State], [SyncState], [BeneficiaryGuid], [BeneficiaryID])
            SELECT
	            '2019-09-20', GETDATE(), (SELECT StatusID FROM [ChildStatus] Where Description = 'Eliminado - Perdido')
	            , 1 , NULL , NULL , NULL , NULL , GETDATE() , 1 , 0 , 0 , ben.Beneficiary_guid, ben.BeneficiaryID
            FROM  [Beneficiary] ben 
            JOIN [Household] hh ON hh.HouseHoldID = ben.HouseholdID
            JOIN [vw_beneficiary_details] benView ON ben.beneficiaryID = benView.ID AND benView.RegistrationDate < '2018-09-20'
            JOIN [ChildStatusHistory] csh ON csh.BeneficiaryID = ben.BeneficiaryID AND csh.EffectiveDate < '2018-09-20'
            JOIN [ChildStatus] cs ON csh.childStatusID = cs.StatusID
            WHERE csh.ChildStatusHistoryID IN
            (--Brings all Beneficiaries LastStatus
	            SELECT 
		            csh.ChildStatusHistoryID
	            FROM  [Beneficiary] ben 
	            JOIN [Household] hh ON hh.HouseHoldID = ben.HouseholdID
	            JOIN [vw_beneficiary_details] benView ON ben.beneficiaryID = benView.ID AND benView.RegistrationDate < '2018-09-20'
	            JOIN [ChildStatusHistory] csh ON (csh.ChildStatusHistoryID = 
	            (SELECT max(csh2.ChildStatusHistoryID) FROM  [ChildStatusHistory] csh2 WHERE csh2.BeneficiaryID = ben.BeneficiaryID))
	            JOIN [ChildStatus] cs ON (csh.childStatusID = cs.StatusID 
	            AND cs.Description in ('Perdido'))
            )");


            //Eliminado - Óbito
            Sql(@"INSERT INTO [dbo].[ChildStatusHistory]
            ([EffectiveDate] ,[CreatedDate], [ChildStatusID], [CreatedUserID], [ChildID], [SyncDate], [SyncGuid], [AdultID], 
            [LastUpdatedDate], [LastUpdatedUserID], [State], [SyncState], [BeneficiaryGuid], [BeneficiaryID])
            SELECT
	            '2019-09-20', GETDATE(), (SELECT StatusID FROM [ChildStatus] Where Description = 'Eliminado - Óbito')
	            , 1 , NULL , NULL , NULL , NULL , GETDATE() , 1 , 0 , 0 , ben.Beneficiary_guid, ben.BeneficiaryID
            FROM  [Beneficiary] ben 
            JOIN [Household] hh ON hh.HouseHoldID = ben.HouseholdID
            JOIN [vw_beneficiary_details] benView ON ben.beneficiaryID = benView.ID AND benView.RegistrationDate < '2018-09-20'
            JOIN [ChildStatusHistory] csh ON csh.BeneficiaryID = ben.BeneficiaryID AND csh.EffectiveDate < '2018-09-20'
            JOIN [ChildStatus] cs ON csh.childStatusID = cs.StatusID
            WHERE csh.ChildStatusHistoryID IN
            (--Brings all Beneficiaries LastStatus
	            SELECT 
		            csh.ChildStatusHistoryID
	            FROM  [Beneficiary] ben 
	            JOIN [Household] hh ON hh.HouseHoldID = ben.HouseholdID
	            JOIN [vw_beneficiary_details] benView ON ben.beneficiaryID = benView.ID AND benView.RegistrationDate < '2018-09-20'
	            JOIN [ChildStatusHistory] csh ON (csh.ChildStatusHistoryID = 
	            (SELECT max(csh2.ChildStatusHistoryID) FROM  [ChildStatusHistory] csh2 WHERE csh2.BeneficiaryID = ben.BeneficiaryID))
	            JOIN [ChildStatus] cs ON (csh.childStatusID = cs.StatusID 
	            AND cs.Description in ('Óbito'))
            )");


            //Eliminado - Outras Saídas
            Sql(@"INSERT INTO [dbo].[ChildStatusHistory]
            ([EffectiveDate] ,[CreatedDate], [ChildStatusID], [CreatedUserID], [ChildID], [SyncDate], [SyncGuid], [AdultID], 
            [LastUpdatedDate], [LastUpdatedUserID], [State], [SyncState], [BeneficiaryGuid], [BeneficiaryID])
            SELECT
	            '2019-09-20', GETDATE(), (SELECT StatusID FROM [ChildStatus] Where Description = 'Eliminado - Outras Saídas')
	            , 1 , NULL , NULL , NULL , NULL , GETDATE() , 1 , 0 , 0 , ben.Beneficiary_guid, ben.BeneficiaryID
            FROM  [Beneficiary] ben 
            JOIN [Household] hh ON hh.HouseHoldID = ben.HouseholdID
            JOIN [vw_beneficiary_details] benView ON ben.beneficiaryID = benView.ID AND benView.RegistrationDate < '2018-09-20'
            JOIN [ChildStatusHistory] csh ON csh.BeneficiaryID = ben.BeneficiaryID AND csh.EffectiveDate < '2018-09-20'
            JOIN [ChildStatus] cs ON csh.childStatusID = cs.StatusID
            WHERE csh.ChildStatusHistoryID IN
            (--Brings all Beneficiaries LastStatus
	            SELECT 
		            csh.ChildStatusHistoryID
	            FROM  [Beneficiary] ben 
	            JOIN [Household] hh ON hh.HouseHoldID = ben.HouseholdID
	            JOIN [vw_beneficiary_details] benView ON ben.beneficiaryID = benView.ID AND benView.RegistrationDate < '2018-09-20'
	            JOIN [ChildStatusHistory] csh ON (csh.ChildStatusHistoryID = 
	            (SELECT max(csh2.ChildStatusHistoryID) FROM  [ChildStatusHistory] csh2 WHERE csh2.BeneficiaryID = ben.BeneficiaryID))
	            JOIN [ChildStatus] cs ON (csh.childStatusID = cs.StatusID 
	            AND cs.Description in ('Outras Saídas'))
            )");

        }
        
        public override void Down()
        {
        }
    }
}
