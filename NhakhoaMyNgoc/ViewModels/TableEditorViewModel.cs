using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;
using NhakhoaMyNgoc.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace NhakhoaMyNgoc.ViewModels
{
    public partial class TableEditorViewModel : ObservableObject
    {
        [ObservableProperty]
        private ObservableCollection<object> tabs = [];

        [ObservableProperty]
        private object selectedTab = new();

        public CustomerViewModel CustomerVM { get; }
        public ImageViewModel ImageVM { get; }
        public InvoiceViewModel InvoiceVM { get; }
        public IDNViewModel IdnVM { get; }
        public ProductViewModel ProductVM { get; }

        public TableEditorViewModel(DataContext db)
        {
            CustomerVM = new CustomerViewModel(db);
            ImageVM = new ImageViewModel(db);
            InvoiceVM = new InvoiceViewModel(db);
            IdnVM = new IDNViewModel(db);
            ProductVM = new ProductViewModel(db);

            Tabs.Add(CustomerVM);
            Tabs.Add(ImageVM);
            Tabs.Add(InvoiceVM);
            Tabs.Add(IdnVM);
            Tabs.Add(ProductVM);
        }
    }
}
