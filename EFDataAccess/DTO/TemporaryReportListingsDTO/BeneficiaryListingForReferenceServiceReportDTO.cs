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
    public class BeneficiaryListingForReferenceServiceReportDTO
    {
        public String ChiefPartner { get; set; }
        public String Partner { get; set; }
        public String HouseholdName { get; set; }
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public String Gender { get; set; }
        public String DateOfBirth { get; set; }
        public int YearAge { get; set; }
        public int MonthAge { get; set; }
        public String ReferenceDate { get; set; }
        public String HealthAttendedDate { get; set; }
        public String SocialAttendedDate { get; set; }
        public String Grouping_Type_Name { get; set; }
        public String ReferenceName { get; set; }
        public String FieldType { get; set; }
        public String Value { get; set; }

        public List<Object> values { get; set; } = new List<Object>();

        public List<Object> populatedValues()
        {
            values.Add(ChiefPartner);
            values.Add(Partner);
            values.Add(HouseholdName);
            values.Add(FirstName);
            values.Add(LastName);
            values.Add(Gender);
            values.Add(DateOfBirth);
            values.Add(YearAge);
            values.Add(MonthAge);
            values.Add(ReferenceDate);
            values.Add(HealthAttendedDate);
            values.Add(SocialAttendedDate);
            values.Add(Grouping_Type_Name);
            values.Add(ReferenceName);
            values.Add(FieldType);
            values.Add(Value);

            return values;
        }
    }
}