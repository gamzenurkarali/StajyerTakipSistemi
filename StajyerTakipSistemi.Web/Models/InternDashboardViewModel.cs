namespace StajyerTakipSistemi.Web.Models
{
    public class InternDashboardViewModel
    {
        public SIntern Intern { get; set; }
        public List<SAssignedTask> AssignedTasks { get; set; }
        public List<STaskDetail> TaskDetails { get; set; }  
        public List<SDailyReport> Reports { get; set; }  

        public List<SAbsenceInformation> AbsenceInfo { get; set; }

        public List<SAssignedTask> AssignedOverdueTask { get; set; }
        public List<SAssignedTask> AssignedActiveTask { get; set; }
        public List<STaskDetail> OverdueTaskDetails { get; set; }
        public List<STaskDetail> ActiveTaskDetails { get; set; }
        
        public SDailyReport ExistingReportForToday { get; set; }
        public bool HasReportForToday { get; set; }
    }
}
