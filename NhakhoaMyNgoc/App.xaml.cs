using NhakhoaMyNgoc.Models;
using NhakhoaMyNgoc.ViewModels;
using NhakhoaMyNgoc.Windows;
using System.Configuration;
using System.Data;
using System.Windows;

namespace NhakhoaMyNgoc
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var splash = new SplashWindow();
            splash.Show();

            var db = new DataContext();
            var mainVM = new MainViewModel(db);

            Task.Run(() =>
            {
                // tạo thư mục temp nếu chưa có
                // kiểm tra kết nối internet
                // init db
                // copy templates nếu cần
                // chuẩn bị giao diện
                // mở MainWindow
                Dispatcher.Invoke(() =>
                {
                    splash.Close();
                    new MainWindow() { DataContext = mainVM }.Show();
                });
            });
        }
    }
}
