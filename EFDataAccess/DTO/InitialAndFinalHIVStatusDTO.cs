using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EFDataAccess.DTO
{
    public class InitialAndFinalHIVStatusDTO
    {
        public String ChiefPartner { get; set; }
        public int BENEFICIARIES { get; set; }
        public int HIV_N1 { get; set; }
        public int HIV_P_IN_TARV1 { get; set; }
        public int HIV_P_NOT_TARV1 { get; set; }
        public int HIV_KNOWN_NREVEAL1 { get; set; }
        public int HIV_UNKNOWN1 { get; set; }
        public int HIV_N2 { get; set; }
        public int HIV_P_IN_TARV2 { get; set; }
        public int HIV_P_NOT_TARV2 { get; set; }
        public int HIV_KNOWN_NREVEAL2 { get; set; }
        public int HIV_UNKNOWN2 { get; set; }

        public List<Object> values { get; set; } = new List<Object>();

        public List<Object> populatedValues()
        {
            values.Add(ChiefPartner);
            values.Add(BENEFICIARIES);
            values.Add(HIV_N1);
            values.Add(HIV_P_IN_TARV1);
            values.Add(HIV_P_NOT_TARV1);
            values.Add(HIV_KNOWN_NREVEAL1);
            values.Add(HIV_UNKNOWN1);
            values.Add(HIV_N2);
            values.Add(HIV_P_IN_TARV2);
            values.Add(HIV_P_NOT_TARV2);
            values.Add(HIV_KNOWN_NREVEAL2);
            values.Add(HIV_UNKNOWN2);
            return values;
        }
    }
}