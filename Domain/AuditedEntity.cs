namespace VPPS.CSI.Domain
{
    using global::Domain.Resources;
    using System;
    using System.ComponentModel.DataAnnotations;

    public partial class AuditedEntity : SyncableEntity, IAuditedEntity
    {
        // (Activo 0, Inactivo 1, Deletado 2, Bloqueado 3)
        [Display(Name = "AuditedEntity_State", ResourceType = typeof(LanguageResource))]
        public int State { get; set; }

        [Display(Name = "AuditedEntity_CreatedDate", ResourceType = typeof(LanguageResource))]
        public DateTime? CreatedDate { get; set; }

        [Display(Name = "AuditedEntity_LastUpdatedDate", ResourceType = typeof(LanguageResource))]
        public DateTime? LastUpdatedDate { get; set; }

        [Display(Name = "AuditedEntity_CreatedUser", ResourceType = typeof(LanguageResource))]
        public virtual User CreatedUser { get; set; }

        [Display(Name = "AuditedEntity_LastUpdatedUser", ResourceType = typeof(LanguageResource))]
        public virtual User LastUpdatedUser { get; set; }

        public Guid CreatedUserGuid { get { return CreatedUser == null ? new Guid() : CreatedUser.User_guid; } }

        public Guid LastUpdatedUserGuid { get { return LastUpdatedUser == null ? new Guid() : LastUpdatedUser.User_guid; } }
    }
}
