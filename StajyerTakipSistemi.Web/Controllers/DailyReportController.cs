using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StajyerTakipSistemi.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StajyerTakipSistemi.Web.Controllers
{
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public class DailyReportController : Controller
    {
        private readonly StajyerTakipSistemiDbContext _context;

        public DailyReportController(StajyerTakipSistemiDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
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
                var loggedinuserguid = Guid.Parse(HttpContext.Session.GetString("Guid"));
                var reports = _context.SDailyReports
                    .Where(s => s.internGuid == loggedinuserguid)
                    .OrderBy(r => r.UnixTime)
                    .ToList();

                if (reports.Count > 0)
                {
                    return View(reports);
                }
                else
                {
                    ViewBag.Message = "Stajyerin günlük raporu bulunmamaktadır.";
                    return View(reports);
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

        public IActionResult ShowReportsOfTheIntern(Guid guid)
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

            if (HttpContext.Session.GetString("UserId") != null && isManagerGuidValid == true)
            {
                var intern_guid = guid;


                var reports = _context.SDailyReports
                    .Where(s => s.internGuid == intern_guid)
                    .OrderBy(r => r.UnixTime)
                    .ToList();
                ViewBag.InternName = _context.SInterns
                                    .Where(s => s.Guid == intern_guid)
                                    .Select(s => s.FirstName + " " + s.LastName)
                                    .FirstOrDefault();
                if (reports.Count > 0)
                {
                    return View("Index", reports);
                }
                else
                {
                    ViewBag.Message = "Stajyerin günlük raporu bulunmamaktadır.";
                    return View("Index", reports);
                }
            }
            else if (isInternGuidValid == true || isAdminGuidValid == true)
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
