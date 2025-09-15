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

        private readonly ObservableCollection<IdnItemWrapper> idnItems;

        public ProductWrapper(Product product, ObservableCollection<IdnItemWrapper> _idnItems)
        {
            Model = product;
            idnItems = _idnItems;

            // Theo dõi khi danh sách IdnItems thay đổi
            idnItems.CollectionChanged += IdnItems_CollectionChanged;

            // Theo dõi từng item thay đổi Quantity hoặc Price
            foreach (var item in idnItems)
                item.PropertyChanged += OnItemChanged;
        }

        public ProductWrapper()
        {
            Model = new();
            idnItems = [];
        }

        public ProductWrapper(Product p)
        {
            Model = p;
            idnItems = [];
        }

        private void IdnItems_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
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

        public long Id => Model.Id;
        public bool Deleted { get => Model.Deleted; set => Model.Deleted = value; }

        public string Name
        {
            get => Model.Name;
            set
            {
                if (Model.Name != value)
                {
                    Model.Name = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Unit
        {
            get => Model.Unit ?? string.Empty;
            set
            {
                if (Model.Unit != value)
                {
                    Model.Unit = value;
                    OnPropertyChanged();
                }
            }
        }

        public long Price { get => Model.Price; set => Model.Price = value; }

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

        public long Total
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
