using System;
using System.Collections.Generic;

namespace NhakhoaMyNgoc.Models;

public partial class Service
{
    public long Id { get; set; }

    public bool Deleted { get; set; }

    public string Name { get; set; } = null!;

    public long Price { get; set; }

    public virtual ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();
}
