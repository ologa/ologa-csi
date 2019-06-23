using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VPPS.CSI.Domain;

namespace EFDataAccess.Repository
{
    public class ChildRepo : BaseRepository<ChildPartner>, IRepoChild 
    {
        public ChildRepo(DbContext context):base(context)
        {

        }

        public List<object> GetChildPartnerInfo()
        {
            var results = (from b in GetAll().Include("Partner")
                           group b by b.Partner.PartnerID into g
                           select new
                           {
                               item = g.FirstOrDefault().Partner,
                               active = g.Count(x => x.Active == true),
                               inactive = g.Count(x => x.Active == false)
                           }).ToList<object>();

            return results;

        }
    }
}
