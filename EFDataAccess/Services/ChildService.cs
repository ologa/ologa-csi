using EFDataAccess.UOW;
using System;
using System.Collections.Generic;
using System.Linq;
using VPPS.CSI.Domain;
using EFDataAccess.DTO;
using System.Data.SqlClient;
using System.Data;
using System.Data.Entity;

namespace EFDataAccess.Services
{
    public class ChildService : BaseService
    {
        private UserService UserService;
        private AdultService AdultService;
        private HouseholdService HouseholdService;
        private HIVStatusService HIVStatusService;
        private HIVStatusQueryService HIVStatusQueryService;

        public ChildService(UnitOfWork uow) : base(uow)
        {
            UserService = new UserService(uow);
            AdultService = new AdultService(uow);
            HouseholdService = new HouseholdService(uow);
            HIVStatusService = new HIVStatusService(uow);
            HIVStatusQueryService = new HIVStatusQueryService(uow);
        }

        private EFDataAccess.Repository.IRepository<Child> ChildRepository => UnitOfWork.Repository<Child>();

        public int CountBySite(Site site)
        {
            return UnitOfWork.DbContext.Database.SqlQuery<int>(
                @"select Count(*) from Beneficiary b 
                inner join HouseHold hh on (hh.HouseHoldID = b.HouseHoldID)
                inner join Partner p on (hh.PartnerID = p.PartnerID)
                and (p.SiteID = @SiteID or @SiteID = 0) and DATEDIFF(year, CAST(b.DateOfBirth As Date), GETDATE()) < 18",
                new SqlParameter("SiteID", site.SiteID)).SingleOrDefault();
        }

        public Child findChildBySyncGuid(Guid ChildGuid) { return ChildRepository.GetAll().Where(c => c.SyncGuid == ChildGuid).FirstOrDefault(); }

        public List<UniqueEntity> FindAllChildUniqueEntities() => UnitOfWork.DbContext.Database.SqlQuery<UniqueEntity>("Select ISNULL(SyncGuid, cast(cast(0 as binary) as uniqueidentifier)) As SyncGuid, ChildID As ID from Child").ToList();

        public Child FetchById(int id) { return ChildRepository.GetAll().Where(x => x.ChildID == id).Include(x => x.HIVStatus).Include(x => x.Household).Include(x => x.KinshipToFamilyHead).Include(x => x.OVCType).FirstOrDefault(); }

        public IQueryable<Child> findByname(string name)
        {
            return UnitOfWork.Repository<Child>()
                .GetAll()
                .Where(c => c.FirstName.Contains(name) || c.LastName.Contains(name))
                .Include(e => e.Household);
        }

        public IQueryable<Child> findBynameAndPartnerAndCode(string name, string partnerName, string beneficiaryCode)
        {
            return UnitOfWork.Repository<Child>()
                .GetAll()
                .Where(c => c.FirstName.Contains(name) || c.LastName.Contains(name))
                .Where(e => (name == null || name.Equals("") || e.FirstName.Contains(name) || e.LastName.Contains(name))
                && (partnerName == null || partnerName.Equals("") || e.Household.Partner.Name.Contains(partnerName))
                && (beneficiaryCode == null || beneficiaryCode.Equals("") || e.Code.Contains(beneficiaryCode))).Include(e => e.Household);
        }

        public List<Child> FetchByHouseholdIDandStatusID(int householdID, int StatusID)
        {
            return ChildRepository.GetAll()
                .Include(e => e.ChildStatusHistories.Select(x => x.ChildStatus))
                .Where(e => e.ChildStatusHistories.OrderByDescending(x => x.EffectiveDate)
                .ThenByDescending(x => x.ChildStatusHistoryID).FirstOrDefault().ChildStatus.Description != "Adulto")
                .Where(e => e.ChildStatusHistories.OrderByDescending(x => x.EffectiveDate)
                .ThenByDescending(x => x.ChildStatusHistoryID).FirstOrDefault().ChildStatus.StatusID != StatusID)
                .Where(x => x.HouseholdID == householdID).ToList();
        }

        public Child FetchByChildIDandStatusID(int childID, int StatusID)
        {
            return ChildRepository.GetAll()
                .Include(e => e.ChildStatusHistories.Select(x => x.ChildStatus))
                .Where(e => e.ChildStatusHistories.OrderByDescending(x => x.EffectiveDate)
                .ThenByDescending(x => x.ChildStatusHistoryID).FirstOrDefault().ChildStatus.Description != "Adulto")
                .Where(e => e.ChildStatusHistories.OrderByDescending(x => x.EffectiveDate)
                .ThenByDescending(x => x.ChildStatusHistoryID).FirstOrDefault().ChildStatus.StatusID != StatusID)
                .Where(x => x.ChildID == childID).FirstOrDefault();
        }

        public Child Reload(Child entity) { ChildRepository.FullReload(entity); return entity; }

        public void Delete(Child Child)
        {
            UnitOfWork.Repository<HIVStatus>().GetAll().Where(x => x.ChildID == Child.ChildID).ToList().ForEach(x => UnitOfWork.Repository<HIVStatus>().Delete(x));
            UnitOfWork.Repository<ChildStatusHistory>().GetAll().Where(x => x.ChildID == Child.ChildID).ToList().ForEach(x => UnitOfWork.Repository<ChildStatusHistory>().Delete(x));
            ChildRepository.Delete(Child);
        }

        public Child Get(int ChildID) { return ChildRepository.GetById(ChildID); }

        public void Save(Child Child)
        {
            if (Child.ChildID > 0)
            { ChildRepository.Update(Child); } 
            else
            { ChildRepository.Add(Child); }
        }

        public int ImportData(string path)
        {
            _logger.Information("IMPORTACAO DE CRIANCAS ...");

            UnitOfWork.DbContext.Configuration.AutoDetectChangesEnabled = false;
            FileImporter imp = new FileImporter();
            string fullPath = path + @"\Children.csv";

            int ImportedChilds = 0;
            string lastGuidToImport = null;
            List<Child> ChildrenToPersist = new List<Child>();
            List<UniqueEntity> ChildDB = FindAllChildUniqueEntities();
            List<UniqueEntity> HouseholdsDB = HouseholdService.findAllHouseholdUniqueEntities();
            List<UniqueEntity> HIVStatusDB = HIVStatusQueryService.FindAllHIVStatusUniqueEntity();
            IEnumerable<DataRow> ChildRows = imp.ImportFromCSV(fullPath).Rows.Cast<DataRow>();
            UsersDB = ConvertListToHashtableUsers(findAllUsersUniqueEntities());

            try
            {
                foreach (DataRow row in ChildRows)
                {
                    Guid childGuid = new Guid(row["child_guid"].ToString());
                    UniqueEntity Household = FindBySyncGuid(HouseholdsDB, new Guid(row["HouseHoldGuid"].ToString()));
                    UniqueEntity HIVStatus = FindBySyncGuid(HIVStatusDB, new Guid(row["HIVStatusGuid"].ToString()));

                    if (Household == null)
                    { _logger.Error("Crianca com o Guid '{0}' tem HouseHold com Guid '{1}' em falta. Este nao sera importado.", childGuid, row["HouseHoldGuid"].ToString()); }
                    else if (HIVStatus == null)
                    { _logger.Error("Crianca com o Guid '{0}' tem HIVStatus com Guid '{1}' em falta. Este nao sera importado.", childGuid, row["HIVStatusGuid"].ToString()); }
                    else
                    {
                        Child child = FindBySyncGuid(ChildDB, childGuid) == null ? child = new Child() : findChildBySyncGuid(childGuid);
                        lastGuidToImport = childGuid.ToString();
                        child.Code = row["Code"].ToString();
                        child.FirstName = row["FirstName"].ToString();
                        child.LastName = row["LastName"].ToString();
                        child.DateOfBirth = (row["DateOfBirth"].ToString()).Length == 0 ? (DateTime?)null : DateTime.Parse(row["DateOfBirth"].ToString());
                        child.DateOfBirthUnknown = row["IsDateOfBirthUnknownInt"].ToString().Equals("1") ? true : false;
                        child.RegistrationDate = (row["RegistrationDate"].ToString()).Length == 0 ? (DateTime?) null : DateTime.Parse(row["RegistrationDate"].ToString());
                        child.RegistrationDateDifferentFromHouseholdDate = row["IsRegistrationDateDifferentFromHouseholdDateInt"].ToString().Equals("1") ? true : false;
                        child.IsPartSavingGroup = row["IsPartSavingGroupInt"].ToString().Equals("1") ? true : false;
                        child.HIVTracked = row["IsHIVTrackedInt"].ToString().Equals("1") ? true : false;
                        child.Gender = row["Gender"].ToString();
                        child.NID = row["NID"].ToString();
                        child.HouseholdID = Household.ID;
                        child.HIVStatusID = HIVStatus.ID;
                        if (row["KinshipToFamilyHeadID"].ToString().Length > 0) { child.KinshipToFamilyHeadID = int.Parse(row["KinshipToFamilyHeadID"].ToString()); }
                        if (row["OtherKinshipToFamilyHead"].ToString().Length > 0) { child.OtherKinshipToFamilyHead = row["OtherKinshipToFamilyHead"].ToString(); }
                        if (row["OVCTypeID"].ToString().Length > 0) { child.OVCTypeID = int.Parse(row["OVCTypeID"].ToString()); }
                        if (row["OrgUnitID"].ToString().Length > 0) { child.OrgUnitID = int.Parse(row["OrgUnitID"].ToString()); }
                        SetCreationDataFields(child, row, childGuid);
                        SetUpdatedDataFields(child, row);
                        ChildrenToPersist.Add(child);
                        ImportedChilds++;
                    }

                    if (ImportedChilds % 100 == 0)
                    { _logger.Information(ImportedChilds + " de " + ChildRows.Count() + " Criancas importadas."); }
                }

                ChildrenToPersist.ForEach(c => Save(c));
                UnitOfWork.Commit();
                Rename(fullPath, fullPath + IMPORTED);
            }
            catch (Exception e)
            {
                _logger.Information("Erro ao importar o Guid : " + lastGuidToImport);
                _logger.Error(e, "Erro ao importar Criancas", null);
                throw e;
            }

            UnitOfWork.Dispose();
            return ChildrenToPersist.Count();
        }

