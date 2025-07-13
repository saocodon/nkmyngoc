using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NhakhoaMyNgoc.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NhakhoaMyNgoc.ViewModels
{
    public partial class AppViewModel : ObservableObject
    {
        // bỏ phần revision trong version
        public string AppVersion =>
            "v" + string.Join(".", Assembly.GetExecutingAssembly()
                                    .GetName().Version!.ToString()
                                    .Split('.').Take(3));

        [RelayCommand]
        static void OpenAbout() => new AboutWindow().ShowDialog();

        [RelayCommand]
        static void OpenSettings() => new SettingsWindow().ShowDialog();
    }
}
