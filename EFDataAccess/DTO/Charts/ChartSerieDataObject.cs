using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDataAccess.DTO.Charts
{
    public class ChartSerieDataObject: IComparable
    {
        public string name { get; set; }
        public IComparable value { get; set; }
        public string detail { get; set; }

        public int CompareTo(object obj)
        {
            return 1;
        }
    }
}
