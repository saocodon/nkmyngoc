using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using NhakhoaMyNgoc.Models;
using NhakhoaMyNgoc.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace NhakhoaMyNgoc.ViewModels
{
    public partial class ImageItem : ObservableObject
    {
        public BitmapImage Image { get; set; } = new();
        public string Note { get; set; } = string.Empty;

        public ICommand? DeleteCommand { get; set; }
    }

    public partial class ImageViewModel : ObservableObject
    {
        private readonly DataContext _db;

        [ObservableProperty]
        private ObservableCollection<Image> records = [];

        [ObservableProperty]
        private ObservableCollection<ImageItem> images = [];

        [ObservableProperty]
        private Image selectedRecord = new();

        public string Title => "Hình ảnh";

        public ImageViewModel(DataContext db)
        {
            _db = db;

            Messenger.Subscribe("OnSelectedCustomerChanged", data =>
            {
                if (data is object[] args &&
                    args.Length == 1 &&
                    args[0] is Customer customer)
                    FindCustomerImages(customer);
            });

            Messenger.Subscribe("AddCustomerImage", data =>
            {
                if (data is object[] args &&
                    args.Length == 2 &&
                    args[0] is Customer customer &&
                    args[1] is string[] fileNames)
                    AddCustomerImage(customer, fileNames);
            });

            // Lưu note của ảnh
            Messenger.Subscribe("SaveCustomer", data =>
            {
                // records.Length = Images.Count
                for (int i = 0; i < Images.Count; i++)
                    records[i].Note = Images[i].Note;

                _db.SaveChanges();
            });
        }

        void CreateListViewItem(Image record, BitmapImage image)
        {
            ImageItem? item = null;
            item = new ImageItem
            {
                Image = image,
                Note = record.Note ?? string.Empty,
                DeleteCommand = new RelayCommand(() =>
                {
                    record.Deleted = 1;
                    Images.Remove(item!);
                    _db.SaveChanges();
                })
            };
            Images.Add(item);
        }

        void AddCustomerImage(Customer customer, string[] paths)
        {
            foreach (string path in paths)
            {
                string destFolder = Path.Combine(Config.full_path, "Images", customer.Id.ToString());
                string tempDesc = Path.GetFileNameWithoutExtension(path);
                string filename = (Directory.GetFiles(destFolder).Length + 1).ToString();
                string extension = Path.GetExtension(path);

                // Kiểm tra nếu khách hàng chưa có thư mục ảnh
                Directory.CreateDirectory(destFolder);

                // Cất vào data
                string destination = Path.Combine(destFolder, filename + extension);
                File.Copy(path, destination);

                // Lưu vào database
                Image img = new()
                {
                    CustomerId = customer.Id,
                    Deleted = 0,
                    Note = tempDesc,
                    Path = filename + extension
                };
                Records.Add(img);
                _db.Images.Add(img);

                // Load lên UI
                CreateListViewItem(img, IOUtil.LoadImage(destination));
            }
            _db.SaveChanges();
        }

        void FindCustomerImages(Customer customer)
        {
            var result = (from i in _db.Images
                          where i.CustomerId == customer.Id && i.Deleted == 0
                          select i).ToList();
            Records = new ObservableCollection<Image>(result);
            Images.Clear();
            foreach (Image image in Records)
            {
                string partialPath = Path.Combine("Images", customer.Id.ToString(), image.Path);
                string fullPath = Path.Combine(Config.full_path, partialPath);
                BitmapImage img = IOUtil.LoadImage(fullPath);
                CreateListViewItem(image, img);
            }
        }

        /// <summary>
        /// Hàm này chỉ có TableEditor được gọi.
        /// </summary>
        [RelayCommand]
        void RestoreImage()
        {
            SelectedRecord.Deleted = 0;
            _db.SaveChanges();

            // cho TableEditor
            Records.Remove(SelectedRecord);
            SelectedRecord = new();
        }
    }
}
