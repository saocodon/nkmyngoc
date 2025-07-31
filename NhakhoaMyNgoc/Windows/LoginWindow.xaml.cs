using NhakhoaMyNgoc.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
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
using System.Windows.Threading;

namespace NhakhoaMyNgoc.Windows
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        List<string> q;
        int q1_id, q2_id;

        DispatcherTimer timer;
        int failed_attempts = 0;
        int remaining_time;

        public LoginWindow()
        {
            InitializeComponent();

            q = [.. Config.security_answers.Keys];
            q1_id = q[0][1] - '0';
            q2_id = q[1][1] - '0';

            lblQ1.Text = Config.security_questions[q1_id];
            lblQ2.Text = Config.security_questions[q2_id];

            failed_attempts = Config.failed_login_streak;
            remaining_time = Config.remaining_time;

            timer = new()
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            timer.Tick += Timer_Tick;

            if (remaining_time <= 0)
                btnOK.IsEnabled = true;
            else
            {
                lblStatus.Text = $"Đã nhập sai {failed_attempts} lần liên tiếp, thử lại sau.";
                btnOK.IsEnabled = false;
                timer.Start();
            }
        }

        private void Timer_Tick(object? sender, EventArgs e)
        {
            if (remaining_time > 0)
            {
                remaining_time--;
                TimeSpan t = TimeSpan.FromSeconds(remaining_time);
                lblStatus.Visibility = Visibility.Visible;
                lblStatus.Text = $"{t.Days * 24 + t.Hours:D2}:{t.Minutes:D2}:{t.Seconds:D2}";
            }
            else
            {
                btnOK.IsEnabled = true;
                lblStatus.Visibility = Visibility.Hidden;
                timer.Stop();
            }
        }

        private void ToggleForgot(object? sender, RoutedEventArgs e)
        {
            bool isCollapsed = ForgotPanel.Visibility == Visibility.Collapsed;

            ForgotPanel.Visibility = isCollapsed ? Visibility.Visible : Visibility.Collapsed;
            btnForgot.Content = isCollapsed ? "Không cần nữa" : "Quên mật khẩu";
        }

        private void ValidatePassword(object? sender, RoutedEventArgs e)
        {
            bool isValid = PBKDF2.VerifyPassword(txtPassword.Password, Config.admin_password, Config.admin_password_salt);
            bool isValid1 = PBKDF2.VerifyPassword(txtQ1.Text, Config.security_answers[q[0]], Config.security_salts[q[0]]);
            bool isValid2 = PBKDF2.VerifyPassword(txtQ2.Text, Config.security_answers[q[1]], Config.security_salts[q[1]]);
            if (isValid || (isValid1 && isValid2))
            {
                failed_attempts = 0;
                DialogResult = true;
                Close();
            }
            else
            {
                failed_attempts++;
                if (failed_attempts % 5 != 0)
                {
                    lblStatus.Visibility = Visibility.Visible;
                    lblStatus.Text = "Thông tin đăng nhập sai.";
                    btnOK.IsEnabled = true;
                }
                else
                {
                    lblStatus.Visibility = Visibility.Visible;
                    lblStatus.Text = $"Đã nhập sai {failed_attempts} lần liên tiếp, thử lại sau.";
                    remaining_time = Convert.ToInt32(30 * Math.Pow(2, failed_attempts / 5 - 1));
                    btnOK.IsEnabled = false;
                }
            }
        }

        private async void WindowClosing(object? sender, CancelEventArgs e)
        {
            Config.failed_login_streak = failed_attempts;
            Config.remaining_time = remaining_time;
            await Config.Save();
        }
    }
}
