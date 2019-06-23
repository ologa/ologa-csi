namespace VPPS.CSI.Domain
{
    using global::Domain.Resources;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using VPPS.CSI.Domain.CustomValidations;

    [Table("Beneficiary")]
    public partial class Beneficiary : AuditedEntity
    {
        public Beneficiary()
        {}

        //== Mandatory fileds ==//

        public int BeneficiaryID { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public Guid Beneficiary_guid { get; set; }

        [Timestamp]
        public byte[] RowVersion { get; set; }

        public int? CreatedUserID { get; set; }

        public int? LastUpdatedUserID { get; set; }

        //== Especific fileds ==//

        private string code;

        public string IDNumber { get; set; }

        private string nid;

        [StringLength(50)]
        public string NID { get { return nid; } set { this.nid = value; } }

        //[Required]
        [Display(Name = "Beneficiary_Code", ResourceType = typeof(LanguageResource))]
        public string Code { get { return code; } set { this.code = value; } }

        private string firstName;

        [Required]
        [StringLength(30)]
        [Display(Name = "Beneficiary_FirstName", ResourceType = typeof(LanguageResource))]
        public string FirstName { get { return firstName; } set { this.firstName = value; } }

        private string lastName;

        [Required]
        [StringLength(30)]
        [Display(Name = "Beneficiary_LastName", ResourceType = typeof(LanguageResource))]
        public string LastName { get { return lastName; } set { this.lastName = value; } }

        [Display(Name = "Beneficiary_FullName", ResourceType = typeof(LanguageResource))]
        public string FullName { get { return FirstName + " " + LastName; } }

        [Required]
        [StringLength(1)]
        [Display(Name = "Beneficiary_Gender", ResourceType = typeof(LanguageResource))]
        public string Gender { get; set; }

        [Display(Name = "Beneficiary_IsHouseHoldChef", ResourceType = typeof(LanguageResource))]
        public bool IsHouseHoldChef { get; set; }

        public int? MaritalStatusID { get; set; }

        [Required]
        [UIHint("Data")]
        [DataType(DataType.Date)]
        [RestrictedDate(ErrorMessageResourceType = typeof(LanguageResource), ErrorMessageResourceName = "DateBeforeCurrentDate")]
        [Display(Name = "Beneficiary_DateOfBirth", ResourceType = typeof(LanguageResource))]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DateOfBirth { get; set; }

        [Display(Name = "Beneficiary_DateOfBirthUnknown", ResourceType = typeof(LanguageResource))]
        public bool DateOfBirthUnknown { get; set; }

        [Display(Name = "Beneficiary_IsPartSavingGroup", ResourceType = typeof(LanguageResource))]
        public bool IsPartSavingGroup { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Beneficiary_RegistrationDate", ResourceType = typeof(LanguageResource))]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? RegistrationDate { get; set; }

        [Display(Name = "Beneficiary_RegistrationDateDifferentFromHouseholdDate", ResourceType = typeof(LanguageResource))]
        public bool RegistrationDateDifferentFromHouseholdDate { get; set; }

        [Display(Name = "Beneficiary_HIVTracked", ResourceType = typeof(LanguageResource))]
        public bool HIVTracked { get; set; }

        public int? OVCTypeID { get; set; }

        [Display(Name = "Beneficiary_OVCType", ResourceType = typeof(LanguageResource))]
        public virtual OVCType OVCType { get; set; }

        [Required]
        [Display(Name = "Beneficiary_KinshipToFamilyHead", ResourceType = typeof(LanguageResource))]
        public int KinshipToFamilyHeadID { get; set; }

        [Display(Name = "Beneficiary_KinshipToFamilyHead", ResourceType = typeof(LanguageResource))]
        public virtual SimpleEntity KinshipToFamilyHead { get; set; }

        [Display(Name = "Beneficiary_OtherKinshipToFamilyHead", ResourceType = typeof(LanguageResource))]
        public string OtherKinshipToFamilyHead { get; set; }

        [Required]
        public int HIVStatusID { get; set; }

        [Display(Name = "Beneficiary_HIVStatus", ResourceType = typeof(LanguageResource))]
        public virtual HIVStatus HIVStatus { get; set; }

        public virtual ICollection<ChildStatusHistory> ChildStatusHistories { get; set; }

        public string BeneficiaryStatus()
        {
            return ChildStatusHistories.OrderByDescending(x => x.EffectiveDate)
            .ThenByDescending(x => x.ChildStatusHistoryID).FirstOrDefault().ChildStatus.Description;
        }

        public int AgeInMonths { get { return AgeInMonthsMethod(DateTime.Now); } }

        public int AgeInMonthsMethod (DateTime? lastDate)
        {
            var months = (lastDate.Value.Year - DateOfBirth.Value.Year) * 12;
            months -= DateOfBirth.Value.Month + 1;
            months += lastDate.Value.Month;
            return months <= 0 ? 0 : months;
        }

        public int AgeInYears { get { return AgeInYearsMethod(null); } }

        public int AgeInYearsMethod(DateTime? date)
        {
            DateTime birthDate = DateOfBirth.Value;
            DateTime today = DateTime.Today;

            if(date != null)
            { today = date.Value; }
            int age = today.Year - birthDate.Year;
            return age = (today.Month < birthDate.Month || (today.Month == birthDate.Month && today.Day < birthDate.Day)) ? (age--) : age;
        }
    }
}
