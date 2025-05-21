using System.ComponentModel.DataAnnotations;

namespace RoditriPekanbaru.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Username harus diisi")]
        [Display(Name = "Username")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password harus diisi")]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Ingat Saya")]
        public bool RememberMe { get; set; }
    }
}