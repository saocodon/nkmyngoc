using System;
using System.Collections.Generic;

namespace NhakhoaMyNgoc.Models;

public partial class Idn
{
    public long Id { get; set; }

    public bool Deleted { get; set; }

    public bool Input { get; set; }

    public DateTime Date { get; set; }

    public string? CertificateId { get; set; }

    public string? Correspondent { get; set; }

    public string? Division { get; set; }

    public string? Reason { get; set; }

    public virtual ICollection<Idnitem> Idnitems { get; set; } = new List<Idnitem>();
}
