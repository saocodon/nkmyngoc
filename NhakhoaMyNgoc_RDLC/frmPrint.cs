using Microsoft.Reporting.WinForms;
using Newtonsoft.Json;
using NhakhoaMyNgoc_Connector;
using NhakhoaMyNgoc_Connector.DTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace NhakhoaMyNgoc_RDLC
{
    public partial class FrmPrint : Form
    {
        public FrmPrint()
        {
            InitializeComponent();
        }

        private void FrmPrint_Load(object sender, EventArgs e)
        {
            var args = Environment.GetCommandLineArgs();
            var parsedArgs = Utilities.ParseArgs(args);

            if (!parsedArgs.TryGetValue("report", out var reportName))
            {
                MessageBox.Show("Thiếu tham số 'report'");
                return;
            }

            var reportTemplate = ReportRegistry.Resolve(reportName);
            if (reportTemplate == null)
            {
                MessageBox.Show($"Không hỗ trợ report '{reportName}'");
                return;
            }

            try
            {
                var dataSources = reportTemplate.GetDataSources(parsedArgs);
                reportViewer.LocalReport.DataSources.Clear();
                foreach (var ds in dataSources)
                    reportViewer.LocalReport.DataSources.Add(ds);
                var parameters = reportTemplate.GetParameters(parsedArgs);
                reportViewer.LocalReport.ReportPath = reportTemplate.ReportPath;
                reportViewer.LocalReport.SetParameters(parameters);
                reportViewer.RefreshReport();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi: {ex.Message}");
            }
        }
    }
}
