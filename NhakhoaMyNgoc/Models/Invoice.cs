using System;
using System.Collections.Generic;

namespace NhakhoaMyNgoc.Models;

public partial class Invoice
{
    public int Id { get; set; }

    public int Deleted { get; set; }

    public int CustomerId { get; set; }

    public DateTime Date { get; set; }

    public int Total { get; set; }

    public int Remaining { get; set; }

    public DateTime? Revisit { get; set; }

    public string? Note { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
