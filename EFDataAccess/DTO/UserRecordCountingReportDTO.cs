using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace EFDataAccess.DTO
{
    public class UserRecordCountingReportDTO
    {
        public string FullName { get; set; }
        public int HouseholdTotal { get; set; }
        public int CSITotal { get; set; }
        public int CarePlanTotal { get; set; }
        public int RoutineVisitTotal { get; set; }
        public int ReferenceServiceTotal { get; set; }
        public int CounterReferenceServiceTotal { get; set; }

    public List<Object> values { get; set; } = new List<Object>();

        public List<Object> populatedValues()
        {
            values.Add(StringUtils.MaskIfConfIsEnabled(FullName));
            values.Add(HouseholdTotal);
            values.Add(CSITotal);
            values.Add(CarePlanTotal);
            values.Add(RoutineVisitTotal);
            values.Add(ReferenceServiceTotal);
            values.Add(CounterReferenceServiceTotal);

            return values;
        }

    }
}
