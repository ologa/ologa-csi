using EFDataAccess.UOW;
using System;
using System.Collections.Generic;
using System.Linq;
using VPPS.CSI.Domain;
using System.Data;
using System.Data.Entity;
using Utilities;
using EFDataAccess.DTO;
using System.Data.SqlClient;
using System.Collections;

namespace EFDataAccess.Services
{
    public class PartnerService : BaseService
    {
        SiteService SiteService;

        public PartnerService(UnitOfWork uow) : base(uow)
        {
            SiteService = new SiteService(uow);
        }

        private EFDataAccess.Repository.IRepository<Partner> PartnerRepository
        {
            get { return UnitOfWork.Repository<Partner>(); }
        }

        public void SaveOrUpdate(Partner Partner)
        {
            if (Partner.PartnerID > 0) { PartnerRepository.Update(Partner); }
            else { PartnerRepository.Add(Partner); }
        }

        public Partner Reload(Partner entity)
        {
            PartnerRepository.FullReload(entity);
            return entity;
        }

        public void Delete(Partner Partner) => PartnerRepository.Delete(Partner);

        public List<Partner> findAllActive(Site site)
        {
            if (site == null)
            {
                return UnitOfWork.Repository<Partner>().GetAll().Where(x => x.Active == true).ToList();
            }
            else
            {
                return findBySite(site).Where(x=> x.Active == true).ToList();
            }
        }

        public Partner findByID(int ID) => UnitOfWork.Repository<Partner>().GetById(ID);

        public Partner findByGuid(Guid guid) => UnitOfWork.Repository<Partner>().Get().Where(x => x.partner_guid == guid).SingleOrDefault();

        public Partner findBySyncGuid(Guid SyncGuid) => UnitOfWork.Repository<Partner>().GetAll().Where(x => x.SyncGuid == SyncGuid).Include(x => x.Superior).Include(x => x.site).SingleOrDefault();

        public Site findSiteBySiteName(string SiteName) => UnitOfWork.Repository<Site>().Get().Where(x => x.SiteName == SiteName).SingleOrDefault();

        public Site findUserBySyncGuid(string SiteName) => UnitOfWork.Repository<Site>().Get().Where(x => x.SiteName == SiteName).SingleOrDefault();

        public List<UniqueEntity> findAllPartnerUniqueEntities() => UnitOfWork.DbContext.Database.SqlQuery<UniqueEntity>("Select ISNULL(SyncGuid, cast(cast(0 as binary) as uniqueidentifier)) As SyncGuid, PartnerID As ID from Partner").ToList();

        public List<Partner> findBySite(Site site) => UnitOfWork.Repository<Partner>().GetAll().Where(x => (site.SiteID == 0 || x.siteID == site.SiteID) && x.CollaboratorRoleID == 1).ToList();

        public List<Partner> findAllPartners() => UnitOfWork.Repository<Partner>().GetAll().ToList();


        public int ImportData(string path)
        {
            _logger.Information("IMPORTACAO DE ACTIVISTAS ...");

            UnitOfWork.DbContext.Configuration.AutoDetectChangesEnabled = false;

            int PartnersCount = 0;
            FileImporter imp = new FileImporter();
            string fullPath = path + @"\Partners.csv";
            DataTable dt2 = imp.ImportFromCSV(fullPath);

            UsersDB = ConvertListToHashtableUsers(findAllUsersUniqueEntities());
            Hashtable PartnersDB = ConvertListToHashtableUsers(findAllPartnerUniqueEntities());
            foreach (DataRow row in dt2.Rows)
            {
                Guid Partner_guid = new Guid(row["partner_guid"].ToString());
                int PartnerID = ParseStringToIntSafe(PartnersDB[Partner_guid]);
                Partner Partner = (PartnerID > 0) ? findByID(PartnerID) : new Partner();
                Partner.Code = row.Table.Columns.Contains("Code") ? row["Code"].ToString() : "";
                Partner.Name = row["Name"].ToString();
                Partner.Address = row["Name"].ToString();
                Partner.ContactNo = row["ContactNo"].ToString();
                Partner.FaxNo = row["FaxNo"].ToString();
                Partner.ContactName = row["ContactName"].ToString();
                Partner Superior = findBySyncGuid(new Guid(row["SuperiorGuid"].ToString()));
                Partner.SuperiorId = (Superior != null) ? Superior.PartnerID : Partner.SuperiorId;
                Partner.CollaboratorRoleID = (row["CollaboratorRoleID"].ToString().Length != 0) ? int.Parse(row["CollaboratorRoleID"].ToString()) : Partner.CollaboratorRoleID;
                Partner.ActivationDate = (row["ActivationDate"].ToString()).Length == 0 ? Partner.ActivationDate : DateTime.Parse(row["ActivationDate"].ToString());
                Partner.InactivationDate = (row["InactivationDate"].ToString()).Length == 0 ? Partner.InactivationDate : DateTime.Parse(row["InactivationDate"].ToString());
                Partner.Active = row["ActiveInt"].ToString().Equals("1") ? true : false;
                Partner.site = SiteService.findSiteBySyncGuid(new Guid(row["SiteGuid"].ToString()));
                SetCreationDataFields(Partner, row, Partner_guid);
                SetUpdatedDataFields(Partner, row);
                SaveOrUpdate(Partner);
                // O Commit é feito aqui, pois existem Partners que se relaccionam entre si
                UnitOfWork.Commit();
                PartnersCount++;
            }

            Rename(fullPath, fullPath + IMPORTED);
            return PartnersCount;
        }

