using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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
        public static string AppVersion =>
            "v" + string.Join(".", Assembly.GetExecutingAssembly()
                                    .GetName().Version!.ToString()
                                    .Split('.').Take(3));

        public AppViewModel(DataContext db)
            { _db = db; }

        [RelayCommand]
        static void OpenAbout() => new AboutWindow().ShowDialog();

        [RelayCommand]
        static void OpenSettings() => new SettingsWindow().ShowDialog();

        [RelayCommand]
        void OpenTableEditor(string key)
        {
            if (key == "CustomerRecycleBin")
            {
                var vm = new TableEditorViewModel<Customer>()
                {
                    CurrentVM = new CustomerViewModel(_db)
                        { Customers = new(_db.Customers.Where(c => c.Deleted == 1)) },
                    Title = "Khách hàng đã xoá"
                };
                new TableEditor() { DataContext = vm }.ShowDialog();
            }
        }
    }
}
