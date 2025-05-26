using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoditriPekanbaru.Data;
using RoditriPekanbaru.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace RoditriPekanbaru.Controllers
{
    public class MobilController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public MobilController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        // GET: Mobil
        public async Task<IActionResult> Index()
        {
            // Check admin access
            var accessCheck = CheckAdminAccess();
            if (accessCheck != null) return accessCheck;

            return View(await _context.Mobils.ToListAsync());
        }

        // GET: Mobil/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var accessCheck = CheckAdminAccess();
            if (accessCheck != null) return accessCheck;

            if (id == null)
            {
                return NotFound();
            }

            var mobil = await _context.Mobils
                .FirstOrDefaultAsync(m => m.MobilId == id);

            if (mobil == null)
            {
                return NotFound();
            }

            return View(mobil);
        }

        // GET: Mobil/Create
        public IActionResult Create()
        {
            var accessCheck = CheckAdminAccess();
            if (accessCheck != null) return accessCheck;

            return View();
        }

        // POST: Mobil/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Merek,Model,Tahun,Warna,Harga,Kondisi,NoRangka,NoMesin,NoPolisi")] Mobil mobil, IFormFile gambarFile)
        {
            var accessCheck = CheckAdminAccess();
            if (accessCheck != null) return accessCheck;

            if (ModelState.IsValid)
            {
                mobil.TanggalInput = DateTime.Now;
                mobil.IsAvailable = true;

                // Process image upload
                if (gambarFile != null && gambarFile.Length > 0)
                {
                    // Create unique filename
                    var fileName = Path.GetFileName(gambarFile.FileName);
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + fileName;

                    // Ensure directory exists
                    var uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "uploads", "mobil");
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
                    mobil.GambarMobil = "/uploads/mobil/" + uniqueFileName;
                }

                _context.Add(mobil);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(mobil);
        }

        // GET: Mobil/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var accessCheck = CheckAdminAccess();
            if (accessCheck != null) return accessCheck;

            if (id == null)
            {
                return NotFound();
            }

            var mobil = await _context.Mobils.FindAsync(id);
            if (mobil == null)
            {
                return NotFound();
            }
            return View(mobil);
        }

        // POST: Mobil/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MobilId,Merek,Model,Tahun,Warna,Harga,Kondisi,IsAvailable,NoRangka,NoMesin,NoPolisi,TanggalInput,GambarMobil")] Mobil mobil, IFormFile gambarFile)
        {
            var accessCheck = CheckAdminAccess();
            if (accessCheck != null) return accessCheck;

            if (id != mobil.MobilId)
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
                        // Delete old image if exists
                        if (!string.IsNullOrEmpty(mobil.GambarMobil))
                        {
                            var oldImagePath = Path.Combine(_hostEnvironment.WebRootPath, mobil.GambarMobil.TrimStart('/'));
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        // Create unique filename
                        var fileName = Path.GetFileName(gambarFile.FileName);
                        var uniqueFileName = Guid.NewGuid().ToString() + "_" + fileName;

                        // Ensure directory exists
                        var uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, "uploads", "mobil");
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
                        mobil.GambarMobil = "/uploads/mobil/" + uniqueFileName;
                    }

                    _context.Update(mobil);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MobilExists(mobil.MobilId))
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
            return View(mobil);
        }

        // GET: Mobil/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var accessCheck = CheckAdminAccess();
            if (accessCheck != null) return accessCheck;

            if (id == null)
            {
                return NotFound();
            }

            var mobil = await _context.Mobils
                .FirstOrDefaultAsync(m => m.MobilId == id);
            if (mobil == null)
            {
                return NotFound();
            }

            return View(mobil);
        }

        // POST: Mobil/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var accessCheck = CheckAdminAccess();
            if (accessCheck != null) return accessCheck;

            var mobil = await _context.Mobils.FindAsync(id);
            if (mobil != null)
            {
                // Delete image file if exists
                if (!string.IsNullOrEmpty(mobil.GambarMobil))
                {
                    var imagePath = Path.Combine(_hostEnvironment.WebRootPath, mobil.GambarMobil.TrimStart('/'));
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }

                _context.Mobils.Remove(mobil);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MobilExists(int id)
        {
            return _context.Mobils.Any(e => e.MobilId == id);
        }
    }
}