using System;
using System.Collections.Generic;

namespace EFDataAccess.DTO
{
    public class HouseholdSupportPlanDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public string HouseholdName { get; set; }
        public string PartnerName { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public DateTime? SupportPlanInitialDate { get; set; }
        public DateTime? SupportPlanFinalDate { get; set; }

        public List<Object> values { get; set; } = new List<Object>();

        public List<Object> populatedValues()
        {
            values.Add(FirstName);
            values.Add(LastName);
            values.Add(Age);
            values.Add(Gender);
            values.Add(HouseholdName);
            values.Add(PartnerName);
            values.Add(RegistrationDate);
            values.Add(SupportPlanInitialDate);
            values.Add(SupportPlanFinalDate);

            return values;
        }
    }
}
