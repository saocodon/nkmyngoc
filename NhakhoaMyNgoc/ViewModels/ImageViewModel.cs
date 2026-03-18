using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.AspNetCore.SignalR.Client;
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
        public BitmapImage? Image { get; set; }
        //public long CustomerId { get; set; }
        //public string Path { get; set; } = string.Empty;

        //[ObservableProperty]
        //private string note = string.Empty;
        public Image? Record { get; set; }

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
        private ImageItem selectedImage = new();

        public static string Title => "Hình ảnh";

        public S3 storage;

        public ImageViewModel(DataContext db, HubConnection syncConn)
        {
            _db = db;
            storage = new S3(
                "http://localhost:9000",
                "minioadmin",
                "minioadmin",
                "images"
            );

            WeakReferenceMessenger.Default.Register<SelectedCustomerChangedMessage>(this, async (r, m) =>
            {
                await FindCustomerImages(m.Value);
            });

            WeakReferenceMessenger.Default.Register<AddCustomerImageMessage>(this, async (r, m) =>
            {
                var (customer, fileNames) = m.Value;
                await AddCustomerImage(customer, fileNames);
            });

            WeakReferenceMessenger.Default.Register<SaveCustomerMessage>(this, (r, m) =>
            {
                foreach (var item in Images)
                {
                    var record = Records.FirstOrDefault(r => r.Path == item.Record!.Path);
                    if (record != null)
                        record.Note = item.Record!.Note;
                }

                _db.SaveChanges();
            });
        }

        [RelayCommand]
        void OpenImage()
        {
            MessageBox.Show(SelectedImage.Record!.Note);
            Process.Start(new ProcessStartInfo()
            {
                FileName = Path.Combine(
             Path.GetTempPath(),
             "NhakhoaMyNgoc",
             SelectedImage.Record.Path),
                UseShellExecute = true
            });
        }

        void CreateListViewItem(Image record, BitmapImage image)
        {
            ImageItem? item = null;
            item = new ImageItem
            {
                Image = image,
                Record = record,
                DeleteCommand = new RelayCommand(async () =>
                {
                    await storage.DeleteAsync(record.Path);
                    _db.Images.Remove(record);
                    Images.Remove(item!);
                    _db.SaveChanges();
                })
            };
            Images.Add(item);
        }

        async Task AddCustomerImage(Customer customer, string[] paths)
        {
            foreach (string path in paths)
            {
                string tempDesc = Path.GetFileNameWithoutExtension(path);
                string filename = Guid.NewGuid().ToString() + Path.GetExtension(path);

                // Cất vào data
                string imageUrl = "";
                using (var imgStream = File.OpenRead(path))
                    imageUrl = await storage.UploadAsync(imgStream, filename, "image/jpeg");

                // Lưu vào database
                Image img = new()
                {
                    CustomerId = customer.Id,
                    Deleted = false,
                    Note = tempDesc,
                    Path = filename
                };
                Records.Add(img);
                _db.Images.Add(img);

                // Load lên UI
                CreateListViewItem(img, await IOUtil.LoadOnlineImageAsync(storage, filename));
            }
            _db.SaveChanges();
        }

        async Task FindCustomerImages(Customer customer)
        {
            if (customer != null)
            {
                var result = (from i in _db.Images
                              where i.CustomerId == customer.Id && i.Deleted == false
                              select i).ToList();
                Records = new ObservableCollection<Image>(result);
                var tasks = Records.Select(async image =>
                {
                    var img = await IOUtil.LoadOnlineImageAsync(storage, image.Path);
                    return (image, img);
                });

                var results = await Task.WhenAll(tasks);

                Images.Clear();
                foreach (var (image, img) in results)
                {
                    CreateListViewItem(image, img);
                }
            }
        }
    }
}
