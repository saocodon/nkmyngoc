using System;
using System.Collections.Generic;
using System.Text;

namespace NhakhoaMyNgoc_Connector.DTOs
{
    public class StockTransactionDto
    {
        public DateTime Date { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public bool IsInput { get; set; }   // true = nhập, false = xuất
        public int Quantity { get; set; }
        public int Price { get; set; }  // đơn giá của từng dòng
    }
}
