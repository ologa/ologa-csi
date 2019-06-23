namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateOldRoutineVisitDomainOrdersToMatchNewOnes : DbMigration
    {
        public override void Up()
        {
            //UPDATE PARA  FORTALECIMENTO ECONÓMICO
            Sql(@"UPDATE obj 
	            SET obj.SupportID = 6
	            FROM 
	            (
		            SELECT 
			            rvs.[RoutineVisitSupportID]
			            ,rvs.[SupportType]
			            ,rvs.[SupportID]
			            ,rvs.[SupportValue]
			            ,rvs.[RoutineVisitMemberID]
			            ,rv.RoutineVisitID
			            ,row_number() over(partition by rvm.[RoutineVisitMemberID] order by rvs.[RoutineVisitSupportID] ASC) as FirstDomainRow
		            FROM [RoutineVisitSupport] rvs 
		            JOIN [RoutineVisitMember] rvm ON rvs.[RoutineVisitMemberID] = rvm.[RoutineVisitMemberID]
		            JOIN [RoutineVisit] rv ON rv.[RoutineVisitID] = rvm.[RoutineVisitID]
		            JOIN [Beneficiary] ben ON ben.beneficiaryID = rvm.BeneficiaryID
		            JOIN [Household] hh ON rv.HouseholdID = hh.HouseholdID
		            WHERE rvs.[SupportType] in ('Domain','DomainEntity')
	            )obj
	            WHERE obj.FirstDomainRow = 1 AND [SupportID] = 3
	            AND obj.[SupportType] in ('Domain','DomainEntity')");

                //UPDATE PARA ALIMENTAÇÃO E NUTRIÇÃO
                Sql(@"UPDATE obj 
	            SET obj.SupportID = 1
	            FROM 
	            (
		            SELECT 
			            rvs.[RoutineVisitSupportID]
			            ,rvs.[SupportType]
			            ,rvs.[SupportID]
			            ,rvs.[SupportValue]
			            ,rvs.[RoutineVisitMemberID]
			            ,rv.RoutineVisitID
			            ,row_number() over(partition by rvm.[RoutineVisitMemberID] order by rvs.[RoutineVisitSupportID] ASC) as FirstDomainRow
		            FROM [RoutineVisitSupport] rvs 
		            JOIN [RoutineVisitMember] rvm ON rvs.[RoutineVisitMemberID] = rvm.[RoutineVisitMemberID]
		            JOIN [RoutineVisit] rv ON rv.[RoutineVisitID] = rvm.[RoutineVisitID]
		            JOIN [Beneficiary] ben ON ben.beneficiaryID = rvm.BeneficiaryID
		            JOIN [Household] hh ON rv.HouseholdID = hh.HouseholdID
		            WHERE rvs.[SupportType] in ('Domain','DomainEntity')
	            )obj
	            WHERE obj.FirstDomainRow = 2 AND [SupportID] = 1
	            AND obj.[SupportType] in ('Domain','DomainEntity')");

                //UPDATE PARA HABITAÇÃO
                Sql(@"UPDATE obj 
	            SET obj.SupportID = 7
	            FROM 
	            (
		            SELECT 
			            rvs.[RoutineVisitSupportID]
			            ,rvs.[SupportType]
			            ,rvs.[SupportID]
			            ,rvs.[SupportValue]
			            ,rvs.[RoutineVisitMemberID]
			            ,rv.RoutineVisitID
			            ,row_number() over(partition by rvm.[RoutineVisitMemberID] order by rvs.[RoutineVisitSupportID] ASC) as FirstDomainRow
		            FROM [RoutineVisitSupport] rvs 
		            JOIN [RoutineVisitMember] rvm ON rvs.[RoutineVisitMemberID] = rvm.[RoutineVisitMemberID]
		            JOIN [RoutineVisit] rv ON rv.[RoutineVisitID] = rvm.[RoutineVisitID]
		            JOIN [Beneficiary] ben ON ben.beneficiaryID = rvm.BeneficiaryID
		            JOIN [Household] hh ON rv.HouseholdID = hh.HouseholdID
		            WHERE rvs.[SupportType] in ('Domain','DomainEntity')
	            )obj
	            WHERE obj.FirstDomainRow = 3 AND [SupportID] = 2
	            AND obj.[SupportType] in ('Domain','DomainEntity')");

                //UPDATE PARA EDUCAÇÃO
                Sql(@"UPDATE obj 
	            SET obj.SupportID = 2
	            FROM 
	            (
		            SELECT 
			            rvs.[RoutineVisitSupportID]
			            ,rvs.[SupportType]
			            ,rvs.[SupportID]
			            ,rvs.[SupportValue]
			            ,rvs.[RoutineVisitMemberID]
			            ,rv.RoutineVisitID
			            ,row_number() over(partition by rvm.[RoutineVisitMemberID] order by rvs.[RoutineVisitSupportID] ASC) as FirstDomainRow
		            FROM [RoutineVisitSupport] rvs 
		            JOIN [RoutineVisitMember] rvm ON rvs.[RoutineVisitMemberID] = rvm.[RoutineVisitMemberID]
		            JOIN [RoutineVisit] rv ON rv.[RoutineVisitID] = rvm.[RoutineVisitID]
		            JOIN [Beneficiary] ben ON ben.beneficiaryID = rvm.BeneficiaryID
		            JOIN [Household] hh ON rv.HouseholdID = hh.HouseholdID
		            WHERE rvs.[SupportType] in ('Domain','DomainEntity')
	            )obj
	            WHERE obj.FirstDomainRow = 4 AND [SupportID] = 4
	            AND obj.[SupportType] in ('Domain','DomainEntity')");

                //UPDATE PARA SAÚDE
                Sql(@"UPDATE obj 
	            SET obj.SupportID = 3
	            FROM 
	            (
		            SELECT 
			            rvs.[RoutineVisitSupportID]
			            ,rvs.[SupportType]
			            ,rvs.[SupportID]
			            ,rvs.[SupportValue]
			            ,rvs.[RoutineVisitMemberID]
			            ,rv.RoutineVisitID
			            ,row_number() over(partition by rvm.[RoutineVisitMemberID] order by rvs.[RoutineVisitSupportID] ASC) as FirstDomainRow
		            FROM [CSI_PROD].[dbo].[RoutineVisitSupport] rvs 
		            JOIN [CSI_PROD].[dbo].[RoutineVisitMember] rvm ON rvs.[RoutineVisitMemberID] = rvm.[RoutineVisitMemberID]
		            JOIN [CSI_PROD].[dbo].[RoutineVisit] rv ON rv.[RoutineVisitID] = rvm.[RoutineVisitID]
		            JOIN [CSI_PROD].[dbo].[Beneficiary] ben ON ben.beneficiaryID = rvm.BeneficiaryID
		            JOIN [CSI_PROD].[dbo].[Household] hh ON rv.HouseholdID = hh.HouseholdID
		            WHERE rvs.[SupportType] in ('Domain','DomainEntity')
	            )obj
	            WHERE obj.FirstDomainRow = 5 AND [SupportID] = 7
	            AND obj.[SupportType] in ('Domain','DomainEntity')");

                //UPDATE PARA APOIO PSICOLÓGICO
                Sql(@"UPDATE obj 
	            SET obj.SupportID = 5
	            FROM 
	            (
		            SELECT 
			            rvs.[RoutineVisitSupportID]
			            ,rvs.[SupportType]
			            ,rvs.[SupportID]
			            ,rvs.[SupportValue]
			            ,rvs.[RoutineVisitMemberID]
			            ,rv.RoutineVisitID
			            ,row_number() over(partition by rvm.[RoutineVisitMemberID] order by rvs.[RoutineVisitSupportID] ASC) as FirstDomainRow
		            FROM [CSI_PROD].[dbo].[RoutineVisitSupport] rvs 
		            JOIN [CSI_PROD].[dbo].[RoutineVisitMember] rvm ON rvs.[RoutineVisitMemberID] = rvm.[RoutineVisitMemberID]
		            JOIN [CSI_PROD].[dbo].[RoutineVisit] rv ON rv.[RoutineVisitID] = rvm.[RoutineVisitID]
		            JOIN [CSI_PROD].[dbo].[Beneficiary] ben ON ben.beneficiaryID = rvm.BeneficiaryID
		            JOIN [CSI_PROD].[dbo].[Household] hh ON rv.HouseholdID = hh.HouseholdID
		            WHERE rvs.[SupportType] in ('Domain','DomainEntity')
	            )obj
	            WHERE obj.FirstDomainRow = 6 AND [SupportID] = 5
	            AND obj.[SupportType] in ('Domain','DomainEntity')");

                //PROTECCAO E APOIO LEGAL
                Sql(@"UPDATE obj 
	            SET obj.SupportID = 4
	            FROM 
	            (
		            SELECT 
			            rvs.[RoutineVisitSupportID]
			            ,rvs.[SupportType]
			            ,rvs.[SupportID]
			            ,rvs.[SupportValue]
			            ,rvs.[RoutineVisitMemberID]
			            ,rv.RoutineVisitID
			            ,row_number() over(partition by rvm.[RoutineVisitMemberID] order by rvs.[RoutineVisitSupportID] ASC) as FirstDomainRow
		            FROM [CSI_PROD].[dbo].[RoutineVisitSupport] rvs 
		            JOIN [CSI_PROD].[dbo].[RoutineVisitMember] rvm ON rvs.[RoutineVisitMemberID] = rvm.[RoutineVisitMemberID]
		            JOIN [CSI_PROD].[dbo].[RoutineVisit] rv ON rv.[RoutineVisitID] = rvm.[RoutineVisitID]
		            JOIN [CSI_PROD].[dbo].[Beneficiary] ben ON ben.beneficiaryID = rvm.BeneficiaryID
		            JOIN [CSI_PROD].[dbo].[Household] hh ON rv.HouseholdID = hh.HouseholdID
		            WHERE rvs.[SupportType] in ('Domain','DomainEntity')
	            )obj
	            WHERE obj.FirstDomainRow = 7 AND [SupportID] = 6
	            AND obj.[SupportType] in ('Domain','DomainEntity')");

        }
        
        public override void Down()
        {
        }
    }
}
