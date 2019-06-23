using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace EFDataAccess.DTO.AgreggatedDTO
{ 
    public class AgeGroupDTO : IAgreggatedReportDTO
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

        private List<Object> values { get; set; } = new List<Object>();

        public List<Object> PopulatedValues()
        {
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

            return values;
        }
    }
}