using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NhakhoaMyNgoc.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NhakhoaMyNgoc.ViewModels
{
    public partial class IDNViewModel : ObservableObject
    {
        private readonly DataContext _db;

        [ObservableProperty]
        private ObservableCollection<Idn> idns = [];

        [ObservableProperty]
        private DateTime? fromDate = DateTime.Today;

        // DatePicker overwrite giá trị thành 12:00:00AM
        [ObservableProperty]
        private DateTime? toDate = DateTime.Today;

        [ObservableProperty]
        private Idn selectedIdn = new();

        [ObservableProperty]
        private ObservableCollection<Idnitem> idnItems = [];

        [ObservableProperty]
        private ObservableCollection<Product> products = [];

        public string Title => "Đơn nhập/xuất";

        public IDNViewModel(DataContext db)
        {
            _db = db;

            // load sẵn kho vào đây.
            Products = new([ .. _db.Products]);
        }

        [RelayCommand]
        void LoadIDNs()
        {
            // vì thế phải làm như này
            DateTime? to = ToDate?.Date.AddDays(1).AddTicks(-1);

            var result = _db.Idns.Where(i => i.Date >= FromDate &&
                                              i.Date <= to).ToList();
            
            Idns = new ObservableCollection<Idn>(result);
        }

        partial void OnSelectedIdnChanged(Idn value)
        {
            if (value is null) return;
            var items = _db.Idnitems.Where(i => i.IdnId == value.Id).ToList();
            IdnItems = new ObservableCollection<Idnitem>(items);
        }

        /// <summary>
        /// Hàm này chỉ có TableEditor được gọi.
        /// </summary>
        [RelayCommand]
        void Restore()
        {
            SelectedIdn.Deleted = 0;
            _db.SaveChanges();

            Idns.Remove(SelectedIdn);
            SelectedIdn = new();
        }
    }
}
