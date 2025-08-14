using NhakhoaMyNgoc.Models;
using NhakhoaMyNgoc.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NhakhoaMyNgoc.ViewModels
{
    public class MainViewModel
    {
        public AppViewModel AppVM { get; }
        public CustomerViewModel CustomerVM { get; }
        public InvoiceViewModel InvoiceVM { get; }
        public ImageViewModel ImageVM { get; }
        public ProductService ProductSvc { get; }
        public IDNViewModel IdnVM { get; }
        public ProductViewModel ProductVM { get; }
        public ExpenseViewModel ExpenseVM { get; }

        public MainViewModel(DataContext db)
        {
            AppVM = new(db);
            CustomerVM = new(db);
            InvoiceVM = new(db);
            ImageVM = new(db);

            ProductSvc = new ProductService(db);

            IdnVM = new(db, ProductSvc);
            ProductVM = new(ProductSvc, loadDeleted: false);

            ExpenseVM = new(db);
        }
    }

}
