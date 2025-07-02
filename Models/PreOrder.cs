using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoditriPekanbaru.Models
{
    public class PreOrder
    {
        [Key]
        public int PreOrderId { get; set; }

        [Required(ErrorMessage = "Customer harus dipilih")]
        [Display(Name = "Customer")]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Mobil harus dipilih")]
        [Display(Name = "Mobil")]
        public int MobilId { get; set; }

        [Required(ErrorMessage = "Tanggal pre-order harus diisi")]
        [Display(Name = "Tanggal Pre-Order")]
        [DataType(DataType.Date)]
        public DateTime TanggalPreOrder { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Jumlah DP harus diisi")]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Jumlah DP")]
        [DisplayFormat(DataFormatString = "{0:C0}", ApplyFormatInEditMode = false)]
        [Range(0.01, double.MaxValue, ErrorMessage = "Jumlah DP harus lebih dari 0")]
        public decimal JumlahDP { get; set; }

        [Required(ErrorMessage = "Status harus diisi")]
        [StringLength(20)]
        [Display(Name = "Status")]
        public string Status { get; set; } = "Pending"; // Pending, Approved, Rejected, Completed

        [StringLength(500)]
        [Display(Name = "Catatan")]
        [DataType(DataType.MultilineText)]
        public string? Catatan { get; set; }

        [Display(Name = "Tanggal Dibuat")]
        public DateTime TanggalDibuat { get; set; } = DateTime.Now;

        [Display(Name = "Tanggal Diupdate")]
        public DateTime? TanggalUpdate { get; set; }

        [StringLength(100)]
        [Display(Name = "Diupdate Oleh")]
        public string? UpdatedBy { get; set; }

        // Navigation Properties
        [ForeignKey("CustomerId")]
        public virtual Customer? Customer { get; set; }

        [ForeignKey("MobilId")]
        public virtual Mobil? Mobil { get; set; }

        // Computed Properties
        [NotMapped]
        [Display(Name = "Nama Customer")]
        public string NamaCustomer => Customer?.NamaCustomer ?? "N/A";

        [NotMapped]
        [Display(Name = "Mobil")]
        public string DetailMobil => Mobil?.NamaMobil ?? "N/A";

        [NotMapped]
        [Display(Name = "Harga Mobil")]
        [DisplayFormat(DataFormatString = "{0:C0}")]
        public decimal HargaMobil => Mobil?.Harga ?? 0;

        [NotMapped]
        [Display(Name = "Sisa Pembayaran")]
        [DisplayFormat(DataFormatString = "{0:C0}")]
        public decimal SisaPembayaran => HargaMobil - JumlahDP;
    }
}