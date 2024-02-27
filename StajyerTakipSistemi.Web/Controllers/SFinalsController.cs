using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StajyerTakipSistemi.Web.Models;

namespace StajyerTakipSistemi.Web.Controllers
{
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    public class SFinalsController : Controller
    {
        private readonly StajyerTakipSistemiDbContext _context;

        public SFinalsController(StajyerTakipSistemiDbContext context)
        {
            _context = context;
        }

        // GET: SFinals
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

                if (_context.SFinal != null)
                {
                    var recs = _context.SFinal.ToList();
                    var interns = _context.SInterns.ToList();

                    ViewData["interns"] = interns;


                    return View(recs);
                }
                else
                {

                    return View(Problem("Entity set 'StajyerTakipSistemiDbContext.SFinal'  is null."));
                }



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

        // GET: SFinals/Details/5
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


                var interns = _context.SInterns.ToList();

                ViewData["interns"] = interns;
                if (id == null || _context.SFinal == null)
                {
                    return NotFound();
                }

                var sFinal = await _context.SFinal
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (sFinal == null)
                {
                    return NotFound();
                }

                return View(sFinal);


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

        // GET: SFinals/Create
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

        // POST: SFinals/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,InternId,RelatedDocuments,GitHubLink,YouTubeLink,EvaluationStatus")] SFinal sFinal)
        {
            if (ModelState.IsValid)
            {
                _context.Add(sFinal);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(sFinal);
        }

        // GET: SFinals/Edit/5
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

                if (id == null || _context.SFinal == null)
                {
                    return NotFound();
                }

                var sFinal = await _context.SFinal.FindAsync(id);
                if (sFinal == null)
                {
                    return NotFound();
                }
                return View(sFinal);



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

        // POST: SFinals/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,InternId,RelatedDocuments,GitHubLink,YouTubeLink,EvaluationStatus")] SFinal sFinal)
        {
            if (id != sFinal.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sFinal);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SFinalExists(sFinal.Id))
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
            return View(sFinal);
        }

        // GET: SFinals/Delete/5
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

                if (id == null || _context.SFinal == null)
                {
                    return NotFound();
                }

                var sFinal = await _context.SFinal
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (sFinal == null)
                {
                    return NotFound();
                }

                return View(sFinal);



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

        // POST: SFinals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.SFinal == null)
            {
                return Problem("Entity set 'StajyerTakipSistemiDbContext.SFinal'  is null.");
            }
            var sFinal = await _context.SFinal.FindAsync(id);
            if (sFinal != null)
            {
                _context.SFinal.Remove(sFinal);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SFinalExists(int id)
        {
          return (_context.SFinal?.Any(e => e.Id == id)).GetValueOrDefault();
        }


        public async Task<IActionResult> ManageFinals()
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




                var userId = int.Parse(HttpContext.Session.GetString("UserId"));

                if (_context.SFinal == null)
                {
                    return View(Problem("Entity set 'StajyerTakipSistemiDbContext.SFinal' is null."));
                }

                var assignedInterns = _context.SInternToManagers
                    .Where(s => s.ManagerId == userId)
                    .ToList();

                var recs = new List<SFinal>();
                var interns = new List<SIntern>();

                foreach (var assignedIntern in assignedInterns)
                {
                    var rec = _context.SFinal.FirstOrDefault(s => s.InternId == assignedIntern.InternId);
                    if (rec != null)
                    {
                        recs.Add(rec);

                        var intern = _context.SInterns.FirstOrDefault(s => s.Id == assignedIntern.InternId);
                        if (intern != null)
                        {
                            interns.Add(intern);
                        }
                    }
                }

                ViewData["interns"] = interns;

                return View(recs);


            }
            else if (isInternGuidValid == true )
            {
                return RedirectToAction("Error", "Home");
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }


        }
        public async Task<IActionResult> DetailsForManager(int? id)
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

                var interns = _context.SInterns.ToList();

                ViewData["interns"] = interns;
                if (id == null || _context.SFinal == null)
                {
                    return NotFound();
                }

                var sFinal = await _context.SFinal
                    .FirstOrDefaultAsync(m => m.Id == id);
                if (sFinal == null)
                {
                    return NotFound();
                }

                return View(sFinal);

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
