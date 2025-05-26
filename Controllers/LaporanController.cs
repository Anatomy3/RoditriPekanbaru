using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoditriPekanbaru.Data;
using RoditriPekanbaru.ViewModels;

namespace RoditriPekanbaru.Controllers
{
    public class LaporanController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public LaporanController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Laporan
        public async Task<IActionResult> Index(DateTime? startDate, DateTime? endDate)
        {
            // Check admin access
            var accessCheck = CheckAdminAccess();
            if (accessCheck != null) return accessCheck;

            // Set default date range if not provided
            if (!startDate.HasValue)
                startDate = DateTime.Now.AddMonths(-1);
            if (!endDate.HasValue)
                endDate = DateTime.Now;

            // Get transactions within date range
            var transaksi = await _context.TransaksiPenjualans
                .Include(t => t.Customer)
                .Include(t => t.Mobil)
                .Where(t => t.TanggalTransaksi >= startDate && t.TanggalTransaksi <= endDate)
                .OrderByDescending(t => t.TanggalTransaksi)
                .ToListAsync();

            // Calculate summary
            var totalTransaksi = transaksi.Count;
            var totalPendapatan = transaksi.Sum(t => t.TotalBayar);
            var totalLunas = transaksi.Count(t => t.StatusPembayaran == "Lunas");
            var totalBelumLunas = transaksi.Count(t => t.StatusPembayaran == "Belum Lunas");

            var viewModel = new LaporanViewModel
            {
                Transaksi = transaksi,
                StartDate = startDate.Value,
                EndDate = endDate.Value,
                TotalTransaksi = totalTransaksi,
                TotalPendapatan = totalPendapatan,
                TotalLunas = totalLunas,
                TotalBelumLunas = totalBelumLunas
            };

            return View(viewModel);
        }
    }
}