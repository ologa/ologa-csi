using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDataAccess.DTO
{
    public class AgreggatedLastChildStatusReportDTO : AgreggatedBaseDataDTO
    {
        public string Partner { get; set; }
        public string Description { get; set; }
        public int MaleTotal { get; set; }
        public int FemaleTotal { get; set; }
        public int Total { get; set; }

        public List<Object> values { get; set; } = new List<Object>();

        public List<Object> populatedValues()
        {
            values.Add(Partner);
            values.Add(Description);
            values.Add(MaleTotal);
            values.Add(FemaleTotal);
            values.Add(Total);

            return values;
        }

    }
}
