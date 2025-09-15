using System;
using System.Collections.Generic;
using System.Text;

namespace NhakhoaMyNgoc_Connector.DTOs
{
    public class InvoiceDto
    {
        public DateTime Date { get; set; }
        public DateTime Revisit { get; set; }
        public long Total { get; set; }
        public string Note { get; set; }
    }
}
