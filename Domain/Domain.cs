namespace VPPS.CSI.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
   

    [Table("Domain")]
    public partial class DomainEntity 
    {
        //== Mandatory fileds ==//

        [Key]
        public int DomainID { get; set; }

        public Guid domain_guid { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        //== Especific fileds ==//

        [Required]
        [StringLength(100)]
        public string Description { get; set; }

        [StringLength(5)]
        public string DomainCode { get; set; }

        [StringLength(30)]
        public string DomainColor { get; set; }

        [Column(TypeName = "varchar(max)")]
        public string Guidelines { get; set; }

        public int Order { get; set; }

        public int OrderForRoutineVisit { get; set; }
    }
}
