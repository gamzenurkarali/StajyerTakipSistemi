using System;
using System.Collections.Generic;

namespace StajyerTakipSistemi.Web.Models;

public partial class SAbsenceInformation
{
    public int Id { get; set; }

    public int? InternId { get; set; }

    public DateTime? AbsenceDate { get; set; }

    public virtual SIntern? Intern { get; set; }
}
