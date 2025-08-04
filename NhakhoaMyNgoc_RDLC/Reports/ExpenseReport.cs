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
        public string ReportPath => Path.Combine(Application.StartupPath, "Templates", "Expense.rdlc");

        public IEnumerable<ReportDataSource> GetDataSources(Dictionary<string, string> args)
        {
            var expense = Utilities.LoadFromJsonFile<ExpenseDto>(args, "expense");

            if (expense == null)
                throw new Exception("Không thể tải thông tin đơn");

            yield return new ReportDataSource("ExpenseDto", new[] { expense });
        }
    }
}
