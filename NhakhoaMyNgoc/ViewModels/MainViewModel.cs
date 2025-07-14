using NhakhoaMyNgoc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NhakhoaMyNgoc.ViewModels
{
    public class MainViewModel(DataContext db)
    {
        public AppViewModel AppVM { get; } = new();
        public CustomerViewModel CustomerVM { get; } = new CustomerViewModel(db);
        public InvoiceViewModel InvoiceVM { get; } = new InvoiceViewModel(db);
        public InvoiceItemViewModel InvoiceItemVM { get; } = new InvoiceItemViewModel(db);
    }
}
