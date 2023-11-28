using System;
using System.Collections.Generic;

namespace StajyerTakipSistemi.Web.Models;

public partial class STaskDetail
{
    public int Id { get; set; }

    public string? Contents { get; set; }

    public string? Subject { get; set; }

    public virtual ICollection<SAssignedTask> SAssignedTasks { get; set; } = new List<SAssignedTask>();
}