        public List<PartnerReportOneDTO> findPartnersAndBeneficiaries()
        {
            String query = @"SELECT
		                    p.[Name] As Nome_do_Activista,
		                    b.FirstName As Nome_do_beneficiario,
		                    b.LastName As Apelido_do_Beneficiario,
		                    CASE WHEN b.[Type] = 'adult' THEN 'Adulto' ELSE 'Criança' END As Tipo_de_beneficiario,
							b.BeneficiaryState As Estado_Actual,
                            b.[BeneficiaryStateEffectiveDate]
							FROM  [Partner] p
		                    join  [HouseHold] hh on (p.PartnerID = hh.PartnerID)
		                    join  [vw_beneficiary_details] b on (b.HouseholdID = hh.HouseHoldID)
							WHERE
							((b.[Type] = 'child' and b.BeneficiaryState != 'Adulto') or b.[Type] = 'adult')
							order by p.[Name], b.FirstName, b.LastName";
            return UnitOfWork.DbContext.Database.SqlQuery<PartnerReportOneDTO>(query).ToList();
        }

        public List<PartnerReportTwoDTO> findChiefPartnersHouseholdNumbersAgreggatedByPartners()
        {
            String query = @"SELECT o.ChiefPartner, o.ChiefPartnerRole, o.Partner, o.PartnerRole, COUNT(*) As Children
                            FROM (
	                            SELECT
		                            cp.Name As ChiefPartner, 
		                            ChiefPartnerRole = CASE cp.CollaboratorRoleID WHEN 3 THEN 'Supervior' WHEN 2 THEN 'Activista Chefe' ELSE 'Activista' END,
		                            p.[Name] As [Partner], 
		                            PartnerRole = CASE p.CollaboratorRoleID WHEN 3 THEN 'Supervior' WHEN 2 THEN 'Activista Chefe' ELSE 'Activista' END,
		                            c.FirstName As ChildFirstName,
		                            c.LastName As ChildLastName,
		                            csh.ChildStatusHistoryID
	                            FROM  [Partner] p
	                            inner join  [Partner] cp on (p.SuperiorId = cp.PartnerID) 
	                            inner join  [HouseHold] hh on (p.PartnerID = hh.PartnerID)
	                            inner join  [Child] c on (hh.HouseHoldID = c.HouseholdID)
	                            inner join  ChildStatusHistory csh on (csh.ChildID = c.ChildID)
	                            inner join  ChildStatus cs on (csh.ChildStatusID = cs.StatusID) 
	                            Where p.CollaboratorRoleID = 1 
	                            and c.ChildID IN
	                            (
		                            SELECT
			                            obj.[ChildID]
		                            FROM
		                            (
			                            SELECT 
				                            row_number() OVER (PARTITION BY ChildID ORDER BY ChildStatusHistoryID DESC) AS numeroLinha
				                            --Obter o número da linha de acordo com ChildID, e ordenado pelo ID do Histórico de forma DESCENDENTE(Último ao Primeiro)
				                            ,[ChildStatusID]
				                            ,[ChildID]
			                            FROM 
				                             [ChildStatusHistory] csh
			                            --WHERE csh.[EffectiveDate]>= @initialDate AND csh.[EffectiveDate] <= @lastDate
		                            )
		                            obj
		                            WHERE 
			                            obj.numeroLinha=1 AND ChildStatusID in (1,2) --EM QUE O ESTÁGIO SEJE INICIAL OU MANUNTENÇÃO
	                            )
	                            group by cp.Name, cp.CollaboratorRoleID, p.Name, p.CollaboratorRoleID, c.FirstName, c.LastName, csh.ChildStatusHistoryID
                            ) o
                            group by o.ChiefPartner, o.ChiefPartnerRole, o.Partner, o.PartnerRole";

            return UnitOfWork.DbContext.Database.SqlQuery<PartnerReportTwoDTO>(query).ToList();
        }

