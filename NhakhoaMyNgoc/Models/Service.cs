using System;
using System.Collections.Generic;

namespace NhakhoaMyNgoc.Models;

public partial class Service
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int Price { get; set; }

    public virtual ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();
}
