using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using EFDataAccess;
using VPPS.CSI.Domain;
using EFDataAccess.Services;
using System.IO;
using System.Web.UI.WebControls;
using EFDataAccess.UOW;
using Excel = Microsoft.Office.Interop.Excel;
using EFDataAccess.DTO;
using MAC.ViewModels;
using System.Web.UI;
using EFDataAccess.DTO.TemporaryReportListingsDTO;
using EFDataAccess.Services.ReportServices.Version_2019;
using EFDataAccess.DTO.SummaryReportsDTO;
using EFDataAccess.DTO.ListingReportsDTOs;
using EFDataAccess.Services.TrimesterServices;

namespace MAC.Controllers
{
    [Allow(Roles = REPORT_VIEWERS)]
    public class ReportsController : BaseController
    {
        private UserService userService = new UserService(new UnitOfWork());
        private SiteService siteService = new SiteService(new UnitOfWork());
        private SiteService SiteService = new SiteService(new UnitOfWork());
        private ReportService reportService = new ReportService(new UnitOfWork());
        private PartnerService partnerService = new PartnerService(new UnitOfWork());
        private ReportDataServiceV2 reportDataServiceV2 = new ReportDataServiceV2(new UnitOfWork());
        private HIVStatusQueryService hivStatusQueryService = new HIVStatusQueryService(new UnitOfWork());
        private ReferenceServiceService referenceServiceService = new ReferenceServiceService(new UnitOfWork());
        private BeneficiaryService beneficiaryService = new BeneficiaryService(new UnitOfWork());
        private SummaryReports summaryReports = new SummaryReports(new UnitOfWork());
        private ListingReports listingReports = new ListingReports(new UnitOfWork());
        private TrimesterQueryService trimesterQueryService = new TrimesterQueryService(new UnitOfWork());



        public ActionResult Index()
        {
            return View();
        }

        /*****************************************
         * InitialRecordSummaryReport
         *****************************************/

        public ActionResult InitialRecordSummaryReport()
        {
            ReportIntervalViewModel reportIntervalDTO = new ReportIntervalViewModel();
            reportIntervalDTO.initialDate = new DateTime(DateTime.Now.Year, 1, 1);
            reportIntervalDTO.finalDate = DateTime.Now;
            return View(reportIntervalDTO);
        }

        [HttpPost, ActionName("download-initial-record-summary-report")]
        public ActionResult DownloadInicialRecordSummaryReport([Bind(Include = "initialDate, finalDate")] ReportIntervalViewModel reportIntervalDTO)
        {
            var fileName = @"~\Templates\initial-record-summary-with-new-services.xlsx";
            Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
            Excel._Workbook workbook = excelApp.Workbooks.Open(Server.MapPath(fileName));
            //Excel._Worksheet worksheet = excelApp.ActiveSheet;

            Site site = siteService.findById(1);

            // Worksheet 2, grouped by partner

            Excel._Worksheet worksheet = excelApp.Worksheets[2];
            worksheet.Range["V2:AE2"].Value = "Período de " + reportIntervalDTO.initialDate.ToShortDateString() + " à " + reportIntervalDTO.finalDate.ToShortDateString();
            worksheet.Range["B2:F2"].Value = "Província: " + site.orgUnit.Parent.Name;
            worksheet.Range["G2:O2"].Value = "Distrito: " + site.orgUnit.Name;
            worksheet.Range["P2:U2"].Value = "Nome da OCB: " + site.SiteName;
            //worksheet.Range["V2:Y2"].Value = "Ano: " + reportIntervalDTO.finalDate.Year;
            //worksheet.Range["AA2:AE2"].Value = "Mês: " + reportIntervalDTO.finalDate.Month;

            var row = 5;
            var column = 0;
            List<InitialRecordSummaryReportDTO> data = reportService.getInitialRecordSummaryPartner(reportIntervalDTO.initialDate, reportIntervalDTO.finalDate);

            foreach (var refType in data)
            {
                row++; column = 0;
                foreach (var item in refType.PopulatedValues())
                {
                    column++;
                    worksheet.Cells[row, column] = item;
                }
                worksheet.Rows[row + 1].Insert();
            }

            // Worksheet 1, grouped by head partner

            worksheet = excelApp.Worksheets[1];
            worksheet.Range["V2:AE2"].Value = "Período de " + reportIntervalDTO.initialDate.ToShortDateString() + " à " + reportIntervalDTO.finalDate.ToShortDateString();
            worksheet.Range["B2:F2"].Value = "Província: " + site.orgUnit.Parent.Name;
            worksheet.Range["G2:O2"].Value = "Distrito: " + site.orgUnit.Name;
            worksheet.Range["P2:U2"].Value = "Nome da OCB: " + site.SiteName;
            //worksheet.Range["V2:Y2"].Value = "Ano: " + reportIntervalDTO.finalDate.Year;
            //worksheet.Range["AA2:AE2"].Value = "Mês: " + reportIntervalDTO.finalDate.Month;

            row = 5;
            column = 0;
            data = reportService.getInitialRecordSummaryChiefPartner(reportIntervalDTO.initialDate, reportIntervalDTO.finalDate);

            foreach (var refType in data)
            {
                row++; column = 0;
                foreach (var item in refType.PopulatedValues())
                {
                    column++;
                    worksheet.Cells[row, column] = item;
                }
                worksheet.Rows[row + 1].Insert();
            }

            var temporaryFilepath = Path.ChangeExtension(Path.GetTempFileName(), ".xlsx");
            workbook.SaveAs(temporaryFilepath);
            workbook.Close(0);
            excelApp.Quit();
            DownloadXlsxFile(temporaryFilepath);
            return RedirectToAction("InitialRecordSummaryReport");
        }


        /*****************************************
         *        New Beneficiaries Report
         *****************************************/

        public ActionResult NewBeneficiariesReport()
        {
            ReportIntervalViewModel reportIntervalDTO = new ReportIntervalViewModel();
            reportIntervalDTO.initialDate = new DateTime(DateTime.Now.Year, 1, 1);
            reportIntervalDTO.finalDate = DateTime.Now;
            return View(reportIntervalDTO);
        }

        [HttpPost, ActionName("download-new-beneficiaries-report")]
        public ActionResult DownloadNewBeneficiariesReport([Bind(Include = "initialDate, finalDate")] ReportIntervalViewModel reportIntervalDTO)
        {
            var fileName = @"~\Templates\initial-record-summary-with-new-services.xlsx";
            Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
            Excel._Workbook workbook = excelApp.Workbooks.Open(Server.MapPath(fileName));
            Site site = siteService.findById(1);

            // Worksheet 2, grouped by partner

            Excel._Worksheet worksheet = excelApp.Worksheets[2];
            worksheet.Range["V2:AE2"].Value = "Período de " + reportIntervalDTO.initialDate.ToShortDateString() + " à " + reportIntervalDTO.finalDate.ToShortDateString();
            worksheet.Range["B2:F2"].Value = "Província: " + site.orgUnit.Parent.Name;
            worksheet.Range["G2:O2"].Value = "Distrito: " + site.orgUnit.Name;
            worksheet.Range["P2:U2"].Value = "Nome da OCB: " + site.SiteName;

            var row = 5;
            var column = 0;
            Dictionary<string, List<string>> groupedDataPartner = reportDataServiceV2
                .FindReportDataByCodeAndInitialDateAndLastDate("2.1.1", reportIntervalDTO.initialDate, reportIntervalDTO.finalDate);

            foreach (var data in groupedDataPartner)
            {
                row++; column = 0;
                foreach (var item in data.Value)
                {
                    column++;
                    worksheet.Cells[row, column] = item;
                }
                worksheet.Rows[row + 1].Insert();
            }

            // Worksheet 1, grouped by head partner

            worksheet = excelApp.Worksheets[1];
            worksheet.Range["V2:AE2"].Value = "Período de " + reportIntervalDTO.initialDate.ToShortDateString() + " à " + reportIntervalDTO.finalDate.ToShortDateString();
            worksheet.Range["B2:F2"].Value = "Província: " + site.orgUnit.Parent.Name;
            worksheet.Range["G2:O2"].Value = "Distrito: " + site.orgUnit.Name;
            worksheet.Range["P2:U2"].Value = "Nome da OCB: " + site.SiteName;

            row = 5;
            column = 0;
            Dictionary<string, List<string>> groupedDataHeadPartner = reportDataServiceV2
                .FindReportDataByCodeAndInitialDateAndLastDate("2.1.1", reportIntervalDTO.initialDate, reportIntervalDTO.finalDate);

            foreach (var data in groupedDataHeadPartner)
            {
                row++; column = 0;
                foreach (var item in data.Value)
                {
                    column++;
                    worksheet.Cells[row, column] = item;
                }
                worksheet.Rows[row + 1].Insert();
            }

            var temporaryFilepath = Path.ChangeExtension(Path.GetTempFileName(), ".xlsx");
            workbook.SaveAs(temporaryFilepath);
            workbook.Close(0);
            excelApp.Quit();
            DownloadXlsxFile(temporaryFilepath);
            return RedirectToAction("InitialRecordSummaryReport");
        }

        ///*****************************************
        // *  MonthlyActiveBeneficiariesSummaryReport
        // *****************************************/

        //public ActionResult MonthlyActiveBeneficiariesSummaryReport()
        //{
        //    ReportIntervalViewModel reportIntervalDTO = new ReportIntervalViewModel();
        //    reportIntervalDTO.initialDate = new DateTime(DateTime.Now.Year, 1, 1);
        //    reportIntervalDTO.finalDate = DateTime.Now;
        //    return View(reportIntervalDTO);
        //}

        //[HttpPost, ActionName("download-monthly-active-beneficiaries-summary")]
        //public ActionResult DownloadMonthlyActiveBeneficiariesSummaryReport([Bind(Include = "initialDate, finalDate")] ReportIntervalViewModel reportIntervalDTO)
        //{
        //    var fileName = @"~\Templates\monthly-active-beneficiaries-summary.xlsx";
        //    Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
        //    Excel._Workbook workbook = excelApp.Workbooks.Open(Server.MapPath(fileName));
        //    //Excel._Worksheet worksheet = excelApp.ActiveSheet;

        //    Site site = siteService.findById(1);

        //    // Worksheet 2, grouped by partner

        //    Excel._Worksheet worksheet = excelApp.Worksheets[2];
        //    worksheet.Range["AB2:AI2"].Value = "Período de " + reportIntervalDTO.initialDate.ToShortDateString() + " à " + reportIntervalDTO.finalDate.ToShortDateString();
        //    worksheet.Range["A2:B2"].Value = "Província: " + site.orgUnit.Parent.Name;
        //    worksheet.Range["C2:H2"].Value = "Distrito: " + site.orgUnit.Name;
        //    worksheet.Range["I2:S2"].Value = "Nome da OCB: " + site.SiteName;

        //    var row = 6;
        //    var column = 0;
        //    // List<MonthlyActiveBeneficiariesSummaryReportDTO> data = reportService.getMonthlyActiveBeneficiariesSummaryChiefPartner(reportIntervalDTO.initialDate, reportIntervalDTO.finalDate);
        //    Dictionary<string, List<string>> groupedDataPartner = reportDataServiceV2
        //        .FindReportDataByCodeAndInitialDateAndLastDate("2.4.2", reportIntervalDTO.initialDate, reportIntervalDTO.finalDate);

        //    foreach (var data in groupedDataPartner)
        //    {
        //        row++; column = 1;
        //        foreach (var item in data.Value)
        //        {
        //            column++;
        //            worksheet.Cells[row, column] = item;
        //        }
        //        worksheet.Rows[row + 1].Insert();
        //    }

        //    // Worksheet 1, grouped by head partner

        //    worksheet = excelApp.Worksheets[1];
        //    worksheet.Range["AG2:AN2"].Value = "Período de " + reportIntervalDTO.initialDate.ToShortDateString() + " à " + reportIntervalDTO.finalDate.ToShortDateString();
        //    worksheet.Range["A2:B2"].Value = "Província: " + site.orgUnit.Parent.Name;
        //    worksheet.Range["C2:H2"].Value = "Distrito: " + site.orgUnit.Name;
        //    worksheet.Range["I2:S2"].Value = "Nome da OCB: " + site.SiteName;

