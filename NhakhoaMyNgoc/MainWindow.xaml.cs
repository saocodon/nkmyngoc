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
        public MainViewModel ViewModel { get; } = new();
        public MainWindow()
        {
            InitializeComponent();
            // vẫn phải set lại trong XAML
            DataContext = ViewModel;

            ViewModel.CustomerVM.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(CustomerViewModel.IsSearchMode))
                {
                    CustomerFormUI.DataContext = ViewModel.CustomerVM.IsSearchMode
                        ? ViewModel.CustomerVM.SearchForm
                        : ViewModel.CustomerVM.SelectedCustomer;
                }
            };
        }
    }
}