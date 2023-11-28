using System;
using System.Collections.Generic;

namespace StajyerTakipSistemi.Web.Models
{
    public class GroupedReportModel
    {
        public DateTime Date { get; set; }
        public List<SDailyReport> Reports { get; set; }
    }
}
