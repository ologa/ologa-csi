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
    public class HouseholdwithIndeterminatedStatusListingDTO
    {
        public String ChiefPartner { get; set; }
        public String Partner { get; set; }
        public String HouseholdName { get; set; }
        public DateTime HouseholdRegistrationDate { get; set; }
        public String FamilyOrigin { get; set; }
        public String FamilyHead { get; set; }

        public List<Object> values { get; set; } = new List<Object>();

        public List<Object> populatedValues()
        {
            values.Add(ChiefPartner);
            values.Add(Partner);
            values.Add(HouseholdName);
            values.Add(HouseholdRegistrationDate);
            values.Add(FamilyOrigin);
            values.Add(FamilyHead);

            return values;
        }

    }
}