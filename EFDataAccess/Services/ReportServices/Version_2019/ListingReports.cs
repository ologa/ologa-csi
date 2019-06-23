using EFDataAccess.UOW;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.SqlClient;
using EFDataAccess.DTO;
using System.Data;
using VPPS.CSI.Domain;
using System.Collections;
using EFDataAccess.DTO.AgreggatedDTO;
using System.Reflection;
using EFDataAccess.Services;
using EFDataAccess.DTO.SummaryReportsDTO;
using EFDataAccess.DTO.ListingReportsDTOs;

namespace EFDataAccess.Services.ReportServices.Version_2019
{
    public class ListingReports : BaseService
    {
        public ListingReports(UnitOfWork uow) : base(uow) { }

        // Lista de Referencias e seu estado
        public static String QueryReferencesListWithStatus =
        @"SELECT 
            p.Name AS PartnerName, 
            cp.Name AS ChiefPartnerName, 
            CONCAT(ben.FirstName,' ' , ben.LastName) AS BeneficiaryName,
            CASE 
	            WHEN rt.ReferenceName IN (
								            'Maternidade p/ Parto', 'CPN', 'CPN Familiar', 'Suspeito de TB',
								            'Consulta Pós-Parto', 'CCR', 'PTV', 'ATS', 'ITS', 'Pré-TARV/IO',
								            'Testado HIV+', 'Abandono TARV', 'PPE', 'Circuncisao Masculina',
								            'Contacto de TB', 'Controlo de BK', 'Abandono de TTB', 'Reacções do TTB',
								            'Suspeito de Malária', 'Suspeito de Malnutrição', 'Banco de Socorro/Controle de triagem',
								            'Controlo da Dor'
							            ) THEN 'Saude'
	            ELSE 'Accao Social'
            END AS ReferenceProvider,
            rt.ReferenceName,
            CONVERT(varchar, rs.ReferenceDate, 23) AS ReferenceDate,
            CASE 
	            WHEN rs.CounterReferenceDate is null OR rs.CounterReferenceDate > @lastDate THEN 'Sim'
	            ELSE ''
            END AS InProgress,
            CASE 
	            WHEN rs.CounterReferenceDate is not null AND rs.CounterReferenceDate <= @lastDate THEN CONVERT(varchar, rs.CounterReferenceDate, 23)
	            ELSE CONVERT(varchar, '', 23)
            END AS IsComplete
            FROM            
            dbo.Partner AS p 
            INNER JOIN dbo.Partner AS cp ON p.SuperiorId = cp.PartnerID
            INNER JOIN dbo.HouseHold AS hh ON p.PartnerID = hh.PartnerID
            INNER JOIN dbo.Beneficiary AS ben ON ben.HouseholdID = hh.HouseHoldID
            INNER JOIN dbo.ReferenceService AS rs ON (rs.BeneficiaryID = ben.BeneficiaryID)
            INNER JOIN dbo.Reference AS r ON (r.ReferenceServiceID = rs.ReferenceServiceID and r.Value = '1')
            INNER JOIN dbo.ReferenceType rt ON (r.ReferenceTypeID = rt.ReferenceTypeID and rt.ReferenceCategory = 'Activist')
            WHERE rs.ReferenceDate BETWEEN @initialDate AND @lastDate";

        public List<ReferencesListWithStatusDTO> getReferencesListWithStatus(DateTime initialDate, DateTime lastDate)
        {
            return UnitOfWork.DbContext.Database.SqlQuery<ReferencesListWithStatusDTO>(QueryReferencesListWithStatus,
                                                            new SqlParameter("initialDate", initialDate),
                                                            new SqlParameter("lastDate", lastDate)).ToList();
        }


