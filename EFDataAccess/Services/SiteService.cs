using EFDataAccess.UOW;
using System;
using System.Collections.Generic;
using System.Linq;
using VPPS.CSI.Domain;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Data;

namespace EFDataAccess.Services
{
    public class SiteService : BaseService
    {
        public SiteService(UnitOfWork uow) : base(uow) { }

        private EFDataAccess.Repository.IRepository<Site> SiteRepository
        {
            get { return UnitOfWork.Repository<Site>(); }
        }

        public Site Reload(Site entity)
        {
            SiteRepository.FullReload(entity);
            return entity;
        }

        public List<GraduationCriteria> findAllGraduationCriteria()
        {
            return UnitOfWork.Repository<GraduationCriteria>().GetAll().ToList();
        }

        public List<OrgUnit> findAllOrgUnitLocations()
        {
            return UnitOfWork.Repository<OrgUnit>().GetAll().Where(e => e.OrgUnitType.OrgUnitTypeID == 3).ToList();
        }

        public List<OrgUnit> findAllOrgUnitLocationsOrderedByName()
        {
            return UnitOfWork.Repository<OrgUnit>().GetAll().Where(e => e.OrgUnitType.OrgUnitTypeID == 3).OrderBy(e => e.Name).ToList();
        }

        public List<Site> findAll()
        {
            return UnitOfWork.Repository<Site>().GetAll().Include(x => x.partners).Include(x => x.orgUnit).ToList();
        }

        public Site findSiteBySyncGuid(Guid SyncGuid)
        {
            return UnitOfWork.Repository<Site>().Get().Where(x => x.SyncGuid == SyncGuid).SingleOrDefault();
        }

        public SiteGoal findSiteGoalBySyncGuid(Guid SyncGuid)
        {
            return UnitOfWork.Repository<SiteGoal>().Get().Where(x => x.SyncGuid == SyncGuid).SingleOrDefault();
        }

        public Site findById(int id)
        {
            return UnitOfWork.Repository<Site>().GetAll().Where(e => e.SiteID == id)
                .Include(e => e.orgUnit).Include(e => e.orgUnit.Parent).Include(e => e.SiteGoals).FirstOrDefault();
        }

        public IQueryable<Site> findSiteByName(string siteName)
        {
            return UnitOfWork.Repository<Site>().GetAll().Where(e => e.SiteName.Contains(siteName))
                .Include(e => e.orgUnit).Include(e => e.orgUnit.Parent).Include(e => e.SiteGoals)
                .Include(e => e.partners);
        }

        public void Delete(Site Site)
        {
            UnitOfWork.Repository<Site>().Delete(Site);
            UnitOfWork.Commit();
        }

        public void SaveOrUpdateSite(Site Site)
        {
            if (Site.SiteID > 0) { UnitOfWork.Repository<Site>().Update(Site); }
            else { UnitOfWork.Repository<Site>().Add(Site); }
        }

        public void SaveOrUpdateSiteGoal(SiteGoal SiteGoal)
        {
            if (SiteGoal.SiteGoalID > 0) { UnitOfWork.Repository<SiteGoal>().Update(SiteGoal); }
            else { UnitOfWork.Repository<SiteGoal>().Add(SiteGoal); }
        }

        public void CreateSiteGoals(Site site, int year)
        {
            DateTime date = new DateTime(year, 8, 31);
            List<SiteGoal> siteGoals = new List<SiteGoal>();

            siteGoals.Add(new SiteGoal { SiteID = site.SiteID, SitePerformanceComment = "", Indicator = "Número de beneficiários servido por programas de PEPFAR para OVC e famílias afetadas pelo HIV / AIDS", GoalDate = date, GoalNumber = 0 });
            siteGoals.Add(new SiteGoal { SiteID = site.SiteID, SitePerformanceComment = "", Indicator = "Número de referencias de saúde e outros serviços sociais", GoalDate = date, GoalNumber = 0 });
            siteGoals.Add(new SiteGoal { SiteID = site.SiteID, SitePerformanceComment = "", Indicator = "Número de referencias de saúde e outros serviços sociais designadas por completas", GoalDate = date, GoalNumber = 0 });
            siteGoals.Add(new SiteGoal { SiteID = site.SiteID, SitePerformanceComment = "", Indicator = "Número de agregados familiares recebendo Kit Familiar", GoalDate = date, GoalNumber = 0 });
            siteGoals.Add(new SiteGoal { SiteID = site.SiteID, SitePerformanceComment = "", Indicator = "Número de crianças dos 6 - 59 meses rastreados para malnutrição aguda ao nível comunitário(MUAC)", GoalDate = date, GoalNumber = 0 });
            siteGoals.Add(new SiteGoal { SiteID = site.SiteID, SitePerformanceComment = "", Indicator = "Número de crianças  6 - 59 meses com malnutrição aguda, detetados ao nível  da comunidade(Muac)", GoalDate = date, GoalNumber = 0 });
            siteGoals.Add(new SiteGoal { SiteID = site.SiteID, SitePerformanceComment = "", Indicator = "Percentagem de OVC com seroestado reportado ao parceiro de implementação(< 18 anos)", GoalDate = date, GoalNumber = 0 });
            siteGoals.Add(new SiteGoal { SiteID = site.SiteID, SitePerformanceComment = "", Indicator = "Percentagem de OVC com seroestado reportado ao parceiro de implementação(> 18 anos)", GoalDate = date, GoalNumber = 0 });
            siteGoals.Add(new SiteGoal { SiteID = site.SiteID, SitePerformanceComment = "", Indicator = "HIV - ", GoalDate = date, GoalNumber = 0 });
            siteGoals.Add(new SiteGoal { SiteID = site.SiteID, SitePerformanceComment = "", Indicator = "HIV + em TARV", GoalDate = date, GoalNumber = 0 });
            siteGoals.Add(new SiteGoal { SiteID = site.SiteID, SitePerformanceComment = "", Indicator = "HIV + Não em TARV", GoalDate = date, GoalNumber = 0 });
            siteGoals.Add(new SiteGoal { SiteID = site.SiteID, SitePerformanceComment = "", Indicator = "Seroestado conhecido não revelado", GoalDate = date, GoalNumber = 0 });
            siteGoals.Add(new SiteGoal { SiteID = site.SiteID, SitePerformanceComment = "", Indicator = "Desconhecido", GoalDate = date, GoalNumber = 0 });

            siteGoals.ForEach(s => UnitOfWork.Repository<SiteGoal>().Add(s));
        }

        public void DeleteSiteGoals(Site site, int year)
        {
            site = findById(site.SiteID);
            site.SiteGoals.Where(sg => sg.GoalDate.Year == year).ToList().ForEach(sg => UnitOfWork.Repository<SiteGoal>().Delete(sg));
        }

        public void DeleteSiteFull(Site site)
        {
            site.SiteGoals.ToList().ForEach(sg => UnitOfWork.Repository<SiteGoal>().Delete(sg));
            UnitOfWork.Repository<Site>().Delete(site);
        }

        public int ImportSiteGoals(string path)
        {
            UnitOfWork.DbContext.Configuration.AutoDetectChangesEnabled = false;
            FileImporter imp = new FileImporter();
            string fullPath = path + @"\SiteGoals.csv";
            DataTable dt1 = imp.ImportFromCSV(fullPath);

            // Site //

            int SiteGoalCount = 0;
            List<SiteGoal> SiteGoalsList = new List<SiteGoal>();
            UsersDB = ConvertListToHashtableUsers(findAllUsersUniqueEntities());

            foreach (DataRow row in dt1.Rows)
            {
                Guid SiteGoalGuid = new Guid(row["SiteGoal_guid"].ToString());
                SiteGoal SiteGoal = findSiteGoalBySyncGuid(SiteGoalGuid);

                if (SiteGoal == null)
                {
                    SiteGoal = new SiteGoal();
                    SiteGoal.SyncGuid = SiteGoalGuid;
                    Guid SiteGuid = new Guid(row["Site_guid"].ToString());
                    if (!SiteGuid.ToString().Contains("0000"))
                    { SiteGoal.Site = findSiteBySyncGuid(SiteGuid); }
                    SetCreationDataFields(SiteGoal, row, SiteGoalGuid);
                }

                SiteGoal.Indicator = row["Indicator"].ToString();
                SiteGoal.SitePerformanceComment = row["SitePerformanceComment"].ToString();
                SiteGoal.value = int.Parse(row["value"].ToString());
                SiteGoal.GoalNumber = int.Parse(row["GoalNumber"].ToString());
                SiteGoal.GoalDate = (row["GoalDate"].ToString()).Length == 0 ? SiteGoal.GoalDate : DateTime.Parse(row["GoalDate"].ToString());
                SiteGoal.SyncDate = DateTime.Now;
                SetUpdatedDataFields(SiteGoal, row);
                SaveOrUpdateSiteGoal(SiteGoal);
                SiteGoalCount++;
                if (SiteGoalCount % 5 == 0)
                { _logger.Information(SiteGoalCount + " de " + dt1.Rows.Count + " Metas da OCB importadas."); }
            }
            UnitOfWork.Commit();

            Rename(fullPath, fullPath + IMPORTED);
            return SiteGoalCount;
        }

