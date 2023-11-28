using System;
using System.Collections.Generic;

namespace StajyerTakipSistemi.Web.Models;

public partial class SMessagesformanager
{
    public int Id { get; set; }

    public int? SenderId { get; set; }

    public int? ReceiverId { get; set; }

    public string? Text { get; set; }

    public long? UnixTimestamp { get; set; }

    public virtual SIntern? Receiver { get; set; }

    public virtual SManager? Sender { get; set; }
}