        public int MigrateChildToAdult()
        {
            int migratedChildCount = 0;
            List<Child> children = UnitOfWork.Repository<Child>().GetAll()
                .Include(x => x.Household).Include(x => x.HIVStatus)
                .Include(e => e.ChildStatusHistories.Select(x => x.ChildStatus))
                .Where(e => e.ChildStatusHistories.OrderByDescending(x => x.EffectiveDate)
                .ThenByDescending(x => x.ChildStatusHistoryID).FirstOrDefault().ChildStatus.Description != "Adulto")
                .Where(y => (DateTime.Now.Year - y.DateOfBirth.Value.Year) > 17)
                .ToList();

            User adminUser = UnitOfWork.Repository<User>().GetById(1);
            ChildStatus adultStatus = UnitOfWork.Repository<ChildStatus>().GetAll().Where(s => s.Description.Equals("Adulto")).SingleOrDefault();
           
            foreach (var child in children)
            {
                //AdultService.CreateAdultFromChild(child, adminUser);

                // Set adultStatus for the grown child
                ChildStatusHistory newAdultStatus = new ChildStatusHistory();
                newAdultStatus.Child = child;
                newAdultStatus.ChildStatus = adultStatus;
                newAdultStatus.childstatushistory_guid = Guid.NewGuid();
                newAdultStatus.EffectiveDate = DateTime.Today;
                newAdultStatus.CreatedDate = DateTime.Today;
                newAdultStatus.CreatedUser = adminUser;
                UnitOfWork.Repository<ChildStatusHistory>().Add(newAdultStatus);
                migratedChildCount++;
            }

            UnitOfWork.Commit();
            return migratedChildCount;
        }

        public bool ChildValidation(string firstname, string lastname, DateTime? birthdate)
        {
            return (from child in UnitOfWork.DbContext.Children
                    where child.FirstName == firstname
                    && child.LastName == lastname
                    && child.DateOfBirth == birthdate
                    select child.FirstName).Any();
        }

        public int LockData() => LockAndUnlockData(3, new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, 20));

        public int UnlockData(DateTime unlockDate) => LockAndUnlockData(0, unlockDate);

        public int LockAndUnlockData(int state, DateTime lockOrUnlockDate)
        {
            UnitOfWork.DbContext.Configuration.AutoDetectChangesEnabled = false;
            List<Child> childToLockOrUnlock = UnitOfWork.Repository<Child>().GetAll().Where(x => x.Household.RegistrationDate <= lockOrUnlockDate).ToList();

            foreach (Child c in childToLockOrUnlock)
            {
                c.State = state;
                Save(c);
            }

            Commit();
            UnitOfWork.DbContext.Configuration.AutoDetectChangesEnabled = true;
            return childToLockOrUnlock.Count();
        }

        /*
         * #######################################################
         * ############## ChildQuestionReport ##############
         * #######################################################
         */

