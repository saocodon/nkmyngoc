using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NhakhoaMyNgoc.Models;
using System.Collections.ObjectModel;

namespace NhakhoaMyNgoc.ViewModels
{
    public partial class InvoiceViewModel : ObservableObject
    {
        private readonly DataContext _db;

        public InvoiceViewModel()
        {
            _db = new DataContext();
        }

        [ObservableProperty]
        private Invoice selectedInvoice = new();

        [ObservableProperty]
        private ObservableCollection<Invoice> invoiceItems = new();

        partial void OnSelectedInvoiceChanged(Invoice value)
        {
            if (value is null) return;

            var result = _db.Invoices
                            .Where(i => i.CustomerId == value.Id)
                            .ToList();

            InvoiceItems = new ObservableCollection<Invoice>(result);
        }
    }
}
