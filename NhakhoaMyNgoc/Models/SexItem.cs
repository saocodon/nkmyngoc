using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NhakhoaMyNgoc.Models
{
    public class SexItem
    {
        public int Value { get; set; }
        public string Display { get; set; }
        public override string ToString() => Display;
    }
}
