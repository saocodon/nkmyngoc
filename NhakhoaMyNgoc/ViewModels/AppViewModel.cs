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
        void OpenAbout() => new AboutWindow().ShowDialog();

        [RelayCommand]
        void OpenSettings() => new SettingsWindow().ShowDialog();

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
            if (key == "ImageRecycleBin")
            {
                TableEditorViewModel<Image> vm = new() { Title = "Ảnh đã xoá" };
                ImageViewModel currentVM = new(_db)
                {
                    Records = new(_db.Images
                                .Include(img => img.Customer) // lazy loading
                                .Where(img => img.Deleted == 1)
                                .ToList())
                };
                vm.CurrentVM = currentVM;
                new TableEditor() { DataContext = vm }.ShowDialog();
            }
        }
    }
}
