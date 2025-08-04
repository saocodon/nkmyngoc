using System;
using System.Collections.Generic;
using System.Text;

namespace NhakhoaMyNgoc_Connector.DTOs
{
    public class ExpenseDto
    {
        public int Id { get; set; }

        public DateTime Date { get; set; }

        public int Input { get; set; }

        public string Participant { get; set; }

        public string Address { get; set; }

        public string Content { get; set; }

        public int Amount { get; set; }
        public string AmountInWords { get; set; }

        public int CertificateId { get; set; }
    }
}