        //    row = 6;
        //    Dictionary<string, List<string>> groupedDataHeadPartner = reportDataServiceV2
        //        .FindReportDataByCodeAndInitialDateAndLastDate("2.4.1", reportIntervalDTO.initialDate, reportIntervalDTO.finalDate);

        //    foreach (var data in groupedDataHeadPartner)
        //    {
        //        row++; column = 1;
        //        foreach (var item in data.Value)
        //        {
        //            column++;
        //            worksheet.Cells[row, column] = item;
        //        }
        //        worksheet.Rows[row + 1].Insert();
        //    }

        //    var temporaryFilepath = Path.ChangeExtension(Path.GetTempFileName(), ".xlsx");
        //    workbook.SaveAs(temporaryFilepath);
        //    workbook.Close(0);
        //    excelApp.Quit();
        //    DownloadXlsxFile(temporaryFilepath);
        //    return RedirectToAction("MonthlyActiveBeneficiariesSummaryReport");
        //}
        

        /*****************************************
         * RoutineVisitSummaryReport
         *****************************************/

        public ActionResult RoutineVisitSummaryReport()
        {
            ReportFiscalIntervalViewModel reportIntervalDTO = new ReportFiscalIntervalViewModel();
            reportIntervalDTO.initialDate = new DateTime(DateTime.Now.Year, 1, 1);
            reportIntervalDTO.finalDate = DateTime.Now;
            return View(reportIntervalDTO);
        }

        [HttpPost, ActionName("download-routine-visit-summary-report")]
        public ActionResult DownloadRoutineVistSummaryReport([Bind(Include = "initialDate, finalDate")] ReportFiscalIntervalViewModel reportIntervalDTO)
        {
            if (!ModelState.IsValid)
            {
                return View("RoutineVisitSummaryReport");
            }

            var fileName = @"~\Templates\routine-visit-summary.xlsx";
            Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
            Excel._Workbook workbook = excelApp.Workbooks.Open(Server.MapPath(fileName));

            Site site = siteService.findById(1);

            // Worksheet 2, grouped by partner

            Excel._Worksheet worksheet = excelApp.Worksheets[2];
            worksheet.Range["AF1:AT1"].Value = "Período de " + reportIntervalDTO.initialDate.ToShortDateString() + " à " + reportIntervalDTO.finalDate.ToShortDateString();
            worksheet.Range["A2:B2"].Value = "Província: " + site.orgUnit.Parent.Name;
            worksheet.Range["C2:J2"].Value = "Distrito: " + site.orgUnit.Name;
            worksheet.Range["K2:T2"].Value = "Nome da OCB: " + site.SiteName;
            worksheet.Range["AB2:AD2"].Value = "Mês: " + reportIntervalDTO.finalDate.Month;
            worksheet.Range["AF2:AG2"].Value = "Ano: " + reportIntervalDTO.finalDate.Year;

            var row = 5; var column = 1;
            List<EFDataAccess.DTO.RoutineVisitSummaryReportDTO> data = reportService.getRoutineVisitSummaryPartner(reportIntervalDTO.initialDate, reportIntervalDTO.finalDate);
            foreach (var d in data)
            {
                row++; column = 1;
                foreach (var item in d.PopulatedValues())
                {
                    column++;
                    worksheet.Cells[row, column] = item;
                }
                worksheet.Rows[row + 1].Insert();
            }

            // Worksheet 1, grouped by head partner

            worksheet = excelApp.Worksheets[1];
            worksheet.Range["AF1:AT1"].Value = "Período de " + reportIntervalDTO.initialDate.ToShortDateString() + " à " + reportIntervalDTO.finalDate.ToShortDateString();
            worksheet.Range["A2:B2"].Value = "Província: " + site.orgUnit.Parent.Name;
            worksheet.Range["C2:J2"].Value = "Distrito: " + site.orgUnit.Name;
            worksheet.Range["K2:T2"].Value = "Nome da OCB: " + site.SiteName;
            worksheet.Range["AB2:AD2"].Value = "Mês: " + reportIntervalDTO.finalDate.Month;
            worksheet.Range["AF2:AG2"].Value = "Ano: " + reportIntervalDTO.finalDate.Year;

            row = 5; column = 1;
            data = reportService.getRoutineVisitSummaryChiefPartner(reportIntervalDTO.initialDate, reportIntervalDTO.finalDate);
            foreach (var d in data)
            {
                row++; column = 1;
                foreach (var item in d.PopulatedValues())
                {
                    column++;
                    worksheet.Cells[row, column] = item;
                }
                worksheet.Rows[row + 1].Insert();
            }

            var temporaryFilepath = Path.ChangeExtension(Path.GetTempFileName(), ".xlsx");
            workbook.SaveAs(temporaryFilepath);
            workbook.Close(0);
            excelApp.Quit();
            DownloadXlsxFile(temporaryFilepath);
            return RedirectToAction("RoutineVisitSummaryReport");
        }

        //public ActionResult ReferencesListWithStatus()
        //{
        //    ReportIntervalViewModel reportIntervalDTO = new ReportIntervalViewModel();
        //    reportIntervalDTO.initialDate = new DateTime(DateTime.Now.Year, 1, 1);
        //    reportIntervalDTO.finalDate = DateTime.Now;
        //    return View(reportIntervalDTO);
        //}

        //[HttpPost, ActionName("download-references-list-with-status-report")]
        //public ActionResult DownloadReferencesListWithStatus([Bind(Include = "initialDate, finalDate")] ReportIntervalViewModel reportIntervalDTO)
        //{
        //    Excel.Application excelApp = GetExcelAppInstance();
        //    Excel._Workbook workbook = OpenWorkBook(excelApp, @"~\Templates\references-list-with-status.xlsx");
        //    Excel._Worksheet activeWorksheet = OpenWorkbookActiveWorksheet(excelApp);

        //    List<ReferencesListWithStatusDTO> referencesList = reportService
        //                                                .GetReferencesListWithStatus(reportIntervalDTO.initialDate, reportIntervalDTO.finalDate);
        //    var row = 4;

        //    referencesList.ForEach(listItem =>
        //    {
        //        var column = 1;

        //        listItem.populatedValues().ForEach(value =>
        //        {
        //            activeWorksheet.Cells[row, column] = value;
        //            column++;
        //        });
        //        //FIXME: activeWorksheet.Rows[row + 1].Insert();
        //        row++;
        //    });

        //    var temporaryFilepath = Path.ChangeExtension(Path.GetTempFileName(), ".xlsx");
        //    workbook.SaveAs(temporaryFilepath);
        //    workbook.Close(0);
        //    excelApp.Quit();
        //    DownloadXlsxFile(temporaryFilepath);

        //    return RedirectToAction("ReferencesListWithStatus");
        //}

        private Excel._Worksheet OpenWorkbookActiveWorksheet(Excel.Application excelApp)
        {
            return excelApp.ActiveSheet;
        }

        private Excel.Application GetExcelAppInstance()
        {
            return new Excel.Application();
        }

        private Excel._Workbook OpenWorkBook(Excel.Application excelApp, string fileName)
        {
            return excelApp.Workbooks.Open(Server.MapPath(fileName));
        }

        /*****************************************
         * ReferencesSummaryReport
         *****************************************/

        public ActionResult ReferencesSummaryReport()
        {
            ReportIntervalViewModel reportIntervalDTO = new ReportIntervalViewModel();
            reportIntervalDTO.initialDate = new DateTime(DateTime.Now.Year, 1, 1);
            reportIntervalDTO.finalDate = DateTime.Now;
            return View(reportIntervalDTO);
        }

