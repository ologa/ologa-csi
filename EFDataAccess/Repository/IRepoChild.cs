using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VPPS.CSI.Domain;

namespace EFDataAccess.Repository
{
    public interface IRepoChild : IRepository<ChildPartner>
    {
        List<Object> GetChildPartnerInfo();
    }
}
