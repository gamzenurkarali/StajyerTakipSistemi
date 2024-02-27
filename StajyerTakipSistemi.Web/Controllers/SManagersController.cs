using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using System.Net.Mail;
using MailKit.Net.Smtp;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;
using StajyerTakipSistemi.Web.Models;
using static System.Net.Mime.MediaTypeNames;

namespace StajyerTakipSistemi.Web.Controllers
{
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public class SManagersController : Controller
    {
        private readonly StajyerTakipSistemiDbContext _context;

        public SManagersController(StajyerTakipSistemiDbContext context)
        {
            _context = context;
        }
        private string GenerateUniqueUsername()
        {
            string username = "";

            do
            {

                username = "M" + new Random().Next(10000000, 99999999).ToString("D4");
            } while (_context.SManagers.Any(i => i.Username == username));

            return username;
        }


        private async Task<string> GenerateUniquePasswordAsync()
        {
            string password;
            SManager existingManager;

            do
            {

                password = GenerateRandomPassword();


                existingManager = await _context.SManagers.FirstOrDefaultAsync(s => s.Password == password);


            } while (existingManager != null);

            return password;
        }
        private string GenerateRandomPassword()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            var password = new string(Enumerable.Repeat(chars, 8) // Parolanın uzunluğu 8 karakter
                .Select(s => s[random.Next(s.Length)]).ToArray());

            return password;
        }