        // Listagem de Beneficiarios no Estado Activo - Amarelo ou Inactivo
        public static String QueryBeneficiariesListingWithYellowActiveOrInactiveStatus =
        @"SELECT 
	            a.FullName, 
	            a.Age, 
	            a.Gender, 
	            a.HouseholdName, 
	            a.Partner, 
	            a.ServiceStatus, 
	            a.ReadyToGraduate, 
	            Service_On_Previous_Trimester = CASE WHEN (SUM(a.Service_On_Previous_Trimester) >= 1) THEN 1 ELSE 0 END,
	            Service_ON_Current_Trimester = CASE WHEN (SUM(a.Service_ON_Current_Trimester) >= 1) THEN 1 ELSE 0 END,
	            CarePlanMonitoring = CASE WHEN (SUM(a.CarePlanMonitoring) >= 1) THEN 1 ELSE 0 END,
	            a.MonthSinceLastPA
            FROM
            (	
	            SELECT
		            distinct FullName = ben.FirstName+' '+ben.LastName,
		            ben.Gender AS Gender,
		            DATEDIFF(Year, CAST(ben.DateOfBirth As Date), @PreviousTrimesterLastDate) AS Age,
		            hh.HouseholdName AS HouseholdName,
		            p.[Name] AS Partner,
		            ServiceStatus = CASE WHEN se.Code = '01' THEN 'Inactivo' WHEN se.Code = '02' THEN 'Activo Amarelo' END,
		            ReadyToGraduate = CASE WHEN cs.Description = 'Pronto para Graduar' THEN 'SIM' ELSE 'NÃO' END,
		            Service_On_Previous_Trimester = CASE WHEN rvm.BeneficiaryHasServices = 1 AND rvm.RoutineVisitDate BETWEEN @PreviousTrimesterInitialDate AND @PreviousTrimesterLastDate THEN 1 ELSE 0 END,
		            Service_On_Current_Trimester = CASE WHEN rvm.BeneficiaryHasServices = 1 AND rvm.RoutineVisitDate BETWEEN @CurrentTrimesterInitialDate AND @CurrentTrimesterLastDate THEN 1 ELSE 0 END,
		            CarePlanMonitoring = CASE WHEN sst.Description = 'Monitoria do Plano de Acção da família' AND rvs.Checked = '1' AND rvm.RoutineVisitDate BETWEEN @CurrentTrimesterInitialDate AND @CurrentTrimesterLastDate THEN 1 ELSE 0 END,
		            DATEDIFF(month, CAST(hsp.SupportPlanInitialDate As Date), @CurrentTrimesterLastDate) AS MonthSinceLastPA
	            FROM [Partner] p
	            inner join [Partner] cp on (p.SuperiorId = cp.PartnerID) 
	            inner join [HouseHold] hh on (p.PartnerID = hh.PartnerID)
	            inner join [HouseholdSupportPlan] hsp on hsp.HouseholdID = hh.HouseHoldID
	            AND hsp.HouseHoldSupportPlanID IN -- Último estado no intervalo de datas
	            (
		            SELECT
			            hspStatusObj.HouseHoldSupportPlanID
		            FROM
		            (
			            SELECT 
				            row_number() OVER (PARTITION BY HouseholdID ORDER BY SupportPlanInitialDate DESC) AS LastStatusRow
				            --Obter o número da linha de acordo com BeneficiaryID, e ordenado pela Data de Efectividade de forma DESCENDENTE(Último ao Primeiro)
				            ,HouseHoldSupportPlanID
				            ,HouseholdID
			            FROM [HouseholdSupportPlan] hsp
			            WHERE hsp.SupportPlanInitialDate < @CurrentTrimesterLastDate
		            )hspStatusObj
		            WHERE hspStatusObj.LastStatusRow = 1 
		            -- Verifica o último estado do Intervalo
	            )
	            inner join [Beneficiary] ben ON ben.HouseholdID = hh.HouseHoldID
	            inner join [SimpleEntity] se ON ben.ServicesStatusForReportID = se.SimpleEntityID and se.Type='ben-services-status' and se.Code in ('01','02')
	            inner join [vw_beneficiary_details] benView ON benView.ID = ben.[BeneficiaryID] and benView.RegistrationDate < @PreviousTrimesterLastDate
	            inner join RoutineVisitMember rvm ON rvm.BeneficiaryID = ben.BeneficiaryID 
	            inner join RoutineVisitSupport rvs on rvm.RoutineVisitMemberID = rvs.RoutineVisitMemberID
	            inner join SupportServiceType sst on rvs.SupportID = sst.SupportServiceTypeID
	            inner join [ChildStatusHistory] csh ON csh.BeneficiaryID = ben.BeneficiaryID 
	            AND csh.ChildStatusHistoryID IN -- Último estado no intervalo de datas
	            (
		            SELECT
			            benStatusObj.ChildStatusHistoryID
		            FROM
		            (
			            SELECT 
				            row_number() OVER (PARTITION BY BeneficiaryID ORDER BY EffectiveDate DESC) AS LastStatusRow
				            --Obter o número da linha de acordo com BeneficiaryID, e ordenado pela Data de Efectividade de forma DESCENDENTE(Último ao Primeiro)
				            ,ChildStatusHistoryID
				            ,BeneficiaryID
			            FROM [ChildStatusHistory] csh
			            WHERE csh.EffectiveDate < @PreviousTrimesterLastDate
		            )benStatusObj
		            WHERE benStatusObj.LastStatusRow = 1 
		            -- Verifica o último estado do Intervalo
	            )
	            inner join [ChildStatus] cs ON cs.StatusID = csh.ChildStatusID
            )a
            group by 
            a.FullName,
            a.Age,
            a.Gender,
            a.HouseholdName, 
            a.Partner, 
            a.ServiceStatus, 
            a.ReadyToGraduate,
            a.MonthSinceLastPA";

