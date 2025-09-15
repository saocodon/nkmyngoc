using System;
using System.Collections.Generic;

namespace NhakhoaMyNgoc.Models;

public partial class Expense
{
    public long Id { get; set; }

    public bool Deleted { get; set; }

    public DateTime Date { get; set; }

    public bool Input { get; set; }

    public string Participant { get; set; } = null!;

    public string? Address { get; set; }

    public string? Content { get; set; }

    public long Amount { get; set; }

    public string? CertificateId { get; set; }
}
