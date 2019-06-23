using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDataAccess.DTO
{
    public class BeneficiariesWithoutServicesDTO
    {
        public string ChiefPartner { get; set; }
        public string Partner { get; set; }
        public string HouseholdName { get; set; }
        public DateTime HouseholdRegistrationDate { get; set; }
        public string BeneficiaryType { get; set; }
        public string BeneficiaryStatus { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public int YearAge { get; set; }
        public int MonthAge { get; set; }

        public List<Object> values { get; set; } = new List<Object>();

        public List<Object> populatedValues()
        {
            values.Add(ChiefPartner);
            values.Add(Partner);
            values.Add(HouseholdName);
            values.Add(HouseholdRegistrationDate);
            values.Add(BeneficiaryType);
            values.Add(BeneficiaryStatus);
            values.Add(FirstName);
            values.Add(LastName);
            values.Add(Gender);
            values.Add(DateOfBirth);
            values.Add(YearAge);
            values.Add(MonthAge);

            return values;
        }
    }
}