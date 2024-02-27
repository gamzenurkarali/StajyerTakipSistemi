using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Bcpg;
using StajyerTakipSistemi.Web;
using StajyerTakipSistemi.Web.Controllers;
using StajyerTakipSistemi.Web.Models;

namespace WebApplication6.Web.Controllers
{
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public class AbsenceInformationController : Controller
    {
        
        private readonly ILogger<HomeController> _logger;
        private readonly StajyerTakipSistemiDbContext _context;


        public AbsenceInformationController(ILogger<HomeController> logger, StajyerTakipSistemiDbContext context)
        {
            _logger = logger;
            _context = context;
        }


        public async Task<IActionResult> Index(int? id)
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

            var loggedinuserid = int.Parse(HttpContext.Session.GetString("UserId"));
            if ((isInternGuidValid == true && loggedinuserid==id) || isManagerGuidValid == true)
            {
                int internId = 0;

                if (id.HasValue)
                {
                    internId = id.Value; // "id" parametresi aktarıldıysa kullan
                }
                else
                {
                    // "id" parametresi aktarılmamışsa, session'dan "UserId" değerini al
                    string userIdStr = HttpContext.Session.GetString("UserId");

                    if (!string.IsNullOrEmpty(userIdStr) && int.TryParse(userIdStr, out int parsedUserId))
                    {
                        internId = parsedUserId;
                    }
                    else
                    {
                        Console.WriteLine("hata");
                    }
                }

                var intern = _context.SInterns.FirstOrDefault(s => s.Id == internId);

                if (intern != null)
                {
                    var absenceInfos = _context.SAbsenceInformations.Where(s => s.InternId == internId).ToList();
                    List<string> markedDates = absenceInfos
                                .Where(a => a.AbsenceDate.HasValue)
                                .Select(a => a.AbsenceDate.Value.ToString("yyyy-MM-dd"))
                                .ToList();

                    ViewBag.MarkedDates = markedDates;

                    var viewModel = new InternAbsenceViewModel
                    {
                        Intern = intern,
                        AbsenceInfo = absenceInfos,
                    };

                    return View(viewModel);
                }
                else
                {
                    return RedirectToAction("Login");
                }
            }
            else if (isAdminGuidValid==true || (isInternGuidValid == true && loggedinuserid != id))
            {
                return RedirectToAction("Error", "Home");
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }

           
        }




    }
}
