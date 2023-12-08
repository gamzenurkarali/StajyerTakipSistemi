using System;
using System.Collections.Generic;

namespace StajyerTakipSistemi.Web.Models;

public partial class SApplication
{
    public int Id { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public DateTime? BirthDate { get; set; }

    public string? Address { get; set; }

    public string? DesiredField { get; set; }

    public string? Explanation { get; set; }

    public string? ApprovalStatus { get; set; }
    public string? Cv { get; set; }

    public DateTime? ApplicationDate { get; set; }
}
