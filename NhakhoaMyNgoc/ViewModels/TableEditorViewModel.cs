using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;
using NhakhoaMyNgoc.Interfaces;
using NhakhoaMyNgoc.Models;
using NhakhoaMyNgoc.ModelWrappers;
using NhakhoaMyNgoc.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit.PropertyGrid;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace NhakhoaMyNgoc.ViewModels
{
    public partial class TableEditorViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<object> tabs = [];

        [ObservableProperty]
        private object selectedTab = new();

        private readonly DataContext _db;

        public CustomerViewModel? CustomerVM { get; }
        public InvoiceViewModel? InvoiceVM { get; }
        public IDNViewModel? IdnVM { get; }
        public ProductService? ProductService { get; }
        public ProductViewModel? ProductVM { get; }
        public ServiceViewModel? ServiceVM { get; }
        public string? Title { get; set; }

        public TableEditorViewModel(DataContext db, int func, IProductService ps)
        {
            _db = db;
            switch (func)
            {
                // Thùng rác
                case 1:
                    Title = "Thùng rác";

                    CustomerVM = new CustomerViewModel(db);
                    ServiceVM = new ServiceViewModel(_db, loadDeleted: true)
                    { Mode = AppViewModel.ViewMode.Restore };
                    InvoiceVM = new InvoiceViewModel(db, ServiceVM.Services);
                    IdnVM = new IDNViewModel(db, ps);
                    ProductVM = new ProductViewModel(ps, loadDeleted: true)
                    { Mode = AppViewModel.ViewMode.Restore };

                    // không load sẵn nên phải làm như thế này
                    CustomerVM.Customers = new(_db.Customers.Where(c => c.Deleted == true));
                    InvoiceVM.Invoices = new([.. _db.Invoices
                                .Include(i => i.Customer)
                                .Where(i => i.Deleted == true)]);
                    IdnVM.Idns = new([.. _db.Idns
                                .Where(i => i.Deleted == true)]);

                    var deletedProducts = _db.Products.Where(i => i.Deleted == true).ToList();
                    // Gán sự kiện tính Total cho mọi item
                    var wrapped = deletedProducts.Select(i =>
                    {
                        var wrapper = new ProductWrapper(i);
                        return wrapper;
                    }).ToList();
                    ProductVM.Products = new(wrapped);

                    Tabs.Add(CustomerVM);
                    Tabs.Add(InvoiceVM);
                    Tabs.Add(IdnVM);
                    Tabs.Add(ProductVM);
                    Tabs.Add(ServiceVM);
                    break;
                // Quản lý tài nguyên (bảng dịch vụ, kho hàng...)
                case 2:
                    Title = "Quản lý tài nguyên";

                    ProductVM = new ProductViewModel(ps, loadDeleted: false)
                    { Mode = AppViewModel.ViewMode.Manage, IsReadOnly = false };
                    ServiceVM = new ServiceViewModel(_db)
                    { Mode = AppViewModel.ViewMode.Manage };

                    Tabs.Add(ProductVM);
                    Tabs.Add(ServiceVM);
                    break;
                default:
                    break;
            }
        }
    }
}
