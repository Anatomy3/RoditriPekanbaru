using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoditriPekanbaru.Data;
using RoditriPekanbaru.Models;

namespace RoditriPekanbaru.Controllers
{
    public class BannerController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public BannerController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        // GET: Banner
        public async Task<IActionResult> Index()
        {
            // Check admin access
            var accessCheck = CheckAdminAccess();
            if (accessCheck != null) return accessCheck;

            var banners = await _context.Banners
                .OrderBy(b => b.Urutan)
                .ThenByDescending(b => b.TanggalDibuat)
                .ToListAsync();

            return View(banners);
        }

        // GET: Banner/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var accessCheck = CheckAdminAccess();
            if (accessCheck != null) return accessCheck;

            if (id == null)
            {
                return NotFound();
            }

            var banner = await _context.Banners
                .FirstOrDefaultAsync(m => m.BannerId == id);

            if (banner == null)
            {
                return NotFound();
            }

            return View(banner);
        }

        // GET: Banner/Create
        public IActionResult Create()
        {
            var accessCheck = CheckAdminAccess();
            if (accessCheck != null) return accessCheck;

            var banner = new Banner
            {
                DibuatOleh = GetCurrentUserFullName() ?? "Admin",
                TanggalMulai = DateTime.Now,
                TanggalBerakhir = DateTime.Now.AddMonths(1)
            };

            return View(banner);
        }

        // POST: Banner/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NamaBanner,LinkTujuan,Urutan,IsActive,TanggalMulai,TanggalBerakhir,DibuatOleh")] Banner banner, IFormFile gambarFile)
        {
            var accessCheck = CheckAdminAccess();
            if (accessCheck != null) return accessCheck;

            // Remove validation for GambarBanner since it's from file upload
            ModelState.Remove("GambarBanner");

            if (ModelState.IsValid)
            {
                banner.TanggalDibuat = DateTime.Now;
                banner.DibuatOleh = GetCurrentUserFullName() ?? "Admin";

                // Process image upload - REQUIRED
                if (gambarFile != null && gambarFile.Length > 0)
                {
                    // Validate file type
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                    var fileExtension = Path.GetExtension(gambarFile.FileName).ToLowerInvariant();

                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        ModelState.AddModelError("", "Format file tidak didukung. Gunakan JPG, PNG, GIF, atau WEBP");
                        return View(banner);
                    }

                    // Check file size (max 5MB)
                    if (gambarFile.Length > 5 * 1024 * 1024)
                    {
                        ModelState.AddModelError("", "Ukuran file maksimal 5MB");
                        return View(banner);
                    }

                    // Create unique filename
                    var fileName = Path.GetFileName(gambarFile.FileName);
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + fileName;

                    // Ensure directory exists
                    var uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "uploads", "banner");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await gambarFile.CopyToAsync(fileStream);
                    }

                    // Save the relative path to the database
                    banner.GambarBanner = "/uploads/banner/" + uniqueFileName;
                }
                else
                {
                    ModelState.AddModelError("", "Gambar banner harus diupload");
                    return View(banner);
                }

                _context.Add(banner);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Banner berhasil ditambahkan!";
                return RedirectToAction(nameof(Index));
            }

            // If model is not valid, return with errors
            return View(banner);
        }

        // GET: Banner/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var accessCheck = CheckAdminAccess();
            if (accessCheck != null) return accessCheck;

            if (id == null)
            {
                return NotFound();
            }

            var banner = await _context.Banners.FindAsync(id);
            if (banner == null)
            {
                return NotFound();
            }
            return View(banner);
        }

        // POST: Banner/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BannerId,Judul,Subjudul,Deskripsi,GambarBanner,TextButton,LinkButton,Urutan,IsActive,TanggalMulai,TanggalBerakhir,WarnaBackground,WarnaText,TanggalDibuat,DibuatOleh")] Banner banner, IFormFile gambarFile)
        {
            var accessCheck = CheckAdminAccess();
            if (accessCheck != null) return accessCheck;

            if (id != banner.BannerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Process image upload
                    if (gambarFile != null && gambarFile.Length > 0)
                    {
                        // Validate file type
                        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                        var fileExtension = Path.GetExtension(gambarFile.FileName).ToLowerInvariant();

                        if (!allowedExtensions.Contains(fileExtension))
                        {
                            ModelState.AddModelError("GambarBanner", "Format file tidak didukung. Gunakan JPG, PNG, GIF, atau WEBP");
                            return View(banner);
                        }

                        // Delete old image if exists
                        if (!string.IsNullOrEmpty(banner.GambarBanner))
                        {
                            var oldImagePath = Path.Combine(_hostEnvironment.WebRootPath, banner.GambarBanner.TrimStart('/'));
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        // Create unique filename
                        var fileName = Path.GetFileName(gambarFile.FileName);
                        var uniqueFileName = Guid.NewGuid().ToString() + "_" + fileName;

                        // Ensure directory exists
                        var uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "uploads", "banner");
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await gambarFile.CopyToAsync(fileStream);
                        }

                        // Save the relative path to the database
                        banner.GambarBanner = "/uploads/banner/" + uniqueFileName;
                    }

                    _context.Update(banner);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Banner berhasil diupdate!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BannerExists(banner.BannerId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(banner);
        }

        // GET: Banner/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var accessCheck = CheckAdminAccess();
            if (accessCheck != null) return accessCheck;

            if (id == null)
            {
                return NotFound();
            }

            var banner = await _context.Banners
                .FirstOrDefaultAsync(m => m.BannerId == id);
            if (banner == null)
            {
                return NotFound();
            }

            return View(banner);
        }

        // POST: Banner/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var accessCheck = CheckAdminAccess();
            if (accessCheck != null) return accessCheck;

            var banner = await _context.Banners.FindAsync(id);
            if (banner != null)
            {
                // Delete image file if exists
                if (!string.IsNullOrEmpty(banner.GambarBanner))
                {
                    var imagePath = Path.Combine(_hostEnvironment.WebRootPath, banner.GambarBanner.TrimStart('/'));
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }

                _context.Banners.Remove(banner);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Banner berhasil dihapus!";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Banner/ToggleStatus/5
        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var accessCheck = CheckAdminAccess();
            if (accessCheck != null) return Json(new { success = false, message = "Unauthorized" });

            var banner = await _context.Banners.FindAsync(id);
            if (banner == null)
            {
                return Json(new { success = false, message = "Banner tidak ditemukan" });
            }

            banner.IsActive = !banner.IsActive;
            await _context.SaveChangesAsync();

            return Json(new
            {
                success = true,
                message = $"Banner {(banner.IsActive ? "diaktifkan" : "dinonaktifkan")}",
                isActive = banner.IsActive
            });
        }

        private bool BannerExists(int id)
        {
            return _context.Banners.Any(e => e.BannerId == id);
        }
    }
}