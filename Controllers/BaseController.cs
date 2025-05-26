using Microsoft.AspNetCore.Mvc;

namespace RoditriPekanbaru.Controllers
{
    public class BaseController : Controller
    {
        // Check if user is logged in
        protected bool IsAuthenticated()
        {
            return HttpContext.Session.GetString("Username") != null;
        }

        // Check if user is admin
        protected bool IsAdmin()
        {
            return HttpContext.Session.GetString("Level") == "Admin";
        }

        // Get current user info
        protected string? GetCurrentUsername()
        {
            return HttpContext.Session.GetString("Username");
        }

        protected string? GetCurrentUserLevel()
        {
            return HttpContext.Session.GetString("Level");
        }

        protected string? GetCurrentUserFullName()
        {
            return HttpContext.Session.GetString("NamaLengkap");
        }

        // Check authentication and redirect if not logged in
        protected IActionResult? CheckAuthentication()
        {
            if (!IsAuthenticated())
            {
                return RedirectToAction("Login", "Auth");
            }
            return null;
        }

        // Check admin access and redirect if not admin
        protected IActionResult? CheckAdminAccess()
        {
            var authCheck = CheckAuthentication();
            if (authCheck != null) return authCheck;

            if (!IsAdmin())
            {
                // Redirect non-admin users to landing page
                return RedirectToAction("Index", "Landing");
            }
            return null;
        }
    }
}