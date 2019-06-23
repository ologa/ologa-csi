using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace EFDataAccess.DTO
{
    public class GraduatedAdultReportDTO
    {
        public string PartnerName { get; set; }
        public string FullNameAdult { get; set; }
        public string Gender { get; set; }
        public int Age { get; set; }
        public string Description { get; set; }

        public List<Object> values { get; set; } = new List<Object>();

        public List<Object> populatedValues()
        {
            values.Add(PartnerName);
            values.Add(StringUtils.MaskIfConfIsEnabled(FullNameAdult));
            values.Add(Gender);
            values.Add(Age);
            values.Add(Description);

            return values;
        }

    }
}
