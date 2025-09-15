using System;
using System.Collections.Generic;

namespace NhakhoaMyNgoc.Models;

public partial class Image
{
    public long Id { get; set; }

    public bool Deleted { get; set; }

    public long CustomerId { get; set; }

    public string Path { get; set; } = null!;

    public string? Note { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Customer Customer { get; set; } = null!;
}
