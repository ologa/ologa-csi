using EFDataAccess.DTO;
using EFDataAccess.UOW;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using VPPS.CSI.Domain;
using System.Dynamic;
using EFDataAccess.Utilities;

namespace EFDataAccess.Services
{
    public class DashboardQueryService : BaseService
    {
        private ReportService ReportService;

        public DashboardQueryService(UnitOfWork uow) : base(uow)
        {
            UnitOfWork = uow;
            ReportService = new ReportService(uow);
        }

        public List<SeriesData> GetBeneficiariesBySex(int SiteID, DateTime initialDate, DateTime finalDate)
        {
            return ReportService.GetDateBeneficiariesBySex(@"select count(*) as num, b.Gender
                                                            from Beneficiary b
                                                            left join ChildStatusHistory csh on (csh.BeneficiaryID = b.BeneficiaryID) and 
                                                            csh.ChildStatusHistoryID = (select MAX(ChildStatusHistoryID) from ChildStatusHistory where BeneficiaryID = b.BeneficiaryID and csh.ChildStatusID in (1, 2))
                                                            and b.RegistrationDate between @initialDate and @lastDate
                                                            inner join HouseHold hh on(hh.HouseHoldID = b.HouseholdID)
                                                            inner join Partner p on(hh.PartnerID = p.PartnerID)
                                                            inner join Site s on(s.SiteID = p.SiteID)
                                                            where s.SiteID = OCB or OCB = 0
                                                            group by b.Gender".Replace("OCB", "'" + SiteID + "'"), initialDate, finalDate);
        }

        public List<SeriesDataComulative> GetBeneficiariesByAge(int SiteID, DateTime initialDate, DateTime finalDate)
        {
            return ReportService.GetDateBeneficiariesByAge(@"select name, count(*) as value, [sequence]
                            from (select DATEDIFF(year, CAST(ben.DateOfBirth As Date), GETDATE()) as age,
                            name = case 
                            when DATEDIFF(year, CAST(ben.DateOfBirth As Date), GETDATE()) between 0 and 4  then '0-4' 
                            when DATEDIFF(year, CAST(ben.DateOfBirth As Date), GETDATE()) between 5 and 9  then '5-9'
                            when DATEDIFF(year, CAST(ben.DateOfBirth As Date), GETDATE()) between 10 and 14  then '10-14'
                            when DATEDIFF(year, CAST(ben.DateOfBirth As Date), GETDATE()) between 15 and 17  then '15-17'
                            else 'Adultos'
                            end,
                            [sequence] = case 
                            when DATEDIFF(year, CAST(ben.DateOfBirth As Date), GETDATE()) between 0 and 4  then 0 
                            when DATEDIFF(year, CAST(ben.DateOfBirth As Date), GETDATE()) between 5 and 9  then 1
                            when DATEDIFF(year, CAST(ben.DateOfBirth As Date), GETDATE()) between 10 and 14  then 2
                            when DATEDIFF(year, CAST(ben.DateOfBirth As Date), GETDATE()) between 15 and 17  then 3
                            else 4
                            end 
                            from Beneficiary ben
                            inner join HouseHold hh on (hh.HouseHoldID = ben.HouseholdID)
                            inner join Partner p on (p.PartnerID = hh.PartnerID)
                            inner join Site s on (p.siteID = s.SiteID) and s.SiteID = OCB or OCB like 0
                            and ben.RegistrationDate between @initialDate and @lastDate
                            ) a
                            group by name, sequence 
                            order by sequence".Replace("OCB", "'" + SiteID + "'"), initialDate, finalDate);

        }

        public List<SeriesDataComulative> GetTargetAndBeneficiariesBySite()
        {
            return ReportService.getDataComulative(
                 "select sum(GoalNumber) as value, s.SiteName as name, tot_ben.total as comulative " +
                 "from SiteGoal sg " +
                 "inner join Site s on (s.SiteID = sg.SiteID) " +
                 "inner join (" +
                 "select sum(total) as total, SiteID, SiteName " +
                 "from(select count(*) as total, s.SiteID, s.SiteName " +
                 "from Child c " +
                 "inner join ChildStatusHistory csh " +
                 "on (csh.ChildID = c.ChildID) " +
                 "and csh.ChildStatusHistoryID = " +
                 "   (select MAX(ChildStatusHistoryID) from ChildStatusHistory " +
                 "       where ChildID = c.ChildID and csh.ChildStatusID in (1,2)) " +
                 "inner join HouseHold hh on(hh.HouseHoldID = c.HouseholdID) " +
                 "inner join Partner p on(hh.PartnerID = p.PartnerID) " +
                 "inner join Site s on(s.SiteID = p.siteID) " +
                 "group by s.SiteID, s.SiteName " +
                 "union all " +
                 "select count(*) as total, s.SiteID, s.SiteName " +
                 "from Adult a " +
                 "inner join HouseHold hh on (hh.HouseHoldID = a.HouseholdID) " +
                 "inner join Partner p on (hh.PartnerID = p.PartnerID) " +
                 "inner join Site s on (s.SiteID = p.siteID) " +
                 "    group by s.SiteID, s.SiteName) q " +
                 "    group by SiteID, SiteName " +
                 ") tot_ben on(tot_ben.SiteID = sg.SiteID) " +
                 "group by s.SiteName, tot_ben.total");
        }
        public List<dynamic> GetAverageBeneficiariesByChiefPartner(int SiteID)
        {
            string query = @"SELECT 
	                            cp.Name,
	                            round(CAST(count_beneficiaries.TotalBen as FLOAT) / CAST(count_simple_partner.SimplePartnerTotal as FLOAT), 0) as AverageBenPerPartner
                            FROM 
	                            [Partner] cp
	                            INNER JOIN 
		                            (SELECT
			                            cp.PartnerID AS ChiefPartnerID,
			                            cp.Name AS ChiefPartnerName,
			                            COUNT(p.partnerID) AS SimplePartnerTotal
		                            FROM 
				                            [Partner] AS cp
			                            INNER JOIN  [Partner] AS p ON cp.PartnerID = p.SuperiorID 
		                            WHERE p.CollaboratorRoleID = 1 
		                            GROUP BY cp.PartnerID, cp.Name) 
	                            count_simple_partner on (count_simple_partner.ChiefPartnerID = cp.PartnerID)
	                            INNER JOIN 
	                            (SELECT
		                            p.SuperiorId AS ChiefPartnerID,
		                            count(ben.BeneficiaryID) TotalBen
	                            FROM 
		                            [Partner] p
		                            INNER JOIN HouseHold hh on (hh.PartnerID = p.PartnerID)
		                            INNER JOIN Beneficiary ben on (ben.HouseholdID = hh.HouseHoldID)
		                            INNER JOIN  [ChildStatusHistory] csh 
	                                ON (csh.ChildStatusHistoryID = (SELECT max(csh2.ChildStatusHistoryID) FROM  [ChildStatusHistory] csh2
	                                WHERE csh2.EffectiveDate<= GETDATE() AND csh2.BeneficiaryID = ben.BeneficiaryID))
	                                INNER JOIN  [ChildStatus] ct ON (csh.childStatusID = ct.StatusID AND ct.Description in ('Inicial'))
	                            WHERE p.CollaboratorRoleID = 1 
	                            GROUP BY p.SuperiorId
	                            ) count_beneficiaries on (count_beneficiaries.ChiefPartnerID = cp.PartnerID)
                            ";

            DatabaseHelper dataBaseHelper = new DatabaseHelper();
            return dataBaseHelper.ExecuteQuery(query, new ExpandoObject());

        }

        public List<dynamic> GetCountOfMonthlyReferencesDoneAndCompletedForTARVAndATS(int SiteID)
        {
            string query = @"       select
										year,
										month,
										done_art,
										done_ats,
										completed_art,
										completed_ats,
										case 
											when done_ats = 0 then 0
											else CAST((completed_ats * 100 / done_ats) as FLOAT) 
										end as percentage_completed_ats,
										case 
											when done_art = 0 then 0
											else CAST((completed_art * 100 / done_art) as FLOAT) 
										end as percentage_completed_art,
										(case month
			                            when  1 then 'Jan'  
			                            when  2 then 'Fev'  
			                            when  3 then 'Mar'  
			                            when  4 then 'Abr'  
			                            when  5 then 'Mai'  
			                            when  6 then 'Jun'  
			                            when  7 then 'Jul'  
			                            when  8 then 'Ago'  
			                            when  9 then 'Set'  
			                            when  10 then 'Out'  
			                            when  11 then 'Nov'  
			                            when  12 then 'Dez'  
			                            else '' 
		                            end) +'-'+cast(year as varchar(4)) as month_year
									from
									(
									select
										YEAR(q_done_ref.IndividualDate) as year,
										MONTH(q_done_ref.IndividualDate) as month,
										sum(q_done_ref.done_art) as done_art,
										sum(q_done_ref.done_ats) as done_ats,
										sum(q_completed_ref.completed_art) as completed_art,
										sum(q_completed_ref.completed_ats) as completed_ats
									from 
									(
										select 
											IndividualDate,
											sum(done_ats) as done_ats,
											sum(done_art) as done_art
										from 
										(
											select 
												dr.IndividualDate,
												 case 
													when rt.ReferenceName = 'ATS' then 1
													else 0
												end as done_ats,
												case 
													when rt.ReferenceName = 'TARV' then 1
													else 0
												end as done_art
											 from
												DateRange('d', @initialDate, @lastDate) dr
												left join ReferenceService rs on (rs.ReferenceDate = dr.IndividualDate)
												left join Reference r on (r.ReferenceServiceID = rs.ReferenceServiceID AND (r.Value <> '0' AND r.Value <> '' AND r.Value IS NOT NULL))
												left join ReferenceType rt on (rt.ReferenceTypeID = r.ReferenceTypeID AND rt.ReferenceCategory = 'Activist')
											) q
										 group by IndividualDate
									) as q_done_ref
									left join 
										(
											select	
												IndividualDate,
												sum(completed_ats) as completed_ats,
												sum(completed_art) as completed_art
											from 
											(select 
												dr.IndividualDate,
												case 
													when rt.ReferenceName = 'ATS' then 1
													else 0
												end as completed_ats,
												case 
													when rt.ReferenceName = 'TARV' then 1
													else 0
												end as completed_art
											 from
											 DateRange('d', @initialDate, @lastDate) dr
											left join ReferenceService rs on ((rs.HealthAttendedDate = dr.IndividualDate) OR (rs.SocialAttendedDate = dr.IndividualDate))
											left join Reference r on (r.ReferenceServiceID = rs.ReferenceServiceID AND (r.Value <> '0' AND r.Value <> '' AND r.Value IS NOT NULL))
											left join ReferenceType rt on (rt.ReferenceTypeID = r.ReferenceTypeID AND rt.ReferenceCategory in ('Health','Social'))
											) q
											group by IndividualDate
										) as q_completed_ref on q_completed_ref.IndividualDate = q_done_ref.IndividualDate
										group by YEAR(q_done_ref.IndividualDate), MONTH(q_done_ref.IndividualDate)
									) q order by year, month";

            DateTime lastMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, 15);
            DateTime lastYear = new DateTime(lastMonth.Year - 1, 9, 16);
            DatabaseHelper dataBaseHelper = new DatabaseHelper();
            dynamic sqlParameters = new ExpandoObject();
            sqlParameters.initialDate = lastYear;
            sqlParameters.lastDate = lastMonth;
            sqlParameters.SiteID = SiteID;
            return dataBaseHelper.ExecuteQuery(query, sqlParameters);
        }


        public List<dynamic> GetCountOfMonthlyReferencesDoneAndCompleted(int SiteID)
        {
            string query = @"select
										year,
										month,
										done_references,
										completed_references,
									    case
		                                    when done_references = 0 then 0
		                                    else round(CAST(completed_references * 100 / done_references as FLOAT), 0) 
	                                    end as percentage_completed_references,
										(case month
			                            when  1 then 'Jan'  
			                            when  2 then 'Fev'  
			                            when  3 then 'Mar'  
			                            when  4 then 'Abr'  
			                            when  5 then 'Mai'  
			                            when  6 then 'Jun'  
			                            when  7 then 'Jul'  
			                            when  8 then 'Ago'  
			                            when  9 then 'Set'  
			                            when  10 then 'Out'  
			                            when  11 then 'Nov'  
			                            when  12 then 'Dez'  
			                            else '' 
		                            end) +'-'+cast(year as varchar(4)) as month_year
									from
									(select
										YEAR(q_done_ref.IndividualDate) as year,
										MONTH(q_done_ref.IndividualDate) as month,
										sum(done_references) as done_references,
										sum(completed_references) as completed_references
									from 
									(select 
										dr.IndividualDate,
										count(rs.ReferenceServiceID) done_references
									 from
									 DateRange('d', @initialDate, @lastDate) dr
									left join ReferenceService rs on (rs.ReferenceDate = dr.IndividualDate)
		                            left join Reference r on (r.ReferenceServiceID = rs.ReferenceServiceID AND (r.Value <> '0' AND r.Value <> '' AND r.Value IS NOT NULL))
		                            left join ReferenceType rt on (rt.ReferenceTypeID = r.ReferenceTypeID AND rt.ReferenceCategory = 'Activist')
									group by dr.IndividualDate
									) as q_done_ref
									left join 
										(select 
											dr.IndividualDate,
											count(rs.ReferenceServiceID) completed_references
										 from
										 DateRange('d', @initialDate, @lastDate) dr
										left join ReferenceService rs on ((rs.HealthAttendedDate = dr.IndividualDate) OR (rs.SocialAttendedDate = dr.IndividualDate))
										left join Reference r on (r.ReferenceServiceID = rs.ReferenceServiceID AND (r.Value <> '0' AND r.Value <> '' AND r.Value IS NOT NULL))
										left join ReferenceType rt on (rt.ReferenceTypeID = r.ReferenceTypeID AND rt.ReferenceCategory in ('Health','Social'))
										group by dr.IndividualDate
										) q_completed_ref on q_completed_ref.IndividualDate = q_done_ref.IndividualDate
										group by YEAR(q_done_ref.IndividualDate), MONTH(q_done_ref.IndividualDate)
									) q order by year, month";

            DateTime lastMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, 15);
            DateTime lastYear = new DateTime(lastMonth.Year - 1, 9, 16);
            DatabaseHelper dataBaseHelper = new DatabaseHelper();
            dynamic sqlParameters = new ExpandoObject();
            sqlParameters.initialDate = lastYear;
            sqlParameters.lastDate = lastMonth;
            sqlParameters.SiteID = SiteID;
            return dataBaseHelper.ExecuteQuery(query, sqlParameters);
        }

