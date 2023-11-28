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
    public class SManagersController : Controller
    {
        private readonly StajyerTakipSistemiDbContext _context;

        public SManagersController(StajyerTakipSistemiDbContext context)
        {
            _context = context;
        }

        // GET: SManagers
        public async Task<IActionResult> Index()
        {
              return _context.SManagers != null ? 
                          View(await _context.SManagers.ToListAsync()) :
                          Problem("Entity set 'StajyerTakipSistemiDbContext.SManagers'  is null.");
        }

        // GET: SManagers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.SManagers == null)
            {
                return NotFound();
            }

            var sManager = await _context.SManagers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sManager == null)
            {
                return NotFound();
            }

            return View(sManager);
        }

        // GET: SManagers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: SManagers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,Email,PhoneNumber,Username,Password,Guid")] SManager sManager)
        {
            if (ModelState.IsValid)
            {
                _context.Add(sManager);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Yetkili kişi eklendi.";
                TempData["AlertClass"] = "alert-success";
                return RedirectToAction(nameof(Index));
            }
            return View(sManager);
        }

        // GET: SManagers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.SManagers == null)
            {
                return NotFound();
            }

            var sManager = await _context.SManagers.FindAsync(id);
            if (sManager == null)
            {
                return NotFound();
            }
            return View(sManager);
        }

        // POST: SManagers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,Email,PhoneNumber,Username,Password,Guid")] SManager sManager)
        {
            if (id != sManager.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(sManager);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SManagerExists(sManager.Id))
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
            return View(sManager);
        }

        // GET: SManagers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.SManagers == null)
            {
                return NotFound();
            }

            var sManager = await _context.SManagers
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sManager == null)
            {
                return NotFound();
            }

            return View(sManager);
        }

        // POST: SManagers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.SManagers == null)
            {
                return Problem("Entity set 'StajyerTakipSistemiDbContext.SManagers' is null.");
            }

            var sManager = await _context.SManagers
                .Include(s => s.SInternToManagers)
                .Include(s => s.SMessagesforinterns)
                .Include(s => s.SMessagesformanagers)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (sManager != null)
            {
                _context.SInternToManagers.RemoveRange(sManager.SInternToManagers);
                _context.SMessagesforinterns.RemoveRange(sManager.SMessagesforinterns);
                _context.SMessagesformanagers.RemoveRange(sManager.SMessagesformanagers);
                _context.SManagers.Remove(sManager);
            }

            await _context.SaveChangesAsync();
            TempData["Message"] = "Silindi.";
            TempData["AlertClass"] = "alert-success";
            return RedirectToAction(nameof(Index));
        }

        private bool SManagerExists(int id)
        {
            return (_context.SManagers?.Any(e => e.Id == id)).GetValueOrDefault();
        }

    }
}
