using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EFDataAccess.DTO
{
    public class ReferencesSummaryByAgeReportDTO
    {
        public string ReferenceName { get; set; }
        public int Male_x_1 { get; set; }
        public int Female_x_1 { get; set; }
        public int Male_1_4 { get; set; }
        public int Female_1_4 { get; set; }
        public int Male_5_9 { get; set; }
        public int Female_5_9 { get; set; }
        public int Male_10_14 { get; set; }
        public int Female_10_14 { get; set; }
        public int Male_15_17 { get; set; }
        public int Female_15_17 { get; set; }
        public int Male_18_24 { get; set; }
        public int Female_18_24 { get; set; }
        public int Male_25_x { get; set; }
        public int Female_25_x { get; set; }

        public List<int> values { get; set; } = new List<int>();

        public List<int> populatedValues()
        {
            values.Add(Male_x_1);
            values.Add(Female_x_1);
            values.Add(Male_1_4);
            values.Add(Female_1_4);
            values.Add(Male_5_9);
            values.Add(Female_5_9);
            values.Add(Male_10_14);
            values.Add(Female_10_14);
            values.Add(Male_15_17);
            values.Add(Female_15_17);
            values.Add(Male_18_24);
            values.Add(Female_18_24);
            values.Add(Male_25_x);
            values.Add(Female_25_x);

            return values;
        }
    }
}