        public List<ChildQuestionSummaryReportDTO> getChildQuestionReport(DateTime initialDate, DateTime lastDate, int partnerID)
        {
            String query = @"SELECT 
		                        Partner.Name AS Partner
		                        ,(FirstName) + ' ' + (LastName) As FullName
		                        , Child.Gender
		                        , DATEDIFF(YEAR, DateOFBirth, GETDATE()) AS Age
		                        ,CONVERT(varchar,CSI.IndexDate,103) AS CSIDate
		                        --, Child.ChildID AS ChildID
		                        --,Question.Description
		                        --,t.col1 as a,
                                ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='1-' THEN ScoreType.Score  END), -1) As P1
                                ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='2-' THEN ScoreType.Score  END), -1) As P2
                                ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='3-' THEN ScoreType.Score  END), -1) As P3
                                ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='4-' THEN ScoreType.Score  END), -1) As P4
                                ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='5-' THEN ScoreType.Score  END), -1) As P5
                                ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='6-' THEN ScoreType.Score  END), -1) As P6
                                ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='7-' THEN ScoreType.Score  END), -1) As P7
                                ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='8-' THEN ScoreType.Score  END), -1) As P8
                                ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='9-' THEN ScoreType.Score  END), -1) As P9
                                ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='10-' THEN ScoreType.Score  END), -1) As P10
                                ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='11-' THEN ScoreType.Score  END), -1) As P11
                                ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='12-' THEN ScoreType.Score  END), -1) As P12
                                ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='13-' THEN ScoreType.Score  END), -1) As P13
                                ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='14-' THEN ScoreType.Score  END), -1) As P14
                                ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='15-' THEN ScoreType.Score  END), -1) As P15
                                ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='16-' THEN ScoreType.Score  END), -1) As P16
                                ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='17-' THEN ScoreType.Score  END), -1) As P17
                                ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='18-' THEN ScoreType.Score  END), -1) As P18
                                ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='19-' THEN ScoreType.Score  END), -1) As P19
                                ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='20-' THEN ScoreType.Score  END), -1) As P20
                                ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='21-' THEN ScoreType.Score  END), -1) As P21
                                ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='22-' THEN ScoreType.Score  END), -1) As P22
                                ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='23-' THEN ScoreType.Score  END), -1) As P23
                                ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='24-' THEN ScoreType.Score  END), -1) As P24
                                ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='25-' THEN ScoreType.Score  END), -1) As P25
                                ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='26-' THEN ScoreType.Score  END), -1) As P26
                                ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='27-' THEN ScoreType.Score  END), -1) As P27
                                ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='28-' THEN ScoreType.Score  END), -1) As P28
                                ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='29-' THEN ScoreType.Score  END), -1) As P29
                                ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='30-' THEN ScoreType.Score  END), -1) As P30
                                ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='31-' THEN ScoreType.Score  END), -1) As P31
                                ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='32-' THEN ScoreType.Score  END), -1) As P32
                                ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='33-' THEN ScoreType.Score  END), -1) As P33
		                        --, DATENAME(d, CSI.IndexDate) + ' ' + DATENAME(m, CSI.IndexDate) + ' ' + DATENAME(YEAR, CSI.IndexDate) AS CSIDate
		                        --, Domain.Description as Domain
                        FROM 
	                         [Partner]
                        INNER JOIN
	                         [HouseHold] ON [HouseHold].PartnerID = [Partner].PartnerID
                        INNER JOIN
	                         Child ON [HouseHold].HouseHoldID = Child.HouseHoldID
                        INNER JOIN 
	                         CSI ON Child.ChildID = CSI.ChildID 
                        INNER JOIN 
	                         CSIDomain ON CSI.CSIID = CSIDomain.CSIID
                        INNER JOIN 
	                         Domain ON Domain.DomainID = CSIDomain.DomainID
                        INNER JOIN 
	                         CSIDomainScore ON CSIDomain.CSIDomainID = CSIDomainScore.CSIDomainID
                        INNER JOIN 
	                         Question ON CSIDomainScore.QuestionID = Question.QuestionID
                        INNER JOIN 
	                         Answer ON CSIDomainScore.AnswerID = Answer.AnswerID
                        INNER JOIN 
	                         ScoreType ON ScoreType.ScoreTypeID = Answer.ScoreID
                        --WHERE CSI.IndexDate >= @initialDate AND CSI.IndexDate <= @lastDate
                        WHERE ([Partner].PartnerID = @partnerID or @partnerID = 0)
                        AND 
                        CSI.CSIID IN
                        (
	
		                        SELECT
			                        obj.CSIID
		                        FROM
		                        (
			                        SELECT 
				                        row_number() OVER (PARTITION BY [ChildID] ORDER BY IndexDate DESC) AS numeroLinha
				                        --Obter o número da linha de acordo com ChildID, e ordenado pelo ID do Histórico de forma DESCENDENTE(Último ao Primeiro)
				                        ,CSIID
				                        ,[ChildID]
				                        ,IndexDate
			                        FROM 
				                         CSI
			                        WHERE CSI.IndexDate >= @initialDate AND CSI.IndexDate <= @lastDate
		                        )
		                        obj
		                        WHERE 
			                        obj.numeroLinha=1
                        )
                        GROUP BY 
	                        Partner.Name
	                        ,Child.FirstName
	                        ,Child.LastName
	                        ,Child.Gender
	                        ,Child.DateOfBirth
	                        , CSI.IndexDate
                        ORDER BY
	                        Partner.Name";

            return UnitOfWork.DbContext.Database.SqlQuery<ChildQuestionSummaryReportDTO>(query,
                                                            new SqlParameter("initialDate", initialDate),
                                                            new SqlParameter("lastDate", lastDate),
                                                            new SqlParameter("partnerID", partnerID)).ToList();
        }


        /*
         * #######################################################
         * ############## AgreggatedChildQuestionReport ##########
         * #######################################################
         */

        public List<ChildQuestionSummaryReportDTO> getAgreggatedChildQuestionReport(int ProvID, int DistID, int SiteID, DateTime initialDate, DateTime lastDate, int partnerID)
        {
            String query = @"SELECT
	                            prov.Name As Province
	                            ,dist.Name As District
	                            ,s.SiteName As SiteName
	                            ,childquestionobj.Partner
	                            ,childquestionobj.FullName
	                            ,childquestionobj.Gender
	                            ,childquestionobj.Age
	                            ,childquestionobj.CSIDate
	                            ,childquestionobj.P1
	                            ,childquestionobj.P2
	                            ,childquestionobj.P3
	                            ,childquestionobj.P4
	                            ,childquestionobj.P5
	                            ,childquestionobj.P6
	                            ,childquestionobj.P7
	                            ,childquestionobj.P8
	                            ,childquestionobj.P9
	                            ,childquestionobj.P10
	                            ,childquestionobj.P11
	                            ,childquestionobj.P12
	                            ,childquestionobj.P13
	                            ,childquestionobj.P14
	                            ,childquestionobj.P15
	                            ,childquestionobj.P16
	                            ,childquestionobj.P17
	                            ,childquestionobj.P18
	                            ,childquestionobj.P19
	                            ,childquestionobj.P20
	                            ,childquestionobj.P21
	                            ,childquestionobj.P22
	                            ,childquestionobj.P23
	                            ,childquestionobj.P24
	                            ,childquestionobj.P25
	                            ,childquestionobj.P26
	                            ,childquestionobj.P27
	                            ,childquestionobj.P28
	                            ,childquestionobj.P29
	                            ,childquestionobj.P30
	                            ,childquestionobj.P31
	                            ,childquestionobj.P32
	                            ,childquestionobj.P33
                            FROM
	                             [Partner] part
	                            inner join  [Site] s on (part.siteID = s.SiteID)
	                            inner join  [OrgUnit] dist on (dist.OrgUnitID = s.OrgUnitID)
	                            inner join  [OrgUnit] prov on (dist.ParentOrgUnitId = prov.OrgUnitID)
                            INNER JOIN 
                            (
		                            SELECT 
				                            Partner.Name AS Partner
				                            ,(FirstName) + ' ' + (LastName) As FullName
				                            , Child.Gender
				                            , DATEDIFF(YEAR, DateOFBirth, GETDATE()) AS Age
				                            ,CONVERT(varchar,CSI.IndexDate,103) AS CSIDate
				                            --, Child.ChildID AS ChildID
				                            --,Question.Description
				                            --,t.col1 as a,
				                            ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='1-' THEN ScoreType.Score  END), -1) As P1
				                            ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='2-' THEN ScoreType.Score  END), -1) As P2
				                            ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='3-' THEN ScoreType.Score  END), -1) As P3
				                            ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='4-' THEN ScoreType.Score  END), -1) As P4
				                            ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='5-' THEN ScoreType.Score  END), -1) As P5
				                            ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='6-' THEN ScoreType.Score  END), -1) As P6
				                            ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='7-' THEN ScoreType.Score  END), -1) As P7
				                            ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='8-' THEN ScoreType.Score  END), -1) As P8
				                            ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='9-' THEN ScoreType.Score  END), -1) As P9
				                            ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='10-' THEN ScoreType.Score  END), -1) As P10
				                            ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='11-' THEN ScoreType.Score  END), -1) As P11
				                            ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='12-' THEN ScoreType.Score  END), -1) As P12
				                            ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='13-' THEN ScoreType.Score  END), -1) As P13
				                            ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='14-' THEN ScoreType.Score  END), -1) As P14
				                            ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='15-' THEN ScoreType.Score  END), -1) As P15
				                            ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='16-' THEN ScoreType.Score  END), -1) As P16
				                            ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='17-' THEN ScoreType.Score  END), -1) As P17
				                            ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='18-' THEN ScoreType.Score  END), -1) As P18
				                            ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='19-' THEN ScoreType.Score  END), -1) As P19
				                            ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='20-' THEN ScoreType.Score  END), -1) As P20
				                            ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='21-' THEN ScoreType.Score  END), -1) As P21
				                            ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='22-' THEN ScoreType.Score  END), -1) As P22
				                            ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='23-' THEN ScoreType.Score  END), -1) As P23
				                            ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='24-' THEN ScoreType.Score  END), -1) As P24
				                            ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='25-' THEN ScoreType.Score  END), -1) As P25
				                            ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='26-' THEN ScoreType.Score  END), -1) As P26
				                            ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='27-' THEN ScoreType.Score  END), -1) As P27
				                            ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='28-' THEN ScoreType.Score  END), -1) As P28
				                            ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='29-' THEN ScoreType.Score  END), -1) As P29
				                            ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='30-' THEN ScoreType.Score  END), -1) As P30
				                            ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='31-' THEN ScoreType.Score  END), -1) As P31
				                            ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='32-' THEN ScoreType.Score  END), -1) As P32
				                            ,ISNULL(MAX(CASE WHEN SUBSTRING ( Question.Description,1,3)='33-' THEN ScoreType.Score  END), -1) As P33
				                            --, DATENAME(d, CSI.IndexDate) + ' ' + DATENAME(m, CSI.IndexDate) + ' ' + DATENAME(YEAR, CSI.IndexDate) AS CSIDate
				                            --, Domain.Description as Domain
		                            FROM 
			                             [Partner]
		                            INNER JOIN
			                             [HouseHold] ON [HouseHold].PartnerID = [Partner].PartnerID
		                            INNER JOIN
			                             Child ON [HouseHold].HouseHoldID = Child.HouseHoldID
		                            INNER JOIN 
			                             CSI ON Child.ChildID = CSI.ChildID 
		                            INNER JOIN 
			                             CSIDomain ON CSI.CSIID = CSIDomain.CSIID
		                            INNER JOIN 
			                             Domain ON Domain.DomainID = CSIDomain.DomainID
		                            INNER JOIN 
			                             CSIDomainScore ON CSIDomain.CSIDomainID = CSIDomainScore.CSIDomainID
		                            INNER JOIN 
			                             Question ON CSIDomainScore.QuestionID = Question.QuestionID
		                            INNER JOIN 
			                             Answer ON CSIDomainScore.AnswerID = Answer.AnswerID
		                            INNER JOIN 
			                             ScoreType ON ScoreType.ScoreTypeID = Answer.ScoreID
		                            --WHERE CSI.IndexDate >= @initialDate AND CSI.IndexDate <= @lastDate
		                            WHERE ([Partner].PartnerID = @partnerID or @partnerID = 0)
		                            AND 
		                            CSI.CSIID IN
		                            (
	
				                            SELECT
					                            obj.CSIID
				                            FROM
				                            (
					                            SELECT 
						                            row_number() OVER (PARTITION BY [ChildID] ORDER BY IndexDate DESC) AS numeroLinha
						                            --Obter o número da linha de acordo com ChildID, e ordenado pelo ID do Histórico de forma DESCENDENTE(Último ao Primeiro)
						                            ,CSIID
						                            ,[ChildID]
						                            ,IndexDate
					                            FROM 
						                             CSI
					                            WHERE CSI.IndexDate >= @initialDate AND CSI.IndexDate <= @lastDate
				                            )
				                            obj
				                            WHERE 
					                            obj.numeroLinha=1
		                            )
		                            GROUP BY 
			                            Partner.Name
			                            ,Child.FirstName
			                            ,Child.LastName
			                            ,Child.Gender
			                            ,Child.DateOfBirth
			                            , CSI.IndexDate
		                            --ORDER BY
			                            --Partner.Name
                            )childquestionobj
                            ON childquestionobj.Partner = part.Name
                            WHERE
	                            ((s.SiteID = @SiteID AND prov.OrgUnitID = @ProvID AND dist.OrgUnitID = @DistID) OR
	                            (s.SiteID = @SiteID AND @ProvID = 0 AND @DistID = 0) OR
	                            (@SiteID = 0 AND dist.OrgUnitID = @DistID AND @ProvID = 0) OR 
	                            (@SiteID = 0 AND @DistID = 0 AND prov.OrgUnitID = @ProvID) OR
	                            (@SiteID = 0 AND @DistID = 0 AND @ProvID = 0))
                            GROUP BY 
	                            prov.Name
	                            ,dist.Name
	                            ,s.SiteName
	                            ,childquestionobj.Partner
	                            ,childquestionobj.FullName
	                            ,childquestionobj.Gender
	                            ,childquestionobj.Age
	                            ,childquestionobj.CSIDate
	                            ,childquestionobj.P1
	                            ,childquestionobj.P2
	                            ,childquestionobj.P3
	                            ,childquestionobj.P4
	                            ,childquestionobj.P5
	                            ,childquestionobj.P6
	                            ,childquestionobj.P7
	                            ,childquestionobj.P8
	                            ,childquestionobj.P9
	                            ,childquestionobj.P10
	                            ,childquestionobj.P11
	                            ,childquestionobj.P12
	                            ,childquestionobj.P13
	                            ,childquestionobj.P14
	                            ,childquestionobj.P15
	                            ,childquestionobj.P16
	                            ,childquestionobj.P17
	                            ,childquestionobj.P18
	                            ,childquestionobj.P19
	                            ,childquestionobj.P20
	                            ,childquestionobj.P21
	                            ,childquestionobj.P22
	                            ,childquestionobj.P23
	                            ,childquestionobj.P24
	                            ,childquestionobj.P25
	                            ,childquestionobj.P26
	                            ,childquestionobj.P27
	                            ,childquestionobj.P28
	                            ,childquestionobj.P29
	                            ,childquestionobj.P30
	                            ,childquestionobj.P31
	                            ,childquestionobj.P32
	                            ,childquestionobj.P33
                            ORDER BY 
	                            prov.Name
	                            ,dist.Name
	                            ,s.SiteName
	                            ,childquestionobj.Partner";

            return UnitOfWork.DbContext.Database.SqlQuery<ChildQuestionSummaryReportDTO>(query,
                                                            new SqlParameter("ProvID", ProvID),
                                                            new SqlParameter("DistID", DistID),
                                                            new SqlParameter("SiteID", SiteID),
                                                            new SqlParameter("initialDate", initialDate),
                                                            new SqlParameter("lastDate", lastDate),
                                                            new SqlParameter("partnerID", partnerID)).ToList();
        }



        /*
         * #######################################################
         * ############## ChildByQuestionReport ##############
         * #######################################################
         */

        public List<ChildByQuestionReportDTO> getChildByQuestionReport(DateTime initialDate, DateTime lastDate, int partnerID, int questionID, int scoreTypeID)
        {

            String query = @"SELECT 
	                            Partner.Name As Partner,
	                            (FirstName) + ' ' + (LastName) As FullName,
	                            DATEDIFF(YEAR, DateOFBirth, GETDATE()) AS Age,
                                Child.Gender,
	                            CONVERT(varchar,CSI.IndexDate,103) AS CSIDate,
                                Domain.Description as Domain,
	                            Question.Description,
	                            ScoreType.Score
                            FROM Child 
	                            INNER JOIN HouseHold ON HouseHold.HouseHoldID = CHild.HouseholdID
	                            INNER JOIN Partner ON HouseHold.PartnerID = [Partner].PartnerID
	                            INNER JOIN CSI ON Child.ChildID = CSI.ChildID 
	                            INNER JOIN CSIDomain ON CSI.CSIID = CSIDomain.CSIID
	                            INNER JOIN Domain ON Domain.DomainID = CSIDomain.DomainID
	                            INNER JOIN CSIDomainScore ON CSIDomain.CSIDomainID = CSIDomainScore.CSIDomainID
	                            INNER JOIN Question ON CSIDomainScore.QuestionID = Question.QuestionID
	                            INNER JOIN Answer ON CSIDomainScore.AnswerID = Answer.AnswerID
	                            INNER JOIN ScoreType ON ScoreType.ScoreTypeID = Answer.ScoreID
                            WHERE CSI.IndexDate >= @initialDate AND CSI.IndexDate <= @lastDate
                            AND (Partner.PartnerID = @partnerID or @partnerID = 0)
                            AND Question.QuestionID = @questionID
                            AND ScoreType.ScoreTypeID = @scoreTypeID
                            ORDER BY Partner.Name";

            return UnitOfWork.DbContext.Database.SqlQuery<ChildByQuestionReportDTO>(query,
                                                            new SqlParameter("initialDate", initialDate),
                                                            new SqlParameter("lastDate", lastDate),
                                                            new SqlParameter("partnerID", partnerID),
                                                            new SqlParameter("questionID", questionID),
                                                            new SqlParameter("scoreTypeID", scoreTypeID)).ToList();
        }


        /*
         * #############################################################
         * ############## AgreggatedChildByQuestionReport ##############
         * #############################################################
         */

        public List<AgreggatedChildByQuestionReportDTO> getAgreggatedChildByQuestionReport(DateTime initialDate, DateTime lastDate, int partnerID, int questionID, int scoreTypeID)
        {

            String query = @"SELECT
	                            prov.Name As Province
	                            ,dist.Name As District
	                            ,s.SiteName As SiteName
	                            ,childbyquestionobj.Partner
	                            ,childbyquestionobj.Domain
	                            ,childbyquestionobj.QuestionDescription
	                            ,childbyquestionobj.Score
	                            ,childbyquestionobj.MaleTotal
	                            ,childbyquestionobj.FemaleTotal
	                            ,childbyquestionobj.Total
                            FROM
	                             [Partner] part
	                            inner join  [Site] s on (part.siteID = s.SiteID)
	                            inner join  [OrgUnit] dist on (dist.OrgUnitID = s.OrgUnitID)
	                            inner join  [OrgUnit] prov on (dist.ParentOrgUnitId = prov.OrgUnitID)
                            INNER JOIN 
                            (
		                            SELECT
			                            childbyquestionobj.Partner
			                            ,childbyquestionobj.Domain
			                            ,childbyquestionobj.QuestionDescription
			                            ,childbyquestionobj.Score
			                            ,SUM(childbyquestionobj.MaleTotal) AS MaleTotal
			                            ,SUM(childbyquestionobj.femaleTotal) AS FemaleTotal
			                            ,SUM(childbyquestionobj.MaleTotal + FemaleTotal) AS Total
		                            FROM
		                            (
				                            SELECT 
					                            Partner.Name AS Partner
					                            ,Domain.Description AS Domain
					                            ,Question.Description AS QuestionDescription
					                            ,ScoreType.Score AS Score
					                            ,MaleTotal = CASE WHEN Child.Gender = 'M' THEN 1 ELSE 0 END
					                            ,femaleTotal = CASE WHEN Child.Gender = 'F' THEN 1 ELSE 0 END
				                            FROM Child 
					                            INNER JOIN HouseHold ON HouseHold.HouseHoldID = CHild.HouseholdID
					                            INNER JOIN Partner ON HouseHold.PartnerID = [Partner].PartnerID
					                            INNER JOIN CSI ON Child.ChildID = CSI.ChildID 
					                            INNER JOIN CSIDomain ON CSI.CSIID = CSIDomain.CSIID
					                            INNER JOIN Domain ON Domain.DomainID = CSIDomain.DomainID
					                            INNER JOIN CSIDomainScore ON CSIDomain.CSIDomainID = CSIDomainScore.CSIDomainID
					                            INNER JOIN Question ON CSIDomainScore.QuestionID = Question.QuestionID
					                            INNER JOIN Answer ON CSIDomainScore.AnswerID = Answer.AnswerID
					                            INNER JOIN ScoreType ON ScoreType.ScoreTypeID = Answer.ScoreID
				                            WHERE CSI.IndexDate >= @initialDate AND CSI.IndexDate <= @lastDate 
				                            AND (Partner.PartnerID = @partnerID or @partnerID = 0)
				                            AND Question.QuestionID = @questionID
				                            AND ScoreType.ScoreTypeID = @scoreTypeID
		                            )childbyquestionobj
		                            GROUP BY 
			                            childbyquestionobj.Partner
			                            ,childbyquestionobj.Domain
			                            ,childbyquestionobj.QuestionDescription
			                            ,childbyquestionobj.Score
		                            --ORDER BY childbyquestionobj.Partner
                            )childbyquestionobj
                            ON childbyquestionobj.Partner = part.Name 
                            GROUP BY
	                            prov.Name
	                            ,dist.Name
	                            ,s.SiteName
	                            ,childbyquestionobj.Partner
	                            ,childbyquestionobj.Domain
	                            ,childbyquestionobj.QuestionDescription
	                            ,childbyquestionobj.Score
	                            ,childbyquestionobj.MaleTotal
	                            ,childbyquestionobj.FemaleTotal
	                            ,childbyquestionobj.Total";

            return UnitOfWork.DbContext.Database.SqlQuery<AgreggatedChildByQuestionReportDTO>(query,
                                                            new SqlParameter("initialDate", initialDate),
                                                            new SqlParameter("lastDate", lastDate),
                                                            new SqlParameter("partnerID", partnerID),
                                                            new SqlParameter("questionID", questionID),
                                                            new SqlParameter("scoreTypeID", scoreTypeID)).ToList();
        }



        /*
         * #######################################################
         * ############## Last Child Status ##############
         * #######################################################
         */

        public List<LastChildStatusReportDTO> getLastChildStatusReport(DateTime initialDate, DateTime lastDate, int partnerID, int childID, int childStatusID)
        {

            String query = @"SELECT
	                            childstatus.PartnerName AS Partner
	                            --,childstatus.ChildID
	                            ,childstatus.FullName
	                            ,childstatus.Gender
	                            ,childstatus.Age
	                            ,childstatus.ChildStatusDate
	                            ,childstatus.Description
                            FROM
                            (
	                            SELECT	
		                            p.PartnerID As PartnerID
		                            ,p.Name AS PartnerName
		                            , (csh.[ChildID]) AS ChildID
		                            , (FirstName) + ' ' + (LastName) As FullName
		                            , c.Gender AS Gender
		                            , DATEDIFF(YEAR, c.DateOFBirth, GETDATE()) AS Age
		                            ,CONVERT(varchar,csh.EffectiveDate,103) AS ChildStatusDate
		                            ,row_number() over(partition by csh.[ChildID] order by csh.[EffectiveDate] desc) as numeroLinha
		                            ,csh.[ChildStatusID] AS ChildStatusID
		                            ,ct.Description
	                            FROM 
		                             [ChildStatusHistory] AS csh
	                            INNER JOIN 
		                             [Child] AS c ON c.[ChildID] = csh.[ChildID]
	                            INNER JOIN
		                             [HouseHold] AS hh ON hh.HouseHoldID = c.HouseholdID
	                            INNER JOIN
		                             [Partner] AS p ON hh.PartnerID = p.PartnerID
	                            INNER JOIN
		                             [ChildStatus] AS ct ON csh.[ChildStatusID] = ct.[StatusID]
	                            WHERE
		                            csh.EffectiveDate >= @initialDate AND csh.EffectiveDate <= @lastDate 
	                            AND (p.PartnerID = @partnerID or @partnerID = 0)
	                            AND (c.ChildID = @childID or @childID =0)
	                            AND (csh.ChildStatusID = @childStatusID or @childStatusID = 0)
                            ) childstatus
                            WHERE
	                            childstatus.numeroLinha = 1
                            ORDER BY childstatus.PartnerName";

            return UnitOfWork.DbContext.Database.SqlQuery<LastChildStatusReportDTO>(query,
                                                            new SqlParameter("initialDate", initialDate),
                                                            new SqlParameter("lastDate", lastDate),
                                                            new SqlParameter("partnerID", partnerID),
                                                            new SqlParameter("childID", childID),
                                                            new SqlParameter("childStatusID", childStatusID)).ToList();
        }


        /*
         * #######################################################
         * ############## Agreggated Last Child Status ###########
         * #######################################################
         */

        public List<AgreggatedLastChildStatusReportDTO> getAgreggatedLastChildStatusReport(int ProvID, int DistID, int SiteID, DateTime initialDate, DateTime lastDate, int partnerID, int childID, int childStatusID)
        {

            String query = @"SELECT
	                            prov.Name As Province
	                            ,dist.Name As District
	                            ,s.SiteName As SiteName
	                            ,lastchildobj.Partner
	                            ,lastchildobj.Description
	                            ,SUM(lastchildobj.MaleTotal) AS MaleTotal
	                            ,SUM(lastchildobj.femaleTotal) AS FemaleTotal
	                            ,SUM(lastchildobj.MaleTotal + FemaleTotal) AS Total
                            FROM
	                             [Partner] part
	                            inner join  [Site] s on (part.siteID = s.SiteID)
	                            inner join  [OrgUnit] dist on (dist.OrgUnitID = s.OrgUnitID)
	                            inner join  [OrgUnit] prov on (dist.ParentOrgUnitId = prov.OrgUnitID)
                            INNER JOIN 
                            (
	                            SELECT
		                            childstatus.PartnerName AS Partner
		                            --,childstatus.ChildID
		                            ,childstatus.FullName
		                            ,MaleTotal = CASE WHEN childstatus.Gender = 'M' THEN 1 ELSE 0 END
		                            ,femaleTotal = CASE WHEN childstatus.Gender = 'F' THEN 1 ELSE 0 END
		                            --,childstatus.Gender
		                            ,childstatus.Age
		                            ,childstatus.ChildStatusDate
		                            ,childstatus.Description
	                            FROM
	                            (
		                            SELECT	
			                            p.PartnerID As PartnerID
			                            ,p.Name AS PartnerName
			                            , (csh.[ChildID]) AS ChildID
			                            , (FirstName) + ' ' + (LastName) As FullName
			                            , c.Gender AS Gender
			                            , DATEDIFF(YEAR, c.DateOFBirth, GETDATE()) AS Age
			                            ,CONVERT(varchar,csh.EffectiveDate,103) AS ChildStatusDate
			                            ,row_number() over(partition by csh.[ChildID] order by csh.[EffectiveDate] desc) as numeroLinha
			                            ,csh.[ChildStatusID] AS ChildStatusID
			                            ,ct.Description
		                            FROM 
			                             [ChildStatusHistory] AS csh
		                            INNER JOIN 
			                             [Child] AS c ON c.[ChildID] = csh.[ChildID]
		                            INNER JOIN
			                             [HouseHold] AS hh ON hh.HouseHoldID = c.HouseholdID
		                            INNER JOIN
			                             [Partner] AS p ON hh.PartnerID = p.PartnerID
		                            INNER JOIN
			                             [ChildStatus] AS ct ON csh.[ChildStatusID] = ct.[StatusID]
		                            WHERE
			                            csh.EffectiveDate >= @initialDate AND csh.EffectiveDate <= @lastDate 
		                            AND (p.PartnerID = @partnerID or @partnerID = 0)
		                            AND (c.ChildID = @childID or @childID =0)
		                            AND (csh.ChildStatusID = @childStatusID or @childStatusID = 0)
	                            ) childstatus
	                            WHERE
		                            childstatus.numeroLinha = 1
                            )lastchildobj
                            ON lastchildobj.Partner = part.Name
                            WHERE
	                            ((s.SiteID = @SiteID AND prov.OrgUnitID = @ProvID AND dist.OrgUnitID = @DistID) OR
	                            (s.SiteID = @SiteID AND @ProvID = 0 AND @DistID = 0) OR
	                            (@SiteID = 0 AND dist.OrgUnitID = @DistID AND @ProvID = 0) OR 
	                            (@SiteID = 0 AND @DistID = 0 AND prov.OrgUnitID = @ProvID) OR
	                            (@SiteID = 0 AND @DistID = 0 AND @ProvID = 0))
                            GROUP BY 
	                            prov.Name
	                            ,dist.Name
	                            ,s.SiteName
	                            ,lastchildobj.Partner
	                            ,lastchildobj.ChildStatusDate
	                            ,lastchildobj.Description
                            ORDER BY 
	                            prov.Name
	                            ,dist.Name
	                            ,s.SiteName
	                            ,lastchildobj.Partner";

            return UnitOfWork.DbContext.Database.SqlQuery<AgreggatedLastChildStatusReportDTO>(query,
                                                            new SqlParameter("ProvID", ProvID),
                                                            new SqlParameter("DistID", DistID),
                                                            new SqlParameter("SiteID", SiteID),
                                                            new SqlParameter("initialDate", initialDate),
                                                            new SqlParameter("lastDate", lastDate),
                                                            new SqlParameter("partnerID", partnerID),
                                                            new SqlParameter("childID", childID),
                                                            new SqlParameter("childStatusID", childStatusID)).ToList();
        }


        /*
         * ############################################################
         * ############## Before and Actual Child Status ##############
         * ############################################################
         */

        public List<BeforeAndActualChildStatusReportDTO> getBeforeAndActualChildStatusReport(DateTime initialDate, DateTime lastDate)
        {
            String query = @"SELECT
	                            estagioAnterior.PartnerName AS Partner
	                            ,estagioAnterior.FullName
	                            ,estagioAnterior.Gender
	                            ,estagioAnterior.Age
	                            ,estagioAnterior.Description AS BeforeActualChildStatus
	                            ,estagioAnterior.ChildStatusDate AS BeforeActualChildStatusDate
	                            ,estagioActual.Description AS ActualChildStatus
	                            ,estagioActual.ChildStatusDate AS ActualChildStatusDate
                            FROM
                            (
		                            SELECT
				                            p.PartnerID As PartnerID, p.Name AS PartnerName
				                            ,(csh.[ChildID]) AS ChildID, (FirstName) + ' ' + (LastName) As FullName
				                            ,c.Gender AS Gender, DATEDIFF(YEAR, c.DateOFBirth, GETDATE()) AS Age
				                            ,csh.[EffectiveDate] AS ChildStatusDate
				                            ,row_number() over(partition by csh.[ChildID] order by csh.[EffectiveDate] DESC) as LastChildStatusDate
				                            ,csh.[ChildStatusID] AS ChildStatusID ,ct.Description
		                            FROM  [ChildStatusHistory] AS csh
		                            INNER JOIN  [Child] AS c ON c.[ChildID] = csh.[ChildID]
		                            INNER JOIN  [HouseHold] AS hh ON hh.HouseHoldID = c.HouseholdID
		                            INNER JOIN  [Partner] AS p ON hh.PartnerID = p.PartnerID
		                            INNER JOIN [ChildStatus] AS ct ON csh.[ChildStatusID] = ct.[StatusID]
		                            WHERE  c.[ChildID] IN
		                            (SELECT [ChildStatusHistory].[ChildID]
		                            FROM  [ChildStatusHistory]
		                            GROUP BY [ChildStatusHistory].[ChildID]
		                            HAVING count([ChildStatusHistory].[ChildID]) > 1)
                            ) estagioAnterior
                            LEFT JOIN
                            (
		                            SELECT	--csh.[ChildStatusHistoryID] AS ChildStatusID
				                            p.PartnerID As PartnerID,p.Name AS PartnerName
				                            , (csh.[ChildID]) AS ChildID, (FirstName) + ' ' + (LastName) As FullName
				                            , c.Gender AS Gender, DATEDIFF(YEAR, c.DateOFBirth, GETDATE()) AS Age
				                            ,csh.[EffectiveDate] AS ChildStatusDate
				                            ,row_number() over(partition by csh.[ChildID] order by csh.[EffectiveDate] DESC) as LastChildStatusDate
				                            ,csh.[ChildStatusID] AS ChildStatusID
				                            ,ct.Description
		                            FROM  [ChildStatusHistory] AS csh
		                            INNER JOIN  [Child] AS c ON c.[ChildID] = csh.[ChildID]
		                            INNER JOIN  [HouseHold] AS hh ON hh.HouseHoldID = c.HouseholdID
		                            INNER JOIN  [Partner] AS p ON hh.PartnerID = p.PartnerID
		                            INNER JOIN  [ChildStatus] AS ct ON csh.[ChildStatusID] = ct.[StatusID]
		                            WHERE  c.[ChildID] IN
		                            (SELECT [ChildStatusHistory].[ChildID]
		                            FROM  [ChildStatusHistory]
		                            GROUP BY [ChildStatusHistory].[ChildID]
		                            HAVING count([ChildStatusHistory].[ChildID]) > 1)
                            ) estagioActual
                            ON (estagioAnterior.FullName=estagioActual.FullName)
                            WHERE
                            estagioActual.LastChildStatusDate = 1 AND estagioAnterior.LastChildStatusDate = 2 AND
                            estagioActual.ChildStatusDate >= @initialDate AND estagioActual.ChildStatusDate <= @lastDate";

            return UnitOfWork.DbContext.Database.SqlQuery<BeforeAndActualChildStatusReportDTO>(query,
                                                            new SqlParameter("initialDate", initialDate),
                                                            new SqlParameter("lastDate", lastDate)).ToList();
        }


        /*
         * #################################################################
         * ############## Agreggated Before and Actual Child Status ########
         * ################################################################
         */

        public List<AgreggatedBeforeAndActualChildStatusReportDTO> getAgreggatedBeforeAndActualChildStatusReport(int ProvID, int DistID, int SiteID, DateTime initialDate, DateTime lastDate, int PartnerID, int BeforeStatusID, int ActualStatusID)
        {
            String query = @"SELECT
	                            prov.Name As Province
	                            ,dist.Name As District
	                            ,s.SiteName As SiteName
	                            ,agreggatedinitiallastobj.Partner
	                            ,agreggatedinitiallastobj.BeforeActualChildStatus
	                            ,agreggatedinitiallastobj.ActualChildStatus
	                            ,agreggatedinitiallastobj.MaleTotal
	                            ,agreggatedinitiallastobj.FemaleTotal
	                            ,agreggatedinitiallastobj.Total
                            FROM
	                             [Partner] part
	                            inner join  [Site] s on (part.siteID = s.SiteID)
	                            inner join  [OrgUnit] dist on (dist.OrgUnitID = s.OrgUnitID)
	                            inner join  [OrgUnit] prov on (dist.ParentOrgUnitId = prov.OrgUnitID)
                            INNER JOIN 
                            (
		                            SELECT
			                            initiallastobj.PartnerID 
			                            ,initiallastobj.Partner
			                            ,initiallastobj.FullName
			                            ,initiallastobj.BeforeActualChildStatus
			                            ,initiallastobj.ActualChildStatus
			                            ,SUM(initiallastobj.MaleTotal) AS MaleTotal
			                            ,SUM(initiallastobj.femaleTotal) AS FemaleTotal
			                            ,SUM(initiallastobj.MaleTotal + FemaleTotal) AS Total
			
		                            FROM
		                            (
			                            SELECT
				                            estagioAnterior.PartnerID
				                            ,estagioAnterior.PartnerName AS Partner
				                            ,estagioAnterior.FullName
				                            ,estagioAnterior.Age
				                            ,estagioAnterior.Description AS BeforeActualChildStatus
				                            ,estagioActual.Description AS ActualChildStatus
				                            ,MaleTotal = CASE WHEN estagioAnterior.Gender = 'M' THEN 1 ELSE 0 END
				                            ,femaleTotal = CASE WHEN estagioAnterior.Gender = 'F' THEN 1 ELSE 0 END
				
			                            FROM
			                            (
					                            SELECT
							                            p.PartnerID As PartnerID, p.Name AS PartnerName
							                            ,(csh.[ChildID]) AS ChildID, (FirstName) + ' ' + (LastName) As FullName
							                            ,c.Gender AS Gender, DATEDIFF(YEAR, c.DateOFBirth, GETDATE()) AS Age
							                            ,csh.[EffectiveDate] AS ChildStatusDate
							                            ,row_number() over(partition by csh.[ChildID] order by csh.[EffectiveDate] DESC) as LastChildStatusDate
							                            ,csh.[ChildStatusID] AS ChildStatusID ,ct.Description
					                            FROM  [ChildStatusHistory] AS csh
					                            INNER JOIN  [Child] AS c ON c.[ChildID] = csh.[ChildID]
					                            INNER JOIN  [HouseHold] AS hh ON hh.HouseHoldID = c.HouseholdID
					                            INNER JOIN  [Partner] AS p ON hh.PartnerID = p.PartnerID
					                            INNER JOIN [ChildStatus] AS ct ON csh.[ChildStatusID] = ct.[StatusID]
					                            WHERE  c.[ChildID] IN
					                            (SELECT [ChildStatusHistory].[ChildID]
					                            FROM  [ChildStatusHistory]
					                            GROUP BY [ChildStatusHistory].[ChildID]
					                            HAVING count([ChildStatusHistory].[ChildID]) > 1)
			                            ) estagioAnterior
			                            LEFT JOIN
			                            (
					                            SELECT	--csh.[ChildStatusHistoryID] AS ChildStatusID
							                            p.PartnerID As PartnerID,p.Name AS PartnerName
							                            , (csh.[ChildID]) AS ChildID, (FirstName) + ' ' + (LastName) As FullName
							                            , c.Gender AS Gender, DATEDIFF(YEAR, c.DateOFBirth, GETDATE()) AS Age
							                            ,csh.[EffectiveDate] AS ChildStatusDate
							                            ,row_number() over(partition by csh.[ChildID] order by csh.[EffectiveDate] DESC) as LastChildStatusDate
							                            ,csh.[ChildStatusID] AS ChildStatusID
							                            ,ct.Description
					                            FROM  [ChildStatusHistory] AS csh
					                            INNER JOIN  [Child] AS c ON c.[ChildID] = csh.[ChildID]
					                            INNER JOIN  [HouseHold] AS hh ON hh.HouseHoldID = c.HouseholdID
					                            INNER JOIN  [Partner] AS p ON hh.PartnerID = p.PartnerID
					                            INNER JOIN  [ChildStatus] AS ct ON csh.[ChildStatusID] = ct.[StatusID]
					                            WHERE  c.[ChildID] IN
					                            (SELECT [ChildStatusHistory].[ChildID]
					                            FROM  [ChildStatusHistory]
					                            GROUP BY [ChildStatusHistory].[ChildID]
					                            HAVING count([ChildStatusHistory].[ChildID]) > 1)
			                            ) estagioActual
			                            ON (estagioAnterior.FullName=estagioActual.FullName)
			                            WHERE
			                            estagioActual.LastChildStatusDate = 1 AND estagioAnterior.LastChildStatusDate = 2 
			                            AND
			                            estagioActual.ChildStatusDate >= @initialDate AND estagioActual.ChildStatusDate <= @lastDate 
			                            AND
			                            (estagioAnterior.ChildStatusID = @BeforeStatusID OR @BeforeStatusID = 0)
			                            AND
			                            (estagioActual.ChildStatusID = @ActualStatusID OR @ActualStatusID = 0)	
			
	                            )initiallastobj
	                            GROUP BY 
		                            initiallastobj.PartnerID
		                            ,initiallastobj.Partner
		                            ,initiallastobj.FullName
		                            ,initiallastobj.BeforeActualChildStatus
		                            ,initiallastobj.ActualChildStatus

                            )agreggatedinitiallastobj
                            ON agreggatedinitiallastobj.Partner = part.Name
                            WHERE  
	                            ((s.SiteID = @SiteID AND @ProvID = 0 AND @DistID = 0) OR
	                            (@SiteID = 0 AND dist.OrgUnitID = @DistID AND @ProvID = 0) OR 
	                            (@SiteID = 0 AND @DistID = 0 AND prov.OrgUnitID = @ProvID) OR
	                            (@SiteID = 0 AND @DistID = 0 AND @ProvID = 0))
                            AND (agreggatedinitiallastobj.PartnerID = @PartnerID OR @PartnerID = 0)
                            GROUP BY 
	                            prov.Name
	                            ,dist.Name
	                            ,s.SiteName
	                            ,agreggatedinitiallastobj.PartnerID
	                            ,agreggatedinitiallastobj.Partner
	                            ,agreggatedinitiallastobj.MaleTotal
	                            ,agreggatedinitiallastobj.FemaleTotal
	                            ,agreggatedinitiallastobj.Total
	                            ,agreggatedinitiallastobj.BeforeActualChildStatus
	                            ,agreggatedinitiallastobj.ActualChildStatus
                            ORDER BY 
	                            prov.Name
	                            ,dist.Name
	                            ,s.SiteName
	                            ,agreggatedinitiallastobj.Partner";

            return UnitOfWork.DbContext.Database.SqlQuery<AgreggatedBeforeAndActualChildStatusReportDTO>(query,
                                                            new SqlParameter("ProvID", ProvID),
                                                            new SqlParameter("DistID", DistID),
                                                            new SqlParameter("SiteID", SiteID),
                                                            new SqlParameter("initialDate", initialDate),
                                                            new SqlParameter("lastDate", lastDate),
                                                            new SqlParameter("PartnerID", PartnerID),
                                                            new SqlParameter("BeforeStatusID", BeforeStatusID),
                                                            new SqlParameter("ActualStatusID", ActualStatusID)).ToList();
        }


        /*
         * ############################################################
         * ############## Initial and Actual Child Status #############
         * ############################################################
         */

        public List<InitialAndActualChildStatusReportDTO> getInitialAndActualChildStatusReport(DateTime initialDate, DateTime lastDate, int PartnerID, int ChildStatusID)
        {
            String query = @"SELECT
                               distinct primeiroEstagio.PartnerName
                               ,primeiroEstagio.FullName
                               ,primeiroEstagio.Gender
                               ,primeiroEstagio.Age
                               ,primeiroEstagio.Description AS InitialChildStatus
                               ,primeiroEstagio.ChildStatusDate AS InitialActualChildStatusDate
                               ,estagioActual.Description AS ActualChildStatus
                               ,estagioActual.ChildStatusDate AS ActualChildStatusDate
                            FROM
                            (
                                   SELECT
                                           p.PartnerID As PartnerID, p.Name AS PartnerName
                                           ,(csh.[ChildID]) AS ChildID, (FirstName) + ' ' + (LastName) As FullName
                                           ,c.Gender AS Gender, DATEDIFF(YEAR, c.DateOFBirth, GETDATE()) AS Age
                                           ,csh.[EffectiveDate] AS ChildStatusDate
                                           ,row_number() over(partition by csh.[ChildID] order by csh.[EffectiveDate] ASC) as InitialChildStatusDate
                                           ,csh.[ChildStatusID] AS ChildStatusID ,ct.Description
                                   FROM  [ChildStatusHistory] AS csh
                                   LEFT JOIN  [Child] AS c ON c.[ChildID] = csh.[ChildID]
                                   LEFT JOIN  [HouseHold] AS hh ON hh.HouseHoldID = c.HouseholdID
                                   LEFT JOIN  [Partner] AS p ON hh.PartnerID = p.PartnerID
                                   LEFT JOIN [ChildStatus] AS ct ON csh.[ChildStatusID] = ct.[StatusID]
                                   WHERE  c.[ChildID] IN
                                   (SELECT [ChildStatusHistory].[ChildID]
                                   FROM  [ChildStatusHistory]
                                   GROUP BY [ChildStatusHistory].[ChildID]
                                   HAVING count([ChildStatusHistory].[ChildID]) > 1)
                            ) primeiroEstagio
                            LEFT JOIN
                            (
                                   SELECT	--csh.[ChildStatusHistoryID] AS ChildStatusID
                                           p.PartnerID As PartnerID,p.Name AS PartnerName
                                           , (csh.[ChildID]) AS ChildID, (FirstName) + ' ' + (LastName) As FullName
                                           , c.Gender AS Gender, DATEDIFF(YEAR, c.DateOFBirth, GETDATE()) AS Age
                                           ,csh.[EffectiveDate] AS ChildStatusDate
                                           ,row_number() over(partition by csh.[ChildID] order by csh.[EffectiveDate] DESC) as LastChildStatusDate
                                           ,csh.[ChildStatusID] AS ChildStatusID
                                           ,ct.Description
                                   FROM  [ChildStatusHistory] AS csh
                                   LEFT JOIN  [Child] AS c ON c.[ChildID] = csh.[ChildID]
                                   LEFT JOIN  [HouseHold] AS hh ON hh.HouseHoldID = c.HouseholdID
                                   LEFT JOIN  [Partner] AS p ON hh.PartnerID = p.PartnerID
                                   LEFT JOIN  [ChildStatus] AS ct ON csh.[ChildStatusID] = ct.[StatusID]
                                   WHERE  c.[ChildID] IN
                                   (SELECT [ChildStatusHistory].[ChildID]
                                   FROM  [ChildStatusHistory]
                                   GROUP BY [ChildStatusHistory].[ChildID]
                                   HAVING count([ChildStatusHistory].[ChildID]) > 1)
                            ) estagioActual
                            ON (primeiroEstagio.FullName=estagioActual.FullName)
                            WHERE
                            primeiroEstagio.InitialChildStatusDate = 1 AND estagioActual.LastChildStatusDate = 1
                            AND
                            estagioActual.ChildStatusDate >= @initialDate AND estagioActual.ChildStatusDate <= @lastDate";// + And;

            return UnitOfWork.DbContext.Database.SqlQuery<InitialAndActualChildStatusReportDTO>(query,
                                                            new SqlParameter("initialDate", initialDate),
                                                            new SqlParameter("lastDate", lastDate)).ToList();
        }


        // ===========================================================================================

        
        public List<CarePlanChildDTO> getCarePlanChildReport(DateTime initialDate, DateTime finalDate, int partnerID, Boolean maintenanceCandidates)
        {
            String queryCarePlanChild = @"SELECT
							p.Name As Partner,
							c.FirstName As FirstName,
							c.LastName As LastName,
							cp.CarePlanDate										
							FROM  [Partner] p 
							inner join  [HouseHold] hh on (p.PartnerID = hh.PartnerID)
							inner join  [Child] c on (hh.HouseHoldID = c.HouseholdID)
							inner join  [CSI] csi on (csi.ChildID = c.ChildID)
							inner join  [CarePlan] cp on (cp.CSIID = csi.CSIID)
							inner join  [ChildStatusHistory] csh on (csh.ChildID = c.ChildID)
							inner join  [ChildStatus] cs on (cs.StatusID = csh.ChildStatusID)
							Where p.CollaboratorRoleID = 1 
							and DATEDIFF(DAY, CAST(cp.CarePlanDate As Date), GETDATE()) > 75
							and cs.Description in ('Manutenção','Inicial')
							and (@partnerID = 0 or p.partnerID = @partnerID)
                            and ((@initialDate is not NULL and @finalDate is not NULL) and (cp.CarePlanDate Between @initialDate AND @finalDate))
                            or ((@initialDate is not NULL and @finalDate is NULL) and (@initialDate <= cp.CarePlanDate))
                            or ((@initialDate is NULL and @finalDate is not NULL) and (@finalDate >= cp.CarePlanDate))
                            or (@initialDate is NULL and @finalDate is NULL)";

            String queryMaintenanceCandidates = @"SELECT
							obj.Partner, obj.FirstName,	obj.LastName, obj.CarePlanDate
							--,obj.Saude, obj.Alimentacao, obj.Educacao, obj.Proteccao, obj.Apoio, obj.Fortalecimento, obj.TaskCompleted
							FROM
							(
								SELECT
								p.Name As Partner, c.FirstName As FirstName, c.LastName As LastName, cp.CarePlanDate,
								Saude = CASE WHEN d.Description like 'Saúde' and (q.Description not like '2-%' and q.Description not like '5-%' and q.Description not like '6-%') and st.Score = 3 THEN 1 ELSE 0 END,
								Alimentacao = CASE WHEN d.Description like 'Alimentacão/Nutrição' and st.Score = 3 THEN 1 ELSE 0 END,
								Educacao = CASE WHEN d.Description like 'Educação' and (q.Description not like '10-%' and q.Description not like '12-%' ) and st.Score = 3 THEN 1 ELSE 0 END,
								Proteccao = CASE WHEN d.Description like 'Protecção e Apoio Legal' and st.Score = 3 THEN 1 ELSE 0 END,
								--Habitacao = CASE WHEN d.Description like 'Habitação' and (q.Description not like '22-%') and st.Score > 1 THEN 1 ELSE 0 END,
								Apoio = CASE WHEN d.Description like 'Apoio Psico social' and st.Score = 3 THEN 1 ELSE 0 END,
								Fortalecimento = CASE WHEN d.Description like 'Fortalecimento Económico' and st.Score = 3 THEN 1 ELSE 0 END,
								tsk.Completed As TaskCompleted
								--d.Description As Domain_Description, q.Description As Question_Description, st.Score
								FROM  [Partner] p 
								inner join  [HouseHold] hh on (p.PartnerID = hh.PartnerID)
								inner join  [Child] c on (hh.HouseHoldID = c.HouseholdID)
								inner join  [CSI] csi on (csi.ChildID = c.ChildID)
								inner join  [ChildStatusHistory] csh on (csh.ChildID = c.ChildID)
								inner join  [ChildStatus] cs on (cs.StatusID = csh.ChildStatusID)
								left join  [CarePlan] cp on (cp.CSIID = csi.CSIID)
								inner join  [CarePlanDomain] cpd on (cpd.CarePlanID = cp.CarePlanID)
								inner join  [CarePlanDomainSupportService] cpdss on (cpdss.CarePlanDomainID = cpd.CarePlanDomainID)
								inner join  [Tasks] tsk on (tsk.CarePlanDomainSupportServiceID = cpdss.CarePlanDomainSupportServiceID)
								left join  [CSIDomain] csiDom on (csiDom.CSIID = csi.CSIID)
								inner join  [CSIDomainScore] csiDomScr on (csiDomScr.CSIDomainID = csiDom.CSIDomainID)
								inner join  [Domain] d on (csiDom.DomainID = d.DomainID)
								inner join  [Question] q on (q.QuestionID = csiDomScr.QuestionID)
								inner join  [Answer] a on (a.AnswerID = csiDomScr.AnswerID)
								inner join  [ScoreType] st on (st.ScoreTypeID = a.ScoreID)
								Where p.CollaboratorRoleID = 1 
								and DATEDIFF(DAY, CAST(cp.CarePlanDate As Date), GETDATE()) > 75
							    and cs.Description in ('Manutenção','Inicial')
							    and @partnerID = 0 or p.partnerID = @partnerID
                                and ((@initialDate is not NULL and @finalDate is not NULL) and (cp.CarePlanDate Between @initialDate AND @finalDate))
                                or ((@initialDate is not NULL and @finalDate is NULL) and (@initialDate <= cp.CarePlanDate))
                                or ((@initialDate is NULL and @finalDate is not NULL) and (@finalDate >= cp.CarePlanDate))
                                or (@initialDate is NULL and @finalDate is NULL)
							) obj
							WHERE
								obj.Saude + obj.Alimentacao + obj.Educacao  + obj.Proteccao + obj.Apoio + obj.Fortalecimento = 6 or obj.TaskCompleted = 1
							Group by
								obj.Partner, obj.FirstName, obj.LastName, obj.CarePlanDate
								--,obj.Saude, obj.Alimentacao, obj.Educacao, obj.Proteccao, obj.Apoio, obj.Fortalecimento, obj.TaskCompleted";

            return UnitOfWork.DbContext.Database.SqlQuery<CarePlanChildDTO>((maintenanceCandidates ? queryMaintenanceCandidates : queryCarePlanChild),
                                                                            new SqlParameter("initialDate", initialDate),
                                                                            new SqlParameter("finalDate", finalDate),
                                                                            new SqlParameter("partnerID", partnerID)).ToList();
        }

        public List<CarePlanChildDTO> getAgreggatedCarePlanChildReport(DateTime initialDate, DateTime finalDate, int partnerID, Boolean maintenanceCandidates)
        {
            String queryCarePlanChild = @"SELECT
							p.Name As Partner, Count(*)	As ChildCount								
							FROM  [Partner] p 
							inner join  [HouseHold] hh on (p.PartnerID = hh.PartnerID)
							inner join  [Child] c on (hh.HouseHoldID = c.HouseholdID)
							inner join  [CSI] csi on (csi.ChildID = c.ChildID)
							inner join  [CarePlan] cp on (cp.CSIID = csi.CSIID)
							inner join  [ChildStatusHistory] csh on (csh.ChildID = c.ChildID)
							inner join  [ChildStatus] cs on (cs.StatusID = csh.ChildStatusID)
							Where p.CollaboratorRoleID = 1 
							and DATEDIFF(DAY, CAST(cp.CarePlanDate As Date), GETDATE()) > 75
							and cs.Description in ('Manutenção','Inicial')
							and (@partnerID = 0 or p.partnerID = @partnerID)
                            and ((@initialDate is not NULL and @finalDate is not NULL) and (cp.CarePlanDate Between @initialDate AND @finalDate))
                            or ((@initialDate is not NULL and @finalDate is NULL) and (@initialDate <= cp.CarePlanDate))
                            or ((@initialDate is NULL and @finalDate is not NULL) and (@finalDate >= cp.CarePlanDate))
                            or (@initialDate is NULL and @finalDate is NULL) group by p.Name";

            String queryMaintenanceCandidates = @"SELECT 
                            obj.Partner, Count(*) As ChildCount
							FROM
							(
								SELECT
								p.Name As Partner, c.FirstName As FirstName, c.LastName As LastName, cp.CarePlanDate,
								Saude = CASE WHEN d.Description like 'Saúde' and (q.Description not like '2-%' and q.Description not like '5-%' and q.Description not like '6-%') and st.Score = 3 THEN 1 ELSE 0 END,
								Alimentacao = CASE WHEN d.Description like 'Alimentacão/Nutrição' and st.Score = 3 THEN 1 ELSE 0 END,
								Educacao = CASE WHEN d.Description like 'Educação' and (q.Description not like '10-%' and q.Description not like '12-%' ) and st.Score = 3 THEN 1 ELSE 0 END,
								Proteccao = CASE WHEN d.Description like 'Protecção e Apoio Legal' and st.Score = 3 THEN 1 ELSE 0 END,
								--Habitacao = CASE WHEN d.Description like 'Habitação' and (q.Description not like '22-%') and st.Score > 1 THEN 1 ELSE 0 END,
								Apoio = CASE WHEN d.Description like 'Apoio Psico social' and st.Score = 3 THEN 1 ELSE 0 END,
								Fortalecimento = CASE WHEN d.Description like 'Fortalecimento Económico' and st.Score = 3 THEN 1 ELSE 0 END,
								tsk.Completed As TaskCompleted
								--d.Description As Domain_Description, q.Description As Question_Description, st.Score
								FROM  [Partner] p 
								inner join  [HouseHold] hh on (p.PartnerID = hh.PartnerID)
								inner join  [Child] c on (hh.HouseHoldID = c.HouseholdID)
								inner join  [CSI] csi on (csi.ChildID = c.ChildID)
								inner join  [ChildStatusHistory] csh on (csh.ChildID = c.ChildID)
								inner join  [ChildStatus] cs on (cs.StatusID = csh.ChildStatusID)
								left join  [CarePlan] cp on (cp.CSIID = csi.CSIID)
								inner join  [CarePlanDomain] cpd on (cpd.CarePlanID = cp.CarePlanID)
								inner join  [CarePlanDomainSupportService] cpdss on (cpdss.CarePlanDomainID = cpd.CarePlanDomainID)
								inner join  [Tasks] tsk on (tsk.CarePlanDomainSupportServiceID = cpdss.CarePlanDomainSupportServiceID)
								left join  [CSIDomain] csiDom on (csiDom.CSIID = csi.CSIID)
								inner join  [CSIDomainScore] csiDomScr on (csiDomScr.CSIDomainID = csiDom.CSIDomainID)
								inner join  [Domain] d on (csiDom.DomainID = d.DomainID)
								inner join  [Question] q on (q.QuestionID = csiDomScr.QuestionID)
								inner join  [Answer] a on (a.AnswerID = csiDomScr.AnswerID)
								inner join  [ScoreType] st on (st.ScoreTypeID = a.ScoreID)
								Where p.CollaboratorRoleID = 1 
								and DATEDIFF(DAY, CAST(cp.CarePlanDate As Date), GETDATE()) > 75
							    and cs.Description in ('Manutenção','Inicial')
							    and @partnerID = 0 or p.partnerID = @partnerID
                                and ((@initialDate is not NULL and @finalDate is not NULL) and (cp.CarePlanDate Between @initialDate AND @finalDate))
                                or ((@initialDate is not NULL and @finalDate is NULL) and (@initialDate <= cp.CarePlanDate))
                                or ((@initialDate is NULL and @finalDate is not NULL) and (@finalDate >= cp.CarePlanDate))
                                or (@initialDate is NULL and @finalDate is NULL)
							) obj
							WHERE
								obj.Saude + obj.Alimentacao + obj.Educacao  + obj.Proteccao + obj.Apoio + obj.Fortalecimento = 6 or obj.TaskCompleted = 1
							Group by
								obj.Partner";
            return UnitOfWork.DbContext.Database.SqlQuery<CarePlanChildDTO>((maintenanceCandidates ? queryMaintenanceCandidates : queryCarePlanChild),
                                                                            new SqlParameter("initialDate", initialDate),
                                                                            new SqlParameter("finalDate", finalDate),
                                                                            new SqlParameter("partnerID", partnerID)).ToList();
        }

        public List<RoutineVisitChildDTO> getAgreggatedChildrenWithouAssistence(int ProvID, int DistID, int SiteID, DateTime initialDate, DateTime finalDate)
        {
            String query = @"SELECT
	                            agreggatedinfo.Province, agreggatedinfo.District, agreggatedinfo.SiteName, 
	                            SUM(agreggatedinfo.Male) As TotalMale, SUM(agreggatedinfo.Female) As TotalFemale, 
	                            SUM(agreggatedinfo.Female)+SUM(agreggatedinfo.Male) As TotalCount
                            FROM
                            (
	                            SELECT
		                            Province, District, SiteName, Person, 
		                            Male = CASE WHEN ServicesTotal = 0 AND Gender = 'M' THEN 1 ELSE 0 END,
		                            Female = CASE WHEN ServicesTotal = 0 AND Gender = 'F' THEN 1 ELSE 0 END
	                            FROM
	                            (
		                            SELECT
			                            prov.Name As Province, dist.Name As District, s.SiteName, c.FirstName+c.LastName As Person, c.Gender, 0 As ServicesTotal
		                            FROM  [Partner] p
			                            inner join  [Site] s on (p.siteID = s.SiteID)
			                            inner join  [OrgUnit] dist on (dist.OrgUnitID = s.OrgUnitID)
			                            inner join  [OrgUnit] prov on (dist.ParentOrgUnitId = prov.OrgUnitID)
			                            inner join  [HouseHold] hh on (p.PartnerID = hh.PartnerID)
			                            inner join  [RoutineVisit] rv on (hh.HouseHoldID = rv.HouseholdID)
			                            inner join  [RoutineVisitMember] rvm on (rv.RoutineVisitID = rvm.RoutineVisitID)
			                            inner join  [RoutineVisitSupport] rvs on (rvm.RoutineVisitMemberID = rvs.RoutineVisitMemberID)
			                            inner join  [Child] c on (c.ChildID = rvm.ChildID)
			                            inner join  [ChildStatusHistory] csh on (c.ChildID = csh.ChildID and csh.ChildStatusID != 6)
		                            WHERE p.CollaboratorRoleID = 1
			                            -- Criança que não teve nova ficha de visita entre o periodo seleccionado
                                        AND ((@initialDate is not NULL and @finalDate is not NULL) and (rv.RoutineVisitDate NOT Between @initialDate AND @finalDate))
                                        OR ((@initialDate is not NULL and @finalDate is NULL) and (@initialDate >= rv.RoutineVisitDate))
                                        OR ((@initialDate is NULL and @finalDate is not NULL) and (@finalDate <= rv.RoutineVisitDate))
                                        OR (@initialDate is NULL and @finalDate is NULL)
                                        AND ((s.SiteID = @SiteID AND @ProvID = 0 AND @DistID = 0) OR (@SiteID = 0 AND dist.OrgUnitID = @DistID AND @ProvID = 0) OR 
			                            (@SiteID = 0 AND @DistID = 0 AND prov.OrgUnitID = @ProvID) OR (@SiteID = 0 AND @DistID = 0 AND @ProvID = 0)) 
		                            GROUP BY prov.Name, dist.Name, s.SiteName, c.FirstName+c.LastName, c.Gender
		                            UNION all
		                            SELECT
			                            prov.Name As Province, dist.Name As District, s.SiteName, c.FirstName+c.LastName As Person, c.Gender, SUM(CONVERT(INT, rvs.SupportValue)) As ServicesTotal
		                            FROM  [Partner] p
			                            inner join  [Site] s on (p.siteID = s.SiteID)
			                            inner join  [OrgUnit] dist on (dist.OrgUnitID = s.OrgUnitID)
			                            inner join  [OrgUnit] prov on (dist.ParentOrgUnitId = prov.OrgUnitID)
			                            inner join  [HouseHold] hh on (p.PartnerID = hh.PartnerID)
			                            inner join  [RoutineVisit] rv on (hh.HouseHoldID = rv.HouseholdID)
			                            inner join  [RoutineVisitMember] rvm on (rv.RoutineVisitID = rvm.RoutineVisitID)
			                            inner join  [RoutineVisitSupport] rvs on (rvm.RoutineVisitMemberID = rvs.RoutineVisitMemberID and rvs.SupportType not in ('SAVINGS_GROUP_MEMBER', 'COMMENT'))
			                            inner join  [Child] c on (c.ChildID = rvm.ChildID)
			                            inner join  [ChildStatusHistory] csh on (c.ChildID = csh.ChildID and csh.ChildStatusID != 6)
		                            WHERE p.CollaboratorRoleID = 1
			                            -- Crianças que tem ficha no periodo seleccionado, mas na ficha em analise, a Criança não teve nenhum serviço prestado
                                        AND ((@initialDate is not NULL and @finalDate is not NULL) and (rv.RoutineVisitDate Between @initialDate AND @finalDate))
                                        OR ((@initialDate is not NULL and @finalDate is NULL) and (@initialDate >= rv.RoutineVisitDate))
                                        OR ((@initialDate is NULL and @finalDate is not NULL) and (@finalDate <= rv.RoutineVisitDate))
                                        OR (@initialDate is NULL and @finalDate is NULL)
			                            AND ((s.SiteID = @SiteID AND @ProvID = 0 AND @DistID = 0) OR (@SiteID = 0 AND dist.OrgUnitID = @DistID AND @ProvID = 0) OR 
			                            (@SiteID = 0 AND @DistID = 0 AND prov.OrgUnitID = @ProvID) OR (@SiteID = 0 AND @DistID = 0 AND @ProvID = 0)) 
		                            GROUP BY prov.Name, dist.Name, s.SiteName, c.FirstName+c.LastName, c.Gender
	                            ) childinfo

	                            UNION ALL

	                            SELECT
		                            Province, District, SiteName, Person, 
		                            Male = CASE WHEN ServicesTotal = 0 AND Gender = 'M' THEN 1 ELSE 0 END,
		                            Female = CASE WHEN ServicesTotal = 0 AND Gender = 'F' THEN 1 ELSE 0 END
	                            FROM
	                            (
		                            SELECT
			                            prov.Name As Province, dist.Name As District, s.SiteName, a.FirstName+a.LastName As Person, a.Gender, 0 As ServicesTotal
		                            FROM  [Partner] p
			                            inner join  [Site] s on (p.siteID = s.SiteID)
			                            inner join  [OrgUnit] dist on (dist.OrgUnitID = s.OrgUnitID)
			                            inner join  [OrgUnit] prov on (dist.ParentOrgUnitId = prov.OrgUnitID)
			                            inner join  [HouseHold] hh on (p.PartnerID = hh.PartnerID)
			                            inner join  [RoutineVisit] rv on (hh.HouseHoldID = rv.HouseholdID)
			                            inner join  [RoutineVisitMember] rvm on (rv.RoutineVisitID = rvm.RoutineVisitID)
			                            inner join  [RoutineVisitSupport] rvs on (rvm.RoutineVisitMemberID = rvs.RoutineVisitMemberID)
			                            inner join  [Adult] a on (a.AdultID = rvm.AdultID)
			                            inner join  [ChildStatusHistory] csh on (a.AdultID = csh.ChildID and csh.ChildStatusID != 6)
		                            WHERE p.CollaboratorRoleID = 1
			                            -- Criança que não teve nova ficha de visita entre o periodo seleccionado
                                        AND ((@initialDate is not NULL and @finalDate is not NULL) and (rv.RoutineVisitDate NOT Between @initialDate AND @finalDate))
                                        OR ((@initialDate is not NULL and @finalDate is NULL) and (@initialDate >= rv.RoutineVisitDate))
                                        OR ((@initialDate is NULL and @finalDate is not NULL) and (@finalDate <= rv.RoutineVisitDate))
                                        OR (@initialDate is NULL and @finalDate is NULL)
                                        AND ((s.SiteID = @SiteID AND @ProvID = 0 AND @DistID = 0) OR (@SiteID = 0 AND dist.OrgUnitID = @DistID AND @ProvID = 0) OR 
			                            (@SiteID = 0 AND @DistID = 0 AND prov.OrgUnitID = @ProvID) OR (@SiteID = 0 AND @DistID = 0 AND @ProvID = 0)) 
		                            GROUP BY prov.Name, dist.Name, s.SiteName, a.FirstName+a.LastName, a.Gender
		                            UNION all
		                            SELECT
			                            prov.Name As Province, dist.Name As District, s.SiteName, a.FirstName+a.LastName As Person, a.Gender, SUM(CONVERT(INT, rvs.SupportValue)) As ServicesTotal
		                            FROM  [Partner] p
			                            inner join  [Site] s on (p.siteID = s.SiteID)
			                            inner join  [OrgUnit] dist on (dist.OrgUnitID = s.OrgUnitID)
			                            inner join  [OrgUnit] prov on (dist.ParentOrgUnitId = prov.OrgUnitID)
			                            inner join  [HouseHold] hh on (p.PartnerID = hh.PartnerID)
			                            inner join  [RoutineVisit] rv on (hh.HouseHoldID = rv.HouseholdID)
			                            inner join  [RoutineVisitMember] rvm on (rv.RoutineVisitID = rvm.RoutineVisitID)
			                            inner join  [RoutineVisitSupport] rvs on (rvm.RoutineVisitMemberID = rvs.RoutineVisitMemberID and rvs.SupportType not in ('SAVINGS_GROUP_MEMBER', 'COMMENT'))
			                            inner join  [Adult] a on (a.AdultID = rvm.AdultID)
			                            inner join  [ChildStatusHistory] csh on (a.AdultID = csh.ChildID and csh.ChildStatusID != 6)
		                            WHERE p.CollaboratorRoleID = 1
			                            -- Crianças que tem ficha no periodo seleccionado, mas na ficha em analise, a Criança não teve nenhum serviço prestado
                                        AND ((@initialDate is not NULL and @finalDate is not NULL) and (rv.RoutineVisitDate Between @initialDate AND @finalDate))
                                        OR ((@initialDate is not NULL and @finalDate is NULL) and (@initialDate >= rv.RoutineVisitDate))
                                        OR ((@initialDate is NULL and @finalDate is not NULL) and (@finalDate <= rv.RoutineVisitDate))
                                        OR (@initialDate is NULL and @finalDate is NULL)
			                            AND ((s.SiteID = @SiteID AND @ProvID = 0 AND @DistID = 0) OR (@SiteID = 0 AND dist.OrgUnitID = @DistID AND @ProvID = 0) OR 
			                            (@SiteID = 0 AND @DistID = 0 AND prov.OrgUnitID = @ProvID) OR (@SiteID = 0 AND @DistID = 0 AND @ProvID = 0)) 
		                            GROUP BY prov.Name, dist.Name, s.SiteName, a.FirstName+a.LastName, a.Gender
	                            ) adultinfo
	                            GROUP BY Province, District, SiteName, Person, Gender, ServicesTotal
                            ) agreggatedinfo
                            GROUP BY agreggatedinfo.Province, agreggatedinfo.District, agreggatedinfo.SiteName";
            return UnitOfWork.DbContext.Database.SqlQuery<RoutineVisitChildDTO>(query,
                                                new SqlParameter("ProvID", ProvID),
                                                new SqlParameter("DistID", DistID),
                                                new SqlParameter("SiteID", SiteID),
                                                new SqlParameter("initialDate", initialDate),
                                                new SqlParameter("finalDate", finalDate)).ToList();
        }

    }
}