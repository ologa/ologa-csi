using System;
using System.Collections.Generic;

namespace EFDataAccess.DTO
{
    public interface IAgreggatedReportDTO : IPartnerNameReportDTO
    {
        string Province { get; set; }
        string District { get; set; }
        string SiteName { get; set; }

        List<Object> PopulatedValues();
    }
}