using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VPPS.CSI.Domain
{
    public interface IAuditedEntity
    {
        // Sync

        // DateTime? SyncDate { get; set; }

        // Guid? SyncGuid { get; set; }

        // Audit

        DateTime? LastUpdatedDate { get; set; }

        DateTime? CreatedDate { get; set; }

        User CreatedUser { get; set; }

        User LastUpdatedUser { get; set; }
    }
}
