using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace EFDataAccess.DTO
{
    public class LastChildStatusReportDTO : IPartnerNameReportDTO
    {
        public string Partner { get; set; }
        public string FullName { get; set; }
        public string Gender { get; set; }
        public int Age { get; set; }
        public string ChildStatusDate { get; set; }
        public string Description { get; set; }

        public List<Object> values { get; set; } = new List<Object>();

        public List<Object> populatedValues()
        {
            values.Add(Partner);
            //values.Add(StringUtils.MaskIfConfIsEnabled(FullName));
            values.Add(FullName);
            values.Add(Gender);
            values.Add(Age);
            values.Add(ChildStatusDate);
            values.Add(Description);

            return values;
        }

    }
}
