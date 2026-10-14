using IdentityMail.Web.Context;
using IdentityMail.Web.DTOs.CategoryCreateDtos;
using IdentityMail.Web.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace IdentityMail.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CategoryController(UserManager<AppUser> _userManager,
                                   AppDbContext _context) : Controller
    {
        public async Task<IActionResult> Index()
        {
            var allCategory = await _context.MessageCategories.ToListAsync();
            return View(allCategory);
        }
        public async Task<IActionResult> ToggleActive(int id)
        {
            var toggleCategory = await _context.MessageCategories.FirstOrDefaultAsync(c => c.Id == id);
            if (toggleCategory.IsActive)
            {
                toggleCategory.IsActive = false;
            }
            else
            {
                toggleCategory.IsActive = true;
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> CreateCategory()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateCategory(CategoryCreate categoryCreate)
        {
            var exists = await _context.MessageCategories.AnyAsync(c => c.Name == categoryCreate.CategoryName);
            if (!exists)
            {
                var category = new MessageCategory()
                {
                    IsActive = true,
                    Name= categoryCreate.CategoryName
                };
                await _context.MessageCategories.AddAsync(category);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ModelState.AddModelError("", "Girdiğiniz Kategori Zaten Mevcut.");
            return View(categoryCreate);
        }
    }
}
