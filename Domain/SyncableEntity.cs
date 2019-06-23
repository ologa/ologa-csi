namespace VPPS.CSI.Domain
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;


    public partial class SyncableEntity
    {
        // (NeverSynced -1, DataChanged 0, Synced 1)
        [DisplayName("Estado de Sincronização")]
        public int SyncState { get; set; }

        [DisplayName("Data de Sincronização")]
        public DateTime? SyncDate { get; set; }

        public Guid? SyncGuid { get; set; }
    }
}
