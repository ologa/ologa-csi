using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace EFDataAccess.DTO
{ 
    public class MonthlyActiveBeneficiariesSummaryReportDTO : IAgreggatedReportDTO
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

        public int DeathChild { get; set; }
        public int DeathAdult { get; set; }
        public int LostChild { get; set; }
        public int LostAdult { get; set; }
        public int GaveUpChild { get; set; }
        public int GaveUpAdult { get; set; }
        public int OthersChild { get; set; }
        public int OthersAdult { get; set; }
        public int TransfersPEPFARChild { get; set; }
        public int TransfersPEPFARAdult { get; set; }
        public int TransfersNotPEPFARChild { get; set; }
        public int TransfersNotPEPFARAdult { get; set; }
        public int BecameAdult { get; set; }
        public int ChildHasServices { get; set; }
        public int AdultHasServices { get; set; }

        public int HIV_P_IN_TARV { get; set; }
        public int HIV_P_NOT_TARV { get; set; }
        public int HIV_N { get; set; }
        public int HIV_KNOWN_NREVEAL { get; set; }
        public int HIV_NOT_RECOMMENDED { get; set; }
        public int HIV_UNKNOWN { get; set; }
        

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

            values.Add(DeathChild);
            values.Add(DeathAdult);
            values.Add(LostChild);
            values.Add(LostAdult);
            values.Add(GaveUpChild);
            values.Add(GaveUpAdult);
            values.Add(OthersChild);
            values.Add(OthersAdult);
            values.Add(TransfersPEPFARChild);
            values.Add(TransfersPEPFARAdult);
            values.Add(TransfersNotPEPFARChild);
            values.Add(TransfersNotPEPFARAdult);
            values.Add(BecameAdult);
            values.Add(ChildHasServices);
            values.Add(AdultHasServices);
            values.Add(HIV_P_IN_TARV);
            values.Add(HIV_P_NOT_TARV);
            values.Add(HIV_N);
            values.Add(HIV_KNOWN_NREVEAL);
            values.Add(HIV_NOT_RECOMMENDED);
            values.Add(HIV_UNKNOWN);

            return values;
        }
    }
}