using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDataAccess.DTO
{
    public class ReferencesListWithStatusDTO
    {
        public string PartnerName { get; set; }
        public string ChiefPartnerName { get; set; }
        public string BeneficiaryName { get; set; }
        public string ReferenceProvider { get; set; }
        public string ReferenceName { get; set; }
        public string ReferenceDate { get; set; }
        public string InProgress { get; set; }
        public string IsComplete { get; set; }
        public List<string> PopulatedValues { get; set; } = new List<string>();

        public List<string> populatedValues()
        {
            PopulatedValues.Add(PartnerName);
            PopulatedValues.Add(ChiefPartnerName);
            PopulatedValues.Add(BeneficiaryName);
            PopulatedValues.Add(ReferenceProvider);
            PopulatedValues.Add(ReferenceName);
            PopulatedValues.Add(ReferenceDate);
            PopulatedValues.Add(InProgress);
            PopulatedValues.Add(IsComplete);

            return PopulatedValues;
        }
    }
}
