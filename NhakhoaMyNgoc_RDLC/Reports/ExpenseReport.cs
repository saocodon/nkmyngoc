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
    class ExpenseReport : IReportTemplate
    {
        public string ReportPath { get; set; }

        public IEnumerable<ReportDataSource> GetDataSources(Dictionary<string, string> args)
        {
            var expense = Utilities.LoadFromJsonFile<ExpenseDto>(args, "expense");
            args.TryGetValue("config", out string config);

            if (expense == null || config == null)
                throw new Exception("Không thể tải thông tin đơn");

            ReportPath = Path.Combine(config, "Templates", "Expense.rdlc");

            yield return new ReportDataSource("ExpenseDto", new[] { expense });
        }
    }
}
