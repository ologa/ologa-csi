using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace EFDataAccess.DTO.AgreggatedDTO
{ 
    public class AgreggatedInitialRecordSummaryReportDTO
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

        public int ovc_father { get; set; }
        public int ovc_mother { get; set; }
        public int ovc_both { get; set; }
        public int IsPartSavingGroup { get; set; }

        public int HIV_N { get; set; }
        public int HIV_P_IN_TARV { get; set; }
        public int HIV_P_NOT_TARV { get; set; }
        public int HIV_KNOWN_NREVEAL { get; set; }
        public int HIV_UNKNOWN { get; set; }

        public int US { get; set; }
        public int Com { get; set; }
        public int CHASS { get; set; }
        public int OCBS { get; set; }
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

            values.Add(ovc_father);
            values.Add(ovc_mother);
            values.Add(ovc_both);
            values.Add(IsPartSavingGroup);

            values.Add(HIV_N);
            values.Add(HIV_P_IN_TARV);
            values.Add(HIV_P_NOT_TARV);
            values.Add(HIV_KNOWN_NREVEAL);
            values.Add(HIV_UNKNOWN);

            values.Add(US);
            values.Add(Com);
            values.Add(CHASS);
            values.Add(OCBS);
            values.Add(Others);

            return values;
        }
    }
}