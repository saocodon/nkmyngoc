using CommunityToolkit.Mvvm.ComponentModel;
using NhakhoaMyNgoc.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NhakhoaMyNgoc.ViewModels
{
    public partial class InvoiceItemViewModel : ObservableObject
    {
        private readonly DataContext _db;
        public InvoiceItemViewModel()
        {
            _db = new DataContext();
        }

        [ObservableProperty]
        private InvoiceItem selectedInvoiceItem = new();

        partial void OnSelectedInvoiceItemChanged(InvoiceItem value)
        {
            Debug.WriteLine(value.ServiceId);
        }
    }
}