        public List<PartnerReportThreeDTO> findChiefPartnersHouseholdNumbers()
        {
            String query = @"SELECT o.ChiefPartner, o.ChiefPartnerRole, COUNT(*) As Children
                            FROM (
	                            SELECT
		                            cp.Name As ChiefPartner, 
		                            ChiefPartnerRole = CASE cp.CollaboratorRoleID WHEN 3 THEN 'Supervior' WHEN 2 THEN 'Activista Chefe' ELSE 'Activista' END,
		                            p.[Name] As [Partner], 
		                            PartnerRole = CASE p.CollaboratorRoleID WHEN 3 THEN 'Supervior' WHEN 2 THEN 'Activista Chefe' ELSE 'Activista' END,
		                            c.FirstName As ChildFirstName,
		                            c.LastName As ChildLastName,
		                            csh.ChildStatusHistoryID
	                            FROM  [Partner] p
	                            left join  [Partner] cp on (p.SuperiorId = cp.PartnerID) 
	                            left join  [HouseHold] hh on (p.PartnerID = hh.PartnerID)
	                            left join  [Child] c on (hh.HouseHoldID = c.HouseholdID)
	                            left join  ChildStatusHistory csh on (csh.ChildID = c.ChildID)
	                            left join  ChildStatus cs on (csh.ChildStatusID = cs.StatusID) 
	                            Where p.CollaboratorRoleID = 1 
	                            and c.ChildID IN
	                            (
		                            SELECT
			                            obj.[ChildID]
		                            FROM
		                            (
			                            SELECT 
				                            row_number() OVER (PARTITION BY ChildID ORDER BY ChildStatusHistoryID DESC) AS numeroLinha
				                            --Obter o número da linha de acordo com ChildID, e ordenado pelo ID do Histórico de forma DESCENDENTE(Último ao Primeiro)
				                            ,[ChildStatusID]
				                            ,[ChildID]
			                            FROM 
				                             [ChildStatusHistory] csh
			                            --WHERE csh.[EffectiveDate]>= @initialDate AND csh.[EffectiveDate] <= @lastDate
		                            )
		                            obj
		                            WHERE 
			                            obj.numeroLinha=1 AND ChildStatusID in (1,2) --EM QUE O ESTÁGIO SEJE INICIAL OU MANUNTENÇÃO
	                            )
	                            group by cp.Name, cp.CollaboratorRoleID, p.Name, p.CollaboratorRoleID, c.FirstName, c.LastName, csh.ChildStatusHistoryID
                            ) o
                            group by o.ChiefPartner, o.ChiefPartnerRole";

            return UnitOfWork.DbContext.Database.SqlQuery<PartnerReportThreeDTO>(query).ToList();
        }

