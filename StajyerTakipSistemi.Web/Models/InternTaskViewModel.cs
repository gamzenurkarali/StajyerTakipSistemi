using System;
using System.Collections.Generic;

namespace StajyerTakipSistemi.Web.Models
{
    public class InternTaskViewModel
    {
        public SIntern Intern { get; set; }
        public List<SAssignedTask> AssignedTasks { get; set; }
        public List<STaskDetail> TaskDetails { get; set; }  

    }
}
