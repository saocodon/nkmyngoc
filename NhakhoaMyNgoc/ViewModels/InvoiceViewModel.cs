using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NhakhoaMyNgoc.Converters;
using NhakhoaMyNgoc.Models;
using NhakhoaMyNgoc.Utilities;
using NhakhoaMyNgoc_Connector.DTOs;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace NhakhoaMyNgoc.ViewModels
{
    public partial class InvoiceViewModel : ObservableObject
    {
        private readonly DataContext _db;

        private Customer SelectedCustomer = new();

        public InvoiceViewModel(DataContext db)
        {
            _db = db;

            // load tất cả dịch vụ
            Services = new ObservableCollection<Service>([.. _db.Services]);

            // khởi tạo ít nhất 1 hàng trong bảng chi tiết
            InvoiceItems.Add(new InvoiceItem());

            // đăng ký nhận khách hàng đang chọn
            Messenger.Subscribe("OnSelectedCustomerChanged", data =>
            {
                if (data is object[] obj &&
                    obj.Length == 1 &&
                    obj[0] is Customer customer)
                {
                    SelectedCustomer = customer;
                    FindCustomersInvoices(customer);
                }
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

        [RelayCommand]
        void Print()
        {
            // Data Transfer Objects (DTO)
            var customer = new CustomerDto
            {
                Id = SelectedCustomer.Id,
                Deleted = SelectedCustomer.Deleted,
                Cid = SelectedCustomer.Cid,
                Name = SelectedCustomer.Name,
                Birthdate = SelectedCustomer.Birthdate ?? DateTime.UnixEpoch,
                Address = SelectedCustomer.Address,
                Phone = SelectedCustomer.Phone,
                Sex = SelectedCustomer.Sex switch
                {
                    0 => "Nam",
                    1 => "Nữ",
                    _ => "Khác",
                }
            };

            var invoice = new InvoiceDto
            {
                Date = SelectedInvoice.Date,
                Total = SelectedInvoice.Total,
                Revisit = SelectedInvoice.Revisit ?? DateTime.UnixEpoch,
                Note = SelectedInvoice.Note +
                       (IsRevisitValid ? $" (Tái khám ngày {SelectedInvoice.Revisit:dd/MM/yyyy})" : "")
            };

            // Tìm lịch sử
            var invoices = _db.Invoices.Include(i => i.InvoiceItems)
                                       .ThenInclude(ii => ii.Service)
                                       .Where(i => i.Id == SelectedInvoice.Id).ToList();
            List<SummaryServiceDto> services = [];
            foreach (var item in invoices[0].InvoiceItems)
            {
                // line in timeline
                var line = new SummaryServiceDto()
                {
                    ServiceName = item.Service.Name,
                    Quantity = item.Quantity,
                    Price = item.Price,
                    Discount = item.Discount,
                    Total = SelectedInvoice.Total
                };
                services.Add(line);
            }

            var customerFilePath = IOUtil.WriteJsonToTempFile(customer, $"Customer{customer.Id}.json");
            var invoiceFilePath = IOUtil.WriteJsonToTempFile(invoice, $"Invoice{customer.Id}.json");
            var servicesFilePath = IOUtil.WriteJsonToTempFile(services, $"Services{SelectedInvoice.Id}.json");

            // TODO: cái này phải thay đổi khi đóng gói
            Process.Start(new ProcessStartInfo()
            {
                FileName = @"..\..\..\..\NhakhoaMyNgoc_RDLC\bin\Debug\NhakhoaMyNgoc_RDLC.exe",
                Arguments = $"--report invoice --customer {customerFilePath} --invoice {invoiceFilePath} --services {servicesFilePath}"
            });
        }

        public void FindCustomersInvoices(Customer customer)
        {
            var result = (from i in _db.Invoices
                          where i.CustomerId == customer.Id && i.Deleted == 0
                          select i).ToList();
            Invoices = new ObservableCollection<Invoice>(result);
            SelectedInvoice = Invoices.FirstOrDefault() ?? new();
        }
    }
}
