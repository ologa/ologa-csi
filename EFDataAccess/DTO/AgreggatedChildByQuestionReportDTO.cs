using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDataAccess.DTO
{
    public class AgreggatedChildByQuestionReportDTO : AgreggatedBaseDataDTO
    {
        public string Partner { get; set; }
        public string Domain { get; set; }
        public string QuestionDescription { get; set; }
        public int Score { get; set; }
        public int MaleTotal { get; set; }
        public int FemaleTotal { get; set; }
        public int Total { get; set; }

        public List<Object> values { get; set; } = new List<Object>();

        public List<Object> populatedAgreggatedValues()
        {
            values.Add(Partner);
            values.Add(Domain);
            values.Add(QuestionDescription);
            values.Add(Score);
            values.Add(MaleTotal);
            values.Add(FemaleTotal);
            values.Add(Total);

            return values;
        }

    }
}
