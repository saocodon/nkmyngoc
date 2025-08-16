using NhakhoaMyNgoc.Utilities;
using System.IO;
using System.Windows;

namespace NhakhoaMyNgoc.Windows
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();

            // free space
            long available_disk_space = IOUtil.FindDriveLetter().AvailableFreeSpace;
            long storage = IOUtil.GetDirectorySize(Config.full_path);
            lblStorage.Text = $"{IOUtil.FormatSize(storage)} / {IOUtil.FormatSize(available_disk_space)}";
            prgbStorage.Maximum = Convert.ToInt64(available_disk_space);
            prgbStorage.Value = Convert.ToInt64(storage);

            // root path
            txtRootPath.Text = Config.full_path;

            // câu hỏi bảo mật
            cboQuestion1.ItemsSource = Config.security_questions.ToArray();
            cboQuestion2.ItemsSource = Config.security_questions.ToArray();
        }

        private void BrowseRoot(object sender, RoutedEventArgs e)
        {
            var dialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
            if (dialog.ShowDialog() == true)
                txtRootPath.Text = dialog.SelectedPath;
        }

        private async void SaveSettings(object sender, RoutedEventArgs e)
        {
            if (txtNewPassword.Password != txtReenterNewPassword.Password)
            {
                MessageBox.Show("Mật khẩu nhập lại không khớp.");
                return;
            }

            // tạo mật khẩu mới, salt mới
            if (!string.IsNullOrWhiteSpace(txtNewPassword.Password))
            {
                (string hash, string salt) = PBKDF2.HashPassword(txtNewPassword.Password);
                Config.admin_password = hash;
                Config.admin_password_salt = salt;
            }

            int question1_id = cboQuestion1.SelectedIndex;
            int question2_id = cboQuestion2.SelectedIndex;
            if (question1_id >= 0 && question2_id >= 0 &&
                !string.IsNullOrWhiteSpace(txtAnswer1.Text) &&
                !string.IsNullOrWhiteSpace(txtAnswer2.Text))
            {
                Config.security_answers.Clear();
                Config.security_salts.Clear();

                (string hash1, string salt1) = PBKDF2.HashPassword(txtAnswer1.Text);
                Config.security_answers[$"q{question1_id}"] = hash1;
                Config.security_salts[$"q{question1_id}"] = salt1;

                (string hash2, string salt2) = PBKDF2.HashPassword(txtAnswer2.Text);
                Config.security_answers[$"q{question2_id}"] = hash2;
                Config.security_salts[$"q{question2_id}"] = salt2;
            }

            // kiểm tra serial volume
            string? serial = IOUtil.GetVolumeSerial(txtRootPath.Text[..3]);
            if (serial == null && Directory.Exists(txtRootPath.Text))
            {
                MessageBox.Show("Nơi lưu dữ liệu bị để trống hoặc không tồn tại.");
                return;
            }
            Config.volume_serial = serial!;
            Config.root_directory = txtRootPath.Text[3..];

            await Config.Save();
            Close();
        }
    }
}
