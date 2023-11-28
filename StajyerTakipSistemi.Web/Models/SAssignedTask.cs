using System;
using System.Collections.Generic;

namespace StajyerTakipSistemi.Web.Models;

public partial class SAssignedTask
{
    public int Id { get; set; }

    public int? InternId { get; set; }

    public int? TaskId { get; set; }

    public DateTime? AssignmentDate { get; set; }

    public DateTime? DueDate { get; set; }

    public virtual SIntern? Intern { get; set; }

    public virtual STaskDetail? Task { get; set; }
}
