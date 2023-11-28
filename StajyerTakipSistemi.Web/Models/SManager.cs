using System;
using System.Collections.Generic;

namespace StajyerTakipSistemi.Web.Models;

public partial class SManager
{
    public int Id { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Username { get; set; }

    public string? Password { get; set; }
    public Guid? Guid { get; set; }

    public virtual ICollection<SInternToManager> SInternToManagers { get; set; } = new List<SInternToManager>();

    public virtual ICollection<SMessagesforintern> SMessagesforinterns { get; set; } = new List<SMessagesforintern>();

    public virtual ICollection<SMessagesformanager> SMessagesformanagers { get; set; } = new List<SMessagesformanager>();
}
