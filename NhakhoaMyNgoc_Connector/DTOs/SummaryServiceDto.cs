﻿using System;
using System.Collections.Generic;
using System.Text;

namespace NhakhoaMyNgoc_Connector.DTOs
{
    public class SummaryServiceDto
    {
        public DateTime Date { get; set; }

        public string ServiceName { get; set; }
        public int Quantity { get; set; }
        public int Price { get; set; }
        public int Discount { get; set; }
        public int Total { get; set; }
    }
}