        [HttpPost, ActionName("download-references-summary-report")]
        public ActionResult DownloadReferencesSummaryReport([Bind(Include = "initialDate, finalDate")] ReportIntervalViewModel reportIntervalDTO)
        {
            var fileName = @"~\Templates\activist_references_summary.xlsx";
            Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
            Excel._Workbook workbook = excelApp.Workbooks.Open(Server.MapPath(fileName));
            Excel._Worksheet worksheet = excelApp.ActiveSheet;

            Site site = siteService.findById(1);


            // Worksheet 2 Partner


            var row = 6;
            var column = 0;

            //List<ReferencesSummaryByRefTypeReportDTO> dataChildren = reportService.getReferencesSummaryForPartnerByGender(reportIntervalDTO.initialDate, reportIntervalDTO.finalDate, "Child");
            //List<ReferencesSummaryByRefTypeReportDTO> dataAdults = reportService.getReferencesSummaryForPartnerByGender(reportIntervalDTO.initialDate, reportIntervalDTO.finalDate, "Adult");
            List<ActivistReferencesSummaryByRefTypeReportDTO> dataChildren = referenceServiceService.getActivistReferencesForChildReportSummaryPartner(reportIntervalDTO.initialDate, reportIntervalDTO.finalDate);
            List<ActivistReferencesSummaryByRefTypeReportDTO> dataAdults = referenceServiceService.getActivistReferencesForAdultReportSummaryPartner(reportIntervalDTO.initialDate, reportIntervalDTO.finalDate);

            List<ReferencesSummaryByAgeReportDTO> dataReferencesByAge = new List<ReferencesSummaryByAgeReportDTO>();
            dataReferencesByAge.Add(referenceServiceService.getReferencesSummaryByAgeEspecific(reportIntervalDTO.initialDate, reportIntervalDTO.finalDate, new List<String>() { "ATS" }, "IN"));
            dataReferencesByAge.Add(referenceServiceService.getReferencesSummaryByAgeEspecific(reportIntervalDTO.initialDate, reportIntervalDTO.finalDate, new List<String>() { "PTV", "Testado HIV+", "Pré-TARV/IO", "Abandono TARV", "PPE" }, "IN")); ;
            dataReferencesByAge.Add(referenceServiceService.getReferencesSummaryByAgeEspecific(reportIntervalDTO.initialDate, reportIntervalDTO.finalDate, new List<String>() { "CCR" }, "IN"));
            dataReferencesByAge.Add(referenceServiceService.getReferencesSummaryByAgeEspecific(reportIntervalDTO.initialDate, reportIntervalDTO.finalDate, new List<String>() { "Maternidade p/ Parto", "CPN", "CPN Familiar", "Consulta Pós-Parto", "ITS", "Circuncisao Masculina" }, "IN"));
            dataReferencesByAge.Add(referenceServiceService.getReferencesSummaryByAgeEspecific(reportIntervalDTO.initialDate, reportIntervalDTO.finalDate, new List<String>() { "GAVV", "Apoio Psico-Social", "Posto Policial" }, "IN"));
            dataReferencesByAge.Add(referenceServiceService.getReferencesSummaryByAgeEspecific(reportIntervalDTO.initialDate, reportIntervalDTO.finalDate, new List<String>() { "Atestado de Pobreza" }, "IN"));
            dataReferencesByAge.Add(referenceServiceService.getReferencesSummaryByAgeEspecific(reportIntervalDTO.initialDate, reportIntervalDTO.finalDate, new List<String>() { "Registo de Nascimento/Cédula" }, "IN")); ;
            dataReferencesByAge.Add(referenceServiceService.getReferencesSummaryByAgeEspecific(reportIntervalDTO.initialDate, reportIntervalDTO.finalDate, new List<String>() { "Bilhete de Identidade (B.I)" }, "IN"));
            dataReferencesByAge.Add(referenceServiceService.getReferencesSummaryByAgeEspecific(reportIntervalDTO.initialDate, reportIntervalDTO.finalDate, new List<String>() { "Integração Escolar" }, "IN"));
            dataReferencesByAge.Add(referenceServiceService.getReferencesSummaryByAgeEspecific(reportIntervalDTO.initialDate, reportIntervalDTO.finalDate, new List<String>() { "Curso de Formação Vocacional" }, "IN"));
            dataReferencesByAge.Add(referenceServiceService.getReferencesSummaryByAgeEspecific(reportIntervalDTO.initialDate, reportIntervalDTO.finalDate, new List<String>() { "Material Escolar" }, "IN"));
            dataReferencesByAge.Add(referenceServiceService.getReferencesSummaryByAgeEspecific(reportIntervalDTO.initialDate, reportIntervalDTO.finalDate, new List<String>() { "Cesta Básica" }, "IN")); ;
            dataReferencesByAge.Add(referenceServiceService.getReferencesSummaryByAgeEspecific(reportIntervalDTO.initialDate, reportIntervalDTO.finalDate, new List<String>() { "Subsídios Sociais do INAS" }, "IN"));
            dataReferencesByAge.Add(referenceServiceService.getReferencesSummaryByAgeEspecific(reportIntervalDTO.initialDate, reportIntervalDTO.finalDate, new List<String>() { "SAAJ" }, "IN"));
            dataReferencesByAge.Add(referenceServiceService.getReferencesSummaryByAgeEspecific(reportIntervalDTO.initialDate, reportIntervalDTO.finalDate, new List<String>() { "Desnutrição" }, "IN"));
            dataReferencesByAge.Add(referenceServiceService.getReferencesSummaryByAgeEspecific(reportIntervalDTO.initialDate, reportIntervalDTO.finalDate, new List<String>() { "Atraso no Desenvolvimento" }, "IN"));
            dataReferencesByAge.Add(referenceServiceService.getReferencesSummaryByAgeEspecific(reportIntervalDTO.initialDate, reportIntervalDTO.finalDate, new List<String>() { "ATS", "PTV", "Testado HIV+", "Pré-TARV/IO", "Abandono TARV", "PPE", "CCR"
                                                                                                                     ,"Maternidade p/ Parto", "CPN", "CPN Familiar", "Consulta Pós-Parto", "ITS", "Circuncisao Masculina","GAVV", "Apoio Psico-Social", "Posto Policial"
                                                                                                                     ,"Atestado de Pobreza","Registo de Nascimento/Cédula","Bilhete de Identidade (B.I)","Integração Escolar"
                                                                                                                        ,"Curso de Formação Vocacional","Material Escolar","Cesta Básica","Subsídios Sociais do INAS","SAAJ","Desnutrição", "Atraso no Desenvolvimento"}, "NOT IN"));


            worksheet = excelApp.Worksheets[2];
            worksheet.Range["P1:AC1"].Value = "Período de " + reportIntervalDTO.initialDate.ToShortDateString() + " à " + reportIntervalDTO.finalDate.ToShortDateString();
            worksheet.Range["A1:H1"].Value = "Nome da Organização: " + site.SiteName;
            worksheet.Range["I1:O1"].Value = "Distrito: " + site.orgUnit.Name;
            //worksheet.Range["P1:T1"].Value = "Mês: " + reportIntervalDTO.finalDate.Month;
            //worksheet.Range["U1:AC1"].Value = "Ano: " + reportIntervalDTO.finalDate.Year;

            foreach (var d in dataChildren)
            {
                row++;
                column = 0;
                foreach (var item in d.PopulatedValues())
                {
                    column++;
                    worksheet.Cells[row, column] = item;
                }
                worksheet.Rows[row + 1].Insert();
            }

            row = row + 5;
            foreach (var d in dataAdults)
            {
                row++;
                column = 0;
                foreach (var item in d.PopulatedValues())
                {
                    column++; worksheet.Cells[row, column] = item;
                }
                worksheet.Rows[row + 1].Insert();
            }

            row = row + 8;
            // TODO: Confirmar com Caximo se isto volta ou nao
            foreach (var refType in dataReferencesByAge)
            {
                row++;
                column = 1;
                foreach (var item in refType.populatedValues())
                {
                    column++; worksheet.Cells[row, column] = item;
                }
            }



            // Worksheet 1 Head Partner
            //dataChildren = reportService.getReferencesSummaryForHeadPartnerByGender(reportIntervalDTO.initialDate, reportIntervalDTO.finalDate, "Child");
            //dataAdults = reportService.getReferencesSummaryForHeadPartnerByGender(reportIntervalDTO.initialDate, reportIntervalDTO.finalDate, "Adult");
            dataChildren = referenceServiceService.getActivistReferencesForChildReportSummaryChiefPartner(reportIntervalDTO.initialDate, reportIntervalDTO.finalDate);
            dataAdults = referenceServiceService.getActivistReferencesForAdultReportSummaryChiefPartner(reportIntervalDTO.initialDate, reportIntervalDTO.finalDate);

            worksheet = excelApp.Worksheets[1];
            worksheet.Range["P1:AC1"].Value = "Período de " + reportIntervalDTO.initialDate.ToShortDateString() + " à " + reportIntervalDTO.finalDate.ToShortDateString();
            worksheet.Range["A1:H1"].Value = "Nome da Organização: " + site.SiteName;
            worksheet.Range["I1:O1"].Value = "Distrito: " + site.orgUnit.Name;
            //worksheet.Range["P1:T1"].Value = "Mês: " + reportIntervalDTO.finalDate.Month;
            //worksheet.Range["U1:AC1"].Value = "Ano: " + reportIntervalDTO.finalDate.Year;

            row = 6;
            column = 0;
            foreach (var d in dataChildren)
            {
                row++;
                column = 0;
                foreach (var item in d.PopulatedValues())
                {
                    column++;
                    worksheet.Cells[row, column] = item;
                }
                worksheet.Rows[row + 1].Insert();
            }

            row = row + 5;
            foreach (var d in dataAdults)
            {
                row++;
                column = 0;
                foreach (var item in d.PopulatedValues())
                {
                    column++; worksheet.Cells[row, column] = item;
                }
                worksheet.Rows[row + 1].Insert();
            }

            row = row + 8;
            foreach (var refType in dataReferencesByAge)
            {
                row++;
                column = 1;
                foreach (var item in refType.values)
                {
                    column++; worksheet.Cells[row, column] = item;
                }
            }



            var temporaryFilepath = Path.ChangeExtension(Path.GetTempFileName(), ".xlsx");
            workbook.SaveAs(temporaryFilepath);
            workbook.Close(0);
            excelApp.Quit();
            DownloadXlsxFile(temporaryFilepath);
            return RedirectToAction("ReferencesSummaryReport");
        }


        /*****************************************
         * CounterReferencesSummaryReport
         *****************************************/

        public ActionResult CounterReferencesSummaryReport()
        {
            ReportIntervalViewModel reportIntervalDTO = new ReportIntervalViewModel();
            reportIntervalDTO.initialDate = new DateTime(DateTime.Now.Year, 1, 1);
            reportIntervalDTO.finalDate = DateTime.Now;
            return View(reportIntervalDTO);
        }

        [HttpPost, ActionName("download-counter-references-summary-report")]
        public ActionResult DownloadCounterReferencesSummaryReport([Bind(Include = "initialDate, finalDate")] ReportIntervalViewModel reportIntervalDTO)
        {
            var fileName = @"~\Templates\counter_references_summary.xlsx";
            Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
            Excel._Workbook workbook = excelApp.Workbooks.Open(Server.MapPath(fileName));
            Excel._Worksheet worksheet = excelApp.ActiveSheet;

            Site site = siteService.findById(1);

            // Worksheet 2 Partner

            var row = 6;
            var column = 0;

            List<CounterReferencesSummaryByRefTypeReportDTO> dataChildren = referenceServiceService.getCounterReferencesForChildReportSummaryPartner(reportIntervalDTO.initialDate, reportIntervalDTO.finalDate);
            List<CounterReferencesSummaryByRefTypeReportDTO> dataAdults = referenceServiceService.getCounterReferencesForAdultReportSummaryPartner(reportIntervalDTO.initialDate, reportIntervalDTO.finalDate);

            worksheet = excelApp.Worksheets[2];
            worksheet.Range["P1:AC1"].Value = "Período de " + reportIntervalDTO.initialDate.ToShortDateString() + " à " + reportIntervalDTO.finalDate.ToShortDateString();
            worksheet.Range["A1:H1"].Value = "Nome da Organização: " + site.SiteName;
            worksheet.Range["I1:O1"].Value = "Distrito: " + site.orgUnit.Name;
            //worksheet.Range["P1:T1"].Value = "Mês: " + reportIntervalDTO.finalDate.Month;
            //worksheet.Range["U1:AC1"].Value = "Ano: " + reportIntervalDTO.finalDate.Year;

            foreach (var d in dataChildren)
            {
                row++;
                column = 0;
                foreach (var item in d.PopulatedValues())
                {
                    column++;
                    worksheet.Cells[row, column] = item;
                }
                worksheet.Rows[row + 1].Insert();
            }

            row = row + 5;
            foreach (var d in dataAdults)
            {
                row++;
                column = 0;
                foreach (var item in d.PopulatedValues())
                {
                    column++; worksheet.Cells[row, column] = item;
                }
                worksheet.Rows[row + 1].Insert();
            }

            // Worksheet 1 Head Partner
            dataChildren = referenceServiceService.getCounterReferencesForChildReportSummaryChiefPartner(reportIntervalDTO.initialDate, reportIntervalDTO.finalDate);
            dataAdults = referenceServiceService.getCounterReferencesForAdultReportSummaryChiefPartner(reportIntervalDTO.initialDate, reportIntervalDTO.finalDate);

            worksheet = excelApp.Worksheets[1];
            worksheet.Range["P1:AC1"].Value = "Período de " + reportIntervalDTO.initialDate.ToShortDateString() + " à " + reportIntervalDTO.finalDate.ToShortDateString();
            worksheet.Range["A1:H1"].Value = "Nome da Organização: " + site.SiteName;
            worksheet.Range["I1:O1"].Value = "Distrito: " + site.orgUnit.Name;
            //worksheet.Range["P1:T1"].Value = "Mês: " + reportIntervalDTO.finalDate.Month;
            //worksheet.Range["U1:AC1"].Value = "Ano: " + reportIntervalDTO.finalDate.Year;

            row = 6;
            column = 0;
            foreach (var d in dataChildren)
            {
                row++;
                column = 0;
                foreach (var item in d.PopulatedValues())
                {
                    column++;
                    worksheet.Cells[row, column] = item;
                }
                worksheet.Rows[row + 1].Insert();
            }

            row = row + 5;
            foreach (var d in dataAdults)
            {
                row++;
                column = 0;
                foreach (var item in d.PopulatedValues())
                {
                    column++; worksheet.Cells[row, column] = item;
                }
                worksheet.Rows[row + 1].Insert();
            }

            var temporaryFilepath = Path.ChangeExtension(Path.GetTempFileName(), ".xlsx");
            workbook.SaveAs(temporaryFilepath);
            workbook.Close(0);
            excelApp.Quit();
            DownloadXlsxFile(temporaryFilepath);
            return RedirectToAction("CounterReferencesSummaryReport");
        }



        /*****************************************
         * MonthlyProgressReport
         *****************************************/

        public ActionResult MonthlyProgressReport()
        {
            ReportIntervalViewModel progressReport = new ReportIntervalViewModel();
            progressReport.initialDate = new DateTime(DateTime.Now.Year, 1, 1);
            progressReport.finalDate = DateTime.Now;
            return View(progressReport);
        }

