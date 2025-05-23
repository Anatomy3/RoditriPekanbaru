using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RoditriPekanbaru.Data;
using RoditriPekanbaru.Models;
using RoditriPekanbaru.ViewModels;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RoditriPekanbaru.Controllers
{
    public class TransaksiController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TransaksiController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Transaksi
        public async Task<IActionResult> Index()
        {
            // Check if user is logged in
            if (HttpContext.Session.GetString("Username") == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var transaksi = await _context.TransaksiPenjualans
                .Include(t => t.Customer)
                .Include(t => t.Mobil)
                .OrderByDescending(t => t.TanggalTransaksi)
                .ToListAsync();

            return View(transaksi);
        }

        // GET: Transaksi/Details/5
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

            var transaksi = await _context.TransaksiPenjualans
                .Include(t => t.Customer)
                .Include(t => t.Mobil)
                .FirstOrDefaultAsync(m => m.TransaksiId == id);

            if (transaksi == null)
            {
                return NotFound();
            }

            return View(transaksi);
        }

        // GET: Transaksi/Create
        public IActionResult Create()
        {
            if (HttpContext.Session.GetString("Username") == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "NamaCustomer");
            ViewData["MobilId"] = new SelectList(_context.Mobils.Where(m => m.IsAvailable), "MobilId", "NamaMobil");

            var model = new TransaksiPenjualan
            {
                TanggalTransaksi = DateTime.Now,
                NoFaktur = GenerateInvoiceNumber(),
                Admin = HttpContext.Session.GetString("NamaLengkap") ?? "Admin"
            };

            return View(model);
        }

        // POST: Transaksi/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NoFaktur,TanggalTransaksi,CustomerId,MobilId,HargaJual,Diskon,TotalBayar,UangMuka,SisaPembayaran,StatusPembayaran,CaraPembayaran,Keterangan,Admin,StatusMobil")] TransaksiPenjualan transaksi)
        {
            if (HttpContext.Session.GetString("Username") == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            if (ModelState.IsValid)
            {
                // Calculate total payment
                transaksi.TotalBayar = transaksi.HargaJual - transaksi.Diskon;
                transaksi.SisaPembayaran = transaksi.TotalBayar - transaksi.UangMuka;

                // Check if payment is completed
                if (transaksi.SisaPembayaran <= 0)
                {
                    transaksi.StatusPembayaran = "Lunas";
                }
                else
                {
                    transaksi.StatusPembayaran = "Belum Lunas";
                }

                // Update Car status to sold
                var mobil = await _context.Mobils.FindAsync(transaksi.MobilId);
                if (mobil != null)
                {
                    mobil.IsAvailable = false;
                    _context.Update(mobil);
                }

                _context.Add(transaksi);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "NamaCustomer", transaksi.CustomerId);
            ViewData["MobilId"] = new SelectList(_context.Mobils.Where(m => m.IsAvailable), "MobilId", "NamaMobil", transaksi.MobilId);
            return View(transaksi);
        }

        // GET: Transaksi/Edit/5
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

            var transaksi = await _context.TransaksiPenjualans.FindAsync(id);
            if (transaksi == null)
            {
                return NotFound();
            }

            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "NamaCustomer", transaksi.CustomerId);
            ViewData["MobilId"] = new SelectList(_context.Mobils, "MobilId", "NamaMobil", transaksi.MobilId);
            return View(transaksi);
        }

        // POST: Transaksi/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TransaksiId,NoFaktur,TanggalTransaksi,CustomerId,MobilId,HargaJual,Diskon,TotalBayar,UangMuka,SisaPembayaran,StatusPembayaran,CaraPembayaran,Keterangan,Admin,StatusMobil")] TransaksiPenjualan transaksi)
        {
            if (HttpContext.Session.GetString("Username") == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            if (id != transaksi.TransaksiId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Calculate total payment
                    transaksi.TotalBayar = transaksi.HargaJual - transaksi.Diskon;
                    transaksi.SisaPembayaran = transaksi.TotalBayar - transaksi.UangMuka;

                    // Check if payment is completed
                    if (transaksi.SisaPembayaran <= 0)
                    {
                        transaksi.StatusPembayaran = "Lunas";
                    }
                    else
                    {
                        transaksi.StatusPembayaran = "Belum Lunas";
                    }

                    _context.Update(transaksi);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TransaksiPenjualanExists(transaksi.TransaksiId))
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
            ViewData["CustomerId"] = new SelectList(_context.Customers, "CustomerId", "NamaCustomer", transaksi.CustomerId);
            ViewData["MobilId"] = new SelectList(_context.Mobils, "MobilId", "NamaMobil", transaksi.MobilId);
            return View(transaksi);
        }

        // GET: Transaksi/Delete/5
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

            var transaksi = await _context.TransaksiPenjualans
                .Include(t => t.Customer)
                .Include(t => t.Mobil)
                .FirstOrDefaultAsync(m => m.TransaksiId == id);
            if (transaksi == null)
            {
                return NotFound();
            }

            return View(transaksi);
        }

        // POST: Transaksi/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (HttpContext.Session.GetString("Username") == null)
            {
                return RedirectToAction("Login", "Auth");
            }

            var transaksi = await _context.TransaksiPenjualans.FindAsync(id);
            if (transaksi != null)
            {
                // Update Car availability status back to true
                var mobil = await _context.Mobils.FindAsync(transaksi.MobilId);
                if (mobil != null)
                {
                    mobil.IsAvailable = true;
                    _context.Update(mobil);
                }

                _context.TransaksiPenjualans.Remove(transaksi);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TransaksiPenjualanExists(int id)
        {
            return _context.TransaksiPenjualans.Any(e => e.TransaksiId == id);
        }

        // Helper method to generate invoice number
        private string GenerateInvoiceNumber()
        {
            var today = DateTime.Now;
            var prefix = $"INV-{today:yyMMdd}";

            // Get the last invoice number for today
            var lastInvoice = _context.TransaksiPenjualans
                .Where(t => t.NoFaktur.StartsWith(prefix))
                .OrderByDescending(t => t.NoFaktur)
                .FirstOrDefault();

            int sequence = 1;
            if (lastInvoice != null)
            {
                var lastSequence = lastInvoice.NoFaktur.Substring(prefix.Length + 1);
                if (int.TryParse(lastSequence, out int lastNumber))
                {
                    sequence = lastNumber + 1;
                }
            }

            return $"{prefix}-{sequence:D3}";
        }

        // Helper method to get car information when selecting car in form
        [HttpGet]
        public async Task<IActionResult> GetMobilInfo(int mobilId)
        {
            var mobil = await _context.Mobils.FindAsync(mobilId);
            if (mobil == null)
            {
                return NotFound();
            }

            return Json(new
            {
                harga = mobil.Harga,
                merek = mobil.Merek,
                model = mobil.Model,
                tahun = mobil.Tahun,
                warna = mobil.Warna,
                noPolisi = mobil.NoPolisi,
                noRangka = mobil.NoRangka,
                noMesin = mobil.NoMesin
            });
        }
    }
}