using System;
using System.IO;
using System.Management;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace NhakhoaMyNgoc.Utilities
{
    public static class IOUtil
    {
        public static void CopyDirectory(string sourceDir, string destinationDir)
        {
            if (!Directory.Exists(sourceDir))
                throw new DirectoryNotFoundException($"Thư mục nguồn không tồn tại: {sourceDir}");

            // Tạo thư mục đích nếu chưa có
            Directory.CreateDirectory(destinationDir);

            // Copy tất cả các file
            foreach (string file in Directory.GetFiles(sourceDir))
            {
                string destFile = Path.Combine(destinationDir, Path.GetFileName(file));
                if (!File.Exists(destFile))
                    File.Copy(file, destFile);
            }

            // Đệ quy copy các thư mục con
            foreach (string subDir in Directory.GetDirectories(sourceDir))
            {
                string destSubDir = Path.Combine(destinationDir, Path.GetFileName(subDir));
                CopyDirectory(subDir, destSubDir);
            }
        }

        public static DriveInfo FindDriveLetter()
        {
            foreach (var drive in DriveInfo.GetDrives())
            {
                string? serial = GetVolumeSerial(drive.Name);
                if (serial == Config.volume_serial)
                    return drive;
            }
            return null!;
        }

        public static string? GetVolumeSerial(string driveLetter)
        {
            driveLetter = driveLetter.TrimEnd('\\');
            var searcher = new ManagementObjectSearcher($"SELECT VolumeSerialNumber FROM Win32_LogicalDisk WHERE DeviceID = '{driveLetter}'");
            foreach (var disk in searcher.Get())
                return disk["VolumeSerialNumber"]?.ToString();

            return null!;
        }

        public static BitmapImage LoadImage(string path)
        {
            var bitmap = new BitmapImage();
            using var stream = File.OpenRead(path);

            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.StreamSource = stream;
            bitmap.EndInit();
            bitmap.Freeze(); // an toàn thread, không bị lock
            return bitmap;
        }
    }
}
