using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDataAccess.DTO
{
    public class ChildByQuestionReportDTO : IPartnerNameReportDTO
    {
        public string Partner { get; set; }
        public string FullName { get; set; }
        public int Age { get; set; }
        public string Gender { get; set; }
        public string CSIDate { get; set; }
        public string Domain { get; set; }
        public string Description { get; set; }
        public int Score { get; set; }

        public List<Object> values { get; set; } = new List<Object>();

        public List<Object> populatedValues()
        {
            values.Add(Partner);
            values.Add(FullName);
            values.Add(Age);
            values.Add(Gender);
            values.Add(CSIDate);
            values.Add(Domain);
            values.Add(Description);
            values.Add(Score);

            return values;
        }

    }
}
