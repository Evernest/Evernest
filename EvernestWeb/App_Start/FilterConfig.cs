using System.Web;
using System.Web.Mvc;

using EvernestWeb.Application;

namespace EvernestWeb
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new SessionAuthorize());
        }
    }
}