        [HttpPost, ActionName("download-monthly-progress-report")]
        public ActionResult DownloadMonthlyProgressReport([Bind(Include = "initialDate, finalDate")] ReportIntervalViewModel reportIntervalDTO)
        {
            DateTime firstDayOfLastSelectedMonth = new DateTime(reportIntervalDTO.finalDate.Year, reportIntervalDTO.finalDate.Month, 21);
            firstDayOfLastSelectedMonth = firstDayOfLastSelectedMonth.AddMonths(-1);
            DateTime lastDayOfLastSelectedMonth = firstDayOfLastSelectedMonth.AddMonths(1).AddDays(-1);

            DateTime firstDayForComulative = DateTime.Now;
            int fiscalYear = DateTime.Now.Year;

            if (reportIntervalDTO.finalDate.CompareTo(new DateTime(reportIntervalDTO.finalDate.Year, 9, 20)) > 0)
            {
                firstDayForComulative = new DateTime(reportIntervalDTO.finalDate.Year, 9, 21);
                fiscalYear = fiscalYear + 1;
            }
            else
            {
                firstDayForComulative = new DateTime(reportIntervalDTO.finalDate.Year - 1, 9, 21);
                fiscalYear = reportIntervalDTO.finalDate.Year;
            }

            Site site = siteService.findById(1);

            List<int> resultsListCurrMonth = siteService.getMonthlyProgressReport(firstDayOfLastSelectedMonth, lastDayOfLastSelectedMonth, 0, 0, site.SiteID);
            List<int> resultsListSelectedMonths = siteService.getMonthlyProgressReport(firstDayForComulative, reportIntervalDTO.finalDate, 0, 0, site.SiteID);
            List<SiteGoal> SiteGoals = db.SiteGoals.Where(x => x.Site.SiteID == 1 && x.GoalDate.Year == fiscalYear).ToList();

            var fileName = @"~\Templates\monthly-progress-report.xlsx";
            Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
            Excel._Workbook workbook = excelApp.Workbooks.Open(Server.MapPath(fileName));
            Excel._Worksheet worksheet = excelApp.ActiveSheet;

            worksheet.Range["B3:C3"].Value = "Província: " + site.orgUnit.Parent.Name;
            worksheet.Range["D3:F3"].Value = "Distrito: " + site.orgUnit.Name;
            worksheet.Range["G3:I3"].Value = "Nome da OCB: " + site.SiteName;
            worksheet.Range["C4"].Value = reportIntervalDTO.initialDate;
            worksheet.Range["C5"].Value = reportIntervalDTO.finalDate;


            int row = 8;
            for (int i = 0; i < resultsListCurrMonth.Count; i++)
            {
                row++;

                if (row == 20 || row == 31)
                {
                    row++;
                }

                if (row == 9 || row == 19)
                {
                    continue;
                }

                worksheet.Cells[row, "C"] = resultsListCurrMonth[i];
                worksheet.Cells[row, "D"] = resultsListSelectedMonths[i];
            }

            row = 8;

            for (int i = 0; i < SiteGoals.Count; i++)
            {
                row++;

                if (row == 10)
                {
                    row += 3;
                }

                if (row == 17)
                {
                    row += 2;
                }

                worksheet.Cells[row, "E"] = SiteGoals[i].GoalNumber;
                worksheet.Cells[row, "G"] = SiteGoals[i].SitePerformanceComment;

                if (i == 5)
                    break;
            }

            var temporaryFilepath = Path.ChangeExtension(Path.GetTempFileName(), ".xlsx");
            workbook.SaveAs(temporaryFilepath);
            workbook.Close(0);
            excelApp.Quit();
            DownloadXlsxFile(temporaryFilepath);
            return RedirectToAction("MonthlyProgressReport");
        }

        /*****************************************
         * InitialAndLastHIVStatusReport
         *****************************************/

        public ActionResult InitialAndLastHIVStatusReport()
        {
            ChildStatusViewModel csrd = new ChildStatusViewModel();
            csrd.initialDate = new DateTime(DateTime.Now.Year, 1, 1);
            csrd.finalDate = DateTime.Now;

            List<SelectListItem> PartnerList = new List<SelectListItem>();
            PartnerList.Insert(0, new SelectListItem { Text = "", Value = "" });
            PartnerList.AddRange(new SelectList(from p in db.Partners where p.CollaboratorRole.Code == "HEAD" orderby p.Name select p, "PartnerID", "Name"));
            ViewBag.PartnerID = PartnerList;

            return View(csrd);
        }

        [HttpPost, ActionName("download-initial-and-last-hivstatus-report")]
        public ActionResult DownloadInitialAndLastHIVStatusReport([Bind(Include = "initialDate, finalDate, PartnerID")] ChildStatusViewModel csrd)
        {
            var fileName = @"~\Templates\HIVStatusReport\initial-and-last-hivstatus-report.xlsx";
            List<InitialAndFinalHIVStatusDTO> data = hivStatusQueryService.getInitialAndFinalHIVStatus(csrd.initialDate, csrd.finalDate, csrd.PartnerID);
            Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
            Excel._Workbook workbook = excelApp.Workbooks.Open(Server.MapPath(fileName));
            Excel._Worksheet worksheet = excelApp.ActiveSheet;

            Site site = siteService.findById(1);

            worksheet.Range["B3:C3"].Value = "Província: " + site.orgUnit.Parent.Name;
            worksheet.Range["D3:H3"].Value = "Distrito: " + site.orgUnit.Name;
            worksheet.Range["I3:M3"].Value = "Nome da OCB: " + site.SiteName;

            int column, row = 8;
            foreach (var d in data)
            {
                row++; column = 1;
                foreach (var item in d.populatedValues())
                {
                    column++;
                    worksheet.Cells[row, column] = item;
                }
            }

            List<SelectListItem> PartnerList = new List<SelectListItem>();
            PartnerList.Insert(0, new SelectListItem { Text = "", Value = "" });
            PartnerList.AddRange(new SelectList(from p in db.Partners where p.CollaboratorRole.Code == "HEAD" orderby p.Name select p, "PartnerID", "Name"));
            ViewBag.PartnerID = PartnerList;

            var temporaryFilepath = Path.ChangeExtension(Path.GetTempFileName(), ".xlsx");
            workbook.SaveAs(temporaryFilepath);
            workbook.Close(0);
            excelApp.Quit();
            DownloadXlsxFile(temporaryFilepath);
            return RedirectToAction("InitialAndLastHIVStatusReport");
        }

        /*****************************************
         * HIVStatusChangesReport
         *****************************************/

        public ActionResult HIVStatusChangesReport()
        {
            ChildStatusViewModel csrd = new ChildStatusViewModel();
            csrd.initialDate = new DateTime(DateTime.Now.Year, 1, 1);
            csrd.finalDate = DateTime.Now;

            List<SelectListItem> PartnerList = new List<SelectListItem>();
            PartnerList.Insert(0, new SelectListItem { Text = "", Value = "" });
            PartnerList.AddRange(new SelectList(from p in db.Partners where p.CollaboratorRole.Code == "HEAD" orderby p.Name select p, "PartnerID", "Name"));
            ViewBag.PartnerID = PartnerList;

            List<SelectListItem> initialHivStatusList = new List<SelectListItem>();
            initialHivStatusList.Insert(0, new SelectListItem { Text = "HIV Negativo", Value = "HIV Negativo" });
            initialHivStatusList.Insert(1, new SelectListItem { Text = "HIV Positivo em Tratamento", Value = "HIV Positivo em Tratamento" });
            initialHivStatusList.Insert(2, new SelectListItem { Text = "HIV Positivo sem Tratamento", Value = "HIV Positivo sem Tratamento" });
            initialHivStatusList.Insert(3, new SelectListItem { Text = "Estado HIV Não revelado", Value = "Estado HIV Não revelado" });
            initialHivStatusList.Insert(4, new SelectListItem { Text = "Estado HIV Desconhecido", Value = "Estado HIV Desconhecido" });
            ViewBag.initialHIVState = initialHivStatusList;

            List<SelectListItem> finalHivStatusList = new List<SelectListItem>();
            finalHivStatusList.AddRange(initialHivStatusList);
            ViewBag.finalHIVState = finalHivStatusList;

            return View(csrd);
        }

        [HttpPost, ActionName("download-hivstatus-changes-report")]
        public ActionResult DownloadHIVStatusChangesReport([Bind(Include = "initialDate, finalDate, PartnerID, InitialHIVState, FinalHIVState")] ChildStatusViewModel csrd)
        {
            var fileName = @"~\Templates\HIVStatusReport\hivstatus-changes-report.xlsx";
            List<HIVStatusChangesDTO> data = hivStatusQueryService.getHIVStatusChanges(csrd.initialDate, csrd.finalDate, csrd.PartnerID, csrd.InitialHIVState, csrd.FinalHIVState);
            Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
            Excel._Workbook workbook = excelApp.Workbooks.Open(Server.MapPath(fileName));
            Excel._Worksheet worksheet = excelApp.ActiveSheet;



            Site site = siteService.findById(1);

            worksheet.Range["B3:C3"].Value = "Província: " + site.orgUnit.Parent.Name;
            worksheet.Range["D3:F3"].Value = "Distrito: " + site.orgUnit.Name;
            worksheet.Range["G3:K3"].Value = "Nome da OCB: " + site.SiteName;


            int column, row = 5;
            foreach (var d in data)
            {
                row++; column = 1;
                foreach (var item in d.populatedValues())
                {
                    column++;
                    worksheet.Cells[row, column] = item;
                }
            }

            List<SelectListItem> PartnerList = new List<SelectListItem>();
            PartnerList.Insert(0, new SelectListItem { Text = "", Value = "" });
            PartnerList.AddRange(new SelectList(from p in db.Partners where p.CollaboratorRole.Code == "HEAD" orderby p.Name select p, "PartnerID", "Name"));
            ViewBag.PartnerID = PartnerList;

            List<SelectListItem> initialHivStatusList = new List<SelectListItem>();
            initialHivStatusList.Insert(0, new SelectListItem { Text = "HIV Negativo", Value = "HIV Negativo" });
            initialHivStatusList.Insert(1, new SelectListItem { Text = "HIV Positivo em Tratamento", Value = "HIV Positivo em Tratamento" });
            initialHivStatusList.Insert(2, new SelectListItem { Text = "HIV Positivo sem Tratamento", Value = "HIV Positivo sem Tratamento" });
            initialHivStatusList.Insert(3, new SelectListItem { Text = "Estado HIV Não revelado", Value = "Estado HIV Não revelado" });
            initialHivStatusList.Insert(4, new SelectListItem { Text = "Estado HIV Desconhecido", Value = "Estado HIV Desconhecido" });
            ViewBag.initialHIVState = initialHivStatusList;

            List<SelectListItem> finalHivStatusList = new List<SelectListItem>();
            finalHivStatusList.AddRange(initialHivStatusList);
            ViewBag.finalHIVState = finalHivStatusList;

            var temporaryFilepath = Path.ChangeExtension(Path.GetTempFileName(), ".xlsx");
            workbook.SaveAs(temporaryFilepath);
            workbook.Close(0);
            excelApp.Quit();
            DownloadXlsxFile(temporaryFilepath);
            return RedirectToAction("HIVStatusChangesReport");
        }

        /*****************************************
         * BeneficiariesAndHIVStatusReport
         *****************************************/

        public ActionResult BeneficiariesAndHIVStatusReport()
        {
            ChildStatusViewModel csrd = new ChildStatusViewModel();
            csrd.initialDate = new DateTime(DateTime.Now.Year, 1, 1);
            csrd.finalDate = DateTime.Now;

            List<SelectListItem> initialHivStatusList = new List<SelectListItem>();
            initialHivStatusList.Insert(0, new SelectListItem { Text = "", Value = "" });
            initialHivStatusList.Insert(1, new SelectListItem { Text = "HIV Negativo", Value = "HIV Negativo" });
            initialHivStatusList.Insert(2, new SelectListItem { Text = "HIV Positivo em Tratamento", Value = "HIV Positivo em Tratamento" });
            initialHivStatusList.Insert(3, new SelectListItem { Text = "HIV Positivo sem Tratamento", Value = "HIV Positivo sem Tratamento" });
            initialHivStatusList.Insert(4, new SelectListItem { Text = "Estado HIV Não revelado", Value = "Estado HIV Não revelado" });
            initialHivStatusList.Insert(5, new SelectListItem { Text = "Estado HIV Desconhecido", Value = "Estado HIV Desconhecido" });
            ViewBag.InitialHIVState = initialHivStatusList;

            List<SelectListItem> PartnerList = new List<SelectListItem>();
            PartnerList.Insert(0, new SelectListItem { Text = "", Value = "" });
            PartnerList.AddRange(new SelectList(from p in db.Partners where p.CollaboratorRole.Code == "ACTIVIST" orderby p.Name select p, "PartnerID", "Name"));
            ViewBag.PartnerID = PartnerList;

            return View(csrd);
        }

