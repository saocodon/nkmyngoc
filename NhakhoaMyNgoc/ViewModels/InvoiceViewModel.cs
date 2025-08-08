using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NhakhoaMyNgoc.Converters;
using NhakhoaMyNgoc.Models;
using NhakhoaMyNgoc.ModelWrappers;
using NhakhoaMyNgoc.Utilities;
using NhakhoaMyNgoc_Connector.DTOs;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;

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
            Services = [.. _db.Services];

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

            // load ngày tháng hiện tại cho datepicker
            SelectedInvoice.Date = DateTime.Now;
            SelectedInvoice.Revisit = DateTime.Now;
        }

        private void InvoiceItems_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems is not null)
            {
                foreach (InvoiceItemWrapper item in e.NewItems)
                    item.PropertyChanged += InvoiceItem_PropertyChanged;
            }

            if (e.OldItems is not null)
            {
                foreach (InvoiceItemWrapper item in e.OldItems)
                    item.PropertyChanged -= InvoiceItem_PropertyChanged;
            }

            OnPropertyChanged(nameof(Total));
        }

        private void InvoiceItem_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is nameof(InvoiceItemWrapper.Quantity)
                or nameof(InvoiceItemWrapper.Price)
                or nameof(InvoiceItemWrapper.Discount)
                or nameof(InvoiceItemWrapper.Total))
            {
                OnPropertyChanged(nameof(Total));
            }
        }

        #region global
        [ObservableProperty]
        private Invoice selectedInvoice = new();

        [ObservableProperty]
        private InvoiceItemWrapper? selectedInvoiceItem;

        // gọi đệ quy xuống thuộc tính con (OnPropertyChanged)
        public bool IsRevisitValid =>
            SelectedInvoice is { } inv &&
            inv.Date != default &&
            inv.Revisit != default &&
            inv.Date.AddYears(1) >= inv.Revisit;

        // đối với các thuộc tính như thế này
        // không nên sử dụng của model mà nên tự tính riêng.
        public int Total => InvoiceItems?.Sum(i => i.Quantity * i.Price - i.Discount) ?? 0;

        [ObservableProperty]
        private ObservableCollection<InvoiceItemWrapper> invoiceItems = [];

        public List<Service> Services { get; }

        [ObservableProperty]
        private ObservableCollection<Invoice> invoices = [];

        public static string Title => "Hoá đơn";

        public bool IsReadOnly { get; set; } = false;
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

            var wrapped = result.Select(i =>
            {
                var wrapper = new InvoiceItemWrapper(i) { Services = this.Services };
                wrapper.PropertyChanged += (_, _) => OnPropertyChanged(nameof(Total));
                return wrapper;
            });

            InvoiceItems.Clear();
            foreach (var item in wrapped)
            {
                item.PropertyChanged += (_, _) => OnPropertyChanged(nameof(Total));
                InvoiceItems.Add(item);
            }
            InvoiceItems.CollectionChanged += InvoiceItems_CollectionChanged;

            OnPropertyChanged(nameof(IsRevisitValid));
            OnPropertyChanged(nameof(Total));
        }

        #region Add & edit
        [RelayCommand]
        void StartAddNew(Customer current) => SelectedInvoice = new()
        {
            CustomerId = current.Id,
            Date = DateTime.Now
        };

        [RelayCommand]
        void SaveInvoice()
        {
            try
            {
                if (SelectedInvoice.Id == 0) // hoá đơn mới
                {
                    _db.Invoices.Add(SelectedInvoice);
                    _db.SaveChanges();

                    Invoices.Add(SelectedInvoice);
                }
                else // hoá đơn cũ
                {
                    _db.Invoices.Update(SelectedInvoice);
                }

                foreach (var item in InvoiceItems)
                {
                    item.InvoiceId = SelectedInvoice.Id;

                    if (item.Id == 0)
                        _db.InvoiceItems.Add(item.Model);
                }

                _db.SaveChanges();
            }
            catch
            {
                MessageBox.Show("Có một số trường dữ liệu bị rỗng. Kiểm tra lại và thử lại.");
            }
        }
        #endregion

        #region Delete
        [RelayCommand]
        void DeleteInvoice()
        {
            SelectedInvoice.Deleted = 1;
            _db.SaveChanges();

            Invoices.Remove(SelectedInvoice);
            SelectedInvoice = new() { Date = DateTime.Now };
        }

        [RelayCommand]
        void DeleteInvoiceItem()
        {
            if (SelectedInvoiceItem == null) return;
            // phải xoá trong DB trước rồi mới xoá trên Model
            // vì sau khi xoá trong Model trước thì nó sẽ bị null
            // (không thể sử dụng tiếp).
            _db.InvoiceItems.Remove(SelectedInvoiceItem.Model);
            InvoiceItems.Remove(SelectedInvoiceItem);
            _db.SaveChanges();
        }
        #endregion

        /// <summary>
        /// Hàm này chỉ có TableEditor được gọi.
        /// </summary>
        [RelayCommand]
        void Restore()
        {
            SelectedInvoice.Deleted = 0;
            _db.SaveChanges();

            Invoices.Remove(SelectedInvoice);
            SelectedInvoice = new() { Date = DateTime.Now };
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
                Total = this.Total,
                Revisit = SelectedInvoice.Revisit ?? DateTime.UnixEpoch,
                Note = SelectedInvoice.Note +
                       (IsRevisitValid ? $" (Tái khám ngày {SelectedInvoice.Revisit:dd/MM/yyyy})" : "")
            };

            // Tìm lịch sử
            var invoices = _db.Invoices.Include(i => i.InvoiceItems)
                                       .ThenInclude(ii => ii.Service)
                                       .Where(i => i.Id == SelectedInvoice.Id).ToList();
            List<SummaryServiceDto> services = [];

            // lấy invoices[0] vì chắc chắn rằng chỉ có 1 kết quả (tìm theo primary key)
            foreach (var item in invoices[0].InvoiceItems)
            {
                // line in timeline
                var line = new SummaryServiceDto()
                {
                    ServiceName = item.Service.Name,
                    Quantity = item.Quantity,
                    Price = item.Price,
                    Discount = item.Discount,
                    Total = this.Total
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
