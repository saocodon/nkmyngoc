using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using NhakhoaMyNgoc.Models;
using NhakhoaMyNgoc.Utilities;
using NhakhoaMyNgoc_Connector.DTOs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        private ObservableCollection<Idnitem> idnItems = [];

        [ObservableProperty]
        private ObservableCollection<Product> products = [];

        public static string Title => "Đơn nhập/xuất";

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
        }

        [RelayCommand]
        void StartAddNew() => SelectedIdn = new() { Date = DateTime.Now };

        [RelayCommand]
        void SaveIdn()
        {
            if (SelectedIdn.Id == 0)
            {
                _db.Idns.Add(SelectedIdn);
                Idns.Add(SelectedIdn);

                foreach (Idnitem item in IdnItems)
                {
                    item.IdnId = _db.Idns.Count() + 1;
                    _db.Idnitems.Add(item);
                }    
            }
            else
            {
                _db.Idns.Update(SelectedIdn);
            }
            _db.SaveChanges();
            SelectedIdn = new();
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
                Total = SelectedIdn.Total
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
            IdnItems = new ObservableCollection<Idnitem>(items);
        }

        partial void OnIdnItemsChanged(ObservableCollection<Idnitem> value)
        {
            SelectedIdn.Total = 0;
            foreach (var item in value)
                SelectedIdn.Total += item.Quantity * item.Price;
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