        public List<PartnerReportFourDTO> FindPartnersWithHouseholdAndBenificiariesNumbersByGender(DateTime initialDate, DateTime lastDate)
        {
            String query = @"SELECT
	                                cp.Name AS Partner
	                                ,p.Name AS SimplePartner
	                                ,ISNULL(householdobj.HouseholdsPerPartner, 0) As HouseholdsPerPartner
	                                ,ISNULL(childobj.MaleChild, 0) AS MaleChild
	                                ,ISNULL(childobj.FemaleChild, 0) As FemaleChild
	                                ,ISNULL(adultobj.MaleAdult, 0) As MaleAdult
	                                ,ISNULL(adultobj.FemaleAdult, 0) As FemaleAdult
	                                ,ISNULL(beneficiaryobj.Total,0) AS Total
	                                ,ISNULL(beneficiarystatusobj.MaleInitialState, 0) As MaleInitialState
	                                ,ISNULL(beneficiarystatusobj.FemaleInitialState, 0) As FemaleInitialState
	                                ,ISNULL(beneficiarystatusobj.MaleGraduationState, 0) As MaleGraduationState
	                                ,ISNULL(beneficiarystatusobj.FemaleGraduationState, 0) As FemaleGraduationState
	                                ,ISNULL(beneficiarystatusobj.MaleTransferState, 0) As MaleTransferState
	                                ,ISNULL(beneficiarystatusobj.FemaleTransferState, 0) As FemaleTransferState
	                                ,ISNULL(beneficiarystatusobj.MaleQuittingState, 0) As MaleQuittingState
	                                ,ISNULL(beneficiarystatusobj.FemaleQuittingState, 0) As FemaleQuittingState
	                                ,ISNULL(beneficiarystatusobj.MaleLostState, 0) As MaleLostState
	                                ,ISNULL(beneficiarystatusobj.FemaleLostState, 0) As FemaleLostState
	                                ,ISNULL(beneficiarystatusobj.MaleDeathState, 0) As MaleDeathState
	                                ,ISNULL(beneficiarystatusobj.FemaleDeathState, 0) As FemaleDeathState
	                                ,ISNULL(beneficiarystatusobj.MaleOtherState, 0) As MaleOtherState
	                                ,ISNULL(beneficiarystatusobj.FemaleOtherState, 0) As FemaleOtherState
                                FROM  [Partner] p
                                INNER JOIN  [Partner] cp ON cp.PartnerID = p.SuperiorID
                                LEFT JOIN 
                                (
	                                SELECT	
		                                p.Name AS SimplePartnerName
		                                ,COUNT(DISTINCT hh.HouseHoldID) AS HouseholdsPerPartner
	                                FROM 
		                                 [Partner] AS cp
	                                INNER JOIN  [Partner] AS p ON cp.PartnerID = p.SuperiorID 
	                                INNER JOIN  [HouseHold] AS hh ON hh.PartnerID = p.PartnerID
	                                INNER JOIN   [vw_beneficiary_details] ben on (ben.HouseholdID = hh.HouseHoldID)
	                                WHERE p.CollaboratorRoleID = 1 AND 
	                                ((ben.[Type] = 'child' and ben.BeneficiaryState != 'adulto') or ben.[Type] = 'adult')
	                                AND 
	                                ben.RegistrationDate between @initialDate and @lastDate
	                                GROUP BY p.Name
                                ) householdobj
                                ON (p.Name = householdobj.SimplePartnerName)
                                LEFT JOIN 
                                (
	                                SELECT 
		                                p.Name AS SimplePartnerName
		                                ,SUM(CASE WHEN ben.Gender = 'M' then 1 else 0 END) AS MaleChild 
		                                ,SUM(CASE WHEN ben.Gender = 'F' then 1 else 0 END) AS FemaleChild
	                                FROM 
		                                 [Partner] AS cp
	                                INNER JOIN  [Partner] AS p ON cp.PartnerID = p.SuperiorID 
	                                INNER JOIN  [HouseHold] AS hh ON hh.PartnerID = p.PartnerID
	                                INNER JOIN  [vw_beneficiary_details] ben ON ben.HouseholdID = hh.HouseholdID AND type='child'
	                                INNER JOIN  [ChildStatusHistory] csh 
	                                ON (csh.ChildStatusHistoryID = (SELECT max(csh2.ChildStatusHistoryID) FROM  [ChildStatusHistory] csh2
	                                WHERE csh2.EffectiveDate<= @lastDate AND csh2.BeneficiaryGuid = ben.Guid))
	                                INNER JOIN  [ChildStatus] ct ON (csh.childStatusID = ct.StatusID AND ct.Description = 'Inicial')
	                                WHERE
	                                p.CollaboratorRoleID = 1 
	                                AND ben.RegistrationDate between @initialDate and @lastDate
	                                GROUP BY p.Name
                                ) childobj
                                ON (p.Name = childobj.SimplePartnerName)
                                LEFT JOIN 
                                (
	                                SELECT
		                                p.Name AS SimplePartnerName
		                                ,SUM(CASE WHEN ben.Gender = 'M' then 1 else 0 END) AS MaleAdult
		                                ,SUM(CASE WHEN ben.Gender = 'F' then 1 else 0 END) AS FemaleAdult
	                                FROM 
		                                 [Partner] AS cp
	                                INNER JOIN  [Partner] AS p ON cp.PartnerID = p.SuperiorID 
	                                INNER JOIN  [HouseHold] AS hh ON hh.PartnerID = p.PartnerID
	                                INNER JOIN  [vw_beneficiary_details] ben ON ben.HouseHoldID = hh.HouseHoldID AND ben.type='adult'
	                                INNER JOIN  [ChildStatusHistory] csh 
	                                ON (csh.ChildStatusHistoryID = (SELECT max(csh2.ChildStatusHistoryID) FROM  [ChildStatusHistory] csh2
	                                WHERE csh2.EffectiveDate<= @lastDate AND csh2.BeneficiaryGuid = ben.Guid))
	                                INNER JOIN  [ChildStatus] ct ON (csh.childStatusID = ct.StatusID AND ct.Description in ('Inicial'))
	                                WHERE p.CollaboratorRoleID = 1 
	                                AND ben.RegistrationDate between @initialDate and @lastDate
	                                GROUP BY p.Name
                                ) adultobj
                                ON (p.Name = adultobj.SimplePartnerName)
                                LEFT JOIN 
                                (
	                                SELECT
		                                p.Name AS SimplePartnerName
		                                ,COUNT(ben.ID) AS Total
	                                FROM 
		                                 [Partner] AS cp
	                                INNER JOIN  [Partner] AS p ON cp.PartnerID = p.SuperiorID 
	                                INNER JOIN  [HouseHold] AS hh ON hh.PartnerID = p.PartnerID
	                                INNER JOIN  [vw_beneficiary_details] ben ON ben.HouseHoldID = hh.HouseHoldID
	                                INNER JOIN  [ChildStatusHistory] csh 
	                                ON (csh.ChildStatusHistoryID = (SELECT max(csh2.ChildStatusHistoryID) FROM  [ChildStatusHistory] csh2
	                                WHERE csh2.EffectiveDate<= @lastDate AND csh2.BeneficiaryGuid = ben.Guid))
	                                INNER JOIN  [ChildStatus] ct ON (csh.childStatusID = ct.StatusID AND ct.Description in ('Inicial'))
	                                WHERE p.CollaboratorRoleID = 1 
	                                AND ben.RegistrationDate between @initialDate and @lastDate
	                                GROUP BY p.Name
                                ) beneficiaryobj
                                ON (p.Name = beneficiaryobj.SimplePartnerName)
                                LEFT JOIN
                                (
	                                SELECT	
			                                p.Name AS SimplePartnerName
			                                ,SUM(CASE WHEN ct.Description='Inicial' and ben.Gender = 'M' then 1 else 0 END) AS MaleInitialState
			                                ,SUM(CASE WHEN ct.Description='Inicial' and ben.Gender = 'F' then 1 else 0 END) AS FemaleInitialState
			                                ,SUM(CASE WHEN ct.Description='Graduação' and ben.Gender = 'M' then 1 else 0 END) AS MaleGraduationState
			                                ,SUM(CASE WHEN ct.Description='Graduação'  and ben.Gender = 'F' then 1 else 0 END) AS FemaleGraduationState
			                                ,SUM(CASE WHEN ct.Description in ('Transferido p/ programas NÃO de PEPFAR)', 'Transferido p/ programas de PEPFAR') and ben.Gender = 'M' then 1 else 0 END) AS MaleTransferState
			                                ,SUM(CASE WHEN ct.Description in ('Transferido p/ programas NÃO de PEPFAR)', 'Transferido p/ programas de PEPFAR') and ben.Gender = 'F' then 1 else 0 END) AS FemaleTransferState
			                                ,SUM(CASE WHEN ct.Description='Desistência' and ben.Gender = 'M' then 1 else 0 END) AS MaleQuittingState
			                                ,SUM(CASE WHEN ct.Description='Desistência' and ben.Gender = 'F' then 1 else 0 END) AS FemaleQuittingState
			                                ,SUM(CASE WHEN ct.Description='Perdido' and ben.Gender = 'M' then 1 else 0 END) AS MaleLostState
			                                ,SUM(CASE WHEN ct.Description='Perdido' and ben.Gender = 'F' then 1 else 0 END) AS FemaleLostState
			                                ,SUM(CASE WHEN ct.Description='Óbito' and ben.Gender = 'M' then 1 else 0 END) AS MaleDeathState
			                                ,SUM(CASE WHEN ct.Description='Óbito' and ben.Gender = 'F' then 1 else 0 END) AS FemaleDeathState
			                                ,SUM(CASE WHEN ct.Description='Outras Saídas' and ben.Gender = 'M' then 1 else 0 END) AS MaleOtherState
			                                ,SUM(CASE WHEN ct.Description='Outras Saídas' and ben.Gender = 'F' then 1 else 0 END) AS FemaleOtherState
		                                FROM 
			                                 [Partner] AS cp
		                                INNER JOIN  [Partner] AS p ON cp.PartnerID = p.SuperiorID 
		                                INNER JOIN  [HouseHold] AS hh ON hh.PartnerID = p.PartnerID
		                                INNER JOIN  [vw_beneficiary_details] ben ON ben.HouseHoldID = hh.HouseHoldID
		                                INNER JOIN  [ChildStatusHistory] csh 
		                                ON (csh.ChildStatusHistoryID = (SELECT max(csh2.ChildStatusHistoryID) FROM  [ChildStatusHistory] csh2
		                                WHERE csh2.EffectiveDate<= @lastDate AND ben.Guid = csh2.BeneficiaryGuid))
		                                INNER JOIN  [ChildStatus] ct ON (csh.childStatusID = ct.StatusID)
		                                WHERE ((ben.[Type] = 'child' and ben.BeneficiaryState != 'adulto') or ben.[Type] = 'adult')
		                                AND p.CollaboratorRoleID = 1 
		                                AND ben.RegistrationDate between @initialDate and @lastDate
		                                GROUP BY p.Name
                                )beneficiarystatusobj
                                ON (p.Name = beneficiarystatusobj.SimplePartnerName) 
                                WHERE p.CollaboratorRoleID = 1
                                ORDER BY p.Name";

            return UnitOfWork.DbContext.Database.SqlQuery<PartnerReportFourDTO>(query,
                                                            new SqlParameter("initialDate", initialDate),
                                                            new SqlParameter("lastDate", lastDate)).ToList();
        }

