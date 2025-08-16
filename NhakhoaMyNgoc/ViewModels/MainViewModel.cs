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
        public ServiceViewModel ServiceVM { get; }
        public ImageViewModel ImageVM { get; }
        public ProductService ProductSvc { get; }
        public IDNViewModel IdnVM { get; }
        public ProductViewModel ProductVM { get; }
        public ExpenseViewModel ExpenseVM { get; }

        public MainViewModel(DataContext db)
        {
            CustomerVM = new(db);
            ServiceVM = new(db, loadDeleted: false);
            InvoiceVM = new(db, ServiceVM.Services);
            ImageVM = new(db);

            ProductSvc = new ProductService(db);
            AppVM = new(db, ProductSvc);

            IdnVM = new(db, ProductSvc);
            ProductVM = new(ProductSvc, loadDeleted: false)
                { IsReadOnly = true };

            ExpenseVM = new(db);
        }
    }

}
