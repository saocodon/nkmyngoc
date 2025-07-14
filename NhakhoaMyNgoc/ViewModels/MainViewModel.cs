using NhakhoaMyNgoc.Models;
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
        public CustomerViewModel CustomerVM { get; }
        public InvoiceViewModel InvoiceVM { get; }
        public InvoiceItemViewModel InvoiceItemVM { get; }

        public MainViewModel(DataContext db)
        {
            InvoiceItemVM = new InvoiceItemViewModel(db);
            InvoiceVM = new InvoiceViewModel(db, InvoiceItemVM);
            CustomerVM = new CustomerViewModel(db);
        }
    }
}
