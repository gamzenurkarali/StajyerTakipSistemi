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
    public class SInternsController : Controller
    {
        private readonly StajyerTakipSistemiDbContext _context;

        public SInternsController(StajyerTakipSistemiDbContext context)
        {
            _context = context;
        }

        // GET: SInterns
        public async Task<IActionResult> Index()
        {
              return _context.SInterns != null ? 
                          View(await _context.SInterns.ToListAsync()) :
                          Problem("Entity set 'StajyerTakipSistemiDbContext.SInterns'  is null.");
        }

        // GET: SInterns/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.SInterns == null)
            {
                return NotFound();
            }

            var sIntern = await _context.SInterns
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sIntern == null)
            {
                return NotFound();
            }

            return View(sIntern);
        }

        // GET: SInterns/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: SInterns/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,Email,PhoneNumber,BirthDate,Address,DesiredField,Explanation,AccessStatus,StartDate,EndDate,Username,Password")] SIntern sIntern)
        {
            if (ModelState.IsValid)
            {
                _context.Add(sIntern);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Stajyer eklendi.";
                TempData["AlertClass"] = "alert-success";
                return RedirectToAction(nameof(Index));
            }
            return View(sIntern);
        }

        // GET: SInterns/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.SInterns == null)
            {
                return NotFound();
            }

            var sIntern = await _context.SInterns.FindAsync(id);
            if (sIntern == null)
            {
                return NotFound();
            }
            return View(sIntern);
        }

        // POST: SInterns/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,Email,PhoneNumber,BirthDate,Address,DesiredField,Explanation,AccessStatus,StartDate,EndDate,Username,Password,Guid")] SIntern sIntern)
        {
            if (id != sIntern.Id)
            {
                return NotFound();
            }

            var existingIntern = await _context.SInterns.FindAsync(id);

            if (existingIntern == null)
            {
                TempData["Message"] = "Stajyer bulunamadı.";
                TempData["AlertClass"] = "alert-danger";
                return View(sIntern);
            }

            // Halihazırda var olan stajyerin Guid değerini koru
            sIntern.Guid = existingIntern.Guid;

            if (ModelState.IsValid)
            {
                try
                {
                    
                    _context.Entry(existingIntern).State = EntityState.Detached;
                    _context.Update(sIntern);
                    await _context.SaveChangesAsync();

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SInternExists(sIntern.Id))
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
            return View(sIntern);
        }


        // GET: SInterns/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.SInterns == null)
            {
                return NotFound();
            }

            var sIntern = await _context.SInterns
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sIntern == null)
            {
                return NotFound();
            }

            return View(sIntern);
        }

        // POST: SInterns/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.SInterns == null)
            {
                return Problem("Entity set 'StajyerTakipSistemiDbContext.SInterns'  is null.");
            }
            var sIntern = await _context.SInterns
                        .Include(s => s.SAbsenceInformations)
                        .Include(s => s.SAssignedTasks)
                        .Include(s => s.SDailyReports)
                        .Include(s => s.SInternToManager)
                        .Include(s => s.SMessagesforinterns)
                        .Include(s => s.SMessagesformanagers)
                        .FirstOrDefaultAsync(s => s.Id == id);

            if (sIntern != null)
            {
                _context.SAbsenceInformations.RemoveRange(sIntern.SAbsenceInformations);
                _context.SAssignedTasks.RemoveRange(sIntern.SAssignedTasks);
                _context.SDailyReports.RemoveRange(sIntern.SDailyReports);
                _context.SInternToManagers.Remove(sIntern.SInternToManager);
                _context.SMessagesforinterns.RemoveRange(sIntern.SMessagesforinterns);
                _context.SMessagesformanagers.RemoveRange(sIntern.SMessagesformanagers);

                _context.SInterns.Remove(sIntern);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Silindi.";
                TempData["AlertClass"] = "alert-success";
            }

            return RedirectToAction(nameof(Index));

        }

        private bool SInternExists(int id)
        {
          return (_context.SInterns?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
