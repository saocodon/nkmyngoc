using System;
using System.Collections.Generic;

namespace NhakhoaMyNgoc.Models;

public partial class Customer
{
    public int Id { get; set; }

    public int Deleted { get; set; }

    public string? Cid { get; set; }

    public string Name { get; set; } = null!;

    public int Sex { get; set; }

    public DateTime? Birthdate { get; set; }

    public string? Address { get; set; }

    public string? Phone { get; set; }

    public virtual ICollection<Image> Images { get; set; } = new List<Image>();

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}
