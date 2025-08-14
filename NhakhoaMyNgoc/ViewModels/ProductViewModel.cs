using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NhakhoaMyNgoc.Interfaces;
using NhakhoaMyNgoc.Models;
using NhakhoaMyNgoc.ModelWrappers;
using NhakhoaMyNgoc.Services;
using System.Collections.ObjectModel;

namespace NhakhoaMyNgoc.ViewModels
{
    public partial class ProductViewModel : ObservableObject
    {
        public static string Title => "Hàng hoá";

        public enum ProductViewMode
        {
            Manage,
            Restore
        }

        [ObservableProperty]
        private ProductViewMode mode;

        private readonly IProductService _productService;

        [ObservableProperty]
        private ObservableCollection<ProductWrapper>? products;

        [ObservableProperty]
        private ProductWrapper? selectedProduct = new();

        public ProductViewModel(IProductService productService, bool loadDeleted = false)
        {
            _productService = productService;
            Products = _productService.GetAllProducts(deleted: loadDeleted);
        }

        [RelayCommand]
        public void StartAddNew() => SelectedProduct = new();

        [RelayCommand]
        public void SaveProduct()
        {
            if (SelectedProduct != null && Products != null)
            {
                _productService.SaveProduct(SelectedProduct);

                if (!Products.Contains(SelectedProduct))
                    Products.Add(SelectedProduct);

                SelectedProduct = new();
            }
        }

        [RelayCommand]
        public void DeleteProduct()
        {
            if (SelectedProduct != null && Products != null)
            {
                _productService.DeleteProduct(SelectedProduct);
                Products.Remove(SelectedProduct);
                SelectedProduct = new();
            }
        }

        [RelayCommand]
        public void Restore()
        {
            if (SelectedProduct != null && Products != null)
            {
                _productService.RestoreProduct(SelectedProduct);
                Products.Remove(SelectedProduct);
                SelectedProduct = new();
            }
        }
    }
}
