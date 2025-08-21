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

        public int IncomeSum => Expenses
            .Where(e => e.Input == 1)
            .Sum(e => e.Amount);

        public int OutcomeSum => Expenses
            .Where(e => e.Input == 0)
            .Sum(e => e.Amount);

        public int RemainingSum => IncomeSum - OutcomeSum;

        [RelayCommand]
        void LoadExpenses()
        {
            // vì thế phải làm như này
            DateTime to = ToDate.Date.AddDays(1).AddTicks(-1);

            var result = _db.Expenses.Where(i => i.Date >= FromDate &&
                                              i.Date <= to &&
                                              i.Deleted == 0).ToList();

            Expenses = new ObservableCollection<Expense>(result);
            UpdateFigures();
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
            var dtoPath = IOUtil.WriteJsonToTempFile(dto, $"{Guid.NewGuid()}.json");
            // TODO: cái này phải thay đổi khi đóng gói
            Process.Start(new ProcessStartInfo()
            {
                FileName = @"NhakhoaMyNgoc_RDLC.exe",
                Arguments = $"--report expense --expense {dtoPath} --config {Config.full_path}"
            });
        }

        [RelayCommand]
        void StartAddNew()
        {
            SelectedExpense = new()
            {
                Deleted = 0,
                Date = DateTime.Now,
                Input = 1,
                Participant = "Chưa rõ",
                Address = "Chưa rõ",
                Content = "Chưa rõ",
                Amount = 0,
                CertificateId = -1
            };
            Expenses.Add(SelectedExpense);
        }

        [RelayCommand]
        void Save()
        {
            if (SelectedExpense == null) return;

            if (SelectedExpense.Id == 0)
            {
                _db.Expenses.Add(SelectedExpense);
                Expenses.Add(SelectedExpense);

                _db.SaveChanges();
            }
            else
            {
                _db.Expenses.Update(SelectedExpense);
                _db.SaveChanges();
            }
            UpdateFigures();
        }

        [RelayCommand]
        void Delete()
        {
            if (SelectedExpense == null) return;
            
            if (SelectedExpense.Id != 0)
            {
                _db.Expenses.Remove(SelectedExpense);
                _db.SaveChanges();
            }
            Expenses.Remove(SelectedExpense);
            UpdateFigures();
        }

        void UpdateFigures()
        {
            OnPropertyChanged(nameof(IncomeSum));
            OnPropertyChanged(nameof(OutcomeSum));
            OnPropertyChanged(nameof(RemainingSum));
        }
    }
}
