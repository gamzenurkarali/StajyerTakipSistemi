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
    public class SInternToManagersController : Controller
    {
        private readonly StajyerTakipSistemiDbContext _context;

        public SInternToManagersController(StajyerTakipSistemiDbContext context)
        {
            _context = context;
        }

        // GET: SInternToManagers
        public async Task<IActionResult> Index()
        {
            var stajyerTakipSistemiDbContext = _context.SInternToManagers.Include(s => s.Intern).Include(s => s.Manager);
            return View(await stajyerTakipSistemiDbContext.ToListAsync());
        }

        // GET: SInternToManagers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.SInternToManagers == null)
            {
                return NotFound();
            }

            var sInternToManager = await _context.SInternToManagers
                .Include(s => s.Intern)
                .Include(s => s.Manager)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (sInternToManager == null)
            {
                return NotFound();
            }

             
            ViewData["InternName"] = sInternToManager.Intern.FirstName + " " + sInternToManager.Intern.LastName;
            ViewData["ManagerName"] = sInternToManager.Manager.FirstName + " " + sInternToManager.Manager.LastName;

            return View(sInternToManager);
        }

        // GET: SInternToManagers/Create
        public IActionResult Create()
        {
            var internList = _context.SInterns.Select(i => new
            {
                Id = i.Id,
                FullName = i.FirstName + " " + i.LastName  
            }).ToList();

            var managerList = _context.SManagers.Select(m => new
            {
                Id = m.Id,
                FullName = m.FirstName + " " + m.LastName  
            }).ToList();

            ViewBag.InternId = new SelectList(internList, "Id", "FullName");
            ViewBag.ManagerId = new SelectList(managerList, "Id", "FullName");

            return View();
        }

        // POST: SInternToManagers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,InternId,ManagerId")] SInternToManager sInternToManager)
        {


            var internList = _context.SInterns.Select(i => new
            {
                Id = i.Id,
                FullName = i.FirstName + " " + i.LastName
            }).ToList();

            var managerList = _context.SManagers.Select(m => new
            {
                Id = m.Id,
                FullName = m.FirstName + " " + m.LastName
            }).ToList();

            
            if (ModelState.IsValid)
            {
                if (_context.SInternToManagers.Any(s => s.InternId == sInternToManager.InternId))
                {

                    ViewBag.InternId = new SelectList(internList, "Id", "FullName");
                    ViewBag.ManagerId = new SelectList(managerList, "Id", "FullName");
                    TempData["Message"] = "Bu stajyer zaten bir yetkili ile eşleştirilmiş.";
                    TempData["AlertClass"] = "alert-danger";
                    return View(sInternToManager);
                }

                _context.Add(sInternToManager);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Eşleştirme başarıyla tamamlandı.";
                TempData["AlertClass"] = "alert-success";
                ViewBag.InternId = new SelectList(internList, "Id", "FullName");
                ViewBag.ManagerId = new SelectList(managerList, "Id", "FullName");
                return RedirectToAction(nameof(Index));
            }
            ViewBag.InternId = new SelectList(internList, "Id", "FullName");
            ViewBag.ManagerId = new SelectList(managerList, "Id", "FullName");
            return View(sInternToManager);
        }

        // GET: SInternToManagers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.SInternToManagers == null)
            {
                return NotFound();
            }

            var sInternToManager = await _context.SInternToManagers.FindAsync(id);

            if (sInternToManager == null)
            {
                return NotFound();
            }

             
            var internList = _context.SInterns.Select(i => new
            {
                Id = i.Id,
                FullName = i.FirstName + " " + i.LastName
            }).ToList();

            var managerList = _context.SManagers.Select(m => new
            {
                Id = m.Id,
                FullName = m.FirstName + " " + m.LastName
            }).ToList();

            ViewBag.InternId = new SelectList(internList, "Id", "FullName", sInternToManager.InternId);
            ViewBag.ManagerId = new SelectList(managerList, "Id", "FullName", sInternToManager.ManagerId);


            //var intern = _context.SInterns.FirstOrDefault(s=>s.Id==sInternToManager.InternId);
            //ViewBag.Intern = intern;
            return View(sInternToManager);
        }

        // POST: SInternToManagers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,InternId,ManagerId")] SInternToManager sInternToManager)
        {
            var internList = _context.SInterns.Select(i => new
            {
                Id = i.Id,
                FullName = i.FirstName + " " + i.LastName
            }).ToList();

            var managerList = _context.SManagers.Select(m => new
            {
                Id = m.Id,
                FullName = m.FirstName + " " + m.LastName
            }).ToList();
            if (id != sInternToManager.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (_context.SInternToManagers.Any(s => s.InternId == sInternToManager.InternId && s.ManagerId==sInternToManager.ManagerId))
                    {
                        TempData["Message"] = "Bu stajyer ve yetkili zaten eşleştirilmiş.";
                        TempData["AlertClass"] = "alert-danger";
                        ViewBag.InternId = new SelectList(internList, "Id", "FullName", sInternToManager.InternId);
                        ViewBag.ManagerId = new SelectList(managerList, "Id", "FullName", sInternToManager.ManagerId);

                        return View();
                    }
                    _context.Update(sInternToManager);
                    await _context.SaveChangesAsync();
                    TempData["Message"] = "Eşleştirme başarıyla tamamlandı.";
                    TempData["AlertClass"] = "alert-success";
                    ViewBag.InternId = new SelectList(internList, "Id", "FullName", sInternToManager.InternId);
                    ViewBag.ManagerId = new SelectList(managerList, "Id", "FullName", sInternToManager.ManagerId);

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SInternToManagerExists(sInternToManager.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                 
            }
            ViewBag.InternId = new SelectList(internList, "Id", "FullName", sInternToManager.InternId);
            ViewBag.ManagerId = new SelectList(managerList, "Id", "FullName", sInternToManager.ManagerId);

            return View(sInternToManager);
        }

        // GET: SInternToManagers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.SInternToManagers == null)
            {
                return NotFound();
            }

            var sInternToManager = await _context.SInternToManagers
                .Include(s => s.Intern)
                .Include(s => s.Manager)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (sInternToManager == null)
            {
                return NotFound();
            }

            
            ViewData["InternName"] = sInternToManager.Intern.FirstName + " " + sInternToManager.Intern.LastName;
            ViewData["ManagerName"] = sInternToManager.Manager.FirstName + " " + sInternToManager.Manager.LastName;

            return View(sInternToManager);
        }

        // POST: SInternToManagers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.SInternToManagers == null)
            {
                return Problem("Entity set 'StajyerTakipSistemiDbContext.SInternToManagers'  is null.");
            }
            var sInternToManager = await _context.SInternToManagers.FindAsync(id);
            if (sInternToManager != null)
            {
                _context.SInternToManagers.Remove(sInternToManager);
            }
            
            await _context.SaveChangesAsync();
            TempData["Message"] = "Silindi.";
            TempData["AlertClass"] = "alert-success";
            return RedirectToAction(nameof(Index));
        }

        private bool SInternToManagerExists(int id)
        {
          return (_context.SInternToManagers?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
