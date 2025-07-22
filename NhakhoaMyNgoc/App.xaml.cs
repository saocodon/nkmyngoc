using NhakhoaMyNgoc.Models;
using NhakhoaMyNgoc.ViewModels;
using NhakhoaMyNgoc.Windows;
using NhakhoaMyNgoc.Utilities;
using System.Configuration;
using System.Data;
using System.IO;
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

            Task.Run(() =>
            {
                // tạo thư mục temp nếu chưa có
                Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), "NhakhoaMyNgoc"));
                splash.SetStatus(1);
                // kiểm tra kết nối internet
                if (Config.Load().GetAwaiter().GetResult() == false)
                {
                    // có thể là không có kết nối internet.
                    if (MessageBox.Show("Có lỗi xảy ra, có lẽ là không có mạng?\n" +
                        "Bạn có muốn tiếp tục, nếu vậy thì phần mềm sẽ áp dụng các cài đặt bảo mật cập nhật từ lần trước.",
                        "Không thể kết nối tới cơ sở dữ liệu nội bộ", MessageBoxButton.YesNo, MessageBoxImage.Error) == MessageBoxResult.Yes)
                    {
                        if (Config.Load(true).GetAwaiter().GetResult() == false)
                        {
                            if (MessageBox.Show("Không tìm thấy cài đặt đã lưu. Hãy cố gắng kết nối mạng rồi thử lại.",
                                "Không thể tiếp tục", MessageBoxButton.OK, MessageBoxImage.Error) == MessageBoxResult.OK)
                                Current.Shutdown();
                        }
                    }
                    else
                        Current.Shutdown();
                }
                // copy templates nếu cần
                splash.SetStatus(2);
                IOUtil.CopyDirectory(Path.Combine(AppContext.BaseDirectory, "Templates"), PrintablePaper.RESOURCE_PATH);

                // lưu lại đường dẫn root
                var drive = IOUtil.FindDriveLetter();
                string fullPath = Path.Combine(drive.Name, Config.root_directory);
                if (File.Exists(fullPath) || Directory.Exists(fullPath))
                    Config.full_path = fullPath;

                var db = new DataContext();
                var mainVM = new MainViewModel(db);

                // mở MainWindow
                splash.SetStatus(3);
                Dispatcher.Invoke(() =>
                {
                    new MainWindow(mainVM).Show();
                    splash.Close();
                });
            });
        }
    }
}
