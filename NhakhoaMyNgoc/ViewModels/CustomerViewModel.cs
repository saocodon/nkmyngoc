using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Win32;
using NhakhoaMyNgoc.Models;
using NhakhoaMyNgoc.Utilities;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Windows;
using NhakhoaMyNgoc_Connector.DTOs;
using NhakhoaMyNgoc_Connector;
using System.IO;
using System.Text.Json;
using NhakhoaMyNgoc.Converters;

namespace NhakhoaMyNgoc.ViewModels
{
    public partial class CustomerViewModel : ObservableObject
    {
        private readonly DataContext _db;

        public CustomerViewModel(DataContext db)
        {
            _db = db;
        }

        #region Global
        [ObservableProperty]
        private ObservableCollection<Customer> customers = [];

        [ObservableProperty]
        private Customer selectedCustomer = new();

        public static string Title => "Khách hàng";

        public bool IsReadOnly { get; set; } = false;
        #endregion

        #region Add & edit
        [RelayCommand]
        void Save()
        {
            if (SelectedCustomer.Id == 0) // khách mới
            {
                _db.Customers.Add(SelectedCustomer);
                Customers.Add(SelectedCustomer);

                _db.SaveChanges();
                SelectedCustomer = new(); // reset sau khi lưu
            }
            else // khách cũ
            {
                _db.Customers.Update(SelectedCustomer);
                _db.SaveChanges();
            }

            // để lưu ảnh
            Messenger.Publish("SaveCustomer", SelectedCustomer);
        }

        [RelayCommand]
        void StartAddNew() => SelectedCustomer = new();
        #endregion

        [RelayCommand]
        void Delete()
        {
            SelectedCustomer.Deleted = 1;
            _db.SaveChanges();

            // xóa trong RAM
            Customers.Remove(SelectedCustomer);
            SelectedCustomer = new();
        }

        #region Find
        [ObservableProperty]
        bool isSearchMode;

        [ObservableProperty]
        Customer searchForm = new();

        [RelayCommand]
        void SwitchToSearchMode() => IsSearchMode = true;

        // bỏ dấu trong tiếng Việt
        static string RemoveDiacritics(string? input)
        {
            if (input == null) return string.Empty;

            return string.Concat(
                input.Normalize(NormalizationForm.FormD)
                     .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark));
        }

        [RelayCommand]
        void FindCustomer()
        {
            string? name = RemoveDiacritics(SearchForm.Name).ToLower();
            string? address = RemoveDiacritics(SearchForm.Address).ToLower();
            string? phone = SearchForm.Phone;
            string? cid = SearchForm.Cid;

            var query = _db.Customers.AsEnumerable()
                                     .Where(i => i.Deleted == 0);

            // kiểm tra từng thông tin trống -> bỏ khỏi SQL query
            if (!string.IsNullOrWhiteSpace(name))
                query = query.Where(i => RemoveDiacritics(i.Name).ToLower().Contains(name));

            if (!string.IsNullOrWhiteSpace(cid))
                query = query.Where(i => i.Cid == cid);

            if (!string.IsNullOrWhiteSpace(address))
                query = query.Where(i => RemoveDiacritics(i.Address).ToLower().Contains(address));

            if (!string.IsNullOrWhiteSpace(phone))
                query = query.Where(i => i.Phone == phone);

            var result = query.ToList();
            Customers = new ObservableCollection<Customer>(result);
            SelectedCustomer = Customers.FirstOrDefault() ?? new();

            IsSearchMode = false;
        }
        #endregion

        /// <summary>
        /// Hàm này chỉ có TableEditor được gọi.
        /// </summary>
        [RelayCommand]
        void Restore()
        {
            SelectedCustomer.Deleted = 0;
            _db.SaveChanges();

            // cho TableEditor
            Customers.Remove(SelectedCustomer);
            SelectedCustomer = new();
        }

        [RelayCommand]
        void AddCustomerImage(Customer customer)
        {
            OpenFileDialog imgOfd = new()
            {
                Multiselect = true,
                Filter = "Ảnh|*.png;*.jpg;*.jpeg;*.bmp"
            };
            if (imgOfd.ShowDialog() == true)
                Messenger.Publish("AddCustomerImage", SelectedCustomer, imgOfd.FileNames);
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

            // Tìm lịch sử
            var invoices = _db.Invoices.Include(i => i.InvoiceItems)
                                       .ThenInclude(ii => ii.Service)
                                       .Where(i => i.CustomerId == customer.Id).ToList();
            List<SummaryServiceDto> history = [];
            foreach (var invoice in invoices)
            {
                foreach (var item in invoice.InvoiceItems)
                {
                    // line in timeline
                    var line = new SummaryServiceDto()
                    {
                        Date = invoice.Date,
                        ServiceName = item.Service.Name
                    };
                    history.Add(line);
                }
            }

            var customerFilePath = IOUtil.WriteJsonToTempFile(customer, $"Customer{customer.Id}.json");
            var historyFilePath = IOUtil.WriteJsonToTempFile(history, $"History{customer.Id}.json");

            // TODO: cái này phải thay đổi khi đóng gói
            Process.Start(new ProcessStartInfo()
            {
                FileName = @"NhakhoaMyNgoc_RDLC.exe",
                Arguments = $"--report customer-history --customer {customerFilePath} --history {historyFilePath}"
            });
        }

        partial void OnSelectedCustomerChanged(Customer value)
        {
            Messenger.Publish("OnSelectedCustomerChanged", value);
        }
    }
}
