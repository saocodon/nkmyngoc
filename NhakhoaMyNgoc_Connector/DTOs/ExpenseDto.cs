using System;
using System.Collections.Generic;
using System.Text;

namespace NhakhoaMyNgoc_Connector.DTOs
{
    public class ExpenseDto
    {
        public long Id { get; set; }

        public DateTime Date { get; set; }

        public bool Input { get; set; }

        public string Participant { get; set; }

        public string Address { get; set; }

        public string Content { get; set; }

        public long Amount { get; set; }
        public string AmountInWords { get; set; }

        public string CertificateId { get; set; }
    }
}