        [HttpPost, ActionName("download-beneficiaries-and-hivstatus-report")]
        public ActionResult DownloadBeneficiariesAndHIVStatusReport([Bind(Include = "InitialHIVState, PartnerID")] ChildStatusViewModel csrd)
        {
            var fileName = @"~\Templates\HIVStatusReport\beneficiaries-and-hivstatus-report.xlsx";
            List<BeneficiariesAndHIVStatusDTO> data = hivStatusQueryService.getBeneficiariesAndHIVStatus(csrd.InitialHIVState, csrd.PartnerID);
            Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
            Excel._Workbook workbook = excelApp.Workbooks.Open(Server.MapPath(fileName));
            Excel._Worksheet worksheet = excelApp.ActiveSheet;

            Site site = siteService.findById(1);

            worksheet.Range["B3:C3"].Value = "Província: " + site.orgUnit.Parent.Name;
            worksheet.Range["D3:F3"].Value = "Distrito: " + site.orgUnit.Name;
            worksheet.Range["G3:K3"].Value = "Nome da OCB: " + site.SiteName;


            int column, row = 5;
            foreach (var d in data)
            {
                row++; column = 1;
                foreach (var item in d.populatedValues())
                {
                    column++;
                    worksheet.Cells[row, column] = item;
                }
            }

            List<SelectListItem> initialHivStatusList = new List<SelectListItem>();
            initialHivStatusList.Insert(0, new SelectListItem { Text = "", Value = "" });
            initialHivStatusList.Insert(1, new SelectListItem { Text = "HIV Negativo", Value = "HIV Negativo" });
            initialHivStatusList.Insert(2, new SelectListItem { Text = "HIV Positivo em Tratamento", Value = "HIV Positivo em Tratamento" });
            initialHivStatusList.Insert(3, new SelectListItem { Text = "HIV Positivo sem Tratamento", Value = "HIV Positivo sem Tratamento" });
            initialHivStatusList.Insert(4, new SelectListItem { Text = "Estado HIV Não revelado", Value = "Estado HIV Não revelado" });
            initialHivStatusList.Insert(5, new SelectListItem { Text = "Estado HIV Desconhecido", Value = "Estado HIV Desconhecido" });
            ViewBag.InitialHIVState = initialHivStatusList;

            List<SelectListItem> PartnerList = new List<SelectListItem>();
            PartnerList.Insert(0, new SelectListItem { Text = "", Value = "" });
            PartnerList.AddRange(new SelectList(from p in db.Partners where p.CollaboratorRole.Code == "ACTIVIST" orderby p.Name select p, "PartnerID", "Name"));
            ViewBag.PartnerID = PartnerList;

            var temporaryFilepath = Path.ChangeExtension(Path.GetTempFileName(), ".xlsx");
            workbook.SaveAs(temporaryFilepath);
            workbook.Close(0);
            excelApp.Quit();
            DownloadXlsxFile(temporaryFilepath);
            return RedirectToAction("BeneficiariesAndHIVStatusReport");
        }


        /*****************************************
         * UserRecordCountingReport
         *****************************************/

        public ActionResult UserRecordCountingReport()
        {
            ReportIntervalViewModel reportIntervalDTO = new ReportIntervalViewModel();
            reportIntervalDTO.initialDate = new DateTime(DateTime.Now.Year, 1, 1);
            reportIntervalDTO.finalDate = DateTime.Now;
            return View(reportIntervalDTO);
        }

        [HttpPost, ActionName("download-user-record-counting-report")]
        public ActionResult UserRecordCountingReport([Bind(Include = "initialDate, finalDate")] ReportIntervalViewModel reportIntervalDTO)
        {
            var fileName = @"~\Templates\user-record-counting-report.xlsx";
            Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
            Excel._Workbook workbook = excelApp.Workbooks.Open(Server.MapPath(fileName));
            Excel._Worksheet worksheet = excelApp.ActiveSheet;

            var row = 4; var column = 2;
            List<EFDataAccess.DTO.UserRecordCountingReportDTO> data = userService.getUserRecordCountingReport(reportIntervalDTO.initialDate, reportIntervalDTO.finalDate);
            foreach (var d in data)
            {
                row++; column = 2;
                foreach (var item in d.populatedValues())
                {
                    column++;
                    worksheet.Cells[row, column] = item;
                }
            }

            var temporaryFilepath = Path.ChangeExtension(Path.GetTempFileName(), ".xlsx");
            workbook.SaveAs(temporaryFilepath);
            workbook.Close(0);
            excelApp.Quit();
            DownloadXlsxFile(temporaryFilepath);
            return RedirectToAction("UserRecordCountingReport");
        }


        /*****************************************
         * GlobalReport
         *****************************************/

        public ActionResult GlobalReport()
        {
            ReportIntervalViewModel reportIntervalDTO = new ReportIntervalViewModel();
            reportIntervalDTO.initialDate = new DateTime(DateTime.Now.Year, 1, 1);
            reportIntervalDTO.finalDate = DateTime.Now;
            return View(reportIntervalDTO);
        }


        [HttpPost, ActionName("download-global-report")]
        public ActionResult GlobalReport([Bind(Include = "initialDate, finalDate")] ReportIntervalViewModel reportIntervalDTO)
        {
            var fileName = @"~\Templates\global-report.xlsx";
            Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
            Excel._Workbook workbook = excelApp.Workbooks.Open(Server.MapPath(fileName));
            Excel._Worksheet worksheet = excelApp.ActiveSheet;

            var row = 3;
            var column = 0;
            List<GlobalReportDTO> data = reportService.getGlobalReport(reportIntervalDTO.initialDate, reportIntervalDTO.finalDate);

            foreach (var refType in data)
            {
                row++; column = 1;
                foreach (var item in refType.populatedValues())
                {
                    column++;
                    worksheet.Cells[row, column] = item;
                }
            }

            var temporaryFilepath = Path.ChangeExtension(Path.GetTempFileName(), ".xlsx");
            workbook.SaveAs(temporaryFilepath);
            workbook.Close(0);
            excelApp.Quit();
            DownloadXlsxFile(temporaryFilepath);
            return View("GlobalReport");
        }


        /****************************************************************
         * BeneficiariesWithoutServicesReport
         ****************************************************************/

        public ActionResult BeneficiariesWithoutServicesReport()
        {
            ReportIntervalViewModel reportIntervalDTO = new ReportIntervalViewModel();
            reportIntervalDTO.initialDate = new DateTime(DateTime.Now.Year, 1, 1);
            reportIntervalDTO.finalDate = DateTime.Now;

            return View(reportIntervalDTO);
        }

        [HttpPost, ActionName("download-beneficiaries-without-services-report")]
        public ActionResult BeneficiariesWithoutServicesReport([Bind(Include = "initialDate, finalDate")] ReportIntervalViewModel reportIntervalDTO)
        {
            var fileName = @"~\Templates\beneficiaries-without-services-report.xlsx";
            Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
            Excel._Workbook workbook = excelApp.Workbooks.Open(Server.MapPath(fileName));
            Excel._Worksheet worksheet = excelApp.ActiveSheet;

            List<BeneficiariesWithoutServicesDTO> data = reportService.getBeneficiariesWithoutServices(reportIntervalDTO.initialDate, reportIntervalDTO.finalDate);

            Site site = siteService.findById(1);

            worksheet.Range["B3:C3"].Value = "Província: " + site.orgUnit.Parent.Name;
            worksheet.Range["D3:F3"].Value = "Distrito: " + site.orgUnit.Name;
            worksheet.Range["G3:J3"].Value = "Nome da OCB: " + site.SiteName;
            worksheet.Range["C4"].Value = reportIntervalDTO.initialDate;
            worksheet.Range["C5"].Value = reportIntervalDTO.finalDate;

            var row = 7;
            var column = 0;
            foreach (var refType in data)
            {
                row++; column = 1;
                foreach (var item in refType.populatedValues())
                {
                    column++;
                    worksheet.Cells[row, column] = item;
                }

            }

            var temporaryFilepath = Path.ChangeExtension(Path.GetTempFileName(), ".xlsx");
            workbook.SaveAs(temporaryFilepath);
            workbook.Close(0);
            excelApp.Quit();
            DownloadXlsxFile(temporaryFilepath);

            return View("BeneficiariesWithoutServicesReport");
        }



        /****************************************************************
         * BeneficiariesByStatusReport
         ****************************************************************/

        public ActionResult BeneficiariesByStatusReport()
        {
            ChildStatusViewModel csrd = new ChildStatusViewModel();
            csrd.initialDate = new DateTime(DateTime.Now.Year, 1, 1);
            csrd.finalDate = DateTime.Now;

            List<SelectListItem> StatusList = new List<SelectListItem>();
            StatusList.Insert(0, new SelectListItem { Text = "", Value = "" });
            StatusList.AddRange(new SelectList(from p in db.ChildStatus select p, "StatusID", "Description"));
            ViewBag.StatusID = StatusList;

            return View(csrd);
        }

        [HttpPost, ActionName("download-beneficiaries-by-status-report")]
        public ActionResult BeneficiariesByStatusReport([Bind(Include = "initialDate, finalDate, StatusID")] ChildStatusViewModel csrd)
        {
            var fileName = @"~\Templates\beneficiaries-by-status-report.xlsx";
            Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
            Excel._Workbook workbook = excelApp.Workbooks.Open(Server.MapPath(fileName));
            Excel._Worksheet worksheet = excelApp.ActiveSheet;

            List<BeneficiaryStatusDTO> data = reportService.getBeneficiariesByStatus(csrd.initialDate, csrd.finalDate, csrd.StatusID);

            Site site = siteService.findById(1);

            worksheet.Range["B3:C3"].Value = "Província: " + site.orgUnit.Parent.Name;
            worksheet.Range["D3:F3"].Value = "Distrito: " + site.orgUnit.Name;
            worksheet.Range["G3:J3"].Value = "Nome da OCB: " + site.SiteName;
            worksheet.Range["C4"].Value = csrd.initialDate;
            worksheet.Range["C5"].Value = csrd.finalDate;

            var row = 7;
            var column = 0;
            foreach (var refType in data)
            {
                row++; column = 1;
                foreach (var item in refType.populatedValues())
                {
                    column++;
                    worksheet.Cells[row, column] = item;
                }

            }

            var temporaryFilepath = Path.ChangeExtension(Path.GetTempFileName(), ".xlsx");
            workbook.SaveAs(temporaryFilepath);
            workbook.Close(0);
            excelApp.Quit();
            DownloadXlsxFile(temporaryFilepath);

            return View("BeneficiariesByStatusReport");
        }


        /***************************************************************
         * BeforeAndActualBeneficiaryStatusReport
         ***************************************************************/

        [Allow(Roles = REPORT_VIEWERS)]
        public ActionResult BeforeAndActualBeneficiaryStatusReport()
        {
            ReportIntervalViewModel reportIntervalDTO = new ReportIntervalViewModel();
            reportIntervalDTO.initialDate = new DateTime(DateTime.Now.Year, 1, 1);
            reportIntervalDTO.finalDate = DateTime.Now;

            return View(reportIntervalDTO);
        }

        [HttpPost, ActionName("download-before-and-actual-beneficiary-status-report")]
        public ActionResult DownloadBeforeAndActualBeneficiaryStatusReport([Bind(Include = "initialDate, finalDate")] ReportIntervalViewModel reportIntervalDTO)
        {

            var fileName = @"~\Templates\before-and-actual-beneficiary-status-report.xlsx";
            Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
            Excel._Workbook workbook = excelApp.Workbooks.Open(Server.MapPath(fileName));
            Excel._Worksheet worksheet = excelApp.ActiveSheet;

            Site site = siteService.findById(1);

            worksheet.Range["B3:C3"].Value = "Província: " + site.orgUnit.Parent.Name;
            worksheet.Range["D3:E3"].Value = "Distrito: " + site.orgUnit.Name;
            worksheet.Range["F3:I3"].Value = "Nome da OCB: " + site.SiteName;

            var row = 5;
            var column = 0;
            List<BeforeAndActualChildStatusReportDTO> data = reportService.getBeforeAndActualBeneficiaryStatusReport(reportIntervalDTO.initialDate, reportIntervalDTO.finalDate);

            foreach (var refType in data)
            {
                row++; column = 1;
                foreach (var item in refType.populatedValues())
                {
                    column++;
                    worksheet.Cells[row, column] = item;
                }
            }

            var temporaryFilepath = Path.ChangeExtension(Path.GetTempFileName(), ".xlsx");
            workbook.SaveAs(temporaryFilepath);
            workbook.Close(0);
            excelApp.Quit();
            DownloadXlsxFile(temporaryFilepath);
            //SetPartnerAndChildAndStatusFiltersOnViewBag(0, 0, 0);
            return RedirectToAction("BeforeAndActualBeneficiaryStatusReport", "Reports");
        }


