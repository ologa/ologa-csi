namespace VPPS.CSI.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("OVCType")]
    public partial class OVCType
    {
        public int OVCTypeID { get; set; }

        [Required]
        [StringLength(50)]
        public string Description { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public Guid ovctype_guid { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }
    }
}
