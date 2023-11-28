using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Bcpg;
using StajyerTakipSistemi.Web.Controllers;
using StajyerTakipSistemi.Web.Models;

namespace WebApplication6.Web.Controllers
{
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




    }
}
