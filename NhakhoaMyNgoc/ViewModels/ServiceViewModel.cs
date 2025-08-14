using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NhakhoaMyNgoc.Models;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using NhakhoaMyNgoc.ModelWrappers;
using static NhakhoaMyNgoc.ViewModels.AppViewModel;

namespace NhakhoaMyNgoc.ViewModels
{
    public partial class ServiceViewModel : ObservableObject
    {
        [ObservableProperty]
        private ViewMode mode;

        private readonly DataContext _db;

        [ObservableProperty]
        private ObservableCollection<Service> services;

        [ObservableProperty]
        private Service selectedService = new();

        [ObservableProperty]
        private bool isReadOnly;

        public static string Title => "Bảng dịch vụ";

        public ServiceViewModel(DataContext db, bool loadDeleted = false)
        {
            _db = db;
            Services = new ObservableCollection<Service>(
                _db.Services.Where(s => s.Deleted == (loadDeleted ? 1 : 0))
            );
        }

        [RelayCommand]
        private void StartAddNew()
        {
            SelectedService = new()
            {
                Name = "Dịch vụ mới",
                Price = 0,
                Deleted = 0,
            };
            Services.Add(SelectedService);
        }

        [RelayCommand]
        private void Save()
        {
            if (SelectedService != null)
            {
                _db.Services.Update(SelectedService);
                _db.SaveChanges();
            }
        }

        [RelayCommand]
        private void Delete()
        {
            if (SelectedService != null)
            {
                SelectedService.Deleted = 1;
                _db.Services.Update(SelectedService);
                _db.SaveChanges();
                Services.Remove(SelectedService);
            }
        }

        [RelayCommand]
        private void Restore()
        {
            if (SelectedService != null)
            {
                SelectedService.Deleted = 0;
                _db.Services.Update(SelectedService);
                _db.SaveChanges();
                Services.Remove(SelectedService); // xóa khỏi danh sách deleted
            }
        }
    }
}
