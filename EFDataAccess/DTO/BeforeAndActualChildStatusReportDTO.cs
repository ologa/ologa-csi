using System;
using System.Collections.Generic;
using Utilities;

namespace EFDataAccess.DTO
{
    public class BeforeAndActualChildStatusReportDTO : IPartnerNameReportDTO
    {
        public string Partner { get; set; }
        public string BeneficiaryType { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Gender { get; set; }
        public int Age { get; set; }
        public string BeforeActualStatus { get; set; }
        public DateTime BeforeActualStatusDate { get; set; }
        public string ActualStatus { get; set; }
        public DateTime ActualStatusDate { get; set; }

        public List<Object> values { get; set; } = new List<Object>();

        public List<Object> populatedValues()
        {
            values.Add(Partner);
            values.Add(BeneficiaryType);
            values.Add(StringUtils.MaskIfConfIsEnabled(FirstName));
            values.Add(StringUtils.MaskIfConfIsEnabled(LastName));
            values.Add(Gender);
            values.Add(Age);
            values.Add(BeforeActualStatus);
            values.Add(BeforeActualStatusDate);
            values.Add(ActualStatus);
            values.Add(ActualStatusDate);


            return values;
        }

    }
}
