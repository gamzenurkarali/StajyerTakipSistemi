using Microsoft.AspNetCore.Mvc;
using StajyerTakipSistemi.Web.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StajyerTakipSistemi.Web.Controllers
{
    public class DailyReportController : Controller
    {
        private readonly StajyerTakipSistemiDbContext _context;

        public DailyReportController(StajyerTakipSistemiDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
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

        public IActionResult ShowReportsOfTheIntern(Guid guid)
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
                return View("Index",reports);
            }
            else
            {
                ViewBag.Message = "Stajyerin günlük raporu bulunmamaktadır.";
                return View("Index", reports);
            }
        }


    }
}
