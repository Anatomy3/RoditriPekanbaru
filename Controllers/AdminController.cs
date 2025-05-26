using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoditriPekanbaru.Data;
using RoditriPekanbaru.Models;

namespace RoditriPekanbaru.Controllers
{
    public class AdminController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin
        public async Task<IActionResult> Index()
        {
            // Check admin access
            var accessCheck = CheckAdminAccess();
            if (accessCheck != null) return accessCheck;

            return View(await _context.Admins.ToListAsync());
        }

        // GET: Admin/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var accessCheck = CheckAdminAccess();
            if (accessCheck != null) return accessCheck;

            if (id == null)
            {
                return NotFound();
            }

            var admin = await _context.Admins
                .FirstOrDefaultAsync(m => m.AdminId == id);

            if (admin == null)
            {
                return NotFound();
            }

            return View(admin);
        }

        // GET: Admin/Create
        public IActionResult Create()
        {
            var accessCheck = CheckAdminAccess();
            if (accessCheck != null) return accessCheck;

            return View();
        }

        // POST: Admin/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Username,Password,NamaLengkap,Level,IsActive")] Admin admin)
        {
            var accessCheck = CheckAdminAccess();
            if (accessCheck != null) return accessCheck;

            if (ModelState.IsValid)
            {
                // Check if username already exists
                var existingAdmin = await _context.Admins
                    .FirstOrDefaultAsync(a => a.Username == admin.Username);

                if (existingAdmin != null)
                {
                    ModelState.AddModelError("Username", "Username sudah digunakan");
                    return View(admin);
                }

                // Validate level - only Admin or User allowed
                if (admin.Level != "Admin" && admin.Level != "User")
                {
                    admin.Level = "User"; // Default to User if invalid level
                }

                admin.CreatedDate = DateTime.Now;
                _context.Add(admin);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(admin);
        }

        // GET: Admin/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var accessCheck = CheckAdminAccess();
            if (accessCheck != null) return accessCheck;

            if (id == null)
            {
                return NotFound();
            }

            var admin = await _context.Admins.FindAsync(id);
            if (admin == null)
            {
                return NotFound();
            }
            return View(admin);
        }

        // POST: Admin/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AdminId,Username,Password,NamaLengkap,Level,IsActive,CreatedDate")] Admin admin)
        {
            var accessCheck = CheckAdminAccess();
            if (accessCheck != null) return accessCheck;

            if (id != admin.AdminId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Validate level - only Admin or User allowed
                    if (admin.Level != "Admin" && admin.Level != "User")
                    {
                        admin.Level = "User"; // Default to User if invalid level
                    }

                    _context.Update(admin);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdminExists(admin.AdminId))
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
            return View(admin);
        }

        // GET: Admin/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var accessCheck = CheckAdminAccess();
            if (accessCheck != null) return accessCheck;

            if (id == null)
            {
                return NotFound();
            }

            var admin = await _context.Admins
                .FirstOrDefaultAsync(m => m.AdminId == id);
            if (admin == null)
            {
                return NotFound();
            }

            return View(admin);
        }

        // POST: Admin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var accessCheck = CheckAdminAccess();
            if (accessCheck != null) return accessCheck;

            var admin = await _context.Admins.FindAsync(id);
            if (admin != null)
            {
                // Prevent deleting the last admin
                var adminCount = await _context.Admins.CountAsync(a => a.Level == "Admin" && a.IsActive);
                if (admin.Level == "Admin" && adminCount <= 1)
                {
                    ModelState.AddModelError("", "Tidak dapat menghapus admin terakhir");
                    return View("Delete", admin);
                }

                _context.Admins.Remove(admin);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AdminExists(int id)
        {
            return _context.Admins.Any(e => e.AdminId == id);
        }
    }
}