        public List<BeneficiariesListingWithYellowActiveOrInactiveStatusDTO> getBeneficiariesListingWithYellowActiveOrInactiveStatus
            (DateTime PreviousTrimesterInitialDate, DateTime PreviousTrimesterLastDate, DateTime CurrentTrimesterInitialDate, DateTime CurrentTrimesterLastDate)
        {
            return UnitOfWork.DbContext.Database.SqlQuery<BeneficiariesListingWithYellowActiveOrInactiveStatusDTO>(QueryBeneficiariesListingWithYellowActiveOrInactiveStatus,
                                                            new SqlParameter("PreviousTrimesterInitialDate", PreviousTrimesterInitialDate),
                                                            new SqlParameter("PreviousTrimesterLastDate", PreviousTrimesterLastDate),
                                                            new SqlParameter("CurrentTrimesterInitialDate", CurrentTrimesterInitialDate),
                                                            new SqlParameter("CurrentTrimesterLastDate", CurrentTrimesterLastDate)).ToList();
        }

        // Listagem Estado do Plano de Accao Familiar
        public static String QueryHouseholdSupportPlanListing =
        @"SELECT
	        FullName = ben.FirstName+' '+ben.LastName
	        ,DATEDIFF(month, CAST(ben.DateOfBirth As Date), @lastDate) AS Age
	        ,ben.Gender AS Gender
	        ,hh.HouseholdName
	        ,p.[Name] AS Partner
	        ,benView.RegistrationDate
	        ,hsp.SupportPlanInitialDate
        FROM [Partner] p
        inner join [Partner] cp on (p.SuperiorId = cp.PartnerID) 
        inner join [HouseHold] hh on (p.PartnerID = hh.PartnerID)
        inner join [Beneficiary] ben ON ben.HouseholdID = hh.HouseHoldID
        inner join [vw_beneficiary_details] benView ON benView.ID = ben.[BeneficiaryID]
        inner join [HouseholdSupportPlan] hsp ON hsp.HouseholdID = hh.HouseholdID AND hsp.SupportPlanInitialDate BETWEEN @initialDate AND @lastDate
        order by ben.FirstName";

        public List<HouseholdSupportPlanListingDTO> getQueryHouseholdSupportPlanListing(DateTime initialDate, DateTime lastDate)
        {
            return UnitOfWork.DbContext.Database.SqlQuery<HouseholdSupportPlanListingDTO>(QueryHouseholdSupportPlanListing,
                                                            new SqlParameter("initialDate", initialDate),
                                                            new SqlParameter("lastDate", lastDate)).ToList();
        }


        public Dictionary<string, List<string>> FindReportDataByCodeAndInitialDateAndLastDate(string QueryCode, DateTime initialDate, DateTime lastDate)
        {
            // Group and Sum this informtation
            List<ReportData> reportData = UnitOfWork.Repository<ReportData>().GetAll().Where(x => x.QueryCode.Equals(QueryCode) && 
                initialDate <= x.InitialPositionDate && lastDate >= x.FinalPositionDate).ToList();

            List<string> outList;
            Dictionary<string, List<string>> groupedData = new Dictionary<string, List<string>>();

            foreach (var DataRow in reportData)
            {
                if (groupedData.TryGetValue(DataRow.Field1, out outList))
                { groupedData[DataRow.Field1] = groupedData[DataRow.Field1].ToList().Concat(ConvertReportDataToStringList(DataRow, 2)).ToList(); }
                else
                { groupedData.Add(DataRow.Field1, ConvertReportDataToStringList(DataRow, 1)); }
            }

            return groupedData;
        }

