using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDataAccess.DTO.TemporaryReportListingsDTO
{
    public class BeneficiariesIndeterminatedStatusListingDTO
    {
        public String ChiefPartner { get; set; }
        public String Partner { get; set; }
        public String HouseholdName { get; set; }
        public DateTime HouseholdRegistrationDate { get; set; }
        public String BeneficiaryType { get; set; }
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public String Gender { get; set; }
        public DateTime DateOFBirth { get; set; }
        public int Age { get; set; }
        public String OVCType { get; set; }
        public String GeralHIVStatus { get; set; }
        public String DetailedHIVStatus { get; set; }
        public String HIVTracked { get; set; }
        public String Degree_of_Kinship { get; set; }

        public List<Object> values { get; set; } = new List<Object>();

        public List<Object> populatedValues()
        {
            values.Add(ChiefPartner);
            values.Add(Partner);
            values.Add(HouseholdName);
            values.Add(HouseholdRegistrationDate);
            values.Add(BeneficiaryType);
            values.Add(FirstName);
            values.Add(LastName);
            values.Add(Gender);
            values.Add(DateOFBirth);
            values.Add(Age);
            values.Add(OVCType);
            values.Add(GeralHIVStatus);
            values.Add(DetailedHIVStatus);
            values.Add(HIVTracked);
            values.Add(Degree_of_Kinship);

            return values;
        }
    }
}