        public List<dynamic> GetTotalBeneficiariesEnrolledByMonthAnd(int SiteID)
        {
            string Query = @"select 
				q.*, 
				CAST((ISNULL(c.value, 0) * 100/q.value) as FLOAT) as child_value,
				CAST((ISNULL(c.comulative, 0) * 100/q.comulative) as FLOAT) as child_comulative
			from 
			(select Total as value, sum(Total) over (order by Year, Month) comulative, MonthYearStr as name
				from (
				select a.Month, a.Year, count(*) Total,
				 (
					case a.Month 
						when  1 then 'Jan'  
						when  2 then 'Fev'  
						when  3 then 'Mar'  
						when  4 then 'Abr'  
						when  5 then 'Mai'  
						when  6 then 'Jun'  
						when  7 then 'Jul'  
						when  8 then 'Ago'  
						when  9 then 'Set'  
						when  10 then 'Out'  
						when  11 then 'Nov'  
						when  12 then 'Dez'  
						else '' 
					end   
				 ) +'-'+cast(a.Year as varchar(4)) MonthYearStr
				from (
				select Month(b.RegistrationDate) Month,
						Year(b.RegistrationDate) Year
				from Beneficiary b
				inner join HouseHold hh on (b.HouseholdID = hh.HouseHoldID)
				inner join Partner p on (hh.PartnerID = p.PartnerID)
				inner join Site s on (p.siteID = s.SiteID) and s.SiteID = @SiteID or @SiteID = 0
				where b.RegistrationDate Between @initialDate and @lastDate
				 
				) a 
				group by a.Month, a.Year
				) b) q 
				left join 
				(select Total as value, sum(Total) over (order by Year, Month) comulative, MonthYearStr as name
				from (
				select a.Month, a.Year, count(*) Total,
				 (
					case a.Month 
						when  1 then 'Jan'  
						when  2 then 'Fev'  
						when  3 then 'Mar'  
						when  4 then 'Abr'  
						when  5 then 'Mai'  
						when  6 then 'Jun'  
						when  7 then 'Jul'  
						when  8 then 'Ago'  
						when  9 then 'Set'  
						when  10 then 'Out'  
						when  11 then 'Nov'  
						when  12 then 'Dez'  
						else '' 
					end   
				 ) +'-'+cast(a.Year as varchar(4)) MonthYearStr
				from (
				select Month(b.RegistrationDate) Month,
						Year(b.RegistrationDate) Year
				from Beneficiary b
				inner join HouseHold hh on (b.HouseholdID = hh.HouseHoldID)
				inner join Partner p on (hh.PartnerID = p.PartnerID)
				inner join Site s on (p.siteID = s.SiteID) and s.SiteID = @SiteID or @SiteID = 0
				where b.RegistrationDate Between @initialDate and @lastDate
					and DATEDIFF(year, CAST(b.DateOfBirth AS Date), GETDATE()) < 18
				) a 
				group by a.Month, a.Year
				) b) c on (c.name = q.name)";

            DateTime lastMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, 15);
            DateTime lastYear = new DateTime(lastMonth.Year - 1, 9, 16);
            DatabaseHelper dataBaseHelper = new DatabaseHelper();
            dynamic sqlParameters = new ExpandoObject();
            sqlParameters.initialDate = lastYear;
            sqlParameters.lastDate = lastMonth;
            sqlParameters.SiteID = SiteID;
            return dataBaseHelper.ExecuteQuery(Query, sqlParameters);
        }
        public List<dynamic> GetMonthlyLeavesWithoutGraduation(int SiteID)
        {
            string query = @"select
	                            year,
	                            month,
	                            month_year,
	                            total,
	                            cumulative,
	                            death,
	                            other,
	                            lost,
	                            transfer_pepfar,
	                            transfer_other,
	                            quit
                            from
	                            (select 
		                            year,
		                            month,
		                            month_year,
		                            (sum(death) + sum(other) + sum(lost) + sum(transfer_pepfar) + sum(transfer_other) + sum(quit)) as total,
		                            sum(sum(death) + sum(other) + sum(lost) + sum(transfer_pepfar) + sum(transfer_other) + sum(quit)) over (order by year, month) as cumulative,
		                            sum(death) as death,
		                            sum(other) as other,
		                            sum(lost) as lost,
		                            sum(transfer_pepfar) as transfer_pepfar,
		                            sum(transfer_other) as transfer_other,
		                            sum(quit) as quit
	                            from 
		                            (select 
				                            Month(dr.IndividualDate) as month,
				                            Year(dr.IndividualDate) as year,
			                            (
				                            case Month(dr.IndividualDate) 
					                            when  1 then 'Jan'  
					                            when  2 then 'Fev'  
					                            when  3 then 'Mar'  
					                            when  4 then 'Abr'  
					                            when  5 then 'Mai'  
					                            when  6 then 'Jun'  
					                            when  7 then 'Jul'  
					                            when  8 then 'Ago'  
					                            when  9 then 'Set'  
					                            when  10 then 'Out'  
					                            when  11 then 'Nov'  
					                            when  12 then 'Dez'  
					                            else '' 
				                            end   
				                            ) +'-'+cast(Year(dr.IndividualDate) as varchar(4)) as month_year
			                            , 
			                            1 as total,
			                            case 
				                            when cs.Description = 'Óbito' then 1
				                            else 0
			                            end as death,
			                            case 
				                            when cs.Description = 'Outras Saídas' then 1
				                            else 0
			                            end as other,
			                            case 
				                            when cs.Description = 'Perdido' then 1
				                            else 0
			                            end as lost,
			                            case 
				                            when cs.Description = 'Transferido p/ programas de PEPFAR' then 1
				                            else 0
			                            end as transfer_pepfar,
			                            case 
				                            when cs.Description = 'Transferido p/ programas NÃO de PEPFAR)' then 1
				                            else 0
			                            end as transfer_other,
			                            case 
				                            when cs.Description = 'Desistência' then 1
				                            else 0
			                            end as quit
		                            from 
			                            DateRange('d', @initialDate, @lastDate) as dr
			                            left join ChildStatusHistory csh on (csh.EffectiveDate = dr.IndividualDate)
			                            left join ChildStatus cs on (cs.StatusID = csh.ChildStatusID and 
										                            cs.Description not in ('Graduação', 'Inicial', 'Adulto') and 
										                            cs.Description not like 'Eliminado%')
	                            ) q
	                            group by q.year, q.month, q.month_year
                            ) f order by f.year, f.month
                            ";

            DateTime lastMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, 15);
            DateTime lastYear = new DateTime(lastMonth.Year - 1, 9, 16);
            DatabaseHelper dataBaseHelper = new DatabaseHelper();
            dynamic sqlParameters = new ExpandoObject();
            sqlParameters.initialDate = lastYear;
            sqlParameters.lastDate = lastMonth;
            sqlParameters.SiteID = SiteID;
            return dataBaseHelper.ExecuteQuery(query, sqlParameters);
        }

        public List<dynamic> GetMonthlyGraduatedBeneficiariesChildPercentageAndCumulative(int SiteID)
        {
            string query = @"select
	                            year,
	                            month,
	                            month_year,
	                            total_graduated,
	                            graduated_cumulative,
	                            percentage_child_graduated,
	                            case
		                            when  graduated_cumulative = 0 then 0
		                            else CAST((child_graduated_cumulative * 100 / graduated_cumulative) as FLOAT) 
	                            end as percent_child_graduated_cumulative
                            from (select
	                            qb.year,
	                            qb.month,
	                            qb.month_year,
	                            qb.total_graduated,
	                            sum(qb.total_graduated) over (order by qb.year, qb.month) graduated_cumulative,
	                            case
		                            when  qb.total_graduated = 0 then 0
		                            else CAST((qc.total_graduated * 100 / qb.total_graduated) as FLOAT) 
	                            end as percentage_child_graduated,
	                            qc.comulative as child_graduated_cumulative
                            from (select 
	                            year,
	                            month,
	                            month_year,
	                            sum(total) as total_graduated--,
	                            --sum(total) over (order by year, month) comulative
                            from 
	                            (select 
		                            Month(dr.IndividualDate) as month,
		                            Year(dr.IndividualDate) as year,
	                            (
		                            case Month(dr.IndividualDate) 
			                            when  1 then 'Jan'  
			                            when  2 then 'Fev'  
			                            when  3 then 'Mar'  
			                            when  4 then 'Abr'  
			                            when  5 then 'Mai'  
			                            when  6 then 'Jun'  
			                            when  7 then 'Jul'  
			                            when  8 then 'Ago'  
			                            when  9 then 'Set'  
			                            when  10 then 'Out'  
			                            when  11 then 'Nov'  
			                            when  12 then 'Dez'  
			                            else '' 
		                            end   
		                            ) +'-'+cast(Year(dr.IndividualDate) as varchar(4)) as month_year
	                            , 
	                            count(csh.ChildStatusHistoryID) as total
                            from 
	                            DateRange('d', @initialDate, @lastDate) as dr
	                            left join ChildStatusHistory csh on (csh.EffectiveDate = dr.IndividualDate)
	                            left join ChildStatus cs on (cs.StatusID = csh.ChildStatusID and cs.Description = 'Graduação')
                            group by dr.IndividualDate
                            ) q 
                            group by q.month_year, q.year, q.month
                            ) qb 
                            inner join 
                            (select
	                            year,
	                            month,
	                            month_year,
	                            total_graduated,
	                            sum(total_graduated) over (order by year, month) comulative
                            from (select 
	                            year,
	                            month,
	                            month_year,
	                            sum(total) as total_graduated
                            from 
	                            (select 
		                            Month(dr.IndividualDate) as month,
		                            Year(dr.IndividualDate) as year,
	                            (
		                            case Month(dr.IndividualDate) 
			                            when  1 then 'Jan'  
			                            when  2 then 'Fev'  
			                            when  3 then 'Mar'  
			                            when  4 then 'Abr'  
			                            when  5 then 'Mai'  
			                            when  6 then 'Jun'  
			                            when  7 then 'Jul'  
			                            when  8 then 'Ago'  
			                            when  9 then 'Set'  
			                            when  10 then 'Out'  
			                            when  11 then 'Nov'  
			                            when  12 then 'Dez'  
			                            else '' 
		                            end   
		                            ) +'-'+cast(Year(dr.IndividualDate) as varchar(4)) as month_year
	                            , 
	                            count(csh.ChildStatusHistoryID) as total
                            from 
	                            DateRange('d', @initialDate, @lastDate) as dr
	                            left join ChildStatusHistory csh on (csh.EffectiveDate = dr.IndividualDate 
		                            and csh.BeneficiaryID in (select BeneficiaryID from Beneficiary where DATEDIFF(year, CAST(DateOfBirth AS Date), GETDATE()) < 18))
	                            left join ChildStatus cs on (cs.StatusID = csh.ChildStatusID and cs.Description = 'Graduação')	
                            group by dr.IndividualDate
                            ) q 
                            group by q.month_year, q.year, q.month
                            ) q) qc on (qc.month_year = qb.month_year)
                            ) f";

            DateTime lastMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, 15);
            DateTime lastYear = new DateTime(lastMonth.Year - 1, 9, 16);
            DatabaseHelper dataBaseHelper = new DatabaseHelper();
            dynamic sqlParameters = new ExpandoObject();
            sqlParameters.initialDate = lastYear;
            sqlParameters.lastDate = lastMonth;
            sqlParameters.SiteID = SiteID;
            return dataBaseHelper.ExecuteQuery(query, sqlParameters);
        }
        public List<dynamic> GetAverageBeneficiariesByPartner(int SiteID)
        {
            String query = @"select
	                            ROUND(CAST(max(total_ben) as FLOAT) / 
		                            CAST((select count(*) from Partner where CollaboratorRoleID = 1 and Active = 1 and siteID = @SiteID) as FLOAT), 0) AverageBeneficiaries,
	                            year,
	                            month,
	                            (case month
		                            when  1 then 'Jan'  
		                            when  2 then 'Fev'  
		                            when  3 then 'Mar'  
		                            when  4 then 'Abr'  
		                            when  5 then 'Mai'  
		                            when  6 then 'Jun'  
		                            when  7 then 'Jul'  
		                            when  8 then 'Ago'  
		                            when  9 then 'Set'  
		                            when  10 then 'Out'  
		                            when  11 then 'Nov'  
		                            when  12 then 'Dez'  
		                            else '' 
	                            end) +'-'+cast(year as varchar(4)) as month_year
                            from
	                            (select 
		                            count(ben.BeneficiaryID) total_ben,
		                            dr.IndividualDate,
		                            YEAR(dr.IndividualDate) as year,
		                            MONTH(dr.IndividualDate) as Month
	                            from
	                            DateRange('d', @initialDate, @lastDate) dr
	                            left join HouseHold hh on (hh.RegistrationDate <= dr.IndividualDate)
	                            left join Partner p on (p.PartnerID = hh.PartnerID and p.siteID = @SiteID)
	                            left join Beneficiary ben on (ben.HouseholdID  = hh.HouseHoldID)
	                            left join ChildStatusHistory csh 
	                            on (csh.ChildStatusHistoryID = (SELECT max(csh2.ChildStatusHistoryID) FROM  [ChildStatusHistory] csh2
												                            WHERE 
												                            csh2.EffectiveDate<= dr.IndividualDate  AND 
												                            ben.BeneficiaryID = csh2.BeneficiaryID))
	                            left join ChildStatus cs on (cs.StatusID = csh.ChildStatusID and cs.Description in ('Inicial', 'Graduação') )
	                            group by dr.IndividualDate, YEAR(dr.IndividualDate), MONTH(dr.IndividualDate)) q
	                            group by year, month
	                            order by year, month
                            ";
            DateTime lastMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, 15);
            DateTime lastYear = new DateTime(lastMonth.Year - 1, 9, 16);
            DatabaseHelper dataBaseHelper = new DatabaseHelper();
            dynamic sqlParameters = new ExpandoObject();
            sqlParameters.initialDate = lastYear;
            sqlParameters.lastDate = lastMonth;
            sqlParameters.SiteID = SiteID;
            return dataBaseHelper.ExecuteQuery(query, sqlParameters);
        }
        public List<dynamic> GetMonthlyBeneficiariesCountKnownStatus(int SiteID)
        {
            String query = @"select
	                        ROUND(CAST(total_ben * 100 / q_child_count.total_childs as FLOAT), 0) as average_child_knowing_hiv_status,
	                        q.year,
	                        q.month,
	                        (case q.month
		                        when  1 then 'Jan'  
		                        when  2 then 'Fev'  
		                        when  3 then 'Mar'  
		                        when  4 then 'Abr'  
		                        when  5 then 'Mai'  
		                        when  6 then 'Jun'  
		                        when  7 then 'Jul'  
		                        when  8 then 'Ago'  
		                        when  9 then 'Set'  
		                        when  10 then 'Out'  
		                        when  11 then 'Nov'  
		                        when  12 then 'Dez'  
		                        else '' 
	                        end) +'-'+cast(q.year as varchar(4)) as month_year
                        from 
	                        (select
		                        count(hs.HIVStatusID) total_ben,
		                        YEAR(dr.IndividualDate) as year,
		                        MONTH(dr.IndividualDate) as month
	                        from
		                        DateRange('d', @initialDate, @lastDate) dr
		                        left join HIVStatus hs on (hs.InformationDate = dr.IndividualDate)
		                        left join Beneficiary ben on (ben.BeneficiaryID = hs.BeneficiaryID and DATEDIFF(year, CAST(ben.DateOfBirth As Date), GETDATE()) < 18)
		                        left join ChildStatusHistory csh 
		                        on (csh.ChildStatusHistoryID = (SELECT max(csh2.ChildStatusHistoryID) FROM  [ChildStatusHistory] csh2
													                        WHERE 
													                        csh2.EffectiveDate<= dr.IndividualDate  AND 
													                        ben.BeneficiaryID = csh2.BeneficiaryID))
		                        left join ChildStatus cs on (cs.StatusID = csh.ChildStatusID and cs.Description in ('Inicial', 'Graduação') )
	                        group by
		                        YEAR(dr.IndividualDate), MONTH(dr.IndividualDate)
	                        ) q 
	                        inner join
	                        (select 
			                        max(total_ben) as total_childs,
			                        year,
			                        month
	                        from
		                        (select 
			                        dr.IndividualDate,
			                        YEAR(dr.IndividualDate) as year,
			                        MONTH(dr.IndividualDate) as month,
			                        (select 
				                        count(ben.BeneficiaryID)
			                        from
				                        Beneficiary ben
				                        inner join HouseHold hh on (hh.HouseHoldID = ben.HouseholdID)
				                        inner join ChildStatusHistory csh 
				                        on (csh.ChildStatusHistoryID = (SELECT max(csh2.ChildStatusHistoryID) FROM  [ChildStatusHistory] csh2
														                        WHERE 
														                        csh2.EffectiveDate<= dr.IndividualDate  AND 
														                        ben.BeneficiaryID = csh2.BeneficiaryID))
				                        inner join ChildStatus cs on (cs.StatusID = csh.ChildStatusID and cs.Description in ('Inicial', 'Graduação') )
			                        where 
				                        hh.RegistrationDate <= dr.IndividualDate and DATEDIFF(year, CAST(ben.DateOfBirth As Date), dr.IndividualDate) < 18
			                        ) as total_ben
		                        from
			                        DateRange('d', @initialDate, @lastDate) dr
		                        ) q
	                        group by year, month
	                        ) q_child_count on (q_child_count.year = q.year and q_child_count.month = q.month)
	                        order by q.year, q.month";

            DateTime lastMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, 15);
            DateTime lastYear = new DateTime(lastMonth.Year - 1, 9, 16);
            DatabaseHelper dataBaseHelper = new DatabaseHelper();
            dynamic sqlParameters = new ExpandoObject();
            sqlParameters.initialDate = lastYear;
            sqlParameters.lastDate = lastMonth;
            sqlParameters.SiteID = SiteID;
            return dataBaseHelper.ExecuteQuery(query, sqlParameters);
        }
        public List<dynamic> GetMonthlyEnrolledBeneficiariesByOrigingReference(int SiteID)
        {
            string Query = @"select
					Month as month, Year as year, MonthYearStr as month_year, 
							CAST((community * 100 / total) as FLOAT) as community,
							CAST((key_pops_partner * 100 / total) as FLOAT) as key_pops_partner,
							CAST((health_facility * 100 / total) as FLOAT) as health_facility,
							CAST((clinical_partner * 100 / total) as FLOAT) as clinical_partner,
							CAST((indeterminate * 100 / total) as FLOAT) as indeterminate,
							CAST((other * 100 / total) as FLOAT) as other,
							CAST((none_above * 100 / total) as FLOAT) as none_above
				from (select Month, Year, MonthYearStr,sum(total) as total, sum(community) as community, sum(key_pops_partner) as key_pops_partner, 
						sum(health_facility) as health_facility, sum(clinical_partner) as clinical_partner, 
						sum(indeterminate) as indeterminate, sum(other) as other, sum(none_above) as none_above
				from (select Month(b.RegistrationDate) Month,
						Year(b.RegistrationDate) Year,
						(
					case Month(b.RegistrationDate) 
						when  1 then 'Janeiro'  
						when  2 then 'Fevereiro'  
						when  3 then 'Marco'  
						when  4 then 'Abril'  
						when  5 then 'Maio'  
						when  6 then 'Junho'  
						when  7 then 'Julho'  
						when  8 then 'Agosto'  
						when  9 then 'Setembro'  
						when  10 then 'Outubro'  
						when  11 then 'Novembro'  
						when  12 then 'Dezembro'  
						else '' 
					end   
				 ) +'-'+cast(Year(b.RegistrationDate) as varchar(4)) as MonthYearStr,
						1 as total,
						CASE 
							WHEN se.Description = 'Comunidade' THEN 1
							ELSE 0
						END AS community,
						CASE
							WHEN se.Description = 'Parceiros de Populacoes-Chave' THEN 1
							ELSE 0
						END AS key_pops_partner,
						CASE 
							WHEN se.Description = 'Unidade Sanitária' THEN 1
							ELSE 0
						END AS health_facility,
						CASE 
							WHEN se.Description = 'Parceiro Clínico' THEN 1
							ELSE 0
						END AS clinical_partner,
						CASE 
							WHEN se.Description = 'Indeterminado' THEN 1
							ELSE 0
						END AS indeterminate,
						CASE 
							WHEN se.Description = 'Outra' THEN 1
							ELSE 0
						END AS other,
						CASE 
							WHEN se.Description = 'Nenhuma' THEN 1
							ELSE 0
						END AS none_above
				from Beneficiary b
				inner join HouseHold hh on (b.HouseholdID = hh.HouseHoldID)
				inner join Partner p on (hh.PartnerID = p.PartnerID)
				inner join Site s on (p.siteID = s.SiteID) and s.SiteID = @SiteID or @SiteID = 0
				inner join SimpleEntity se on (se.SimpleEntityID = hh.FamilyOriginRefID)
				where b.RegistrationDate Between @initialDate and @lastDate) q
				group by q.Month, q.Year, q.MonthYearStr) q";

            DateTime lastMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, 15);
            DateTime lastYear = new DateTime(lastMonth.Year - 1, 9, 16);
            DatabaseHelper dataBaseHelper = new DatabaseHelper();
            dynamic sqlParameters = new ExpandoObject();
            sqlParameters.initialDate = lastYear;
            sqlParameters.lastDate = lastMonth;
            sqlParameters.SiteID = SiteID;
            return dataBaseHelper.ExecuteQuery(Query, sqlParameters);
        }
        public List<dynamic> GetMonthlyBeneficiariesPercentageInART(int SiteID)
        {
            String query = @"select 
			                    ROUND(CAST(max(total_ben_in_art) * 100 / max(total_ben) as FLOAT), 0) as percentage_child_in_art,
			                    year,
			                    month,
			                        (case month
		                                                when  1 then 'Jan'  
		                                                when  2 then 'Fev'  
		                                                when  3 then 'Mar'  
		                                                when  4 then 'Abr'  
		                                                when  5 then 'Mai'  
		                                                when  6 then 'Jun'  
		                                                when  7 then 'Jul'  
		                                                when  8 then 'Ago'  
		                                                when  9 then 'Set'  
		                                                when  10 then 'Out'  
		                                                when  11 then 'Nov'  
		                                                when  12 then 'Dez'  
		                                                else '' 
	                                                end) +'-'+cast(year as varchar(4)) as month_year
	                    from
		                    (select 
			                    dr.IndividualDate,
			                    YEAR(dr.IndividualDate) as year,
			                    MONTH(dr.IndividualDate) as month,
			                    (select 
				                    count(ben.BeneficiaryID)
			                    from
				                    Beneficiary ben
				                    inner join HouseHold hh on (hh.HouseHoldID = ben.HouseholdID)
				                    inner join ChildStatusHistory csh 
				                    on (csh.ChildStatusHistoryID = (SELECT max(csh2.ChildStatusHistoryID) FROM  [ChildStatusHistory] csh2
														                    WHERE 
														                    csh2.EffectiveDate<= dr.IndividualDate  AND 
														                    ben.BeneficiaryID = csh2.BeneficiaryID))
				                    inner join ChildStatus cs on (cs.StatusID = csh.ChildStatusID and cs.Description in ('Inicial', 'Graduação') )
			                    where 
				                    hh.RegistrationDate <= dr.IndividualDate and DATEDIFF(year, CAST(ben.DateOfBirth As Date), dr.IndividualDate) < 18
			                    ) as total_ben,
			
			                    (select 
				                    count(ben.BeneficiaryID)
			                    from
				                    Beneficiary ben
				                    inner join HouseHold hh on (hh.HouseHoldID = ben.HouseholdID)
				                    inner join HIVStatus hs on (hs.BeneficiaryID = ben.BeneficiaryID and hs.HIVInTreatment = 0)
				                    inner join ChildStatusHistory csh 
				                    on (csh.ChildStatusHistoryID = (SELECT max(csh2.ChildStatusHistoryID) FROM  [ChildStatusHistory] csh2
														                    WHERE 
														                    csh2.EffectiveDate<= dr.IndividualDate  AND 
														                    ben.BeneficiaryID = csh2.BeneficiaryID))
				                    inner join ChildStatus cs on (cs.StatusID = csh.ChildStatusID and cs.Description in ('Inicial', 'Graduação') )
			                    where 
				                    hh.RegistrationDate <= dr.IndividualDate and DATEDIFF(year, CAST(ben.DateOfBirth As Date), dr.IndividualDate) < 18
			                    ) as total_ben_in_art
		                    from
			                    DateRange('d', @initialDate, @lastDate) dr
		                    ) q
	                    group by year, month
	                    order by year, month";

            DateTime lastMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, 15);
            DateTime lastYear = new DateTime(lastMonth.Year - 1, 9, 16);
            DatabaseHelper dataBaseHelper = new DatabaseHelper();
            dynamic sqlParameters = new ExpandoObject();
            sqlParameters.initialDate = lastYear;
            sqlParameters.lastDate = lastMonth;
            sqlParameters.SiteID = SiteID;
            return dataBaseHelper.ExecuteQuery(query, sqlParameters);
        }

        public List<SeriesDataComulative> GetTotalBenificiariesByMonth(int SiteID)
        {
            String Query = @"select Total as value, sum(Total) over (order by Year, Month) comulative, MonthYearStr as name
				from (
				select a.Month, a.Year, count(*) Total,
				 (
					case a.Month 
						when  1 then 'Jan'  
						when  2 then 'Fev'  
						when  3 then 'Mar'  
						when  4 then 'Abr'  
						when  5 then 'Mai'  
						when  6 then 'Jun'  
						when  7 then 'Jul'  
						when  8 then 'Ago'  
						when  9 then 'Set'  
						when  10 then 'Out'  
						when  11 then 'Nov'  
						when  12 then 'Dez'  
						else '' 
					end   
				 ) +'-'+cast(a.Year as varchar(4)) MonthYearStr
				from (
				select Month(b.RegistrationDate) Month,
						Year(b.RegistrationDate) Year
				from Beneficiary b
				inner join HouseHold hh on (b.HouseholdID = hh.HouseHoldID)
				inner join Partner p on (hh.PartnerID = p.PartnerID)
				inner join Site s on (p.siteID = s.SiteID) and s.SiteID = @SiteID or @SiteID = 0
				where b.RegistrationDate Between @initialDate and @lastDate
				 
				) a 
				group by a.Month, a.Year
				) b";

            DateTime lastMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, 15);
            DateTime lastYear = new DateTime(lastMonth.Year - 1, 9, 16);

            return UnitOfWork.DbContext.Database.SqlQuery<SeriesDataComulative>
                (Query,
                new SqlParameter("initialDate", lastYear),
                new SqlParameter("lastDate", lastMonth),
                new SqlParameter("SiteID", SiteID))
                .ToList();
        }

        public List<SeriesDataComulative2> GetTotalReferencesAndComulativeByMonth(int SiteID)
        {
            String Query = @"select  (  
                	case month  
                 		when  1 then 'Jan'  
                		when  2 then 'Fev'  
                		when  3 then 'Mar'  
                    	when  4 then 'Abr'  
                		when  5 then 'Mai'  
                		when  6 then 'Jun'  
                		when  7 then 'Jul'  
                		when  8 then 'Ago'  
                		when  9 then 'Set'  
                		when  10 then 'Out'  
                		when  11 then 'Nov'  
                		when  12 then 'Dez'  
                		else '' end 
					) + '-' + cast(year as varchar(4)) as name
                , tot_done as value, sum(tot_done) over (order by year, month) as comulative, tot_complete as value1, sum(tot_complete) over (order by year, month) as comulative1
                from 
					(select done_ref.month, done_ref.year, done_ref.total as tot_done, complete_ref.total as tot_complete
					from (select count(*) as total, Month(ReferenceDate) as month, Year(ReferenceDate) as year 
					from ReferenceService rs  
					inner join Reference r on (r.ReferenceServiceID = rs.ReferenceServiceID) 
					inner join ReferenceType rt on (rt.ReferenceTypeID = r.ReferenceTypeID) and  
					rt.ReferenceCategory = 'Activist' and r.Value not in ('0','')  
					inner join Child c on (c.ChildID = rs.ChildID)  
					inner join HouseHold hh on (hh.HouseHoldID = c.HouseholdID)  
					inner join Partner p on(p.PartnerID = hh.PartnerID)  
					inner join Site s on(s.SiteID = p.siteID)  
				    where s.SiteID = @SiteID or @SiteID = 0  and 
					rs.ReferenceDate Between @initialDate and @lastDate
					group by Month(ReferenceDate), Year(ReferenceDate)) done_ref
					inner join  
					(select count(*) as total, Month(ReferenceDate) as month, Year(ReferenceDate) as year
					from ReferenceService rs  
					inner join Reference r on (r.ReferenceServiceID = rs.ReferenceServiceID)  
					inner join ReferenceType rt on (rt.ReferenceTypeID = r.ReferenceTypeID) and  
					rt.ReferenceCategory in ('Health','Social') and r.Value = '1' 
					inner join Child c on (c.ChildID = rs.ChildID)  
					inner join HouseHold hh on (hh.HouseHoldID = c.HouseholdID)  
					inner join Partner p on(p.PartnerID = hh.PartnerID)  
					inner join Site s on(s.SiteID = p.siteID)  
				    where s.SiteID = @SiteID or @SiteID = 0  and
					rs.ReferenceDate Between @initialDate and @lastDate
					group by Month(ReferenceDate), Year(ReferenceDate)) complete_ref 
					on (complete_ref.month = done_ref.month and complete_ref.year = done_ref.year)) q";

            DateTime lastMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, 21);
            DateTime lastYear = new DateTime(lastMonth.Year - 1, lastMonth.Month, 20);

            return UnitOfWork.DbContext.Database.SqlQuery<SeriesDataComulative2>
               (Query,
               new SqlParameter("initialDate", lastYear),
               new SqlParameter("lastDate", lastMonth),
               new SqlParameter("SiteID", SiteID))
               .ToList();
        }

        public List<SeriesDataBeneficiaryStates> GetBeneficiariesStates(int SiteID, DateTime initialDate, DateTime finalDate)
        {

            return ReportService.getDataBeneficiaryStates(@"SELECT 'Inicial' as State,
		                            ISNULL(ChildInitialState, 0) as ChildCount,
		                            ISNULL(AdultInitialState,0) AdultCount,
		                            ISNULL((ChildInitialState + AdultInitialState), 0) as Total
		                            FROM
			                            (SELECT	
					                            SUM(CASE WHEN ct.Description='Inicial' and 
												DATEDIFF(year, CAST(ben.DateOfBirth As Date), GETDATE()) < 18 then 1 else 0 END) AS ChildInitialState,
					                            SUM(CASE WHEN ct.Description='Inicial' and 
												DATEDIFF(year, CAST(ben.DateOfBirth As Date), GETDATE()) >= 18 then 1 else 0 END) AS AdultInitialState
				                            FROM
				                            [dbo].[HouseHold] hh 
				                            INNER JOIN  [Beneficiary] ben ON ben.HouseHoldID = hh.HouseHoldID
				                            INNER JOIN  [Partner] p on (p.PartnerID = hh.PartnerID)
				                            INNER JOIN  [Site] s on (s.SiteID = p.siteID) and s.SiteID = OCB or OCB = 0
				                            INNER JOIN  [ChildStatusHistory] csh 
				                            ON (csh.ChildStatusHistoryID = (SELECT max(csh2.ChildStatusHistoryID) FROM  [ChildStatusHistory] csh2
				                            WHERE 
											csh2.EffectiveDate<= @lastDate AND 
											ben.BeneficiaryID = csh2.BeneficiaryID))
				                            INNER JOIN  [ChildStatus] ct ON (csh.childStatusID = ct.StatusID) and ct.Description not in ('Adulto') 
				                            AND ben.RegistrationDate between @initialDate and @lastDate) a
                            union all
                            SELECT 'Graduação' as State,
		                            ISNULL(ChildGraduationState,0) as ChildCount,
		                            ISNULL(AdultGraduationState,0) AdultCount,
		                            ISNULL((ChildGraduationState + AdultGraduationState),0) as Total
		                            FROM
			                            (SELECT	
					                            SUM(CASE WHEN ct.Description='Graduação' and 
												DATEDIFF(year, CAST(ben.DateOfBirth As Date), GETDATE()) < 18 then 1 else 0 END) AS ChildGraduationState,
					                            SUM(CASE WHEN ct.Description='Graduação' and 
												DATEDIFF(year, CAST(ben.DateOfBirth As Date), GETDATE()) >= 18 then 1 else 0 END) AS AdultGraduationState
				                            FROM
				                            [dbo].[HouseHold] hh 
				                            INNER JOIN  [Beneficiary] ben ON ben.HouseHoldID = hh.HouseHoldID
				                            INNER JOIN  [Partner] p on (p.PartnerID = hh.PartnerID)
				                            INNER JOIN  [Site] s on (s.SiteID = p.siteID) and s.SiteID = OCB or OCB = 0
				                            INNER JOIN  [ChildStatusHistory] csh 
				                            ON (csh.ChildStatusHistoryID = (SELECT max(csh2.ChildStatusHistoryID) FROM  [ChildStatusHistory] csh2
				                            WHERE 
											csh2.EffectiveDate<= @lastDate AND 
											ben.BeneficiaryID = csh2.BeneficiaryID))
				                            INNER JOIN  [ChildStatus] ct ON (csh.childStatusID = ct.StatusID) and ct.Description not in ('Adulto') 
				                            AND ben.RegistrationDate between @initialDate and @lastDate) b
                            union all
                            SELECT 'Saidos sem graduação' as State,
		                            ISNULL(ChildOtherState,0) as ChildCount,
		                            ISNULL(AdultOtherState,0) AdultCount,
		                            ISNULL((ChildOtherState + AdultOtherState),0) as Total
		                            FROM
			                            (SELECT	
					                            SUM(CASE WHEN ct.Description in('Transferido p/ programas NÃO de PEPFAR)', 'Transferido p/ programas de PEPFAR', 'Desistência', 'Perdido', 'Óbito') and
												DATEDIFF(year, CAST(ben.DateOfBirth As Date), GETDATE()) < 18 then 1 else 0 END) AS ChildOtherState,
					                            SUM(CASE WHEN ct.Description in('Transferido p/ programas NÃO de PEPFAR)', 'Transferido p/ programas de PEPFAR', 'Desistência', 'Perdido', 'Óbito') and 
												DATEDIFF(year, CAST(ben.DateOfBirth As Date), GETDATE()) >= 18 then 1 else 0 END) AS AdultOtherState
				                            FROM
				                            [dbo].[HouseHold] hh 
				                            INNER JOIN  [Beneficiary] ben ON ben.HouseHoldID = hh.HouseHoldID
				                            INNER JOIN  [Partner] p on (p.PartnerID = hh.PartnerID)
				                            INNER JOIN  [Site] s on (s.SiteID = p.siteID) and s.SiteID = OCB or OCB = 0
				                            INNER JOIN  [ChildStatusHistory] csh 
				                            ON (csh.ChildStatusHistoryID = (SELECT max(csh2.ChildStatusHistoryID) FROM  [ChildStatusHistory] csh2
				                            WHERE 
											csh2.EffectiveDate<= @lastDate AND 
											ben.BeneficiaryID = csh2.BeneficiaryID))
				                            INNER JOIN  [ChildStatus] ct ON (csh.childStatusID = ct.StatusID) and ct.Description not in ('Adulto') 
				                            AND ben.RegistrationDate between @initialDate and @lastDate) b".Replace("OCB", "'" + SiteID + "'"), initialDate, finalDate);
        }

        public List<SeriesDataComulative> GetChildsFrom0to5WhoReceivedDPI(int SiteID, DateTime initialDate, DateTime finalDate)
        {
            /* return ReportService.getDataComulative(
                 "select COUNT(DISTINCT(c.ChildID)) as value, s.SiteName as name, " +
                     "(select count(*) " +
                     "from Child c " +
                     "inner join ChildStatusHistory csh on (csh.ChildID = c.ChildID) " +
                     "and csh.ChildStatusHistoryID = (select MAX(ChildStatusHistoryID) from ChildStatusHistory  " +
                     "where ChildID = c.ChildID and csh.ChildStatusID in (1,2)) " +
                     "inner join HouseHold hh on(hh.HouseHoldID = c.HouseholdID) " +
                     "inner join Partner p on(p.PartnerID = hh.PartnerID)  " +
                     "inner join Site s on(s.SiteID = p.siteID) and s.SiteName like 'AMODEFA' or '' like ''  " +
                     "where ((YEAR(GETDATE()) - YEAR(c.DateOfBirth)) between 0 and 5)) as comulative  " +
                     "from Child c   " +
                     "inner join ChildStatusHistory csh   " +
                     "on (csh.ChildID = c.ChildID)  " +
                     "and csh.ChildStatusHistoryID =   " +
                     "(select MAX(ChildStatusHistoryID) from ChildStatusHistory   " +
                     "where ChildID = c.ChildID and csh.ChildStatusID in (1,2))  " +
                     "inner join RoutineVisitMember rvm on(c.ChildID = rvm.ChildID)   " +
                     "inner join RoutineVisitSupport rvs on(rvs.RoutineVisitMemberID = rvm.RoutineVisitMemberID)  " +
                     "and rvs.SupportType = 'DPI' and rvs.SupportValue != '0'   " +
                     "inner join HouseHold hh on(hh.HouseHoldID = c.HouseholdID)  " +
                     "inner join Partner p on(p.PartnerID = hh.PartnerID)   " +
                     "inner join Site s on(s.SiteID = p.siteID)   " +
                     "where(YEAR(GETDATE()) - YEAR(DateOfBirth)) between 0 and 5  " +
                     "and s.SiteID = " + SiteID + " or " + SiteID + " = 0 " +
                     "group by s.SiteName ");

             */

            return ReportService.GetDateChildsFrom0to5WhoReceivedDPI(@"
                                select COUNT(DISTINCT(c.ChildID)) as value, s.SiteName as name ,
                                    (select count(*)  
                                    from Child c  

					                inner join ChildStatusHistory csh on (csh.ChildID = c.ChildID)  
                                    and csh.ChildStatusHistoryID = (select MAX(ChildStatusHistoryID) from ChildStatusHistory   
                                    where ChildID = c.ChildID and csh.ChildStatusID in (1,2))  
                                    inner join HouseHold hh on(hh.HouseHoldID = c.HouseholdID)  
                                    inner join Partner p on(p.PartnerID = hh.PartnerID)   
                                    inner join Site s on(s.SiteID = p.siteID) and s.SiteName like 'AMODEFA' or '' like ''   
                                    where ((YEAR(GETDATE()) - YEAR(c.DateOfBirth)) between 0 and 5)) as comulative   
                                    from Child c    
                                    inner join ChildStatusHistory csh    
                                    on (csh.ChildID = c.ChildID)   
                                    and csh.ChildStatusHistoryID =    
                                    (select MAX(ChildStatusHistoryID) from ChildStatusHistory    
                                    where ChildID = c.ChildID and csh.ChildStatusID in (1,2))   
                                    inner join RoutineVisitMember rvm on(c.ChildID = rvm.ChildID)    
                                    inner join RoutineVisitSupport rvs on(rvs.RoutineVisitMemberID = rvm.RoutineVisitMemberID)   
                                    and rvs.SupportType = 'DPI' and rvs.SupportValue != '0'    
					                and rvs.CreatedDate  between @initialDate and @lastDate 
                                    inner join HouseHold hh on(hh.HouseHoldID = c.HouseholdID)   
                                    inner join Partner p on(p.PartnerID = hh.PartnerID)    
                                    inner join Site s on(s.SiteID = p.siteID)    
                                    where  (YEAR(GETDATE()) - YEAR(DateOfBirth)) between 0 and 5   
                                    and s.SiteID =   OCB   or   OCB   = 0  
					 
                                    group by s.SiteName ".Replace("OCB", "'" + SiteID + "'"), initialDate, finalDate);
        }

        //public List<SeriesData> GetAverageBenificiariesByPartner(int SiteID)
        //{
        //    /*
        //    return ReportService.getData(
        //        "select SUM(q.num)/(select count(*) from Partner p " +
        //        "       where p.siteID = q.SiteID and CollaboratorRoleID = 1) as value " +
        //        ", q.SiteName as name " +
        //        "from " +
        //        "(select count(a.Gender) as num, s.SiteName, s.SiteID " +
        //        "from Adult a " +
        //        "inner join HouseHold h on (h.HouseHoldID = a.HouseholdID) " +
        //        "inner join Partner p on (h.PartnerID = p.PartnerID) " +
        //        "inner join Site s on (s.SiteID = p.siteID) " +
        //        "group by s.SiteName, s.SiteID " +
        //        "union " +
        //        "select count(c.Gender) as num, s.SiteName, s.SiteID " +
        //        "from Child c " +
        //        "inner join ChildStatusHistory csh on (c.ChildID = csh.ChildID) " +
        //        "and csh.ChildStatusHistoryID = " +
        //        "   (select MAX(ChildStatusHistoryID) from ChildStatusHistory " +
        //        "       where ChildID = c.ChildID and csh.ChildStatusID in (1,2)) " +
        //        "inner join HouseHold h on (h.HouseHoldID = c.HouseholdID) " +
        //        "inner join Partner p on (h.PartnerID = p.PartnerID) " +
        //        "inner join Site s on (s.SiteID = p.siteID) " +
        //        "group by s.SiteName, s.SiteID " +
        //        ") q " +
        //        "where q.SiteID = " + SiteID + " or " + SiteID + " = 0 " +
        //        "group by q.SiteName, q.SiteID");
        //        */

        //    string Query = @"select SUM(q.num)/(select count(*) from Partner p  
        //                    where p.siteID = q.SiteID and CollaboratorRoleID = 1) as value  
        //                    , q.SiteName as name  
        //                    from  
        //                    (select count(a.Gender) as num, s.SiteName, s.SiteID  
        //                    from Adult a  
        //                    inner join Beneficiary B1 on B1.ID=a.AdultId and B1.Type='adult'
        //                    inner join HouseHold h on (h.HouseHoldID = a.HouseholdID)  
        //                    inner join Partner p on (h.PartnerID = p.PartnerID)  
        //                    inner join Site s on (s.SiteID = p.siteID)  
        //                    group by s.SiteName, s.SiteID  
        //                    union  
        //                    select count(c.Gender) as num, s.SiteName, s.SiteID  
        //                    from Child c  
        //                    inner join Beneficiary B2 on B2.ID=c.ChildID and B2.Type='child'
        //                    inner join ChildStatusHistory csh on (c.ChildID = csh.ChildID)  
        //                    and csh.ChildStatusHistoryID =  
        //                    (select MAX(ChildStatusHistoryID) from ChildStatusHistory  
        //                    where ChildID = c.ChildID and csh.ChildStatusID in (1,2)
        //                    and B2.RegistrationDate between @initialDate and @lastDate
        //                    )  
        //                    inner join HouseHold h on (h.HouseHoldID = c.HouseholdID)  
        //                    inner join Partner p on (h.PartnerID = p.PartnerID)  
        //                    inner join Site s on (s.SiteID = p.siteID)  
        //                    group by s.SiteName, s.SiteID  
        //                    ) q  
        //                    where q.SiteID =   @SiteID   or   @SiteID   = 0  
        //                    group by q.SiteName, q.SiteID";

        //    DateTime lastMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, 21);
        //    DateTime lastYear = new DateTime(lastMonth.Year - 1, lastMonth.Month, 20);

        //    return UnitOfWork.DbContext.Database.SqlQuery<SeriesData>
        //        (Query,
        //        new SqlParameter("initialDate", lastYear),
        //        new SqlParameter("lastDate", lastMonth),
        //        new SqlParameter("SiteID", SiteID))
        //        .ToList();

        //}

        //public List<SeriesData> GetFamilyOriginReferencesByOCB(int SiteID)
        //{
        //    /*
        //    return ReportService.getData(
        //        "select count(*) as value, se.Description as name " +
        //        "from HouseHold hh " +
        //        "   inner join SimpleEntity se on (se.SimpleEntityID = hh.FamilyOriginRefID) " +
        //        "   inner join Partner p on (p.PartnerID = hh.PartnerID) " +
        //        "   inner join Site s on (s.SiteID = p.siteID) " +
        //        "   where s.SiteID = " + SiteID + " or " + SiteID + " = 0 " +
        //        "   group by se.Description");
        //    */

        //    string Query = @"select count(*) as value, se.Description as name  
        //                    from HouseHold hh  
        //                    inner join SimpleEntity se on (se.SimpleEntityID = hh.FamilyOriginRefID)  
        //                    inner join Partner p on (p.PartnerID = hh.PartnerID)  
        //                    inner join Site s on (s.SiteID = p.siteID)  
        //                    where (hh.RegistrationDate between  @initialDate and @lastDate ) and (s.SiteID =   s.SiteID   or   s.SiteID   = 0 ) 
        //                    group by se.Description";

        //    DateTime lastMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, 21);
        //    DateTime lastYear = new DateTime(lastMonth.Year - 1, lastMonth.Month, 20);

        //    return UnitOfWork.DbContext.Database.SqlQuery<SeriesData>
        //        (Query,
        //        new SqlParameter("initialDate", lastYear),
        //        new SqlParameter("lastDate", lastMonth),
        //        new SqlParameter("SiteID", SiteID))
        //        .ToList();



        //}

        public List<SeriesDecimalData> GetCHASSandHealthFacilityFamilyOriginReferencesByOCB(int SiteID, DateTime initialDate, DateTime finalDate)
        {
            /*
             return ReportService.getDecimalData(
                 @"select 
                         ROUND(
                         (
                             CAST(count(hh.HouseHoldID) as FLOAT) 
                             /
                             CAST(
                                 (select count(*) 
                                  from HouseHold h
                                     inner join Partner p1 on (p1.PartnerID = h.PartnerID)
                                     inner join Site s1 on (s1.SiteID = p1.siteID) and s1.SiteID = OCB or OCB = 0 
                                 ) as FLOAT) * 100.00), 2
                         ) as value, 
                         s.SiteName as name
                     from HouseHold hh  
                         inner join SimpleEntity se on (se.SimpleEntityID = hh.FamilyOriginRefID)
                             and se.Description in ('Unidade Sanitária', 'Parceiro Clínico') 
                         inner join Partner p on (p.PartnerID = hh.PartnerID)  
                         inner join Site s on (s.SiteID = p.siteID)  
                         where s.SiteID = OCB or OCB = 0  
                     group by s.SiteName".Replace("OCB", SiteID.ToString()));
           * /
             string Query = @"select 
                             ROUND(
                             (
                             CAST(count(hh.HouseHoldID)  as FLOAT) 
                             /
                             CAST(
                             (select count(*) 
                             from HouseHold h
                             inner join Partner p1 on (p1.PartnerID = h.PartnerID)
                             inner join Site s1 on (s1.SiteID = p1.siteID) and s1.SiteID = s1.SiteID or s1.SiteID = 0 

                             ) as  FLOAT) * 100.00), 2
                             )   value, 
                             s.SiteName   name
                             from HouseHold hh  
                             inner join SimpleEntity se on (se.SimpleEntityID = hh.FamilyOriginRefID)
                             and se.Description in ('Unidade Sanitária', 'Parceiro Clínico') 
                             inner join Partner p on (p.PartnerID = hh.PartnerID)  
                             inner join Site s on (s.SiteID = p.siteID)  

                             where ( hh.RegistrationDate between  @initialDate and @lastDate) and (s.SiteID = s.SiteID or s.SiteID = 0  )
                             group by s.SiteName";

             DateTime lastMonth = new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, 21);
             DateTime lastYear = new DateTime(lastMonth.Year - 1, lastMonth.Month, 20);

             return UnitOfWork.DbContext.Database.SqlQuery<SeriesDecimalData>
                 (Query,
                 new SqlParameter("initialDate", lastYear),
                 new SqlParameter("lastDate", lastMonth),
                 new SqlParameter("SiteID", SiteID))
                 .ToList();
             */

            return ReportService.GetDataGetCHASSandHealthFacilityFamilyOriginReferencesByOCB(@"select 
                             ROUND(
                             (
                             CAST(count(hh.HouseHoldID)  as FLOAT) 
                             /
                             CAST(
                             (select count(*) 
                             from HouseHold h
                             inner join Partner p1 on (p1.PartnerID = h.PartnerID)
                             inner join Site s1 on (s1.SiteID = p1.siteID) and s1.SiteID = OCB or OCB = 0 

                             ) as  FLOAT) * 100.00), 2
                             )   value, 
                             s.SiteName   name
                             from HouseHold hh  
                             inner join SimpleEntity se on (se.SimpleEntityID = hh.FamilyOriginRefID)
                             and se.Description in ('Unidade Sanitária', 'Parceiro Clínico') 
                             inner join Partner p on (p.PartnerID = hh.PartnerID)  
                             inner join Site s on (s.SiteID = p.siteID)  

                             where ( hh.RegistrationDate between  @initialDate and @lastDate) and (s.SiteID = OCB or OCB = 0  )
                             group by s.SiteName".Replace("OCB", "'" + SiteID + "'"), initialDate, finalDate);
        }

        public List<SeriesData> GetHIVCascade(DateTime initialDate, DateTime finalDate)
        {

            return ReportService.GetDateHIVCascade(@"-- 1ª barra - Total de Criancas (menor de 18 anos) Exclui saídas sem graduação 
                            select 
                            name, count as value, idx as comulative
                            from
                            (select 
                            1 as idx, 'Total de Criancas' as name, count(*) as count
                            from vw_beneficiary_details
                            where Type = 'child'
                            and BeneficiaryState not in ('Adulto', 'Perdido', 'Óbito', 'Transferido p/ programas NÃO de PEPFAR)', 'Transferido p/ programas de PEPFAR', 'Desistência')
                            and RegistrationDate between @initialdate and  @lastDate
                            -- 2ª Segunda barra – quantos conhecem o seu seroestado 
                            -- Soma: positivos + Negativos + Conhecem mas não revelaram + Teste nao recomendado
                            union
                            select 
                            2 as idx, 'Conhecem seu estado' as name, count(*) as count
                            from vw_beneficiary_details ben
                            inner join HIVStatus hs on (hs.HIVStatusID = ben.LastHIVStatusID) 
                            and hs.ChildID != 0
                            and 
                            (
                            (hs.HIV in ('P', 'N')) or 
                            (hs.HIV = 'U' and hs.HIVUndisclosedReason in (0, 2))
                            )
                            where ben.Type = 'child' and ben.BeneficiaryState not in ('Adulto', 'Perdido', 'Óbito', 'Transferido p/ programas NÃO de PEPFAR)', 'Transferido p/ programas de PEPFAR', 'Desistência')
                            and ben.RegistrationDate between @initialdate and  @lastDate
                            -- 3º barra – positivo em tarv e nao tarv e negativos
                            union
                            select 
                            3 as idx, 'Positivos (TARV e nao TARV) e Negativos' as name, count(*) as count
                            from vw_beneficiary_details ben
                            inner join HIVStatus hs on (hs.HIVStatusID = ben.LastHIVStatusID) 
                            and hs.ChildID is not null
                            and hs.HIV in ('P', 'N')
                            where ben.Type = 'child'and ben.BeneficiaryState not in ('Adulto', 'Perdido', 'Óbito', 'Transferido p/ programas NÃO de PEPFAR)', 'Transferido p/ programas de PEPFAR', 'Desistência')
                            and ben.RegistrationDate between @initialdate and  @lastDate
                            -- 4ª barra – HIV positivo  Soma: Em tarv e não em tarv (ou total HIV positivo)
                            union
                            select 
                            4 as idx, 'Total HIV Positivos ' as name, count(*) as count
                            from vw_beneficiary_details ben
                            inner join HIVStatus hs on (hs.HIVStatusID = ben.LastHIVStatusID) 
                            and hs.ChildID is not null
                            and hs.HIV = 'P'
                            where ben.Type = 'child' and ben.BeneficiaryState not in ('Adulto', 'Perdido', 'Óbito', 'Transferido p/ programas NÃO de PEPFAR)', 'Transferido p/ programas de PEPFAR', 'Desistência')
                            and ben.RegistrationDate between @initialdate and  @lastDate
                            -- 5ª  barara – Não em tarv (estado de HIV)
                            union
                            select 
                            5 as idx, 'Nao em TARV' as name, count(*) as count
                            from vw_beneficiary_details ben
                            inner join HIVStatus hs on (hs.HIVStatusID = ben.LastHIVStatusID) 
                            and hs.ChildID is not null
                            and hs.HIV = 'P' and hs.HIVInTreatment = 1
                            where ben.Type = 'child' and ben.BeneficiaryState in ('Adulto', 'Perdido', 'Óbito', 'Transferido p/ programas NÃO de PEPFAR)', 'Transferido p/ programas de PEPFAR', 'Desistência')
                            and ben.RegistrationDate between @initialdate and  @lastDate
                            -- 6ª  barra – Referidos para Tarv (Guia de referencia)
                            union
                            select 
                            6 as idx, 'Referidos para TARV' as name, count(distinct(c.ID)) as count 
                            from Reference r
                            inner join ReferenceService rs on (rs.ReferenceServiceID = r.ReferenceServiceID) 
                            --and rs.ReferenceDate >= '2017-06-01' and rs.ReferenceDate <= '2018-06-01'
                            inner join ReferenceType rt on (rt.ReferenceTypeID = r.ReferenceTypeID) 
                            and rt.ReferenceName in ('PTV', 'Testado HIV+', 'Pré-TARV/IO', 'PPE', 'TARV', 'Abandono TARV')
                            and rt.ReferenceCategory = 'Activist' and r.Value not in  ('0', '')
                            inner join vw_beneficiary_details c on (c.ID = rs.ChildID) and c.Type = 'child' 
                            and c.BeneficiaryState not in ('Adulto', 'Perdido', 'Óbito', 'Transferido p/ programas NÃO de PEPFAR)', 'Transferido p/ programas de PEPFAR', 'Desistência')
								  	 
                            -- 7ª barra - Referencia completa para TARV (Guia de referencia - contra referencia)
                            union
                            select 
                            7 as idx, 'Referencias Completas TARV', count(distinct(c.ID)) as count
                            from Reference r
                            inner join ReferenceService rs on (rs.ReferenceServiceID = r.ReferenceServiceID) 
                            --and rs.HealthAttendedDate >= '2017-06-01' and rs.HealthAttendedDate <= '2018-06-01'
                            and CONVERT(datetime, rs.SocialAttendedDate, 103) Between '2017-06-01' and '2018-06-01'
                            inner join ReferenceType rt on (rt.ReferenceTypeID = r.ReferenceTypeID) 
                            and rt.ReferenceName in ('PTV', 'Testado HIV+', 'Pré-TARV/IO', 'PPE', 'TARV', 'CD')
                            and rt.ReferenceCategory in ('Health','Social') and r.Value = '1'
                            inner join vw_beneficiary_details c on (c.ID = rs.ChildID) and c.Type = 'child' 
                            and c.BeneficiaryState not in ('Adulto', 'Perdido', 'Óbito', 'Transferido p/ programas NÃO de PEPFAR)', 'Transferido p/ programas de PEPFAR', 'Desistência')
						 
                            -- 8ª barra –testes não recomendados (Estado inicial de HIV)
                            union
                            select 
                            8 as idx, 'Testes nao recomendados' as name, count(*) as count
                            from vw_beneficiary_details ben
                            inner join HIVStatus hs on (hs.HIVStatusID = ben.HIVStatusID) 
                            and hs.ChildID is not null
                            and hs.HIV = 'U' and hs.HIVUndisclosedReason = 2
                            where ben.Type = 'child' and ben.BeneficiaryState not in ('Adulto', 'Perdido', 'Óbito', 'Transferido p/ programas NÃO de PEPFAR)', 'Transferido p/ programas de PEPFAR', 'Desistência')
                            and ben.RegistrationDate between @initialdate and @lastDate
                            -- 9ª barra – desconhecidos (Rastreado para HIV mas exclui os testes não recomendados)
                            union
                            select 
                            9 as idx, 'Desconhecidos' as name, count(*) as count
                            from vw_beneficiary_details ben
                            inner join HIVStatus hs on (hs.HIVStatusID = ben.HIVStatusID) 
                            and hs.ChildID is not null
                            and hs.HIV = 'U'
                            where ben.Type = 'child' and ben.RegistrationDate between @initialdate and @lastDate
                            -- 10º barra -  Referidos para ATS 
                            union
                            select 
                            10 as idx, 'Referidos para ATS' as name, count(distinct(c.ID)) as count 
                            from Reference r
                            inner join ReferenceService rs on (rs.ReferenceServiceID = r.ReferenceServiceID) 
                            --and rs.ReferenceDate >= '2017-06-01' and rs.ReferenceDate <= '2018-06-01'
                            inner join ReferenceType rt on (rt.ReferenceTypeID = r.ReferenceTypeID) 
                            and rt.ReferenceName in ('ATS')
                            and rt.ReferenceCategory = 'Activist' and r.Value not in  ('0', '')
                            inner join vw_beneficiary_details c on (c.ID = rs.ChildID) and c.Type = 'child' 
                            and c.BeneficiaryState not in ('Adulto', 'Perdido', 'Óbito', 'Transferido p/ programas NÃO de PEPFAR)', 'Transferido p/ programas de PEPFAR', 'Desistência')
					 	  
                            -- 11º Referencia completa para ATS
                            union
                            select 
                            11 as idx, 'Referencias Completas ATS', count(distinct(c.ID)) as count
                            from Reference r
                            inner join ReferenceService rs on (rs.ReferenceServiceID = r.ReferenceServiceID) 
                            --and rs.HealthAttendedDate >= '2017-06-01' and rs.HealthAttendedDate <= '2018-06-01'
                            and CONVERT(datetime, rs.SocialAttendedDate, 103) Between '2017-06-01' and '2018-06-01'
                            inner join ReferenceType rt on (rt.ReferenceTypeID = r.ReferenceTypeID) 
                            and rt.ReferenceName in ('ATS')
                            and rt.ReferenceCategory in ('Health','Social') and r.Value = '1'
                            inner join vw_beneficiary_details c on (c.ID = rs.ChildID) and c.Type = 'child' 
                            and c.BeneficiaryState not in ('Adulto', 'Perdido', 'Óbito', 'Transferido p/ programas NÃO de PEPFAR)', 'Transferido p/ programas de PEPFAR', 'Desistência')
                            where r.CreatedDate between @initialdate and @lastDate
                            ) q
                            order by comulative ", initialDate, finalDate);
        }
    }
}