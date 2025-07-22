using System;
using System.Collections.Generic;

namespace NhakhoaMyNgoc.Models;

public partial class Product
{
    public int Id { get; set; }

    public int Deleted { get; set; }

    public string Name { get; set; } = null!;

    public string? Unit { get; set; }

    public int Quantity { get; set; }

    public int Total { get; set; }

    public virtual ICollection<Idnitem> Idnitems { get; set; } = new List<Idnitem>();
}
