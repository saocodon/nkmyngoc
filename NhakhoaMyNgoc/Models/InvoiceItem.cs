using System;
using System.Collections.Generic;

namespace NhakhoaMyNgoc.Models;

public partial class InvoiceItem
{
    public long Id { get; set; }

    public bool Deleted { get; set; }

    public long InvoiceId { get; set; }

    public long ServiceId { get; set; }

    public int Quantity { get; set; }

    public long Price { get; set; }

    public long Discount { get; set; }

    public virtual Invoice Invoice { get; set; } = null!;

    public virtual Service Service { get; set; } = null!;
}
