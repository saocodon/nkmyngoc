using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Win32;
using NhakhoaMyNgoc.Converters;
using NhakhoaMyNgoc.Models;
using NhakhoaMyNgoc.Utilities;
using NhakhoaMyNgoc_Connector;
using NhakhoaMyNgoc_Connector.DTOs;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Interop;

namespace NhakhoaMyNgoc.ViewModels
{
    public partial class CustomerViewModel : ObservableObject
    {
        private readonly DataContext _db;
        private readonly HubConnection _syncConn;
        private CustomerDto? selectedCustomerDto;

        Customer FromDto(CustomerDto msg)
        {
            return new Customer
            {
                Id = msg.Id,
                Deleted = msg.Deleted,
                Cid = msg.Cid,
                Name = msg.Name,
                Sex = msg.Sex switch { "Nam" => 0, "Nữ" => 1, _ => 2 },
                Birthdate = DateOnly.FromDateTime(msg.Birthdate),
                Address = msg.Address,
                Phone = msg.Phone
            };
        }

        public CustomerViewModel(DataContext db, HubConnection syncConn = null!)
        {
            _db = db;
            _syncConn = syncConn;

            _syncConn.On<CustomerDto>("OnSaveCustomer", msg =>
            {
                Application.Current.Dispatcher.Invoke(async () =>
                {
                    SelectedCustomer = FromDto(msg);
                    await Save();
                });
            });

            _syncConn.On<CustomerDto>("OnDeleteCustomer", msg =>
            {
                Application.Current.Dispatcher.Invoke(async () =>
                {
                    SelectedCustomer = FromDto(msg);
                    await Delete();
                });
            });
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
        async Task Save()
        {
            if (SelectedCustomer.Id == 0) // khách mới
            {
                _db.Customers.Add(SelectedCustomer);
                Customers.Add(SelectedCustomer);
                _db.SaveChanges();
            }
            else // khách cũ
            {
                _db.Customers.Update(SelectedCustomer);
                _db.SaveChanges();
            }

            OnSelectedCustomerChanged(SelectedCustomer);

            // để lưu ảnh
            WeakReferenceMessenger.Default.Send(new SaveCustomerMessage(SelectedCustomer));
            if (_syncConn != null)
                await _syncConn.InvokeAsync<bool>("SaveCustomer", selectedCustomerDto);
        }

        [RelayCommand]
        void StartAddNew()
        {
            SelectedCustomer = new() { Birthdate = DateOnly.FromDateTime(DateTime.Now) };
        }
        #endregion

        [RelayCommand]
        async Task Delete()
        {
            if (_syncConn != null)
                await _syncConn.InvokeAsync<bool>("DeleteCustomer", selectedCustomerDto);

            SelectedCustomer.Deleted = true;
            _db.SaveChanges();

            // xóa trong RAM
            Customers.Remove(SelectedCustomer);
            SelectedCustomer = new();
        }

        #region Find
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
            string? name = RemoveDiacritics(SelectedCustomer.Name).ToLower();
            string? address = RemoveDiacritics(SelectedCustomer.Address).ToLower();
            string? phone = SelectedCustomer.Phone;
            string? cid = SelectedCustomer.Cid;

            var query = _db.Customers.AsEnumerable()
                                     .Where(i => i.Deleted == false);

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
        }
        #endregion

        /// <summary>
        /// Hàm này chỉ có TableEditor được gọi.
        /// </summary>
        [RelayCommand]
        void Restore()
        {
            SelectedCustomer.Deleted = false;
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
                WeakReferenceMessenger.Default.Send(
                    new AddCustomerImageMessage((SelectedCustomer, imgOfd.FileNames))
                );
        }

        [RelayCommand]
        void Print()
        {
            if (selectedCustomerDto == null) return;

            // Tìm lịch sử
            var invoices = _db.Invoices.Include(i => i.InvoiceItems)
                                       .ThenInclude(ii => ii.Service)
                                       .Where(i => i.CustomerId == selectedCustomerDto.Id).ToList();
            List<SummaryServiceDto> history = [];
            foreach (var invoice in invoices)
            {
                foreach (var item in invoice.InvoiceItems)
                {
                    // line in timeline
                    var line = new SummaryServiceDto()
                    {
                        Date = invoice.Date,
                        ServiceName = item.Service!.Name
                    };
                    history.Add(line);
                }
            }

            var customerFilePath = IOUtil.WriteJsonToTempFile(selectedCustomerDto, $"{Guid.NewGuid()}.json");
            var historyFilePath = IOUtil.WriteJsonToTempFile(history, $"{Guid.NewGuid()}.json");

            // TODO: cái này phải thay đổi khi đóng gói
            Process.Start(new ProcessStartInfo()
            {
                FileName = @"NhakhoaMyNgoc_RDLC.exe",
                Arguments = $"--report customer-history --customer {customerFilePath} --history {historyFilePath} --config {Config.full_path}"
            });
        }

        partial void OnSelectedCustomerChanged(Customer value)
        {
            WeakReferenceMessenger.Default.Send(new SelectedCustomerChangedMessage(value));
            if (selectedCustomer is null) return;

            DateOnly birthdate = SelectedCustomer.Birthdate ?? DateOnly.MaxValue;
            selectedCustomerDto = new CustomerDto(
                SelectedCustomer.Id,
                SelectedCustomer.Deleted,
                SelectedCustomer.Cid,
                SelectedCustomer.Name,
                SelectedCustomer.Sex switch { 0 => "Nam", 1 => "Nữ", _ => "Khác" },
                birthdate.ToDateTime(TimeOnly.MinValue),
                SelectedCustomer.Address,
                SelectedCustomer.Phone
            );
        }
    }
}