        /****************************************************************
         * BeneficiariesWithNotInTARVandUnknownHIVStatus
         ****************************************************************/

        public ActionResult BeneficiariesWithNotInTARVandUnknownHIVStatus()
        {
            ReportIntervalViewModel reportIntervalDTO = new ReportIntervalViewModel();
            reportIntervalDTO.initialDate = new DateTime(DateTime.Now.Year, 1, 1);
            reportIntervalDTO.finalDate = DateTime.Now;

            return View(reportIntervalDTO);
        }

        [HttpPost, ActionName("download-beneficiaries-with-not-in-tarv-and-unknown-hiv-status")]
        public ActionResult BeneficiariesWithNotInTARVandUnknownHIVStatus([Bind(Include = "initialDate, finalDate")] ReportIntervalViewModel reportIntervalDTO)
        {
            var fileName = @"~\Templates\beneficiaries-with-not-in-tarv-and-unknown-hiv-status.xlsx";
            Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
            Excel._Workbook workbook = excelApp.Workbooks.Open(Server.MapPath(fileName));
            Excel._Worksheet worksheet = excelApp.ActiveSheet;

            List<BeneficiaryHIVStatusDTO> data = reportService.getBeneficiariesWithNotInTARVandUnknownHIVStatus(reportIntervalDTO.initialDate, reportIntervalDTO.finalDate);

            Site site = siteService.findById(1);

            worksheet.Range["B3:C3"].Value = "Província: " + site.orgUnit.Parent.Name;
            worksheet.Range["D3:F3"].Value = "Distrito: " + site.orgUnit.Name;
            worksheet.Range["G3:J3"].Value = "Nome da OCB: " + site.SiteName;
            worksheet.Range["C4"].Value = reportIntervalDTO.initialDate;
            worksheet.Range["C5"].Value = reportIntervalDTO.finalDate;

            var row = 7;
            var column = 0;
            foreach (var refType in data)
            {
                row++; column = 1;
                foreach (var item in refType.populatedValues())
                {
                    column++;
                    worksheet.Cells[row, column] = item;
                }

            }

            var temporaryFilepath = Path.ChangeExtension(Path.GetTempFileName(), ".xlsx");
            workbook.SaveAs(temporaryFilepath);
            workbook.Close(0);
            excelApp.Quit();
            DownloadXlsxFile(temporaryFilepath);

            return View("BeneficiariesWithNotInTARVandUnknownHIVStatus");
        }
        

        /*****************************************
         * MonthlyActiveBeneficiariesSummaryReport
         *****************************************/

        //public ActionResult MonthlyActiveBeneficiariesSummaryReport()
        //{
        //    ReportIntervalViewModel reportIntervalDTO = new ReportIntervalViewModel();
        //    reportIntervalDTO.initialDate = new DateTime(DateTime.Now.Year, 1, 1);
        //    reportIntervalDTO.finalDate = DateTime.Now;
        //    return View(reportIntervalDTO);
        //}

        //[HttpPost, ActionName("download-monthly-active-beneficiaries-summary")]
        //public ActionResult DownloadMonthlyActiveBeneficiariesSummaryReport([Bind(Include = "initialDate, finalDate")] ReportIntervalViewModel reportIntervalDTO)
        //{
        //    var fileName = @"~\Templates\monthly-active-beneficiaries-summary.xlsx";
        //    Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
        //    Excel._Workbook workbook = excelApp.Workbooks.Open(Server.MapPath(fileName));
        //    //Excel._Worksheet worksheet = excelApp.ActiveSheet;

        //    Site site = siteService.findById(1);

        //    // Worksheet 2, grouped by partner

        //    Excel._Worksheet worksheet = excelApp.Worksheets[2];
        //    worksheet.Range["AG2:AN2"].Value = "Período de " + reportIntervalDTO.initialDate.ToShortDateString() + " à " + reportIntervalDTO.finalDate.ToShortDateString();
        //    worksheet.Range["A2:B2"].Value = "Província: " + site.orgUnit.Parent.Name;
        //    worksheet.Range["C2:H2"].Value = "Distrito: " + site.orgUnit.Name;
        //    worksheet.Range["I2:S2"].Value = "Nome da OCB: " + site.SiteName;

        //    string evaluationType = "ReportData";
        //    beneficiaryService.EvaluateAllBeneficiariesServicesState(evaluationType, reportIntervalDTO.finalDate);

        //    var row = 6;
        //    var column = 0;
        //    List<MonthlyActiveBeneficiariesSummaryReportDTO> data = reportService.getMonthlyActiveBeneficiariesSummaryChiefPartner(reportIntervalDTO.initialDate, reportIntervalDTO.finalDate);

        //    foreach (var refType in data)
        //    {
        //        row++; column = 1;
        //        foreach (var item in refType.PopulatedValues())
        //        {
        //            column++;
        //            worksheet.Cells[row, column] = item;
        //        }
        //        worksheet.Rows[row + 1].Insert();
        //    }

        //    // Worksheet 1, grouped by head partner

        //    worksheet = excelApp.Worksheets[1];
        //    worksheet.Range["AG2:AN2"].Value = "Período de " + reportIntervalDTO.initialDate.ToShortDateString() + " à " + reportIntervalDTO.finalDate.ToShortDateString();
        //    worksheet.Range["A2:B2"].Value = "Província: " + site.orgUnit.Parent.Name;
        //    worksheet.Range["C2:H2"].Value = "Distrito: " + site.orgUnit.Name;
        //    worksheet.Range["I2:S2"].Value = "Nome da OCB: " + site.SiteName;

        //    row = 6;
        //    column = 0;
        //    data = reportService.getMonthlyActiveBeneficiariesSummaryChiefPartner(reportIntervalDTO.initialDate, reportIntervalDTO.finalDate);

        //    foreach (var refType in data)
        //    {
        //        row++; column = 1;
        //        foreach (var item in refType.PopulatedValues())
        //        {
        //            column++;
        //            worksheet.Cells[row, column] = item;
        //        }
        //        worksheet.Rows[row + 1].Insert();
        //    }

        //    var temporaryFilepath = Path.ChangeExtension(Path.GetTempFileName(), ".xlsx");
        //    workbook.SaveAs(temporaryFilepath);
        //    workbook.Close(0);
        //    excelApp.Quit();
        //    DownloadXlsxFile(temporaryFilepath);
        //    return RedirectToAction("MonthlyActiveBeneficiariesSummaryReport");
        //}

        /**************************************************************
         * ---------------- SUMMARY NEW REPORTS 2019 ---------------- *
         *************************************************************/

        /*****************************************
         * 2.1 New Beneficiaries Summary Report
         *****************************************/

        public ActionResult NewBeneficiariesSummaryReport()
        {
            ReportIntervalViewModel reportIntervalDTO = new ReportIntervalViewModel();
            reportIntervalDTO.initialDate = new DateTime(DateTime.Now.Year, 1, 1);
            reportIntervalDTO.finalDate = DateTime.Now;
            return View(reportIntervalDTO);
        }

        [HttpPost, ActionName("download-new-beneficiaries-summary-report")]
        public ActionResult DownloadNewBeneficiariesSummaryReport([Bind(Include = "initialDate, finalDate")] ReportIntervalViewModel reportIntervalDTO)
        {
            var fileName = @"~\Templates\SummaryReports\new-beneficiaries-summary-report.xlsx";
            Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
            Excel._Workbook workbook = excelApp.Workbooks.Open(Server.MapPath(fileName));
            //Excel._Worksheet worksheet = excelApp.ActiveSheet;

            Site site = siteService.findById(1);

            // Worksheet 2, grouped by partner

            Excel._Worksheet worksheet = excelApp.Worksheets[2];
            worksheet.Range["A2:F2"].Value = "Província: " + site.orgUnit.Parent.Name;
            worksheet.Range["G2:O2"].Value = "Distrito: " + site.orgUnit.Name;
            worksheet.Range["P2:V2"].Value = "Nome da OCB: " + site.SiteName;
            worksheet.Range["W2:AL2"].Value = "Período de " + reportIntervalDTO.initialDate.ToShortDateString() + " à " + reportIntervalDTO.finalDate.ToShortDateString();

            var row = 5;
            var column = 0;
            List<NewBeneficiariesSummaryReportDTO> dataPartner = summaryReports.getNewBeneficiariesSummaryPartner(reportIntervalDTO.initialDate, reportIntervalDTO.finalDate);

            foreach (var refType in dataPartner)
            {
                row++; column = 0;
                foreach (var item in refType.PopulatedValues())
                {
                    column++;
                    worksheet.Cells[row, column] = item;
                }
                worksheet.Rows[row + 1].Insert();
            }

            //Dictionary<string, List<string>> groupedDataPartner = reportDataServiceV2
            //    .FindReportDataByCodeAndInitialDateAndLastDate("2.1.2", reportIntervalDTO.initialDate, reportIntervalDTO.finalDate);

            //foreach (var data in groupedDataPartner)
            //{
            //    row++; column = 0;
            //    foreach (var item in data.Value)
            //    {
            //        column++;
            //        worksheet.Cells[row, column] = item;
            //    }
            //    worksheet.Rows[row + 1].Insert();
            //}

            // Worksheet 1, grouped by head partner

            worksheet = excelApp.Worksheets[1];
            worksheet.Range["A2:F2"].Value = "Província: " + site.orgUnit.Parent.Name;
            worksheet.Range["G2:O2"].Value = "Distrito: " + site.orgUnit.Name;
            worksheet.Range["P2:V2"].Value = "Nome da OCB: " + site.SiteName;
            worksheet.Range["W2:AL2"].Value = "Período de " + reportIntervalDTO.initialDate.ToShortDateString() + " à " + reportIntervalDTO.finalDate.ToShortDateString();

            row = 5;
            column = 0;
            List<NewBeneficiariesSummaryReportDTO> dataChiefPartner = summaryReports.getNewBeneficiariesSummaryChiefPartner(reportIntervalDTO.initialDate, reportIntervalDTO.finalDate);

            foreach (var refType in dataChiefPartner)
            {
                row++; column = 0;
                foreach (var item in refType.PopulatedValues())
                {
                    column++;
                    worksheet.Cells[row, column] = item;
                }
                worksheet.Rows[row + 1].Insert();
            }
            //Dictionary<string, List<string>> groupedDataHeadPartner = reportDataServiceV2
            //    .FindReportDataByCodeAndInitialDateAndLastDate("2.1.1", reportIntervalDTO.initialDate, reportIntervalDTO.finalDate);

            //foreach (var data in groupedDataHeadPartner)
            //{
            //    row++; column = 0;
            //    foreach (var item in data.Value)
            //    {
            //        column++;
            //        worksheet.Cells[row, column] = item;
            //    }
            //    worksheet.Rows[row + 1].Insert();
            //}

            var temporaryFilepath = Path.ChangeExtension(Path.GetTempFileName(), ".xlsx");
            workbook.SaveAs(temporaryFilepath);
            workbook.Close(0);
            excelApp.Quit();
            DownloadXlsxFile(temporaryFilepath);
            return RedirectToAction("NewBeneficiariesSummaryReport");
        }


        /***********************************************
         * 2.2 Monthly Active Beneficiaries Summary Report
         ***********************************************/

