using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace EFDataAccess.DTO.ListingReportsDTOs
{ 
    public class BeneficiariesListingWithYellowActiveOrInactiveStatusDTO : IAgreggatedReportDTO
    {
        public string Province { get; set; }
        public string District { get; set; }
        public string SiteName { get; set; }

        public string FullName { get; set; }
        public string Gender { get; set; }
        public int Age { get; set; }
        public string HouseholdName { get; set; }
        public string Partner { get; set; }
        public string ServiceStatus { get; set; }
        public string ReadyToGraduate { get; set; }
        public int Service_On_Previous_Trimester { get; set; }
        public int Service_On_Current_Trimester { get; set; }
        public int CarePlanMonitoring { get; set; }
        public int MonthSinceLastPA { get; set; }

        private List<Object> values { get; set; } = new List<Object>();

        public List<Object> PopulatedValues()
        {
            values.Add(FullName);
            values.Add(Gender);
            values.Add(Age);
            values.Add(HouseholdName);
            values.Add(Partner);
            values.Add(ServiceStatus);
            values.Add(ReadyToGraduate);
            values.Add(Service_On_Previous_Trimester);
            values.Add(Service_On_Current_Trimester);
            values.Add(CarePlanMonitoring);
            values.Add(MonthSinceLastPA);

            return values;
        }
    }
}