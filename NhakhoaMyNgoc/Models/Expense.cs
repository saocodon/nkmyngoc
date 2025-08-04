using System;
using System.Collections.Generic;

namespace NhakhoaMyNgoc.Models;

public partial class Expense
{
    public int Id { get; set; }

    public int Deleted { get; set; }

    public DateTime Date { get; set; }

    public int Input { get; set; }

    public string? Participant { get; set; }

    public string? Address { get; set; }

    public string? Content { get; set; }

    public int Amount { get; set; }

    public int CertificateId { get; set; }
}