        public List<PartnerReportFiveDTO> FindChiefPartnersWithPartnersAndBenificiariesNumbers(DateTime initialDate, DateTime lastDate)
        {
            String query = @"SELECT
	                            cp.Name AS Partner
	                            ,ISNULL(partnerobj.SimplePartnerTotal, 0) As SimplePartnerTotal
	                            ,ISNULL(householdobj.HouseholdsPerPartner, 0) As HouseholdsPerPartner
	                            ,ISNULL(childobj.TotalChild, 0) As TotalChild
	                            ,ISNULL(adultobj.TotalAdult, 0) As TotalAdult
	                            ,ISNULL(beneficiaryobj.Total,0) AS Total
	                            ,ISNULL(beneficiarystatusobj.InitialState, 0) As InitialState
	                            ,ISNULL(beneficiarystatusobj.GraduationState, 0) As GraduationState
	                            ,ISNULL(beneficiarystatusobj.TransferState, 0) As TransferState
	                            ,ISNULL(beneficiarystatusobj.QuittingState, 0) As QuittingState
	                            ,ISNULL(beneficiarystatusobj.LostState, 0) As LostState
	                            ,ISNULL(beneficiarystatusobj.DeathState, 0) As DeathState
	                            ,ISNULL(beneficiarystatusobj.OtherState, 0) As OtherState
                            FROM  [Partner] cp
                            INNER JOIN 
                            (
	                            SELECT
		                            cp.PartnerID AS ChiefPartnerID
		                            ,COUNT(p.partnerID) AS SimplePartnerTotal
	                            FROM 
                                     [Partner] AS cp
	                                INNER JOIN  [Partner] AS p ON cp.PartnerID = p.SuperiorID 
	                            WHERE p.CollaboratorRoleID = 1 
	                            GROUP BY cp.PartnerID
                            )partnerobj
                            ON (cp.PartnerID = partnerobj.ChiefPartnerID)
                            INNER JOIN 
                            (
	                            SELECT	
		                            cp.PartnerID AS ChiefPartnerID
		                            ,COUNT(distinct hh.HouseHoldID) AS HouseholdsPerPartner
	                            FROM 
                                     [Partner] AS cp
	                            INNER JOIN  [Partner] AS p ON cp.PartnerID = p.SuperiorID 
	                            INNER JOIN  [HouseHold] AS hh ON hh.PartnerID = p.PartnerID
	                            INNER JOIN  [vw_beneficiary_details] b on (b.HouseholdID = hh.HouseHoldID)
	                            WHERE p.CollaboratorRoleID = 1 AND 
	                            ((b.[Type] = 'child' and b.BeneficiaryState != 'adulto') or b.[Type] = 'adult')
	                            AND 
	                            hh.RegistrationDate between @initialDate and @lastDate
	                            GROUP BY cp.PartnerID
                            ) householdobj
                            ON (cp.PartnerID = householdobj.ChiefPartnerID)
                            LEFT JOIN 
                            (
	                            SELECT	
		                            cp.PartnerID AS ChiefPartnerID
		                            ,COUNT(ben.ID) AS TotalChild
	                            FROM 
                                     [Partner] AS cp
	                            INNER JOIN  [Partner] AS p ON cp.PartnerID = p.SuperiorID 
	                            INNER JOIN  [HouseHold] AS hh ON hh.PartnerID = p.PartnerID
	                            INNER JOIN  [vw_beneficiary_details] ben ON ben.HouseHoldID = hh.HouseHoldID AND ben.type='child'
	                            INNER JOIN  [ChildStatusHistory] csh 
	                            ON (csh.ChildStatusHistoryID = (SELECT max(csh2.ChildStatusHistoryID) FROM  [ChildStatusHistory] csh2
	                            WHERE csh2.EffectiveDate<= @lastDate AND csh2.BeneficiaryGuid = ben.Guid))
	                            INNER JOIN  [ChildStatus] ct ON (csh.childStatusID = ct.StatusID AND ct.Description in ('Inicial'))
	                            WHERE p.CollaboratorRoleID = 1 
	                            AND ben.RegistrationDate between @initialDate and @lastDate
	                            GROUP BY cp.PartnerID
                            ) childobj
                            ON (cp.PartnerID = childobj.ChiefPartnerID)
                            LEFT JOIN 
                            (
	                            SELECT	
		                            cp.PartnerID AS ChiefPartnerID
		                            ,COUNT(ben.ID) AS TotalAdult
	                            FROM 
                                     [Partner] AS cp
	                            INNER JOIN  [Partner] AS p ON cp.PartnerID = p.SuperiorID 
	                            INNER JOIN  [HouseHold] AS hh ON hh.PartnerID = p.PartnerID
	                            INNER JOIN  [vw_beneficiary_details] ben ON ben.HouseHoldID = hh.HouseHoldID AND ben.type='adult'
	                            INNER JOIN  [ChildStatusHistory] csh 
	                            ON (csh.ChildStatusHistoryID = (SELECT max(csh2.ChildStatusHistoryID) FROM  [ChildStatusHistory] csh2
	                            WHERE csh2.EffectiveDate<= @lastDate AND csh2.BeneficiaryGuid = ben.Guid))
	                            INNER JOIN  [ChildStatus] ct ON (csh.childStatusID = ct.StatusID AND ct.Description in ('Inicial'))
	                            WHERE p.CollaboratorRoleID = 1 
	                            AND ben.RegistrationDate between @initialDate and @lastDate
	                            GROUP BY cp.PartnerID
                            ) adultobj
                            ON (cp.PartnerID = adultobj.ChiefPartnerID)
                            LEFT JOIN 
                            (
	                            SELECT
		                            cp.PartnerID AS ChiefPartnerID
		                            ,COUNT(ben.ID) AS Total
	                            FROM 
		                             [Partner] AS cp
	                            INNER JOIN  [Partner] AS p ON cp.PartnerID = p.SuperiorID 
	                            INNER JOIN  [HouseHold] AS hh ON hh.PartnerID = p.PartnerID
	                            INNER JOIN  [vw_beneficiary_details] ben ON ben.HouseHoldID = hh.HouseHoldID
	                            INNER JOIN  [ChildStatusHistory] csh 
	                            ON (csh.ChildStatusHistoryID = (SELECT max(csh2.ChildStatusHistoryID) FROM  [ChildStatusHistory] csh2
	                            WHERE csh2.EffectiveDate<= @lastDate AND csh2.BeneficiaryGuid = ben.Guid))
	                            INNER JOIN  [ChildStatus] ct ON (csh.childStatusID = ct.StatusID AND ct.Description in ('Inicial'))
	                            WHERE p.CollaboratorRoleID = 1 
	                            AND ben.RegistrationDate between @initialDate and @lastDate
	                            GROUP BY cp.PartnerID
                            ) beneficiaryobj
                            ON (cp.PartnerID = beneficiaryobj.ChiefPartnerID)
                            LEFT JOIN
                            (
	                            SELECT	
			                            cp.PartnerID AS ChiefPartnerID
			                            ,SUM(CASE WHEN ct.Description='Inicial' then 1 else 0 END) AS InitialState
			                            ,SUM(CASE WHEN ct.Description='Graduação' then 1 else 0 END) AS GraduationState
			                            ,SUM(CASE WHEN ct.Description in ('Transferido p/ programas NÃO de PEPFAR)', 'Transferido p/ programas de PEPFAR') then 1 else 0 END) AS TransferState
			                            ,SUM(CASE WHEN ct.Description='Desistência' then 1 else 0 END) AS QuittingState
			                            ,SUM(CASE WHEN ct.Description='Perdido' then 1 else 0 END) AS LostState
			                            ,SUM(CASE WHEN ct.Description='Óbito' then 1 else 0 END) AS DeathState
			                            ,SUM(CASE WHEN ct.Description='Outras Saídas' then 1 else 0 END) AS OtherState
		                            FROM 
			                             [Partner] AS cp
		                            INNER JOIN  [Partner] AS p ON cp.PartnerID = p.SuperiorID 
		                            INNER JOIN  [HouseHold] AS hh ON hh.PartnerID = p.PartnerID
		                            INNER JOIN  [vw_beneficiary_details] ben ON ben.HouseHoldID = hh.HouseHoldID
		                            INNER JOIN  [ChildStatusHistory] csh 
		                            ON (csh.ChildStatusHistoryID = (SELECT max(csh2.ChildStatusHistoryID) FROM  [ChildStatusHistory] csh2
		                            WHERE csh2.EffectiveDate<= @lastDate AND csh2.BeneficiaryGuid = ben.Guid))
		                            INNER JOIN  [ChildStatus] ct ON (csh.childStatusID = ct.StatusID)
		                            WHERE ((ben.[Type] = 'child' and ben.BeneficiaryState != 'adulto') or ben.[Type] = 'adult')
		                            AND p.CollaboratorRoleID = 1 
		                            AND ben.RegistrationDate between @initialDate and @lastDate
		                            GROUP BY cp.PartnerID
                            )beneficiarystatusobj
                            ON (cp.PartnerID = beneficiarystatusobj.ChiefPartnerID) 
                            WHERE cp.CollaboratorRoleID = 2
                            ORDER BY cp.Name";

            return UnitOfWork.DbContext.Database.SqlQuery<PartnerReportFiveDTO>(query,
                                                            new SqlParameter("initialDate", initialDate),
                                                            new SqlParameter("lastDate", lastDate)).ToList();
        }
    }

