using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using StajyerTakipSistemi.Web.Models;
using System.Net.Mail;
using MailKit.Net.Smtp;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;
using static System.Net.Mime.MediaTypeNames;

namespace StajyerTakipSistemi.Web.Controllers
{
    public class SInternsController : Controller
    {
        private readonly StajyerTakipSistemiDbContext _context;

        public SInternsController(StajyerTakipSistemiDbContext context)
        {
            _context = context;
        }
        private bool IsValidEmail(string email)
        {
            return !string.IsNullOrEmpty(email) && email.Contains("@");
        }

        private bool IsValidPhoneNumber(string phoneNumber)
        {
            return !string.IsNullOrEmpty(phoneNumber) && phoneNumber.Length == 11 && phoneNumber.All(char.IsDigit);
        }
        private string GenerateUniqueUsername()
        {
            string username = "";

            do
            {

                username = "G" + new Random().Next(10000000, 99999999).ToString("D4");
            } while (_context.SInterns.Any(i => i.Username == username));

            return username;
        }


        private async Task<string> GenerateUniquePasswordAsync()
        {
            string password;
            SIntern existingIntern;

            do
            {

                password = GenerateRandomPassword();


                existingIntern = await _context.SInterns.FirstOrDefaultAsync(s => s.Password == password);


            } while (existingIntern != null);

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
        // GET: SInterns
        public async Task<IActionResult> Index()
        {
              return _context.SInterns != null ? 
                          View(await _context.SInterns.ToListAsync()) :
                          Problem("Entity set 'StajyerTakipSistemiDbContext.SInterns'  is null.");
        }

        // GET: SInterns/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.SInterns == null)
            {
                return NotFound();
            }

            var sIntern = await _context.SInterns
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sIntern == null)
            {
                return NotFound();
            }

            return View(sIntern);
        }

