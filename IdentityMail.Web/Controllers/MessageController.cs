using IdentityMail.Web.Context;
using IdentityMail.Web.DTOs.UserMessagesDtos;
using IdentityMail.Web.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Reflection;

namespace IdentityMail.Web.Controllers
{
    [Authorize(Roles = "User,Admin")]
    public class MessageController(UserManager<AppUser> _userManager,
                                   AppDbContext _context) : Controller
    {
        private const int PageSize = 7;
        public async Task<IActionResult> Index(string filter = "all", int page = 1)
        {

            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            ViewBag.fullName = $"{user.FirstName} {user.LastName}";
            var query = _context.UserMessages
            .Include(x => x.Sender)
            .Include(x => x.Category)
            .Where(x => x.ReceiverId == user.Id).Where(x => x.IsDraft == false)
            .AsQueryable();

            var totalMessagesCount = await query.CountAsync();
            ViewBag.TotalMessagesCount = totalMessagesCount;

            var unreadMessagesCount = (await query.Where(x => x.IsRead == false).ToListAsync()).Count();
            ViewBag.UnreadMessagesCount = unreadMessagesCount;

            var readMessagesCount = (await query.Where(x => x.IsRead == true).ToListAsync()).Count();
            ViewBag.ReadMessagesCount = readMessagesCount;
            var importantMessageCount = (await query.Where(x => x.IsImportant == true).ToListAsync()).Count();
            ViewBag.ImportantMessageCount = importantMessageCount;
            if (filter == "read")
            {
                query = query.Where(x => x.IsRead == true);
            }
            else if (filter == "unread")
            {
                query = query.Where(x => x.IsRead == false);
            }
            // Toplam kayıt sayısını, SAYFALAMA yapılmadan önce hesaplıyoruz
            int totalMessages = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalMessages / (double)PageSize);
            var messages = await query.OrderByDescending(x => x.SendDate)
                .Skip((page - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();
            ViewBag.CurrentFilter = filter;   // View'de, hangi butonun "aktif" görüneceğini belirlemek için
            ViewBag.CurrentPage = page; // View'de mevcut sayfa numarasını gösterir
            ViewBag.TotalPages = totalPages; // View'de toplam sayfa sayısını gösterir
            return View(messages);
        }
        public async Task<IActionResult> SendMail()
        {
            ViewBag.Categories = new SelectList(await _context.MessageCategories.Where(c => c.IsActive).ToListAsync(), "Id", "Name");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SendMail(SendMailDto sendMailDto, string action)
        {
            var sender = await _userManager.GetUserAsync(User);
            var receiver = await _userManager.FindByEmailAsync(sendMailDto.ReceiverMail);
            if (receiver is null)
            {
                ModelState.AddModelError(string.Empty, "Girdiğiniz mail ile sistemde kayıtlı kullanıcı bulunamadı.");
                return View(sendMailDto);
            }
            if (receiver == sender)
            {
                ModelState.AddModelError(string.Empty, "Kendinize mail gönderemezsiniz.");
                return View(sendMailDto);
            }
            if (sendMailDto.CategoryId != null &&
            !await _context.MessageCategories.AnyAsync(c => c.Id == sendMailDto.CategoryId && c.IsActive))
            {
                ModelState.AddModelError("CategoryId", "Geçersiz kategori.");
            }
            var newMessage = new UserMessage()
            {
                Subject = sendMailDto.Subject,
                Body = sendMailDto.Body,
                SendDate = DateTime.Now,
                Sender = sender,
                Receiver = receiver,
                IsDraft = (action == "draft"),
                CategoryId = sendMailDto.CategoryId
            };
            _context.UserMessages.Add(newMessage);
            await _context.SaveChangesAsync();
            if (action == "draft")
                return RedirectToAction("Draft");

            return RedirectToAction("Index");
        }
        public async Task<IActionResult> MailDetail(int id)
        {
            var mailDetail = await _context.UserMessages.Include(x => x.Sender).FirstOrDefaultAsync(x => x.Id == id);
            mailDetail.IsRead = true;
            await _context.SaveChangesAsync();
            CultureInfo culture = new CultureInfo("tr-TR");
            DateTime date = mailDetail.SendDate;
            string dateTimeFormat = date.ToString("d MMMM dddd HH:mm", culture);
            ViewBag.DateTimeFormat = dateTimeFormat;
            return View(mailDetail);
        }
        public async Task<IActionResult> MailImportantStar(int id, string? returnUrl)
        {
            var mailImportant = await _context.UserMessages.FirstOrDefaultAsync(x => x.Id == id);
            if (mailImportant.IsImportant)
            {
                mailImportant.IsImportant = false;
            }
            else
            {
                mailImportant.IsImportant = true;
            }
            await _context.SaveChangesAsync();
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
                
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> SentMessages()
        {
            var user = await _userManager.GetUserAsync(User);
            var sentMessages = await _context.UserMessages.Include(m => m.Receiver).Include(m => m.Category).Where(x => x.SenderId == user.Id && !x.IsDraft).ToListAsync();
            return View(sentMessages);
        }
        public async Task<IActionResult> Draft()
        {
            var user = await _userManager.GetUserAsync(User);
            var userDraftList = await _context.UserMessages.Include(x=>x.Category).Where(x => x.SenderId == user.Id && x.IsDraft && !x.IsTrashed).ToListAsync();
            return View(userDraftList);
        }
        public async Task<IActionResult> DraftEdit(int id)
        {
            var draft = await _context.UserMessages.Include(x => x.Receiver).FirstOrDefaultAsync(x => x.Id == id);
            SendMailDto model = new()
            {
                Body = draft.Body,
                ReceiverMail = draft.Receiver.Email,
                Subject = draft.Subject,
                CategoryId = draft.CategoryId
            };
            ViewBag.Categories = new SelectList(await _context.MessageCategories.Where(c => c.IsActive).ToListAsync(), "Id", "Name");
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> DraftEdit(SendMailDto sendMailDto, string action, int id)
        {
            var receiver = await _userManager.FindByEmailAsync(sendMailDto.ReceiverMail);
            var sender = await _userManager.GetUserAsync(User);
            var draftEdit = await _context.UserMessages.FirstOrDefaultAsync(x => x.Id == id && x.SenderId == sender.Id && x.IsDraft);
            if (receiver is null)
            {
                ModelState.AddModelError(string.Empty, "Girdiğiniz mail ile sistemde kayıtlı kullanıcı bulunamadı.");
                return View(sendMailDto);
            }
            if (receiver == sender)
            {
                ModelState.AddModelError(string.Empty, "Kendinize mail gönderemezsiniz.");
                return View(sendMailDto);
            }
            if (draftEdit == null)
            {
                ModelState.AddModelError(string.Empty, "Güncellenecek taslağı değiştirdiniz. Böyle bir taslak yok.");
                return View(sendMailDto);
            }
            draftEdit.Body = sendMailDto.Body;
            draftEdit.Receiver = receiver;
            draftEdit.Subject = sendMailDto.Subject;
            draftEdit.SendDate = DateTime.Now;
            draftEdit.CategoryId = sendMailDto.CategoryId;
            if (action == "draft")
            {
                draftEdit.IsDraft = true;
            }
            else
            {
                draftEdit.IsDraft = false;
            }
            await _context.SaveChangesAsync();
            return RedirectToAction("Draft");

        }
        public async Task<IActionResult> MoveToTrashDraft(int id)
        {
            var sender = await _userManager.GetUserAsync(User);
            var moveToTrashDraft = await _context.UserMessages.FirstOrDefaultAsync(x => x.Id == id && x.SenderId == sender.Id && !x.IsTrashed);
            if (moveToTrashDraft == null)
            {
                TempData["MoveToTrashDraftErrorMessage"] = "Silmek istediğiniz mesaj bulunamadı veya bu işleme yetkiniz yok.";
                return RedirectToAction("Draft");
            }
            moveToTrashDraft.IsTrashed = true;
            moveToTrashDraft.TrashedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            TempData["MoveToTrashDraftErrorMessage"] = "Taslak çöp kutusuna taşındı.";
            return RedirectToAction("Draft");

        }
        public async Task<IActionResult> Trash()
        {
            var sender = await _userManager.GetUserAsync(User);
            var trashDrafts = await _context.UserMessages.Where(x => x.SenderId == sender.Id && x.IsDraft && x.IsTrashed).ToListAsync();
            return View(trashDrafts);
        }
        public async Task<IActionResult> DeletePermanentlyDraft(int id)
        {
            var sender = await _userManager.GetUserAsync(User);
            var deletePermanetlyDraft = await _context.UserMessages.FirstOrDefaultAsync(x => x.Id == id && x.SenderId == sender.Id);
            _context.UserMessages.Remove(deletePermanetlyDraft);
            await _context.SaveChangesAsync();
            TempData["TrashMessage"] = "Taslak kalıcı olarak silindi.";
            return RedirectToAction("Trash");

        }
        public async Task<IActionResult> RestoreDraftFromTrash(int id)
        {
            var sender = await _userManager.GetUserAsync(User);
            var restoreDraftFromTrash = await _context.UserMessages.FirstOrDefaultAsync(x => x.Id == id && x.SenderId == sender.Id && x.IsDraft && x.IsTrashed);
            restoreDraftFromTrash.IsTrashed = false;
            restoreDraftFromTrash.TrashedDate = null;
            restoreDraftFromTrash.SendDate = DateTime.Now;
            await _context.SaveChangesAsync();
            TempData["TrashMessage"] = "Taslak geri yüklendi.";
            return RedirectToAction("Trash");
        }
        public async Task<IActionResult> ImportantMessages()
        {
            var user = await _userManager.GetUserAsync(User);
            var importantMessages = await _context.UserMessages.Include(x => x.Category).Include(x => x.Sender).Where(m => m.ReceiverId == user.Id && m.IsImportant).ToListAsync();
            return View(importantMessages);
        }
    }
}