        public List<string> ConvertReportDataToStringList(ReportData DataRow, int startFrom)
        {
            List<string> DataRowList = new List<string>();

            for (int i = startFrom; true; i++)
            {
                PropertyInfo prop = DataRow.GetType().GetProperty("Field" + i, BindingFlags.Public | BindingFlags.Instance);
                if (null == prop) { break; }
                else {
                    var value = prop.GetValue(DataRow, null);
                    DataRowList.Add(value == null ? string.Empty : (string) prop.GetValue(DataRow, null));
                }
            }

            return DataRowList;
        }

        public List<UniqueEntity> findAllReportDataUniqueEntities() => UnitOfWork.DbContext.Database.SqlQuery<UniqueEntity>("Select SyncGuid, ReportDataID As ID from ReportData").ToList();

        public ReportData findByID(int ID) => UnitOfWork.Repository<ReportData>().GetById(ID);

        public void SaveOrUpdate(ReportData reportData)
        {
            if (reportData.ReportDataID == 0)
            { UnitOfWork.Repository<ReportData>().Add(reportData); }
            else
            { UnitOfWork.Repository<ReportData>().Update(reportData); }
        }

        public int ImportData(string fullPath)
        {
            _logger.Information("IMPORTACAO DE DADOS DOS RELATÓRIOS...");

            UnitOfWork.DbContext.Configuration.AutoDetectChangesEnabled = false;

            string lastGuidToImport = null;
            int ReportDataCount = 0;
            FileImporter imp = new FileImporter();
            DataTable dt2 = imp.ImportFromCSV(fullPath);

            UsersDB = ConvertListToHashtableUsers(findAllUsersUniqueEntities());
            Hashtable ReportDataDB = ConvertListToHashtable(findAllReportDataUniqueEntities());
            // ClearMEReportTableBeforeImport(dt2);

            try
            {
                foreach (DataRow row in dt2.Rows)
                {
                    Guid ReportData_guid = new Guid(row["ReportData_guid"].ToString());
                    lastGuidToImport = ReportData_guid.ToString();

                    int ReportDataID = ParseStringToIntSafe(ReportDataDB[ReportData_guid]);
                    ReportData ReportData = (ReportDataID > 0) ? findByID(ReportDataID) : new ReportData();
                    ReportData.ExecutionNumber = int.Parse(row["ExecutionNumber"].ToString());
                    ReportData.QueryCode = row["QueryCode"].ToString();
                    ReportData.SiteName = row["SiteName"].ToString();
                    ReportData.Province = row["Province"].ToString();
                    ReportData.District = row["District"].ToString();
                    ReportData.Field1 = row["Field1"].ToString().Equals("") ? "0" : row["Field1"].ToString();
                    ReportData.Field2 = row["Field2"].ToString().Equals("") ? "0" : row["Field2"].ToString();
                    ReportData.Field3 = row["Field3"].ToString().Equals("") ? "0" : row["Field3"].ToString();
                    ReportData.Field4 = row["Field4"].ToString().Equals("") ? "0" : row["Field4"].ToString();
                    ReportData.Field5 = row["Field5"].ToString().Equals("") ? "0" : row["Field5"].ToString();
                    ReportData.Field6 = row["Field6"].ToString().Equals("") ? "0" : row["Field6"].ToString();
                    ReportData.Field7 = row["Field7"].ToString().Equals("") ? "0" : row["Field7"].ToString();
                    ReportData.Field8 = row["Field8"].ToString().Equals("") ? "0" : row["Field8"].ToString();
                    ReportData.Field9 = row["Field9"].ToString().Equals("") ? "0" : row["Field9"].ToString();
                    ReportData.Field10 = row["Field10"].ToString().Equals("") ? "0" : row["Field10"].ToString();
                    ReportData.Field11 = row["Field11"].ToString().Equals("") ? "0" : row["Field11"].ToString();
                    ReportData.Field12 = row["Field12"].ToString().Equals("") ? "0" : row["Field12"].ToString();
                    ReportData.Field13 = row["Field13"].ToString().Equals("") ? "0" : row["Field13"].ToString();
                    ReportData.Field14 = row["Field14"].ToString().Equals("") ? "0" : row["Field14"].ToString();
                    ReportData.Field15 = row["Field15"].ToString().Equals("") ? "0" : row["Field15"].ToString();
                    ReportData.Field16 = row["Field16"].ToString().Equals("") ? "0" : row["Field16"].ToString();
                    ReportData.Field17 = row["Field17"].ToString().Equals("") ? "0" : row["Field17"].ToString();
                    ReportData.Field18 = row["Field18"].ToString().Equals("") ? "0" : row["Field18"].ToString();
                    ReportData.Field19 = row["Field19"].ToString().Equals("") ? "0" : row["Field19"].ToString();
                    ReportData.Field20 = row["Field20"].ToString().Equals("") ? "0" : row["Field20"].ToString();
                    ReportData.Field21 = row["Field21"].ToString().Equals("") ? "0" : row["Field21"].ToString();
                    ReportData.Field22 = row["Field22"].ToString().Equals("") ? "0" : row["Field22"].ToString();
                    ReportData.Field23 = row["Field23"].ToString().Equals("") ? "0" : row["Field23"].ToString();
                    ReportData.Field24 = row["Field24"].ToString().Equals("") ? "0" : row["Field24"].ToString();
                    ReportData.Field25 = row["Field25"].ToString().Equals("") ? "0" : row["Field25"].ToString();
                    ReportData.Field26 = row["Field26"].ToString().Equals("") ? "0" : row["Field26"].ToString();
                    ReportData.Field27 = row["Field27"].ToString().Equals("") ? "0" : row["Field27"].ToString();
                    ReportData.Field28 = row["Field28"].ToString().Equals("") ? "0" : row["Field28"].ToString();
                    ReportData.Field29 = row["Field29"].ToString().Equals("") ? "0" : row["Field29"].ToString();
                    ReportData.Field30 = row["Field30"].ToString().Equals("") ? "0" : row["Field30"].ToString();
                    ReportData.Field31 = row["Field31"].ToString().Equals("") ? "0" : row["Field31"].ToString();
                    ReportData.Field32 = row["Field32"].ToString().Equals("") ? "0" : row["Field32"].ToString();
                    ReportData.Field33 = row["Field33"].ToString().Equals("") ? "0" : row["Field33"].ToString();
                    ReportData.Field34 = row["Field34"].ToString().Equals("") ? "0" : row["Field34"].ToString();
                    ReportData.Field35 = row["Field35"].ToString().Equals("") ? "0" : row["Field35"].ToString();
                    ReportData.InitialPositionDate = (row["InitialPositionDate"].ToString()).Length == 0 ? (DateTime?)null : DateTime.Parse(row["InitialPositionDate"].ToString());
                    ReportData.FinalPositionDate = (row["FinalPositionDate"].ToString()).Length == 0 ? (DateTime?)null : DateTime.Parse(row["FinalPositionDate"].ToString());
                    SetCreationDataFields(ReportData, row, ReportData_guid);
                    SetUpdatedDataFields(ReportData, row);
                    SaveOrUpdate(ReportData);
                    ReportDataCount++;
                }

                UnitOfWork.Commit();
                Rename(fullPath, fullPath + IMPORTED);
                return ReportDataCount;
            }
            catch (Exception e)
            {
                _logger.Information("Erro ao importar o Guid : " + lastGuidToImport);
                _logger.Error(e, "Erro ao importar Dados dos Relatórios", null);
                throw e;
            }
            finally
            {
                UnitOfWork.Dispose();
            }
        }

        private void ClearMEReportTableBeforeImport(DataTable dt2)
        {
            DataRow FileFirstRow = dt2.Rows[0];
            string SiteName = FileFirstRow["SiteName"].ToString();
            DateTime initialDate = DateTime.Parse(FileFirstRow["InitialPositionDate"].ToString());
            DateTime lastDate = DateTime.Parse(FileFirstRow["FinalPositionDate"].ToString());
            List<string> QueryCodes = new List<string>
            { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11", "12", "13"};
            QueryCodes.ForEach(queryCode =>
            {
                List<ReportData> existingRecords = UnitOfWork.DbContext.ReportData.Where(data =>
                data.InitialPositionDate == initialDate &&
                data.FinalPositionDate == lastDate &&
                data.QueryCode == queryCode && data.SiteName == SiteName)
                .ToList();

                if (existingRecords.Count() > 0)
                {
                    existingRecords.ForEach(record => UnitOfWork.DbContext.ReportData.Remove(record));
                    UnitOfWork.DbContext.SaveChanges();
                }
            });
        }
    }
}
