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
    public class SApplicationsController : Controller
    {
        private readonly StajyerTakipSistemiDbContext _context;

        public SApplicationsController(StajyerTakipSistemiDbContext context)
        {
            _context = context;
        }

        // GET: SApplications
        public async Task<IActionResult> Index()
        {
              return _context.SApplications != null ? 
                          View(await _context.SApplications.ToListAsync()) :
                          Problem("Entity set 'StajyerTakipSistemiDbContext.SApplications'  is null.");
        }

        // GET: SApplications/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.SApplications == null)
            {
                return NotFound();
            }

            var sApplication = await _context.SApplications
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sApplication == null)
            {
                return NotFound();
            }

            return View(sApplication);
        }

        // GET: SApplications/Create
        public IActionResult Create()
        {
            return View();
        }
        private bool IsValidEmail(string email)
        {
            return !string.IsNullOrEmpty(email) && email.Contains("@");
        }

        private bool IsValidPhoneNumber(string phoneNumber)
        {
            return !string.IsNullOrEmpty(phoneNumber) && phoneNumber.Length == 11 && phoneNumber.All(char.IsDigit);
        }

        // POST: SApplications/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
         
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,Email,PhoneNumber,BirthDate,Address,DesiredField,Explanation,ApprovalStatus,ApplicationDate")] SApplication sApplication)
        {
            if (ModelState.IsValid)
            {



                List<string> errorMessages = new List<string>();

                if (string.IsNullOrEmpty(sApplication.FirstName) || string.IsNullOrEmpty(sApplication.LastName) || string.IsNullOrEmpty(sApplication.Email) || string.IsNullOrEmpty(sApplication.PhoneNumber) || string.IsNullOrEmpty(sApplication.Address) || string.IsNullOrEmpty(sApplication.DesiredField) || string.IsNullOrEmpty(sApplication.Explanation) || sApplication.BirthDate == null)
                {
                    errorMessages.Add("Lütfen istenen bilgileri doldurunuz.");
                }


                if (!string.IsNullOrEmpty(sApplication.LastName) && !IsValidEmail(sApplication.Email))
                {
                    errorMessages.Add(" Geçersiz email formatı.");
                }


                if (!string.IsNullOrEmpty(sApplication.LastName) && !IsValidPhoneNumber(sApplication.PhoneNumber))
                {
                    errorMessages.Add(" Geçersiz telefon numarası formatı.");
                }


                if (sApplication.BirthDate != null && sApplication.BirthDate == DateTime.MinValue)
                {
                    errorMessages.Add(" Geçerli bir doğum tarihi seçiniz.");
                }


                if (errorMessages.Count > 0)
                {
                    TempData["Message"] = string.Join(" ", errorMessages);
                    TempData["AlertClass"] = "alert-danger";
                    return View(sApplication);
                }

                sApplication.ApplicationDate = DateTime.Now;
                _context.Add(sApplication);
                await _context.SaveChangesAsync();

                if (HttpContext.Session.GetString("Username") != null)
                {
                    TempData["Message"] = "Aday eklendi.";
                    TempData["AlertClass"] = "alert-success";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    TempData["Message"] = "Başvurunuz alınmıştır.";
                    TempData["AlertClass"] = "alert-success";
                    return RedirectToAction( "HomePage","Home" );
                }
            }
            else
            {
                return View(sApplication);
            }


        }
        
        public async Task<IActionResult> addApplicant(SApplication model)
        {
            if (ModelState.IsValid)
            {
                model.ApplicationDate = DateTime.Now;
                _context.Add(model);
                await _context.SaveChangesAsync();

                TempData["Message"] = "Başvurunuz alınmıştır.";
                TempData["AlertClass"] = "alert-success";
                return RedirectToAction("HomePage", "Home");
            }
            else
            {
                 
                TempData["Message"] = "Başvurunuz alınamadı. Lütfen tüm gerekli bilgileri doldurun.";
                TempData["AlertClass"] = "alert-danger";
                return RedirectToAction("HomePage", "Home");
            }
        }

        // GET: SApplications/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.SApplications == null)
            {
                return NotFound();
            }

            var sApplication = await _context.SApplications.FindAsync(id);
            if (sApplication == null)
            {
                return NotFound();
            }
            return View(sApplication);
        }

        // POST: SApplications/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,Email,PhoneNumber,BirthDate,Address,DesiredField,Explanation,ApprovalStatus,ApplicationDate")] SApplication sApplication)
        {
            if (id != sApplication.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    List<string> errorMessages = new List<string>();

                    if (string.IsNullOrEmpty(sApplication.FirstName) || string.IsNullOrEmpty(sApplication.LastName) || string.IsNullOrEmpty(sApplication.Email) || string.IsNullOrEmpty(sApplication.PhoneNumber) || string.IsNullOrEmpty(sApplication.Address) || string.IsNullOrEmpty(sApplication.DesiredField) || string.IsNullOrEmpty(sApplication.Explanation) || sApplication.BirthDate == null)
                    {
                        errorMessages.Add("Lütfen istenen bilgileri doldurunuz.");
                    }


                    if (!string.IsNullOrEmpty(sApplication.LastName) && !IsValidEmail(sApplication.Email))
                    {
                        errorMessages.Add(" Geçersiz email formatı.");
                    }


                    if (!string.IsNullOrEmpty(sApplication.LastName) && !IsValidPhoneNumber(sApplication.PhoneNumber))
                    {
                        errorMessages.Add(" Geçersiz telefon numarası formatı.");
                    }


                    if (sApplication.BirthDate != null && sApplication.BirthDate == DateTime.MinValue)
                    {
                        errorMessages.Add(" Geçerli bir doğum tarihi seçiniz.");
                    }


                    if (errorMessages.Count > 0)
                    {
                        TempData["Message"] = string.Join(" ", errorMessages);
                        TempData["AlertClass"] = "alert-danger";
                        return View(sApplication);
                    }
                    var originalApplication = await _context.SApplications.FindAsync(id);

                    if (sApplication.ApprovalStatus == "Onaylandı")
                    {

                        var internRecord = new SIntern
                        {
                            FirstName = sApplication.FirstName,
                            LastName = sApplication.LastName,
                            Email = sApplication.Email,
                            PhoneNumber = sApplication.PhoneNumber,
                            BirthDate = sApplication.BirthDate,
                            Address = sApplication.Address,
                            DesiredField = sApplication.DesiredField,
                            Explanation = sApplication.Explanation,
                            AccessStatus = "Active",
                            StartDate = DateTime.Now,
                            EndDate = DateTime.Now.AddDays(90),
                            Username = GenerateUniqueUsername(),
                            Password = await GenerateUniquePasswordAsync(),
                            

                        };

                        _context.SInterns.Add(internRecord);
                        await _context.SaveChangesAsync();
                        _context.SInternToManagers.Add(new SInternToManager
                        {
                            InternId = internRecord.Id,
                            ManagerId = GetRandomManagerId()
                        });
                        await _context.SaveChangesAsync();
                        TempData["Message"] = "Kişi stajyer listesine eklendi.";
                        TempData["AlertClass"] = "alert-success";

                        _context.SApplications.Remove(originalApplication);
                        await _context.SaveChangesAsync();
                    }
                    else if (sApplication.ApprovalStatus == "Reddedildi") { 
                        _context.SApplications.Remove(originalApplication);
                        await _context.SaveChangesAsync();
                    }
                    else
                    {
                        TempData["Message"] = "Başvuru düzenlendi.";
                        TempData["AlertClass"] = "alert-success";
                        _context.Entry(originalApplication).State = EntityState.Detached;
                        _context.Update(sApplication);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SApplicationExists(sApplication.Id))
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
            return View(sApplication);
        }
        private int GetRandomManagerId()
        { 
            var randomManagerId = _context.SManagers
                .OrderBy(x => Guid.NewGuid())  
                .Select(x => x.Id)
                .FirstOrDefault();

            return randomManagerId;
        }
        private string GenerateUniqueUsername()
        {
            string username = "";

            do
            {
                 
                username = "G" + new Random().Next(10000000, 99999999).ToString("D4");
            } while (_context.SInterns.Any(i => i.Username == username));

            return username;
        }


        private async Task<string> GenerateUniquePasswordAsync()
        {
            string password;
            SIntern existingIntern;

            do
            {
                 
                password = GenerateRandomPassword();

                  
                existingIntern = await _context.SInterns.FirstOrDefaultAsync(s => s.Password == password);

                 
            } while (existingIntern != null);

            return password;
        }
        private string GenerateRandomPassword()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            var password = new string(Enumerable.Repeat(chars, 8) // Parolanın uzunluğu 8 karakter
                .Select(s => s[random.Next(s.Length)]).ToArray());

            return password;
        }


        // GET: SApplications/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.SApplications == null)
            {
                return NotFound();
            }

            var sApplication = await _context.SApplications
                .FirstOrDefaultAsync(m => m.Id == id);
            if (sApplication == null)
            {
                return NotFound();
            }

            return View(sApplication);
        }

        // POST: SApplications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.SApplications == null)
            {
                return Problem("Entity set 'StajyerTakipSistemiDbContext.SApplications'  is null.");
            }
            var sApplication = await _context.SApplications.FindAsync(id);
            if (sApplication != null)
            {
                _context.SApplications.Remove(sApplication);
            }
            
            await _context.SaveChangesAsync();
            TempData["Message"] = "Silindi.";
            TempData["AlertClass"] = "alert-success";
            return RedirectToAction(nameof(Index));
        }

        private bool SApplicationExists(int id)
        {
          return (_context.SApplications?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
