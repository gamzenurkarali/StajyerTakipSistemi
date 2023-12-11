using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using MailKit.Net.Smtp;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using StajyerTakipSistemi.Web.Models;
using static Google.Apis.Storage.v1.Data.Bucket.LifecycleData;
using Microsoft.Extensions.Hosting.Internal;

namespace StajyerTakipSistemi.Web.Controllers
{
    public class SApplicationsController : Controller
    {
        private readonly StajyerTakipSistemiDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ILogger<SApplicationsController> _logger;
        public SApplicationsController(StajyerTakipSistemiDbContext context, IWebHostEnvironment hostingEnvironment, ILogger<SApplicationsController> logger)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _logger = logger;
        }

        // GET: SApplications
        public async Task<IActionResult> Index()
        {
              return _context.SApplications != null ? 
                          View(await _context.SApplications.ToListAsync()) :
                          Problem("Entity set 'StajyerTakipSistemiDbContext.SApplications'  is null.");
        }

        // GET: SApplications/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.SApplications == null)
            {
                return NotFound();
            }

            var sApplication = await _context.SApplications
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sApplication == null)
            {
                return NotFound();
            }

            return View(sApplication);
        }

        // GET: SApplications/Create
        public IActionResult Create()
        {
            return View();
        }
        private bool IsValidEmail(string email)
        {
            return !string.IsNullOrEmpty(email) && email.Contains("@");
        }

        private bool IsValidPhoneNumber(string phoneNumber)
        {
            return !string.IsNullOrEmpty(phoneNumber) && phoneNumber.Length == 11 && phoneNumber.All(char.IsDigit);
        }

        // POST: SApplications/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
         
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,Email,PhoneNumber,BirthDate,Address,DesiredField,Explanation,ApprovalStatus,ApplicationDate")] SApplication sApplication)
        {
            if (ModelState.IsValid)
            {



                List<string> errorMessages = new List<string>();

                if (string.IsNullOrEmpty(sApplication.FirstName) || string.IsNullOrEmpty(sApplication.LastName) || string.IsNullOrEmpty(sApplication.Email) || string.IsNullOrEmpty(sApplication.PhoneNumber) || string.IsNullOrEmpty(sApplication.Address) || string.IsNullOrEmpty(sApplication.DesiredField) || string.IsNullOrEmpty(sApplication.Explanation) || sApplication.BirthDate == null || sApplication.ApplicationDate == null)
                {
                    errorMessages.Add("Lütfen istenen bilgileri doldurunuz.");
                }


                if (!string.IsNullOrEmpty(sApplication.LastName) && !IsValidEmail(sApplication.Email))
                {
                    errorMessages.Add(" Geçersiz email formatı.");
                }


                if (!string.IsNullOrEmpty(sApplication.LastName) && !IsValidPhoneNumber(sApplication.PhoneNumber))
                {
                    errorMessages.Add(" Geçersiz telefon numarası formatı.");
                }


                if (sApplication.BirthDate != null && sApplication.BirthDate == DateTime.MinValue)
                {
                    errorMessages.Add(" Geçerli bir doğum tarihi seçiniz.");
                }


                if (errorMessages.Count > 0)
                {
                    TempData["Message"] = string.Join(" ", errorMessages);
                    TempData["AlertClass"] = "alert-danger";
                    return View(sApplication);
                }

                sApplication.ApplicationDate = DateTime.Now;
                _context.Add(sApplication);
                await _context.SaveChangesAsync();

                if (HttpContext.Session.GetString("Username") != null)
                {
                    TempData["Message"] = "Aday eklendi.";
                    TempData["AlertClass"] = "alert-success";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["Message"] = "Başvurunuz alınmıştır.";
                    TempData["AlertClass"] = "alert-success";
                    return RedirectToAction( "HomePage","Home" );
                }
            }
            else
            {
                return View(sApplication);
            }


        }

        public async Task<IActionResult> addApplicant(SApplication model)
        {
            if (ModelState.IsValid)
            {
                try
                { 
                    var tempFileName = TempData["TempFileName"]?.ToString();

                    if (!string.IsNullOrEmpty(tempFileName))
                    { 
                        model.ApplicationDate = DateTime.Now;
                        model.Cv = tempFileName;
                        _context.Add(model);

                        await _context.SaveChangesAsync();

                        TempData["Message"] = "Başvurunuz alındı.";
                        TempData["AlertClass"] = "alert-success";
                    }
                    else
                    {
                        TempData["Message"] = "Yüklenmiş dosya bulunamadı. Lütfen yüklemek için bir dosya seçin.";
                        TempData["AlertClass"] = "alert-warning";
                    }

                    return RedirectToAction("HomePage", "Home");
                }
                catch (Exception ex)
                {
                    _logger.LogError("Dosya yükleme sırasında bir hata oluştu: " + ex.Message);
                    return RedirectToAction("Error");
                }
            }
            else
            {
                TempData["Message"] = "Başvurunuz alınamadı. Lütfen tüm gerekli bilgileri doldurun.";
                TempData["AlertClass"] = "alert-danger";
                return RedirectToAction("HomePage", "Home");
            }
        }



        // GET: SApplications/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.SApplications == null)
            {
                return NotFound();
            }

            var sApplication = await _context.SApplications.FindAsync(id);
            if (sApplication == null)
            {
                return NotFound();
            }
            return View(sApplication);
        }

        // POST: SApplications/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Cv,FirstName,LastName,Email,PhoneNumber,BirthDate,Address,DesiredField,Explanation,ApprovalStatus,ApplicationDate")] SApplication sApplication)
        {
            if (id != sApplication.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    List<string> errorMessages = new List<string>();

                    if (string.IsNullOrEmpty(sApplication.FirstName) || string.IsNullOrEmpty(sApplication.LastName) || string.IsNullOrEmpty(sApplication.Email) || string.IsNullOrEmpty(sApplication.PhoneNumber) || string.IsNullOrEmpty(sApplication.Address) || string.IsNullOrEmpty(sApplication.DesiredField) || string.IsNullOrEmpty(sApplication.Explanation) || sApplication.BirthDate == null || sApplication.ApplicationDate == null)
                    {
                        errorMessages.Add("Lütfen istenen bilgileri doldurunuz.");
                    }


                    if (!string.IsNullOrEmpty(sApplication.LastName) && !IsValidEmail(sApplication.Email))
                    {
                        errorMessages.Add(" Geçersiz email formatı.");
                    }


                    if (!string.IsNullOrEmpty(sApplication.LastName) && !IsValidPhoneNumber(sApplication.PhoneNumber))
                    {
                        errorMessages.Add(" Geçersiz telefon numarası formatı.");
                    }


                    if (sApplication.BirthDate != null && sApplication.BirthDate == DateTime.MinValue)
                    {
                        errorMessages.Add(" Geçerli bir doğum tarihi seçiniz.");
                    }


                    if (errorMessages.Count > 0)
                    {
                        TempData["Message"] = string.Join(" ", errorMessages);
                        TempData["AlertClass"] = "alert-danger";
                        return View(sApplication);
                    }
                    var originalApplication = await _context.SApplications.FindAsync(id);
                    var beforeHash = await GenerateUniquePasswordAsync();
                    if (sApplication.ApprovalStatus == "Onaylandı")
                    {

                        var internRecord = new SIntern
                        {
                            FirstName = sApplication.FirstName,
                            LastName = sApplication.LastName,
                            Email = sApplication.Email,
                            PhoneNumber = sApplication.PhoneNumber,
                            BirthDate = sApplication.BirthDate,
                            Address = sApplication.Address,
                            DesiredField = sApplication.DesiredField,
                            Explanation = sApplication.Explanation,
                            AccessStatus = "Active",
                            StartDate = DateTime.Now,
                            EndDate = DateTime.Now.AddDays(90),
                            Username = GenerateUniqueUsername(),
                            Password = PasswordHasher.HashPassword(beforeHash),
                            

                        };

                        _context.SInterns.Add(internRecord);
                        await _context.SaveChangesAsync();
                        _context.SInternToManagers.Add(new SInternToManager
                        {
                            InternId = internRecord.Id,
                            ManagerId = GetRandomManagerId()
                        });
                        await _context.SaveChangesAsync();



                        //Stajyere kabul maili gönderme
                        var from = "stajyertakip@gmail.com";
                        var to = sApplication.Email;
                        var subject = "İNSPİMO Staj Kabul";
                        var content = "<h2 style=\"color: #3498db;\">İNSPİMO Staj Başvurunuz Onaylandı!</h2>";
                        content += "<p>Staja kabul edildiniz! Aşağıda staj bilgileriniz bulunmaktadır:</p>";
                        content += $"<ul>";
                        content += $"<li>Kullanıcı Adı: <strong>{internRecord.Username}</strong></li>";
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
                         


                        TempData["Message"] = "Kişi stajyer listesine eklendi.";
                        TempData["AlertClass"] = "alert-success";

                        _context.SApplications.Remove(originalApplication);
                        await _context.SaveChangesAsync();
                    }
                    else if (sApplication.ApprovalStatus == "Reddedildi") {

                        //Stajyere red bilgi maili gönderme
                        var from = "stajyertakip@gmail.com";
                        var to = sApplication.Email;
                        var subject = "İNSPİMO Staj Başvurunuz Reddedildi";

                        var content = "<h2 style=\"color: #e74c3c;\">İNSPİMO Staj Başvurunuz Reddedildi</h2>";
                        content += "<p>Üzgünüz, staj başvurunuz ne yazık ki kabul edilmemiştir.</p>";
                        content += "<p>Başvurunuzla ilgili detaylı geri bildirim almak için lütfen bizimle iletişime geçin.</p>";
                        content += "<p>İlginiz için teşekkür ederiz.</p>";


                        ////html template 
                        //var content = @"<!DOCTYPE html>
                        //    <html lang=""en"">
                        //    <head>
                        //        <meta charset=""UTF-8"">
                        //        <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                        //        <style>
                        //            body {
                        //                font-family: 'Arial', sans-serif;
                        //                margin: 0;
                        //                padding: 0;
                        //                display: flex;
                        //                flex-direction: column;
                        //                justify-content: center;
                        //            }

                        //            .content-container {
                        //                padding: 2rem;
                        //                display: flex;
                        //                flex-direction: column;
                        //                align-items: center;
                        //                width: 500px;
                        //                box-shadow: rgb(204, 219, 232) 3px 3px 6px 0px inset, rgba(255, 255, 255, 0.5) -3px -3px 6px 1px inset;
                        //            }

                        //            .container {
                        //                width: 80%;
                        //                margin: auto;
                        //                overflow: hidden;
                        //            }

                        //            header {
                        //                background: white;
                        //                padding: 20px 0;
                        //                color: #fff;
                        //                text-align: center;
                        //            }

                        //            header img {
                        //                max-width: 100px;
                        //                height: auto;
                        //            }

                        //            main {
                        //                padding: 20px 0;
                        //                background-color: #fff;
                        //                border-radius: 8px;
                        //                box-shadow: 0 0 10px rgba(0, 0, 0, 0.1);
                        //            }

                        //            h2 {
                        //                color: #ff8b8b;
                        //            }

                        //            p {
                        //                color: #777;
                        //                line-height: 1.6em;
                        //            }

                        //            .cta-button {
                        //                display: inline-block;
                        //                padding: 10px 20px;
                        //                margin-top: 20px;
                        //                background-color: #ff8b8b;
                        //                color: #fff;
                        //                text-decoration: none;
                        //                border-radius: 5px;
                        //            }

                        //            .cta-button:hover {
                        //                background-color: #e25959;
                        //            }

                        //            .social-icons a {
                        //                text-decoration: none;
                        //                color: #ff8b8b;
                        //                margin: 0 10px;
                        //                transition: color 0.3s ease-in-out;
                        //            }

                        //            .social-icons a:hover {
                        //                color: #e25959;
                        //            }
                        //        </style>
                        //    </head>

                        //    <body>
                        //        <div style=""display:flex;flex-direction:column;justify-content:center;align-items: center;"">
                        //            <header>
                        //                <div class=""container"">
                        //                    <img style=""transform: scale(2.5);"" src=""https://yt3.googleusercontent.com/oHpCBs12tkC-nYySgUlu7PAtmKcJ5JIbzdcmrVxd_twhSQQFcQFerk-rLWO-0NDKp6igeN_0--A=w1060-fcrop64=1,00005a57ffffa5a8-k-c0xffffffff-no-nd-rj""
                        //                        alt=""Company Logo"">
                        //                </div>
                        //            </header>

                        //            <div class=""content-container"" style=""box-shadow: rgb(204, 219, 232) 3px 3px 6px 0px inset, rgba(255, 255, 255, 0.5) -3px -3px 6px 1px inset; padding: 2rem; width: 500px;"">
                        //                <h2>Merhaba " + sApplication.FirstName + " " + sApplication.LastName + @",</h2>
                        //                <p>
                        //                    Üzgünüz, staj başvurunuz kabul edilmemiştir.
                        //                </p>
                        //                <p>
                        //                    Başvurunuzla ilgili detaylı geri bildirim almak için lütfen bizimle iletişime geçin.
                        //                </p>
                        //                <p>İlginiz için teşekkür ederiz.</p>
                        //            </div>

                        //            <footer>
                        //                <div class=""container"">
                        //                    <div style=""display:flex;flex-direction:row;justify-content:center;padding:2rem;"" class=""social-icons"">
                        //                        <a href=""https://www.instagram.com/inspimoconsultancy/"" target=""_blank""><img style=""height: 30px;""
                        //                                src=""https://cdn4.iconfinder.com/data/icons/logos-brands-7/512/instagram_icon-instagram_buttoninstegram-256.png""
                        //                                alt=""Instagram""></a>
                        //                        <a href=""https://www.linkedin.com/company/inspimo-consultancy/?originalSubdomain=tr"" target=""_blank""><img
                        //                                style=""height: 30px;""
                        //                                src=""https://cdn1.iconfinder.com/data/icons/logotypes/32/square-linkedin-256.png""
                        //                                alt=""LinkedIn""></a>
                        //                    </div>
                        //                </div>
                        //            </footer>
                        //        </div>
                        //    </body>

                        //    </html>";



                        //Reddedilen kişinin CV sini sil
                        if (!string.IsNullOrEmpty(sApplication.Cv))
                        {
                            var filePath = Path.Combine(_hostingEnvironment.WebRootPath, "CVs", sApplication.Cv);

                            try
                            {
                                if (System.IO.File.Exists(filePath))
                                {
                                    System.IO.File.Delete(filePath);
                                }
                            }
                            catch (Exception ex)
                            {
                                _logger.LogError("Dosya silme hatası: " + ex.Message);
                                return RedirectToAction("Error");
                            }
                        }


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
                            return RedirectToAction("Evaluate");
                        }

                        _context.SApplications.Remove(originalApplication);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        TempData["Message"] = "Başvuru düzenlendi.";
                        TempData["AlertClass"] = "alert-success";
                        _context.Entry(originalApplication).State = EntityState.Detached;
                        _context.Update(sApplication);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SApplicationExists(sApplication.Id))
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
            return View(sApplication);
        }
        private int GetRandomManagerId()
        { 
            var randomManagerId = _context.SManagers
                .OrderBy(x => Guid.NewGuid())  
                .Select(x => x.Id)
                .FirstOrDefault();

            return randomManagerId;
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


        // GET: SApplications/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.SApplications == null)
            {
                return NotFound();
            }

            var sApplication = await _context.SApplications
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sApplication == null)
            {
                return NotFound();
            }

            return View(sApplication);
        }

        // POST: SApplications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.SApplications == null)
            {
                return Problem("Entity set 'StajyerTakipSistemiDbContext.SApplications'  is null.");
            }
            var sApplication = await _context.SApplications.FindAsync(id);
            if (sApplication != null)
            {
                if (sApplication.Cv!=null)
                {
                    var filePath = Path.Combine(_hostingEnvironment.WebRootPath, "CVs", sApplication.Cv);

                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }
                _context.SApplications.Remove(sApplication);
            }
            
            await _context.SaveChangesAsync();
            TempData["Message"] = "Silindi.";
            TempData["AlertClass"] = "alert-success";
            return RedirectToAction(nameof(Index));
        }

        private bool SApplicationExists(int id)
        {
          return (_context.SApplications?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
