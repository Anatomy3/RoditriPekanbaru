using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoditriPekanbaru.Data;
using RoditriPekanbaru.ViewModels;
using RoditriPekanbaru.Models;

namespace RoditriPekanbaru.Controllers
{
    public class LandingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LandingController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Landing/Index
        public async Task<IActionResult> Index(string? merek, string? search, decimal? minHarga, decimal? maxHarga, int? tahunMin, int? tahunMax, int page = 1)
        {
            try
            {
                // Get active banners
                var banners = await _context.Banners
                    .Where(b => b.IsActive &&
                               (b.TanggalMulai == null || b.TanggalMulai <= DateTime.Now) &&
                               (b.TanggalBerakhir == null || b.TanggalBerakhir >= DateTime.Now))
                    .OrderBy(b => b.Urutan)
                    .ThenByDescending(b => b.TanggalDibuat)
                    .ToListAsync();

                ViewBag.Banners = banners;

                // Get all available brands from database
                var brands = await _context.Mobils
                    .Where(m => m.IsAvailable)
                    .Select(m => m.Merek)
                    .Distinct()
                    .OrderBy(m => m)
                    .ToListAsync();

                // Build query for cars
                var carsQuery = _context.Mobils.Where(m => m.IsAvailable);

                // Apply filters
                if (!string.IsNullOrEmpty(merek))
                {
                    carsQuery = carsQuery.Where(m => m.Merek.Contains(merek));
                }

                if (!string.IsNullOrEmpty(search))
                {
                    carsQuery = carsQuery.Where(m =>
                        m.Merek.Contains(search) ||
                        m.Model.Contains(search) ||
                        m.Warna.Contains(search));
                }

                if (minHarga.HasValue)
                {
                    carsQuery = carsQuery.Where(m => m.Harga >= minHarga.Value);
                }

                if (maxHarga.HasValue)
                {
                    carsQuery = carsQuery.Where(m => m.Harga <= maxHarga.Value);
                }

                if (tahunMin.HasValue)
                {
                    carsQuery = carsQuery.Where(m => m.Tahun >= tahunMin.Value);
                }

                if (tahunMax.HasValue)
                {
                    carsQuery = carsQuery.Where(m => m.Tahun <= tahunMax.Value);
                }

                // Pagination
                const int pageSize = 12;
                var totalCars = await carsQuery.CountAsync();
                var totalPages = (int)Math.Ceiling(totalCars / (double)pageSize);

                var cars = await carsQuery
                    .OrderByDescending(m => m.TanggalInput)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                var viewModel = new LandingViewModel
                {
                    Brands = brands ?? new List<string>(),
                    Cars = cars ?? new List<Mobil>(),
                    CurrentPage = page,
                    TotalPages = totalPages,
                    TotalCars = totalCars,

                    // Current filters
                    SelectedMerek = merek,
                    SearchTerm = search,
                    MinHarga = minHarga,
                    MaxHarga = maxHarga,
                    TahunMin = tahunMin,
                    TahunMax = tahunMax
                };

                return View(viewModel);
            }
            catch (Exception)
            {
                // Return empty view model if there's an error
                ViewBag.Banners = new List<Banner>();

                var emptyViewModel = new LandingViewModel
                {
                    Brands = new List<string>(),
                    Cars = new List<Mobil>(),
                    CurrentPage = 1,
                    TotalPages = 1,
                    TotalCars = 0
                };

                return View(emptyViewModel);
            }
        }

        // GET: /Landing/CarDetails/5
        public async Task<IActionResult> CarDetails(int id)
        {
            if (id <= 0)
            {
                return RedirectToAction("Index");
            }

            var car = await _context.Mobils
                .FirstOrDefaultAsync(m => m.MobilId == id && m.IsAvailable);

            if (car == null)
            {
                // Return view with null model to show "not found" message
                return View((Mobil?)null);
            }

            return View(car);
        }

        // GET: /Landing/Search
        public async Task<IActionResult> Search(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return RedirectToAction("Index");
            }

            try
            {
                var cars = await _context.Mobils
                    .Where(m => m.IsAvailable &&
                               (m.Merek.Contains(query) ||
                                m.Model.Contains(query) ||
                                m.Warna.Contains(query)))
                    .OrderByDescending(m => m.TanggalInput)
                    .ToListAsync();

                ViewBag.SearchQuery = query;
                return View("SearchResults", cars ?? new List<Mobil>());
            }
            catch (Exception)
            {
                ViewBag.SearchQuery = query;
                return View("SearchResults", new List<Mobil>());
            }
        }
    }
}