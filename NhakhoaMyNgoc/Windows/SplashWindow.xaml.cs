using NhakhoaMyNgoc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace NhakhoaMyNgoc.Windows
{
    /// <summary>
    /// Interaction logic for SplashWindow.xaml
    /// </summary>
    public partial class SplashWindow : Window
    {
        public string AppVersion => $"v{Assembly.GetExecutingAssembly().GetName().Version!.ToString(2)}";
        public SplashWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        public void SetStatus(int code)
        {
            string message = code switch
            {
                1 => "Đang khởi tạo cài đặt nội bộ",
                2 => "Đang kiểm tra tài nguyên hệ thống",
                3 => "Sẵn sàng",
                _ => string.Empty
            };

            Application.Current.Dispatcher.Invoke(() =>
            {
                lblStatus.Text = message;
                prgbStatus.Value = code;
            });
        }
    }
}
