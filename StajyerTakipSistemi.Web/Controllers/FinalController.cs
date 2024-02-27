using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using MimeKit;
using StajyerTakipSistemi.Web.Models;
using System.Net.Mail;
using MailKit.Net.Smtp;
using SmtpClient = MailKit.Net.Smtp.SmtpClient;
using System.Reflection.Metadata;
using Microsoft.EntityFrameworkCore;

namespace StajyerTakipSistemi.Web.Controllers
{
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public class FinalController : Controller
    {
        private readonly StajyerTakipSistemiDbContext _context;

        private readonly IWebHostEnvironment _hostingEnvironment;
        public FinalController(StajyerTakipSistemiDbContext context, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Final()
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

            if (HttpContext.Session.GetString("UserId") != null && isInternGuidValid == true)
            {
                if (HttpContext.Session.GetString("UserId") != null)
                {
                    var loggedinuserr = _context.SInterns.FirstOrDefault(x=>x.Id==int.Parse(HttpContext.Session.GetString("UserId")));

                    if (DateTime.Now.Date == loggedinuserr.EndDate.Date)
                    {
                        var hasRecord = _context.SFinal.Any(s => s.InternId == int.Parse(HttpContext.Session.GetString("UserId")));
                        if (hasRecord)
                        {
                            ViewBag.Flag = true;
                        }
                        else
                        {
                            ViewBag.Flag = false;
                        }
                        return View();
                    }
                    else
                    {
                        return RedirectToAction("Error", "Home");
                    }

                }
                else
                {
                    return View("Login", "Home");
                }
            }
            else if (isAdminGuidValid == true || isManagerGuidValid == true)
            {
                return RedirectToAction("Error", "Home");
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
           
        }

        [HttpPost]
        public async Task<IActionResult> StudentFinalRecord(IFormFile file,string GitHubLink, string YouTubeLink,bool EvaluationStatus)
        {

            try
            {
                if (file != null && GitHubLink!=null && YouTubeLink!=null && EvaluationStatus!=null)
                {
                     
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
                    var uploadsDirectory = Path.Combine(_hostingEnvironment.WebRootPath, "FinalFiles");
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
                     
                    var final = new SFinal
                    {
                        InternId = int.Parse(HttpContext.Session.GetString("UserId")),
                        RelatedDocuments = uniqueFileName,
                        GitHubLink = GitHubLink,
                        YouTubeLink = YouTubeLink,
                        EvaluationStatus= EvaluationStatus,
                        SubmitDate= DateTime.Now,
                    };

                    _context.SFinal.Add(final);
                    await _context.SaveChangesAsync();

                    TempData["Message"] = "Final kayıtları başarıyla yüklendi";
                    TempData["AlertClass"] = "alert-success";
                    return RedirectToAction("InternDashboard","Home");
                }
                else
                {
                    TempData["Message"] = "Lütfen boş bırakılan alanları doldurunuz.";
                    TempData["AlertClass"] = "alert-warning";
                    return RedirectToAction("Final");
                }

               
            }
            catch (Exception ex)
            { 
                return RedirectToAction("Login","Home");
            }
        }


        [HttpPost]
        public async Task<ActionResult> EvaluateProcess(IFormFile file,string FinalId)
        {
            var sFinal = _context.SFinal.FirstOrDefault(s=>s.Id== int.Parse(FinalId));
            try
            {
                if (file != null)
                {
                    byte[] fileData;
                    using (var memoryStream = new MemoryStream())
                    {
                        await file.CopyToAsync(memoryStream);
                        fileData = memoryStream.ToArray();
                    }
                    var intern = _context.SInterns.FirstOrDefault(s=>s.Id==sFinal.InternId);
                    var from = "your gmail address";
                    var to = intern.Email;
                    var subject = "Stajınız puanlandı.";
                    var content = "";

                    try
                    {
                        var message = new MimeMessage();
                        message.From.Add(new MailboxAddress("", from));
                        message.To.Add(new MailboxAddress("", to));
                        message.Subject = subject;

                        var bodyBuilder = new BodyBuilder();
                        bodyBuilder.HtmlBody = content;

                        // Dosyayı attachment olarak ekle
                        using (var memoryStream = new MemoryStream(fileData))
                        {
                            bodyBuilder.Attachments.Add(file.FileName, memoryStream);
                        }

                        message.Body = bodyBuilder.ToMessageBody();

                        using (var client = new SmtpClient())
                        {
                            client.Connect("smtp.gmail.com", 587, false);
                            client.Authenticate("your gmail address", "your gmail application password");

                            client.Send(message);
                            client.Disconnect(true);
                        }


                        sFinal.EvaluationStatus = true;
                        _context.Update(sFinal);
                        await _context.SaveChangesAsync();

                        TempData["Message"] = "Puanlama mail ile gönderildi.";
                        TempData["AlertClass"] = "alert-success";
                        return RedirectToAction("Evaluate");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("E-posta gönderme hatası: " + ex.Message);
                        return RedirectToAction("Evaluate");
                    }
                }
                else
                {
                    TempData["Message"] = "Lütfen dosya seçiniz.";
                    TempData["AlertClass"] = "alert-warning";
                    return RedirectToAction("Evaluate");
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", "Home");
            }
        }
        public async Task<IActionResult> Evaluate(int id)
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

            if (HttpContext.Session.GetString("UserId") != null && (isAdminGuidValid == true || isManagerGuidValid == true))
            {
                TempData["FinalId"] = id;
                return View();
            }
            else if (isInternGuidValid==true)
            {
                return RedirectToAction("Error", "Home");
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }
           
        }
        public async Task<IActionResult> EvaluateForManager(int id)
        {
            TempData["FinalId"] = id;
            return View();
        }



    }
}
