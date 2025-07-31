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
        public string ReportPath => Path.Combine(Application.StartupPath, "Templates", "DeliveryNote.rdlc");

        public IEnumerable<ReportDataSource> GetDataSources(Dictionary<string, string> args)
        {
            var idn      = Utilities.LoadFromJsonFile<IdnDto>(args, "idn");
            var items    = Utilities.LoadFromJsonFile<List<IdnItemDto>>(args, "items");

            if (idn == null)
                throw new Exception("Không thể tải thông tin đơn nhập");

            yield return new ReportDataSource("IdnDto", new[] { idn });
            yield return new ReportDataSource("IdnItemDto", items);
        }
    }
}
