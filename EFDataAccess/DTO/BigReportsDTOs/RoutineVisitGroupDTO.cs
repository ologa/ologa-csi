using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace EFDataAccess.DTO.AgreggatedDTO
{ 
    public class RoutineVisitGroupDTO : IAgreggatedReportDTO
    {
        public string Province { get; set; }
        public string District { get; set; }
        public string SiteName { get; set; }

        public string Partner { get; set; }
        public int HIVTracked { get; set; }
        public int HIVRisk { get; set; }
        public int FinanceAid { get; set; }
        public int Food { get; set; }
        public int Housing { get; set; }
        public int Education { get; set; }
        public int Health { get; set; }
        public int SocialAid { get; set; }
        public int LegalAdvice { get; set; }
        public int DPI { get; set; }
        public int MUACGreen { get; set; }
        public int MUACYellow { get; set; }
        public int MUACRed { get; set; }

        private List<Object> values { get; set; } = new List<Object>();

        public List<Object> PopulatedValues()
        {
            values.Add(HIVTracked);
            values.Add(HIVRisk);
            values.Add(FinanceAid);
            values.Add(Food);
            values.Add(Housing);
            values.Add(Education);
            values.Add(Health);
            values.Add(SocialAid);
            values.Add(LegalAdvice);
            values.Add(DPI);
            values.Add(MUACGreen);
            values.Add(MUACYellow);
            values.Add(MUACRed);

            return values;
        }
    }
}