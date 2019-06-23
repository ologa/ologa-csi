using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace EFDataAccess.DTO
{ 
    public class HIVStatusByPartnerReportDTO : IAgreggatedReportDTO
    {
        public string Province { get; set; }
        public string District { get; set; }
        public string SiteName { get; set; }

        public string Partner { get; set; }
        public int NewChildInTARV { get; set; }
        public int NewChildNotInTARV { get; set; }
        public int ChildNegative { get; set; }
        public int NewChildNotRevealed { get; set; }
        public int NewChildNotRecommended { get; set; }
        public int NewChildUnknown { get; set; }

        public int NewAdultInTARV { get; set; }
        public int NewAdultNotInTARV { get; set; }
        public int AdultNegative { get; set; }
        public int NewAdultNotRevealed { get; set; }
        public int NewAdultUnknown { get; set; }

        public int GraduatedChildInTARV { get; set; }
        public int GraduatedChildNotInTARV { get; set; }
        public int GraduatedChildNegative { get; set; }
        public int GraduatedChildNotRevealed { get; set; }
        public int GraduatedChildNotRecommended { get; set; }
        public int GraduatedChildUnknown { get; set; }

        public int GraduatedAdultInTARV { get; set; }
        public int GraduatedAdultNotInTARV { get; set; }
        public int GraduatedAdultNegative { get; set; }
        public int GraduatedAdultNotRevealed { get; set; }
        public int GraduatedAdultUnknown { get; set; }

        public int InitialChildInTARV { get; set; }
        public int InitialChildNotInTARV { get; set; }
        public int InitialChildNegative { get; set; }
        public int InitialChildNotRevealed { get; set; }
        public int InitialChildNotRecommended { get; set; }
        public int InitialChildUnknown { get; set; }

        public int InitialAdultInTARV { get; set; }
        public int InitialAdultNotInTARV { get; set; }
        public int InitialAdultNegative { get; set; }
        public int InitialAdultNotRevealed { get; set; }
        public int InitialAdultUnknown { get; set; }

    private List<Object> values { get; set; } = new List<Object>();

        public List<Object> PopulatedValues()
        {
            values.Add(Partner);
            values.Add(NewChildInTARV);
            values.Add(NewChildNotInTARV);
            values.Add(ChildNegative);
            values.Add(NewChildNotRevealed);
            values.Add(NewChildNotRecommended);
            values.Add(NewChildUnknown);

            values.Add(NewAdultInTARV);
            values.Add(NewAdultNotInTARV);
            values.Add(AdultNegative);
            values.Add(NewAdultNotRevealed);
            values.Add(NewAdultUnknown);

            values.Add(GraduatedChildInTARV);
            values.Add(GraduatedChildNotInTARV);
            values.Add(GraduatedChildNegative);
            values.Add(GraduatedChildNotRevealed);
            values.Add(GraduatedChildNotRecommended);
            values.Add(GraduatedChildUnknown);

            values.Add(GraduatedAdultInTARV);
            values.Add(GraduatedAdultNotInTARV);
            values.Add(GraduatedAdultNegative);
            values.Add(GraduatedAdultNotRevealed);
            values.Add(GraduatedAdultUnknown);

            values.Add(InitialChildInTARV);
            values.Add(InitialChildNotInTARV);
            values.Add(InitialChildNegative);
            values.Add(InitialChildNotRevealed);
            values.Add(InitialChildNotRecommended);
            values.Add(InitialChildUnknown);

            values.Add(InitialAdultInTARV);
            values.Add(InitialAdultNotInTARV);
            values.Add(InitialAdultNegative);
            values.Add(InitialAdultNotRevealed);
            values.Add(InitialAdultUnknown);

            return values;
        }
    }
}