        public ActionResult MonthlyActiveBeneficiariesSummaryReport()
        {
            TrimesterIntervalViewModel trimesterInterval = new TrimesterIntervalViewModel();
            DateTime projectStartDate = new DateTime(2016, 09, 21);
            DateTime currentDate = DateTime.Now;
            Trimester currenteTrimester = trimesterQueryService.GetTrimesterByDate(currentDate);

            List<SelectListItem> TrimesterList = new List<SelectListItem>();
            TrimesterList.AddRange(new SelectList(db.Trimesters.Where(t => t.StartDate >= projectStartDate && t.EndDate <= currenteTrimester.EndDate)
                .OrderByDescending(x => x.Seq), "Seq", "TrimesterDescription"));
            //if (SequenceID > 0) TrimesterList.Where(x => x.Value == SequenceID.ToString()).FirstOrDefault().Selected = true;
            ViewBag.SequenceID = TrimesterList;

            return View(trimesterInterval);
        }

        [HttpPost, ActionName("download-monthly-active-beneficiaries-summary-report")]
        public ActionResult DownloadMonthlyActiveBeneficiariesSummaryReport([Bind(Include = "SequenceID")] TrimesterIntervalViewModel trimesterIntervalDTO)
        {
            var fileName = @"~\Templates\SummaryReports\monthly-active-beneficiaries-summary.xlsx";
            Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
            Excel._Workbook workbook = excelApp.Workbooks.Open(Server.MapPath(fileName));
            //Excel._Worksheet worksheet = excelApp.ActiveSheet;
            
            DateTime CurrentTrimesterInitialDate = trimesterQueryService.getTrimesterStartOrEndDateByID(trimesterIntervalDTO.SequenceID, "start");
            DateTime CurrentTrimesterLastDate = trimesterQueryService.getTrimesterStartOrEndDateByID(trimesterIntervalDTO.SequenceID, "end");

            string evaluationType = "ReportData";
            beneficiaryService.cleanEvaluatedBeneficiaries(evaluationType);
            beneficiaryService.EvaluateAllBeneficiariesServicesState(evaluationType, CurrentTrimesterLastDate);
            Site site = siteService.findById(1);

            // Worksheet 2, grouped by partner

            Excel._Worksheet worksheet = excelApp.Worksheets[2];
            worksheet.Range["A2:B2"].Value = "Província: " + site.orgUnit.Parent.Name;
            worksheet.Range["C2:H2"].Value = "Distrito: " + site.orgUnit.Name;
            worksheet.Range["I2:S2"].Value = "Nome da OCB: " + site.SiteName;
            worksheet.Range["AG2:AN2"].Value = "Período de " + CurrentTrimesterInitialDate.ToShortDateString() + " à " + CurrentTrimesterLastDate.ToShortDateString();

            var row = 6;
            var column = 0;
            List<MonthlyActiveBeneficiariesSummaryDTO> dataPartner = summaryReports.getMonthlyActiveBeneficiariesSummaryPartner(CurrentTrimesterInitialDate, CurrentTrimesterLastDate);

            foreach (var refType in dataPartner)
            {
                row++; column = 1;
                foreach (var item in refType.PopulatedValues())
                {
                    column++;
                    worksheet.Cells[row, column] = item;
                }
                worksheet.Rows[row + 1].Insert();
            }

            // Worksheet 1, grouped by head partner

            worksheet = excelApp.Worksheets[1];
            worksheet.Range["A2:B2"].Value = "Província: " + site.orgUnit.Parent.Name;
            worksheet.Range["C2:H2"].Value = "Distrito: " + site.orgUnit.Name;
            worksheet.Range["I2:S2"].Value = "Nome da OCB: " + site.SiteName;
            worksheet.Range["AG2:AN2"].Value = "Período de " + CurrentTrimesterInitialDate.ToShortDateString() + " à " + CurrentTrimesterLastDate.ToShortDateString();

            row = 6;
            column = 0;
            List<MonthlyActiveBeneficiariesSummaryDTO> dataChiefPartner = summaryReports.getMonthlyActiveBeneficiariesSummaryChiefPartner(CurrentTrimesterInitialDate, CurrentTrimesterLastDate);

            foreach (var refType in dataChiefPartner)
            {
                row++; column = 1;
                foreach (var item in refType.PopulatedValues())
                {
                    column++;
                    worksheet.Cells[row, column] = item;
                }
                worksheet.Rows[row + 1].Insert();
            }

            var temporaryFilepath = Path.ChangeExtension(Path.GetTempFileName(), ".xlsx");
            workbook.SaveAs(temporaryFilepath);
            workbook.Close(0);
            excelApp.Quit();
            DownloadXlsxFile(temporaryFilepath);
            return RedirectToAction("MonthlyActiveBeneficiariesSummaryReport");
        }

        /*****************************************************
        *  2.3 Monthly Graduated Beneficiaries Summary Report
        ******************************************************/

        public ActionResult MonthlyGraduatedBeneficiariesSummaryReport()
        {
            ReportIntervalViewModel reportIntervalDTO = new ReportIntervalViewModel();
            reportIntervalDTO.initialDate = new DateTime(DateTime.Now.Year, 1, 1);
            reportIntervalDTO.finalDate = DateTime.Now;
            return View(reportIntervalDTO);
        }

        [HttpPost, ActionName("download-monthly-graduated-beneficiaries-summary-report")]
        public ActionResult DownloadMonthlyGraduatedBeneficiariesSummaryReport([Bind(Include = "initialDate, finalDate")] ReportIntervalViewModel reportIntervalDTO)
        {
            var fileName = @"~\Templates\SummaryReports\monthly-graduated-beneficiaries-summary-report.xlsx";
            Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
            Excel._Workbook workbook = excelApp.Workbooks.Open(Server.MapPath(fileName));
            //Excel._Worksheet worksheet = excelApp.ActiveSheet;

            Site site = siteService.findById(1);

            // Worksheet 2, grouped by partner

            Excel._Worksheet worksheet = excelApp.Worksheets[2];
            worksheet.Range["V2:AE2"].Value = "Período de " + reportIntervalDTO.initialDate.ToShortDateString() + " à " + reportIntervalDTO.finalDate.ToShortDateString();
            worksheet.Range["A2:B2"].Value = "Província: " + site.orgUnit.Parent.Name;
            worksheet.Range["C2:H2"].Value = "Distrito: " + site.orgUnit.Name;
            worksheet.Range["I2:O2"].Value = "Nome da OCB: " + site.SiteName;

            var row = 7;
            var column = 0;
            // List<MonthlyActiveBeneficiariesSummaryReportDTO> data = reportService.getMonthlyActiveBeneficiariesSummaryChiefPartner(reportIntervalDTO.initialDate, reportIntervalDTO.finalDate);
            //Dictionary<string, List<string>> groupedDataPartner = reportDataServiceV2
            //    .FindReportDataByCodeAndInitialDateAndLastDate("2.5.2", reportIntervalDTO.initialDate, reportIntervalDTO.finalDate);

            //foreach (var data in groupedDataPartner)
            //{
            //    row++; column = 1;
            //    foreach (var item in data.Value)
            //    {
            //        column++;
            //        worksheet.Cells[row, column] = item;
            //    }
            //    worksheet.Rows[row + 1].Insert();
            //}

            DateTime FiscalYearDate  = new DateTime(2018, 9, 21);

            // Worksheet 1, grouped by head partner
            List<MonthlyGraduatedBeneficiariesSummaryDTO> dataPerPeriodPartner = summaryReports.getMonthlyGraduatedBeneficiariesSummaryPartner(reportIntervalDTO.initialDate, reportIntervalDTO.finalDate);
            List<MonthlyGraduatedBeneficiariesSummaryDTO> cumulativeDataPartner = summaryReports.getCumulativeGraduatedBeneficiariesSummaryPartner(FiscalYearDate, reportIntervalDTO.initialDate, reportIntervalDTO.finalDate);

            foreach (var refType in dataPerPeriodPartner)
            {
                row++; column = 1;
                foreach (var item in refType.PopulatedValues())
                {
                    column++;
                    worksheet.Cells[row, column] = item;
                }
                worksheet.Rows[row + 1].Insert();
            }

            row = row + 17;
            foreach (var refType in cumulativeDataPartner)
            {
                row++;
                column = 1;
                foreach (var item in refType.PopulatedValues())
                {
                    column++; worksheet.Cells[row, column] = item;
                }
                worksheet.Rows[row + 1].Insert();
            }

            worksheet = excelApp.Worksheets[1];
            worksheet.Range["V2:AE2"].Value = "Período de " + reportIntervalDTO.initialDate.ToShortDateString() + " à " + reportIntervalDTO.finalDate.ToShortDateString();
            worksheet.Range["A2:B2"].Value = "Província: " + site.orgUnit.Parent.Name;
            worksheet.Range["C2:H2"].Value = "Distrito: " + site.orgUnit.Name;
            worksheet.Range["I2:O2"].Value = "Nome da OCB: " + site.SiteName;

            row = 7;
            //Dictionary<string, List<string>> groupedDataHeadPartner = reportDataServiceV2
            //    .FindReportDataByCodeAndInitialDateAndLastDate("2.5.1", reportIntervalDTO.initialDate, reportIntervalDTO.finalDate);

            //foreach (var data in groupedDataHeadPartner)
            //{
            //    row++; column = 1;
            //    foreach (var item in data.Value)
            //    {
            //        column++;
            //        worksheet.Cells[row, column] = item;
            //    }
            //    worksheet.Rows[row + 1].Insert();
            //}
            List<MonthlyGraduatedBeneficiariesSummaryDTO> dataPerPeriodChiefPartner = summaryReports.getMonthlyGraduatedBeneficiariesSummaryChiefPartner(reportIntervalDTO.initialDate, reportIntervalDTO.finalDate);
            List<MonthlyGraduatedBeneficiariesSummaryDTO> cumulativeDataChiefPartner = summaryReports.getCumulativeGraduatedBeneficiariesSummaryChiefPartner(FiscalYearDate, reportIntervalDTO.initialDate, reportIntervalDTO.finalDate);

            foreach (var refType in dataPerPeriodChiefPartner)
            {
                row++; column = 1;
                foreach (var item in refType.PopulatedValues())
                {
                    column++;
                    worksheet.Cells[row, column] = item;
                }
                worksheet.Rows[row + 1].Insert();
            }

            row = row + 17;
            foreach (var refType in cumulativeDataChiefPartner)
            {
                row++;
                column = 1;
                foreach (var item in refType.PopulatedValues())
                {
                    column++; worksheet.Cells[row, column] = item;
                }
                worksheet.Rows[row + 1].Insert();
            }

            var temporaryFilepath = Path.ChangeExtension(Path.GetTempFileName(), ".xlsx");
            workbook.SaveAs(temporaryFilepath);
            workbook.Close(0);
            excelApp.Quit();
            DownloadXlsxFile(temporaryFilepath);
            return RedirectToAction("MonthlyGraduatedBeneficiariesSummaryReport");
        }

        /*********************************************************************
         * 2.4 Routine Visit Monthly Summary Report
         *********************************************************************/

        public ActionResult RoutineVisitMonthlySummaryReport()
        {
            ReportIntervalViewModel reportIntervalDTO = new ReportIntervalViewModel();
            reportIntervalDTO.initialDate = new DateTime(DateTime.Now.Year, 1, 1);
            reportIntervalDTO.finalDate = DateTime.Now;

            return View(reportIntervalDTO);
        }

