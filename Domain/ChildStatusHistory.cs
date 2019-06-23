namespace VPPS.CSI.Domain
{
    using global::Domain.Resources;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    

    [Table("ChildStatusHistory")]
    public partial class ChildStatusHistory : AuditedEntity
    {
        //== Mandatory fileds ==//

        public int ChildStatusHistoryID { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public Guid childstatushistory_guid { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public int? CreatedUserID { get; set; }

        public int? LastUpdatedUserID { get; set; }

        //== Especific fileds ==//

        public Guid BeneficiaryGuid { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "BeneficiaryStatus_RegistrationDate", ResourceType = typeof(LanguageResource))]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? EffectiveDate { get; set; }

        public virtual Child Child { get; set; }

        public int? ChildID { get; set; }

        public virtual Adult Adult { get; set; }

        public int? AdultID { get; set; }

        public virtual Beneficiary Beneficiary { get; set; }

        public int? BeneficiaryID { get; set; }

        public virtual ChildStatus ChildStatus { get; set; }

        public int? ChildStatusID { get; set; }

        // Related data to sync

        public Guid ChildGuid { get { return Child == null ? new Guid() : Child.child_guid; } }

        public Guid AdultGuid { get { return Adult == null ? new Guid() : Adult.AdultGuid; } }

        public Guid Beneficiary_guid { get { return Beneficiary == null ? new Guid() : Beneficiary.Beneficiary_guid; } }
    }
}
