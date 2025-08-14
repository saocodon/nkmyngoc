using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;
using NhakhoaMyNgoc.Models;
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

        public TableEditorViewModel(DataContext db, int func)
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
                    ProductService = new ProductService(db);
                    IdnVM = new IDNViewModel(db, ProductService);
                    ProductVM = new ProductViewModel(ProductService, loadDeleted: true)
                    { Mode = AppViewModel.ViewMode.Restore };       

                    Tabs.Add(CustomerVM);
                    Tabs.Add(InvoiceVM);
                    Tabs.Add(IdnVM);
                    Tabs.Add(ProductVM);
                    Tabs.Add(ServiceVM);
                    break;
                // Quản lý tài nguyên (bảng dịch vụ, kho hàng...)
                case 2:
                    Title = "Quản lý tài nguyên";

                    ProductService = new ProductService(db);
                    ProductVM = new ProductViewModel(ProductService, loadDeleted: false)
                    { Mode = AppViewModel.ViewMode.Manage };
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
