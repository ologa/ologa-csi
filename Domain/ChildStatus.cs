namespace VPPS.CSI.Domain
{
    using global::Domain.Resources;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class ChildStatus : AuditedEntity
    {
        [Key]
        public int StatusID { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "BeneficiaryStatus_Description", ResourceType = typeof(LanguageResource))]
        public string Description { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public Guid childstatus_guid { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public override bool Equals(object obj)
        {            
            if (obj == null)
            {
                return false;
            }

            if (Object.ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            var item = obj as ChildStatus;            

            return this.StatusID.Equals(item.StatusID);
        }

        public override int GetHashCode()
        {
            return this.StatusID.GetHashCode();
        }

        public int? CreatedUserID { get; set; }

        public int? LastUpdatedUserID { get; set; }
    }
}
