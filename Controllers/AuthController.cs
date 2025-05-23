// Controllers/AuthController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoditriPekanbaru.Data;
using RoditriPekanbaru.Models;
using RoditriPekanbaru.ViewModels;

namespace RoditriPekanbaru.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthController(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        // GET: /Auth/Login
        public IActionResult Login()
        {
            // Redirect if already logged in
            if (HttpContext.Session.GetString("Username") != null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: /Auth/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Simple login for presentation (no password hashing)
                var admin = await _context.Admins
                    .FirstOrDefaultAsync(a => a.Username == model.Username && a.Password == model.Password);

                if (admin != null)
                {
                    // Set session
                    HttpContext.Session.SetString("UserId", admin.AdminId.ToString());
                    HttpContext.Session.SetString("Username", admin.Username);
                    HttpContext.Session.SetString("NamaLengkap", admin.NamaLengkap);
                    HttpContext.Session.SetString("Level", admin.Level);

                    // Update last login time
                    admin.LastLogin = DateTime.Now;
                    await _context.SaveChangesAsync();

                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "Username atau password salah");
            }

            return View(model);
        }

        // GET: /Auth/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        // Helper method to check if user is logged in
        private bool IsAuthenticated()
        {
            return HttpContext.Session.GetString("Username") != null;
        }
    }
}