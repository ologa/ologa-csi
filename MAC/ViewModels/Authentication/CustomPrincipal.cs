using System;
using System.Security.Principal;

namespace MAC.ViewModels.Authentication
{
    public class CustomPrincipal : System.Security.Principal.IPrincipal
    {
        public IIdentity Identity { get; private set; }

        string FirstName { get; set; }

        string LastName { get; set; }

        string SiteName { get; set; }

        string District { get; set; }

        string Province { get; set; }

        public CustomPrincipal(string username)
        {
            this.Identity = new GenericIdentity(username);
        }

        public bool IsInRole(string role)
        {
            return Identity != null && Identity.IsAuthenticated &&
               !string.IsNullOrWhiteSpace(role); //&& Roles.IsUserInRole(Identity.Name, role);
        }

        public string FullName { get { return FirstName + " " + LastName; } }

        IIdentity IPrincipal.Identity
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }

    public class CustomPrincipalSerializedModel
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        string SiteName { get; set; }

        string District { get; set; }

        string Province { get; set; }
    }
}