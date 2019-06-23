using Microsoft.AspNet.Identity;
using Microsoft.VisualBasic.ApplicationServices;
using System.Web.Mvc;

public class AllowAttribute : AuthorizeAttribute
{
    public override void OnAuthorization(AuthorizationContext filterContext)
    {
        if (filterContext.HttpContext.Request.IsAuthenticated)
        {
            // If they are authorized, handle accordingly
            if (this.AuthorizeCore(filterContext.HttpContext))
            {
                base.OnAuthorization(filterContext);
            }
            else
            {
                // Otherwise redirect to your specific authorized area
                filterContext.Result = new RedirectResult("~/Account/NotAllowed");
            }
        }
        else
        {
            filterContext.Result = new RedirectResult("~/Account/Login");
        }
    }
}