        public int ImportSites(string path)
        {
            UnitOfWork.DbContext.Configuration.AutoDetectChangesEnabled = false;

            FileImporter imp = new FileImporter();
            string fullPath = path + @"\Sites.csv";
            DataTable dt1 = imp.ImportFromCSV(fullPath);

            if (dt1.Rows.Count == 0) return 0;

            // Site //

            int SiteCount = 0;
            List<Site> SitesList = new List<Site>();
            foreach (DataRow row in dt1.Rows)
            {
                Guid SiteGuid = new Guid(row["Site_guid"].ToString());
                Site site = findSiteBySyncGuid(SiteGuid);

                if (site == null)
                {
                    site = new Site();
                    site.SyncGuid = SiteGuid;
                }

                site.SiteName = row["SiteName"].ToString();
                site.SiteType = row["SiteType"].ToString();
                site.orgUnitID = int.Parse(row["orgUnitID"].ToString());
                site.LastReplicatedDate = (row["LastReplicatedDate"].ToString()).Length == 0 ? site.LastReplicatedDate : DateTime.Parse(row["LastReplicatedDate"].ToString());
                site.SyncDate = DateTime.Now;
                SaveOrUpdateSite(site);
                SiteCount++;
                { _logger.Information(SiteCount + " de " + dt1.Rows.Count + " OCBs importadas."); }

            }
            UnitOfWork.Commit();
            Rename(fullPath, fullPath + IMPORTED);

            return SiteCount;
        }

        public int ImportData(string path)
        {
            _logger.Information("IMPORTACAO DE OCBS ...");
            int ImportedSites = ImportSites(path);
            int ImportedSiteGoals = ImportSiteGoals(path);
            return ImportedSites;
        }

