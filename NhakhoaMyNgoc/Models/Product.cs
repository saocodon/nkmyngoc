using System;
using System.Collections.Generic;

namespace NhakhoaMyNgoc.Models;

public partial class Product
{
    public long Id { get; set; }

    public bool Deleted { get; set; }

    public string Name { get; set; } = null!;

    public string? Unit { get; set; }

    public long Price { get; set; }

    public int Quantity { get; set; }

    public long Total { get; set; }

    public virtual ICollection<Idnitem> Idnitems { get; set; } = new List<Idnitem>();
}
