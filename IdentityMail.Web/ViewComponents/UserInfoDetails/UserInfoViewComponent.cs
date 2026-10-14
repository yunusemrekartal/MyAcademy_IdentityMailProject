using IdentityMail.Web.DTOs.UserProfileEditsDtos;
using Microsoft.AspNetCore.Mvc;

namespace IdentityMail.Web.ViewComponents.UserInfo
{
    public class UserInfoViewComponent:ViewComponent
    {
        public IViewComponentResult Invoke(UserProfilePageDto userInfoModel)
        {
            return View(userInfoModel);
        }
    }
}
