using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NhakhoaMyNgoc.Models;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace NhakhoaMyNgoc.ViewModels
{
    public partial class InvoiceViewModel : ObservableObject
    {
        private readonly DataContext _db;
        private readonly InvoiceItemViewModel invoiceItemVM;

        public InvoiceViewModel(InvoiceItemViewModel iivm)
        {
            _db = new DataContext();
            invoiceItemVM = iivm;

            // load tất cả dịch vụ
            Services = new ObservableCollection<Service>(_db.Services.ToList());

            // khởi tạo ít nhất 1 hàng trong bảng chi tiết
            InvoiceItems.Add(new InvoiceItem());
        }

        #region global
        [ObservableProperty]
        private Invoice selectedInvoice = new();

        // gọi đệ quy xuống thuộc tính con (OnPropertyChanged)
        public bool IsRevisitValid =>
            SelectedInvoice is { } inv &&
            inv.Date != default &&
            inv.Revisit != default &&
            inv.Date.AddYears(1) >= inv.Revisit;

        [ObservableProperty]
        private ObservableCollection<InvoiceItem> invoiceItems = new();

        [ObservableProperty]
        private ObservableCollection<Service> services;

        [ObservableProperty]
        private ObservableCollection<Invoice> invoices = new();
        #endregion

        /// <summary>
        /// Bind hoá đơn
        /// </summary>
        /// <param name="value"></param>
        partial void OnSelectedInvoiceChanged(Invoice value)
        {
            if (value is null) return;

            var result = _db.InvoiceItems
                            .Where(i => i.InvoiceId == value.Id)
                            .ToList();

            InvoiceItems = new ObservableCollection<InvoiceItem>(result);

            // thông báo là thuộc tính con của SelectedInvoice thay đổi
            OnPropertyChanged(nameof(IsRevisitValid));
        }

        #region Add & edit
        [RelayCommand]
        void StartAddNew(Customer current) => SelectedInvoice = new() { CustomerId = current.Id };

        [RelayCommand]
        void SaveInvoice()
        {
            // Một field NOT NULL = NULL thì return ngay.
            if (SelectedInvoice.CustomerId == 0) return;

            if (SelectedInvoice.Id == 0) // hoá đơn mới
            {
                _db.Invoices.Add(SelectedInvoice);
                Invoices.Add(SelectedInvoice);

                // TODO: thêm các chi tiết hoá đơn nữa
                // (code behind)
                foreach (var item in InvoiceItems)
                {

                }
            }
            else // hoá đơn cũ
            {
                _db.Invoices.Update(SelectedInvoice);
            }

            _db.SaveChanges();
            SelectedInvoice = new(); // reset sau khi lưu
        }
        #endregion

        [RelayCommand]
        void DeleteInvoice()
        {
            SelectedInvoice.Deleted = 1;
            _db.SaveChanges();

            Invoices.Remove(SelectedInvoice);
            SelectedInvoice = new();
        }

        public void FindCustomersInvoices(Customer customer)
        {
            var result = (from i in _db.Invoices
                          where i.CustomerId == customer.Id
                          select i).ToList();
            Invoices = new ObservableCollection<Invoice>(result);
        }
    }
}
