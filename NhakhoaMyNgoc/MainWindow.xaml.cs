using NhakhoaMyNgoc.Converters;
using NhakhoaMyNgoc.Models;
using NhakhoaMyNgoc.ModelWrappers;
using NhakhoaMyNgoc.ViewModels;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml;

namespace NhakhoaMyNgoc
{
    public partial class MainWindow : Window
    {
        readonly MainViewModel vm;
        public MainWindow(MainViewModel _vm)
        {
            DataContext = vm = _vm;
            InitializeComponent();

            // nhảy UI theo CustomerVM.IsSearchMode
            vm.CustomerVM.PropertyChanged += CustomerVM_PropertyChanged;

            // load bảng Services của ServicesIdToNameConverter
            // lấy key trong Resources (XAML).
            if (Resources["ServiceIdToNameConverter"] is ServiceIdToNameConverter stn)
                stn.Services = vm.InvoiceVM.Services;
            if (Resources["ProductsToNameConverter"] is ProductsToNameConverter ptn)
                ptn.Products = vm.IdnVM.Products!; // không null được vì đã load từ lúc bật app
        }

        /// <summary>
        /// Đổi form tìm kiếm và form nhập.
        /// </summary>
        private void CustomerVM_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CustomerViewModel.IsSearchMode))
            {
                CustomerFormUI.DataContext = vm.CustomerVM.IsSearchMode
                    ? vm.CustomerVM.SearchForm
                    : vm.CustomerVM.SelectedCustomer;
            }
        }

        #region Giả label mô tả hình
        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox tb)
            {
                tb.Background = Brushes.White;
                tb.BorderThickness = new Thickness(1);
                tb.IsReadOnly = false;
                tb.Cursor = Cursors.IBeam;
                tb.SelectAll();
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (sender is TextBox tb)
            {
                tb.Background = Brushes.Transparent;
                tb.BorderThickness = new Thickness(0);
                tb.IsReadOnly = true;
                tb.Cursor = Cursors.Arrow;
            }
        }
        #endregion

        private void InvoiceItemsGrid_InitializingNewItem(object sender, InitializingNewItemEventArgs e)
        {
            if (e.NewItem is InvoiceItemWrapper wrapper)
            {
                var vm = DataContext as MainViewModel; // hoặc ViewModel bạn dùng
                wrapper.Services = vm?.InvoiceVM?.Services ?? new();
            }
        }
    }
}