    public class PartnerReportOneDTO
    {
        public string Nome_do_Activista { get; set; }
        //public string nome_do_beneficiario;
        //public string Nome_do_beneficiario { get { return StringUtils.MaskIfConfIsEnabled(nome_do_beneficiario); } set { nome_do_beneficiario = value; } }
        public string Nome_do_beneficiario { get; set; }
        //public string apelido_do_Beneficiario;
        //public string Apelido_do_Beneficiario { get { return StringUtils.MaskIfConfIsEnabled(apelido_do_Beneficiario); } set { apelido_do_Beneficiario = value; } }
        public string Apelido_do_Beneficiario { get; set; }
        public string Tipo_de_beneficiario { get; set; }

        public string Estado_Actual { get; set; }
    }

    public class PartnerReportTwoDTO
    {
        public string ChiefPartner { get; set; }
        public string ChiefPartnerRole { get; set; }
        public string Partner { get; set; }
        public string PartnerRole { get; set; }
        public int Children { get; set; }
    }

    public class PartnerReportThreeDTO
    {
        public string ChiefPartner { get; set; }
        public string ChiefPartnerRole { get; set; }
        public int Children { get; set; }
    }

    public class PartnerReportFourDTO : IPartnerNameReportDTO
    {
        public string Partner { get; set; }
        public string SimplePartner { get; set; }
        public int HouseholdsPerPartner { get; set; }
        public int MaleChild { get; set; }
        public int FemaleChild { get; set; }
        public int MaleAdult { get; set; }
        public int FemaleAdult { get; set; }
        public int Total { get; set; }
        public int MaleInitialState { get; set; }
        public int FemaleInitialState { get; set; }
        public int MaleGraduationState { get; set; }
        public int FemaleGraduationState { get; set; }
        public int MaleTransferState { get; set; }
        public int FemaleTransferState { get; set; }
        public int MaleQuittingState { get; set; }
        public int FemaleQuittingState { get; set; }
        public int MaleLostState { get; set; }
        public int FemaleLostState { get; set; }
        public int MaleDeathState { get; set; }
        public int FemaleDeathState { get; set; }
        public int MaleOtherState { get; set; }
        public int FemaleOtherState { get; set; }

