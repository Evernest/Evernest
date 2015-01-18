using System.Web;
using System.Web.Mvc;

using EvernestWeb.Models;

namespace EvernestWeb.Application
{
    /// <summary>
    /// SessionAuthorize adds to AuthorizeAttribute a check of session variable User.
    /// </summary>
    public class SessionAuthorize : AuthorizeAttribute
    {
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            // Run original authorization process and if it is ok, look for User session
            return
                base.AuthorizeCore(httpContext)
                && httpContext.Session["User"] != null
                && httpContext.Session["User"] is User;
        }
    }
}