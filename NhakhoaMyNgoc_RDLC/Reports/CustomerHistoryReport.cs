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
    class CustomerHistoryReport : IReportTemplate
    {
        public string ReportPath => Path.Combine(Application.StartupPath, "Templates", "CustomerHistory.rdlc");

        public IEnumerable<ReportDataSource> GetDataSources(Dictionary<string, string> args)
        {
            var customer = Utilities.LoadFromJsonFile<CustomerDto>(args, "customer");
            var history = Utilities.LoadFromJsonFile<List<SummaryServiceDto>>(args, "history") ?? new List<SummaryServiceDto>();

            if (customer == null)
                throw new Exception("Không thể tải thông tin khách hàng");

            yield return new ReportDataSource("CustomerDto", new[] { customer });
            yield return new ReportDataSource("SummaryServiceDto", history);
        }
    }
}
