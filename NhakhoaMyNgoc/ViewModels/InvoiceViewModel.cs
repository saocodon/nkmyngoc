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

            // load tất cả dịch vụ
            Services = new ObservableCollection<Service>(_db.Services.ToList());
        }

        [ObservableProperty]
        private Invoice selectedInvoice = new();

        [ObservableProperty]
        private ObservableCollection<InvoiceItem> invoiceItems = new();

        [ObservableProperty]
        private ObservableCollection<Service> services;

        partial void OnSelectedInvoiceChanged(Invoice value)
        {
            if (value is null) return;

            var result = _db.InvoiceItems
                            .Where(i => i.InvoiceId == value.Id)
                            .ToList();

            InvoiceItems = new ObservableCollection<InvoiceItem>(result);
        }
    }
}
