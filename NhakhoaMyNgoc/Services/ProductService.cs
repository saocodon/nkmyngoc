using NhakhoaMyNgoc.Interfaces;
using NhakhoaMyNgoc.Models;
using NhakhoaMyNgoc.ModelWrappers;
using System.Collections.ObjectModel;
using System.Linq;

namespace NhakhoaMyNgoc.Services
{
    public class ProductService : IProductService
    {
        private readonly DataContext _db;

        public ProductService(DataContext db)
        {
            _db = db;
        }

        public ObservableCollection<ProductWrapper> GetAllProducts()
        {
            var productsList = _db.Products
                                  .Where(p => p.Deleted == 0)
                                  .ToList();

            return new ObservableCollection<ProductWrapper>(
                productsList.Select(p => new ProductWrapper(p))
            );
        }

        public void SaveProduct(ProductWrapper product)
        {
            if (product.Id == 0)
                _db.Products.Add(product.Model);
            else
                _db.Products.Update(product.Model);

            _db.SaveChanges();
        }

        public void DeleteProduct(ProductWrapper product)
        {
            product.Deleted = 1;
            _db.SaveChanges();
        }

        public void RestoreProduct(ProductWrapper product)
        {
            product.Deleted = 0;
            _db.SaveChanges();
        }

        public void UpdateInventory(int productId, int quantityDelta, int totalDelta)
        {
            var product = _db.Products.FirstOrDefault(p => p.Id == productId);
            if (product == null) return;

            product.Quantity += quantityDelta;
            product.Total += totalDelta;
            _db.SaveChanges();
        }

        public ObservableCollection<ProductWrapper> GetAllProducts(bool deleted = false)
        {
            var productsList = _db.Products
                                  .Where(p => p.Deleted == (deleted ? 1 : 0))
                                  .ToList();

            return new ObservableCollection<ProductWrapper>(
                productsList.Select(p => new ProductWrapper(p))
            );
        }
    }
}
