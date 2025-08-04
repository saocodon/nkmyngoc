using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NhakhoaMyNgoc.Models;
using NhakhoaMyNgoc.Utilities;
using NhakhoaMyNgoc_Connector.DTOs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NhakhoaMyNgoc.ViewModels
{
    public partial class ExpenseViewModel(DataContext db) : ObservableObject
    {
        private readonly DataContext _db = db;

        [ObservableProperty]
        private DateTime fromDate = DateTime.Now;

        [ObservableProperty]
        private DateTime toDate = DateTime.Now;

        [ObservableProperty]
        private ObservableCollection<Expense> expenses = [];

        [ObservableProperty]
        private Expense? selectedExpense;

        [RelayCommand]
        void LoadExpenses()
        {
            // vì thế phải làm như này
            DateTime to = ToDate.Date.AddDays(1).AddTicks(-1);

            var result = _db.Expenses.Where(i => i.Date >= FromDate &&
                                              i.Date <= to &&
                                              i.Deleted == 0).ToList();

            Expenses = new ObservableCollection<Expense>(result);
        }

        [RelayCommand]
        void Print()
        {
            if (SelectedExpense == null) return;

            var dto = new ExpenseDto
            {
                Address = SelectedExpense.Address,
                Amount = SelectedExpense.Amount,
                AmountInWords = NumberToWords.DocTienBangChu(SelectedExpense.Amount),
                CertificateId = SelectedExpense.CertificateId,
                Content = SelectedExpense.Content,
                Date = SelectedExpense.Date,
                Id = SelectedExpense.Id,
                Input = SelectedExpense.Input,
                Participant = SelectedExpense.Participant
            };
            var dtoPath = IOUtil.WriteJsonToTempFile(dto, $"Expense{dto.Id}.json");
            // TODO: cái này phải thay đổi khi đóng gói
            Process.Start(new ProcessStartInfo()
            {
                FileName = @"..\..\..\..\NhakhoaMyNgoc_RDLC\bin\Debug\NhakhoaMyNgoc_RDLC.exe",
                Arguments = $"--report expense --expense {dtoPath}"
            });
        }
    }
}
