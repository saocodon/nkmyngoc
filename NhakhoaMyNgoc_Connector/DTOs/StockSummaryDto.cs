using System;
using System.Collections.Generic;
using System.Text;

namespace NhakhoaMyNgoc_Connector.DTOs
{
    public class StockSummaryDto
    {
        public bool Input { get; set; }
        public string ProductName { get; set; }
        public long ProductId { get; set; }
        public string Unit { get; set; }

        public int BeginningQty { get; set; }
        public long BeginningValue { get; set; }

        public int InQty { get; set; }
        public long InValue { get; set; }

        public int OutQty { get; set; }
        public long OutValue { get; set; }
        public int EndQty { get; set; }
        public long EndValue { get; set; }
    }
}
