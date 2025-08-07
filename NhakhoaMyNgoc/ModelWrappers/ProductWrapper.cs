using CommunityToolkit.Mvvm.ComponentModel;
using NhakhoaMyNgoc.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NhakhoaMyNgoc.ModelWrappers
{
    public class ProductWrapper : ObservableObject
    {
        public Product Model { get; }

        private readonly ObservableCollection<IdnItemWrapper> _idnItems;

        public ProductWrapper(Product product, ObservableCollection<IdnItemWrapper> idnItems)
        {
            Model = product;
            _idnItems = idnItems;

            // Theo dõi khi danh sách IdnItems thay đổi
            _idnItems.CollectionChanged += _idnItems_CollectionChanged;

            // Theo dõi từng item thay đổi Quantity hoặc Price
            foreach (var item in _idnItems)
                item.PropertyChanged += OnItemChanged;
        }

        private void _idnItems_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems is not null)
            {
                foreach (IdnItemWrapper item in e.NewItems)
                    item.PropertyChanged += OnItemChanged;
            }

            if (e.OldItems is not null)
            {
                foreach (IdnItemWrapper item in e.OldItems)
                    item.PropertyChanged -= OnItemChanged;
            }
        }

        private void OnItemChanged(object? sender, PropertyChangedEventArgs e)
        {
            // I don't even know, ChatGPT did this.
            // Leaving this blank to avoid getting even more errors.
        }

        public int Id => Model.Id;

        public string Name => Model.Name;

        public string Unit => Model.Unit ?? string.Empty;

        public int Quantity
        {
            get => Model.Quantity;
            set
            {
                if (Model.Quantity != value)
                {
                    Model.Quantity = value;
                    OnPropertyChanged();
                }
            }
        }

        public int Total
        {
            get => Model.Total;
            set
            {
                if (Model.Total != value)
                {
                    Model.Total = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
