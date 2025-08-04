using NhakhoaMyNgoc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NhakhoaMyNgoc.ViewModels
{
    public class MainViewModel(DataContext db)
    {
        public AppViewModel AppVM { get; } = new(db);
        public CustomerViewModel CustomerVM { get; set; } = new(db);
        public InvoiceViewModel InvoiceVM { get; set; } = new(db);
        public ImageViewModel ImageVM { get; set; } = new(db);
        public IDNViewModel IdnVM { get; set; } = new(db);
        public ExpenseViewModel ExpenseVM { get; set; } = new(db);
    }
}
