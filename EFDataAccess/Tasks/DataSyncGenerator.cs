using Quartz;
using System;
using EFDataAccess.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EFDataAccess.Tasks
{
    public class DataSyncGenerator : IJob
    {
        // IMPORTANTE : Actualmente a implementação somente suporta queries de 20 campos !!!

        async Task IJob.Execute(IJobExecutionContext context)
        {
            //ReportGeneratorService rgs = new ReportGeneratorService(
            //    new UOW.UnitOfWork(), null, (VPPS.CSI.Domain.Query) context.Get("query"));
            //List<List<String>> rowsList = rgs.GenerateReportData();
            //await rgs.PersistReportDataAsync(rowsList);
        }
    }
}
