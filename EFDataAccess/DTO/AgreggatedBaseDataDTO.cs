using System;
using System.Collections.Generic;

namespace EFDataAccess.DTO
{
    public class AgreggatedBaseDataDTO
    {
        public int PartnerID { get; set; }
        public string Partner { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
        public string SiteName { get; set; }
        public IPartnerNameReportDTO summaryReportDTO { get; set; }

        public List<Object> values { get; set; } = new List<Object>();

        public List<Object> populatedValues()
        {
            values.Add(Province);
            values.Add(District);
            values.Add(SiteName);
            values.Add(Partner);
            return values;
        }
    }
}
