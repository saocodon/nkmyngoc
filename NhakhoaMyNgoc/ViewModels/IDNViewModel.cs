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

        [ObservableProperty]
        private IdnItemWrapper? selectedIdnItem;

        [ObservableProperty]
        public ObservableCollection<ProductWrapper>? products;

        public static string Title => "Đơn nhập/xuất";

        // tính lại Total bằng OnPropertyChanged (gọi lại getter)
        public int Total => IdnItems?.Sum(i => i.Quantity * i.Price) ?? 0;

        [ObservableProperty]
        private int? input;

        public IDNViewModel(DataContext db)
        {
            _db = db;

            // load sẵn kho
            LoadIDNs();

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
            SelectedIdn = Idns.FirstOrDefault() ?? new() { Date = DateTime.Now };

            // load lại số lượng từ kho
            var products_list = _db.Products.ToList();
            Products = new();
            foreach (var p in products_list)
                Products.Add(new ProductWrapper(p, IdnItems));
        }

        [RelayCommand]
        void StartAddNew() => SelectedIdn = new() { Date = DateTime.Now };

        [RelayCommand]
        void SaveIdn()
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
                    var product = Products!.FirstOrDefault(p => p.Id == item.ItemId); // hoặc item.ItemId

                    if (SelectedIdn.Input == 1)
                    {
                        // Phiếu nhập → cộng hàng
                        // không thể null vì chỉ cho chọn hàng có trong kho.

                        // nếu hàng này đã được thêm vào database, phải trừ đi số lượng cũ
                        if (item.Id != 0)
                        {
                            var oldItem = _db.Idnitems
                                            .AsNoTracking()
                                            .First(x => x.Id == item.Id);
                            // không thể có chuyện product.Quantity < oldItem.Quantity được.
                            product!.Quantity -= oldItem.Quantity;
                            product!.Total -= oldItem.Quantity * oldItem.Price;
                        }
                        product!.Quantity += item.Quantity;
                        product!.Total += item.Quantity * item.Price;
                    }
                    else
                    {
                        // Phiếu xuất → trừ hàng
                        if (product!.Quantity < item.Quantity)
                        {
                            MessageBox.Show($"Không đủ hàng để xuất cho sản phẩm: {product.Name}");
                            return;
                        }

                        // nếu hàng này đã được thêm vào database, phải cộng lại số lượng cũ
                        if (item.Id != 0)
                        {
                            var oldItem = _db.Idnitems
                                            .AsNoTracking()
                                            .First(x => x.Id == item.Id);
                            product!.Quantity += oldItem.Quantity;
                            product!.Total += oldItem.Quantity * oldItem.Price;
                        }
                        product.Quantity -= item.Quantity;
                        product.Total -= item.Quantity * item.Price;
                    }

                    if (item.Id != 0)
                        _db.Idnitems.Update(item.Model);
                    else
                        _db.Idnitems.Add(item.Model);
                }

                SaveProducts();
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

            var items = _db.Idnitems.Where(i => i.IdnId == SelectedIdn.Id).ToList();

            foreach (var item in items)
            {
                var product = Products!.FirstOrDefault(p => p.Id == item.ItemId);

                if (SelectedIdn.Input == 1)
                {
                    // Hủy phiếu nhập → trừ lại hàng
                    // không thể null.
                    product!.Quantity -= item.Quantity;
                    product!.Total -= item.Quantity * item.Price;
                }
                else
                {
                    // Hủy phiếu xuất → cộng lại hàng
                    product!.Quantity += item.Quantity;
                    product!.Total += item.Quantity * item.Price;
                }
            }

            SaveProducts();

            Idns.Remove(SelectedIdn);
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

            // load sẵn kho vào đây.
            var products_list = _db.Products.ToList();
            Products = new();
            foreach (var p in products_list)
                Products.Add(new ProductWrapper(p, IdnItems));

            // cập nhật binding UI
            Input = value.Input;

            var items = _db.Idnitems.Where(i => i.IdnId == value.Id).ToList();
            // Gán sự kiện tính Total cho mọi item
            var wrapped = items.Select(i =>
            {
                var wrapper = new IdnItemWrapper(i) { Products = this.Products };
                wrapper.PropertyChanged += SelectedIdnItem_PropertyChanged;
                return wrapper;
            }).ToList(); // Ép thành List để có thể .FirstOrDefault

            IdnItems.CollectionChanged -= IdnItems_CollectionChanged;
            IdnItems.Clear();

            foreach (var item in wrapped)
                IdnItems.Add(item);

            // Gán lại SelectedIdnItem là 1 item thật sự trong danh sách
            SelectedIdnItem = IdnItems.FirstOrDefault() ?? new();

            IdnItems.CollectionChanged += IdnItems_CollectionChanged;

            OnPropertyChanged(nameof(Total));
        }

        partial void OnInputChanged(int? oldValue, int? newValue)
        {
            if (oldValue is null || newValue is null) return;
            if (oldValue == newValue) return;

            // Trạng thái Input đã thay đổi → cần cập nhật kho
            foreach (var item in IdnItems)
            {
                var product = Products!.FirstOrDefault(p => p.Id == item.ItemId);
                if (product is null) continue;

                if (oldValue == 0)
                {
                    // chuyển từ Xuất → Nhập
                    product.Quantity = product.Quantity + item.Quantity * 2;
                    product.Total = product.Total + item.Quantity * item.Price * 2;
                }
                else
                {
                    // chuyển từ Nhập → Xuất
                    product.Quantity = product.Quantity - item.Quantity * 2;
                    product.Total = product.Total - item.Quantity * item.Price * 2;
                }
            }

            SaveProducts();
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
                    // Trừ lại kho vì item đã bị xoá khỏi đơn
                    var product = Products!.FirstOrDefault(p => p.Id == item.ItemId);
                    if (product is not null)
                    {
                        if (SelectedIdn.Input == 1)
                        {
                            product.Quantity -= item.Quantity;
                            product.Total -= item.Quantity * item.Price;
                        }
                        else
                        {
                            product.Quantity += item.Quantity;
                            product.Total += item.Quantity * item.Price;
                        }
                    }

                    // Nếu đã có trong database, cần xoá luôn
                    if (item.Id != 0)
                    {
                        _db.Idnitems.Remove(item.Model);
                    }
                }
            }

            SaveProducts();

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

            var items = _db.Idnitems.Where(i => i.IdnId == SelectedIdn.Id).ToList();

            foreach (var item in items)
            {
                var product = Products!.FirstOrDefault(p => p.Id == item.ItemId);
                if (product is null) continue;

                if (SelectedIdn.Input == 1)
                {
                    product.Quantity = product.Quantity + item.Quantity;
                    product.Total = product.Total + item.Quantity * item.Price;
                }
                else
                {
                    product.Quantity = product.Quantity - item.Quantity;
                    product.Total = product.Total - item.Quantity * item.Price;
                }
            }

            SaveProducts();

            Idns.Remove(SelectedIdn);
        }

        void SaveProducts()
        {
            foreach (var wrapper in Products!)
            {
                var product = wrapper.Model;
                product.Quantity = wrapper.Quantity;
                product.Total = wrapper.Total;
            }
            _db.SaveChanges();
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
                // Gọi gì đó khi ItemId của item được chọn thay đổi
                Debug.WriteLine("Selected item's ItemId changed!");
                // hoặc OnPropertyChanged(nameof(...)) nếu bạn cần cập nhật
            }

            OnPropertyChanged(nameof(Total));
        }
    }
}
