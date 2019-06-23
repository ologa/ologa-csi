using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDataAccess.DTO
{
    public class SeriesDataBeneficiaryStates
    {
        public String State { get; set; }
        public int AdultCount { get; set; }
        public int ChildCount { get; set; }
        public int Total { get; set; }
    }
}
