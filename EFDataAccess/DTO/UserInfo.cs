using System;

namespace EFDataAccess.DTO
{
    public class UserInfo
    {
        public int UserID { get; set; }
        public String UserName { get; set; }
        public int OrgUnitID { get; set; }
        public String SiteName { get; set; }
    }
}
