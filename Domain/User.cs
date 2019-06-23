using Domain.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VPPS.CSI.Domain
{
    [Table("User")]
    public partial class User : SyncableEntity
    {
        public User() { passwordStr = "*#==/==#*"; }

        public int UserID { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public Guid User_guid { get; set; }

        [Required]
        [StringLength(10)]
        [Display(Name = "User_Username", ResourceType = typeof(LanguageResource))]
        public string Username { get; set; }

        [Required]
        public byte[] Password { get; set; }

        [Required]
        [NotMapped]
        [DisplayName("Password")]
        private string passwordStr { get; set; }

        [Required]
        [NotMapped]
        [DisplayName("Password")]
        public string PasswordStr {
            get { return passwordStr; }
            set { passwordStr = value; if(!"*#==/==#*".Equals(value)) Password = Encryptor.Encrypt(value); }
        }

        public string PasswordBase64 { get { return Convert.ToBase64String(Password); } }

        [Required]
        [StringLength(20)]
        [Display(Name = "User_FirstName", ResourceType = typeof(LanguageResource))]
        public string FirstName { get; set; }

        [Required]
        [StringLength(30)]
        [Display(Name = "User_LastName", ResourceType = typeof(LanguageResource))]
        public string LastName { get; set; }

        public bool Admin { get; set; }

        [Display(Name = "User_IsOCBUser", ResourceType = typeof(LanguageResource))]
        public bool IsOCBUser { get; set; }

        [Display(Name = "User_Site", ResourceType = typeof(LanguageResource))]
        public int DefSite { get; set; }

        [Display(Name = "User_LoggedOn", ResourceType = typeof(LanguageResource))]
        public bool LoggedOn { get; set; }

        [Column(TypeName = "DateTime2")]
        [Display(Name = "User_LastLogin", ResourceType = typeof(LanguageResource))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? LastLoginDate { get; set; }

        [Column(TypeName = "DateTime2")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "User_InactiveDaysExceededDate", ResourceType = typeof(LanguageResource))]
        public DateTime? InactiveDaysExceededDate { get; set; }

        [Display(Name = "User_FullName", ResourceType = typeof(LanguageResource))]
        public string FullName { get { return String.Format("{0} {1}", FirstName, LastName); } }

        [Display(Name = "User_Role", ResourceType = typeof(LanguageResource))]
        public virtual Role Role { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public virtual ICollection<UserRole> UserRoles { get; set; }

        // Sync

        public int AdminInt { get { return Admin ? 1 : 0; } }

        public int IsOCBUserInt { get { return IsOCBUser ? 1 : 0; } }

        public int LoggedOnInt { get { return LoggedOn ? 1 : 0; } }

        [Display(Name = "User_Role", ResourceType = typeof(LanguageResource))]
        public int? RoleID { get; set; }

        // Audit

        [Display(Name = "AuditedEntity_CreatedDate", ResourceType = typeof(LanguageResource))]
        public DateTime? CreatedDate { get; set; }

        [Display(Name = "AuditedEntity_LastUpdatedDate", ResourceType = typeof(LanguageResource))]
        public DateTime? LastUpdatedDate { get; set; }
    }
}
