using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
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

        public bool RequireLogin()
        {
            return new LoginWindow().ShowDialog() == true;
        }

        [RelayCommand]
        void OpenTrashCan()
        {
            var vm = new TableEditorViewModel(_db, 1);
            vm.CustomerVM!.Customers = new(_db.Customers.Where(c => c.Deleted == 1));
            vm.InvoiceVM!.Invoices = new([.. _db.Invoices
                                .Include(i => i.Customer)
                                .Where(i => i.Deleted == 1)]);
            vm.IdnVM!.Idns = new([.. _db.Idns
                                .Where(i => i.Deleted == 1)]);

            var deletedProducts = _db.Products.Where(i => i.Deleted == 1).ToList();
            // Gán sự kiện tính Total cho mọi item
            var wrapped = deletedProducts.Select(i =>
            {
                var wrapper = new ProductWrapper(i);
                return wrapper;
            }).ToList();
            vm.ProductVM!.Products = new(wrapped);

            vm.CustomerVM.IsReadOnly
                = vm.InvoiceVM.IsReadOnly
                = vm.IdnVM.IsReadOnly
                = true;

            vm.SelectedTab = vm.Tabs.FirstOrDefault()!;
            new TableEditor() { DataContext = vm }.ShowDialog();
        }

        [RelayCommand]
        void OpenResources()
        {
            var vm = new TableEditorViewModel(_db, 2);
            

            vm.SelectedTab = vm.Tabs.FirstOrDefault()!;
            new TableEditor() { DataContext = vm }.ShowDialog();
        }
    }
}
