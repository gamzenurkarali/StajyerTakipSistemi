using System;
using System.Collections.Generic;

namespace StajyerTakipSistemi.Web.Models
{
    public class NewMessages
    {
        public int Id { get; set; }
        public Guid Guid { get; set; }
        public Guid Sender { get; set; }
        public Guid Receiver { get; set; } 
        public long? UnixTime { get; set; }
    }
}
