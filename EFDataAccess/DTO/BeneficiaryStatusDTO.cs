using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDataAccess.DTO
{
    public class BeneficiaryHIVStatusDTO
    {
        public string ChiefPartner { get; set; }
        public string Partner { get; set; }
        public string HouseholdName { get; set; }
        public DateTime RegistrationDate { get; set; }
        public string BeneficiaryType { get; set; }
        public string HIVStatus { get; set; }
        public DateTime HIVStatusDate { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }

        public List<Object> values { get; set; } = new List<Object>();

        public List<Object> populatedValues()
        {
            values.Add(ChiefPartner);
            values.Add(Partner);
            values.Add(HouseholdName);
            values.Add(RegistrationDate);
            values.Add(BeneficiaryType);
            values.Add(HIVStatus);
            values.Add(HIVStatusDate);
            values.Add(FirstName);
            values.Add(LastName);
            values.Add(Gender);
            values.Add(DateOfBirth);

            return values;
        }
    }
}
