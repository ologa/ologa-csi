using System;
using System.Collections.Generic;

namespace EFDataAccess.DTO
{
    public class AgreggatedInitialRecordSummaryReportDTO : InitialRecordSummaryReportDTO
    {
        public AgreggatedInitialRecordSummaryReportDTO() : base() { }

        public string Province { get; set; }
        public string District { get; set; }
        public string SiteName { get; set; }
    }
}
