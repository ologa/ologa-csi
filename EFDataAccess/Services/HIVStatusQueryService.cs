using EFDataAccess.DTO;
using EFDataAccess.UOW;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using VPPS.CSI.Domain;
using VPPS.CSI.Domain;

namespace EFDataAccess.Services
{
    public class HIVStatusQueryService : BaseService
    {
        public HIVStatusQueryService(UnitOfWork uow) : base(uow)
        {
            UnitOfWork = uow;
        }
        
        public List<UniqueEntity> FindAllHIVStatusUniqueEntity() => UnitOfWork.DbContext.Database.SqlQuery<UniqueEntity>("Select ISNULL(SyncGuid, cast(cast(0 as binary) as uniqueidentifier)) As SyncGuid, HIVStatusID As ID from HIVStatus").ToList();
    }
}
