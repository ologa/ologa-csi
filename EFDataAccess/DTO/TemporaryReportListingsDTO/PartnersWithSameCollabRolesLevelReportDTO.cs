using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDataAccess.DTO.TemporaryReportListingsDTO
{
    public class PartnersWithSameCollabRolesLevelReportDTO
    {
        public String SuperiorName { get; set; }
        public String SuperiorLevel { get; set; }
        public String PartnerName { get; set; }
        public String PartnerLevel { get; set; }

        public List<Object> values { get; set; } = new List<Object>();

        public List<Object> populatedValues()
        {
            values.Add(SuperiorName);
            values.Add(SuperiorLevel);
            values.Add(PartnerName);
            values.Add(PartnerLevel);

            return values;
        }
    }
}