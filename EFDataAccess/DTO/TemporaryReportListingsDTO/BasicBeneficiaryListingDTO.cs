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
    public class BasicBeneficiaryListingDTO
    {
        public String ChiefPartner { get; set; }
        public String Partner { get; set; }
        public String HouseholdName { get; set; }
        public String BeneficiaryRegistrationDate { get; set; }
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public String Gender { get; set; }
        public String DateOfBirth { get; set; }
        public int YearAge { get; set; }
        public int MonthAge { get; set; }

        public List<Object> values { get; set; } = new List<Object>();

        public List<Object> populatedValues()
        {
            values.Add(ChiefPartner);
            values.Add(Partner);
            values.Add(HouseholdName);
            values.Add(BeneficiaryRegistrationDate);
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