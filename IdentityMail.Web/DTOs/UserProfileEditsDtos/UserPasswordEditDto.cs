using System.ComponentModel.DataAnnotations;

namespace IdentityMail.Web.DTOs.UserProfileEditsDtos
{
    public class UserPasswordEditDto
    {
        [Required(ErrorMessage ="Mevcut Şifrenizi Giriniz.")]
        public string CurrentPassword { get; set; }
        [Required(ErrorMessage = "Yeni Şifrenizi Giriniz.")]
        public string NewPassword { get; set; }
        [Required(ErrorMessage = "Yeni Şifrenizi Tekrar Giriniz.")]
        [Compare(nameof(NewPassword), ErrorMessage = "Şifreler birbiriyle uyumlu değil.")]
        public string ConfirmPassword { get; set; }
    }
}