        public List<int> getMonthlyProgressReport(DateTime initialDate, DateTime lastDate, int ProvID, int DistID, int SiteID)
        {
            // CONVERT(DATETIME,'05/05/2017')

            List<int> numberList = new List<int>();
            ApplicationDbContext dbContext = new ApplicationDbContext();


            String countHIVStateForAdultsAndChildren = @"SELECT ISNULL(Sum(HIVPositivCount.HIVP), 0) FROM 
                                                            (
								                            SELECT H.ChildID, Count(*) As HIVP
								                            FROM  [Partner] p
					                                        inner join  [Site] s on (p.siteID = s.SiteID)
					                                        inner join  [OrgUnit] dist on (dist.OrgUnitID = s.OrgUnitID)
					                                        inner join  [OrgUnit] prov on (dist.ParentOrgUnitId = prov.OrgUnitID)
					                                        inner join  [HouseHold] hh on (p.PartnerID = hh.PartnerID)
								                            inner join  [Child] c on (c.HouseholdID = hh.HouseHoldID)
								                            inner join  [HIVStatus] H on (c.ChildID = H.ChildID)
								                            WHERE H.ChildID != 0 and HIVState
								                            AND H.InformationDate >= @initialDate AND H.InformationDate <= @initialDate
								                            AND ((s.SiteID = @SiteID AND @ProvID = 0 AND @DistID = 0) OR (@SiteID = 0 AND dist.OrgUnitID = @DistID AND @ProvID = 0) OR 
					                                        (@SiteID = 0 AND @DistID = 0 AND prov.OrgUnitID = @ProvID) OR (@SiteID = 0 AND @DistID = 0 AND @ProvID = 0)) 
								                            GROUP BY H.ChildID
								                            union 
								                            SELECT H.AdultID, Count(*) AS HIVP 
								                            FROM  [Partner] p
					                                        inner join  [Site] s on (p.siteID = s.SiteID)
					                                        inner join  [OrgUnit] dist on (dist.OrgUnitID = s.OrgUnitID)
					                                        inner join  [OrgUnit] prov on (dist.ParentOrgUnitId = prov.OrgUnitID)
					                                        inner join  [HouseHold] hh on (p.PartnerID = hh.PartnerID)
								                            inner join  [Adult] a on (a.HouseholdID = hh.HouseHoldID)
								                            inner join  [HIVStatus] H on (a.AdultId = H.AdultID)
								                            WHERE H.AdultID != 0 and HIVState
								                            AND H.InformationDate >= @initialDate AND H.InformationDate <= @lastDate
								                            AND ((s.SiteID = @SiteID AND @ProvID = 0 AND @DistID = 0) OR (@SiteID = 0 AND dist.OrgUnitID = @DistID AND @ProvID = 0) OR 
					                                        (@SiteID = 0 AND @DistID = 0 AND prov.OrgUnitID = @ProvID) OR (@SiteID = 0 AND @DistID = 0 AND @ProvID = 0)) 
								                            GROUP BY H.AdultID
								                            ) AS HIVPositivCount";
            numberList.Add(dbContext.Database.SqlQuery<int>(countHIVStateForAdultsAndChildren.Replace("HIVState", "H.HIV = 'P'"),
                                                            new SqlParameter("initialDate", initialDate),
                                                            new SqlParameter("lastDate", lastDate),
                                                            new SqlParameter("ProvID", ProvID),
                                                            new SqlParameter("DistID", DistID),
                                                            new SqlParameter("SiteID", SiteID)).FirstOrDefault());

            // Benificiarios Servidos
            numberList.Add(dbContext.Database.SqlQuery<int>(@"SELECT COUNT(*) FROM 
                                                            ( 
	                                                            SELECT
		                                                            ben.[Guid]
		                                                            FROM [Beneficiary] ben
		                                                            inner join  [ChildStatusHistory] csh on (csh.ChildStatusHistoryID = 
		                                                            (
			                                                            SELECT max(csh2.ChildStatusHistoryID) FROM  [ChildStatusHistory] csh2 
			                                                            WHERE 
			                                                            ((csh2.ChildID = ben.ID) AND (ben.Type = 'child')) 
			                                                            OR 
			                                                            ((csh2.AdultID = ben.ID) AND (ben.Type = 'adult'))	
		                                                            ))
		                                                            AND (csh.EffectiveDate < @lastDate) 
		                                                            inner join  [ChildStatus] ct on (csh.childStatusID = ct.StatusID and ct.Description = 'Inicial')
		                                                            AND ben.[Guid] in 
		                                                            (
			                                                            SELECT d.[Guid] FROM
			                                                            (
				                                                            SELECT
				                                                            routv_all.[Guid] As [Guid],
				                                                            ISNULL(SUM(routv_all.FinaceAid),0) As AllFinanceAid, 
				                                                            ISNULL(SUM(routv_dt.FinaceAid),0) As DtFinanceAid,
				                                                            ISNULL(SUM(routv_all.Health),0) As AllHealth, 
				                                                            ISNULL(SUM(routv_dt.Health),0) As DtHealth,
				                                                            ISNULL(SUM(routv_all.Food),0) As AllFood,
				                                                            ISNULL(SUM(routv_dt.Food),0) As DtFood,
				                                                            ISNULL(SUM(routv_all.Education),0) As AllEducation,
				                                                            ISNULL(SUM(routv_dt.Education),0) As DtEducation,
				                                                            ISNULL(SUM(routv_all.LegalAdvice),0) As AllLegalAdvice,
				                                                            ISNULL(SUM(routv_dt.LegalAdvice),0) As DtLegalAdvice,
				                                                            ISNULL(SUM(routv_all.Housing),0) As AllHousing,
				                                                            ISNULL(SUM(routv_dt.Housing),0) As DtHousing,
				                                                            ISNULL(SUM(routv_all.SocialAid),0) As AllSocialAid,
				                                                            ISNULL(SUM(routv_dt.SocialAid),0) As DtSocialAid,
				                                                            AllDPI = CASE WHEN DATEDIFF(YEAR, CAST(routv_dt.DateOfBirth As Date), @lastDate) < 5 THEN SUM(routv_all.DPI) ELSE 0 END,
				                                                            DtDPI = CASE WHEN DATEDIFF(YEAR, CAST(routv_dt.DateOfBirth As Date), @lastDate) < 5 THEN SUM(routv_dt.DPI) ELSE 0 END,
				                                                            MUACGreen = CASE WHEN DATEDIFF(month, CAST(routv_dt.DateOfBirth As Date), @lastDate) BETWEEN 6 AND 59 THEN SUM(routv_dt.MUACGreen) ELSE 0 END,
				                                                            MUACYellow = CASE WHEN DATEDIFF(month, CAST(routv_dt.DateOfBirth As Date), @lastDate) BETWEEN 6 AND 59 THEN SUM(routv_dt.MUACYellow) ELSE 0 END,
				                                                            MUACRed = CASE WHEN DATEDIFF(month, CAST(routv_dt.DateOfBirth As Date), @lastDate) BETWEEN 6 AND 59 THEN SUM(routv_dt.MUACRed) ELSE 0 END
				                                                            FROM
				                                                            (
					                                                            SELECT c.*  from  [Children_Routine_Visit_Summary] c
					                                                            --inner join  [ChildStatusHistory] csh on (csh.ChildStatusHistoryID = 
					                                                            --(SELECT max(csh2.ChildStatusHistoryID) FROM  [ChildStatusHistory] csh2 WHERE (csh2.EffectiveDate < @lastDate) AND (csh2.ChildID = c.ChildID)))
					                                                            --inner join  [ChildStatus] ct on (csh.childStatusID = ct.StatusID and ct.Description = 'Inicial')
					                                                            WHERE c.RoutineVisitDate BETWEEN @initialDate AND @lastDate

					                                                            union all

					                                                            SELECT a.* from  [Adults_Routine_Visit_Summary] a
					                                                            --inner join  [ChildStatusHistory] csh on (csh.ChildStatusHistoryID = 
					                                                            --(SELECT max(csh2.ChildStatusHistoryID) FROM  [ChildStatusHistory] csh2 WHERE (csh2.EffectiveDate < @lastDate) AND (csh2.AdultID = a.AdultID)))
					                                                            --inner join  [ChildStatus] ct on (csh.childStatusID = ct.StatusID and ct.Description = 'Inicial')
					                                                            WHERE a.RoutineVisitDate BETWEEN @initialDate AND @lastDate
				                                                            ) 
				                                                            routv_all left join
				                                                            (
					                                                            SELECT rvs.* FROM  [Routine_Visit_Summary] rvs
					                                                            WHERE rvs.RoutineVisitDate BETWEEN @initialDate AND @lastDate
				                                                            ) routv_dt on (routv_all.RoutineVisitSupportID = routv_dt.RoutineVisitSupportID)
				                                                            group by routv_all.[Guid], routv_dt.[Guid], routv_dt.DateOfBirth
			                                                            ) d
			                                                            WHERE
			                                                            ((d.AllFinanceAid + d.DtFinanceAid + d.AllHealth  + d.DtHealth + d.AllFood  + d.DtFood  +
			                                                            d.AllEducation + d.DtEducation + d.AllLegalAdvice + d.DtLegalAdvice + d.AllHousing + d.DtHousing  +
			                                                            d.AllSocialAid + d.DtSocialAid  + d.AllDPI + d.DtDPI + d.MUACGreen + d.MUACYellow + d.MUACRed) > 0)
			                                                            OR d.[Guid] in 
			                                                            (
				                                                            SELECT b.Guid from  [Beneficiary] b
				                                                            inner join  [HIVStatus] hs on (hs.HIVStatusID != b.HIVStatusID and hs.HIVStatusID =
				                                                            (SELECT max(hs2.HIVStatusID) FROM  [HIVStatus] hs2 WHERE hs2.[InformationDate] BETWEEN @initialDate AND @lastDate AND (hs2.BeneficiaryGuid = b.Guid)))  
			                                                            )	
	                                                            )
                                                            ) q", 
                                        new SqlParameter("initialDate", initialDate),
                                        new SqlParameter("lastDate", lastDate)).FirstOrDefault());

            // Benificiarios Graduados
            String countGraduatedBenificiaries = @"SELECT COUNT(*) FROM 
                                                ( 
	                                                SELECT
		                                                ben.[Guid]
		                                                FROM [Beneficiary] ben
		                                                inner join  [ChildStatusHistory] csh on (csh.ChildStatusHistoryID = 
		                                                (
			                                                SELECT max(csh2.ChildStatusHistoryID) FROM  [ChildStatusHistory] csh2 
			                                                WHERE 
			                                                ((csh2.ChildID = ben.ID) AND (ben.Type = 'child')) 
			                                                OR 
			                                                ((csh2.AdultID = ben.ID) AND (ben.Type = 'adult'))	
		                                                ))
		                                                AND (csh.EffectiveDate < @lastDate) 
		                                                inner join  [ChildStatus] ct on (csh.childStatusID = ct.StatusID and ct.Description = 'Graduação')
		                                                AND ben.[Guid] in 
		                                                (
			                                                SELECT d.[Guid] FROM
			                                                (
				                                                SELECT
				                                                routv_all.[Guid] As [Guid],
				                                                ISNULL(SUM(routv_all.FinaceAid),0) As AllFinanceAid, 
				                                                ISNULL(SUM(routv_dt.FinaceAid),0) As DtFinanceAid,
				                                                ISNULL(SUM(routv_all.Health),0) As AllHealth, 
				                                                ISNULL(SUM(routv_dt.Health),0) As DtHealth,
				                                                ISNULL(SUM(routv_all.Food),0) As AllFood,
				                                                ISNULL(SUM(routv_dt.Food),0) As DtFood,
				                                                ISNULL(SUM(routv_all.Education),0) As AllEducation,
				                                                ISNULL(SUM(routv_dt.Education),0) As DtEducation,
				                                                ISNULL(SUM(routv_all.LegalAdvice),0) As AllLegalAdvice,
				                                                ISNULL(SUM(routv_dt.LegalAdvice),0) As DtLegalAdvice,
				                                                ISNULL(SUM(routv_all.Housing),0) As AllHousing,
				                                                ISNULL(SUM(routv_dt.Housing),0) As DtHousing,
				                                                ISNULL(SUM(routv_all.SocialAid),0) As AllSocialAid,
				                                                ISNULL(SUM(routv_dt.SocialAid),0) As DtSocialAid,
				                                                AllDPI = CASE WHEN DATEDIFF(YEAR, CAST(routv_dt.DateOfBirth As Date), @lastDate) < 5 THEN SUM(routv_all.DPI) ELSE 0 END,
				                                                DtDPI = CASE WHEN DATEDIFF(YEAR, CAST(routv_dt.DateOfBirth As Date), @lastDate) < 5 THEN SUM(routv_dt.DPI) ELSE 0 END,
				                                                MUACGreen = CASE WHEN DATEDIFF(month, CAST(routv_dt.DateOfBirth As Date), @lastDate) BETWEEN 6 AND 59 THEN SUM(routv_dt.MUACGreen) ELSE 0 END,
				                                                MUACYellow = CASE WHEN DATEDIFF(month, CAST(routv_dt.DateOfBirth As Date), @lastDate) BETWEEN 6 AND 59 THEN SUM(routv_dt.MUACYellow) ELSE 0 END,
				                                                MUACRed = CASE WHEN DATEDIFF(month, CAST(routv_dt.DateOfBirth As Date), @lastDate) BETWEEN 6 AND 59 THEN SUM(routv_dt.MUACRed) ELSE 0 END
				                                                FROM
				                                                (
					                                                SELECT c.*  from  [Children_Routine_Visit_Summary] c
					                                                --inner join  [ChildStatusHistory] csh on (csh.ChildStatusHistoryID = 
					                                                --(SELECT max(csh2.ChildStatusHistoryID) FROM  [ChildStatusHistory] csh2 WHERE (csh2.EffectiveDate < @lastDate) AND (csh2.ChildID = c.ChildID)))
					                                                --inner join  [ChildStatus] ct on (csh.childStatusID = ct.StatusID and ct.Description = 'Inicial')
					                                                WHERE c.RoutineVisitDate BETWEEN @initialDate AND @lastDate

					                                                union all

					                                                SELECT a.* from  [Adults_Routine_Visit_Summary] a
					                                                --inner join  [ChildStatusHistory] csh on (csh.ChildStatusHistoryID = 
					                                                --(SELECT max(csh2.ChildStatusHistoryID) FROM  [ChildStatusHistory] csh2 WHERE (csh2.EffectiveDate < @lastDate) AND (csh2.AdultID = a.AdultID)))
					                                                --inner join  [ChildStatus] ct on (csh.childStatusID = ct.StatusID and ct.Description = 'Inicial')
					                                                WHERE a.RoutineVisitDate BETWEEN @initialDate AND @lastDate
				                                                ) 
				                                                routv_all left join
				                                                (
					                                                SELECT rvs.* FROM  [Routine_Visit_Summary] rvs
					                                                WHERE rvs.RoutineVisitDate BETWEEN @initialDate AND @lastDate
				                                                ) routv_dt on (routv_all.RoutineVisitSupportID = routv_dt.RoutineVisitSupportID)
				                                                group by routv_all.[Guid], routv_dt.[Guid], routv_dt.DateOfBirth
			                                                ) d
			                                                WHERE
			                                                ((d.AllFinanceAid + d.DtFinanceAid + d.AllHealth  + d.DtHealth + d.AllFood  + d.DtFood  +
			                                                d.AllEducation + d.DtEducation + d.AllLegalAdvice + d.DtLegalAdvice + d.AllHousing + d.DtHousing  +
			                                                d.AllSocialAid + d.DtSocialAid  + d.AllDPI + d.DtDPI + d.MUACGreen + d.MUACYellow + d.MUACRed) > 0)
			                                                OR d.[Guid] in 
			                                                (
				                                                SELECT b.Guid from  [Beneficiary] b
				                                                inner join  [HIVStatus] hs on (hs.HIVStatusID != b.HIVStatusID and hs.HIVStatusID =
				                                                (SELECT max(hs2.HIVStatusID) FROM  [HIVStatus] hs2 WHERE hs2.[InformationDate] BETWEEN @initialDate AND @lastDate AND (hs2.BeneficiaryGuid = b.Guid)))  
			                                                )	
	                                                )
                                                ) q";
            numberList.Add(dbContext.Database.SqlQuery<int>(countGraduatedBenificiaries,
                                                            new SqlParameter("initialDate", initialDate),
                                                            new SqlParameter("lastDate", lastDate),
                                                            new SqlParameter("ProvID", ProvID),
                                                            new SqlParameter("DistID", DistID),
                                                            new SqlParameter("SiteID", SiteID)).FirstOrDefault());

            // Benificiarios Inactivos
            numberList.Add(dbContext.Database.SqlQuery<int>(@"SELECT COUNT(*) - 
                                                            (
	                                                            SELECT COUNT(*) FROM 
	                                                            ( 
		                                                            SELECT
			                                                            ben.[Guid]
			                                                            FROM [CSI_PROD].[dbo].[Beneficiary] ben
			                                                            inner join  [ChildStatusHistory] csh on (csh.ChildStatusHistoryID = 
		                                                                (
			                                                                SELECT max(csh2.ChildStatusHistoryID) FROM  [ChildStatusHistory] csh2 
			                                                                WHERE 
			                                                                ((csh2.ChildID = ben.ID) AND (ben.Type = 'child')) 
			                                                                OR 
			                                                                ((csh2.AdultID = ben.ID) AND (ben.Type = 'adult'))	
		                                                                ))
		                                                                AND (csh.EffectiveDate < @lastDate) 
			                                                            inner join  [ChildStatus] ct on (csh.childStatusID = ct.StatusID AND ct.Description not in ('Adulto'))
			                                                            AND ben.[Guid] in 
			                                                            (
				                                                            SELECT d.[Guid] FROM
				                                                            (
					                                                            SELECT
					                                                            routv_all.[Guid] As [Guid],
					                                                            ISNULL(SUM(routv_all.FinaceAid),0) As AllFinanceAid, 
					                                                            ISNULL(SUM(routv_dt.FinaceAid),0) As DtFinanceAid,
					                                                            ISNULL(SUM(routv_all.Health),0) As AllHealth, 
					                                                            ISNULL(SUM(routv_dt.Health),0) As DtHealth,
					                                                            ISNULL(SUM(routv_all.Food),0) As AllFood,
					                                                            ISNULL(SUM(routv_dt.Food),0) As DtFood,
					                                                            ISNULL(SUM(routv_all.Education),0) As AllEducation,
					                                                            ISNULL(SUM(routv_dt.Education),0) As DtEducation,
					                                                            ISNULL(SUM(routv_all.LegalAdvice),0) As AllLegalAdvice,
					                                                            ISNULL(SUM(routv_dt.LegalAdvice),0) As DtLegalAdvice,
					                                                            ISNULL(SUM(routv_all.Housing),0) As AllHousing,
					                                                            ISNULL(SUM(routv_dt.Housing),0) As DtHousing,
					                                                            ISNULL(SUM(routv_all.SocialAid),0) As AllSocialAid,
					                                                            ISNULL(SUM(routv_dt.SocialAid),0) As DtSocialAid,
					                                                            AllDPI = CASE WHEN DATEDIFF(YEAR, CAST(routv_dt.DateOfBirth As Date), @lastDate) < 5 THEN SUM(routv_all.DPI) ELSE 0 END,
					                                                            DtDPI = CASE WHEN DATEDIFF(YEAR, CAST(routv_dt.DateOfBirth As Date), @lastDate) < 5 THEN SUM(routv_dt.DPI) ELSE 0 END,
					                                                            MUACGreen = CASE WHEN DATEDIFF(month, CAST(routv_dt.DateOfBirth As Date), @lastDate) BETWEEN 6 AND 59 THEN SUM(routv_dt.MUACGreen) ELSE 0 END,
					                                                            MUACYellow = CASE WHEN DATEDIFF(month, CAST(routv_dt.DateOfBirth As Date), @lastDate) BETWEEN 6 AND 59 THEN SUM(routv_dt.MUACYellow) ELSE 0 END,
					                                                            MUACRed = CASE WHEN DATEDIFF(month, CAST(routv_dt.DateOfBirth As Date), @lastDate) BETWEEN 6 AND 59 THEN SUM(routv_dt.MUACRed) ELSE 0 END
					                                                            FROM
					                                                            (
						                                                            SELECT c.*  from  [Children_Routine_Visit_Summary] c
						                                                            WHERE c.RoutineVisitDate BETWEEN @initialDate AND @lastDate

						                                                            union all

						                                                            SELECT a.* from  [Adults_Routine_Visit_Summary] a
						                                                            WHERE a.RoutineVisitDate BETWEEN @initialDate AND @lastDate
					                                                            ) 
					                                                            routv_all left join
					                                                            (
						                                                            SELECT rvs.* FROM  [Routine_Visit_Summary] rvs
						                                                            WHERE rvs.RoutineVisitDate BETWEEN @initialDate AND @lastDate
					                                                            ) routv_dt on (routv_all.RoutineVisitSupportID = routv_dt.RoutineVisitSupportID)
					                                                            group by routv_all.[Guid], routv_dt.[Guid], routv_dt.DateOfBirth
				                                                            ) d
				                                                            WHERE
				                                                            ((d.AllFinanceAid + d.DtFinanceAid + d.AllHealth  + d.DtHealth + d.AllFood  + d.DtFood  +
				                                                            d.AllEducation + d.DtEducation + d.AllLegalAdvice + d.DtLegalAdvice + d.AllHousing + d.DtHousing  +
				                                                            d.AllSocialAid + d.DtSocialAid  + d.AllDPI + d.DtDPI + d.MUACGreen + d.MUACYellow + d.MUACRed) > 0)
				                                                            OR d.[Guid] in 
				                                                            (
					                                                            SELECT b.Guid from  [Beneficiary] b
					                                                            inner join  [HIVStatus] hs on (hs.HIVStatusID != b.HIVStatusID and hs.HIVStatusID =
					                                                            (SELECT max(hs2.HIVStatusID) FROM  [HIVStatus] hs2 WHERE hs2.[InformationDate] BETWEEN @initialDate AND @lastDate AND (hs2.BeneficiaryGuid = b.Guid)))  
				                                                            )	
		                                                            )
	                                                            ) q
                                                            )
                                                            FROM [Beneficiary] ben
                                                            inner join [ChildStatusHistory] csh on (csh.ChildStatusHistoryID = 
                                                            (
	                                                            SELECT max(csh2.ChildStatusHistoryID) FROM  [ChildStatusHistory] csh2 
	                                                            WHERE 
	                                                            ((csh2.ChildID = ben.ID) AND (ben.Type = 'child')) 
	                                                            OR 
	                                                            ((csh2.AdultID = ben.ID) AND (ben.Type = 'adult'))	
                                                            ))
                                                            AND (csh.EffectiveDate < @lastDate) 
                                                            inner join  [ChildStatus] ct on (csh.childStatusID = ct.StatusID AND ct.Description not in ('Adulto'))",
                                        new SqlParameter("initialDate", initialDate),
                                        new SqlParameter("lastDate", lastDate)).FirstOrDefault());


            String countReferencesToHealthAndSocialServices = @"SELECT SUM(Count) FROM
                                                                (
	                                                                SELECT count(*) as Count
	                                                                FROM  [Partner] p
	                                                                inner join  [Partner] cp on (p.SuperiorId = cp.PartnerID) 
	                                                                inner join  [HouseHold] hh on (p.PartnerID = hh.PartnerID)
	                                                                inner join  [Child] c on (c.HouseHoldID = hh.HouseHoldID)
	                                                                inner join  [ReferenceService] rs on (c.ChildID = rs.ChildID)
	                                                                inner join  [Reference] r on (r.ReferenceServiceID = rs.ReferenceServiceID)
	                                                                inner join  [ReferenceType] rt on (rt.ReferenceTypeID = r.ReferenceTypeID)
                                                                    WHERE c.ChildID IS NOT NULL
                                                                    AND rs.ReferenceDate between @initialDate AND @lastDate
	                                                                AND rt.ReferenceCategory = 'Activist' AND r.Value <> '0' AND r.Value <> '' AND r.Value IS NOT NULL
	                                                                AND NOT LOWER(r.Value) LIKE LOWER('%test%')--Testagem, teste, ATS
	                                                                AND NOT LOWER(r.Value) LIKE LOWER('%ats%')--ATS
	                                                                AND NOT LOWER(r.Value) LIKE LOWER('%its%') --ITS
	                                                                AND NOT LOWER(r.Value) LIKE LOWER('%tarv%')--TARV
	                                                                AND NOT LOWER(r.Value) LIKE LOWER('%profila%')--PPE
	                                                                AND NOT LOWER(r.Value) LIKE LOWER('%ppe%')--PPE
	                                                                AND NOT LOWER(r.Value) LIKE LOWER('%natal%')--Testagem, teste
	                                                                AND NOT LOWER(r.Value) LIKE LOWER('%ccr%') --CCR
	                                                                AND NOT LOWER(r.Value) LIKE LOWER('%risco%') --CCR
	                                                                AND NOT LOWER(r.Value) LIKE LOWER('%viol%')--GAAV
	                                                                AND NOT LOWER(r.Value) LIKE LOWER('%gaav%')--GAAV
	                                                                AND NOT LOWER(r.Value) LIKE LOWER('%parto%')--CPN Maternidade p/ Parto,Consulta Pós-Parto
	                                                                AND NOT LOWER(r.Value) LIKE LOWER('%cpn%')--CPN
	                                                                AND NOT LOWER(r.Value) LIKE LOWER('%planeamento familiar%')--CPF
	                                                                AND NOT LOWER(r.Value) LIKE LOWER('%oportuni%')--IO
	                                                                AND NOT LOWER(r.Value) LIKE LOWER('%domic%')--CD
	                                                                AND NOT LOWER(r.Value) LIKE LOWER('%sadia%')--CCS
	                                                                AND NOT LOWER(r.Value) LIKE LOWER('%ptv%')--PTV
	                                                                AND NOT LOWER(r.Value) LIKE LOWER('%vertical%')--PTV
	                                                                AND NOT LOWER(r.Value) LIKE LOWER('%bk%')--BK
	                                                                AND NOT LOWER(r.Value) LIKE LOWER('%mal%')--malária
	                                                                AND NOT LOWER(r.Value) LIKE LOWER('%circun%')--Circuncisão
	                                                                AND NOT LOWER(r.Value) LIKE LOWER('%pscico%')--Apoio Psico-Social
	                                                                AND NOT LOWER(r.Value) LIKE LOWER('%social%')--Acção Social,Apoio Psico-Social
	                                                                AND NOT LOWER(r.Value) LIKE LOWER('%polic%')--policial

	                                                                union all

	                                                                SELECT count(*) as Count
	                                                                FROM  [Partner] p
	                                                                inner join  [Partner] cp on (p.SuperiorId = cp.PartnerID) 
	                                                                inner join  [HouseHold] hh on (p.PartnerID = hh.PartnerID)
	                                                                inner join  [Adult] a on (a.HouseHoldID = hh.HouseHoldID)
	                                                                inner join  [ReferenceService] rs on (a.AdultID = rs.AdultID)
	                                                                inner join  [Reference] r on (r.ReferenceServiceID = rs.ReferenceServiceID)
	                                                                inner join  [ReferenceType] rt on (rt.ReferenceTypeID = r.ReferenceTypeID)
                                                                    WHERE a.AdultID IS NOT NULL
                                                                    AND rs.ReferenceDate between @initialDate AND @lastDate
	                                                                AND rt.ReferenceCategory = 'Activist' AND r.Value <> '0' AND r.Value <> '' AND r.Value IS NOT NULL
	                                                                AND NOT LOWER(r.Value) LIKE LOWER('%test%')--Testagem, teste, ATS
	                                                                AND NOT LOWER(r.Value) LIKE LOWER('%ats%')--ATS
	                                                                AND NOT LOWER(r.Value) LIKE LOWER('%its%') --ITS
	                                                                AND NOT LOWER(r.Value) LIKE LOWER('%tarv%')--TARV
	                                                                AND NOT LOWER(r.Value) LIKE LOWER('%profila%')--PPE
	                                                                AND NOT LOWER(r.Value) LIKE LOWER('%ppe%')--PPE
	                                                                AND NOT LOWER(r.Value) LIKE LOWER('%natal%')--Testagem, teste
	                                                                AND NOT LOWER(r.Value) LIKE LOWER('%ccr%') --CCR
	                                                                AND NOT LOWER(r.Value) LIKE LOWER('%risco%') --CCR
	                                                                AND NOT LOWER(r.Value) LIKE LOWER('%viol%')--GAAV
	                                                                AND NOT LOWER(r.Value) LIKE LOWER('%gaav%')--GAAV
	                                                                AND NOT LOWER(r.Value) LIKE LOWER('%parto%')--CPN Maternidade p/ Parto,Consulta Pós-Parto
	                                                                AND NOT LOWER(r.Value) LIKE LOWER('%cpn%')--CPN
	                                                                AND NOT LOWER(r.Value) LIKE LOWER('%planeamento familiar%')--CPF
	                                                                AND NOT LOWER(r.Value) LIKE LOWER('%oportuni%')--IO
	                                                                AND NOT LOWER(r.Value) LIKE LOWER('%domic%')--CD
	                                                                AND NOT LOWER(r.Value) LIKE LOWER('%sadia%')--CCS
	                                                                AND NOT LOWER(r.Value) LIKE LOWER('%ptv%')--PTV
	                                                                AND NOT LOWER(r.Value) LIKE LOWER('%vertical%')--PTV
	                                                                AND NOT LOWER(r.Value) LIKE LOWER('%bk%')--BK
	                                                                AND NOT LOWER(r.Value) LIKE LOWER('%mal%')--malária
	                                                                AND NOT LOWER(r.Value) LIKE LOWER('%circun%')--Circuncisão
	                                                                AND NOT LOWER(r.Value) LIKE LOWER('%pscico%')--Apoio Psico-Social
	                                                                AND NOT LOWER(r.Value) LIKE LOWER('%social%')--Acção Social,Apoio Psico-Social
	                                                                AND NOT LOWER(r.Value) LIKE LOWER('%polic%')--policial
                                                                ) total";

            numberList.Add(dbContext.Database.SqlQuery<int>(countReferencesToHealthAndSocialServices,
                                                            new SqlParameter("initialDate", initialDate),
                                                            new SqlParameter("lastDate", lastDate),
                                                            new SqlParameter("ProvID", ProvID),
                                                            new SqlParameter("DistID", DistID),
                                                            new SqlParameter("SiteID", SiteID)).FirstOrDefault());


            // TODO : Rever Isto algo não bate certo aqui, o total não é igual a soma das partes
            String countCompleteReferencesToHealthAndSocialServices = @"SELECT SUM(Count) FROM
                                                                        (
	                                                                        SELECT count(*) as Count
	                                                                        FROM  [Partner] p
	                                                                        inner join  [Partner] cp on (p.SuperiorId = cp.PartnerID) 
	                                                                        inner join  [HouseHold] hh on (p.PartnerID = hh.PartnerID)
	                                                                        inner join  [Child] c on (c.HouseHoldID = hh.HouseHoldID)
	                                                                        inner join  [ReferenceService] rs on (c.ChildID = rs.ChildID)
	                                                                        inner join  [Reference] r on (r.ReferenceServiceID = rs.ReferenceServiceID)
	                                                                        inner join  [ReferenceType] rt on (rt.ReferenceTypeID = r.ReferenceTypeID)
                                                                            WHERE c.ChildID IS NOT NULL
                                                                            AND ((rs.HealthAttendedDate between @initialDate AND @lastDate)
                                                                            OR (rs.SocialAttendedDate between @initialDate AND @lastDate))
                                                                            AND rt.ReferenceCategory in ('Health','Social') AND r.Value <> '0' AND r.Value <> '' AND r.Value IS NOT NULL
	                                                                        AND NOT LOWER(r.Value) LIKE LOWER('%test%')--Testagem, teste, ATS
	                                                                        AND NOT LOWER(r.Value) LIKE LOWER('%ats%')--ATS
	                                                                        AND NOT LOWER(r.Value) LIKE LOWER('%its%') --ITS
	                                                                        AND NOT LOWER(r.Value) LIKE LOWER('%tarv%')--TARV
	                                                                        AND NOT LOWER(r.Value) LIKE LOWER('%profila%')--PPE
	                                                                        AND NOT LOWER(r.Value) LIKE LOWER('%ppe%')--PPE
	                                                                        AND NOT LOWER(r.Value) LIKE LOWER('%natal%')--Testagem, teste
	                                                                        AND NOT LOWER(r.Value) LIKE LOWER('%ccr%') --CCR
	                                                                        AND NOT LOWER(r.Value) LIKE LOWER('%risco%') --CCR
	                                                                        AND NOT LOWER(r.Value) LIKE LOWER('%viol%')--GAAV
	                                                                        AND NOT LOWER(r.Value) LIKE LOWER('%gaav%')--GAAV
	                                                                        AND NOT LOWER(r.Value) LIKE LOWER('%parto%')--CPN Maternidade p/ Parto,Consulta Pós-Parto
	                                                                        AND NOT LOWER(r.Value) LIKE LOWER('%cpn%')--CPN
	                                                                        AND NOT LOWER(r.Value) LIKE LOWER('%planeamento familiar%')--CPF
	                                                                        AND NOT LOWER(r.Value) LIKE LOWER('%oportuni%')--IO
	                                                                        AND NOT LOWER(r.Value) LIKE LOWER('%domic%')--CD
	                                                                        AND NOT LOWER(r.Value) LIKE LOWER('%sadia%')--CCS
	                                                                        AND NOT LOWER(r.Value) LIKE LOWER('%ptv%')--PTV
	                                                                        AND NOT LOWER(r.Value) LIKE LOWER('%vertical%')--PTV
	                                                                        AND NOT LOWER(r.Value) LIKE LOWER('%bk%')--BK
	                                                                        AND NOT LOWER(r.Value) LIKE LOWER('%mal%')--malária
	                                                                        AND NOT LOWER(r.Value) LIKE LOWER('%circun%')--Circuncisão
	                                                                        AND NOT LOWER(r.Value) LIKE LOWER('%pscico%')--Apoio Psico-Social
	                                                                        AND NOT LOWER(r.Value) LIKE LOWER('%social%')--Acção Social,Apoio Psico-Social
	                                                                        AND NOT LOWER(r.Value) LIKE LOWER('%polic%')--policial

	                                                                        union all

	                                                                        SELECT count(*) as Count
	                                                                        FROM  [Partner] p
	                                                                        inner join  [Partner] cp on (p.SuperiorId = cp.PartnerID) 
	                                                                        inner join  [HouseHold] hh on (p.PartnerID = hh.PartnerID)
	                                                                        inner join  [Adult] a on (a.HouseHoldID = hh.HouseHoldID)
	                                                                        inner join  [ReferenceService] rs on (a.AdultID = rs.AdultID)
	                                                                        inner join  [Reference] r on (r.ReferenceServiceID = rs.ReferenceServiceID)
	                                                                        inner join  [ReferenceType] rt on (rt.ReferenceTypeID = r.ReferenceTypeID)
                                                                            WHERE a.AdultID IS NOT NULL
                                                                            AND ((rs.HealthAttendedDate between @initialDate AND @lastDate)
                                                                            OR (rs.SocialAttendedDate between @initialDate AND @lastDate))
                                                                            AND rt.ReferenceCategory in ('Health','Social') AND r.Value <> '0' AND r.Value <> '' AND r.Value IS NOT NULL
	                                                                        AND NOT LOWER(r.Value) LIKE LOWER('%test%')--Testagem, teste, ATS
	                                                                        AND NOT LOWER(r.Value) LIKE LOWER('%ats%')--ATS
	                                                                        AND NOT LOWER(r.Value) LIKE LOWER('%its%') --ITS
	                                                                        AND NOT LOWER(r.Value) LIKE LOWER('%tarv%')--TARV
	                                                                        AND NOT LOWER(r.Value) LIKE LOWER('%profila%')--PPE
	                                                                        AND NOT LOWER(r.Value) LIKE LOWER('%ppe%')--PPE
	                                                                        AND NOT LOWER(r.Value) LIKE LOWER('%natal%')--Testagem, teste
	                                                                        AND NOT LOWER(r.Value) LIKE LOWER('%ccr%') --CCR
	                                                                        AND NOT LOWER(r.Value) LIKE LOWER('%risco%') --CCR
	                                                                        AND NOT LOWER(r.Value) LIKE LOWER('%viol%')--GAAV
	                                                                        AND NOT LOWER(r.Value) LIKE LOWER('%gaav%')--GAAV
	                                                                        AND NOT LOWER(r.Value) LIKE LOWER('%parto%')--CPN Maternidade p/ Parto,Consulta Pós-Parto
	                                                                        AND NOT LOWER(r.Value) LIKE LOWER('%cpn%')--CPN
	                                                                        AND NOT LOWER(r.Value) LIKE LOWER('%planeamento familiar%')--CPF
	                                                                        AND NOT LOWER(r.Value) LIKE LOWER('%oportuni%')--IO
	                                                                        AND NOT LOWER(r.Value) LIKE LOWER('%domic%')--CD
	                                                                        AND NOT LOWER(r.Value) LIKE LOWER('%sadia%')--CCS
	                                                                        AND NOT LOWER(r.Value) LIKE LOWER('%ptv%')--PTV
	                                                                        AND NOT LOWER(r.Value) LIKE LOWER('%vertical%')--PTV
	                                                                        AND NOT LOWER(r.Value) LIKE LOWER('%bk%')--BK
	                                                                        AND NOT LOWER(r.Value) LIKE LOWER('%mal%')--malária
	                                                                        AND NOT LOWER(r.Value) LIKE LOWER('%circun%')--Circuncisão
	                                                                        AND NOT LOWER(r.Value) LIKE LOWER('%pscico%')--Apoio Psico-Social
	                                                                        AND NOT LOWER(r.Value) LIKE LOWER('%social%')--Acção Social,Apoio Psico-Social
	                                                                        AND NOT LOWER(r.Value) LIKE LOWER('%polic%')--policial
                                                                        ) total";
            numberList.Add(dbContext.Database.SqlQuery<int>(countCompleteReferencesToHealthAndSocialServices,
                                                            new SqlParameter("initialDate", initialDate),
                                                            new SqlParameter("lastDate", lastDate),
                                                            new SqlParameter("ProvID", ProvID),
                                                            new SqlParameter("DistID", DistID),
                                                            new SqlParameter("SiteID", SiteID)).FirstOrDefault());


            String countHouseHoldsThatReceivedFamilyKit = @"select ISNULL(Sum(Cast(FamilyKitReceived As Int)),0)
                                                            from 
                                                            (
	                                                            select cp.Name As ChiefPartner, p.Name As Partner, hh.HouseholdName, hh.HouseholdID,
			                                                            (CASE WHEN rv.FamilyKitReceived = 1 THEN 1 ELSE 0 END) As FamilyKitReceived
			                                                            from RoutineVisit rv 
			                                                            inner join HouseHold hh on (rv.HouseholdID = hh.HouseHoldID)
			                                                            inner join Partner p on (hh.PartnerID = p.PartnerID)
			                                                            inner join Partner cp on (p.SuperiorId = cp.PartnerID)
			                                                            inner join Site s on (p.siteID = s.SiteID)
			                                                            inner join OrgUnit dist on (dist.OrgUnitID = s.OrgUnitID)
			                                                            inner join OrgUnit prov on (dist.ParentOrgUnitId = prov.OrgUnitID)
			                                                            where rv.RoutineVisitDate BETWEEN @initialDate AND @lastDate 
			                                                            AND  
			                                                            ((s.SiteID = @SiteID AND @ProvID = 0 AND @DistID = 0) OR (@SiteID = 0 AND dist.OrgUnitID = @DistID AND @ProvID = 0) OR 
			                                                            (@SiteID = 0 AND @DistID = 0 AND prov.OrgUnitID = @ProvID) OR (@SiteID = 0 AND @DistID = 0 AND @ProvID = 0))
			                                                            group by cp.Name, p.Name, hh.HouseholdName, hh.HouseholdID, rv.FamilyKitReceived		
                                                            ) q";
            numberList.Add(dbContext.Database.SqlQuery<int>(countHouseHoldsThatReceivedFamilyKit,
                                                            new SqlParameter("initialDate", initialDate),
                                                            new SqlParameter("lastDate", lastDate),
                                                            new SqlParameter("ProvID", ProvID),
                                                            new SqlParameter("DistID", DistID),
                                                            new SqlParameter("SiteID", SiteID)).FirstOrDefault());


            String countMUACChildReferencesGreen = @"SELECT  ISNULL(Sum(Cast(SupportValue As Int)),0)
										                     FROM
										                     (
											                 SELECT
												             RVS.SupportType, RVS.SupportID, RVM.ChildID, RVS.SupportValue
											                 FROM  [Partner] p
												             inner join  [Site] s on (p.siteID = s.SiteID)
												             inner join  [OrgUnit] dist on (dist.OrgUnitID = s.OrgUnitID)
												             inner join  [OrgUnit] prov on (dist.ParentOrgUnitId = prov.OrgUnitID)
												             inner join  [HouseHold] hh on (p.PartnerID = hh.PartnerID)
												             inner join  [RoutineVisit] rv on (hh.HouseHoldID = rv.HouseholdID)
												             inner join  [RoutineVisitMember] rvm on (rv.RoutineVisitID = rvm.RoutineVisitID)
												             inner join  [RoutineVisitSupport] rvs on (rvm.RoutineVisitMemberID = rvs.RoutineVisitMemberID)
											                 WHERE rvs.SupportType = 'MUAC' and rvm.ChildID is NOT NULL and rvs.SupportValue != '0' and rvs.SupportID = 1
												             AND (rv.RoutineVisitDate >= @initialDate AND rv.RoutineVisitDate <= @lastDate)
												             AND ((s.SiteID = @SiteID AND @ProvID = 0 AND @DistID = 0) OR (@SiteID = 0 AND dist.OrgUnitID = @DistID AND @ProvID = 0) OR 
												             (@SiteID = 0 AND @DistID = 0 AND prov.OrgUnitID = @ProvID) OR (@SiteID = 0 AND @DistID = 0 AND @ProvID = 0))
										                     ) Result
										                     GROUP BY Result.SupportType, Result.SupportID";
            numberList.Add(dbContext.Database.SqlQuery<int>(countMUACChildReferencesGreen,
                                                            new SqlParameter("initialDate", initialDate),
                                                            new SqlParameter("lastDate", lastDate),
                                                            new SqlParameter("ProvID", ProvID),
                                                            new SqlParameter("DistID", DistID),
                                                            new SqlParameter("SiteID", SiteID)).FirstOrDefault());

            String countMUACChildReferencesYellow = @"SELECT  ISNULL(Sum(Cast(SupportValue As Int)),0)
										                     FROM
										                     (
											                 SELECT
												             RVS.SupportType, RVS.SupportID, RVM.ChildID, RVS.SupportValue
											                 FROM  [Partner] p
												             inner join  [Site] s on (p.siteID = s.SiteID)
												             inner join  [OrgUnit] dist on (dist.OrgUnitID = s.OrgUnitID)
												             inner join  [OrgUnit] prov on (dist.ParentOrgUnitId = prov.OrgUnitID)
												             inner join  [HouseHold] hh on (p.PartnerID = hh.PartnerID)
												             inner join  [RoutineVisit] rv on (hh.HouseHoldID = rv.HouseholdID)
												             inner join  [RoutineVisitMember] rvm on (rv.RoutineVisitID = rvm.RoutineVisitID)
												             inner join  [RoutineVisitSupport] rvs on (rvm.RoutineVisitMemberID = rvs.RoutineVisitMemberID)
											                 WHERE rvs.SupportType = 'MUAC' and rvm.ChildID is NOT NULL and rvs.SupportValue != '0' and rvs.SupportID = 2
												             AND (rv.RoutineVisitDate >= @initialDate AND rv.RoutineVisitDate <= @lastDate)
												             AND ((s.SiteID = @SiteID AND @ProvID = 0 AND @DistID = 0) OR (@SiteID = 0 AND dist.OrgUnitID = @DistID AND @ProvID = 0) OR 
												             (@SiteID = 0 AND @DistID = 0 AND prov.OrgUnitID = @ProvID) OR (@SiteID = 0 AND @DistID = 0 AND @ProvID = 0))
										                     ) Result
										                     GROUP BY Result.SupportType, Result.SupportID";
            numberList.Add(dbContext.Database.SqlQuery<int>(countMUACChildReferencesYellow,
                                                            new SqlParameter("initialDate", initialDate),
                                                            new SqlParameter("lastDate", lastDate),
                                                            new SqlParameter("ProvID", ProvID),
                                                            new SqlParameter("DistID", DistID),
                                                            new SqlParameter("SiteID", SiteID)).FirstOrDefault());

            String countMUACChildReferencesRed = @"SELECT  ISNULL(Sum(Cast(SupportValue As Int)),0)
										                     FROM
										                     (
											                 SELECT
												             RVS.SupportType, RVS.SupportID, RVM.ChildID, RVS.SupportValue
											                 FROM  [Partner] p
												             inner join  [Site] s on (p.siteID = s.SiteID)
												             inner join  [OrgUnit] dist on (dist.OrgUnitID = s.OrgUnitID)
												             inner join  [OrgUnit] prov on (dist.ParentOrgUnitId = prov.OrgUnitID)
												             inner join  [HouseHold] hh on (p.PartnerID = hh.PartnerID)
												             inner join  [RoutineVisit] rv on (hh.HouseHoldID = rv.HouseholdID)
												             inner join  [RoutineVisitMember] rvm on (rv.RoutineVisitID = rvm.RoutineVisitID)
												             inner join  [RoutineVisitSupport] rvs on (rvm.RoutineVisitMemberID = rvs.RoutineVisitMemberID)
											                 WHERE rvs.SupportType = 'MUAC' and rvm.ChildID is NOT NULL and rvs.SupportValue != '0' and rvs.SupportID = 3
												             AND (rv.RoutineVisitDate >= @initialDate AND rv.RoutineVisitDate <= @lastDate)
												             AND ((s.SiteID = @SiteID AND @ProvID = 0 AND @DistID = 0) OR (@SiteID = 0 AND dist.OrgUnitID = @DistID AND @ProvID = 0) OR 
												             (@SiteID = 0 AND @DistID = 0 AND prov.OrgUnitID = @ProvID) OR (@SiteID = 0 AND @DistID = 0 AND @ProvID = 0))
										                     ) Result
										                     GROUP BY Result.SupportType, Result.SupportID";

            numberList.Add(dbContext.Database.SqlQuery<int>(countMUACChildReferencesRed,
                                                            new SqlParameter("initialDate", initialDate),
                                                            new SqlParameter("lastDate", lastDate),
                                                            new SqlParameter("ProvID", ProvID),
                                                            new SqlParameter("DistID", DistID),
                                                            new SqlParameter("SiteID", SiteID)).FirstOrDefault());

            numberList.Add(dbContext.Database.SqlQuery<int>(countMUACChildReferencesRed,
                                                            new SqlParameter("initialDate", initialDate),
                                                            new SqlParameter("lastDate", lastDate),
                                                            new SqlParameter("ProvID", ProvID),
                                                            new SqlParameter("DistID", DistID),
                                                            new SqlParameter("SiteID", SiteID)).FirstOrDefault());


            // criancas que conhecem o seroestado
            numberList.Add(this.GetHIVStatusCountByCriteria(
               initialDate, lastDate, ProvID, DistID, SiteID, dbContext, "H.ChildID != 0 and (H.HIV = 'P' or H.HIV = 'N' " +
                                                             "or (H.HIV = 'U' and H.HIVUndisclosedReason = 0) or " +
                                                             "(H.HIV = 'U' and H.HIVUndisclosedReason = 2)) "));

            // Criancas: Positivo em tarv & Não em TAV & HIV-
            numberList.Add(this.GetHIVStatusCountByCriteria(
               initialDate, lastDate, ProvID, DistID, SiteID, dbContext, "H.ChildID != 0 and H.HIV in ('P', 'N') "));

            // Criancas: HIV Positivo
            numberList.Add(this.GetHIVStatusCountByCriteria(
               initialDate, lastDate, ProvID, DistID, SiteID, dbContext, "H.ChildID != 0 and H.HIV = 'P' "));

            // Criancas: Nao em tarv
            numberList.Add(this.GetHIVStatusCountByCriteria(
                initialDate, lastDate, ProvID, DistID, SiteID, dbContext, "H.ChildID != 0 and H.HIV = 'P' and H.HIVInTreatment = 0"));

            // Criancas: Referidos para TARV
            numberList.Add(CountReferences(initialDate, lastDate, ProvID, DistID, SiteID, dbContext, "Child", false, "DoneForTARV"));

            // Criancas: Referencia completa para TARV
            numberList.Add(CountReferences(initialDate, lastDate, ProvID, DistID, SiteID, dbContext, "Child", true, "CompletedForTARV"));


            // Criancas: Teste não recomendado
            numberList.Add(this.GetHIVStatusCountByCriteria(
                initialDate, lastDate, ProvID, DistID, SiteID, dbContext, "H.ChildID != 0 and H.HIV = 'U' and H.HIVUndisclosedReason = 2 "));

            // Criancas: Desconhecidos
            numberList.Add(this.GetHIVStatusCountByCriteria(
                initialDate, lastDate, ProvID, DistID, SiteID, dbContext, "H.ChildID != 0 and H.HIV = 'U' "));

            // Criancas: Referidos para ATS
            numberList.Add(CountReferences(initialDate, lastDate, ProvID, DistID, SiteID, dbContext, "Child", false, "ATS"));

            //Criancas: Referencia completa para ATS
            numberList.Add(CountReferences(initialDate, lastDate, ProvID, DistID, SiteID, dbContext, "Child", true, "ATS"));

            // Adultos: Conhecem o seroestado
            numberList.Add(this.GetHIVStatusCountByCriteria(
               initialDate, lastDate, ProvID, DistID, SiteID, dbContext, "H.AdultID != 0 and (H.HIV = 'P' or H.HIV = 'N' " +
                                                             "or (H.HIV = 'U' and H.HIVUndisclosedReason = 0) or " +
                                                             "(H.HIV = 'U' and H.HIVUndisclosedReason = 2)) "));

            // Adultos: Positivo em tarv & Não em TAV & HIV-
            numberList.Add(this.GetHIVStatusCountByCriteria(
               initialDate, lastDate, ProvID, DistID, SiteID, dbContext, "H.AdultID != 0 and H.HIV in ('P', 'N') "));

            // Adultos: HIV Positivo
            numberList.Add(this.GetHIVStatusCountByCriteria(
               initialDate, lastDate, ProvID, DistID, SiteID, dbContext, "H.AdultID != 0 and H.HIV = 'P' "));

            // Adultos: Nao em tarv
            numberList.Add(this.GetHIVStatusCountByCriteria(
                initialDate, lastDate, ProvID, DistID, SiteID, dbContext, "H.AdultID != 0 and H.HIV = 'P' and H.HIVInTreatment = 1"));

            // Adultos: Referidos para TARV
            numberList.Add(CountReferences(initialDate, lastDate, ProvID, DistID, SiteID, dbContext, "Adult", false, "DoneForTARV"));

            // Adultos: Referencia completa para TARV
            numberList.Add(CountReferences(initialDate, lastDate, ProvID, DistID, SiteID, dbContext, "Adult", true, "CompletedForTARV"));

            // Adultos: Teste não recomendado
            numberList.Add(this.GetHIVStatusCountByCriteria(
                initialDate, lastDate, ProvID, DistID, SiteID, dbContext, "H.AdultID != 0 and H.HIV = 'U' and H.HIVUndisclosedReason = 2 "));

            // Adultos: Desconhecidos
            numberList.Add(this.GetHIVStatusCountByCriteria(
                initialDate, lastDate, ProvID, DistID, SiteID, dbContext, "H.AdultID != 0 and H.HIV = 'U' "));

            // Adultos: Referidos para ATS
            numberList.Add(CountReferences(initialDate, lastDate, ProvID, DistID, SiteID, dbContext, "Adult", false, "ATS"));

            //Adultos: Referencia completa para ATS
            numberList.Add(CountReferences(initialDate, lastDate, ProvID, DistID, SiteID, dbContext, "Adult", true, "ATS"));

            return numberList;
        }

