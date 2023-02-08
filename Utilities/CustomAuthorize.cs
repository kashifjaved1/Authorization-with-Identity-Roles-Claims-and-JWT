using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using System.Net.Http;

namespace IdentityNetCore.Utilities
{
    // writing custom attribute to do authorize/role check using session.
    public class CustomAuthorize : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Session.GetString("Department") != "IT")
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary
                {
                    { "Controller", "Account" },
                    { "Action", "SignIn" }
                });
            }
        }
    }
}
