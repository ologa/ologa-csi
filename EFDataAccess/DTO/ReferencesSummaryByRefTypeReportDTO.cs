using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EFDataAccess.DTO
{
    public class ReferencesSummaryByRefTypeReportDTO : IPartnerNameReportDTO
    {
        public string Partner { get; set; }
        public int ATS_Male { get; set; }
        public int ATS_Female { get; set; }
        public int TARV_Male { get; set; }
        public int TARV_Female { get; set; }
        public int CCR_Male { get; set; }
        public int CCR_Female { get; set; }
        public int SSR_Male { get; set; }
        public int SSR_Female { get; set; }
        public int VGB_Male { get; set; }
        public int VGB_Female { get; set; }
        public int Others_Male { get; set; }
        public int Others_Female { get; set; }
        public int Total_Male { get; set; }
        public int Total_Female { get; set; }

        public int RC_ATS_Male { get; set; }
        public int RC_ATS_Female { get; set; }
        public int RC_TARV_Male { get; set; }
        public int RC_TARV_Female { get; set; }
        public int RC_CCR_Male { get; set; }
        public int RC_CCR_Female { get; set; }
        public int RC_SSR_Male { get; set; }
        public int RC_SSR_Female { get; set; }
        public int RC_VGB_Male { get; set; }
        public int RC_VGB_Female { get; set; }
        public int RC_Others_Male { get; set; }
        public int RC_Others_Female { get; set; }
        public int RC_Total_Male { get; set; }
        public int RC_Total_Female { get; set; }

        public List<Object> values { get; set; } = new List<Object>();

        public List<Object> populatedValues()
        {
            values.Add(Partner);
            values.Add(ATS_Male);
            values.Add(ATS_Female);
            values.Add(TARV_Male);
            values.Add(TARV_Female);
            values.Add(CCR_Male);
            values.Add(CCR_Female);
            values.Add(SSR_Male);
            values.Add(SSR_Female);
            values.Add(VGB_Male);
            values.Add(VGB_Female);
            values.Add(Others_Male);
            values.Add(Others_Female);
            values.Add(Total_Male);
            values.Add(Total_Female);

            values.Add(RC_ATS_Male);
            values.Add(RC_ATS_Female);
            values.Add(RC_TARV_Male);
            values.Add(RC_TARV_Female);
            values.Add(RC_CCR_Male);
            values.Add(RC_CCR_Female);
            values.Add(RC_SSR_Male);
            values.Add(RC_SSR_Female);
            values.Add(RC_VGB_Male);
            values.Add(RC_VGB_Female);
            values.Add(RC_Others_Male);
            values.Add(RC_Others_Female);
            values.Add(RC_Total_Male);
            values.Add(RC_Total_Female);

            return values;
        }
    }
}