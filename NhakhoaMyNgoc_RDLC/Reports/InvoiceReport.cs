using Microsoft.Reporting.WinForms;
using NhakhoaMyNgoc_Connector.DTOs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NhakhoaMyNgoc_RDLC.Reports
{
    class InvoiceReport : IReportTemplate
    {
        public string ReportPath { get; set; }

        public IEnumerable<ReportDataSource> GetDataSources(Dictionary<string, string> args)
        {
            var customer = Utilities.LoadFromJsonFile<CustomerDto>(args, "customer");
            var invoice  = Utilities.LoadFromJsonFile<InvoiceDto>(args, "invoice");
            var services = Utilities.LoadFromJsonFile<List<SummaryServiceDto>>(args, "services") ?? new List<SummaryServiceDto>();

            args.TryGetValue("config", out string config);

            if (customer == null || invoice == null || services == null || config == null)
                throw new Exception("Không thể tải thông tin khách hàng");

            ReportPath = Path.Combine(config, "Templates", "Invoice.rdlc");

            yield return new ReportDataSource("CustomerDto", new[] { customer });
            yield return new ReportDataSource("InvoiceDto", new[] { invoice });
            yield return new ReportDataSource("SummaryServiceDto", services);
        }

        public IEnumerable<ReportParameter> GetParameters(Dictionary<string, string> args)
        {
            return new List<ReportParameter>();
        }
    }
}
