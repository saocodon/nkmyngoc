using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using NhakhoaMyNgoc.Models;
using NhakhoaMyNgoc.Windows;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NhakhoaMyNgoc.ViewModels
{
    public partial class AppViewModel : ObservableObject
    {
        private readonly DataContext _db;

        // bỏ phần revision trong version
        public AppViewModel(DataContext db)
        {
            _db = db;
        }

        [RelayCommand]
        static void OpenAbout() => new AboutWindow().ShowDialog();

        [RelayCommand]
        static void OpenSettings()
        {
            var login = new LoginWindow();
            if (login.ShowDialog() == true)
                new SettingsWindow().ShowDialog();
        }

        [RelayCommand]
        void OpenTableEditor(string key)
        {
            var vm = new TableEditorViewModel(_db);
            vm.CustomerVM.Customers = new(_db.Customers.Where(c => c.Deleted == 1));
            vm.ImageVM.Records = new([.. _db.Images
                                .Include(img => img.Customer) // eager loading để tăng tốc độ
                                .Where(img => img.Deleted == 1)]);
            vm.SelectedTab = vm.Tabs.FirstOrDefault()!;
            new TableEditor() { DataContext = vm }.ShowDialog();
        }
    }
}
