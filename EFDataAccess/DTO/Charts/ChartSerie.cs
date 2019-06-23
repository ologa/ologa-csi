using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDataAccess.DTO.Charts
{
    public class ChartSerie
    {
        public string type { get; set; }
        public string name { get; set; }
        public int yAxisIndex { get; set; }
        public string stack { get; set; }
        public float startAngle { get; set; }
        public float endAngle { get; set; }
        public List<IComparable> data { get; set; }
        public List<object> dataObjects { get; set; }
    }
}
