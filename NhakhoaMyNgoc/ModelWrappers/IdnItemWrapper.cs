using CommunityToolkit.Mvvm.ComponentModel;
using NhakhoaMyNgoc.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NhakhoaMyNgoc.ModelWrappers
{
    public class IdnItemWrapper(Idnitem item) : ObservableObject
    {
        public Idnitem Model { get; } = item;

        public IdnItemWrapper() : this(new Idnitem()) { }

        public ObservableCollection<ProductWrapper> Products { get; set; } = [];

        private int _previousItemId = -1;

        public int ItemId
        {
            get => Model.ItemId;
            set
            {
                if (Model.ItemId != value)
                {
                    // Lưu lại ID cũ để biết chuyển từ đâu
                    int oldItemId = Model.ItemId;
                    Model.ItemId = value;
                    OnPropertyChanged();

                    // Chuyển số lượng sang item mới (nếu Products đã có)
                    if (Products is not null && Products.Count > 0)
                    {
                        var oldProduct = Products.FirstOrDefault(p => p.Id == oldItemId);
                        var newProduct = Products.FirstOrDefault(p => p.Id == value);

                        if (oldProduct != null)
                        {
                            oldProduct.Quantity = oldProduct.Quantity - Quantity;
                            oldProduct.Total = oldProduct.Total - Quantity * Price;
                        }

                        if (newProduct != null)
                        {
                            newProduct.Quantity = newProduct.Quantity + Quantity;
                            newProduct.Total = newProduct.Total + Quantity * Price;
                        }
                    }

                    OnPropertyChanged(nameof(Item));
                }
            }
        }

        public int Quantity
        {
            get => Model.Quantity;
            set
            {
                if (Model.Quantity != value)
                {
                    Model.Quantity = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Total));
                }
            }
        }

        public int Demand
        {
            get => Model.Demand;
            set
            {
                if (Model.Demand != value)
                {
                    Model.Demand = value;
                    OnPropertyChanged();
                }
            }
        }

        public int Total => Quantity * Price;

        public int Price
        {
            get => Model.Price;
            set
            {
                if (Model.Price != value)
                {
                    Model.Price = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Total));
                }
            }
        }

        public int Id => Model.Id;
        public int IdnId
        {
            get => Model.IdnId;
            set
            {
                if (Model.IdnId != value)
                {
                    Model.IdnId = value;
                    OnPropertyChanged();
                }
            }
        }

        public Product Item { get => Model.Item; set => Model.Item = value; }
        public Idn Idn { get => Model.Idn; set => Model.Idn = value; }
    }
}
