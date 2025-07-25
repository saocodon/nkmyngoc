using System;

namespace NhakhoaMyNgoc_Connector.DTOs
{
    public class CustomerDto
    {
        public int Id { get; set; }

        public int Deleted { get; set; }

        // nullable
        public string Cid { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Sex { get; set; }

        // nullable
        public DateTime Birthdate { get; set; }

        // nullable
        public string Address { get; set; }

        // nullable
        public string Phone { get; set; }
    }
}
