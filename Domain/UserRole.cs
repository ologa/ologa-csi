using Domain.Resources;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VPPS.CSI.Domain
{
    [Table("UserRole")]
    public partial class UserRole
    {
        public int UserRoleID { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public Guid UserRole_guid { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public virtual User User { get; set; }

        public int UserID { get; set; }

        public virtual Role Role { get; set; }

        public int RoleID { get; set; }

        [Display(Name = "AuditedEntity_CreatedDate", ResourceType = typeof(LanguageResource))]
        public DateTime? CreatedDate { get; set; }
    }
}