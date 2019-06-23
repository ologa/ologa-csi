using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.IO;
using System.Runtime.InteropServices;
using EFDataAccess.Services;

public class FileExporter
{
    private static string SEPARATOR = ";";

    public void exportToCSV <T>(string fileName, List<T> list, string CSVType)
    {
        if (list.Count == 0) { return; }

        StreamWriter sw = new StreamWriter(fileName, false);
        GridView gv = new System.Web.UI.WebControls.GridView();
        gv.DataSource = list;
        gv.DataBind();

        // Header
        foreach (TableCell header in gv.HeaderRow.Cells)
        { sw.Write(header.Text); sw.Write(SEPARATOR); }

        // Content  
        foreach (GridViewRow row in gv.Rows)
        {
            sw.Write(sw.NewLine);
            foreach (TableCell cell in row.Cells)
            {
                sw.Write(System.Net.WebUtility.HtmlDecode(cell.Text));
                sw.Write(SEPARATOR);
            }
        }
        sw.Close();

        DataSyncService dataSync = new DataSyncService();


        if (CSVType == "OCBData")
        {
            dataSync.GetService(fileName, CSVType);
        }
        else if (CSVType == "MEReport")
        {
            dataSync.GetService(fileName, CSVType);
        }
    }
    
    public void ExportToExcel<T>(string fileName, List<T> list)
    {
        Microsoft.Office.Interop.Excel.Application excelApp = new Microsoft.Office.Interop.Excel.Application();
        Microsoft.Office.Interop.Excel.Workbook workbook = excelApp.Workbooks.Add();
        workbook.Worksheets.Add();

        GridView gv = new System.Web.UI.WebControls.GridView();
        gv.DataSource = list;
        gv.DataBind();


        int r = 1; int c = 1;

        // Header
        foreach (TableCell header in gv.HeaderRow.Cells)
        { workbook.Worksheets[1].Cells[r, c] = header.Text; c++; }

        // Content
        foreach (GridViewRow row in gv.Rows)
        {
            r++; c = 0;
            foreach (TableCell cell in row.Cells)
            { c++; workbook.Worksheets[1].Cells[r, c] = cell.Text.Equals("&nbsp;") ? "" : cell.Text; }
            // FIXME: Remover esta restricao de 30 linhas
            // if (r == 30) break;
        }

        workbook.SaveAs(fileName);

        if (excelApp != null)
        {
            workbook.Close();
            excelApp.Quit();
            Marshal.FinalReleaseComObject(excelApp);
            excelApp = null;
        }
    }
}