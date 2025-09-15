using System;
using System.Collections.Generic;
using System.Text;

namespace NhakhoaMyNgoc_Connector.DTOs
{
    public class SummaryServiceDto
    {
        public DateTime Date { get; set; }

        public string ServiceName { get; set; }
        public int Quantity { get; set; }
        public long Price { get; set; }
        public long Discount { get; set; }
        public long Total { get; set; }
    }
}
