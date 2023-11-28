using System;
using System.Collections.Generic;

namespace StajyerTakipSistemi.Web.Models;

public partial class SDailyReport
{
    public int Id { get; set; }
    public int? InternId { get; set; }
    public string FileName { get; set; }
    public string ContentType { get; set; }
    public byte[] Data { get; set; }
    public long UnixTime { get; set; }
    public Guid internGuid { get; set; }
    public string FilePath { get; set; }


    // Yabancı anahtar ilişkisi için S_interns tablosuna bağlantı
    public SIntern Intern { get; set; }
}
