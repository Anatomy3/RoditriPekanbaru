using System.ComponentModel.DataAnnotations;

namespace RoditriPekanbaru.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Username harus diisi")]
        [StringLength(50, ErrorMessage = "Username maksimal 50 karakter")]
        [Display(Name = "Username")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password harus diisi")]
        [StringLength(255, MinimumLength = 6, ErrorMessage = "Password minimal 6 karakter")]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Konfirmasi password harus diisi")]
        [Display(Name = "Konfirmasi Password")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password dan konfirmasi password tidak sama")]
        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nama lengkap harus diisi")]
        [StringLength(100, ErrorMessage = "Nama lengkap maksimal 100 karakter")]
        [Display(Name = "Nama Lengkap")]
        public string NamaLengkap { get; set; } = string.Empty;
    }
}