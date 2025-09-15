using System;
using System.Collections.Generic;

namespace NhakhoaMyNgoc.Models;

public partial class Invoice
{
    public long Id { get; set; }

    public bool Deleted { get; set; }

    public long CustomerId { get; set; }

    public DateTime Date { get; set; }

    public long Remaining { get; set; }

    public DateOnly? Revisit { get; set; }

    public string? Note { get; set; }

    public virtual Customer Customer { get; set; } = null!;

    public virtual ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();
}
