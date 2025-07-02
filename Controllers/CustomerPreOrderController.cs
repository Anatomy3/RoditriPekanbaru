using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RoditriPekanbaru.Data;
using RoditriPekanbaru.Models;

namespace RoditriPekanbaru.Controllers
{
    public class CustomerPreOrderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CustomerPreOrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: CustomerPreOrder/MyOrders (untuk user yang sudah login)
        public async Task<IActionResult> MyOrders()
        {
            // Cek apakah user sudah login
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                // Redirect ke TrackOrder jika belum login
                return RedirectToAction("TrackOrder");
            }

            try
            {
                // ===== SOLUSI UTAMA: GUNAKAN CUSTOMERID DARI SESSION =====
                var customerIdStr = HttpContext.Session.GetString("CustomerId");
                var customerName = HttpContext.Session.GetString("NamaLengkap");
                int customerId = 0; // Inisialisasi default value

                List<PreOrder> preOrders = new List<PreOrder>();

                if (!string.IsNullOrEmpty(customerIdStr) && int.TryParse(customerIdStr, out customerId) && customerId > 0)
                {
                    // Cari berdasarkan CustomerId (method utama)
                    preOrders = await _context.PreOrders
                        .Include(p => p.Customer)
                        .Include(p => p.Mobil)
                        .Where(p => p.CustomerId == customerId)
                        .OrderByDescending(p => p.TanggalDibuat)
                        .ToListAsync();
                }
                else
                {
                    // Fallback: cari berdasarkan nama customer (method lama)
                    preOrders = await _context.PreOrders
                        .Include(p => p.Customer)
                        .Include(p => p.Mobil)
                        .Where(p => p.Customer.NamaCustomer == customerName)
                        .OrderByDescending(p => p.TanggalDibuat)
                        .ToListAsync();
                }

                ViewBag.IsLoggedIn = true;
                ViewBag.CustomerName = customerName;
                ViewBag.CustomerId = customerId;

                return View("MyOrders", preOrders);
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = "Terjadi kesalahan saat mengambil data pesanan.";
                return View("MyOrders", new List<PreOrder>());
            }
        }

        // GET: CustomerPreOrder/Create/5 (mobilId) - HARUS LOGIN
        public async Task<IActionResult> Create(int? mobilId)
        {
            // Cek login terlebih dahulu
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                TempData["ErrorMessage"] = "Anda harus login terlebih dahulu untuk melakukan pre-order.";
                return RedirectToAction("Login", "Auth", new { returnUrl = Url.Action("Create", new { mobilId }) });
            }

            if (mobilId == null)
            {
                TempData["ErrorMessage"] = "Mobil tidak ditemukan.";
                return RedirectToAction("Index", "Landing");
            }

            var mobil = await _context.Mobils
                .FirstOrDefaultAsync(m => m.MobilId == mobilId && m.IsAvailable);

            if (mobil == null)
            {
                TempData["ErrorMessage"] = "Mobil tidak tersedia untuk pre-order.";
                return RedirectToAction("Index", "Landing");
            }

            // ===== SOLUSI: AUTO-FILL DARI SESSION DAN DATABASE =====
            var model = new PreOrderFormViewModel
            {
                MobilId = mobil.MobilId,
                NamaCustomer = HttpContext.Session.GetString("NamaLengkap") ?? "",
            };

            // Coba ambil data customer yang sudah ada dari session
            var customerIdStr = HttpContext.Session.GetString("CustomerId");
            if (!string.IsNullOrEmpty(customerIdStr) && int.TryParse(customerIdStr, out int customerId) && customerId > 0)
            {
                var existingCustomer = await _context.Customers.FindAsync(customerId);
                if (existingCustomer != null)
                {
                    // Fill form dengan data customer yang sudah ada
                    model.NamaCustomer = existingCustomer.NamaCustomer;
                    model.Alamat = existingCustomer.Alamat;
                    model.NoTelepon = existingCustomer.NoTelepon;
                    model.Email = existingCustomer.Email;
                    model.JenisKelamin = existingCustomer.JenisKelamin;
                    model.Pekerjaan = existingCustomer.Pekerjaan;
                }
            }

            ViewBag.Mobil = mobil;
            ViewBag.IsLoggedIn = true;
            return View(model);
        }

        // POST: CustomerPreOrder/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PreOrderFormViewModel model)
        {
            // Cek login
            var username = HttpContext.Session.GetString("Username");
            if (string.IsNullOrEmpty(username))
            {
                TempData["ErrorMessage"] = "Session expired. Silakan login kembali.";
                return RedirectToAction("Login", "Auth");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Get mobil details
                    var mobil = await _context.Mobils
                        .FirstOrDefaultAsync(m => m.MobilId == model.MobilId && m.IsAvailable);

                    if (mobil == null)
                    {
                        TempData["ErrorMessage"] = "Mobil tidak tersedia.";
                        return RedirectToAction("Index", "Landing");
                    }

                    // Validate DP amount
                    if (model.JumlahDP <= 0 || model.JumlahDP >= mobil.Harga)
                    {
                        ModelState.AddModelError("JumlahDP", "Jumlah DP harus lebih dari 0 dan kurang dari harga mobil.");
                        ViewBag.Mobil = mobil;
                        return View(model);
                    }

                    // ===== SOLUSI UTAMA: GUNAKAN CUSTOMERID DARI SESSION =====
                    Customer customer = null;
                    var customerIdStr = HttpContext.Session.GetString("CustomerId");

                    if (!string.IsNullOrEmpty(customerIdStr) && int.TryParse(customerIdStr, out int customerId) && customerId > 0)
                    {
                        // Cari customer berdasarkan CustomerId dari session
                        customer = await _context.Customers.FindAsync(customerId);

                        if (customer != null)
                        {
                            // Update data customer dengan input terbaru dari form
                            customer.NamaCustomer = model.NamaCustomer;
                            customer.Alamat = model.Alamat;
                            customer.NoTelepon = model.NoTelepon;
                            customer.Email = model.Email;
                            customer.JenisKelamin = model.JenisKelamin;
                            customer.Pekerjaan = model.Pekerjaan;

                            _context.Update(customer);
                            await _context.SaveChangesAsync();
                        }
                    }

                    // Jika customer tidak ditemukan berdasarkan session, cari berdasarkan nomor telepon
                    if (customer == null)
                    {
                        customer = await _context.Customers
                            .FirstOrDefaultAsync(c => c.NoTelepon == model.NoTelepon);
                    }

                    // Jika masih tidak ada, buat customer baru
                    if (customer == null)
                    {
                        customer = new Customer
                        {
                            NamaCustomer = model.NamaCustomer,
                            Alamat = model.Alamat,
                            NoTelepon = model.NoTelepon,
                            Email = model.Email,
                            JenisKelamin = model.JenisKelamin,
                            Pekerjaan = model.Pekerjaan,
                            TanggalDaftar = DateTime.Now
                        };

                        _context.Customers.Add(customer);
                        await _context.SaveChangesAsync();

                        // Update session dengan CustomerId yang baru dibuat
                        HttpContext.Session.SetString("CustomerId", customer.CustomerId.ToString());
                        HttpContext.Session.SetString("CustomerName", customer.NamaCustomer);
                    }

                    // Generate tracking number
                    var trackingNumber = GenerateTrackingNumber();

                    // Create pre-order
                    var preOrder = new PreOrder
                    {
                        CustomerId = customer.CustomerId, // Gunakan CustomerId yang benar
                        MobilId = model.MobilId,
                        TanggalPreOrder = DateTime.Now,
                        JumlahDP = model.JumlahDP,
                        Status = "Pending",
                        Catatan = $"Pre-order dari website oleh user: {username}. Nomor Tracking: {trackingNumber}. " +
                                 (!string.IsNullOrEmpty(model.Catatan) ? $"Catatan: {model.Catatan}" : ""),
                        TanggalDibuat = DateTime.Now,
                        UpdatedBy = username
                    };

                    _context.PreOrders.Add(preOrder);
                    await _context.SaveChangesAsync();

                    // Store tracking number in session for success page
                    TempData["TrackingNumber"] = trackingNumber;
                    TempData["PreOrderId"] = preOrder.PreOrderId;
                    TempData["CustomerName"] = customer.NamaCustomer;
                    TempData["MobilName"] = $"{mobil.Merek} {mobil.Model}";

                    return RedirectToAction("Success");
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Terjadi kesalahan saat memproses pre-order. Silakan coba lagi.";

                    var mobil = await _context.Mobils
                        .FirstOrDefaultAsync(m => m.MobilId == model.MobilId);
                    ViewBag.Mobil = mobil;

                    return View(model);
                }
            }

            // If model is not valid, return with mobil data
            var mobilData = await _context.Mobils
                .FirstOrDefaultAsync(m => m.MobilId == model.MobilId);
            ViewBag.Mobil = mobilData;

            return View(model);
        }

        // GET: CustomerPreOrder/Success
        public IActionResult Success()
        {
            if (TempData["TrackingNumber"] == null)
            {
                return RedirectToAction("Index", "Landing");
            }

            return View();
        }

        // GET: CustomerPreOrder/TrackOrder (untuk user belum login)
        public IActionResult TrackOrder()
        {
            return View();
        }

        // POST: CustomerPreOrder/TrackOrder
        [HttpPost]
        public async Task<IActionResult> TrackOrder(string trackingInput)
        {
            if (string.IsNullOrEmpty(trackingInput))
            {
                ViewBag.ErrorMessage = "Silakan masukkan nomor tracking atau nomor telepon.";
                return View();
            }

            try
            {
                List<PreOrder> preOrders;

                // Check if input is tracking number (contains "TRK") or phone number
                if (trackingInput.Contains("TRK"))
                {
                    // Search by tracking number in Catatan field
                    preOrders = await _context.PreOrders
                        .Include(p => p.Customer)
                        .Include(p => p.Mobil)
                        .Where(p => p.Catatan.Contains(trackingInput))
                        .OrderByDescending(p => p.TanggalDibuat)
                        .ToListAsync();
                }
                else
                {
                    // Search by phone number
                    preOrders = await _context.PreOrders
                        .Include(p => p.Customer)
                        .Include(p => p.Mobil)
                        .Where(p => p.Customer.NoTelepon.Contains(trackingInput))
                        .OrderByDescending(p => p.TanggalDibuat)
                        .ToListAsync();
                }

                if (!preOrders.Any())
                {
                    ViewBag.ErrorMessage = "Tidak ada pesanan ditemukan dengan nomor tracking atau telepon tersebut.";
                    return View();
                }

                ViewBag.SearchInput = trackingInput;
                return View("TrackResults", preOrders);
            }
            catch (Exception)
            {
                ViewBag.ErrorMessage = "Terjadi kesalahan saat mencari pesanan. Silakan coba lagi.";
                return View();
            }
        }

        // GET: CustomerPreOrder/Detail/5
        public async Task<IActionResult> Detail(int id, string? trackingNumber)
        {
            var preOrder = await _context.PreOrders
                .Include(p => p.Customer)
                .Include(p => p.Mobil)
                .FirstOrDefaultAsync(p => p.PreOrderId == id);

            if (preOrder == null)
            {
                TempData["ErrorMessage"] = "Pesanan tidak ditemukan.";
                return RedirectToAction("TrackOrder");
            }

            // Optional: Verify tracking number for additional security
            if (!string.IsNullOrEmpty(trackingNumber) &&
                !preOrder.Catatan.Contains(trackingNumber))
            {
                TempData["ErrorMessage"] = "Nomor tracking tidak valid.";
                return RedirectToAction("TrackOrder");
            }

            return View(preOrder);
        }

        // AJAX: Get Mobil Price
        [HttpGet]
        public async Task<JsonResult> GetMobilPrice(int mobilId)
        {
            try
            {
                var mobil = await _context.Mobils.FindAsync(mobilId);
                if (mobil != null)
                {
                    return Json(new
                    {
                        success = true,
                        harga = mobil.Harga,
                        namaMobil = $"{mobil.Merek} {mobil.Model}"
                    });
                }
                return Json(new { success = false, message = "Mobil tidak ditemukan" });
            }
            catch (Exception)
            {
                return Json(new { success = false, message = "Terjadi kesalahan" });
            }
        }

        // Helper method to generate tracking number
        private string GenerateTrackingNumber()
        {
            var timestamp = DateTime.Now.ToString("yyyyMMddHHmmss");
            var random = new Random().Next(100, 999);
            return $"TRK{timestamp}{random}";
        }

        // ===== HELPER METHODS TAMBAHAN =====

        // Method untuk mendapatkan customer dari session
        private async Task<Customer?> GetCurrentCustomer()
        {
            var customerIdStr = HttpContext.Session.GetString("CustomerId");
            if (!string.IsNullOrEmpty(customerIdStr) && int.TryParse(customerIdStr, out int customerId) && customerId > 0)
            {
                return await _context.Customers.FindAsync(customerId);
            }
            return null;
        }

        // Method untuk memastikan customer terhubung dengan user yang login
        private async Task<Customer> EnsureCustomerExists()
        {
            var customer = await GetCurrentCustomer();
            if (customer != null)
            {
                return customer;
            }

            // Jika tidak ada customer di session, coba buat dari data admin
            var username = HttpContext.Session.GetString("Username");
            var namaLengkap = HttpContext.Session.GetString("NamaLengkap");

            if (!string.IsNullOrEmpty(namaLengkap))
            {
                customer = new Customer
                {
                    NamaCustomer = namaLengkap,
                    Alamat = "Alamat akan diupdate saat pre-order",
                    NoTelepon = "000000000000",
                    Email = $"{username}@roditripekanbaru.com",
                    JenisKelamin = "Laki-laki",
                    Pekerjaan = "Tidak diketahui",
                    TanggalDaftar = DateTime.Now
                };

                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();

                // Update session
                HttpContext.Session.SetString("CustomerId", customer.CustomerId.ToString());
                HttpContext.Session.SetString("CustomerName", customer.NamaCustomer);

                return customer;
            }

            throw new InvalidOperationException("Cannot create customer - no user data in session");
        }
    }

    // ViewModel for Pre-Order Form
    public class PreOrderFormViewModel
    {
        public int MobilId { get; set; }

        [Required(ErrorMessage = "Nama lengkap harus diisi")]
        [StringLength(100)]
        public string NamaCustomer { get; set; } = string.Empty;

        [Required(ErrorMessage = "Alamat harus diisi")]
        [StringLength(200)]
        public string Alamat { get; set; } = string.Empty;

        [Required(ErrorMessage = "No. Telepon harus diisi")]
        [StringLength(15)]
        [Phone(ErrorMessage = "Format nomor telepon tidak valid")]
        public string NoTelepon { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Format email tidak valid")]
        [StringLength(100)]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Jenis kelamin harus dipilih")]
        public string JenisKelamin { get; set; } = string.Empty;

        [StringLength(50)]
        public string? Pekerjaan { get; set; }

        [Required(ErrorMessage = "Jumlah DP harus diisi")]
        [Range(1000000, double.MaxValue, ErrorMessage = "Jumlah DP minimal Rp 1.000.000")]
        public decimal JumlahDP { get; set; }

        [StringLength(500)]
        public string? Catatan { get; set; }
    }
}