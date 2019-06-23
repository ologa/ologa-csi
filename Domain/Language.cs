namespace VPPS.CSI.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("Language")]
    public partial class Language
    {
        public int LanguageID { get; set; }

        [Required]
        [StringLength(30)]
        public string Description { get; set; }

        public Guid language_guid { get; set; }
    }
}
