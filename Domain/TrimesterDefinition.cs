using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VPPS.CSI.Domain
{
    [Table("TrimesterDefinition")]
    public partial class TrimesterDefinition : AuditedEntity
    {
        public TrimesterDefinition()
        {

        }

        public int TrimesterDefinitionID { get; set; }
        public int FiscalYearVersion { get; set; }
        public int FirstDay { get; set; }
        public int LastDay { get; set; }
        public int FirstMonth { get; set; }
        public int LastMonth { get; set; }
        public int TrimesterSequence { get; set; }
    }
}
