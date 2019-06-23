using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using EFDataAccess.DTO;
using VPPS.CSI.Domain;
using EFDataAccess.Services;
using System.IO;
using System.Web.UI.WebControls;
using EFDataAccess.UOW;
using Excel = Microsoft.Office.Interop.Excel;
using EFDataAccess.Services.TrimesterServices;
using MAC.ViewModels;
using EFDataAccess.Services.ReportServices.Version_2019;

namespace MAC.Controllers
{
    [Allow(Roles = REPORT_VIEWERS)]
    public class ReportsController : BaseController
    {
        private UserService userService = new UserService(new UnitOfWork());
        private SummaryReports summaryReports = new SummaryReports(new UnitOfWork());
        private HIVStatusQueryService hivStatusQueryService = new HIVStatusQueryService(new UnitOfWork());
        private BeneficiaryService beneficiaryService = new BeneficiaryService(new UnitOfWork());
        private TrimesterQueryService trimesterQueryService = new TrimesterQueryService(new UnitOfWork());

        public ActionResult Index()
        {
            return View();
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

            // Worksheet 2, grouped by partner

            Excel._Worksheet worksheet = excelApp.Worksheets[2];
            worksheet.Range["AG2:AN2"].Value = "Período de " + CurrentTrimesterInitialDate.ToShortDateString() + " à " + CurrentTrimesterLastDate.ToShortDateString();

            var row = 6;
            var column = 0;
            List<EFDataAccess.DTO.MonthlyActiveBeneficiariesSummaryReportDTO> dataPartner = summaryReports.getMonthlyActiveBeneficiariesSummaryPartner(CurrentTrimesterInitialDate, CurrentTrimesterLastDate);

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
            worksheet.Range["AG2:AN2"].Value = "Período de " + CurrentTrimesterInitialDate.ToShortDateString() + " à " + CurrentTrimesterLastDate.ToShortDateString();

            row = 6;
            column = 0;
            List<EFDataAccess.DTO.MonthlyActiveBeneficiariesSummaryReportDTO> dataChiefPartner = summaryReports.getMonthlyActiveBeneficiariesSummaryChiefPartner(CurrentTrimesterInitialDate, CurrentTrimesterLastDate);

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

    }
}
