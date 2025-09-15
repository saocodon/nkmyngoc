using System;
using System.Collections.Generic;

namespace NhakhoaMyNgoc.Models;

public partial class Idnitem
{
    public long Id { get; set; }

    public long IdnId { get; set; }

    public long ItemId { get; set; }

    public int Quantity { get; set; }

    public int Demand { get; set; }

    public long Price { get; set; }

    public virtual Idn Idn { get; set; } = null!;

    public virtual Product Item { get; set; } = null!;
}
