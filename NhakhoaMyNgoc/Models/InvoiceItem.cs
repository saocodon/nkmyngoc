using System;
using System.Collections.Generic;

namespace NhakhoaMyNgoc.Models;

public partial class InvoiceItem
{
    public int Id { get; set; }

    public int Deleted { get; set; }

    public int InvoiceId { get; set; }

    public int ServiceId { get; set; }

    public int Quantity { get; set; }

    public int Price { get; set; }

    public int Discount { get; set; }

    public virtual Invoice Invoice { get; set; } = null!;

    public virtual Service Service { get; set; } = null!;
}