        // GET: SInterns/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: SInterns/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,Email,PhoneNumber,BirthDate,Address,DesiredField,Explanation,AccessStatus,StartDate,EndDate,Username,Password")] SIntern sIntern)
        {
            if (ModelState.IsValid)
            {

                List<string> errorMessages = new List<string>();

                if (string.IsNullOrEmpty(sIntern.FirstName) || string.IsNullOrEmpty(sIntern.LastName) || string.IsNullOrEmpty(sIntern.Email) || string.IsNullOrEmpty(sIntern.PhoneNumber) || string.IsNullOrEmpty(sIntern.Address) || string.IsNullOrEmpty(sIntern.DesiredField) || string.IsNullOrEmpty(sIntern.Explanation) || string.IsNullOrEmpty(sIntern.AccessStatus) || sIntern.BirthDate == null || sIntern.StartDate == null || sIntern.EndDate == null)
                {
                    errorMessages.Add("Lütfen istenen bilgileri doldurunuz.");
                }


                if (!string.IsNullOrEmpty(sIntern.LastName) && !IsValidEmail(sIntern.Email))
                {
                    errorMessages.Add(" Geçersiz email formatı.");
                }


                if (!string.IsNullOrEmpty(sIntern.LastName) && !IsValidPhoneNumber(sIntern.PhoneNumber))
                {
                    errorMessages.Add(" Geçersiz telefon numarası formatı.");
                }


                if (sIntern.BirthDate != null && sIntern.BirthDate == DateTime.MinValue)
                {
                    errorMessages.Add(" Geçerli bir doğum tarihi seçiniz.");
                }

                if (errorMessages.Count > 0)
                {
                    TempData["Message"] = string.Join(" ", errorMessages);
                    TempData["AlertClass"] = "alert-danger";
                    return View(sIntern);
                }



                if (sIntern.EndDate != null  )
                { 
                    sIntern.EndDate=DateTime.Now.AddDays(90);
                }




                var beforeHash = await GenerateUniquePasswordAsync();
                sIntern.Password = PasswordHasher.HashPassword(beforeHash);
                sIntern.Username = GenerateUniqueUsername();
                _context.Add(sIntern);
                await _context.SaveChangesAsync();



                var from = "stajyertakip@gmail.com";
                var to = sIntern.Email;
                var subject = "İNSPİMO Staj Kabul";
                var content = "<h2 style=\"color: #3498db;\">İNSPİMO Staj Başvurunuz Onaylandı!</h2>";
                content += "<p>Staja kabul edildiniz! Aşağıda staj bilgileriniz bulunmaktadır:</p>";
                content += $"<ul>";
                content += $"<li>Kullanıcı Adı: <strong>{sIntern.Username}</strong></li>";
                content += $"<li>Şifre: <strong>{beforeHash}</strong></li>";
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
                        client.Authenticate("stajyertakip@gmail.com", "aircjpwffhjocewl");

                        client.Send(message);
                        client.Disconnect(true);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("E-posta gönderme hatası: " + ex.Message);
                    return View();
                }



                TempData["Message"] = "Stajyer eklendi.";
                TempData["AlertClass"] = "alert-success";
                return RedirectToAction(nameof(Index));
            }
            return View(sIntern);
        }

        // GET: SInterns/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.SInterns == null)
            {
                return NotFound();
            }

            var sIntern = await _context.SInterns.FindAsync(id);
            if (sIntern == null)
            {
                return NotFound();
            }
            return View(sIntern);
        }

        // POST: SInterns/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,Email,PhoneNumber,BirthDate,Address,DesiredField,Explanation,AccessStatus,StartDate,EndDate,Username,Password,Guid")] SIntern sIntern)
        {
            if (id != sIntern.Id)
            {
                return NotFound();
            }

            var existingIntern = await _context.SInterns.FindAsync(id);

            if (existingIntern == null)
            {
                TempData["Message"] = "Stajyer bulunamadı.";
                TempData["AlertClass"] = "alert-danger";
                return View(sIntern);
            }

            // Halihazırda var olan stajyerin Guid değerini koru
            sIntern.Guid = existingIntern.Guid;

            if (ModelState.IsValid)
            {

                List<string> errorMessages = new List<string>();

                if (string.IsNullOrEmpty(sIntern.FirstName) || string.IsNullOrEmpty(sIntern.LastName) || string.IsNullOrEmpty(sIntern.Email) || string.IsNullOrEmpty(sIntern.PhoneNumber) || string.IsNullOrEmpty(sIntern.Address) || string.IsNullOrEmpty(sIntern.DesiredField) || string.IsNullOrEmpty(sIntern.Explanation) || string.IsNullOrEmpty(sIntern.AccessStatus) || sIntern.BirthDate == null || sIntern.StartDate == null || sIntern.EndDate == null)
                {
                    errorMessages.Add("Lütfen istenen bilgileri doldurunuz.");
                }


                if (!string.IsNullOrEmpty(sIntern.LastName) && !IsValidEmail(sIntern.Email))
                {
                    errorMessages.Add(" Geçersiz email formatı.");
                }


                if (!string.IsNullOrEmpty(sIntern.LastName) && !IsValidPhoneNumber(sIntern.PhoneNumber))
                {
                    errorMessages.Add(" Geçersiz telefon numarası formatı.");
                }


                if (sIntern.BirthDate != null && sIntern.BirthDate == DateTime.MinValue)
                {
                    errorMessages.Add(" Geçerli bir doğum tarihi seçiniz.");
                }

                if (errorMessages.Count > 0)
                {
                    TempData["Message"] = string.Join(" ", errorMessages);
                    TempData["AlertClass"] = "alert-danger";
                    return View(sIntern);
                }
                try
                {
                    
                    _context.Entry(existingIntern).State = EntityState.Detached;
                    _context.Update(sIntern);
                    await _context.SaveChangesAsync();
                    TempData["Message"] = "Stajyer düzenlendi.";
                    TempData["AlertClass"] = "alert-success";

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SInternExists(sIntern.Id))
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
            return View(sIntern);
        }


        // GET: SInterns/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.SInterns == null)
            {
                return NotFound();
            }

            var sIntern = await _context.SInterns
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sIntern == null)
            {
                return NotFound();
            }

            return View(sIntern);
        }

        // POST: SInterns/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.SInterns == null)
            {
                return Problem("Entity set 'StajyerTakipSistemiDbContext.SInterns'  is null.");
            }
            var sIntern = await _context.SInterns
                        .Include(s => s.SAbsenceInformations)
                        .Include(s => s.SAssignedTasks)
                        .Include(s => s.SDailyReports)
                        .Include(s => s.SInternToManager)
                        .Include(s => s.SMessagesforinterns)
                        .Include(s => s.SMessagesformanagers)
                        .FirstOrDefaultAsync(s => s.Id == id);

            if (sIntern != null)
            {
                _context.SAbsenceInformations.RemoveRange(sIntern.SAbsenceInformations);
                _context.SAssignedTasks.RemoveRange(sIntern.SAssignedTasks);
                _context.SDailyReports.RemoveRange(sIntern.SDailyReports);
                _context.SInternToManagers.Remove(sIntern.SInternToManager);
                _context.SMessagesforinterns.RemoveRange(sIntern.SMessagesforinterns);
                _context.SMessagesformanagers.RemoveRange(sIntern.SMessagesformanagers);

                _context.SInterns.Remove(sIntern);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Silindi.";
                TempData["AlertClass"] = "alert-success";
            }

            return RedirectToAction(nameof(Index));

        }

        private bool SInternExists(int id)
        {
          return (_context.SInterns?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
