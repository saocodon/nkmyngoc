using NhakhoaMyNgoc.Converters;
using NhakhoaMyNgoc.Models;
using NhakhoaMyNgoc.ViewModels;
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
            if (Resources["ServiceIdToNameConverter"] is ServiceIdToNameConverter cvt)
                cvt.Services = vm.InvoiceVM.Services;
        }

        private void CustomerVM_PropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(CustomerViewModel.IsSearchMode))
            {
                CustomerFormUI.DataContext = vm.CustomerVM.IsSearchMode
                    ? vm.CustomerVM.SearchForm
                    : vm.CustomerVM.SelectedCustomer;
            }
        }
    }
}