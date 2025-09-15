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
        private bool? input;

        public static string Title => "Đơn nhập/xuất";

        public long Total => IdnItems?.Sum(i => i.Quantity * i.Price) ?? 0;

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
                                              i.Deleted == false).ToList();

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
                SelectedIdn.Input = Input ?? false;
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

                    int quantityDelta;
                    long totalDelta;

                    if (SelectedIdn.Input == true) // Phiếu nhập
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
            SelectedIdn.Deleted = true;

            var items = _db.Idnitems.Where(i => i.IdnId == SelectedIdn.Id).ToList();

            foreach (var item in items)
            {
                int quantityDelta = SelectedIdn.Input == true ? -item.Quantity : item.Quantity;
                long totalDelta = SelectedIdn.Input == true ? -item.Quantity * item.Price : item.Quantity * item.Price;
                _productService.UpdateInventory(item.ItemId, quantityDelta, totalDelta);
            }

            Idns.Remove(SelectedIdn);
            _db.SaveChanges();
        }

        [RelayCommand]
        void Restore()
        {
            SelectedIdn.Deleted = false;

            var items = _db.Idnitems.Where(i => i.IdnId == SelectedIdn.Id).ToList();

            foreach (var item in items)
            {
                int quantityDelta = SelectedIdn.Input == false ? -item.Quantity : item.Quantity;
                long totalDelta = SelectedIdn.Input == false ? -item.Quantity * item.Price : item.Quantity * item.Price;
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
                Arguments = $"--report delivery-note --idn {idnPath} --items {itemsPath} --config {Config.full_path}"
            });
        }

        [RelayCommand]
        void PrintStock()
        {
            DateTime to = ToDate.Date.AddDays(1).AddTicks(-1);

            var transactions = (
                from idn in _db.Idns
                join item in _db.Idnitems on idn.Id equals item.IdnId
                join p in _db.Products on item.ItemId equals p.Id
                where idn.Deleted == false
                orderby idn.Date
                select new StockTransactionDto
                {
                    Date = idn.Date,
                    IsInput = idn.Input == true,
                    ProductId = item.ItemId,
                    ProductName = p.Name,
                    Quantity = item.Quantity,
                    Price = item.Price
                }
            ).ToList();

            // summary cho từng sản phẩm
            var summary = new Dictionary<long, StockSummaryDto>();

            // tồn hiện tại cho từng sản phẩm
            var current = new Dictionary<long, (int qty, long value)>();

            foreach (var t in transactions.OrderBy(x => x.Date))
            {
                if (!summary.ContainsKey(t.ProductId))
                {
                    summary[t.ProductId] = new StockSummaryDto
                    {
                        ProductId = t.ProductId,
                        ProductName = t.ProductName
                    };
                    current[t.ProductId] = (0, 0);
                }

                var cur = current[t.ProductId];

                if (t.Date < FromDate)
                {
                    // tồn đầu kỳ
                    if (t.IsInput)
                    {
                        cur.qty += t.Quantity;
                        cur.value += t.Quantity * t.Price;
                    }
                    else
                    {
                        var avgPrice = cur.qty > 0 ? cur.value / cur.qty : 0;
                        cur.qty -= t.Quantity;
                        cur.value -= t.Quantity * avgPrice;
                    }
                }
                else if (t.Date <= to)
                {
                    if (t.IsInput)
                    {
                        summary[t.ProductId].InQty += t.Quantity;
                        summary[t.ProductId].InValue += t.Quantity * t.Price;

                        cur.qty += t.Quantity;
                        cur.value += t.Quantity * t.Price;
                    }
                    else
                    {
                        var avgPrice = cur.qty > 0 ? cur.value / cur.qty : 0;

                        summary[t.ProductId].OutQty += t.Quantity;
                        summary[t.ProductId].OutValue += Convert.ToInt32(t.Quantity * avgPrice);

                        cur.qty -= t.Quantity;
                        cur.value -= t.Quantity * avgPrice;
                    }
                }

                current[t.ProductId] = cur;
            }

            // sau khi duyệt hết, gán tồn đầu kỳ và cuối kỳ cho từng sản phẩm
            foreach (var kv in summary)
            {
                var pid = kv.Key;
                var s = kv.Value;
                var (qty, value) = current[pid];

                s.BeginningQty = s.InQty == 0 && s.OutQty == 0 ? qty : qty + s.OutQty - s.InQty;
                s.BeginningValue = s.InValue == 0 && s.OutValue == 0 ? value : value + s.OutValue - s.InValue;

                s.EndQty = qty;
                s.EndValue = value;

                summary[pid] = s;
            }

            var summaryPath = IOUtil.WriteJsonToTempFile(summary, $"{Guid.NewGuid()}.json");
            Process.Start(new ProcessStartInfo()
            {
                FileName = @"NhakhoaMyNgoc_RDLC.exe",
                Arguments = $"--report stock-summary --summary {summaryPath} --from {FromDate:dd/MM/yyyy} --to {to:dd/MM/yyyy} --total {summary.Values.Sum(s => s.EndValue)} --config {Config.full_path}"
            });
        }

        partial void OnSelectedIdnChanged(Idn value)
        {
            if (value is null) return;

            Input = value.Input;

            var items = _db.Idnitems.Where(i => i.IdnId == value.Id).ToList();
            var wrapped = items.Select(i =>
            {
                var wrapper = new IdnItemWrapper(i)
                    { Products = _productService.GetAllProducts() };
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
                    int quantityDelta = SelectedIdn.Input == true ? -item.Quantity : item.Quantity;
                    long totalDelta = SelectedIdn.Input == true ? -item.Quantity * item.Price : item.Quantity * item.Price;
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
            OnPropertyChanged(nameof(Total));
        }
    }
}
