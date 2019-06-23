using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using System.Data;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.VisualBasic.FileIO;
using System.Diagnostics;
using EFDataAccess.Services;
using EFDataAccess.Logging;

public class FileImporter
{
    private static string SEPARATOR = ";";
    protected ILogger logger = new Logger();


    public DataTable ImportFromCSV(string csv_file_path)
    {
        DataTable csvData = new DataTable();
        try
        {
            using (TextFieldParser csvReader = new TextFieldParser(csv_file_path))
            {
                csvReader.SetDelimiters(new string[] { SEPARATOR });
                csvReader.HasFieldsEnclosedInQuotes = true;
                string[] colFields = csvReader.ReadFields();
                foreach (string column in colFields)
                {
                    DataColumn datecolumn = new DataColumn(column);
                    datecolumn.AllowDBNull = true;
                    csvData.Columns.Add(datecolumn);
                }
                while (!csvReader.EndOfData)
                {
                    string[] fieldData = csvReader.ReadFields();

                    //Making empty value as null
                    for (int i = 0; i < fieldData.Length; i++)
                    {
                        if (fieldData[i] == "")
                        {
                            fieldData[i] = null;
                        }
                    }
                    csvData.Rows.Add(fieldData);
                }
            }
        }
        catch (FileNotFoundException)
        {
            if (File.Exists(csv_file_path + BaseService.IMPORTED))
            { Trace.TraceInformation(DateTime.Now + " Ficheiro '" + csv_file_path + "' ja foi importado."); }
            else
            { Trace.TraceInformation(DateTime.Now + " Ficheiro '" + csv_file_path + "' nao encontrado."); }
            return new DataTable();
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Erro na importação do ficheiro '{0}'.", csv_file_path);
            return new DataTable();
        }

        return csvData;
    }
}