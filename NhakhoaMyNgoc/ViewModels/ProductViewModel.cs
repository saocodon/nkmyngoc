using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NhakhoaMyNgoc.Interfaces;
using NhakhoaMyNgoc.Models;
using NhakhoaMyNgoc.ModelWrappers;
using NhakhoaMyNgoc.Services;
using System.Collections.ObjectModel;
using static NhakhoaMyNgoc.ViewModels.AppViewModel;

namespace NhakhoaMyNgoc.ViewModels
{
    public partial class ProductViewModel : ObservableObject
    {
        public static string Title => "Hàng hoá";

        [ObservableProperty]
        private ViewMode mode;

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
        public void StartAddNew()
        {
            if (Products != null)
            {
                SelectedProduct = new()
                {
                    Deleted = 0,
                    Name = "Chưa rõ",
                    Unit = "cái",
                    Quantity = 0,
                    Total = 0
                };
                Products.Add(SelectedProduct);
            }
        }

        [RelayCommand]
        public void Save()
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
        public void Delete()
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
