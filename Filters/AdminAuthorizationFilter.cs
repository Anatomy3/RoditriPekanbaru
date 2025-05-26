using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace RoditriPekanbaru.Filters
{
    public class AdminAuthorizationFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var httpContext = context.HttpContext;
            var username = httpContext.Session.GetString("Username");
            var level = httpContext.Session.GetString("Level");

            if (string.IsNullOrEmpty(username))
            {
                context.Result = new RedirectToActionResult("Login", "Auth", null);
                return;
            }

            if (level != "Admin")
            {
                context.Result = new RedirectToActionResult("Index", "Landing", null);
                return;
            }

            base.OnActionExecuting(context);
        }
    }
}