using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace EFDataAccess.DTO.SummaryReportsDTO
{ 
    public class MonthlyGraduatedBeneficiariesSummaryDTO : IAgreggatedReportDTO
    {
        public string Province { get; set; }
        public string District { get; set; }
        public string SiteName { get; set; }

        public string Partner { get; set; }
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
       
        public int HIV_P_IN_TARV_Child { get; set; }
        public int HIV_P_NOT_TARV_Child { get; set; }
        public int HIV_N_Child { get; set; }
        public int HIV_KNOWN_NREVEAL_Child { get; set; }
        public int HIV_NOT_RECOMMENDED_Child { get; set; }
        public int HIV_UNKNOWN_Child { get; set; }
        public int HIV_P_IN_TARV_Adult { get; set; }
        public int HIV_P_NOT_TARV_Adult { get; set; }
        public int HIV_N_Adult { get; set; }
        public int HIV_KNOWN_NREVEAL_Adult { get; set; }
        public int HIV_UNKNOWN_Adult { get; set; }

        public int Permanence_btw_x_6_month { get; set; }
        public int Permanence_btw_6_11_month { get; set; }
        public int Permanence_btw_12_17_month { get; set; }
    

    private List<Object> values { get; set; } = new List<Object>();

        public List<Object> PopulatedValues()
        {
            values.Add(Partner);
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

            values.Add(HIV_P_IN_TARV_Child);
            values.Add(HIV_P_NOT_TARV_Child);
            values.Add(HIV_N_Child);
            values.Add(HIV_KNOWN_NREVEAL_Child);
            values.Add(HIV_NOT_RECOMMENDED_Child);
            values.Add(HIV_UNKNOWN_Child);
            values.Add(HIV_P_IN_TARV_Adult);
            values.Add(HIV_P_NOT_TARV_Adult);
            values.Add(HIV_N_Adult);
            values.Add(HIV_KNOWN_NREVEAL_Adult);
            values.Add(HIV_UNKNOWN_Adult);

            values.Add(Permanence_btw_x_6_month);
            values.Add(Permanence_btw_6_11_month);
            values.Add(Permanence_btw_12_17_month);

            return values;
        }
    }
}