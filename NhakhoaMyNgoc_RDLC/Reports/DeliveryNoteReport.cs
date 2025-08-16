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
    class DeliveryNoteReport : IReportTemplate
    {
        public string ReportPath { get; set; }

        public IEnumerable<ReportDataSource> GetDataSources(Dictionary<string, string> args)
        {
            var idn      = Utilities.LoadFromJsonFile<IdnDto>(args, "idn");
            var items    = Utilities.LoadFromJsonFile<List<IdnItemDto>>(args, "items");

            args.TryGetValue("config", out var config);

            if (config == null || idn == null || items == null)
                throw new Exception("Không thể tải thông tin đơn nhập");

            ReportPath = Path.Combine(config, "Templates", "DeliveryNote.rdlc");

            yield return new ReportDataSource("IdnDto", new[] { idn });
            yield return new ReportDataSource("IdnItemDto", items);
        }
    }
}
