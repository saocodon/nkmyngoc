using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using NhakhoaMyNgoc.Models;
using NhakhoaMyNgoc.ModelWrappers;
using NhakhoaMyNgoc.Utilities;
using NhakhoaMyNgoc_Connector.DTOs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NhakhoaMyNgoc.ViewModels
{
    public partial class IDNViewModel : ObservableObject
    {
        private readonly DataContext _db;

        [ObservableProperty]
        private ObservableCollection<Idn> idns = [];

        [ObservableProperty]
        private DateTime fromDate = DateTime.Today;

        // DatePicker overwrite giá trị thành 12:00:00AM
        [ObservableProperty]
        private DateTime toDate = DateTime.Today;

        [ObservableProperty]
        private Idn selectedIdn = new();

        [ObservableProperty]
        private ObservableCollection<IdnItemWrapper> idnItems = [];

        public List<Product> Products { get; }

        public static string Title => "Đơn nhập/xuất";

        public int Total => IdnItems?.Sum(i => i.Quantity * i.Price) ?? 0;

        public IDNViewModel(DataContext db)
        {
            _db = db;

            // load sẵn kho vào đây.
            Products = new([ .. _db.Products]);

            // load ngày hôm nay vào datepicker.
            SelectedIdn.Date = DateTime.Now;
        }

        [RelayCommand]
        void LoadIDNs()
        {
            // vì thế phải làm như này
            DateTime to = ToDate.Date.AddDays(1).AddTicks(-1);

            var result = _db.Idns.Where(i => i.Date >= FromDate &&
                                              i.Date <= to &&
                                              i.Deleted == 0).ToList();
            
            Idns = new ObservableCollection<Idn>(result);
            SelectedIdn = Idns.FirstOrDefault() ?? new();
        }

        [RelayCommand]
        void StartAddNew() => SelectedIdn = new() { Date = DateTime.Now };

        [RelayCommand]
        void SaveIdn()
        {
            try
            {
                if (SelectedIdn.Id == 0) // hoá đơn mới
                {
                    _db.Idns.Add(SelectedIdn);
                    _db.SaveChanges();

                    Idns.Add(SelectedIdn);
                }
                else // hoá đơn cũ
                {
                    _db.Idns.Update(SelectedIdn);
                }

                foreach (var item in IdnItems)
                {
                    item.IdnId = SelectedIdn.Id;

                    if (item.Id == 0)
                        _db.Idnitems.Add(item.Model);
                }

                _db.SaveChanges();
            }
            catch
            {
                MessageBox.Show("Có một số trường dữ liệu bị rỗng. Kiểm tra lại và thử lại.");
            }
        }

        [RelayCommand]
        void DeleteIdn()
        {
            SelectedIdn.Deleted = 1;
            _db.SaveChanges();

            Idns.Remove(SelectedIdn);
            SelectedIdn = new();
        }

        [RelayCommand]
        void Print()
        {
            var idn = new IdnDto
            {
                Input = SelectedIdn.Input,
                Date = SelectedIdn.Date,
                Correspondent = SelectedIdn.Correspondent,
                Division = SelectedIdn.Division,
                Reason = SelectedIdn.Reason,
                CertificateId = SelectedIdn.CertificateId,
                Total = this.Total
            };
            List<IdnItemDto> dtoItems = [];
            var items = _db.Idnitems.Include(i => i.Item)
                                    .Where(i => i.IdnId == SelectedIdn.Id).ToList();
            foreach (var item in items)
            {
                var dtoItem = new IdnItemDto
                {
                    ProductName = item.Item.Name,
                    ProductId = item.Item.Id,
                    Unit = item.Item.Unit,
                    Demand = item.Demand,
                    Quantity = item.Quantity,
                    Price = item.Price,
                    Total = item.Total ?? 0
                };
                dtoItems.Add(dtoItem);
            }   
            var idnPath = IOUtil.WriteJsonToTempFile(idn, $"IDN{SelectedIdn.Id}.json");
            var itemsPath = IOUtil.WriteJsonToTempFile(dtoItems, $"IDNItem{SelectedIdn.Id}.json");
            // TODO: cái này phải thay đổi khi đóng gói
            Process.Start(new ProcessStartInfo()
            {
                FileName = @"..\..\..\..\NhakhoaMyNgoc_RDLC\bin\Debug\NhakhoaMyNgoc_RDLC.exe",
                Arguments = $"--report delivery-note --idn {idnPath} --items {itemsPath}"
            });
        }

        partial void OnSelectedIdnChanged(Idn value)
        {
            if (value is null) return;
            var items = _db.Idnitems.Where(i => i.IdnId == value.Id).ToList();
            var wrapped = items.Select(i =>
            {
                var wrapper = new IdnItemWrapper(i) { Products = this.Products };
                wrapper.PropertyChanged += (_, _) => OnPropertyChanged(nameof(Total));
                return wrapper;
            });
            IdnItems.Clear();
            foreach (var item in wrapped)
            {
                item.PropertyChanged += (_, _) => OnPropertyChanged(nameof(Total));
                IdnItems.Add(item);
            }
            IdnItems.CollectionChanged += IdnItems_CollectionChanged;

            OnPropertyChanged(nameof(Total));
        }

        private void IdnItems_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems is not null)
            {
                foreach (IdnItemWrapper item in e.NewItems)
                    item.PropertyChanged += IdnItem_PropertyChanged;
            }

            if (e.OldItems is not null)
            {
                foreach (IdnItemWrapper item in e.OldItems)
                    item.PropertyChanged -= IdnItem_PropertyChanged;
            }

            OnPropertyChanged(nameof(Total));
        }

        private void IdnItem_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is nameof(InvoiceItemWrapper.Quantity)
                or nameof(InvoiceItemWrapper.Price)
                or nameof(InvoiceItemWrapper.Total))
            {
                OnPropertyChanged(nameof(Total));
            }
        }

        partial void OnIdnItemsChanged(ObservableCollection<IdnItemWrapper> value)
        {
            OnPropertyChanged(nameof(Total));
        }

        /// <summary>
        /// Hàm này chỉ có TableEditor được gọi.
        /// </summary>
        [RelayCommand]
        void Restore()
        {
            SelectedIdn.Deleted = 0;
            _db.SaveChanges();

            Idns.Remove(SelectedIdn);
            SelectedIdn = new();
        }
    }
}
