using IdentityMail.Web.DTOs.UserProfileEditsDtos;
using IdentityMail.Web.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IdentityMail.Web.Controllers
{
    [Authorize(Roles = "User,Admin")]
    public class UserProfileController(UserManager<AppUser> _userManager, IWebHostEnvironment _env, SignInManager<AppUser> _signInManager) : Controller
    {
        private const string DefaultImagePath = "/images/profiles/default.jpg";
        private UserProfilePageDto BuildProfilePageModel(AppUser user)
        {
            return new UserProfilePageDto
            {
                ProfileImageEdit = new ProfileImageEditDto
                {
                    ProfileImageUrl = string.IsNullOrEmpty(user.ProfileImageUrl)
                        ? DefaultImagePath
                        : user.ProfileImageUrl
                },
                UserInfoEdit = new UserInfoEditDto
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Mail = user.Email,
                    UserName = user.UserName
                },
                UserPasswordEdit = new UserPasswordEditDto()
            };
        }
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            var model = BuildProfilePageModel(user);

            ViewBag.FirstName = user.FirstName;
            ViewBag.LastName= user.LastName;
            ViewBag.Email= user.Email;

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> ProfileImageSaveChanges(IFormFile profileImage, bool removePhoto)
        {
            var user = await _userManager.GetUserAsync(User);

            // Durum 1: Kullanıcı yeni bir resim seçmiş
            if (profileImage != null && profileImage.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var ext = Path.GetExtension(profileImage.FileName).ToLower();

                if (!allowedExtensions.Contains(ext))
                {
                    TempData["Error"] = "Sadece resim dosyası yükleyebilirsiniz.";
                    return RedirectToAction("Index");
                }

                // Eski dosyayı sil (default.jpg değilse)
                DeleteOldPhotoIfNotDefault(user.ProfileImageUrl);

                // Yeni dosyayı kaydet
                var fileName = Guid.NewGuid() + ext;
                var uploadsFolder = Path.Combine(_env.WebRootPath, "images", "profiles");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await profileImage.CopyToAsync(stream);
                }

                user.ProfileImageUrl = "/images/profiles/" + fileName;
            }
            // Durum 2: Kullanıcı "Kaldır"a basmış (ve yeni dosya seçmemiş)
            else if (removePhoto)
            {
                DeleteOldPhotoIfNotDefault(user.ProfileImageUrl);
                user.ProfileImageUrl = DefaultImagePath;
            }
            // Durum 3: Hiçbir değişiklik yok -> hiçbir şey yapma

            await _userManager.UpdateAsync(user);

            return RedirectToAction("Index");
        }

        // Yardımcı metod: eski dosyayı siler, ama default.jpg ise dokunmaz
        private void DeleteOldPhotoIfNotDefault(string oldImageUrl)
        {
            if (string.IsNullOrEmpty(oldImageUrl)) return;
            if (oldImageUrl == DefaultImagePath) return;   // default.jpg'ye asla dokunma

            var oldPath = Path.Combine(_env.WebRootPath, oldImageUrl.TrimStart('/'));
            if (System.IO.File.Exists(oldPath))
            {
                System.IO.File.Delete(oldPath);
            }
        }
        [HttpPost]
        public async Task<IActionResult> UserInfoSaveChanges(UserInfoEditDto userInfoEditDto)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var model = BuildProfilePageModel(currentUser);
            model.UserInfoEdit = userInfoEditDto;
            if (currentUser == null)
            {
                await _signInManager.SignOutAsync();   // kullanıcı bilgileri güncellerken admin kullanıcıyı pasif yaparsa çerezi sil.
                return RedirectToAction("Index", "Home");
            }

            var userWithSameEmail = await _userManager.FindByEmailAsync(userInfoEditDto.Mail);
            if (userWithSameEmail != null && userWithSameEmail.Id != currentUser.Id)
            {
                ModelState.AddModelError(nameof(userInfoEditDto.Mail), "Bu e-posta adresi başka bir kullanıcı tarafından kullanılıyor.");
                model.UserInfoEdit.Mail = currentUser.Email;
            }

            var userWithSameUserName = await _userManager.FindByNameAsync(userInfoEditDto.UserName);
            if (userWithSameUserName != null && userWithSameUserName.Id != currentUser.Id)
            {
                ModelState.AddModelError(nameof(userInfoEditDto.UserName), "Bu kullanıcı adı başka bir kullanıcı tarafından kullanılıyor.");
                model.UserInfoEdit.UserName = currentUser.UserName;
            }
            if (!ModelState.IsValid)
            {
                return View("Index",model);
            }
            currentUser.FirstName = userInfoEditDto.FirstName;
            currentUser.LastName = userInfoEditDto.LastName;
            currentUser.Email = userInfoEditDto.Mail;
            currentUser.UserName = userInfoEditDto.UserName;

            var result = await _userManager.UpdateAsync(currentUser);

            if (result.Succeeded)
            {
                return RedirectToAction("Index");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(error.Code, error.Description);
            }

            return View("Index",model);

        }
        [HttpPost]
        public async Task<IActionResult> UserPasswordSaveChanges(UserPasswordEditDto userPasswordEditDto)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var model = BuildProfilePageModel(currentUser);
            model.UserPasswordEdit.CurrentPassword = userPasswordEditDto.CurrentPassword;
            model.UserPasswordEdit.NewPassword = userPasswordEditDto.NewPassword;
            model.UserPasswordEdit.ConfirmPassword = userPasswordEditDto.ConfirmPassword;
            if (!ModelState.IsValid)
            {
                return View("Index",model);
            }
            if (currentUser == null)
            {
                await _signInManager.SignOutAsync();
                return RedirectToAction("Index");
            }
            var result = await _userManager.ChangePasswordAsync(currentUser, userPasswordEditDto.CurrentPassword, userPasswordEditDto.NewPassword);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(error.Code, error.Description);
                }
                return View("Index",model);
            }
            await _signInManager.RefreshSignInAsync(currentUser);
            TempData["SuccessMessage"] = "Şifreniz başarıyla güncellendi.";
            return RedirectToAction("Index");
        }
    }
}
