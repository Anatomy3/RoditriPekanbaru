using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoditriPekanbaru.Data;
using RoditriPekanbaru.Models;

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

        // ===== TAMBAHAN BARU: CUSTOMER SESSION MANAGEMENT =====

        // Get current customer ID from session
        protected int? GetCurrentCustomerId()
        {
            var customerIdStr = HttpContext.Session.GetString("CustomerId");
            if (!string.IsNullOrEmpty(customerIdStr) && int.TryParse(customerIdStr, out int customerId))
            {
                return customerId;
            }
            return null;
        }

        // Get current customer name from session
        protected string? GetCurrentCustomerName()
        {
            return HttpContext.Session.GetString("CustomerName");
        }

        // Get current user ID from session
        protected int? GetCurrentUserId()
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (!string.IsNullOrEmpty(userIdStr) && int.TryParse(userIdStr, out int userId))
            {
                return userId;
            }
            return null;
        }

        // Helper method to get customer from session (requires DbContext injection in derived controller)
        protected async Task<Customer?> GetCurrentCustomerAsync(ApplicationDbContext context)
        {
            var customerId = GetCurrentCustomerId();
            if (customerId.HasValue && customerId.Value > 0)
            {
                return await context.Customers.FindAsync(customerId.Value);
            }
            return null;
        }

        // Helper method to check if user has valid customer session
        protected bool HasValidCustomerSession()
        {
            var customerId = GetCurrentCustomerId();
            return customerId.HasValue && customerId.Value > 0;
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

        // ===== TAMBAHAN BARU: CUSTOMER ACCESS CHECK =====

        // Check if user is logged in and has customer session
        protected IActionResult? CheckCustomerAccess()
        {
            var authCheck = CheckAuthentication();
            if (authCheck != null) return authCheck;

            if (!HasValidCustomerSession())
            {
                TempData["ErrorMessage"] = "Session customer tidak valid. Silakan login ulang.";
                return RedirectToAction("Login", "Auth");
            }
            return null;
        }

        // ===== HELPER METHODS UNTUK SESSION MANAGEMENT =====

        // Set customer session data
        protected void SetCustomerSession(Customer customer)
        {
            if (customer != null)
            {
                HttpContext.Session.SetString("CustomerId", customer.CustomerId.ToString());
                HttpContext.Session.SetString("CustomerName", customer.NamaCustomer);
            }
        }

        // Clear customer session data (keep admin session)
        protected void ClearCustomerSession()
        {
            HttpContext.Session.Remove("CustomerId");
            HttpContext.Session.Remove("CustomerName");
        }

        // Get all session data for debugging
        protected Dictionary<string, string?> GetAllSessionData()
        {
            return new Dictionary<string, string?>
            {
                {"UserId", HttpContext.Session.GetString("UserId")},
                {"Username", HttpContext.Session.GetString("Username")},
                {"NamaLengkap", HttpContext.Session.GetString("NamaLengkap")},
                {"Level", HttpContext.Session.GetString("Level")},
                {"CustomerId", HttpContext.Session.GetString("CustomerId")},
                {"CustomerName", HttpContext.Session.GetString("CustomerName")}
            };
        }

        // ===== EXTENDED HELPER METHODS =====

        // Check if current user can access specific customer data
        protected bool CanAccessCustomerData(int customerId)
        {
            if (IsAdmin())
            {
                return true; // Admin can access all customer data
            }

            var currentCustomerId = GetCurrentCustomerId();
            return currentCustomerId.HasValue && currentCustomerId.Value == customerId;
        }

        // Get redirect URL based on user level
        protected string GetDashboardUrl()
        {
            if (IsAdmin())
            {
                return Url.Action("Index", "Admin") ?? "/Admin";
            }
            else
            {
                return Url.Action("Index", "Landing") ?? "/";
            }
        }

        // Check if user can perform pre-order
        protected bool CanPerformPreOrder()
        {
            return IsAuthenticated() && (HasValidCustomerSession() || IsAdmin());
        }
    }
}