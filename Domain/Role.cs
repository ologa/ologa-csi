using Domain.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VPPS.CSI.Domain
{
    [Table("Role")]
    public class Role
    {
        public int RoleID { get; set; }

        [StringLength(200)]
        [Display(Name = "Role_Description", ResourceType = typeof(LanguageResource))]
        public string Description { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public Guid Role_guid { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}
