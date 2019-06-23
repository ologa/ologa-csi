using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDataAccess.DTO.AgreggatedDTO
{
    public class AgreggatedRoutineVisitSummaryReportDTO
    {
        public string Province { get; set; }
        public string District { get; set; }
        public string SiteName { get; set; }
        public int btw_x_1_M { get; set; }
        public int btw_1_4_M { get; set; }
        public int btw_5_9_M { get; set; }
        public int btw_10_14_M { get; set; }
        public int btw_15_17_M { get; set; }
        public int btw_18_24_M { get; set; }
        public int btw_25_x_M { get; set; }
        public int btw_x_1_F { get; set; }
        public int btw_1_4_F { get; set; }
        public int btw_5_9_F { get; set; }
        public int btw_10_14_F { get; set; }
        public int btw_15_17_F { get; set; }
        public int btw_18_24_F { get; set; }
        public int btw_25_x_F { get; set; }

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
        public int FamilyKitReceived { get; set; }
        public int NewDPI { get; set; }
        public int RptDPI { get; set; }
        public int MUACGreen { get; set; }
        public int MUACYellow { get; set; }
        public int MUACRed { get; set; }

        public int Death { get; set; }
        public int Lost { get; set; }
        public int GaveUp { get; set; }
        public int Others { get; set; }

        public List<Object> values { get; set; } = new List<Object>();

        public List<Object> populatedValues()
        {
            values.Add(Province);
            values.Add(District);
            values.Add(SiteName);
            values.Add(btw_x_1_M);
            values.Add(btw_1_4_M);
            values.Add(btw_5_9_M);
            values.Add(btw_10_14_M);
            values.Add(btw_15_17_M);
            values.Add(btw_18_24_M);
            values.Add(btw_25_x_M);
            values.Add(btw_x_1_F);
            values.Add(btw_1_4_F);
            values.Add(btw_5_9_F);
            values.Add(btw_10_14_F);
            values.Add(btw_15_17_F);
            values.Add(btw_18_24_F);
            values.Add(btw_25_x_F);

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
            values.Add(FamilyKitReceived);
            values.Add(NewDPI);
            values.Add(RptDPI);
            values.Add(MUACGreen);
            values.Add(MUACYellow);
            values.Add(MUACRed);

            values.Add(Death);
            values.Add(Lost);
            values.Add(GaveUp);
            values.Add(Others);

            return values;
        }
    }
}