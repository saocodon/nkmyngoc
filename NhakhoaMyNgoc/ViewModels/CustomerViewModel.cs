using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Win32;
using NhakhoaMyNgoc.Models;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;

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
        #endregion

        #region Add & edit
        [RelayCommand]
        void SaveCustomer()
        {
            // Một field NOT NULL = NULL thì return ngay.
            if (string.IsNullOrWhiteSpace(SelectedCustomer.Name)) return;

            if (SelectedCustomer.Id == 0) // khách mới
            {
                _db.Customers.Add(SelectedCustomer);
                Customers.Add(SelectedCustomer);
            }
            else // khách cũ
            {
                _db.Customers.Update(SelectedCustomer);
            }

            _db.SaveChanges();
            SelectedCustomer = new(); // reset sau khi lưu
        }

        [RelayCommand]
        void StartAddNew() => SelectedCustomer = new();
        #endregion

        [RelayCommand]
        void DeleteCustomer()
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

            IsSearchMode = false;
        }
        #endregion

        [RelayCommand]
        void RestoreCustomer()
        {
            SelectedCustomer.Deleted = 0;
            _db.SaveChanges();

            // dành cho bảng TableEditor
            Customers.Remove(SelectedCustomer);
            SelectedCustomer = new();
        }


        [RelayCommand]
        void AddCustomerImage()
        {
            OpenFileDialog imgOfd = new OpenFileDialog()
            {
                Multiselect = true,
                Filter = "Ảnh|*.png;*.jpg;*.jpeg;*.bmp"
            };
            if (imgOfd.ShowDialog() == true)
            {
                foreach (var path in imgOfd.FileNames)
                {
                    // Copy file đã chọn vào thư mục có mã ID khách hàng
                    // TODO: get config.
                    // Lưu đường dẫn vào database
                }
            }
        }

        partial void OnSelectedCustomerChanged(Customer value)
        {
            Messenger.Publish("CustomerSelected", value);
        }
    }
}
