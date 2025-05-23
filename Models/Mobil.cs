using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace RoditriPekanbaru.Models
{
    public class Mobil
    {
        [Key]
        public int MobilId { get; set; }
        [Required(ErrorMessage = "Merek harus diisi")]
        [StringLength(50)]
        [Display(Name = "Merek")]
        public string Merek { get; set; } = string.Empty;
        [Required(ErrorMessage = "Model harus diisi")]
        [StringLength(50)]
        [Display(Name = "Model/Type")]
        public string Model { get; set; } = string.Empty;
        [Display(Name = "Gambar Mobil")]
        [StringLength(255)]
        public string? GambarMobil { get; set; }
        [Required(ErrorMessage = "Tahun harus diisi")]
        [Display(Name = "Tahun")]
        [Range(1990, 2030, ErrorMessage = "Tahun harus antara 1990-2030")]
        public int Tahun { get; set; }
        [Required(ErrorMessage = "Warna harus diisi")]
        [StringLength(30)]
        [Display(Name = "Warna")]
        public string Warna { get; set; } = string.Empty;
        [Required(ErrorMessage = "Harga harus diisi")]
        [Column(TypeName = "decimal(18,2)")]
        [Display(Name = "Harga")]
        [DisplayFormat(DataFormatString = "{0:C0}", ApplyFormatInEditMode = false)]
        public decimal Harga { get; set; }
        [StringLength(200)]
        [Display(Name = "Kondisi/Deskripsi")]
        public string? Kondisi { get; set; }
        [Display(Name = "Status Stok")]
        public bool IsAvailable { get; set; } = true;
        [StringLength(50)]
        [Display(Name = "No. Rangka")]
        public string? NoRangka { get; set; }
        [StringLength(50)]
        [Display(Name = "No. Mesin")]
        public string? NoMesin { get; set; }
        [StringLength(50)]
        [Display(Name = "No. Polisi")]
        public string? NoPolisi { get; set; }
        [Display(Name = "Tanggal Input")]
        public DateTime TanggalInput { get; set; } = DateTime.Now;

        // Extension method untuk menampilkan nama mobil lengkap
        [NotMapped]
        public string NamaMobil => $"{Merek} {Model} ({Tahun}) - {Warna}";

        // Navigation Properties
        public virtual ICollection<TransaksiPenjualan>? TransaksiPenjualan { get; set; }
    }
}