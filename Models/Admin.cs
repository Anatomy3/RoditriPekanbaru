using System.ComponentModel.DataAnnotations;

namespace RoditriPekanbaru.Models
{
    public class Admin
    {
        [Key]
        public int AdminId { get; set; }

        [Required(ErrorMessage = "Username harus diisi")]
        [StringLength(50)]
        [Display(Name = "Username")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password harus diisi")]
        [StringLength(255)]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nama admin harus diisi")]
        [StringLength(100)]
        [Display(Name = "Nama Lengkap")]
        public string NamaLengkap { get; set; } = string.Empty;

        [Display(Name = "Level Access")]
        public string Level { get; set; } = "Bagian Penjualan";

        [Display(Name = "Status")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Tanggal Dibuat")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        [Display(Name = "Terakhir Login")]
        public DateTime? LastLogin { get; set; }
    }
}