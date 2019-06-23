using Domain.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VPPS.CSI.Domain.CustomValidations;

namespace VPPS.CSI.Domain
{
    [Table("HIVStatus")]
    public class HIVStatus : SyncableEntity
    {
        // HIV field values
        public const string UNKNOWN = "U";
        public const string POSITIVE = "P";
        public const string NEGATIVE = "N";
        public const string INDETERMINATE = "Z";

        // HIV treatment values
        public const int IN_TARV = 0;
        public const int NOT_IN_TARV = 1;
        public const int NOTAPPLICABLE_TARV = -1;

        // HIV Undisclosed reason values
        public const int NOT_REVEALED = 0;
        public const int NOT_KNOWN = 1;
        public const int NOT_RECOMMENDED = 2;

        // Constractor
        public HIVStatus()
        {
            CreatedAt = DateTime.Now;
        }

        // Key fields
        
        public int HIVStatusID { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public Guid HIVStatus_guid { get; set; }

        public Guid BeneficiaryGuid { get; set; }

        // Remain fields

        [Required]
        [StringLength(1)]
        [DisplayName("HIV")]
        public string HIV { get; set; }

        [RequiredIf("HIV", "P", "Deve escolher se está em tratamento, caso esta Positiva")]
        [DisplayName("Em Tratamento")]
        public int? HIVInTreatment { get; set; }
        
        [DataType(DataType.Date)]
        [Display(Name = "HIV_TARV_Date", ResourceType = typeof(LanguageResource))]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? TarvInitialDate { get; set; }

        [RequiredIf("HIV", "U", "Deve escolher o caso de Desconhecimento, caso seje desconhecido")]
        [DisplayName("Desconhecido Porque")]
        public int? HIVUndisclosedReason { get; set; }
        
        [DataType(DataType.Date)]
        [Display(Name = "Beneficiary_HIVStatus_Date", ResourceType = typeof(LanguageResource))]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? InformationDate { get; set; }

        [DisplayName("NID")]
        public string NID { get; set; }

        public DateTime CreatedAt { get; set; }

        public int AdultID { get; set; }

        public int ChildID { get; set; }

        public int BeneficiaryID { get; set; }

        public virtual User User { get; set; }

        public int? UserID { get; set; }

        // Related data to sync

        [NotMapped]
        public Guid AdultGuid { get; set; }

        [NotMapped]
        public Guid ChildGuid { get; set; }

        public Guid CreatedUserGuid { get { return User == null ? new Guid() : User.User_guid; } }
    }
}
