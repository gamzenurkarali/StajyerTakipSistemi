using Microsoft.AspNetCore.Mvc;
using StajyerTakipSistemi.Web.Models;

namespace StajyerTakipSistemi.Web.Controllers
{
    public class deneme : Controller
    {
        private readonly StajyerTakipSistemiDbContext _context;

        public deneme(StajyerTakipSistemiDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var values = _context.SManagers.ToList();
            return View(values);
        }
    }
}
