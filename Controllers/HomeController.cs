using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoditriPekanbaru.Data;
using RoditriPekanbaru.Models;
using RoditriPekanbaru.ViewModels;
using System.Diagnostics;

namespace RoditriPekanbaru.Controllers
{
    public class HomeController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ApplicationDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: /Home/Index - Admin Dashboard
        public async Task<IActionResult> Index()
        {
            // Check admin access
            var accessCheck = CheckAdminAccess();
            if (accessCheck != null) return accessCheck;

            // Create dashboard view model for admin
            var dashboard = new DashboardViewModel
            {
                TotalCustomer = await _context.Customers.CountAsync(),
                TotalMobil = await _context.Mobils.CountAsync(),
                MobilTersedia = await _context.Mobils.CountAsync(m => m.IsAvailable),
                MobilTerjual = await _context.Mobils.CountAsync(m => !m.IsAvailable),
                TotalTransaksi = await _context.TransaksiPenjualans.CountAsync(),
                TotalPenjualan = await _context.TransaksiPenjualans.SumAsync(t => t.TotalBayar),

                // Get recent transactions
                TransaksiTerbaru = await _context.TransaksiPenjualans
                    .Include(t => t.Customer)
                    .Include(t => t.Mobil)
                    .OrderByDescending(t => t.TanggalTransaksi)
                    .Take(5)
                    .ToListAsync()
            };

            return View(dashboard);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // Helper methods for other controllers
        public static bool IsAdmin(HttpContext context)
        {
            return context.Session.GetString("Level") == "Admin";
        }

        public static bool IsAuthenticated(HttpContext context)
        {
            return context.Session.GetString("Username") != null;
        }
    }

    // Error ViewModel
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}