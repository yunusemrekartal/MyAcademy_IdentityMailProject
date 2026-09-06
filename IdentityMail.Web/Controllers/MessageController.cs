using IdentityMail.Web.Context;
using IdentityMail.Web.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdentityMail.Web.Controllers
{
    [Authorize]
    public class MessageController(UserManager<AppUser> _userManager,
                                   AppDbContext _context) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            ViewBag.fullName = $"Merhaba {user.FirstName} {user.LastName}";
            var messages = _context.UserMessages.Include(x => x.Sender).Where(x => x.ReceiverId == user.Id).ToListAsync();
            return View();
        }
    }
}
