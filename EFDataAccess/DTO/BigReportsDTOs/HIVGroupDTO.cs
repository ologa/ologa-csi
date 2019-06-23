using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace EFDataAccess.DTO.AgreggatedDTO
{ 
    public class HIVGroupDTO : IAgreggatedReportDTO
    {
        public string Province { get; set; }
        public string District { get; set; }
        public string SiteName { get; set; }

        public string Partner { get; set; }
        public int HIV_P_IN_TARV { get; set; }
        public int HIV_P_NOT_TARV { get; set; }
        public int HIV_N { get; set; }
        public int HIV_KNOWN_NREVEAL { get; set; }
        public int HIV_UNKNOWN { get; set; }
        public int HIV_NOT_RECOMMENDED { get; set; }

        private List<Object> values { get; set; } = new List<Object>();

        public List<Object> PopulatedValues()
        {
            values.Add(HIV_P_IN_TARV);
            values.Add(HIV_P_NOT_TARV);
            values.Add(HIV_N);
            values.Add(HIV_KNOWN_NREVEAL);
            values.Add(HIV_UNKNOWN);
            values.Add(HIV_NOT_RECOMMENDED);

            return values;
        }
    }
}