using NhakhoaMyNgoc.ModelWrappers;
using System.Collections.ObjectModel;

namespace NhakhoaMyNgoc.Interfaces
{
    public interface IProductService
    {
        ObservableCollection<ProductWrapper> GetAllProducts();
        void SaveProduct(ProductWrapper product);
        void DeleteProduct(ProductWrapper product);
        void RestoreProduct(ProductWrapper product);
        void UpdateInventory(int productId, int quantityDelta, int totalDelta);
        ObservableCollection<ProductWrapper> GetAllProducts(bool deleted = false);
    }
}