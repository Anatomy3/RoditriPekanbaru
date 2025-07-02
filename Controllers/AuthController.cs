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
                    // Set admin session
                    HttpContext.Session.SetString("UserId", admin.AdminId.ToString());
                    HttpContext.Session.SetString("Username", admin.Username);
                    HttpContext.Session.SetString("NamaLengkap", admin.NamaLengkap);
                    HttpContext.Session.SetString("Level", admin.Level);

                    // ===== SOLUSI UTAMA: LINK ADMIN DENGAN CUSTOMER =====
                    await SetupCustomerSession(admin);

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

                // ===== SOLUSI: AUTO LOGIN SETELAH REGISTER =====
                // Set admin session
                HttpContext.Session.SetString("UserId", newUser.AdminId.ToString());
                HttpContext.Session.SetString("Username", newUser.Username);
                HttpContext.Session.SetString("NamaLengkap", newUser.NamaLengkap);
                HttpContext.Session.SetString("Level", newUser.Level);

                // Setup customer session untuk user baru
                await SetupCustomerSession(newUser);

                // Redirect ke landing page, bukan ke login
                return RedirectToAction("Index", "Landing");
            }

            return View(model);
        }

        // GET: /Auth/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Landing");
        }

        // ===== METODE UTAMA UNTUK SINKRONISASI ADMIN-CUSTOMER =====
        private async Task SetupCustomerSession(Admin admin)
        {
            try
            {
                // 1. Cari customer berdasarkan nama lengkap admin
                var existingCustomer = await _context.Customers
                    .FirstOrDefaultAsync(c => c.NamaCustomer.ToLower() == admin.NamaLengkap.ToLower());

                Customer customer;

                if (existingCustomer != null)
                {
                    // 2a. Jika customer sudah ada, gunakan yang ada
                    customer = existingCustomer;
                }
                else
                {
                    // 2b. Jika customer belum ada, create customer baru
                    customer = new Customer
                    {
                        NamaCustomer = admin.NamaLengkap,
                        Alamat = "Alamat akan diupdate saat pre-order", // Default alamat
                        NoTelepon = "000000000000", // Default phone, akan diupdate
                        Email = $"{admin.Username}@roditripekanbaru.com", // Generate email
                        JenisKelamin = "Laki-laki", // Default, bisa diupdate nanti
                        Pekerjaan = "Tidak diketahui", // Default
                        TanggalDaftar = DateTime.Now
                    };

                    _context.Customers.Add(customer);
                    await _context.SaveChangesAsync();
                }

                // 3. Simpan CustomerId ke session
                HttpContext.Session.SetString("CustomerId", customer.CustomerId.ToString());
                HttpContext.Session.SetString("CustomerName", customer.NamaCustomer);

                // 4. Optional: Simpan data customer lain yang mungkin diperlukan
                HttpContext.Session.SetString("CustomerEmail", customer.Email ?? "");
                HttpContext.Session.SetString("CustomerPhone", customer.NoTelepon);
            }
            catch (Exception ex)
            {
                // Log error tapi jangan gagalkan login
                // In production, use proper logging
                Console.WriteLine($"Error setting up customer session: {ex.Message}");

                // Set default customer session jika ada error
                HttpContext.Session.SetString("CustomerId", "0");
                HttpContext.Session.SetString("CustomerName", admin.NamaLengkap);
            }
        }

        // ===== METODE HELPER UNTUK MENDAPATKAN ATAU MEMBUAT CUSTOMER =====
        public async Task<Customer> GetOrCreateCustomerForCurrentUser()
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            var customerIdStr = HttpContext.Session.GetString("CustomerId");

            // Jika sudah ada CustomerId di session, coba ambil customer
            if (!string.IsNullOrEmpty(customerIdStr) && int.TryParse(customerIdStr, out int customerId) && customerId > 0)
            {
                var existingCustomer = await _context.Customers.FindAsync(customerId);
                if (existingCustomer != null)
                {
                    return existingCustomer;
                }
            }

            // Jika tidak ada atau tidak ditemukan, buat berdasarkan admin yang login
            if (!string.IsNullOrEmpty(userIdStr) && int.TryParse(userIdStr, out int userId))
            {
                var admin = await _context.Admins.FindAsync(userId);
                if (admin != null)
                {
                    await SetupCustomerSession(admin);

                    // Ambil customer yang baru dibuat/ditemukan
                    var newCustomerIdStr = HttpContext.Session.GetString("CustomerId");
                    if (!string.IsNullOrEmpty(newCustomerIdStr) && int.TryParse(newCustomerIdStr, out int newCustomerId))
                    {
                        return await _context.Customers.FindAsync(newCustomerId);
                    }
                }
            }

            // Fallback: return null jika tidak bisa create
            return null;
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

        // Helper method to get current customer ID from session
        public int? GetCurrentCustomerId()
        {
            var customerIdStr = HttpContext.Session.GetString("CustomerId");
            if (!string.IsNullOrEmpty(customerIdStr) && int.TryParse(customerIdStr, out int customerId))
            {
                return customerId;
            }
            return null;
        }
    }
}