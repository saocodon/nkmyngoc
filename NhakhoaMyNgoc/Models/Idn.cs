using System;
using System.Collections.Generic;

namespace NhakhoaMyNgoc.Models;

public partial class Idn
{
    public int Id { get; set; }

    public int Deleted { get; set; }

    public int Input { get; set; }

    public DateTime Date { get; set; }

    public int CertificateId { get; set; }

    public string Correspondent { get; set; } = null!;

    public string? Division { get; set; }

    public string? Reason { get; set; }

    public int Total { get; set; }

    public virtual ICollection<Idnitem> Idnitems { get; set; } = new List<Idnitem>();
}
