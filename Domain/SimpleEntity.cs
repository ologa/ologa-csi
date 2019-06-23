using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VPPS.CSI.Domain
{
    [Table("SimpleEntity")]
    public class SimpleEntity : AuditedEntity
    {
        public Guid SimpleEntityGuid { get; set; }

        public int SimpleEntityID { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        [Required]
        [StringLength(20)]
        public string Type { get; set; }

        [Required]
        [StringLength(20)]
        public string Code { get; set; }

        [StringLength(200)]
        public string Description { get; set; }

        public int? CreatedUserID { get; set; }

        public int? LastUpdatedUserID { get; set; }
    }
}
