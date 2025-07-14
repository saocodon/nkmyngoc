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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainViewModel mainVM { get; } = new();
        public MainWindow()
        {
            InitializeComponent();

            // vẫn phải set lại trong XAML
            DataContext = mainVM;

            // nhảy UI theo CustomerVM.IsSearchMode
            mainVM.CustomerVM.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(CustomerViewModel.IsSearchMode))
                {
                    CustomerFormUI.DataContext = mainVM.CustomerVM.IsSearchMode
                        ? mainVM.CustomerVM.SearchForm
                        : mainVM.CustomerVM.SelectedCustomer;
                }
            };

            // load bảng Services của Services ID -> Name converter
            if (Resources["ServiceIdToNameConverter"] is ServiceIdToNameConverter cvt)
                cvt.Services = mainVM.InvoiceVM.Services;
        }
    }
}