        private int GetHIVStatusCountByCriteria(DateTime initialDate, DateTime lastDate, int ProvID, int DistID, int SiteID, ApplicationDbContext dbContext, string criteria)
        {
            String hivStatusCountMainQuery = @"SELECT ISNULL(Count(H.HIVStatusID),0) AS HIVP
										                    FROM  [Partner] p
					                                        inner join  [Site] s on (p.siteID = s.SiteID)
					                                        inner join  [OrgUnit] dist on (dist.OrgUnitID = s.OrgUnitID)
					                                        inner join  [OrgUnit] prov on (dist.ParentOrgUnitId = prov.OrgUnitID)
					                                        inner join  [HouseHold] hh on (p.PartnerID = hh.PartnerID)
								                            inner join  [BeneficiaryTable] b on (b.HouseholdID = hh.HouseHoldID)
								                            inner join  [HIVStatus] H on JoinClause
								                            WHERE HIVState
										                    AND H.InformationDate >= @initialDate AND H.InformationDate <= @lastDate
										                    AND ((s.SiteID = @SiteID AND @ProvID = 0 AND @DistID = 0) OR (@SiteID = 0 AND dist.OrgUnitID = @DistID AND @ProvID = 0) OR
										                    (@SiteID = 0 AND @DistID = 0 AND prov.OrgUnitID = @ProvID) OR (@SiteID = 0 AND @DistID = 0 AND @ProvID = 0))";

            hivStatusCountMainQuery = hivStatusCountMainQuery.Replace("[BeneficiaryTable]", criteria.Contains("Child") ? "[Child]" : "[Adult]");
            hivStatusCountMainQuery = hivStatusCountMainQuery.Replace("JoinClause", criteria.Contains("Child") ? "(b.ChildID = H.ChildID)" : "(b.AdultId = H.AdultID)");
            hivStatusCountMainQuery = hivStatusCountMainQuery.Replace("HIVState", criteria);

            return dbContext.Database.SqlQuery<int>(hivStatusCountMainQuery,
                                                                        new SqlParameter("initialDate", initialDate),
                                                                        new SqlParameter("lastDate", lastDate),
                                                                        new SqlParameter("ProvID", ProvID),
                                                                        new SqlParameter("DistID", DistID),
                                                                        new SqlParameter("SiteID", SiteID)).FirstOrDefault();
        }

