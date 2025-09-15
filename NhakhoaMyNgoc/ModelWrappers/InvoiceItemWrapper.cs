using CommunityToolkit.Mvvm.ComponentModel;
using NhakhoaMyNgoc.Models;
using System.Diagnostics;

namespace NhakhoaMyNgoc.ModelWrappers
{
    public class InvoiceItemWrapper(InvoiceItem item) : ObservableObject
    {
        public InvoiceItem Model { get; } = item;

        public InvoiceItemWrapper() : this(new InvoiceItem()) { }

        public List<Service> Services { get; set; } = [];

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

        public long Price
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

        public long Discount
        {
            get => Model.Discount;
            set
            {
                if (Model.Discount != value)
                {
                    Model.Discount = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Total));
                }
            }
        }

        public long Total => Quantity * Price - Discount;

        public long Id => Model.Id;
        public long InvoiceId { get => Model.InvoiceId; set => Model.InvoiceId = value; }
        public long ServiceId
        {
            get => Model.ServiceId;
            set
            {
                // SetProperty không hỗ trợ POCO (Plain Old CLR Object) class nên phải viết bằng tay
                if (Model.ServiceId != value)
                {
                    Model.ServiceId = value;
                    OnPropertyChanged();
                    OnServiceIdChanged(value);
                }
            }
        }

        private void OnServiceIdChanged(long serviceId)
        {
            var matched = Services.FirstOrDefault(s => s.Id == serviceId);
            if (matched is not null)
            {
                Price = matched.Price;
                OnPropertyChanged(nameof(Total));
            }
        }

        public Service Service
        {
            get => Model.Service;
            set => Model.Service = value;
        }
    }
}