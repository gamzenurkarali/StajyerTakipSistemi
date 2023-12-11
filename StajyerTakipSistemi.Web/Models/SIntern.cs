using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace StajyerTakipSistemi.Web.Models;

public partial class SIntern
{
    public int Id { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    [Required(ErrorMessage = "Doğum Tarihi zorunludur.")]
    [DataType(DataType.Date)]
    public DateTime BirthDate { get; set; }
    //public DateTime? BirthDate { get; set; }

    public string? Address { get; set; }

    public string? DesiredField { get; set; }

    public string? Explanation { get; set; }

    public string? AccessStatus { get; set; }

    [DataType(DataType.Date)]
    public DateTime StartDate { get; set; }

    [DataType(DataType.Date)]
    public DateTime EndDate { get; set; }

    public string? Username { get; set; }

    public string? Password { get; set; }
    public string? Photo { get; set; }

    public Guid? Guid { get; set; }

    public virtual ICollection<SAbsenceInformation> SAbsenceInformations { get; set; } = new List<SAbsenceInformation>();

    public virtual ICollection<SAssignedTask> SAssignedTasks { get; set; } = new List<SAssignedTask>();

    public virtual ICollection<SDailyReport> SDailyReports { get; set; } = new List<SDailyReport>();

    public virtual SInternToManager? SInternToManager { get; set; }

    public virtual ICollection<SMessagesforintern> SMessagesforinterns { get; set; } = new List<SMessagesforintern>();

    public virtual ICollection<SMessagesformanager> SMessagesformanagers { get; set; } = new List<SMessagesformanager>();
}