        private int GetCountForQuery(DateTime initialDate, DateTime lastDate, int ProvID, int DistID, int SiteID, ApplicationDbContext dbContext, string queryString)
        {
            return dbContext.Database.SqlQuery<int>(queryString,
                                                                        new SqlParameter("initialDate", initialDate),
                                                                        new SqlParameter("lastDate", lastDate),
                                                                        new SqlParameter("ProvID", ProvID),
                                                                        new SqlParameter("DistID", DistID),
                                                                        new SqlParameter("SiteID", SiteID)).FirstOrDefault();
        }

        private int CountReferences(DateTime initialDate, DateTime lastDate, int ProvID, int DistID, int SiteID,
            ApplicationDbContext dbContext, String BeneficiaryType, bool isComplete, String referenceType)
        {
            Dictionary<string, string> referenceMapping = new Dictionary<string, string>();
            referenceMapping.Add("DoneForTARV", "'PTV', 'Testado HIV+', 'Pré-TARV/IO', 'PPE', 'Abandono TARV'");
            referenceMapping.Add("CompletedForTARV", "'Pré TARV/IO','Testado HIV+','PPE','PTV','TARV','CD'");
            referenceMapping.Add("ATS", "'ATS'");

            String BeneficiaryID = BeneficiaryType == "Adult" ? "a.AdultId" : "c.ChildID";
            String BeneficiaryJoin = BeneficiaryType == "Adult" ? "[Adult] a on (a.HouseholdID = hh.HouseHoldID) "
                : "[Child] c on (c.HouseholdID = hh.HouseHoldID)  ";
            String ReferenceCategoryClause = isComplete ? "in ('Social', 'Health') " : " = 'Activist'";
            String ReferenceDates = isComplete ? "((RS.HealthAttendedDate >= @initialDateHealth AND RS.HealthAttendedDate <= @lastDateHealth) " +
                "OR (RS.SocialAttendedDate >= @initialDateSocial AND RS.SocialAttendedDate <= @lastDateSocial)) "
                : " (RS.ReferenceDate >= @initialDate AND RS.ReferenceDate <= @lastDate) ";
            String ReferenceNameList = referenceMapping[referenceType];

            String query = @"SELECT
	                                ISNULL(count(distinct(BeneficiaryID)), 0)
	                                    FROM  [Partner] p
	                                        INNER JOIN  [Site] s on (p.siteID = s.SiteID)
	                                        INNER JOIN  [OrgUnit] dist on (dist.OrgUnitID = s.OrgUnitID)
	                                        INNER JOIN  [OrgUnit] prov on (dist.ParentOrgUnitId = prov.OrgUnitID)
	                                        INNER JOIN  [HouseHold] hh on (p.PartnerID = hh.PartnerID)
	                                        INNER JOIN  BeneficiaryJoin 
	                                        INNER JOIN  [RoutineVisit] rv on (hh.HouseHoldID = rv.HouseholdID)
	                                        INNER JOIN  [RoutineVisitMember] rvm on (rv.RoutineVisitID = rvm.RoutineVisitID)
	                                        INNER JOIN  [ReferenceService] rs on (rvm.RoutineVisitMemberID = rs.RoutineVisitMemberID)
	                                        INNER JOIN  [Reference] r on (rs.ReferenceServiceID = r.ReferenceServiceID)
	                                        INNER JOIN  [ReferenceType] rt on (r.ReferenceTypeID = rt.ReferenceTypeID)
	                                        WHERE RT.FieldType = 'CheckBox' AND RT.ReferenceCategory ReferenceCategoryClause AND R.Value = '1'
	                                        AND RT.ReferenceName in (ReferenceNameList)
	                                        AND ReferenceDates
	                                        AND ((s.SiteID = @SiteID AND @ProvID = 0 AND @DistID = 0) OR (@SiteID = 0 AND dist.OrgUnitID = @DistID AND @ProvID = 0) OR 
	                                        (@SiteID = 0 AND @DistID = 0 AND prov.OrgUnitID = @ProvID) OR (@SiteID = 0 AND @DistID = 0 AND @ProvID = 0))"
                                            .Replace("BeneficiaryID", BeneficiaryID)
                                            .Replace("BeneficiaryJoin", BeneficiaryJoin)
                                            .Replace("ReferenceCategoryClause", ReferenceCategoryClause)
                                            .Replace("ReferenceNameList", ReferenceNameList)
                                            .Replace("ReferenceDates", ReferenceDates);

            if (isComplete)
            {
                return dbContext.Database.SqlQuery<int>(query,
                                                           new SqlParameter("initialDateHealth", initialDate),
                                                           new SqlParameter("lastDateHealth", lastDate),
                                                           new SqlParameter("initialDateSocial", initialDate),
                                                           new SqlParameter("lastDateSocial", lastDate),
                                                           new SqlParameter("ProvID", ProvID),
                                                           new SqlParameter("DistID", DistID),
                                                           new SqlParameter("SiteID", SiteID)).FirstOrDefault();
            }

            return dbContext.Database.SqlQuery<int>(query,
                                                                new SqlParameter("initialDate", initialDate),
                                                                new SqlParameter("lastDate", lastDate),
                                                                new SqlParameter("ProvID", ProvID),
                                                                new SqlParameter("DistID", DistID),
                                                                new SqlParameter("SiteID", SiteID)).FirstOrDefault();
        }
    }
}
