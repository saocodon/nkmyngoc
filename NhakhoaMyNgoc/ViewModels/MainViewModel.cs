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
        public CustomerViewModel CustomerVM { get; } = new(db);
        public InvoiceViewModel InvoiceVM { get; } = new(db);
        public ImageViewModel ImageVM { get; } = new(db);
        public IDNViewModel IdnVM { get; } = new(db);
    }
}
