using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EFDataAccess.DTO
{
    public class CounterReferencesSummaryByRefTypeReportDTO : IAgreggatedReportDTO
    {
        public string Province { get; set; }
        public string District { get; set; }
        public string SiteName { get; set; }

        public string Partner { get; set; }
        public int ATS_Male { get; set; }
        public int ATS_Female { get; set; }
        public int TARV_Male { get; set; }
        public int TARV_Female { get; set; }
        public int CCR_Male { get; set; }
        public int CCR_Female { get; set; }
        public int SSR_Male { get; set; }
        public int SSR_Female { get; set; }
        public int VGB_Male { get; set; }
        public int VGB_Female { get; set; }
        public int Birth_Registration_Male { get; set; }
        public int Birth_Registration_Female { get; set; }
        public int Identification_Card_Male { get; set; }
        public int Identification_Card_Female { get; set; }
        public int School_Integration_Male { get; set; }
        public int School_Integration_Female { get; set; }
        public int Vocational_Courses_Male { get; set; }
        public int Vocational_Courses_Female { get; set; }
        public int School_Material_Male { get; set; }
        public int School_Material_Female { get; set; }
        public int Basic_Basket_Male { get; set; }
        public int Basic_Basket_Female { get; set; }
        public int INAS_Benefit_Male { get; set; }
        public int INAS_Benefit_Female { get; set; }
        public int SAAJ_Male { get; set; }
        public int SAAJ_Female { get; set; }
        public int Desnutrition_Male { get; set; }
        public int Desnutrition_Female { get; set; }
        public int Delay_in_Development_Male { get; set; }
        public int Delay_in_Development_Female { get; set; }
        public int Others_Male { get; set; }
        public int Others_Female { get; set; }

        public List<Object> values { get; set; } = new List<Object>();

        public List<Object> PopulatedValues()
        {
            values.Add(Partner);
            values.Add(ATS_Male);
            values.Add(ATS_Female);
            values.Add(TARV_Male);
            values.Add(TARV_Female);
            values.Add(CCR_Male);
            values.Add(CCR_Female);
            values.Add(SSR_Male);
            values.Add(SSR_Female);
            values.Add(VGB_Male);
            values.Add(VGB_Female);
            values.Add(Birth_Registration_Male);
            values.Add(Birth_Registration_Female);
            values.Add(Identification_Card_Male);
            values.Add(Identification_Card_Female);
            values.Add(School_Integration_Male);
            values.Add(School_Integration_Female);
            values.Add(Vocational_Courses_Male);
            values.Add(Vocational_Courses_Female);
            values.Add(School_Material_Male);
            values.Add(School_Material_Female);
            values.Add(Basic_Basket_Male);
            values.Add(Basic_Basket_Female);
            values.Add(INAS_Benefit_Male);
            values.Add(INAS_Benefit_Female);
            values.Add(SAAJ_Male);
            values.Add(SAAJ_Female);
            values.Add(Desnutrition_Male);
            values.Add(Desnutrition_Female);
            values.Add(Delay_in_Development_Male);
            values.Add(Delay_in_Development_Female);
            values.Add(Others_Male);
            values.Add(Others_Female);

            return values;
        }
    }
}