using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EFDataAccess.DTO
{
    public class HIVStatusChangesDTO : IPartnerNameReportDTO
    {
        public String Partner { get; set; }
        public int Beneficiaries { get; set; }
        public String Initial_HIV_State { get; set; }
        public String Final_HIV_State { get; set; }

        public List<Object> values { get; set; } = new List<Object>();

        public List<Object> populatedValues()
        {
            values.Add(Partner);
            values.Add(Beneficiaries);
            values.Add(Initial_HIV_State);
            values.Add(Final_HIV_State);
            return values;
        }
    }
}