using System;
using System.Collections.Generic;

namespace EFDataAccess.DTO.AgreggatedDTO
{
    public class AgreggatedMEDataDTO
    {
        public string Province { get; set; }
        public string District { get; set; }
        public string SiteName { get; set; }
        public ISiteNameReportDTO siteNameReportDTO { get; set; }

        public List<Object> values { get; set; } = new List<Object>();

        public List<Object> populatedValues()
        {
            values.Add(Province);
            values.Add(District);
            values.Add(SiteName);
            return values;
        }
    }
}
