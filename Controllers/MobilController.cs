using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoditriPekanbaru.Data;
using RoditriPekanbaru.Models;

namespace RoditriPekanbaru.Controllers
{
    public class MobilController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MobilController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Mobil
        public async Task<IActionResult> Index()
        {
            // Check if user is logged in
            if (HttpContext.Session.GetString("Username") == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            return View(await _context.Mobils.ToListAsync());
        }

        // GET: Mobil/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (HttpContext.Session.GetString("Username") == null)
            {
                return RedirectToAction("Login", "Auth");
            }

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
            if (HttpContext.Session.GetString("Username") == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            return View();
        }

        // POST: Mobil/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Merek,Model,Tahun,Warna,Harga,Kondisi,NoRangka,NoMesin,NoPolisi")] Mobil mobil)
        {
            if (HttpContext.Session.GetString("Username") == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            if (ModelState.IsValid)
            {
                mobil.TanggalInput = DateTime.Now;
                mobil.IsAvailable = true;
                _context.Add(mobil);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(mobil);
        }

        // GET: Mobil/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (HttpContext.Session.GetString("Username") == null)
            {
                return RedirectToAction("Login", "Auth");
            }

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
        public async Task<IActionResult> Edit(int id, [Bind("MobilId,Merek,Model,Tahun,Warna,Harga,Kondisi,IsAvailable,NoRangka,NoMesin,NoPolisi,TanggalInput")] Mobil mobil)
        {
            if (HttpContext.Session.GetString("Username") == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            if (id != mobil.MobilId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
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
            if (HttpContext.Session.GetString("Username") == null)
            {
                return RedirectToAction("Login", "Auth");
            }

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
            if (HttpContext.Session.GetString("Username") == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var mobil = await _context.Mobils.FindAsync(id);
            if (mobil != null)
            {
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