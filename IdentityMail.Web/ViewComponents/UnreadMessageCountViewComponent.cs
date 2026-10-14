using IdentityMail.Web.Context;
using IdentityMail.Web.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdentityMail.Web.ViewComponents
{
    public class UnreadMessageCountViewComponent(UserManager<AppUser> _userManager,
        AppDbContext _context) : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            var unreadMessageCount = await _context.UserMessages
            .CountAsync(x => x.ReceiverId == user.Id && x.IsDraft == false && x.IsRead == false);
            return View(unreadMessageCount);
        }

    }
}
