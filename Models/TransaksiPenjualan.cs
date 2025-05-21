using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoditriPekanbaru.Models
{
    public class TransaksiPenjualan
    {
        [Key]
        public int TransaksiId { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "No. Faktur")]
        public string NoFaktur { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Tanggal Transaksi")]
        [DataType(DataType.Date)]
        public DateTime TanggalTransaksi { get; set; } = DateTime.Now;

        [Required]
        [ForeignKey("Customer")]
        [Display(Name = "Customer")]
        public int CustomerId { get; set; }

        [Required]
        [ForeignKey("Mobil")]
        [Display(Name = "Mobil")]
        public int MobilId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Harga Jual")]
        [DisplayFormat(DataFormatString = "{0:C0}", ApplyFormatInEditMode = false)]
        public decimal HargaJual { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Diskon")]
        [DisplayFormat(DataFormatString = "{0:C0}", ApplyFormatInEditMode = false)]
        public decimal Diskon { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Total Bayar")]
        [DisplayFormat(DataFormatString = "{0:C0}", ApplyFormatInEditMode = false)]
        public decimal TotalBayar { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Uang Muka")]
        [DisplayFormat(DataFormatString = "{0:C0}", ApplyFormatInEditMode = false)]
        public decimal UangMuka { get; set; } = 0;

        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Sisa Pembayaran")]
        [DisplayFormat(DataFormatString = "{0:C0}", ApplyFormatInEditMode = false)]
        public decimal SisaPembayaran { get; set; } = 0;

        [Required]
        [StringLength(50)]
        [Display(Name = "Status Pembayaran")]
        public string StatusPembayaran { get; set; } = "Lunas";

        [Required]
        [StringLength(50)]
        [Display(Name = "Cara Pembayaran")]
        public string CaraPembayaran { get; set; } = "Tunai";

        [StringLength(200)]
        [Display(Name = "Keterangan")]
        public string? Keterangan { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Admin")]
        public string Admin { get; set; } = string.Empty;

        [Display(Name = "Status Mobil")]
        public string StatusMobil { get; set; } = "Siap Diambil";

        // Navigation Properties
        public virtual Customer? Customer { get; set; }
        public virtual Mobil? Mobil { get; set; }
    }
}