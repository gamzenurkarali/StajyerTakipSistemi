using Microsoft.AspNetCore.Mvc;
using StajyerTakipSistemi.Web.Models;

namespace WebApplication6.Web.Controllers
{
    public class ChatController : Controller
    {
        private readonly StajyerTakipSistemiDbContext _context;

        public ChatController(StajyerTakipSistemiDbContext context)
        {
            _context = context;
        }

        public IActionResult Chat()
        {
            if (HttpContext.Session.GetString("Username").StartsWith("G"))
            {

                return RedirectToAction("Index", "Chat");
            }
            else if (HttpContext.Session.GetString("Username").StartsWith("M"))
            {

                return RedirectToAction("ManagerChat", "Chat");
            }
            else
            {
                return RedirectToAction("Error", "Home");
            }
        }
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("Username")?.StartsWith("G") == true)
            {
                 
                var manager = _context.SInternToManagers
                    .Where(s => s.InternId == int.Parse(HttpContext.Session.GetString("UserId")))
                    .Select(s => s.Manager)
                    .FirstOrDefault();

                if (manager != null)
                {
                    return RedirectToAction("getusermessage", new { userGuid = manager.Guid });
                }
                else
                {
                    TempData["Message"] = "Hiçbir yetkiliyle eşleştirilmemişsiniz. Sorumlu kişiden bir yetkili atanmasını talep ediniz.";
                    TempData["AlertClass"] = "alert-danger";
                    return View();
                }
            }
            else
            {
                // "G" ile başlamayan kullanıcılar için 
                return View();
            }
        }


        public IActionResult ManagerChat()
        {
            var loggedInUserGuid = Guid.Parse(HttpContext.Session.GetString("Guid"));

            MessageViewModel viewModel;

            if (HttpContext.Session.GetString("Username").StartsWith("M"))
            {
                 
                var users = _context.SInternToManagers
                    .Where(s => s.ManagerId == int.Parse(HttpContext.Session.GetString("UserId")))
                    .Select(s => s.Intern)
                    .ToList();
                viewModel = new MessageViewModel
                {
                    LoggedInUserGuid = loggedInUserGuid,
                    Messages = new List<Message>(), 
                    UsersIntern = users,
                    NewMessages = _context.NewMessages.ToList()
                };
            }
            else if (HttpContext.Session.GetString("Username").StartsWith("G"))
            {
               
                var users = _context.SInternToManagers
                    .Where(s => s.InternId == int.Parse(HttpContext.Session.GetString("UserId")))
                    .Select(s => s.Manager)
                    .ToList();

                viewModel = new MessageViewModel
                {
                    LoggedInUserGuid = loggedInUserGuid,
                    Messages = new List<Message>(),  
                    UsersManager = users,
                    NewMessages = _context.NewMessages.ToList()
                };
            }
            else
            {
                 
                return View("Error");  
            }

            return View(viewModel);
        }
         
        

        public IActionResult getmessage()
        {
            var loggedInUserGuid = Guid.Parse(HttpContext.Session.GetString("Guid"));

            MessageViewModel viewModel;

            if (HttpContext.Session.GetString("Username").StartsWith("M"))
            {
                 
                var users = _context.SInternToManagers
                    .Where(s => s.ManagerId == int.Parse(HttpContext.Session.GetString("UserId")))
                    .Select(s => s.Intern)
                    .ToList();

                viewModel = new MessageViewModel
                {
                    LoggedInUserGuid = loggedInUserGuid,
                    Messages = _context.Messages.ToList(), 
                    UsersIntern = users,
                    NewMessages = _context.NewMessages.ToList()
                };
            }
            else if (HttpContext.Session.GetString("Username").StartsWith("G"))
            {
                
                var users = _context.SInternToManagers
                    .Where(s => s.InternId == int.Parse(HttpContext.Session.GetString("UserId")))
                    .Select(s => s.Manager)
                    .ToList();

                viewModel = new MessageViewModel
                {
                    LoggedInUserGuid = loggedInUserGuid,
                    Messages = _context.Messages.ToList(),  
                    UsersManager = users,
                    NewMessages = _context.NewMessages.ToList()
                };
            }
            else
            {
                 
                return View("Error");  
            }

            return View(viewModel);
        }


         
        // Kullanıcının mesajlarını göster
        public IActionResult getusermessage(string userGuid)
        {
            var loggedInUserGuid = Guid.Parse(HttpContext.Session.GetString("Guid"));
            var getuserguid = new Guid(userGuid);
            var messages = _context.Messages
                .Where(m => (m.Sender == loggedInUserGuid && m.Receiver == getuserguid) || (m.Sender == getuserguid && m.Receiver == loggedInUserGuid))
                .ToList();


            var groupedMessages = messages
                 .GroupBy(msg => DateTimeOffset.FromUnixTimeSeconds(msg.UnixTime.Value).Date)
                 .Select(group => new GroupedMessagesViewModel
                 {
                     Date = group.Key,
                     Messages = group.ToList()
                 }).ToList();

            // Bildirimleri sil
            var messagesToDelete = _context.NewMessages
                .Where(m => (m.Sender == getuserguid && m.Receiver == loggedInUserGuid))
                .ToList();

            foreach (var message in messagesToDelete)
            {
                _context.NewMessages.Remove(message);
            }

            _context.SaveChanges();

            MessageViewModel viewModel;
            if (HttpContext.Session.GetString("Username").StartsWith("M"))
            {
                
                var users = _context.SInternToManagers
                            .Where(s => s.ManagerId == int.Parse(HttpContext.Session.GetString("UserId")))
                            .Select(s => s.Intern)
                            .ToList();
                var intern = _context.SInterns
                             .Where(s => s.Guid == new Guid(userGuid))
                             .FirstOrDefault(); 

                viewModel = new MessageViewModel
                {
                    LoggedInUserGuid = Guid.Parse(HttpContext.Session.GetString("Guid")),
                    Messages = messages,
                    UsersIntern = users,
                    TheIntern=intern,
                    NewMessages = _context.NewMessages.ToList(),
                    Messagesgrouped = groupedMessages
                };
                viewModel.SelectedUserGuid = getuserguid;
                ViewBag.SelectedUserGuid = getuserguid;
                return View("ManagerChat", viewModel);
            }
            else if (HttpContext.Session.GetString("Username").StartsWith("G"))
            {
             
                var users = _context.SInternToManagers
                    .Where(s => s.InternId == int.Parse(HttpContext.Session.GetString("UserId")))
                    .Select(s => s.Manager)
                    .ToList();

                var manager = _context.SManagers
                             .Where(s => s.Guid == new Guid(userGuid))
                             .FirstOrDefault(); 

                viewModel = new MessageViewModel
                {
                    LoggedInUserGuid = Guid.Parse(HttpContext.Session.GetString("Guid")),
                    Messages = messages,
                    UsersManager = users,
                    TheManager=manager,
                    NewMessages = _context.NewMessages.ToList(),
                    Messagesgrouped = groupedMessages
                };
                return View("Index", viewModel);
            }
            else
            {
                
                return View("Error");  
            }

            
        }

    }
}

