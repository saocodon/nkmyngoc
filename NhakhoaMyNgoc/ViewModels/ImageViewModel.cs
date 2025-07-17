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
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace NhakhoaMyNgoc.ViewModels
{
    public class ImageItem
    {
        public BitmapImage Image { get; set; } = new();
        public string Note { get; set; } = string.Empty;
    }

    public partial class ImageViewModel : ObservableObject
    {
        private readonly DataContext _db;
        private ObservableCollection<Image> records = [];

        [ObservableProperty]
        private ObservableCollection<ImageItem> images = [];

        public ImageViewModel(DataContext db)
        {
            _db = db;

            Messenger.Subscribe("CustomerSelected", data =>
            {
                Debug.WriteLine("ImageViewModel received");
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
                Image img = new Image()
                {
                    CustomerId = customer.Id,
                    Deleted = 0,
                    Note = filename,
                    Path = filename + extension
                };
                _db.Images.Add(img);

                // Load lên UI
                Images.Add(new ImageItem()
                {
                    Image = IOUtil.LoadImage(destination),
                    Note = img.Note
                });
            }
            _db.SaveChanges();
        }

        void FindCustomerImages(Customer customer)
        {
            var result = (from i in _db.Images
                          where i.CustomerId == customer.Id && i.Deleted == 0
                          select i).ToList();
            records = new ObservableCollection<Image>(result);
            Images.Clear();
            foreach (Image image in records)
            {
                string partialPath = Path.Combine("Images", customer.Id.ToString(), image.Path);
                string fullPath = Path.Combine(Config.full_path, partialPath);
                BitmapImage img = IOUtil.LoadImage(fullPath);
                Images.Add(new ImageItem { Image = img, Note = image.Note ?? string.Empty });
            }
        }
    }
}
