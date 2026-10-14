using Microsoft.AspNetCore.Mvc;

namespace IdentityMail.Web.ViewComponents.UserProfile
{
    public class UserProfileImageViewComponent:ViewComponent
    {
        public IViewComponentResult Invoke(string imageUrl)
        {
            return View(model:imageUrl);
        }
    }
}
