using EFDataAccess.Services;
using System;
using System.Collections.Generic;
using Utilities;

namespace EFDataAccess.DTO
{
    public class BeneficiariesAndHIVStatusDTO : IPartnerNameReportDTO
    {

        public String Partner { get; set; }
        public String FirstName { get; set; }
        public String LastName { get; set; }
        public String Gender { get; set; }
        public int Age { get; set; }
        public String HIV_State { get; set; }

        public List<Object> values { get; set; } = new List<Object>();

        public List<Object> populatedValues()
        {
            values.Add(Partner);
            values.Add(StringUtils.MaskIfConfIsEnabled(FirstName));
            values.Add(StringUtils.MaskIfConfIsEnabled(LastName));
            values.Add(Gender);
            values.Add(Age);
            values.Add(HIV_State);
            return values;
        }
    }
}