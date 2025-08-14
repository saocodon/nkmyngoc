using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using NhakhoaMyNgoc.Interfaces;
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
using System.Windows;

namespace NhakhoaMyNgoc.ViewModels
{
    public partial class IDNViewModel : ObservableObject
    {
        private readonly DataContext _db;
        private readonly IProductService _productService;

        [ObservableProperty]
        private ObservableCollection<Idn> idns = [];

        [ObservableProperty]
        private DateTime fromDate = DateTime.Today;

        [ObservableProperty]
        private DateTime toDate = DateTime.Today;

        [ObservableProperty]
        private Idn selectedIdn = new();

        [ObservableProperty]
        private ObservableCollection<IdnItemWrapper> idnItems = [];

        [ObservableProperty]
        private IdnItemWrapper? selectedIdnItem;

        [ObservableProperty]
        private int? input;

        public static string Title => "Đơn nhập/xuất";

        public int Total => IdnItems?.Sum(i => i.Quantity * i.Price) ?? 0;

        public bool IsReadOnly { get; set; } = false;

        public IDNViewModel(DataContext db, IProductService productService)
        {
            _db = db;
            _productService = productService;

            LoadIDNs();
            SelectedIdn.Date = DateTime.Now;
        }

        [RelayCommand]
        void LoadIDNs()
        {
            DateTime to = ToDate.Date.AddDays(1).AddTicks(-1);

            var result = _db.Idns.Where(i => i.Date >= FromDate &&
                                              i.Date <= to &&
                                              i.Deleted == 0).ToList();

            Idns = new ObservableCollection<Idn>(result);
            SelectedIdn = Idns.FirstOrDefault() ?? new() { Date = DateTime.Now };
        }

        [RelayCommand]
        void StartAddNew() => SelectedIdn = new() { Date = DateTime.Now };

        [RelayCommand]
        void Save()
        {
            try
            {
                SelectedIdn.Input = Input ?? 0;
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

                    int quantityDelta, totalDelta;

                    if (SelectedIdn.Input == 1) // Phiếu nhập
                    {
                        if (item.Id != 0)
                        {
                            var oldItem = _db.Idnitems
                                            .AsNoTracking()
                                            .First(x => x.Id == item.Id);
                            quantityDelta = -oldItem.Quantity + item.Quantity;
                            totalDelta = -oldItem.Quantity * oldItem.Price + item.Quantity * item.Price;
                        }
                        else
                        {
                            quantityDelta = item.Quantity;
                            totalDelta = item.Quantity * item.Price;
                        }
                    }
                    else // Phiếu xuất
                    {
                        var currentProduct = _productService.GetAllProducts().FirstOrDefault(p => p.Id == item.ItemId);
                        if (currentProduct != null && currentProduct.Quantity < item.Quantity)
                        {
                            MessageBox.Show($"Không đủ hàng để xuất cho sản phẩm: {currentProduct.Name}");
                            return;
                        }

                        if (item.Id != 0)
                        {
                            var oldItem = _db.Idnitems
                                            .AsNoTracking()
                                            .First(x => x.Id == item.Id);
                            quantityDelta = oldItem.Quantity - item.Quantity;
                            totalDelta = oldItem.Quantity * oldItem.Price - item.Quantity * item.Price;
                        }
                        else
                        {
                            quantityDelta = -item.Quantity;
                            totalDelta = -item.Quantity * item.Price;
                        }
                    }

                    _productService.UpdateInventory(item.ItemId, quantityDelta, totalDelta);

                    if (item.Id != 0)
                        _db.Idnitems.Update(item.Model);
                    else
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
        void Delete()
        {
            SelectedIdn.Deleted = 1;

            var items = _db.Idnitems.Where(i => i.IdnId == SelectedIdn.Id).ToList();

            foreach (var item in items)
            {
                int quantityDelta = SelectedIdn.Input == 1 ? -item.Quantity : item.Quantity;
                int totalDelta = SelectedIdn.Input == 1 ? -item.Quantity * item.Price : item.Quantity * item.Price;
                _productService.UpdateInventory(item.ItemId, quantityDelta, totalDelta);
            }

            Idns.Remove(SelectedIdn);
            _db.SaveChanges();
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
                    Total = item.Quantity * item.Price
                };
                dtoItems.Add(dtoItem);
            }

            var idnPath = IOUtil.WriteJsonToTempFile(idn, $"IDN{SelectedIdn.Id}.json");
            var itemsPath = IOUtil.WriteJsonToTempFile(dtoItems, $"IDNItem{SelectedIdn.Id}.json");

            Process.Start(new ProcessStartInfo()
            {
                FileName = @"NhakhoaMyNgoc_RDLC.exe",
                Arguments = $"--report delivery-note --idn {idnPath} --items {itemsPath}"
            });
        }

        partial void OnSelectedIdnChanged(Idn value)
        {
            if (value is null) return;

            Input = value.Input;

            var items = _db.Idnitems.Where(i => i.IdnId == value.Id).ToList();
            var wrapped = items.Select(i =>
            {
                var wrapper = new IdnItemWrapper(i);
                wrapper.PropertyChanged += SelectedIdnItem_PropertyChanged;
                return wrapper;
            }).ToList();

            IdnItems.CollectionChanged -= IdnItems_CollectionChanged;
            IdnItems.Clear();
            foreach (var item in wrapped)
                IdnItems.Add(item);

            SelectedIdnItem = IdnItems.FirstOrDefault() ?? new();

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
                {
                    item.PropertyChanged -= IdnItem_PropertyChanged;
                    int quantityDelta = SelectedIdn.Input == 1 ? -item.Quantity : item.Quantity;
                    int totalDelta = SelectedIdn.Input == 1 ? -item.Quantity * item.Price : item.Quantity * item.Price;
                    _productService.UpdateInventory(item.ItemId, quantityDelta, totalDelta);

                    if (item.Id != 0)
                        _db.Idnitems.Remove(item.Model);
                }
            }

            OnPropertyChanged(nameof(Total));
        }

        private void IdnItem_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is nameof(IdnItemWrapper.Quantity)
                or nameof(IdnItemWrapper.Price)
                or nameof(IdnItemWrapper.Total))
            {
                OnPropertyChanged(nameof(Total));
            }
        }

        partial void OnSelectedIdnItemChanged(IdnItemWrapper? oldValue, IdnItemWrapper? newValue)
        {
            if (oldValue is not null)
                oldValue.PropertyChanged -= SelectedIdnItem_PropertyChanged;

            if (newValue is not null)
                newValue.PropertyChanged += SelectedIdnItem_PropertyChanged;
        }

        private void SelectedIdnItem_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IdnItemWrapper.ItemId))
            {
                Debug.WriteLine("Selected item's ItemId changed!");
            }

            OnPropertyChanged(nameof(Total));
        }
    }
}
