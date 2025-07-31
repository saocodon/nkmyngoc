using System;
using System.Collections.Generic;
using System.Text;

namespace NhakhoaMyNgoc_Connector.DTOs
{
    public class IdnItemDto
    {
        public int IdnId { get; set; }

        public string ProductName { get; set; }
        public int ProductId { get; set; }
        public string Unit { get; set; }

        public int Quantity { get; set; }

        public int Demand { get; set; }

        public int Price { get; set; }
        public int Total { get; set; }
    }
}
