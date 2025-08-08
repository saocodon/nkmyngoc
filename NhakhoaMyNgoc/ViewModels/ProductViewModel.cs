using CommunityToolkit.Mvvm.Input;
using NhakhoaMyNgoc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace NhakhoaMyNgoc.ViewModels
{
    public partial class ProductViewModel(DataContext db) : IDNViewModel(db)
    {
        private readonly DataContext _db = db;
        public new static string Title => "Hàng hoá";

        protected override void Restore()
        {
            if (Products is null) return;

            SelectedProduct.Deleted = 0;
            _db.SaveChanges();

            // cho TableEditor
            Products.Remove(SelectedProduct);
            SelectedProduct = new();
        }
    }
}
