using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace EFDataAccess.DTO.AgreggatedDTO
{ 
    public class NonGraduatedBeneficiaryDTO : IAgreggatedReportDTO
    {
        public string Province { get; set; }
        public string District { get; set; }
        public string SiteName { get; set; }

        public string Partner { get; set; }
        public int Total { get; set; }
        public int Death { get; set; }
        public int Lost { get; set; }
        public int GaveUp { get; set; }
        public int Others { get; set; }

        private List<Object> values { get; set; } = new List<Object>();

        public List<Object> PopulatedValues()
        {
            values.Add(Total);
            values.Add(Death);
            values.Add(Lost);
            values.Add(GaveUp);
            values.Add(Others);

            return values;
        }
    }
}