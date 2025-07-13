using System;
using System.Collections.Generic;

namespace NhakhoaMyNgoc.Models;

public partial class Payment
{
    public int Id { get; set; }

    public int Deleted { get; set; }

    public int InvoiceId { get; set; }

    public string Date { get; set; } = null!;

    public int Amount { get; set; }

    public int PaymentMethod { get; set; }

    public string? Note { get; set; }

    public virtual Invoice Invoice { get; set; } = null!;
}
