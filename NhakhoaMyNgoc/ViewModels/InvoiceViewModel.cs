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

        public InvoiceViewModel(DataContext db)
        {
            _db = db;

            // load tất cả dịch vụ
            Services = new ObservableCollection<Service>(_db.Services.ToList());

            // khởi tạo ít nhất 1 hàng trong bảng chi tiết
            InvoiceItems.Add(new InvoiceItem());

            // đăng ký nhận khách hàng đang chọn
            Messenger.Subscribe("CustomerSelected", data =>
            {
                if (data is object[] obj &&
                    obj.Length == 1 &&
                    obj[0] is Customer customer)
                    FindCustomersInvoices(customer);
            });
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
        private ObservableCollection<InvoiceItem> invoiceItems = [];

        [ObservableProperty]
        private ObservableCollection<Service> services;

        [ObservableProperty]
        private ObservableCollection<Invoice> invoices = [];
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

                // thêm từng dịch vụ
                foreach (var item in InvoiceItems)
                {
                    // đặt mã hoá đơn
                    // vì chưa gọi AcceptChanges() nên phải +1
                    item.InvoiceId = _db.Invoices.Count() + 1;

                    _db.InvoiceItems.Add(item);
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
                          where i.CustomerId == customer.Id && i.Deleted == 0
                          select i).ToList();
            Invoices = new ObservableCollection<Invoice>(result);
        }
    }
}
