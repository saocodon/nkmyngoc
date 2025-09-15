using System;
using System.Collections.Generic;

namespace NhakhoaMyNgoc.Models;

public partial class Customer
{
    public long Id { get; set; }

    public bool Deleted { get; set; }

    public string? Cid { get; set; }

    public string? Name { get; set; }

    public short? Sex { get; set; }

    public DateOnly? Birthdate { get; set; }

    public string? Address { get; set; }

    public string? Phone { get; set; }

    public virtual ICollection<Image> Images { get; set; } = new List<Image>();

    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
}
