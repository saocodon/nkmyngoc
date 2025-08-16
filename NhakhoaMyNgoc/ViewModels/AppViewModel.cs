using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using NhakhoaMyNgoc.Interfaces;
using NhakhoaMyNgoc.Models;
using NhakhoaMyNgoc.ModelWrappers;
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

        private IProductService productService;

        // bỏ phần revision trong version
        public AppViewModel(DataContext db, IProductService ps)
        {
            _db = db;
            productService = ps;
        }

        public enum ViewMode
        {
            Manage,
            Restore
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
        void OpenTrashCan()
        {
            var vm = new TableEditorViewModel(_db, 1, productService);
            vm.SelectedTab = vm.Tabs.FirstOrDefault()!;
            new TableEditor() { DataContext = vm }.ShowDialog();
        }

        [RelayCommand]
        void OpenResources()
        {
            var vm = new TableEditorViewModel(_db, 2, productService);
            vm.SelectedTab = vm.Tabs.FirstOrDefault()!;
            new TableEditor() { DataContext = vm }.ShowDialog();
        }
    }
}