        // GET: SManagers
        public async Task<IActionResult> Index()
        {
            var guidString = HttpContext.Session.GetString("Guid");

            if (string.IsNullOrWhiteSpace(guidString) || !Guid.TryParse(guidString, out Guid userGuid))
            {
                return RedirectToAction("Login", "Home");
            }
            var userAdminExist = new UserAdminExist(_context);
            bool isAdminGuidValid = userAdminExist.CheckGuid(HttpContext.Session.GetString("Guid"));
            var userInternExist = new UserInternExist(_context);
            bool isInternGuidValid = userInternExist.CheckGuid(HttpContext.Session.GetString("Guid"));
            var userManagerExist = new UserManagerExist(_context);
            bool isManagerGuidValid = userManagerExist.CheckGuid(HttpContext.Session.GetString("Guid"));

            if (HttpContext.Session.GetString("UserId") != null && isAdminGuidValid == true)
            {
                return _context.SManagers != null ?
                          View(await _context.SManagers.ToListAsync()) :
                          Problem("Entity set 'StajyerTakipSistemiDbContext.SManagers'  is null.");
            }
            else if (isInternGuidValid == true || isManagerGuidValid == true)
            {
                return RedirectToAction("Error", "Home");
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
            
        }

        // GET: SManagers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            var guidString = HttpContext.Session.GetString("Guid");

            if (string.IsNullOrWhiteSpace(guidString) || !Guid.TryParse(guidString, out Guid userGuid))
            {
                return RedirectToAction("Login", "Home");
            }
            var userAdminExist = new UserAdminExist(_context);
            bool isAdminGuidValid = userAdminExist.CheckGuid(HttpContext.Session.GetString("Guid"));
            var userInternExist = new UserInternExist(_context);
            bool isInternGuidValid = userInternExist.CheckGuid(HttpContext.Session.GetString("Guid"));
            var userManagerExist = new UserManagerExist(_context);
            bool isManagerGuidValid = userManagerExist.CheckGuid(HttpContext.Session.GetString("Guid"));

            if (HttpContext.Session.GetString("UserId") != null && isAdminGuidValid == true)
            {
                if (id == null || _context.SManagers == null)
                {
                    return NotFound();
                }

                var sManager = await _context.SManagers
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (sManager == null)
                {
                    return NotFound();
                }

                return View(sManager);
            }
            else if (isInternGuidValid == true || isManagerGuidValid == true)
            {
                return RedirectToAction("Error", "Home");
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
            
        }

        // GET: SManagers/Create
        public IActionResult Create()
        {
            var guidString = HttpContext.Session.GetString("Guid");

            if (string.IsNullOrWhiteSpace(guidString) || !Guid.TryParse(guidString, out Guid userGuid))
            {
                return RedirectToAction("Login", "Home");
            }
            var userAdminExist = new UserAdminExist(_context);
            bool isAdminGuidValid = userAdminExist.CheckGuid(HttpContext.Session.GetString("Guid"));
            var userInternExist = new UserInternExist(_context);
            bool isInternGuidValid = userInternExist.CheckGuid(HttpContext.Session.GetString("Guid"));
            var userManagerExist = new UserManagerExist(_context);
            bool isManagerGuidValid = userManagerExist.CheckGuid(HttpContext.Session.GetString("Guid"));

            if (HttpContext.Session.GetString("UserId") != null && isAdminGuidValid == true)
            {
                return View();
            }
            else if (isInternGuidValid == true || isManagerGuidValid == true)
            {
                return RedirectToAction("Error", "Home");
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
            
        }
        private bool IsValidEmail(string email)
        {
            return !string.IsNullOrEmpty(email) && email.Contains("@");
        }

        private bool IsValidPhoneNumber(string phoneNumber)
        {
            return !string.IsNullOrEmpty(phoneNumber) && phoneNumber.Length == 11 && phoneNumber.All(char.IsDigit);
        }
        // POST: SManagers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,Email,PhoneNumber,Username,Password,Guid")] SManager sManager)
        {
            if (ModelState.IsValid)
            {
                List<string> errorMessages = ValidateManager(sManager);

                if (errorMessages.Count > 0)
                {
                    TempData["Message"] = string.Join(" ", errorMessages);
                    TempData["AlertClass"] = "alert-danger";
                    return View(sManager);
                }

                try
                {
                    var beforeHash = await GenerateUniquePasswordAsync();
                    sManager.Password = PasswordHasher.HashPassword(beforeHash);
                    sManager.Username = GenerateUniqueUsername();

                    _context.Add(sManager);
                    await _context.SaveChangesAsync();

                    await SendConfirmationEmail(sManager, beforeHash);

                    TempData["Message"] = "Yetkili kişi eklendi.";
                    TempData["AlertClass"] = "alert-success";

                    if (HttpContext.Session.GetString("Username") != null)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                }
                catch (Exception ex)
                { 
                    Console.WriteLine("İstisna oluştu: " + ex.Message);
                    TempData["Message"] = "Yetkili kişi eklenirken bir hata oluştu.";
                    TempData["AlertClass"] = "alert-danger";
                    return View(sManager);
                }

                return RedirectToAction(nameof(Index));
            }

            return View(sManager);
        }

        private List<string> ValidateManager(SManager sManager)
        {
            List<string> errorMessages = new List<string>();

            if (string.IsNullOrEmpty(sManager.FirstName) || string.IsNullOrEmpty(sManager.LastName) || string.IsNullOrEmpty(sManager.Email) || string.IsNullOrEmpty(sManager.PhoneNumber))
            {
                errorMessages.Add("Lütfen istenen bilgileri doldurunuz.");
            }

            if (!string.IsNullOrEmpty(sManager.Email) && !IsValidEmail(sManager.Email))
            {
                errorMessages.Add("Geçersiz email formatı.");
            }

            if (!string.IsNullOrEmpty(sManager.PhoneNumber) && !IsValidPhoneNumber(sManager.PhoneNumber))
            {
                errorMessages.Add("Geçersiz telefon numarası formatı.");
            }

            return errorMessages;
        }

        private async Task SendConfirmationEmail(SManager sManager, string password)
        {
            var from = "your gmail address";
            var to = sManager.Email;
            var subject = "İNSPİMO";

              
            var content  = "<p>İnspimo'da artık siz de yetkilisiniz! Aşağıda bilgileriniz bulunmaktadır:</p>";
            content += $"<ul>";
            content += $"<li>Kullanıcı Adı: <strong>{sManager.Username}</strong></li>";
            content += $"<li>Şifre: <strong>{password}</strong></li>";
            content += $"</ul>";
            content += "<p>İyi çalışmalar dileriz!</p>";

            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("", from));
                message.To.Add(new MailboxAddress("", to));
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder();
                bodyBuilder.HtmlBody = content;

                message.Body = bodyBuilder.ToMessageBody();

                using (var client = new SmtpClient())
                {
                    client.Connect("smtp.gmail.com", 587, false);
                    client.Authenticate("your gmail address", "your gmail application password");

                    client.Send(message);
                    client.Disconnect(true);
                }
            }
            catch (Exception ex)
            { 
                Console.WriteLine("E-posta gönderme hatası: " + ex.Message);
                throw;  
            }
        }

        // GET: SManagers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            var guidString = HttpContext.Session.GetString("Guid");

            if (string.IsNullOrWhiteSpace(guidString) || !Guid.TryParse(guidString, out Guid userGuid))
            {
                return RedirectToAction("Login", "Home");
            }
            var userAdminExist = new UserAdminExist(_context);
            bool isAdminGuidValid = userAdminExist.CheckGuid(HttpContext.Session.GetString("Guid"));
            var userInternExist = new UserInternExist(_context);
            bool isInternGuidValid = userInternExist.CheckGuid(HttpContext.Session.GetString("Guid"));
            var userManagerExist = new UserManagerExist(_context);
            bool isManagerGuidValid = userManagerExist.CheckGuid(HttpContext.Session.GetString("Guid"));

