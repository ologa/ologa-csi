using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace EFDataAccess.DTO
{
    public class InitialAndActualChildStatusReportDTO
    {
        public string PartnerName { get; set; }
        public string FullName { get; set; }
        public string Gender { get; set; }
        public int Age { get; set; }
        public string InitialChildStatus { get; set; }
        public DateTime InitialChildStatusDate { get; set; }
        public string ActualChildStatus { get; set; }
        public DateTime ActualChildStatusDate { get; set; }

        public List<Object> values { get; set; } = new List<Object>();

        public List<Object> populatedValues()
        {
            values.Add(PartnerName);
            values.Add(StringUtils.MaskIfConfIsEnabled(FullName));
            values.Add(Gender);
            values.Add(Age);
            values.Add(InitialChildStatus);
            values.Add(InitialChildStatusDate);
            values.Add(ActualChildStatus);
            values.Add(ActualChildStatusDate);


            return values;
        }

    }
}
