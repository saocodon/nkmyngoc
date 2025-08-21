using System;
using System.Collections.Generic;
using System.Text;

namespace NhakhoaMyNgoc_Connector.DTOs
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
        public int StartQuantity { get; set; }
        public int Input { get; set; }
        public int Output { get; set; }
        public int EndQuantity { get; set; }
    }
}
