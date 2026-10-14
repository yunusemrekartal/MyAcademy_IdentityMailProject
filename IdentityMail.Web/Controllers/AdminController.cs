using IdentityMail.Web.Context;
using IdentityMail.Web.DTOs.UserListItemDtos;
using IdentityMail.Web.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdentityMail.Web.Controllers
{
    public class AdminController(UserManager<AppUser> _userManager,
                                   AppDbContext _context) : Controller
    {
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var totalUsersCount = await _userManager.Users.CountAsync();
            ViewBag.TotalUsers = totalUsersCount;

            var totalMessageCount = await _context.UserMessages.CountAsync(m => !m.IsDraft && !m.IsTrashed);
            ViewBag.TotalMessageCount = totalMessageCount;

            var totalUnreadMessageCount = await _context.UserMessages.CountAsync(m => !m.IsDraft && m.IsTrashed && !m.IsRead);
            ViewBag.TotalUnreadMessageCount = totalUnreadMessageCount;

            var totalTrashedDraftCount = await _context.UserMessages.CountAsync(d => d.IsDraft && d.IsTrashed);
            ViewBag.TotalTrashedDraftCount = totalTrashedDraftCount;

            var totalUsers = await _userManager.Users.Include(u => u.SentMessages).ToListAsync();

            var totalCategories = await _context.MessageCategories.Select(c => c.Name).ToListAsync();
            ViewBag.TotalCategories = totalCategories;

            var model = await _userManager.Users
            .Select(u => new UserListItemDto
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                UserName = u.UserName,
                Email = u.Email,
                SentMessageCount = u.SentMessages.Count(m => !m.IsDraft && !m.IsTrashed)
            })
            .ToListAsync();

            return View(model);
        }
    }
}
