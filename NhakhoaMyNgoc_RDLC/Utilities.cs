using Microsoft.Reporting.WinForms;
using Newtonsoft.Json;
using NhakhoaMyNgoc_RDLC.Reports;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NhakhoaMyNgoc_RDLC
{
    public class Utilities
    {
        public static Dictionary<string, string> ParseArgs(string[] args)
        {
            var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];

                // Nếu bắt đầu bằng "--" hoặc "-"
                if (arg.StartsWith("--") || arg.StartsWith("-"))
                {
                    string key = arg.TrimStart('-');

                    // Nếu còn giá trị phía sau
                    if (i + 1 < args.Length && !args[i + 1].StartsWith("-"))
                    {
                        result[key] = args[i + 1];
                        i++; // Skip value in next iteration
                    }
                    else
                    {
                        // Nếu không có giá trị phía sau, gán true (flag dạng bool)
                        result[key] = "true";
                    }
                }
            }

            return result;
        }

        public static T LoadFromJsonFile<T>(Dictionary<string, string> args, string key)
        {
            if (!args.TryGetValue(key, out var path) || string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            {
                return default;
            }

            try
            {
                var json = File.ReadAllText(path);
                return JsonConvert.DeserializeObject<T>(json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi đọc file {key}: {ex.Message}");
                return default;
            }
        }
    }

    interface IReportTemplate
    {
        string ReportPath { get; }
        IEnumerable<ReportDataSource> GetDataSources(Dictionary<string, string> args);
    }

    static class ReportRegistry
    {
        private static readonly Dictionary<string, IReportTemplate> reports = new Dictionary<string, IReportTemplate>
        {
            { "customer-history", new CustomerHistoryReport() },
            // { "invoice", new InvoiceReport() }, ...
        };

        public static IReportTemplate Resolve(string key)
        {
            reports.TryGetValue(key, out var report);
            return report;
        }
    }
}
