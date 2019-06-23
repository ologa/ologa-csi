using System;

namespace VPPS.CSI.Domain
{
    public class HIVStatusDTO
    {
        public int HIVStatusID { get; set; }
        public Guid HIVStatus_guid { get; set; }
        public int SyncState { get; set; }

        public string HIV { get; set; }
        public int HIVInTreatment { get; set; }
        public int HIVUndisclosedReason { get; set; }
        public string NID { get; set; }

        public DateTime InformationDate { get; set; }
        public DateTime CreatedAt { get; set; }

        public int AdultID { get; set; }
        public int ChildID { get; set; }

        public Guid BeneficiaryGuid { get; set; }
        public Guid AdultGuid { get; set; }
        public Guid ChildGuid { get; set; }
        public Guid CreatedUserGuid { get; set; }
    }
}
