﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StajyerTakipSistemi.Web.Models;
using System.Net.Mail;
using System.Diagnostics;
using System.Net;
using MimeKit;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Threading.Tasks;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;
using Org.BouncyCastle.Tls;
using System.IO;
using Microsoft.Extensions.Hosting.Internal;
using static Google.Apis.Storage.v1.Data.Bucket.LifecycleData;
using SQLitePCL;
using Microsoft.IdentityModel.Tokens;
using static System.Net.Mime.MediaTypeNames;

namespace StajyerTakipSistemi.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly StajyerTakipSistemiDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public HomeController(ILogger<HomeController> logger, StajyerTakipSistemiDbContext context, IWebHostEnvironment hostingEnvironment)
        {
            _logger = logger;
            this._context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            var customer= _context.SInterns.ToList();
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult HomePage()
        {
            return View();
        }
        [HttpPost]
        public IActionResult ValidateApplication(SApplication model)
        {
            List<string> errorMessages = new List<string>();
            if (string.IsNullOrEmpty(model.FirstName) || string.IsNullOrEmpty(model.LastName) || string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.PhoneNumber) || string.IsNullOrEmpty(model.Address) || string.IsNullOrEmpty(model.DesiredField) || string.IsNullOrEmpty(model.Explanation) || model.BirthDate == null)
            {
                errorMessages.Add("Lütfen istenen bilgileri doldurunuz.");
            }
            if (!string.IsNullOrEmpty(model.LastName)&&!IsValidEmail(model.Email))
            {
                errorMessages.Add(" Geçersiz email formatı.");
            }
            if (!string.IsNullOrEmpty(model.LastName) && !IsValidPhoneNumber(model.PhoneNumber))
            {
                errorMessages.Add(" Geçersiz telefon numarası formatı.");
            }
            if (model.BirthDate != null && model.BirthDate == DateTime.MinValue)
            {
                errorMessages.Add(" Geçerli bir doğum tarihi seçiniz.");
            }
            if (errorMessages.Count > 0)
            {
                TempData["Message"] = string.Join(" ", errorMessages);
                TempData["AlertClass"] = "alert-danger";
                return View("HomePage", model);
            }
            return RedirectToAction("addApplicant", "SApplications", model);
        }
        private bool IsValidEmail(string email)
        {
            return !string.IsNullOrEmpty(email) && email.Contains("@");
        }
        private bool IsValidPhoneNumber(string phoneNumber)
        {
            return !string.IsNullOrEmpty(phoneNumber) && phoneNumber.Length == 11 && phoneNumber.All(char.IsDigit);
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(SIntern objUser)
        {
            if (objUser.Username == null || objUser.Password==null)
            {
                TempData["Message"] = "Giriş yapmak için istenen bilgileri giriniz.";
                TempData["AlertClass"]= "alert-danger";
                return View();
            }
            if (ModelState.IsValid)
            {
                var internUser = _context.SInterns
                .Where(a => a.Username.Equals(objUser.Username))
                .FirstOrDefault();

                if (internUser!=null)
                {
                    if (internUser.AccessStatus == "Not active")
                    {
                        TempData["Message"] = "Erişim izniniz yoktur.";
                        TempData["AlertClass"] = "alert-danger";
                        return View();

                    }
                    var isThisPassword = _context.SInterns
                    .Where(a => a.Username.Equals(objUser.Username) && a.Password.Equals(objUser.Password))
                    .FirstOrDefault();
                    if (isThisPassword!=null)
                    {
                        HttpContext.Session.SetString("UserId", internUser.Id.ToString());
                        HttpContext.Session.SetString("Username", internUser.Username);
                        HttpContext.Session.SetString("Firstname", internUser.FirstName);
                        HttpContext.Session.SetString("Lastname", internUser.LastName);
                        HttpContext.Session.SetString("Email", internUser.Email);
                        HttpContext.Session.SetString("Phone", internUser.PhoneNumber);
                        HttpContext.Session.SetString("Birthdate", internUser.BirthDate.ToString());
                        HttpContext.Session.SetString("Address", internUser.Address);
                        HttpContext.Session.SetString("Desiredfield", internUser.DesiredField);
                        HttpContext.Session.SetString("Explanation", internUser.Explanation);
                        HttpContext.Session.SetString("Accessstatus", internUser.AccessStatus);
                        HttpContext.Session.SetString("Startdate", internUser.StartDate.ToString());
                        HttpContext.Session.SetString("Guid", internUser.Guid.ToString());

                        return RedirectToAction("InternDashBoard");
                    }
                    else
                    {
                        TempData["Message"] = "Şifre yanlış.";
                        TempData["AlertClass"] = "alert-danger";
                        return View();
                    }
                }
                
                var managerUser = _context.SManagers
                    .Where(a => a.Username.Equals(objUser.Username) )
                    .FirstOrDefault();
                if (managerUser != null)
                {
                    var isThisPassword = _context.SManagers
                    .Where(a => a.Username.Equals(objUser.Username) && a.Password.Equals(objUser.Password))
                    .FirstOrDefault();
                    if (isThisPassword != null)
                    {
                        HttpContext.Session.SetString("UserId", managerUser.Id.ToString());
                        HttpContext.Session.SetString("Username", managerUser.Username);
                        HttpContext.Session.SetString("Firstname", managerUser.FirstName);
                        HttpContext.Session.SetString("Lastname", managerUser.LastName);
                        HttpContext.Session.SetString("Email", managerUser.Email);
                        HttpContext.Session.SetString("Phone", managerUser.PhoneNumber);
                        HttpContext.Session.SetString("Guid", managerUser.Guid.ToString());

                        return RedirectToAction("ManagerDashBoard");
                    }
                    else
                    {
                        TempData["Message"] = "Şifre yanlış.";
                        TempData["AlertClass"] = "alert-danger";
                        return View();
                    }
                }
                
                var adminUser = _context.admin
                    .Where(a => a.Username.Equals(objUser.Username))
                    .FirstOrDefault();
                 if (adminUser != null)
                {
                    var isThisPassword = _context.admin
                    .Where(a => a.Username.Equals(objUser.Username) && a.Password.Equals(objUser.Password))
                    .FirstOrDefault();
                    if (isThisPassword != null)
                    {
                        HttpContext.Session.SetString("UserId", adminUser.Id.ToString());
                        HttpContext.Session.SetString("Username", adminUser.Username);

                        return RedirectToAction("DashBoard");
                    }
                    else
                    {
                        TempData["Message"] = "Şifre yanlış.";
                        TempData["AlertClass"] = "alert-danger";
                        return View();
                    }
                }
                if (adminUser == null || internUser == null || managerUser == null) {
                     
                        TempData["Message"] = "Böyle bir kullanıcı bulunmamakta.";
                        TempData["AlertClass"] = "alert-danger";
                        return View();
                    
                }

                

            }
            return View(objUser);
        }

        public IActionResult Dashboard( )
        {
            if (HttpContext.Session.GetString("Username").StartsWith("G"))
            {
               
                return RedirectToAction("InternDashboard", "Home");
            }
            else if (HttpContext.Session.GetString("Username").StartsWith("M"))
            {
                
                return RedirectToAction("ManagerDashboard", "Home");
            }
            else if (HttpContext.Session.GetString("Username").StartsWith("a"))
            {

                return RedirectToAction("Index", "SApplications");
            }
            else
            {
                return RedirectToAction("Error", "Home");
            }
        }
         
        [HttpPost]
        public async Task<IActionResult> DeleteReport(int reportId)
        {
            try
            {
                // O güne ait raporu kontrol et
                var existingReport = _context.SDailyReports.FirstOrDefault(r => r.Id == reportId);

                if (existingReport == null)
                {
                    TempData["Message"] = "Rapor bulunamadı!";
                    TempData["AlertClass"] = "alert-info";
                    return RedirectToAction("InternDashboard");
                }

                if (System.IO.File.Exists(existingReport.FilePath))
                {
                    System.IO.File.Delete(existingReport.FilePath);
                }

                 
                _context.SDailyReports.Remove(existingReport);
                await _context.SaveChangesAsync();

                TempData["Message"] = "Rapor başarıyla silindi.";
                TempData["AlertClass"] = "alert-success";
                return Json(new { success = true });  // Başarı durumunu JSON olarak geri dön
            }
            catch (Exception ex)
            {
                TempData["Message"] = "Rapor silinemedi!";
                TempData["AlertClass"] = "alert-danger";
                return Json(new
                {
                    success = false,
                    error = ex.Message

                }); 
            }
        }

        public IActionResult deneme()
        {
            try
            {
                var userIdString = HttpContext.Session.GetString("UserId");

                if (string.IsNullOrEmpty(userIdString))
                {
                    // UserId alınamadı, yönlendirme yapılabilir.
                    return RedirectToAction("Login");
                }

                if (!int.TryParse(userIdString, out int userId))
                {
                    // UserId dönüştürülemedi, uygun bir işlem yapılabilir.
                    return RedirectToAction("Login");
                }

                var intern = _context.SInterns.FirstOrDefault(s => s.Id == userId);
                

                if (intern == null)
                {
                    // İlgili stajyer bulunamadı, yönlendirme yapılabilir.
                    return RedirectToAction("Login");
                }
                var loggedinUserGuidString = HttpContext.Session.GetString("Guid");

                if (string.IsNullOrEmpty(loggedinUserGuidString) || !Guid.TryParse(loggedinUserGuidString, out Guid loggedinUserGuid))
                {
                    // Guid alınamadı veya dönüştürülemedi 
                    return RedirectToAction("Login");
                }

                 
                var viewModel = new InternDashboardViewModel
                {
                    Intern = intern
                };



                //Raporları getirme
                var reports = _context.SDailyReports
                    .Where(s => s.internGuid == loggedinUserGuid)
                    .OrderBy(r => r.UnixTime)
                    .ToList();
                if (reports.Count > 0)
                {
                    viewModel.Reports = reports;
                }

                

                
                //Devamsızlık bilgisi getirme
                var absenceInfos = _context.SAbsenceInformations.Where(s => s.InternId == userId).ToList();
                if (absenceInfos.Count > 0)
                {
                    viewModel.AbsenceInfo = absenceInfos;
                }



                //aktif ve son teslim tarihi geçmiş görevleri getirme
                var assignedTasks = _context.SAssignedTasks
                   .Where(t => t.InternId == userId)
                   .ToList();

                var overdueTaskAssigned = assignedTasks.Where(t => t.DueDate.HasValue && t.DueDate < DateTime.Now).ToList();
                var activeTaskAssigned = assignedTasks.Where(t => t.DueDate.HasValue && t.DueDate >= DateTime.Now.Date).ToList();

                var activeTaskDetails = new List<STaskDetail>();

                foreach (var activeTask in activeTaskAssigned)
                {
                    var activeTaskDetail = _context.STaskDetails.FirstOrDefault(td => td.Id == activeTask.TaskId);

                    if (activeTaskDetails != null)
                    {
                        activeTaskDetails.Add(activeTaskDetail);
                    }
                }
                var overdueTaskDetails = new List<STaskDetail>();
                foreach (var overdueTask in overdueTaskAssigned)
                {
                    var overdueTaskDetail = _context.STaskDetails.FirstOrDefault(td => td.Id == overdueTask.TaskId);

                    if (overdueTaskDetails != null)
                    {
                        overdueTaskDetails.Add(overdueTaskDetail);
                    }
                }
                if (overdueTaskAssigned.Count > 0)
                {
                    viewModel.AssignedOverdueTask = overdueTaskAssigned;
                    viewModel.OverdueTaskDetails = overdueTaskDetails;
                }
                if (activeTaskAssigned.Count > 0)
                {
                    viewModel.AssignedActiveTask = activeTaskAssigned;
                    viewModel.ActiveTaskDetails = activeTaskDetails;
                }

                var today = DateTime.Now.Date; // Sadece gün, ay ve yıl bilgisiyle oluşturulan tarih
                var hasReportForToday = _context.SDailyReports.Any(r => r.InternId == userId &&
                                          DateTime.UnixEpoch.AddSeconds(r.UnixTime).Date == today);
                if (hasReportForToday)
                {
                    var existingReport = _context.SDailyReports
                        .FirstOrDefault(r => r.InternId == userId &&
                                              DateTime.UnixEpoch.AddSeconds(r.UnixTime).Date == today);
                    
                        viewModel.ExistingReportForToday = existingReport;
                   
                    
                }
                viewModel.HasReportForToday = hasReportForToday;

                return View(viewModel);
            }
            catch (Exception ex)
            {
                 
                return RedirectToAction("Error");
            }
        }
        public IActionResult InternDashboard()
        {
            try
            {
                var userIdString = HttpContext.Session.GetString("UserId");

                if (string.IsNullOrEmpty(userIdString))
                {
                    // UserId alınamadı .
                    return RedirectToAction("Login");
                }

                if (!int.TryParse(userIdString, out int userId))
                {
                    // UserId dönüştürülemedi 
                    return RedirectToAction("Login");
                }

                var intern = _context.SInterns.FirstOrDefault(s => s.Id == userId);


                if (intern == null)
                {
                    // İlgili stajyer bulunamadı 
                    return RedirectToAction("Login");
                }
                var loggedinUserGuidString = HttpContext.Session.GetString("Guid");

                if (string.IsNullOrEmpty(loggedinUserGuidString) || !Guid.TryParse(loggedinUserGuidString, out Guid loggedinUserGuid))
                {
                    
                    return RedirectToAction("Login");
                }

                 
                var viewModel = new InternDashboardViewModel
                {
                    Intern = intern
                };



                //Raporları getirme
                var reports = _context.SDailyReports
                    .Where(s => s.internGuid == loggedinUserGuid)
                    .OrderBy(r => r.UnixTime)
                    .ToList();
                if (reports.Count > 0)
                {
                    viewModel.Reports = reports;
                }




                //Devamsızlık bilgisi getirme
                var absenceInfos = _context.SAbsenceInformations.Where(s => s.InternId == userId).ToList();
                if (absenceInfos.Count > 0)
                {
                    viewModel.AbsenceInfo = absenceInfos;
                }
                List<string> markedDates = absenceInfos
                           .Where(a => a.AbsenceDate.HasValue)
                           .Select(a => a.AbsenceDate.Value.ToString("yyyy-MM-dd"))
                           .ToList();
                ViewBag.MarkedDates = markedDates;

                //aktif ve son teslim tarihi geçmiş görevleri getirme
                var assignedTasks = _context.SAssignedTasks
                   .Where(t => t.InternId == userId)
                   .ToList();

                var overdueTaskAssigned = assignedTasks.Where(t => t.DueDate.HasValue && t.DueDate < DateTime.Now).ToList();
                var activeTaskAssigned = assignedTasks.Where(t => t.DueDate.HasValue && t.DueDate >= DateTime.Now.Date).ToList();

                var activeTaskDetails = new List<STaskDetail>();

                foreach (var activeTask in activeTaskAssigned)
                {
                    var activeTaskDetail = _context.STaskDetails.FirstOrDefault(td => td.Id == activeTask.TaskId);

                    if (activeTaskDetails != null)
                    {
                        activeTaskDetails.Add(activeTaskDetail);
                    }
                }
                var overdueTaskDetails = new List<STaskDetail>();
                foreach (var overdueTask in overdueTaskAssigned)
                {
                    var overdueTaskDetail = _context.STaskDetails.FirstOrDefault(td => td.Id == overdueTask.TaskId);

                    if (overdueTaskDetails != null)
                    {
                        overdueTaskDetails.Add(overdueTaskDetail);
                    }
                }
                if (overdueTaskAssigned.Count > 0)
                {
                    viewModel.AssignedOverdueTask = overdueTaskAssigned;
                    viewModel.OverdueTaskDetails = overdueTaskDetails;
                }
                if (activeTaskAssigned.Count > 0)
                {
                    viewModel.AssignedActiveTask = activeTaskAssigned;
                    viewModel.ActiveTaskDetails = activeTaskDetails;
                }

                var today = DateTime.Now.Date; 
                var hasReportForToday = _context.SDailyReports.Any(r => r.InternId == userId &&
                                          DateTime.UnixEpoch.AddSeconds(r.UnixTime).Date == today);
                if (hasReportForToday)
                {
                    var existingReport = _context.SDailyReports
                        .FirstOrDefault(r => r.InternId == userId &&
                                              DateTime.UnixEpoch.AddSeconds(r.UnixTime).Date == today);

                    viewModel.ExistingReportForToday = existingReport;


                }


                viewModel.HasReportForToday = hasReportForToday;

                 

                return View(viewModel);
            }
            catch (Exception ex)
            {
                 
                return RedirectToAction("Error");
            }
        }
        

 

  

    public async Task<IActionResult> ManagerDashBoard()
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                var userId = int.Parse(HttpContext.Session.GetString("UserId"));

                var internList = await _context.SInternToManagers
                    .Where(s => s.ManagerId == userId)
                    .Select(s => s.Intern)
                    .ToListAsync();

                var overdueTaskCounts = new Dictionary<int, int>();
                var activeTaskCounts = new Dictionary<int, int>();
                foreach (var intern in internList)
                {
                     
                    var assignedTasks = _context.SAssignedTasks
                        .Where(t => t.InternId == intern.Id)
                        .ToList();

                     
                    var overdueTaskCount = assignedTasks.Count(t => t.DueDate.HasValue && t.DueDate < DateTime.Now);
                    var activeTaskCount = assignedTasks.Count(t => t.DueDate.HasValue && t.DueDate >= DateTime.Now.Date);
                     
                    overdueTaskCounts.Add(intern.Id, overdueTaskCount);
                    activeTaskCounts.Add(intern.Id, activeTaskCount);
                }

           
                var viewModel = new ManagerDashboardViewModel
                {
                    InternList = internList,
                    OverdueTaskCounts = overdueTaskCounts,
                    ActiveTaskCounts = activeTaskCounts
                };

                return View(viewModel);
            }
            else
            {
                return RedirectToAction("Login");
            }
            
        }
        public ActionResult Logout()
        {
            if (HttpContext.Session.GetString("Username").StartsWith("M"))
            {
                HttpContext.Session.SetString("UserId", "");
                HttpContext.Session.SetString("Username", "");
                HttpContext.Session.SetString("Firstname", "");
                HttpContext.Session.SetString("Lastname", "");
                HttpContext.Session.SetString("Email", "");
                HttpContext.Session.SetString("Phone", "");
                HttpContext.Session.SetString("Guid", "");
                return RedirectToAction("Login");
            }
            if (HttpContext.Session.GetString("Username").StartsWith("G"))
            {
                HttpContext.Session.SetString("UserId", "");
                HttpContext.Session.SetString("Username", "");
                HttpContext.Session.SetString("Firstname", "");
                HttpContext.Session.SetString("Lastname", "");
                HttpContext.Session.SetString("Email", "");
                HttpContext.Session.SetString("Phone", "");
                HttpContext.Session.SetString("Birthdate", "");
                HttpContext.Session.SetString("Address", "");
                HttpContext.Session.SetString("Desiredfield", "");
                HttpContext.Session.SetString("Explanation", "");
                HttpContext.Session.SetString("Accessstatus", "");
                HttpContext.Session.SetString("Startdate", "");
                HttpContext.Session.SetString("Guid", "");

                return RedirectToAction("Login");
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
      

        private bool SInternExists(int id)
        {
            return (_context.SInterns?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        public bool SendEmail(string from, string to, string subject, string content)
        {
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

                return true;
            }
            catch (Exception ex)
            {
                 
                Console.WriteLine("E-posta gönderme hatası: " + ex.Message);
                return false;
            }
        }

        [HttpPost]
        public IActionResult CodeSend(string inputName)
        {
            var intern = _context.SInterns.FirstOrDefault(s => s.Email == inputName);
            var manager = _context.SManagers.FirstOrDefault(s => s.Email == inputName);
            string guid = string.Empty;

            if (intern != null)
            {
                guid = intern.Guid.ToString();
            }
            else if (manager != null)
            {
                guid = manager.Guid.ToString();
            }
            else
            {
                TempData["Message"] = "User not found or email could not be sent.";
                TempData["AlertClass"] = "alert-danger";
            }
            if (!string.IsNullOrEmpty(guid))
            {
                var token = Guid.NewGuid().ToString();
                var expirationTime = DateTime.UtcNow.AddHours(1);
                var tokenData = new PasswordResetToken
                {
                    Guid = guid,
                    Token = token,
                    ExpirationTime = expirationTime
                };

                _context.PasswordResetTokens.Add(tokenData);
                _context.SaveChanges();
                
                var from = "stajyertakip@gmail.com";
                var to = inputName;
                var subject = "Reset your password 👾";
                var content = $"Please click the following link to reset your password: https://localhost:44373/Home/ChangePassword?token={token}";

                // E-posta gönderme işlemi
                bool emailSent = SendEmail(from, to, subject, content);

                if (emailSent)
                {
                    ViewBag.EmailSent = true;
                    TempData["Message"] = "Email sent successfully.";
                    TempData["AlertClass"] = "alert-success";
                }
                else
                {
                    ViewBag.EmailSent = false;
                    TempData["Message"] = "User not found or email could not be sent.";
                    TempData["AlertClass"] = "alert-danger";

                }
            }
            else
            {
                ViewBag.EmailSent = false;
                TempData["Message"] = "User not found or email could not be sent.";
                TempData["AlertClass"] = "alert-danger";

            }
            return View("Login");
        }

        [HttpGet]
        public IActionResult ChangePassword(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
               
                TempData["Message"] = "Geçersiz istek!";
                TempData["AlertClass"]= "alert-danger";
                return RedirectToAction("Login", "Home");
            }
            var tokenData = _context.PasswordResetTokens.FirstOrDefault(t => t.Token == token);
            if (tokenData != null && tokenData.ExpirationTime > DateTime.UtcNow)
            {
                ViewBag.Guid = tokenData.Guid;
                return View("ChangePassword");
            }

            TempData["Message"] = "Geçersiz veya süresi dolmuş belirteç!";
            TempData["AlertClass"] = "alert-danger";
            return RedirectToAction("Login", "Home");
            
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(string guid, string newPassword)
        {
            if (string.IsNullOrEmpty(guid) || string.IsNullOrEmpty(newPassword))
            {
                 
                TempData["Message"] = "Geçersiz istek!";
                TempData["AlertClass"] = "alert-danger";
            }

             
            if (Guid.TryParse(guid, out Guid guidValue))
            {
                
                var intern = await _context.SInterns.FirstOrDefaultAsync(s => s.Guid == guidValue);
                var manager = await _context.SManagers.FirstOrDefaultAsync(s => s.Guid == guidValue);

                if (intern != null)
                { 
                    intern.Password = newPassword;
                    _context.Update(intern);
                }
                else if (manager != null)
                {
                    
                    manager.Password = newPassword;
                    _context.Update(manager);
                }
                else
                {
                    
                    TempData["Message"] = "Geçersiz istek!";
                    TempData["AlertClass"] = "alert-danger";
                    return RedirectToAction("ChangePassword", new { guid });
                }

                await _context.SaveChangesAsync();

                // Şifre sıfırlama işlemi başarılı 
                TempData["Message"] = "Şifre başarıyla sıfırlandı!";
                TempData["AlertClass"] = "alert-success";
                return RedirectToAction("Login" );//return RedirectToAction("ChangePassword", new { guid });
            }

            // Geçersiz guid 
            TempData["Message"] = "Geçersiz istek!";
            TempData["AlertClass"] = "alert-danger";
            return RedirectToAction("ChangePassword", new { guid });
        }


        [HttpPost]
        public async Task<IActionResult> UploadFiles(IFormFile File)
        {
            try
            {
                if (File != null && File.Length > 0)
                {
                    var userId = HttpContext.Session.GetString("UserId");
                    if (string.IsNullOrEmpty(userId) || !int.TryParse(userId, out int internId))
                    {
                         
                        return RedirectToAction("Error");
                    }

                    var guidString = HttpContext.Session.GetString("Guid");
                    if (!Guid.TryParse(guidString, out Guid internGuid))
                    {
                        
                        return RedirectToAction("Error");
                    }

                    var today = DateTime.Now.Date;

                    // O güne ait raporu kontrol et
                    var existingReport = _context.SDailyReports
                        .FirstOrDefault(r => r.InternId == internId &&
                                              DateTime.UnixEpoch.AddSeconds(r.UnixTime).Date == today);


                    if (existingReport != null)
                    {
                        // Eğer rapor varsa, eski dosyayı sil
                        var oldFilePath = existingReport.FilePath;
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                        var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(File.FileName);

                        var uploadsDirectory = Path.Combine(_hostingEnvironment.WebRootPath, "uploads");
                        var filePath = Path.Combine(uploadsDirectory, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await File.CopyToAsync(fileStream);
                        }

                        byte[] fileData;
                        using (var memoryStream = new MemoryStream())
                        {
                            await File.CopyToAsync(memoryStream);
                            fileData = memoryStream.ToArray();
                        }
                        // Raporun içeriği güncelle
                        existingReport.FileName = uniqueFileName;
                        existingReport.ContentType = File.ContentType;

                        using (var memoryStream = new MemoryStream())
                        {
                            await File.CopyToAsync(memoryStream);
                            existingReport.Data = memoryStream.ToArray();
                        }

                        existingReport.FilePath = Path.Combine(_hostingEnvironment.WebRootPath, "uploads", existingReport.FileName);
                    }
                    else
                    {
                        // Eğer rapor yoksa, yeni bir rapor oluştur
                        var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(File.FileName);

                        var uploadsDirectory = Path.Combine(_hostingEnvironment.WebRootPath, "uploads");
                        var filePath = Path.Combine(uploadsDirectory, uniqueFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await File.CopyToAsync(fileStream);
                        }

                        byte[] fileData;
                        using (var memoryStream = new MemoryStream())
                        {
                            await File.CopyToAsync(memoryStream);
                            fileData = memoryStream.ToArray();
                        }

                        var newReport = new SDailyReport
                        {
                            FileName = uniqueFileName,
                            ContentType = File.ContentType,
                            Data = fileData,
                            FilePath = filePath,
                            InternId = internId,
                            internGuid = internGuid
                        };

                        _context.SDailyReports.Add(newReport);
                    }

                    await _context.SaveChangesAsync();

                    TempData["Message"] = "File(s) have been successfully uploaded.";
                    TempData["AlertClass"] = "alert-success";
                }
                else
                {
                    TempData["Message"] = "No files were attached. Please select a file to upload.";
                    TempData["AlertClass"] = "alert-warning";
                }

                return RedirectToAction("InternDashboard");
            }
            catch (Exception ex)
            {
                 
                _logger.LogError("An error occurred while uploading files: " + ex.Message);
                return RedirectToAction("Error");
            }
        }






        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file, string taskSubject, int internid, DateTime dueDate)
        {
            try
            {
                if (file != null && file.Length > 0 && !string.IsNullOrWhiteSpace(taskSubject) && dueDate!=null)
                {
                    //var fileName = Path.GetFileName(file.FileName);
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
                    var uploadsDirectory = Path.Combine(_hostingEnvironment.WebRootPath, "TaskFiles");
                    var filePath = Path.Combine(uploadsDirectory, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }

                    byte[] fileData;
                    using (var memoryStream = new MemoryStream())
                    {
                        await file.CopyToAsync(memoryStream);
                        fileData = memoryStream.ToArray();
                    }

                    var task = new STaskDetail
                    {
                        Subject = taskSubject,
                        Contents = uniqueFileName
                    };
                    _context.STaskDetails.Add(task);
                    await _context.SaveChangesAsync();

                    var assignedTask = new SAssignedTask
                    {
                        InternId = internid,
                        TaskId = task.Id,
                        AssignmentDate = DateTime.Now,
                        DueDate = dueDate
                    };

                    _context.SAssignedTasks.Add(assignedTask);
                    await _context.SaveChangesAsync();

                    TempData["Message"] = "Task has been successfully uploaded.";
                    TempData["AlertClass"] = "alert-success";
                }
                else
                {
                    TempData["Message"] = "Please enter Task Subject, select a file, and choose a due date.";
                    TempData["AlertClass"] = "alert-warning";
                }

                return RedirectToAction("ManagerDashboard");
            }
            catch (Exception ex)
            {
               
                _logger.LogError("An error occurred while uploading the file: " + ex.Message);
                return RedirectToAction("Error");
            }
        }
         
    }
}