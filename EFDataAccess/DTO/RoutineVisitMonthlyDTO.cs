using System;
using System.Collections.Generic;

namespace EFDataAccess.DTO
{
    public class RoutineVisitMonthlyDTO
    {
        public int DomainOrder { get; set; }
        public string Domain { get; set; }
        public string Question { get; set; }
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

        public List<object> PopulatedValuesList { get; set; } = new List<object>();

        public List<object> PopulatedValues()
        {
            PopulatedValuesList.Add(btw_x_1_M);
            PopulatedValuesList.Add(btw_1_4_M);
            PopulatedValuesList.Add(btw_5_9_M);
            PopulatedValuesList.Add(btw_10_14_M);
            PopulatedValuesList.Add(btw_15_17_M);
            PopulatedValuesList.Add(btw_18_24_M);
            PopulatedValuesList.Add(btw_25_x_M);
            PopulatedValuesList.Add(btw_x_1_F);
            PopulatedValuesList.Add(btw_1_4_F);
            PopulatedValuesList.Add(btw_5_9_F);
            PopulatedValuesList.Add(btw_10_14_F);
            PopulatedValuesList.Add(btw_15_17_F);
            PopulatedValuesList.Add(btw_18_24_F);
            PopulatedValuesList.Add(btw_25_x_F);

            return PopulatedValuesList;
        }
    }
}
