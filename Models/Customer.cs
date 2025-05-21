using System.ComponentModel.DataAnnotations;

namespace RoditriPekanbaru.Models
{
    public class Customer
    {
        [Key]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Nama customer harus diisi")]
        [StringLength(100)]
        [Display(Name = "Nama Customer")]
        public string NamaCustomer { get; set; } = string.Empty;

        [Required(ErrorMessage = "Alamat harus diisi")]
        [StringLength(200)]
        [Display(Name = "Alamat")]
        public string Alamat { get; set; } = string.Empty;

        [Required(ErrorMessage = "No. Telepon harus diisi")]
        [StringLength(15)]
        [Display(Name = "No. Telepon")]
        public string NoTelepon { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Format email tidak valid")]
        [StringLength(100)]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Jenis kelamin harus diisi")]
        [Display(Name = "Jenis Kelamin")]
        public string JenisKelamin { get; set; } = string.Empty;

        [Display(Name = "Pekerjaan")]
        [StringLength(50)]
        public string? Pekerjaan { get; set; }

        [Display(Name = "Tanggal Daftar")]
        public DateTime TanggalDaftar { get; set; } = DateTime.Now;

        // Navigation Properties
        public virtual ICollection<TransaksiPenjualan>? TransaksiPenjualan { get; set; }
    }
}