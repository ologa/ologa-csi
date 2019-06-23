using EFDataAccess.DTO;
using EFDataAccess.DTO.AgreggatedDTO;
using EFDataAccess.UOW;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Reflection;
using VPPS.CSI.Domain;


namespace EFDataAccess.Services
{
    public class ReportGeneratorService : BaseService
    {
        private Site Site { get; set; }
        private Query query { get; set; }
        private DateTime FinalPositionDate { get; set; }
        private DateTime InitialPositionDate { get; set; }
        private List<AgeGroupDTO> ageDataList;
        private List<HIVGroupDTO> HIVDataList;
        private List<ReferenceGroupDTO> referenceDataList;
        private List<ReferenceGroupV2DTO> referenceDataListV2;
        private List<ReferenceOriginDTO> referenceOriginDataList;
        private List<RoutineVisitGroupDTO> routineVisitDataList;
        private List<RoutineVisitGroupV2DTO> routineVisitDataListV2;
        private List<NonGraduatedBeneficiaryDTO> nonGraduatedBeneficiaryDataList;
        private List<NonGraduatedBeneficiaryV2DTO> nonGraduatedBeneficiaryDataListV2;
        private List<Query> Queries = new List<Query>();

        public ReportGeneratorService(UnitOfWork uow, Site Site, DateTime InitialPositionDate, DateTime FinalPositionDate) : base(uow)
        {
            this.Site = Site;
            this.InitialPositionDate = InitialPositionDate;
            this.FinalPositionDate = FinalPositionDate;
        }

        public ReportGeneratorService(UnitOfWork uow, Query query, List<AgeGroupDTO> ageDataList, 
            List<HIVGroupDTO> HIVDataList, List<NonGraduatedBeneficiaryDTO> nonGraduatedBeneficiaryDataList,
            List<NonGraduatedBeneficiaryV2DTO> nonGraduatedBeneficiaryDataListV2, List<ReferenceGroupDTO> referenceDataList, 
            List<ReferenceGroupV2DTO> referenceDataListV2, List<ReferenceOriginDTO> referenceOriginDataList, 
            List<RoutineVisitGroupDTO> routineVisitDataList, List<RoutineVisitGroupV2DTO> routineVisitDataListV2,
            Site site, DateTime InitialPositionDate, DateTime FinalPositionDate) : base(uow)
        {
            this.Site = site;
            this.query = query;
            this.ageDataList = ageDataList;
            this.HIVDataList = HIVDataList;
            this.nonGraduatedBeneficiaryDataList = nonGraduatedBeneficiaryDataList;
            this.nonGraduatedBeneficiaryDataListV2 = nonGraduatedBeneficiaryDataListV2;
            this.referenceDataList = referenceDataList;
            this.referenceDataListV2 = referenceDataListV2;
            this.referenceOriginDataList = referenceOriginDataList;
            this.routineVisitDataList = routineVisitDataList;
            this.routineVisitDataListV2 = routineVisitDataListV2;
            this.InitialPositionDate = InitialPositionDate;
            this.FinalPositionDate = FinalPositionDate;
        }

        public void AddQuery(Query query)
        {
            this.Queries.Add(query);
        }

        public void GenerateReportDataGeneric()
        {
            _logger.Information("== INICIO DO PROCESSO DE GERACAO DE DADOS DE RELATORIOS BATCH ==");

            foreach (var query in this.Queries)
            {
                List<List<System.String>> rowsList = new List<List<string>>();

                string conString = UnitOfWork.DbContext.Database.Connection.ConnectionString + ";Connection Timeout=180";
                using (SqlConnection connection = new SqlConnection(conString))
                {
                    connection.Open();
                    using (SqlCommand command = new SqlCommand(query.Sql, connection))
                    {
                        command.Parameters.Add(new SqlParameter("initialDate", InitialPositionDate));
                        command.Parameters.Add(new SqlParameter("lastDate", FinalPositionDate));

                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                List<string> row = new List<String>();
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    row.Add(Convert.ToString(reader.GetValue(i)));
                                }
                                rowsList.Add(row);
                            }
                        }
                    }
                }

