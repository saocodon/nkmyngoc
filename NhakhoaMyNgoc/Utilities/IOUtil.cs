using System;
using System.IO;
using System.Management;
using System.Reflection;
using System.Text.Json;
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

        public static string WriteJsonToTempFile<T>(T obj, string fileName)
        {
            var path = Path.Combine(Path.GetTempPath(), fileName);
            File.WriteAllText(path, JsonSerializer.Serialize(obj));
            return path;
        }

        private static object? GetDefaultValue(Type type)
        {
            // Nullable types -> unwrap để lấy underlying type
            Type? underlying = Nullable.GetUnderlyingType(type);
            if (underlying != null)
                type = underlying;

            // Value types: return Activator.CreateInstance (0, false, v.v.)
            if (type.IsValueType)
                return Activator.CreateInstance(type);

            // Reference types: try to new instance (List<>, Dictionary<>, etc.)
            try
            {
                return Activator.CreateInstance(type);
            }
            catch
            {
                return null; // không thể tạo
            }
        }

        public static object? ConvertJsonElement(JsonElement element, Type targetType)
        {
            try
            {
                if (targetType == typeof(string)) return element.GetString();
                if (targetType == typeof(int)) return element.GetInt32();
                if (targetType == typeof(bool)) return element.GetBoolean();
                if (targetType == typeof(double)) return element.GetDouble();
                if (targetType == typeof(List<string>))
                {
                    var list = new List<string>();
                    foreach (var item in element.EnumerateArray())
                        list.Add(item.GetString()!);
                    return list;
                }
                if (targetType == typeof(Dictionary<string, object>))
                {
                    if (element.ValueKind != JsonValueKind.Object) return null;
                    var dict = new Dictionary<string, object>();
                    foreach (var prop in element.EnumerateObject())
                        dict[prop.Name] = ConvertJsonElement(prop.Value, typeof(object))!;
                    return dict;
                }
                // Add more types as needed
            }
            catch { }

            return GetDefaultValue(targetType);
        }

        public static long GetDirectorySize(string path)
        {
            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException($"Không tìm thấy thư mục: {path}");

            long size = 0;

            // Cộng dung lượng tất cả file trong thư mục
            string[] files = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
            foreach (string file in files)
            {
                try
                {
                    FileInfo info = new FileInfo(file);
                    size += info.Length;
                }
                catch
                {
                    // Bỏ qua file nếu bị lỗi truy cập
                }
            }

            return size;
        }

        public static string FormatSize(long sizeInBytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            double len = sizeInBytes;
            int order = 0;
            while (len >= 1024 && order < sizes.Length - 1)
            {
                order++;
                len /= 1024;
            }
            return $"{len:0.##} {sizes[order]}";
        }
    }
}