        [HttpPost, ActionName("download-routine-visit-monthly-summary-report")]
        public ActionResult DownloadRoutineVisitMonthlySummaryReport([Bind(Include = "initialDate, finalDate")] ReportIntervalViewModel reportIntervalDTO)
        {
            List<RoutineVisitMonthlyDTO> data = summaryReports.getRoutineVisitMonthlySummary(reportIntervalDTO.initialDate, reportIntervalDTO.finalDate);
            Site site = siteService.findById(1);

            Excel.Application excelApp = GetExcelAppInstance();
            Excel._Workbook workbook = OpenWorkBook(excelApp, @"~\Templates\SummaryReports\routine-visit-monthly-report.xlsx");
            Excel._Worksheet activeWorksheet = OpenWorkbookActiveWorksheet(excelApp);

            activeWorksheet.Range["A1:B1"].Value = "Nome da OCB: " + site.SiteName;
            activeWorksheet.Range["C1:D1"].Value = "Província: " + site.orgUnit.Parent.Name;
            activeWorksheet.Range["E1:J1"].Value = "Distrito: " + site.orgUnit.Name;
            activeWorksheet.Range["L1:Q1"].Value = "Mês/ano: " + DateTime.Now.Month + "/" + DateTime.Now.Year;
            activeWorksheet.Range["A2:B2"].Value = "Periodo do Relatório: " + reportIntervalDTO.initialDate + " a " + reportIntervalDTO.finalDate;

            var row = 6;

            data.ForEach(listItem =>
            {
                var column = 4;

                listItem.PopulatedValues().ForEach(value =>
                {
                    activeWorksheet.Cells[row, column] = value;
                    column++;
                });

                row++;
            });

            var temporaryFilepath = Path.ChangeExtension(Path.GetTempFileName(), ".xlsx");
            workbook.SaveAs(temporaryFilepath);
            workbook.Close(0);
            excelApp.Quit();
            DownloadXlsxFile(temporaryFilepath);

            return View("RoutineVisitMonthlySummaryReport");
        }
        

        /**************************************************************
         * ---------------- LISTING NEW REPORTS 2019 ---------------- *
         *************************************************************/


        /***********************************************
         * References List With Status Report
         ***********************************************/

        public ActionResult ReferencesListWithStatusReport()
        {
            ReportIntervalViewModel reportIntervalDTO = new ReportIntervalViewModel();
            reportIntervalDTO.initialDate = new DateTime(DateTime.Now.Year, 1, 1);
            reportIntervalDTO.finalDate = DateTime.Now;
            return View(reportIntervalDTO);
        }

        [HttpPost, ActionName("download-references-list-with-status")]
        public ActionResult DownloadReferencesListWithStatusReport([Bind(Include = "initialDate, finalDate")] ReportIntervalViewModel reportIntervalDTO)
        {
            Excel.Application excelApp = GetExcelAppInstance();
            Excel._Workbook workbook = OpenWorkBook(excelApp, @"~\Templates\ListingReports\references-list-with-status.xlsx");
            Excel._Worksheet activeWorksheet = OpenWorkbookActiveWorksheet(excelApp);

            List<ReferencesListWithStatusDTO> referencesList = listingReports.getReferencesListWithStatus(reportIntervalDTO.initialDate, reportIntervalDTO.finalDate);
            var row = 4;

            referencesList.ForEach(listItem =>
            {
                var column = 1;

                listItem.populatedValues().ForEach(value =>
                {
                    activeWorksheet.Cells[row, column] = value;
                    column++;
                });
                //FIXME: activeWorksheet.Rows[row + 1].Insert();
                row++;
            });

            var temporaryFilepath = Path.ChangeExtension(Path.GetTempFileName(), ".xlsx");
            workbook.SaveAs(temporaryFilepath);
            workbook.Close(0);
            excelApp.Quit();
            DownloadXlsxFile(temporaryFilepath);

            return RedirectToAction("ReferencesListWithStatus");
        }


        /*********************************************************************
         * Beneficiaries Listing With Yellow Active Or Inactive Status Report
         ********************************************************************/

        public ActionResult BeneficiariesListingWithYellowActiveOrInactiveStatusReport()
        {

            TrimesterIntervalViewModel trimesterInterval = new TrimesterIntervalViewModel();
            DateTime projectStartDate = new DateTime(2016, 09, 21);
            DateTime currentDate = DateTime.Now;
            Trimester currenteTrimester = trimesterQueryService.GetTrimesterByDate(currentDate);

            List<SelectListItem> TrimesterList = new List<SelectListItem>();
            TrimesterList.AddRange(new SelectList(db.Trimesters.Where(t => t.StartDate >= projectStartDate && t.EndDate <= currenteTrimester.EndDate)
                .OrderByDescending(x => x.Seq), "Seq", "TrimesterDescription"));
            //if (SequenceID > 0) TrimesterList.Where(x => x.Value == SequenceID.ToString()).FirstOrDefault().Selected = true;
            ViewBag.SequenceID = TrimesterList;

            return View(trimesterInterval);
        }

        [HttpPost, ActionName("download-beneficiaries-listing-with-yellow-active-or-inactive-status")]
        public ActionResult DownloadBeneficiariesListingWithYellowActiveOrInactiveStatusReport([Bind(Include = "SequenceID")] TrimesterIntervalViewModel trimesterIntervalDTO)
        {
            Excel.Application excelApp = GetExcelAppInstance();
            Excel._Workbook workbook = OpenWorkBook(excelApp, @"~\Templates\ListingReports\beneficiaries-listing-with-yellow-active-or-inactive-status.xlsx");
            Excel._Worksheet activeWorksheet = OpenWorkbookActiveWorksheet(excelApp);

            DateTime PreviousTrimesterInitialDate = trimesterQueryService.getTrimesterStartOrEndDateByID(trimesterIntervalDTO.SequenceID - 1, "start");
            DateTime PreviousTrimesterLastDate = trimesterQueryService.getTrimesterStartOrEndDateByID(trimesterIntervalDTO.SequenceID - 1, "end");
            DateTime CurrentTrimesterInitialDate = trimesterQueryService.getTrimesterStartOrEndDateByID(trimesterIntervalDTO.SequenceID, "start");
            DateTime CurrentTrimesterLastDate = trimesterQueryService.getTrimesterStartOrEndDateByID(trimesterIntervalDTO.SequenceID, "end");

            string evaluationType = "ReportData";
            beneficiaryService.cleanEvaluatedBeneficiaries(evaluationType);
            beneficiaryService.EvaluateAllBeneficiariesServicesState(evaluationType, CurrentTrimesterLastDate);

            List <BeneficiariesListingWithYellowActiveOrInactiveStatusDTO> dataList = listingReports
                .getBeneficiariesListingWithYellowActiveOrInactiveStatus(PreviousTrimesterInitialDate, PreviousTrimesterLastDate, CurrentTrimesterInitialDate, CurrentTrimesterLastDate);
            var row = 3;

            dataList.ForEach(listItem =>
            {
                var column = 1;

                listItem.PopulatedValues().ForEach(value =>
                {
                    activeWorksheet.Cells[row, column] = value;
                    column++;
                });
                //FIXME: activeWorksheet.Rows[row + 1].Insert();
                row++;
            });

            var temporaryFilepath = Path.ChangeExtension(Path.GetTempFileName(), ".xlsx");
            workbook.SaveAs(temporaryFilepath);
            workbook.Close(0);
            excelApp.Quit();
            DownloadXlsxFile(temporaryFilepath);

            return RedirectToAction("BeneficiariesListingWithYellowActiveOrInactiveStatusReport");
        }


        /***********************************************
         * Household Support Plan Listing
         ***********************************************/

        public ActionResult HouseholdSupportPlanListingReport()
        {
            ReportIntervalViewModel reportIntervalDTO = new ReportIntervalViewModel();
            reportIntervalDTO.initialDate = new DateTime(DateTime.Now.Year, 1, 1);
            reportIntervalDTO.finalDate = DateTime.Now;
            return View(reportIntervalDTO);
        }

        [HttpPost, ActionName("download-household-support-plan-listing")]
        public ActionResult DownloadHouseholdSupportPlanListingReport([Bind(Include = "initialDate, finalDate")] ReportIntervalViewModel reportIntervalDTO)
        {
            Excel.Application excelApp = GetExcelAppInstance();
            Excel._Workbook workbook = OpenWorkBook(excelApp, @"~\Templates\ListingReports\household-support-plan-listing.xlsx");
            Excel._Worksheet activeWorksheet = OpenWorkbookActiveWorksheet(excelApp);

            List<HouseholdSupportPlanListingDTO> referencesList = listingReports.getQueryHouseholdSupportPlanListing(reportIntervalDTO.initialDate, reportIntervalDTO.finalDate);
            var row = 3;

            referencesList.ForEach(listItem =>
            {
                var column = 1;

                listItem.PopulatedValues().ForEach(value =>
                {
                    activeWorksheet.Cells[row, column] = value;
                    column++;
                });
                //FIXME: activeWorksheet.Rows[row + 1].Insert();
                row++;
            });

            var temporaryFilepath = Path.ChangeExtension(Path.GetTempFileName(), ".xlsx");
            workbook.SaveAs(temporaryFilepath);
            workbook.Close(0);
            excelApp.Quit();
            DownloadXlsxFile(temporaryFilepath);

            return RedirectToAction("ReferencesListWithStatus");
        }



        //private Excel._Worksheet PopulateWorksheets(Excel._Worksheet worksheet, List<>)
        //{

        //}


        public ActionResult GenerateMonthlyReportData()
        {
            ReportIntervalViewModel ar = new ReportIntervalViewModel();
            ar.initialDate = new DateTime(DateTime.Now.Year, 1, 1);
            ar.finalDate = DateTime.Now;
            return View(ar);
        }

        [HttpPost, ActionName("generate-monthly-reports-data")]
        public ActionResult GenerateMonthlyReportData([Bind(Include = "initialDate, finalDate")] ReportIntervalViewModel ar)
        {
            Site Site = SiteService.findById(1);
            ReportGeneratorService reportGeneratorService = new ReportGeneratorService(new UnitOfWork(), Site, ar.initialDate, ar.finalDate);


            //reportGeneratorService.AddQuery(new Query()
            //{ Code = "2.1.1.1", Name = "NewBeneficiariesAgeAndOVCType", Sql = ReportDataServiceV2.QueryNewBeneficiariesAgeAndOVCType });
            //reportGeneratorService.AddQuery(new Query()
            //{ Code = "2.1.1.2", Name = "NewQueryNewBeneficiariesHIVChildren", Sql = ReportDataServiceV2.query2 });
            //reportGeneratorService.AddQuery(new Query()
            //{ Code = "2.1.1.3", Name = "NewQueryNewBeneficiariesHIVAdults", Sql = ReportDataServiceV2.query2 });
            //reportGeneratorService.AddQuery(new Query()
            //{ Code = "2.1.1.4", Name = "QueryNewBeneficiariesAgeAndOVCType", Sql = ReportDataServiceV2.query6 });
            //reportGeneratorService.AddQuery(new Query()
            //{ Code = "2.4.1", Name = "MonthlyActiveBeneficiariesSummaryChiefPartner", Sql = ReportDataServiceV2.QueryMonthlyActiveBeneficiariesSummary });
            //reportGeneratorService.AddQuery(new Query()
            //{ Code = "2.4.2", Name = "MonthlyActiveBeneficiariesSummaryPartner", Sql = ReportDataServiceV2.QueryMonthlyActiveBeneficiariesSummary.Replace("ChiefPartner--<<ReplaceColumn<<--", "Partner") });
            //reportGeneratorService.AddQuery(new Query()
            //{ Code = "2.5.1", Name = "MonthlyGraduatedBeneficiariesSummaryChiefPartner", Sql = ReportDataServiceV2.QueryMonthlyGraduatedBeneficiariesSummary });
            //reportGeneratorService.AddQuery(new Query()
            //{ Code = "2.5.2", Name = "MonthlyGraduatedBeneficiariesSummaryPartner", Sql = ReportDataServiceV2.QueryMonthlyGraduatedBeneficiariesSummary.Replace("ChiefPartner--<<ReplaceColumn<<--", "Partner") });

            reportGeneratorService.AddQuery(new Query()
            { Code = "2.1.1", Name = "NewBeneficiariesSummaryChiefPartner", Sql = SummaryReports.QueryNewBeneficiariesSummary });

            reportGeneratorService.AddQuery(new Query()
            { Code = "2.1.2", Name = "NewBeneficiariesSummaryPartner", Sql = SummaryReports.QueryNewBeneficiariesSummary.Replace("ChiefPartner--<<ReplaceColumn<<--", "Partner") });

            reportGeneratorService.GenerateReportDataGeneric();
            reportGeneratorService.PersistReportDataAsync();
            return View("GenerateMonthlyReportData");
        }


       

    }
}
