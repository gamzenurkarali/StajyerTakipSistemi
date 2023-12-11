using Microsoft.AspNetCore.Mvc;
using StajyerTakipSistemi.Web.Models;

namespace StajyerTakipSistemi.Web.Controllers
{
    public class TaskController : Controller
    {
        public readonly StajyerTakipSistemiDbContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;
        public TaskController(StajyerTakipSistemiDbContext context, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> ShowOverdueTasks(int? id)
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                var intern = _context.SInterns.FirstOrDefault(i => i.Id == id);

                if (intern != null)
                {
                     
                    var assignedTasks = _context.SAssignedTasks.Where(s => s.InternId == id).ToList();
                    var overdueTasks = assignedTasks.Where(t => t.DueDate.HasValue && t.DueDate < DateTime.Now.Date).ToList();
                    var taskDetails = new List<STaskDetail>();  

                    foreach (var activeTask in overdueTasks)
                    {
                        var taskDetail = _context.STaskDetails.FirstOrDefault(td => td.Id == activeTask.TaskId);

                        if (taskDetail != null)
                        {
                            taskDetails.Add(taskDetail);
                        }
                    }

                    var viewModel = new InternTaskViewModel
                    {
                        Intern = intern,
                        AssignedTasks = overdueTasks,
                        TaskDetails = taskDetails
                    };
                    ViewBag.username = HttpContext.Session.GetString("Username");
                    return View(viewModel);
                }

            }

            return RedirectToAction("Login");
        }


        public async Task<IActionResult> ShowActiveTasks(int? id)
        {
            if (HttpContext.Session.GetString("UserId") != null)
            {
                var intern = _context.SInterns.FirstOrDefault(i => i.Id == id);

               if (intern != null)
                    {
                        var assignedTasks = _context.SAssignedTasks.Where(s => s.InternId == id).ToList();
                        var activeTasks = assignedTasks.Where(t => t.DueDate.HasValue && t.DueDate >= DateTime.Now.Date).ToList();
                        var taskDetails = new List<STaskDetail>(); 

                        foreach (var activeTask in activeTasks)
                        {
                            var taskDetail = _context.STaskDetails.FirstOrDefault(td => td.Id == activeTask.TaskId);

                            if (taskDetail != null)
                            {
                                taskDetails.Add(taskDetail);
                            }
                        }

                        var viewModel = new InternTaskViewModel
                        {
                            Intern = intern,
                            AssignedTasks =activeTasks,
                            TaskDetails = taskDetails
                        };
                    ViewBag.username = HttpContext.Session.GetString("Username");
                    return View(viewModel);
                }
                 
            }

            return RedirectToAction("Login");
        }
        [HttpPost]
        public ActionResult deleteTask(int taskId)
        {
            try
            {
                var assigned=_context.SAssignedTasks.FirstOrDefault(i => i.TaskId == taskId);
                 
                if (assigned != null) {
                    var task = _context.STaskDetails.FirstOrDefault(t => t.Id == assigned.TaskId); 
                    _context.SAssignedTasks.Remove(assigned);
                    _context.SaveChanges();
                    if (task != null)
                    {
                        _context.STaskDetails.Remove(task);
                        var fileName = task.Contents; 

                        _context.SaveChanges();

                        var uploadsDirectory = Path.Combine(_hostingEnvironment.WebRootPath, "TaskFiles");
                        var filePath = Path.Combine(uploadsDirectory, fileName);

                        if (System.IO.File.Exists(filePath))
                        {
                            System.IO.File.Delete(filePath);
                        }
                        return Json(new { success = true });
                    }
                    else
                    {
                        return Json(new { success = false });
                    }
                     
                    
                }
                else
                {
                    return Json(new { success = false, error = "Task not found" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }





    }
}
