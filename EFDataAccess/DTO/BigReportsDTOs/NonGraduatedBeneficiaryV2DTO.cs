using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace EFDataAccess.DTO.AgreggatedDTO
{ 
    public class NonGraduatedBeneficiaryV2DTO : IAgreggatedReportDTO
    {
        public string Province { get; set; }
        public string District { get; set; }
        public string SiteName { get; set; }

        public string Partner { get; set; }
        public int Total { get; set; }
        public int DeathChild { get; set; }
        public int DeathAdult { get; set; }
        public int LostChild { get; set; }
        public int LostAdult { get; set; }
        public int GaveUpChild { get; set; }
        public int GaveUpAdult { get; set; }
        public int OthersChild { get; set; }
        public int OthersAdult { get; set; }
        public int TransfersChild { get; set; }
        public int TransfersAdult { get; set; }

        private List<Object> values { get; set; } = new List<Object>();

        public List<Object> PopulatedValues()
        {
            values.Add(Total);
            values.Add(DeathChild);
            values.Add(DeathAdult);
            values.Add(LostChild);
            values.Add(LostAdult);
            values.Add(GaveUpChild);
            values.Add(GaveUpAdult);
            values.Add(OthersChild);
            values.Add(OthersAdult);
            values.Add(TransfersChild);
            values.Add(TransfersAdult);

            return values;
        }
    }
}