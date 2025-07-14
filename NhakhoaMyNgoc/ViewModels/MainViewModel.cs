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

        public MainViewModel()
        {
            InvoiceItemVM = new InvoiceItemViewModel();
            InvoiceVM = new InvoiceViewModel(InvoiceItemVM);
            CustomerVM = new CustomerViewModel(InvoiceVM);
        }
    }
}
