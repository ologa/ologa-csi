using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace VPPS.CSI.Domain
{
    [Table("Trimester")]
    public partial class Trimester : AuditedEntity
    {
        public int TrimesterID { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Seq { get; set; }
        public TrimesterDefinition TrimesterDefinition { get; set; }
        public int? TrimesterDefinitionID { get; set; }
        public string TrimesterDescription { get; set; }
    }
}
