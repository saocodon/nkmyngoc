using CommunityToolkit.Mvvm.ComponentModel;
using NhakhoaMyNgoc.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NhakhoaMyNgoc.ViewModels
{
    public partial class InvoiceItemViewModel : ObservableObject
    {
        private readonly DataContext _db;
        public InvoiceItemViewModel(DataContext db)
        {
            _db = db;

            Messenger.Subscribe("AddInvoiceItem", data => {
                if (data != null)
                {
                    var items = data as ObservableCollection<InvoiceItem>;
                    AddInvoiceItem(items!);
                }
            });
        }

        [ObservableProperty]
        private InvoiceItem selectedInvoiceItem = new();

        public void AddInvoiceItem(ObservableCollection<InvoiceItem> items)
        {
            foreach (var item in items)
                _db.InvoiceItems.Add(item);
        }
    }
}
