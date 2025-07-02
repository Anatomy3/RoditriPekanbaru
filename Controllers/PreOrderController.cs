using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RoditriPekanbaru.Data;
using RoditriPekanbaru.Models;

namespace RoditriPekanbaru.Controllers
{
    public class PreOrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PreOrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: PreOrder
        public async Task<IActionResult> Index()
        {
            var preOrders = await _context.PreOrders
                .Include(p => p.Customer)
                .Include(p => p.Mobil)
                .OrderByDescending(p => p.TanggalDibuat)
                .ToListAsync();

            return View(preOrders);
        }

        // GET: PreOrder/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var preOrder = await _context.PreOrders
                .Include(p => p.Customer)
                .Include(p => p.Mobil)
                .FirstOrDefaultAsync(m => m.PreOrderId == id);

            if (preOrder == null)
            {
                return NotFound();
            }

            return View(preOrder);
        }

        // GET: PreOrder/Create
        public IActionResult Create()
        {
            PopulateDropdowns();
            return View();
        }

        // POST: PreOrder/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CustomerId,MobilId,TanggalPreOrder,JumlahDP,Status,Catatan")] PreOrder preOrder)
        {
            if (ModelState.IsValid)
            {
                // Set default values
                preOrder.TanggalDibuat = DateTime.Now;
                preOrder.UpdatedBy = "admin"; // In production, use actual logged user

                _context.Add(preOrder);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Pre-order berhasil dibuat!";
                return RedirectToAction(nameof(Index));
            }

            PopulateDropdowns(preOrder);
            return View(preOrder);
        }

        // GET: PreOrder/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var preOrder = await _context.PreOrders.FindAsync(id);
            if (preOrder == null)
            {
                return NotFound();
            }

            PopulateDropdowns(preOrder);
            return View(preOrder);
        }

        // POST: PreOrder/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PreOrderId,CustomerId,MobilId,TanggalPreOrder,JumlahDP,Status,Catatan,TanggalDibuat")] PreOrder preOrder)
        {
            if (id != preOrder.PreOrderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Update tracking fields
                    preOrder.TanggalUpdate = DateTime.Now;
                    preOrder.UpdatedBy = "admin"; // In production, use actual logged user

                    _context.Update(preOrder);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Pre-order berhasil diupdate!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PreOrderExists(preOrder.PreOrderId))
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

            PopulateDropdowns(preOrder);
            return View(preOrder);
        }

        // GET: PreOrder/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var preOrder = await _context.PreOrders
                .Include(p => p.Customer)
                .Include(p => p.Mobil)
                .FirstOrDefaultAsync(m => m.PreOrderId == id);

            if (preOrder == null)
            {
                return NotFound();
            }

            return View(preOrder);
        }

        // POST: PreOrder/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var preOrder = await _context.PreOrders.FindAsync(id);
            if (preOrder != null)
            {
                _context.PreOrders.Remove(preOrder);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Pre-order berhasil dihapus!";
            }

            return RedirectToAction(nameof(Index));
        }

        // AJAX: Get Mobil Price by ID
        [HttpGet]
        public async Task<JsonResult> GetMobilPrice(int mobilId)
        {
            var mobil = await _context.Mobils.FindAsync(mobilId);
            if (mobil != null)
            {
                return Json(new
                {
                    harga = mobil.Harga,
                    namaMobil = mobil.NamaMobil
                });
            }
            return Json(new { harga = 0, namaMobil = "" });
        }

        // Action untuk mengubah status PreOrder
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var preOrder = await _context.PreOrders.FindAsync(id);
            if (preOrder == null)
            {
                return Json(new { success = false, message = "Pre-order tidak ditemukan" });
            }

            preOrder.Status = status;
            preOrder.TanggalUpdate = DateTime.Now;
            preOrder.UpdatedBy = "admin"; // In production, use actual logged user

            try
            {
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = $"Status berhasil diubah menjadi {status}" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Gagal mengubah status: " + ex.Message });
            }
        }

        // Helper Methods
        private bool PreOrderExists(int id)
        {
            return _context.PreOrders.Any(e => e.PreOrderId == id);
        }

        private void PopulateDropdowns(PreOrder? preOrder = null)
        {
            // Dropdown untuk Customer
            ViewData["CustomerId"] = new SelectList(
                _context.Customers.OrderBy(c => c.NamaCustomer),
                "CustomerId",
                "NamaCustomer",
                preOrder?.CustomerId
            );

            // Dropdown untuk Mobil (hanya yang available)
            ViewData["MobilId"] = new SelectList(
                _context.Mobils.Where(m => m.IsAvailable).OrderBy(m => m.Merek),
                "MobilId",
                "NamaMobil",
                preOrder?.MobilId
            );

            // Dropdown untuk Status
            var statusList = new List<SelectListItem>
            {
                new SelectListItem { Value = "Pending", Text = "Pending" },
                new SelectListItem { Value = "Approved", Text = "Approved" },
                new SelectListItem { Value = "Rejected", Text = "Rejected" },
                new SelectListItem { Value = "Completed", Text = "Completed" }
            };

            ViewData["StatusList"] = new SelectList(statusList, "Value", "Text", preOrder?.Status);
        }
    }
}