                _logger.Information("Foram geradas " + rowsList.Count + " linhas.");
                query.Result = rowsList;
            }

            _logger.Information("== TERMINO DO PROCESSO DE GERACAO DE RELATORIOS BATCH ==");
        }

        public async System.Threading.Tasks.Task PersistReportDataAsync()
        {
            await PersistReportDataAsync(null);
        }

        public async System.Threading.Tasks.Task PersistReportDataAsync(List<List<System.String>> Result)
        {
            if (Result != null)
            {
                this.query.Result = Result;
                this.Queries = new List<Query>();
                this.Queries.Add(this.query);
            }

            int executionNumber = 1;
            _logger.Information("== INICIO DO PROCESSO DE PERSISTENCIA DE RELATORIOS BATCH ==");
            //int executionNumber = await UnitOfWork.DbContext.Database.SqlQuery<int>("SELECT MAX(ExecutionNumber) FROM ReportData").SingleOrDefaultAsync();

            foreach (var query in this.Queries)
            {
                foreach (List<String> row in query.Result)
                {
                    int i = 1;
                    ReportData reportData = new ReportData();
                    foreach (String cell in row)
                    {
                        reportData.SiteName = this.Site.SiteName;
                        reportData.Province = this.Site.orgUnit.Parent.Name;
                        reportData.District = this.Site.orgUnit.Name;
                        reportData.CreatedUserID = UnitOfWork.DbContext.GetUserInfo().UserID;
                        reportData.InitialPositionDate = this.InitialPositionDate;
                        reportData.FinalPositionDate = this.FinalPositionDate;
                        reportData.QueryCode = query.Code;
                        reportData.ExecutionNumber = executionNumber;
                        PropertyInfo prop = reportData.GetType().GetProperty("Field" + i, BindingFlags.Public | BindingFlags.Instance);
                        if (null != prop && prop.CanWrite) { prop.SetValue(reportData, cell, null); }
                        i++;
                    }

                    UnitOfWork.Repository<ReportData>().Add(reportData);
                }

                _logger.Information("Foram persistidas " + query.Result.Count + " linhas.");
            }

            _logger.Information("== TERMINO DO PROCESSO DE PERSISTENCIA DE RELATORIOS BATCH ==");
            UnitOfWork.Commit();
        }

        public List<List<String>> GenerateReportDataFromDataLists()
        {
            _logger.Information("== INICIO DO PROCESSO DE GERACAO DE DADOS DE RELATORIOS BATCH ==");

            List<List<System.String>> rowsList = new List<List<string>>();

            if (this.ageDataList != null)
            {
                foreach (var refType in this.ageDataList)
                {
                    List<string> row = new List<String>();
                    foreach (var item in refType.PopulatedValues())
                    {
                        row.Add(Convert.ToString(item));
                    }
                    rowsList.Add(row);
                }
            }


            if (this.HIVDataList != null)
            {
                foreach (var refType in this.HIVDataList)
                {
                    List<string> row = new List<String>();
                    foreach (var item in refType.PopulatedValues())
                    {
                        row.Add(Convert.ToString(item));
                    }
                    rowsList.Add(row);
                }
            }

            if (this.nonGraduatedBeneficiaryDataList != null)
            {
                foreach (var refType in this.nonGraduatedBeneficiaryDataList)
                {
                    List<string> row = new List<String>();
                    foreach (var item in refType.PopulatedValues())
                    {
                        row.Add(Convert.ToString(item));
                    }
                    rowsList.Add(row);
                }
            }

            if (this.nonGraduatedBeneficiaryDataListV2 != null)
            {
                foreach (var refType in this.nonGraduatedBeneficiaryDataListV2)
                {
                    List<string> row = new List<String>();
                    foreach (var item in refType.PopulatedValues())
                    {
                        row.Add(Convert.ToString(item));
                    }
                    rowsList.Add(row);
                }
            }


            if (this.referenceDataList != null)
            {
                foreach (var refType in this.referenceDataList)
                {
                    List<string> row = new List<String>();
                    foreach (var item in refType.PopulatedValues())
                    {
                        row.Add(Convert.ToString(item));
                    }
                    rowsList.Add(row);
                }
            }

            if (this.referenceDataListV2 != null)
            {
                foreach (var refType in this.referenceDataListV2)
                {
                    List<string> row = new List<String>();
                    foreach (var item in refType.PopulatedValues())
                    {
                        row.Add(Convert.ToString(item));
                    }
                    rowsList.Add(row);
                }
            }


            if (this.referenceOriginDataList != null)
            {
                foreach (var refType in this.referenceOriginDataList)
                {
                    List<string> row = new List<String>();
                    foreach (var item in refType.PopulatedValues())
                    {
                        row.Add(Convert.ToString(item));
                    }
                    rowsList.Add(row);
                }
            }


            if (this.routineVisitDataList != null)
            {
                foreach (var refType in this.routineVisitDataList)
                {
                    List<string> row = new List<String>();
                    foreach (var item in refType.PopulatedValues())
                    {
                        row.Add(Convert.ToString(item));
                    }
                    rowsList.Add(row);
                }
            }

            _logger.Information("Foram geradas " + rowsList.Count + " linhas.");
            _logger.Information("== TERMINO DO PROCESSO DE GERACAO DE RELATORIOS BATCH ==");

            return rowsList;
        }


        public List<List<String>> GenerateReportDataFromHIVDataList()
        {
            _logger.Information("== INICIO DO PROCESSO DE GERACAO DE DADOS DE RELATORIOS BATCH ==");

            List<List<System.String>> rowsList = new List<List<string>>();

            foreach (var refType in this.HIVDataList)
            {
                List<string> row = new List<String>();
                foreach (var item in refType.PopulatedValues())
                {
                    row.Add(Convert.ToString(item));
                }
                rowsList.Add(row);
            }
            if (this.routineVisitDataListV2 != null)
            {
                foreach (var refType in this.routineVisitDataListV2)
                {
                    List<string> row = new List<String>();
                    foreach (var item in refType.PopulatedValues())
                    {
                        row.Add(Convert.ToString(item));
                    }
                    rowsList.Add(row);
                }
            }

            _logger.Information("Foram geradas " + rowsList.Count + " linhas.");
            _logger.Information("== TERMINO DO PROCESSO DE GERACAO DE RELATORIOS BATCH ==");

            return rowsList;
        }
    }
}