            if (HttpContext.Session.GetString("UserId") != null && isAdminGuidValid == true)
            {
                if (id == null || _context.SManagers == null)
                {
                    return NotFound();
                }

                var sManager = await _context.SManagers.FindAsync(id);
                if (sManager == null)
                {
                    return NotFound();
                }
                return View(sManager);
            }
            else if (isInternGuidValid == true || isManagerGuidValid == true)
            {
                return RedirectToAction("Error", "Home");
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
           
        }

        // POST: SManagers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,Email,PhoneNumber,Username,Password,Guid")] SManager sManager)
        {
            if (id != sManager.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                List<string> errorMessages = new List<string>();
                if (string.IsNullOrEmpty(sManager.FirstName) || string.IsNullOrEmpty(sManager.LastName) || string.IsNullOrEmpty(sManager.Email) || string.IsNullOrEmpty(sManager.PhoneNumber))
                {
                    errorMessages.Add("Lütfen istenen bilgileri doldurunuz.");
                }
                if (!string.IsNullOrEmpty(sManager.LastName) && !IsValidEmail(sManager.Email))
                {
                    errorMessages.Add(" Geçersiz email formatı.");
                }
                if (!string.IsNullOrEmpty(sManager.LastName) && !IsValidPhoneNumber(sManager.PhoneNumber))
                {
                    errorMessages.Add(" Geçersiz telefon numarası formatı.");
                }
                if (errorMessages.Count > 0)
                {
                    TempData["Message"] = string.Join(" ", errorMessages);
                    TempData["AlertClass"] = "alert-danger";
                    return View(sManager);
                } 
                
                try
                {
                    _context.Update(sManager);
                    await _context.SaveChangesAsync();
                    if (HttpContext.Session.GetString("Username") != null)
                    {
                        TempData["Message"] = "Yetkili düzenlendi.";
                        TempData["AlertClass"] = "alert-success";
                        return RedirectToAction(nameof(Index));
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SManagerExists(sManager.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(sManager);
        }

        // GET: SManagers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            var guidString = HttpContext.Session.GetString("Guid");

            if (string.IsNullOrWhiteSpace(guidString) || !Guid.TryParse(guidString, out Guid userGuid))
            {
                return RedirectToAction("Login", "Home");
            }
            var userAdminExist = new UserAdminExist(_context);
            bool isAdminGuidValid = userAdminExist.CheckGuid(HttpContext.Session.GetString("Guid"));
            var userInternExist = new UserInternExist(_context);
            bool isInternGuidValid = userInternExist.CheckGuid(HttpContext.Session.GetString("Guid"));
            var userManagerExist = new UserManagerExist(_context);
            bool isManagerGuidValid = userManagerExist.CheckGuid(HttpContext.Session.GetString("Guid"));

            if (HttpContext.Session.GetString("UserId") != null && isAdminGuidValid == true)
            {
                if (id == null || _context.SManagers == null)
                {
                    return NotFound();
                }

                var sManager = await _context.SManagers
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (sManager == null)
                {
                    return NotFound();
                }

                return View(sManager);
            }
            else if (isInternGuidValid == true || isManagerGuidValid == true)
            {
                return RedirectToAction("Error", "Home");
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
            
        }

        // POST: SManagers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.SManagers == null)
            {
                return Problem("Entity set 'StajyerTakipSistemiDbContext.SManagers' is null.");
            }

            var sManager = await _context.SManagers
                .Include(s => s.SInternToManagers)
                .Include(s => s.SMessagesforinterns)
                .Include(s => s.SMessagesformanagers)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (sManager != null)
            {
                _context.SInternToManagers.RemoveRange(sManager.SInternToManagers);
                _context.SMessagesforinterns.RemoveRange(sManager.SMessagesforinterns);
                _context.SMessagesformanagers.RemoveRange(sManager.SMessagesformanagers);
                _context.SManagers.Remove(sManager);
            }

            await _context.SaveChangesAsync();
            TempData["Message"] = "Silindi.";
            TempData["AlertClass"] = "alert-success";
            return RedirectToAction(nameof(Index));
        }

        private bool SManagerExists(int id)
        {
            return (_context.SManagers?.Any(e => e.Id == id)).GetValueOrDefault();
        }

    }
}
