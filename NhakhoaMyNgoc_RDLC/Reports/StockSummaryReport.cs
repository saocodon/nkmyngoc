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
    class StockSummaryReport : IReportTemplate
    {
        public string ReportPath { get; set; }

        public IEnumerable<ReportDataSource> GetDataSources(Dictionary<string, string> args)
        {
            var summary = Utilities.LoadFromJsonFile<Dictionary<int, StockSummaryDto>>(args, "summary") ?? new Dictionary<int, StockSummaryDto>();

            args.TryGetValue("config", out string config);

            if (summary == null || config == null)
                throw new Exception("Không thể tải thông tin đơn");

            ReportPath = Path.Combine(config, "Templates", "StockSummary.rdlc");

            yield return new ReportDataSource("StockSummaryDto", summary.Values.ToList());
        }

        public IEnumerable<ReportParameter> GetParameters(Dictionary<string, string> args)
        {
            args.TryGetValue("from", out string fromDateString);
            args.TryGetValue("to", out string toDateString);
            args.TryGetValue("total", out string totalString);

            yield return new ReportParameter("FromDate", fromDateString);
            yield return new ReportParameter("ToDate", toDateString);
            yield return new ReportParameter("Total", totalString);
        }
    }
}
