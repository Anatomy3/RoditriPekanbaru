using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoditriPekanbaru.Data;
using RoditriPekanbaru.Models;

namespace RoditriPekanbaru.Controllers
{
    public class CustomerController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public CustomerController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Customer
        public async Task<IActionResult> Index()
        {
            // Check admin access
            var accessCheck = CheckAdminAccess();
            if (accessCheck != null) return accessCheck;

            return View(await _context.Customers.ToListAsync());
        }

        // GET: Customer/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var accessCheck = CheckAdminAccess();
            if (accessCheck != null) return accessCheck;

            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.CustomerId == id);

            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // GET: Customer/Create
        public IActionResult Create()
        {
            var accessCheck = CheckAdminAccess();
            if (accessCheck != null) return accessCheck;

            return View();
        }

        // POST: Customer/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NamaCustomer,Alamat,NoTelepon,Email,JenisKelamin,Pekerjaan")] Customer customer)
        {
            var accessCheck = CheckAdminAccess();
            if (accessCheck != null) return accessCheck;

            if (ModelState.IsValid)
            {
                customer.TanggalDaftar = DateTime.Now;
                _context.Add(customer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        // GET: Customer/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var accessCheck = CheckAdminAccess();
            if (accessCheck != null) return accessCheck;

            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        // POST: Customer/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CustomerId,NamaCustomer,Alamat,NoTelepon,Email,JenisKelamin,Pekerjaan,TanggalDaftar")] Customer customer)
        {
            var accessCheck = CheckAdminAccess();
            if (accessCheck != null) return accessCheck;

            if (id != customer.CustomerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.CustomerId))
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
            return View(customer);
        }

        // GET: Customer/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var accessCheck = CheckAdminAccess();
            if (accessCheck != null) return accessCheck;

            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.CustomerId == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Customer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var accessCheck = CheckAdminAccess();
            if (accessCheck != null) return accessCheck;

            var customer = await _context.Customers.FindAsync(id);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CustomerExists(int id)
        {
            return _context.Customers.Any(e => e.CustomerId == id);
        }
    }
}