using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDataAccess.CustomQueryObject
{
    public class HouseholdCQO
    {
        public int HouseHoldID { get; set; }

        public int OfficialHouseholdIdentifierNumber { get; set; }

        public string HouseholdName { get; set; }

        public string PrincipalChiefName { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? RegistrationDate { get; set; }

        public string Address { get; set; }

        public string NeighborhoodName { get; set; }

        public string Block { get; set; }

        public string HouseNumber { get; set; }

        public Guid HouseholdUniqueIdentifier { get; set; }

        public DateTime LastUpdatedDate { get; set; }

        public int CreatedUserID { get; set; }

        public int LastUpdatedUserID { get; set; }

        public int? OrgUnitID { get; set; }

        public int? PartnerID { get; set; }

        public int TotalAdult { get; set; }

        public int TotalChild { get; set; }
    }
}
