namespace StajyerTakipSistemi.Web.Models
{
    public class ManagerDashboardViewModel
    {
        public List<SIntern> InternList { get; set; }

        public Dictionary<int, int> OverdueTaskCounts { get; set; }
        public Dictionary<int, int> ActiveTaskCounts { get; set; }
    }
}
