using System.ComponentModel.DataAnnotations;

namespace RoditriPekanbaru.Models
{
    public class Banner
    {
        [Key]
        public int BannerId { get; set; }

        [Required(ErrorMessage = "Nama banner harus diisi")]
        [StringLength(100)]
        [Display(Name = "Nama Banner")]
        public string NamaBanner { get; set; } = string.Empty;

        [Required(ErrorMessage = "Gambar banner harus diupload")]
        [Display(Name = "Gambar Banner")]
        [StringLength(255)]
        public string GambarBanner { get; set; } = string.Empty;

        [StringLength(255)]
        [Display(Name = "Link Tujuan")]
        public string? LinkTujuan { get; set; }

        [Display(Name = "Urutan")]
        public int Urutan { get; set; } = 1;

        [Display(Name = "Status Aktif")]
        public bool IsActive { get; set; } = true;

        [Display(Name = "Tanggal Mulai")]
        public DateTime? TanggalMulai { get; set; }

        [Display(Name = "Tanggal Berakhir")]
        public DateTime? TanggalBerakhir { get; set; }

        [Display(Name = "Tanggal Dibuat")]
        public DateTime TanggalDibuat { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Admin harus diisi")]
        [StringLength(100)]
        [Display(Name = "Dibuat Oleh")]
        public string DibuatOleh { get; set; } = string.Empty;
    }
}