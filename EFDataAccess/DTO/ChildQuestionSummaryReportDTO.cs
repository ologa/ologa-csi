using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace EFDataAccess.DTO
{
    public class ChildQuestionSummaryReportDTO : AgreggatedBaseDataDTO
    {
        public string Partner { get; set; }
        public string FullName { get; set; }
        public string Gender { get; set; }
        public int Age { get; set; }
        public string CSIDate { get; set; }
        public int P1 { get; set; }
        public int P2 { get; set; }
        public int P3 { get; set; }
        public int P4 { get; set; }
        public int P5 { get; set; }
        public int P6 { get; set; }
        public int P7 { get; set; }
        public int P8 { get; set; }
        public int P9 { get; set; }
        public int P10 { get; set; }
        public int P11 { get; set; }
        public int P12 { get; set; }
        public int P13 { get; set; }
        public int P14 { get; set; }
        public int P15 { get; set; }
        public int P16 { get; set; }
        public int P17 { get; set; }
        public int P18 { get; set; }
        public int P19 { get; set; }
        public int P20 { get; set; }
        public int P21 { get; set; }
        public int P22 { get; set; }
        public int P23 { get; set; }
        public int P24 { get; set; }
        public int P25 { get; set; }
        public int P26 { get; set; }
        public int P27 { get; set; }
        public int P28 { get; set; }
        public int P29 { get; set; }
        public int P30 { get; set; }
        public int P31 { get; set; }
        public int P32 { get; set; }
        public int P33 { get; set; }

        public List<Object> values { get; set; } = new List<Object>();

        public List<Object> populatedValues()
        {
            values.Add(Partner);
            values.Add(FullName);
            values.Add(Gender);
            values.Add(Age);
            values.Add(CSIDate);
            values.Add(P1);
            values.Add(P2);
            values.Add(P3);
            values.Add(P4);
            values.Add(P5);
            values.Add(P6);
            values.Add(P7);
            values.Add(P8);
            values.Add(P9);
            values.Add(P10);
            values.Add(P11);
            values.Add(P12);
            values.Add(P13);
            values.Add(P14);
            values.Add(P15);
            values.Add(P16);
            values.Add(P17);
            values.Add(P18);
            values.Add(P19);
            values.Add(P20);
            values.Add(P21);
            values.Add(P22);
            values.Add(P23);
            values.Add(P24);
            values.Add(P25);
            values.Add(P26);
            values.Add(P27);
            values.Add(P28);
            values.Add(P29);
            values.Add(P30);
            values.Add(P31);
            values.Add(P32);
            values.Add(P33);

            return values;
        }

        public List<Object> populatedAgreggatedValues()
        {
            values.Add(Partner);
            values.Add(StringUtils.MaskIfConfIsEnabled(FullName));
            values.Add(Gender);
            values.Add(Age);
            values.Add(CSIDate);
            values.Add(P1);
            values.Add(P2);
            values.Add(P3);
            values.Add(P4);
            values.Add(P5);
            values.Add(P6);
            values.Add(P7);
            values.Add(P8);
            values.Add(P9);
            values.Add(P10);
            values.Add(P11);
            values.Add(P12);
            values.Add(P13);
            values.Add(P14);
            values.Add(P15);
            values.Add(P16);
            values.Add(P17);
            values.Add(P18);
            values.Add(P19);
            values.Add(P20);
            values.Add(P21);
            values.Add(P22);
            values.Add(P23);
            values.Add(P24);
            values.Add(P25);
            values.Add(P26);
            values.Add(P27);
            values.Add(P28);
            values.Add(P29);
            values.Add(P30);
            values.Add(P31);
            values.Add(P32);
            values.Add(P33);

            return values;
        }

    }
}
