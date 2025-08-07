using System;
using System.Collections.Generic;

namespace NhakhoaMyNgoc.Models;

public partial class Idnitem
{
    public int Id { get; set; }

    public int IdnId { get; set; }

    public int ItemId { get; set; }

    public int Quantity { get; set; }

    public int Demand { get; set; }

    public int Price { get; set; }

    public virtual Idn Idn { get; set; } = null!;

    public virtual Product Item { get; set; } = null!;
}
