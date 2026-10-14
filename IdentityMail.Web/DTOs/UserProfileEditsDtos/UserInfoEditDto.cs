using System.ComponentModel.DataAnnotations;

namespace IdentityMail.Web.DTOs.UserProfileEditsDtos
{
    public class UserInfoEditDto
    {
        [Required(ErrorMessage ="Adınızı Giriniz")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Soyadınızı Giriniz")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Kullanıcı Adınızı Giriniz")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Mail Adresinizi Giriniz")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz.")]
        public string Mail { get; set; }
    }
}
