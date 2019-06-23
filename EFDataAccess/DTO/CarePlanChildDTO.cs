using System;
using System.Collections.Generic;
using Utilities;

namespace EFDataAccess.DTO
{
    public class CarePlanChildDTO : IPartnerNameReportDTO
    {
        public String Partner { get; set; }
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public DateTime CarePlanDate { get; set; }
        public int ChildCount { get; set; }

        public List<Object> values { get; set; } = new List<Object>();

        public List<Object> populatedValues()
        {
            values.Add(Partner);
            values.Add(StringUtils.MaskIfConfIsEnabled(FirstName));
            values.Add(StringUtils.MaskIfConfIsEnabled(LastName));
            values.Add(CarePlanDate);
            return values;
        }

        public List<Object> populatedAgreggatedValues()
        {
            values.Add(Partner);
            values.Add(ChildCount);
            return values;
        }
    }
}