        public List<Object> values { get; set; } = new List<Object>();

        public List<Object> populatedValues()
        {
            values.Add(Partner);
            values.Add(SimplePartner);
            values.Add(HouseholdsPerPartner);
            values.Add(MaleChild);
            values.Add(FemaleChild);
            values.Add(MaleAdult);
            values.Add(FemaleAdult);
            values.Add(Total);
            values.Add(MaleInitialState);
            values.Add(FemaleInitialState);
            values.Add(MaleGraduationState);
            values.Add(FemaleGraduationState);
            values.Add(MaleTransferState);
            values.Add(FemaleTransferState);
            values.Add(MaleQuittingState);
            values.Add(FemaleQuittingState);
            values.Add(MaleLostState);
            values.Add(FemaleLostState);
            values.Add(MaleDeathState);
            values.Add(FemaleDeathState);
            values.Add(MaleOtherState);
            values.Add(FemaleOtherState);
            return values;
        }
    }

    public class PartnerReportFiveDTO : IPartnerNameReportDTO
    {
        public string Partner { get; set; }
        public int SimplePartnerTotal { get; set; }
        public int HouseholdsPerPartner { get; set; }
        public int TotalChild { get; set; }
        public int TotalAdult { get; set; }
        public int Total { get; set; }
        public int InitialState { get; set; }
        public int GraduationState { get; set; }
        public int TransferState { get; set; }
        public int QuittingState { get; set; }
        public int LostState { get; set; }
        public int DeathState { get; set; }
        public int OtherState { get; set; }

        public List<Object> values { get; set; } = new List<Object>();

        public List<Object> populatedValues()
        {
            values.Add(Partner);
            values.Add(SimplePartnerTotal);
            values.Add(HouseholdsPerPartner);
            values.Add(TotalChild);
            values.Add(TotalAdult);
            values.Add(Total);
            values.Add(InitialState);
            values.Add(GraduationState);
            values.Add(TransferState);
            values.Add(QuittingState);
            values.Add(LostState);
            values.Add(DeathState);
            values.Add(OtherState);
            return values;
        }
    }
}
