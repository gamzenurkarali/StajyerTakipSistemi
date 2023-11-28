using System;
using System.Collections.Generic;

namespace StajyerTakipSistemi.Web.Models;

public partial class SInternToManager
{
    public int Id { get; set; }

    public int? InternId { get; set; }

    public int? ManagerId { get; set; }

    public virtual SIntern? Intern { get; set; }

    public virtual SManager? Manager { get; set; }
}
