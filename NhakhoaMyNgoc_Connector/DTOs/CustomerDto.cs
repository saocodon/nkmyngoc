using System;

namespace NhakhoaMyNgoc_Connector.DTOs
{
    public class CustomerDto
    {
        public long Id { get; set; }

        public bool Deleted { get; set; }

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

        public CustomerDto(long id, bool deleted, string cid, string name, string sex, DateTime birthdate, string address, string phone)
        {
            Id = id;
            Deleted = deleted;
            Cid = cid;
            Name = name;
            Sex = sex;
            Birthdate = birthdate;
            Address = address;
            Phone = phone;
        }
    }
}
