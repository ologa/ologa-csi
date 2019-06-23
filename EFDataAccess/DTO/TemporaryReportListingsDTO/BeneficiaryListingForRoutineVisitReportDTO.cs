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
    public class BeneficiaryListingForRoutineVisitReportDTO
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
        public String RoutineVisitAll { get; set; }
        public String RoutineVisitInterval { get; set; }
        public int NewFinaceAid { get; set; }
        public int RptFinaceAid { get; set; }
        public int NewHealth { get; set; }
        public int RptHealth { get; set; }
        public int NewFood { get; set; }
        public int RptFood { get; set; }
        public int NewEducation { get; set; }
        public int RptEducation { get; set; }
        public int NewLegalAdvice { get; set; }
        public int RptLegalAdvice { get; set; }
        public int NewHousing { get; set; }
        public int RptHousing { get; set; }
        public int NewSocialAid { get; set; }
        public int RptSocialAid { get; set; }
        public int NewDPI { get; set; }
        public int RptDPI { get; set; }
        public int HIVRisk { get; set; }
        public int HIVRisk_in_Household { get; set; }
        public int MUACGreen { get; set; }
        public int MUACYellow { get; set; }
        public int MUACRed { get; set; }


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
            values.Add(RoutineVisitAll);
            values.Add(RoutineVisitInterval);
            values.Add(NewFinaceAid);
            values.Add(RptFinaceAid);
            values.Add(NewHealth);
            values.Add(RptHealth);
            values.Add(NewFood);
            values.Add(RptFood);
            values.Add(NewEducation);
            values.Add(RptEducation);
            values.Add(NewLegalAdvice);
            values.Add(RptLegalAdvice);
            values.Add(NewHousing);
            values.Add(RptHousing);
            values.Add(NewSocialAid);
            values.Add(RptSocialAid);
            values.Add(NewDPI);
            values.Add(RptDPI);
            values.Add(HIVRisk);
            values.Add(HIVRisk_in_Household);
            values.Add(MUACGreen);
            values.Add(MUACYellow);
            values.Add(MUACRed);

            return values;
        }
    }
}