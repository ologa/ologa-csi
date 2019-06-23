using Quartz;
using System;
using EFDataAccess.Services;
using System.Collections.Generic;
using EFDataAccess.UOW;
using VPPS.CSI.Domain;

namespace EFDataAccess.Tasks
{
    public class ReportGenerator : IJob
    {
        // IMPORTANTE : Actualmente a implementação somente suporta queries de 20 campos !!!

        async System.Threading.Tasks.Task IJob.Execute(IJobExecutionContext context)
        {

            //ReportGeneratorService rgs = new ReportGeneratorService(
            //    new UOW.UnitOfWork(), null, (VPPS.CSI.Domain.Query)context.Get("query")
            //    , null, site, initialDate, finalDate);
            //List<List<String>> rowsList = rgs.GenerateReportData();
            //await rgs.PersistReportDataAsync(rowsList);
        }
    }
}
