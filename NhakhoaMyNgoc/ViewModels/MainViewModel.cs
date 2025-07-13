using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NhakhoaMyNgoc.ViewModels
{
    public class MainViewModel
    {
        public AppViewModel AppVM { get; } = new();
        public CustomerViewModel CustomerVM { get; } = new();
        public InvoiceViewModel InvoiceVM { get; } = new();
    }
}
