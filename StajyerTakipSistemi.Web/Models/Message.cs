using System;
using System.Collections.Generic;

namespace StajyerTakipSistemi.Web.Models
{
    public partial class Message
    {
        public int MessageId { get; set; }
        public Guid Sender { get; set; }
        public Guid Receiver { get; set; }
        public string? MessageText { get; set; }
        public long? UnixTime { get; set; }
    }
}
