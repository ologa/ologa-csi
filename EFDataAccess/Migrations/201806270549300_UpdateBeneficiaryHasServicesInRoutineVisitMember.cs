namespace EFDataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

    public partial class UpdateBeneficiaryHasServicesInRoutineVisitMember : DbMigration
    {
        public override void Up()
        {
            //Update RoutineVisitDate based on RoutineVisit
            Sql(@" UPDATE rvm
              SET rvm.RoutineVisitDate = rv.RoutineVisitDate
              FROM  [RoutineVisitMember] rvm
              INNER JOIN  [RoutineVisit] rv ON rv.RoutineVisitID = rvm.RoutineVisitID", false);

            //Update with 1 if beneficiary had at least 1 Service
            Sql(@"UPDATE rvm
            SET rvm.[BeneficiaryHasServices] = 1
            FROM  [RoutineVisitMember] rvm
            WHERE rvm.RoutineVisitMemberID in
            (
	            SELECT 
		            rvm.RoutineVisitMemberID
	            FROM
	            (
		            SELECT
		            a.RoutineVisitMemberID
		            FROM
		            (
			            SELECT  --  Group By Adult
				            routv_dt.RoutineVisitMemberID
				            ,ISNULL(SUM(routv_dt.FinaceAid),0) As FinanceAid
				            ,ISNULL(SUM(routv_dt.Health),0) As Health
				            ,ISNULL(SUM(routv_dt.Food),0) As Food
				            ,ISNULL(SUM(routv_dt.Education),0) As Education
				            ,ISNULL(SUM(routv_dt.LegalAdvice),0) As LegalAdvice
				            ,ISNULL(SUM(routv_dt.Housing),0) As Housing 
				            ,ISNULL(SUM(routv_dt.SocialAid),0) As SocialAid
				            ,ISNULL(SUM(routv_dt.SocialAid),0) As DPI
				            ,ISNULL(SUM(routv_dt.HIVRisk),0) As HIVRisk
				            ,ISNULL(SUM(routv_dt.MUACGreen),0) As MUACGreen
				            ,ISNULL(SUM(routv_dt.MUACYellow),0) As MUACYellow
				            ,ISNULL(SUM(routv_dt.MUACRed),0) As MUACRed
			            FROM
			            (
				            SELECT 
					            adultRVSummart.*
				            FROM  [Routine_Visit_Summary] adultRVSummart
				            inner join  [Adult] a ON adultRVSummart.AdultID = a.AdultID
				            AND adultRVSummart.AdultID IS NOT NULL
			            ) routv_dt
			            group by routv_dt.ChiefPartner, routv_dt.Partner, routv_dt.RoutineVisitMemberID
		            )a
		            WHERE (a.FinanceAid + a.Health  + a.Food + a.Education + a.LegalAdvice + a.Housing  + a.SocialAid 
			            + a.DPI + a.HIVRisk + a.MUACGreen + a.MUACYellow + a.MUACRed) > 0

		            UNION ALL

		            SELECT
			            c.RoutineVisitMemberID
		            FROM
		            (
			            SELECT  --  Group By Child
				            routv_dt.RoutineVisitMemberID
				            ,ISNULL(SUM(routv_dt.FinaceAid),0) As FinanceAid
				            ,ISNULL(SUM(routv_dt.Health),0) As Health
				            ,ISNULL(SUM(routv_dt.Food),0) As Food
				            ,ISNULL(SUM(routv_dt.Education),0) As Education
				            ,ISNULL(SUM(routv_dt.LegalAdvice),0) As LegalAdvice
				            ,ISNULL(SUM(routv_dt.Housing),0) As Housing 
				            ,ISNULL(SUM(routv_dt.SocialAid),0) As SocialAid
				            ,ISNULL(SUM(routv_dt.SocialAid),0) As DPI
				            ,ISNULL(SUM(routv_dt.HIVRisk),0) As HIVRisk
				            ,ISNULL(SUM(routv_dt.MUACGreen),0) As MUACGreen
				            ,ISNULL(SUM(routv_dt.MUACYellow),0) As MUACYellow
				            ,ISNULL(SUM(routv_dt.MUACRed),0) As MUACRed
			            FROM
			            (
				            SELECT 
					            ChildRVSummart.*
				            FROM  [Routine_Visit_Summary] ChildRVSummart
				            inner join  [Child] c ON ChildRVSummart.ChildID = c.ChildID
				            AND ChildRVSummart.ChildID IS NOT NULL
			            ) routv_dt
			            group by routv_dt.ChiefPartner, routv_dt.Partner, routv_dt.RoutineVisitMemberID
		            )c
		            WHERE (c.FinanceAid + c.Health  + c.Food + c.Education + c.LegalAdvice + c.Housing  + c.SocialAid 
			            + c.DPI + c.HIVRisk + c.MUACGreen + c.MUACYellow + c.MUACRed) > 0
	            )rvm
            )", false);

        }

        public override void Down()
        {
        }
    }
}
