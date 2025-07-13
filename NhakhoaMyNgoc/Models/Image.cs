using System;
using System.Collections.Generic;

namespace NhakhoaMyNgoc.Models;

public partial class Image
{
    public int Id { get; set; }

    public int Deleted { get; set; }

    public int CustomerId { get; set; }

    public string Path { get; set; } = null!;

    public string? Note { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Customer Customer { get; set; } = null!;
}
