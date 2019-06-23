using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDataAccess.DTO
{
    public class HouseholdSupportPlanListingDTO
    {
        public string Province { get; set; }
        public string District { get; set; }
        public string SiteName { get; set; }

        public string FullName { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public string HouseholdName { get; set; }
        public string Partner { get; set; }
        public DateTime RegistrationDate { get; set; }
        public DateTime SupportPlanInitialDate { get; set; }

        public List<Object> values { get; set; } = new List<Object>();

        public List<Object> PopulatedValues()
        {
            values.Add(FullName);
            values.Add(Age);
            values.Add(Gender);
            values.Add(HouseholdName);
            values.Add(Partner);
            values.Add(RegistrationDate);
            values.Add(SupportPlanInitialDate);

            return values;
        }
    }
}
