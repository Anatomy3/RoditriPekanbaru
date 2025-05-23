using RoditriPekanbaru.Models;

namespace RoditriPekanbaru.ViewModels
{
    public class LaporanViewModel
    {
        public List<TransaksiPenjualan> Transaksi { get; set; } = new List<TransaksiPenjualan>();
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalTransaksi { get; set; }
        public decimal TotalPendapatan { get; set; }
        public int TotalLunas { get; set; }
        public int TotalBelumLunas { get; set; }
    }
}