using System;
using System.Collections.Generic;
using System.Text;

namespace NhakhoaMyNgoc_Connector.DTOs
{
    public class IdnDto
    {
        public int Input { get; set; }

        public DateTime Date { get; set; }

        public int CertificateId { get; set; }

        public string Correspondent { get; set; } = null;

        public string Division { get; set; }

        public string Reason { get; set; }

        public int Total { get; set; }
    }
}
