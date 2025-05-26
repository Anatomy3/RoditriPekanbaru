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
                return RedirectToAction("Index", "Landing");
            }

            // Always return a valid model
            var model = new LoginViewModel();
            return View(model);
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
                    .FirstOrDefaultAsync(a => a.Username == model.Username && a.Password == model.Password && a.IsActive);

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

                    // Redirect to landing page instead of admin dashboard
                    return RedirectToAction("Index", "Landing");
                }

                ModelState.AddModelError("", "Username atau password salah");
            }

            return View(model);
        }

        // GET: /Auth/Register
        public IActionResult Register()
        {
            // Always return a valid model
            var model = new RegisterViewModel();
            return View(model);
        }

        // POST: /Auth/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if username already exists
                var existingUser = await _context.Admins
                    .FirstOrDefaultAsync(a => a.Username == model.Username);

                if (existingUser != null)
                {
                    ModelState.AddModelError("Username", "Username sudah digunakan");
                    return View(model);
                }

                var newUser = new Admin
                {
                    Username = model.Username,
                    Password = model.Password, // In production, hash this
                    NamaLengkap = model.NamaLengkap,
                    Level = "User", // Default level for registration
                    IsActive = true,
                    CreatedDate = DateTime.Now
                };

                _context.Add(newUser);
                await _context.SaveChangesAsync();

                return RedirectToAction("Login");
            }

            return View(model);
        }

        // GET: /Auth/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Landing");
        }

        // Helper method to check if user is logged in
        private bool IsAuthenticated()
        {
            return HttpContext.Session.GetString("Username") != null;
        }

        // Helper method to check if user is admin
        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("Level") == "Admin";
        }
    }
}