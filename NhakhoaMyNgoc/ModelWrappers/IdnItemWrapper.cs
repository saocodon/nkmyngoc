using CommunityToolkit.Mvvm.ComponentModel;
using NhakhoaMyNgoc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NhakhoaMyNgoc.ModelWrappers
{
    public class IdnItemWrapper(Idnitem item) : ObservableObject
    {
        public Idnitem Model { get; } = item;

        public IdnItemWrapper() : this(new Idnitem()) { }

        public List<Product> Products { get; set; } = [];

        public int ItemId { get => Model.ItemId; set => Model.ItemId = value; }

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
