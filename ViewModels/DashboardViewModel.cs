using RoditriPekanbaru.Models;

namespace RoditriPekanbaru.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalCustomer { get; set; }
        public int TotalMobil { get; set; }
        public int TotalTransaksi { get; set; }
        public decimal TotalPenjualan { get; set; }
        public int MobilTersedia { get; set; }
        public int MobilTerjual { get; set; }

        public List<TransaksiPenjualan> TransaksiTerbaru { get; set; } = new List<TransaksiPenjualan>();
    }
}